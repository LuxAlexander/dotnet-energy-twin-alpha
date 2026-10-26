namespace Verbraucher.Simulation;

/// <summary>
/// Automatikbetrieb: öffnet und schließt die Kühlhaustür nach Tageszeit (Lieferverkehr, Schichtwechsel).
/// Die Rate ist eine Wahrscheinlichkeit je Stunde, umgerechnet auf die Schrittweite; die Tür bleibt danach
/// 1–10 Minuten offen. Alles mit dem übergebenen <see cref="Random"/>, damit Läufe mit <c>--seed</c> wiederholbar sind.
/// </summary>
public sealed class AutoModeController
{
    private readonly Kuehlhaus _room;
    private readonly Random _random;
    private DateTime? _doorClosesAt;

    public AutoModeController(Kuehlhaus room, Random random)
    {
        _room = room;
        _random = random;
    }

    /// <summary>Erwartete Türöffnungen je Stunde nach Tageszeit.</summary>
    public static double DoorRatePerHour(int hour) => hour switch
    {
        < 6 => 0.1,     // Nacht: fast niemand
        < 9 => 1.0,     // Anlieferung am Morgen
        < 12 => 1.5,    // Vormittag: meiste Bewegung
        < 18 => 0.8,    // Nachmittag
        _ => 0.4        // Abend
    };

    /// <summary>Wahrscheinlichkeit mindestens einer Öffnung im Zeitraum <paramref name="dt"/>.</summary>
    public static double DoorProbability(int hour, TimeSpan dt) =>
        1 - Math.Exp(-DoorRatePerHour(hour) * dt.TotalHours);

    /// <summary>Ein Modellschritt: offene Tür zuerst schließen, sonst vielleicht öffnen.</summary>
    public void Step(DateTime now, TimeSpan dt)
    {
        if (_doorClosesAt is { } closes)
        {
            if (now >= closes)
            {
                _room.SetDoor(false);
                _doorClosesAt = null;
            }

            return;                                  // eine offene Tür wird nicht durch eine neue Öffnung verlängert
        }

        if (_random.NextDouble() < DoorProbability(now.Hour, dt))
        {
            _room.SetDoor(true);
            _doorClosesAt = now + TimeSpan.FromMinutes(1 + _random.NextDouble() * 9);
        }
    }

    /// <summary>Virtuelle Zeit, zu der die offene Tür automatisch schließt; <c>null</c> = Tür zu.</summary>
    public DateTime? DoorClosesAt => _doorClosesAt;
}
