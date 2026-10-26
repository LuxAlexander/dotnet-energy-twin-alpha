namespace Devices.Common.Weather;

/// <summary>
/// Gemeinsame Startoptionen für den Wetterbezug (<c>--weather</c>, <c>--location</c>).
/// Mit Master ist der Wettersimulator standardmäßig unter <see cref="DefaultUrl"/> eingetragen, standalone nicht.
/// </summary>
public sealed record WeatherOptions
{
    public const string DefaultUrl = "http://127.0.0.1:8080";
    public const string DefaultLocation = "salzburg";

    public const string Usage = """
          --weather <url|off>   Wettersimulator (Standard mit Master: http://127.0.0.1:8080; standalone: aus)
          --location <ort>      Standort für die Wetterabfrage (salzburg, wien, innsbruck; Standard: salzburg)
        """;

    /// <summary>Basis-URL des Wettersimulators; <c>null</c> = kein Wetterbezug (feste Werte).</summary>
    public string? Url { get; init; }

    /// <summary>Standortkennung für die Wetterabfrage.</summary>
    public string Location { get; init; } = DefaultLocation;

    /// <summary>Wurde <c>--weather</c> ausdrücklich angegeben (auch <c>off</c>)?</summary>
    public bool Specified { get; init; }

    /// <summary>Setzt die Standard-URL, wenn kein <c>--weather</c> angegeben wurde und nicht standalone gelaufen wird.</summary>
    public WeatherOptions WithDefaultUrl(bool standalone) =>
        !Specified && !standalone ? this with { Url = DefaultUrl } : this;

    /// <summary>Verarbeitet eine Wetter-Option, falls <paramref name="name"/> eine ist.</summary>
    /// <param name="nextValue">Liefert den Wert hinter der Option (wirft, wenn er fehlt).</param>
    /// <returns><c>false</c>, wenn die Option nicht zum Wetter gehört.</returns>
    /// <exception cref="ArgumentException">Ungültiger Wert.</exception>
    public static bool TryApply(string name, Func<string> nextValue, WeatherOptions current, out WeatherOptions updated)
    {
        switch (name.ToLowerInvariant())
        {
            case "--weather":
                string value = nextValue().Trim();
                if (value.Length == 0)
                    throw new ArgumentException("--weather erwartet eine URL oder 'off'.");
                updated = current with
                {
                    Specified = true,
                    Url = IsOff(value) ? null : value
                };
                return true;

            case "--location":
                string location = nextValue().Trim().ToLowerInvariant();
                if (location.Length == 0)
                    throw new ArgumentException("--location darf nicht leer sein.");
                updated = current with { Location = location };
                return true;

            default:
                updated = current;
                return false;
        }
    }

    private static bool IsOff(string value) =>
        value.Equals("off", StringComparison.OrdinalIgnoreCase)
        || value.Equals("none", StringComparison.OrdinalIgnoreCase)
        || value.Equals("lokal", StringComparison.OrdinalIgnoreCase);
}
