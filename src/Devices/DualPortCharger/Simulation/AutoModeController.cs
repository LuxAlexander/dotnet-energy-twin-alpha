using DualChargerSim.Models;

namespace DualChargerSim.Simulation;

/// <summary>
/// Auto-Modus: zufällige Ankünfte (Rate pro Stunde), Start-/Ziel-SOC und Verweilzeiten.
/// </summary>
public class AutoModeController
{
    private readonly DualCharger _charger;
    private readonly Random _random;
    private readonly Dictionary<ChargingPoint, DateTime> _departures = new();

    public AutoModeController(DualCharger charger, Random random)
    {
        _charger = charger;
        _random = random;
    }

    /// <summary>Ankünfte pro Stunde abhängig von der Tageszeit.</summary>
    public static double GetArrivalRatePerHour(int hour) => hour switch
    {
        < 6 => 0.2,
        < 9 => 1.5,
        < 16 => 1.0,
        < 20 => 2.5,
        _ => 0.8,
    };

    /// <summary>
    /// Wahrscheinlichkeit für eine Ankunft im Schritt: p = 1 − e^(−Rate · Δt).
    /// Unabhängig von der Schrittweite, anders als eine feste Wahrscheinlichkeit pro Schritt.
    /// </summary>
    public static double GetArrivalProbability(int hour, TimeSpan dt) =>
        1 - Math.Exp(-GetArrivalRatePerHour(hour) * dt.TotalHours);

    public void Step(DateTime now, TimeSpan dt)
    {
        HandleDepartures(now);

        if (_random.NextDouble() < GetArrivalProbability(now.Hour, dt))
            _charger.Enqueue(VehicleCatalog.CreateRandom(_random), now);
    }

    /// <summary>Volle Fahrzeuge fahren nach einer zufälligen Verweilzeit ab.</summary>
    private void HandleDepartures(DateTime now)
    {
        foreach (var point in _charger.Points)
        {
            if (point.State != ChargingPointState.Finished)
            {
                _departures.Remove(point);
                continue;
            }

            if (!_departures.TryGetValue(point, out var departure))
            {
                departure = now + RandomDwellTime();
                _departures[point] = departure;
            }

            if (now >= departure)
            {
                point.Disconnect();
                _departures.Remove(point);
            }
        }
    }

    /// <summary>80 %: 0–5 min, 20 %: 10–40 min (blockiert den Punkt).</summary>
    private TimeSpan RandomDwellTime() =>
        _random.NextDouble() < 0.8
            ? TimeSpan.FromMinutes(_random.NextDouble() * 5)
            : TimeSpan.FromMinutes(10 + _random.NextDouble() * 30);
}
