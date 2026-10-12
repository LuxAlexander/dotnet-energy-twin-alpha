namespace DualChargerSim.Models;

/// <summary>
/// Einzelner Ladepunkt: Zustand, verbundenes Fahrzeug, Leistung, Temperatur und kumulierte Energie.
/// </summary>
public class ChargingPoint
{
    // Temperaturmodell (Kabel/Stecker) – plausible Annahmen, keine Normwerte
    public const double AmbientTemperatureC = 20.0;
    public const double TemperatureRiseAtRatedPowerC = 35.0;   // 20 + 35 = 55 °C bei 150 kW
    public const double RatedPowerKW = 150.0;
    public const double DeratingStartC = 60.0;
    public const double FaultTemperatureC = 80.0;
    public const double ManualResetTemperatureC = 60.0;   // Techniker darf quittieren, sobald kein Derating mehr nötig ist
    public const double AutoResetTemperatureC = 50.0;     // automatisch erst mit mehr Abstand (Hysterese)
    public const double CoolingDefectTemperatureC = 85.0;
    public static readonly TimeSpan ThermalTimeConstant = TimeSpan.FromMinutes(10);
    public static readonly TimeSpan CoolingDefectTimeConstant = TimeSpan.FromMinutes(30);
    public const double DamagedInletHeatFactor = 2.0;     // doppelte Erwärmung durch Übergangswiderstand

    public const double DefaultEfficiency = 0.95;

    private bool _wasChargingBeforeFault;
    private bool _hasCoolingDefect;

    public int Number { get; }
    public double Efficiency { get; }
    public ChargingPointState State { get; private set; } = ChargingPointState.Available;
    public Vehicle? Vehicle { get; private set; }
    public double PowerKW { get; private set; }
    public double TemperatureC { get; private set; } = AmbientTemperatureC;
    public double EnergyKWh { get; private set; }

    public bool IsFree => State == ChargingPointState.Available;

    /// <summary>
    /// Leistungsfaktor durch Temperatur-Derating: bis 60 °C 100 %, linear auf 0 % bei 80 °C.
    /// Bei defekter Ladebuchse fehlt der Messwert, es wird nicht reduziert – nur die Abschaltung bei 80 °C schützt.
    /// </summary>
    public double DeratingFactor =>
        TemperatureC <= DeratingStartC || Vehicle is { HasDamagedInlet: true }
            ? 1.0
            : Math.Clamp((FaultTemperatureC - TemperatureC) / (FaultTemperatureC - DeratingStartC), 0.0, 1.0);

    public ChargingPoint(int number, double efficiency = DefaultEfficiency)
    {
        if (efficiency is <= 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(efficiency), "Wirkungsgrad muss zwischen 0 und 1 liegen.");

        Number = number;
        Efficiency = efficiency;
    }

    public bool Connect(Vehicle vehicle)
    {
        if (State != ChargingPointState.Available)
            return false;

        Vehicle = vehicle;
        State = ChargingPointState.Connected;
        return true;
    }

    public bool Start()
    {
        if (State != ChargingPointState.Connected || Vehicle is null)
            return false;

        State = Vehicle.IsFull ? ChargingPointState.Finished : ChargingPointState.Charging;
        return true;
    }

    public bool Stop()
    {
        if (State != ChargingPointState.Charging)
            return false;

        State = ChargingPointState.Connected;
        PowerKW = 0;
        return true;
    }

    /// <summary>Abstecken. Bei <c>Faulted</c> nicht möglich, weil das Kabel verriegelt bleibt.</summary>
    public bool Disconnect()
    {
        if (State is not (ChargingPointState.Connected or ChargingPointState.Charging or ChargingPointState.Finished))
            return false;

        Vehicle = null;
        State = ChargingPointState.Available;
        PowerKW = 0;
        return true;
    }

