namespace WeatherSimulator.Simulation;

/// <summary>Ein vom Wettersimulator unterstützter Standort (Kennung plus Geometrie).</summary>
/// <param name="Key">Kleingeschriebene, eindeutige Kennung für die HTTP-Abfrage.</param>
/// <param name="Name">Anzeigename.</param>
/// <param name="LatitudeDegrees">Geographische Breite in Grad.</param>
/// <param name="LongitudeDegrees">Geographische Länge in Grad.</param>
/// <param name="AltitudeMeters">Höhe über dem Meeresspiegel in Metern.</param>
public sealed record WeatherLocation(
    string Key,
    string Name,
    double LatitudeDegrees,
    double LongitudeDegrees,
    double AltitudeMeters);

/// <summary>Feste Standorttabelle. Unbekannte Kennungen werden abgelehnt (HTTP 404).</summary>
public static class LocationCatalog
{
    public const string DefaultKey = "salzburg";

    private static readonly WeatherLocation[] Locations =
    [
        new("salzburg", "Salzburg", 47.8095, 13.0550, 424.0),
        new("wien", "Wien", 48.2082, 16.3738, 171.0),
        new("innsbruck", "Innsbruck", 47.2692, 11.4041, 574.0)
    ];

    public static IReadOnlyList<WeatherLocation> All => Locations;

    /// <summary>Sucht einen Standort; <paramref name="key"/> wird ohne Rücksicht auf Groß-/Kleinschreibung verglichen.</summary>
    public static bool TryGet(string? key, out WeatherLocation location)
    {
        location = Locations[0];
        if (string.IsNullOrWhiteSpace(key))
            return false;

        foreach (var candidate in Locations)
        {
            if (string.Equals(candidate.Key, key.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                location = candidate;
                return true;
            }
        }

        return false;
    }
}
