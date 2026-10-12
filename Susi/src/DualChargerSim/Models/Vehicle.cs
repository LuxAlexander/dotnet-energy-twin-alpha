namespace DualChargerSim.Models;

/// <summary>
/// Elektrofahrzeug mit Batterie (Kapazität, SOC, Ziel-SOC) und modellabhängiger Ladekurve.
/// </summary>
public class Vehicle
{
    /// <summary>SOC in %, ab dem die Ladeleistung exponentiell abfällt.</summary>
    public const double CurveKneeSocPercent = 80.0;

    /// <summary>Abfallkonstante k = ln(10)/20: Bei 100 % SOC bleiben 10 % der Maximalleistung.</summary>
    public static readonly double CurveDecayK = Math.Log(10) / 20.0;

    public string ModelName { get; }
    public double BatteryCapacityKWh { get; }
    public double MaxChargePowerKW { get; }
    public double SocPercent { get; private set; }
    public double TargetSocPercent { get; }

    /// <summary>
    /// Defekte Ladebuchse: Temperatursensor ausgefallen (kein Derating) und erhöhter Übergangswiderstand.
    /// </summary>
    public bool HasDamagedInlet { get; }

    /// <summary>Ziel-SOC erreicht → Ladepunkt wechselt nach <c>Finished</c>.</summary>
    public bool IsFull => SocPercent >= TargetSocPercent;

    public Vehicle(string modelName, double batteryCapacityKWh, double maxChargePowerKW,
                   double socPercent, double targetSocPercent, bool hasDamagedInlet = false)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentException("Modellname darf nicht leer sein.", nameof(modelName));
        if (batteryCapacityKWh <= 0)
            throw new ArgumentOutOfRangeException(nameof(batteryCapacityKWh), "Kapazität muss > 0 sein.");
        if (maxChargePowerKW <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxChargePowerKW), "Maximale Ladeleistung muss > 0 sein.");
        if (socPercent is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(socPercent), "SOC muss zwischen 0 und 100 % liegen.");
        if (targetSocPercent is <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(targetSocPercent), "Ziel-SOC muss zwischen 0 und 100 % liegen.");

        ModelName = modelName;
        BatteryCapacityKWh = batteryCapacityKWh;
        MaxChargePowerKW = maxChargePowerKW;
        SocPercent = socPercent;
        TargetSocPercent = targetSocPercent;
        HasDamagedInlet = hasDamagedInlet;
    }

    /// <summary>
    /// Ladekurve: bis 80 % SOC volle Leistung, darüber P = Pmax · e^(−k · (SOC − 80)).
    /// </summary>
    public double GetMaxPowerAtSoc()
    {
        if (SocPercent <= CurveKneeSocPercent)
            return MaxChargePowerKW;

        return MaxChargePowerKW * Math.Exp(-CurveDecayK * (SocPercent - CurveKneeSocPercent));
    }

    /// <summary>
    /// Speichert Energie in der Batterie (bereits nach Wirkungsgrad, also E_akku).
    /// Begrenzt auf den Ziel-SOC und liefert die tatsächlich aufgenommene Energie zurück,
    /// damit der Ladepunkt daraus E_netz = E_akku / η berechnen kann.
    /// </summary>
    public double AddEnergy(double batteryEnergyKWh)
    {
        if (batteryEnergyKWh <= 0 || IsFull)
            return 0;

        double maxEnergyKWh = (TargetSocPercent - SocPercent) * BatteryCapacityKWh / 100.0;
        double acceptedKWh = Math.Min(batteryEnergyKWh, maxEnergyKWh);

        SocPercent += acceptedKWh / BatteryCapacityKWh * 100.0;
        if (acceptedKWh == maxEnergyKWh)
            SocPercent = TargetSocPercent; // Rundungsfehler vermeiden, damit IsFull sicher greift

        return acceptedKWh;
    }
}