    /// <summary>
    /// Manuelle Störung: Kühlungsdefekt. Temperatur springt auf 85 °C und sinkt nur langsam (τ = 30 min).
    /// </summary>
    public void TriggerFault()
    {
        TemperatureC = Math.Max(TemperatureC, CoolingDefectTemperatureC);
        _hasCoolingDefect = true;
        EnterFault();
    }

    /// <summary>
    /// Manueller Reset unter 60 °C. Unter 50 °C hebt sich die Störung im nächsten Schritt von selbst auf.
    /// Ein unterbrochener Ladevorgang läuft weiter.
    /// </summary>
    public bool Reset() => TryReset(ManualResetTemperatureC);

    private bool TryReset(double maxTemperatureC)
    {
        if (State != ChargingPointState.Faulted || TemperatureC >= maxTemperatureC)
            return false;

        _hasCoolingDefect = false;

        if (Vehicle is null)
            State = ChargingPointState.Available;
        else if (Vehicle.IsFull)
            State = ChargingPointState.Finished;
        else
            State = _wasChargingBeforeFault ? ChargingPointState.Charging : ChargingPointState.Connected;

        return true;
    }

    /// <summary>Leistungsbedarf = Min(Ladekurve, Temperatur-Derating). Nur im Zustand <c>Charging</c>.</summary>
    public double GetDemandKW()
    {
        if (State != ChargingPointState.Charging || Vehicle is null)
            return 0;

        return Vehicle.GetMaxPowerAtSoc() * DeratingFactor;
    }

    /// <summary>
    /// Ein Simulationsschritt mit der von der Säule zugeteilten Leistung:
    /// Energie, SOC und Temperatur aktualisieren.
    /// </summary>
    public void Update(double allocatedKW, TimeSpan dt)
    {
        double hours = dt.TotalHours;
        double power = Math.Clamp(allocatedKW, 0, GetDemandKW());
        double gridEnergyKWh = 0;

        if (power > 0 && Vehicle is not null)
        {
            // E_akku = E_netz · η, begrenzt auf den Ziel-SOC; E_netz daraus zurückrechnen
            double batteryEnergyKWh = Vehicle.AddEnergy(power * hours * Efficiency);
            gridEnergyKWh = batteryEnergyKWh / Efficiency;
            EnergyKWh += gridEnergyKWh;

            if (Vehicle.IsFull)
                State = ChargingPointState.Finished;
        }

        // Mittlere Leistung im Schritt, damit Leistung · Δt immer der gezählten Energie entspricht
        PowerKW = hours > 0 ? gridEnergyKWh / hours : 0;

        UpdateTemperature(PowerKW, dt);
        if (State == ChargingPointState.Faulted)
            TryReset(AutoResetTemperatureC);
        else if (TemperatureC >= FaultTemperatureC)
            EnterFault();
    }

    /// <summary>
    /// Exakte Lösung der Erwärmung 1. Ordnung (stabil für jede Schrittweite, kein Euler-Überschwingen).
    /// Verluste ∝ I² ∝ P². Bei Kühlungsdefekt größere Zeitkonstante (langsameres Abkühlen),
    /// bei defekter Ladebuchse doppelte Erwärmung.
    /// </summary>
    private void UpdateTemperature(double powerKW, TimeSpan dt)
    {
        double ratio = powerKW / RatedPowerKW;
        double heatFactor = Vehicle is { HasDamagedInlet: true } ? DamagedInletHeatFactor : 1.0;
        double targetC = AmbientTemperatureC + TemperatureRiseAtRatedPowerC * heatFactor * ratio * ratio;
        var tau = _hasCoolingDefect ? CoolingDefectTimeConstant : ThermalTimeConstant;
        TemperatureC = targetC + (TemperatureC - targetC) * Math.Exp(-dt / tau);
    }

    private void EnterFault()
    {
        if (State == ChargingPointState.Faulted)
            return;

        _wasChargingBeforeFault = State == ChargingPointState.Charging;
        State = ChargingPointState.Faulted;
        PowerKW = 0;
    }
}
