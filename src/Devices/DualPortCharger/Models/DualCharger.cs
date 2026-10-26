namespace DualChargerSim.Models;

/// <summary>
/// DC-Ladesäule mit zwei Ladepunkten und gemeinsamem Leistungsbudget; teilt die Leistung auf und verwaltet die Warteschlange.
/// </summary>
public class DualCharger
{
    public const double MinPowerLimitKW = 50;
    public const double MaxPowerLimitKW = 350;
    public static readonly TimeSpan MaxQueueWait = TimeSpan.FromMinutes(5);

    private readonly Queue<(Vehicle Vehicle, DateTime ArrivedAt)> _queue = new();
    private double _powerLimitKW;

    public string Id { get; }
    public bool IsOn { get; private set; } = true;
    public IReadOnlyList<ChargingPoint> Points { get; }
    public int RejectedVehicles { get; private set; }
    public int QueueLength => _queue.Count;

    /// <summary>
    /// Gemeinsames Leistungsbudget; zur Laufzeit änderbar (Lastmanagement).
    /// Wird auf 50–350 kW begrenzt, Werte außerhalb werden auf die Grenze gesetzt.
    /// </summary>
    public double PowerLimitKW
    {
        get => _powerLimitKW;
        set => _powerLimitKW = Math.Clamp(value, MinPowerLimitKW, MaxPowerLimitKW);
    }

    /// <summary>Aussentemperatur beider Ladepunkte (kommt mit Wetterbezug vom Wettersimulator).</summary>
    public double AmbientTemperatureC
    {
        set
        {
            foreach (var point in Points)
                point.AmbientTemperatureC = value;
        }
    }

    public double TotalPowerKW => Points.Sum(p => p.PowerKW);
    public double TotalEnergyKWh => Points.Sum(p => p.EnergyKWh);

    public DualCharger(string id, double powerLimitKW)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Geräte-ID darf nicht leer sein.", nameof(id));

        if (powerLimitKW is < MinPowerLimitKW or > MaxPowerLimitKW)
            throw new ArgumentOutOfRangeException(nameof(powerLimitKW),
                $"Gesamtleistung muss zwischen {MinPowerLimitKW} und {MaxPowerLimitKW} kW liegen.");

        Id = id;
        PowerLimitKW = powerLimitKW;
        Points = [new ChargingPoint(1), new ChargingPoint(2)];
    }

    public void SwitchOn() => IsOn = true;
    public void SwitchOff() => IsOn = false;

    /// <summary>Ankommendes Fahrzeug; wird sofort an einen freien Punkt gesteckt, sonst wartet es.</summary>
    public void Enqueue(Vehicle vehicle, DateTime now)
    {
        _queue.Enqueue((vehicle, now));
        ConnectWaitingVehicles();
    }

    /// <summary>
    /// Ein Simulationsschritt: Warteschlange abarbeiten, Bedarf beider Punkte ermitteln,
    /// Budget aufteilen, dann beide Punkte aktualisieren.
    /// </summary>
    public void Tick(DateTime now, TimeSpan dt)
    {
        ProcessQueue(now);

        double demandA = IsOn ? Points[0].GetDemandKW() : 0;
        double demandB = IsOn ? Points[1].GetDemandKW() : 0;
        var (a, b) = AllocatePower(PowerLimitKW, demandA, demandB);

        Points[0].Update(a, dt);
        Points[1].Update(b, dt);
    }

    /// <summary>
    /// Teilt das Budget auf: zuerst bis zur Hälfte, ungenutzte Leistung geht an den anderen Punkt.
    /// Beispiel 150 kW, Bedarf 15 + 150 → 15 + 135 (nicht 75 + 75).
    /// </summary>
    public static (double A, double B) AllocatePower(double budgetKW, double demandA, double demandB)
    {
        if (demandA + demandB <= budgetKW)
            return (demandA, demandB);

        double a = Math.Min(demandA, budgetKW / 2);
        double b = Math.Min(demandB, budgetKW - a);
        a = Math.Min(demandA, budgetKW - b);
        return (a, b);
    }

    private void ProcessQueue(DateTime now)
    {
        ConnectWaitingVehicles();

        // Wer zu lange gewartet hat, fährt weg
        while (_queue.Count > 0 && now - _queue.Peek().ArrivedAt >= MaxQueueWait)
        {
            _queue.Dequeue();
            RejectedVehicles++;
        }
    }

    private void ConnectWaitingVehicles()
    {
        while (_queue.Count > 0)
        {
            var freePoint = Points.FirstOrDefault(p => p.IsFree);
            if (freePoint is null)
                break;

            var (vehicle, _) = _queue.Dequeue();
            freePoint.Connect(vehicle);
            freePoint.Start();
        }
    }
}
