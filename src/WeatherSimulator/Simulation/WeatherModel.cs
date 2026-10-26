using System.Collections.Concurrent;
using System.Globalization;
using SimCore.Contracts;
using WeatherSynth;

namespace WeatherSimulator.Simulation;

/// <summary>
/// Deterministisches Wettermodell. Grundlage sind die stündlichen Strahlungs- und Windwerte von
/// WeatherSynth (offline, DWD-Daten, CC BY 4.0); Temperatur, Bedeckung, Niederschlag, Sonnenhöhe
/// und Zustand werden daraus bzw. aus Standort und Zeit berechnet. Gleicher Standort, gleiche
/// virtuelle Zeit und gleicher Lauf ergeben stets dasselbe Ergebnis.
/// </summary>
public sealed class WeatherModel
{
    private readonly ConcurrentDictionary<(string Location, int Run, int Year), CoupledWeatherYear> _years = new();
    private readonly ManualWeather _manual;

    public WeatherModel(ManualWeather manual) => _manual = manual;

    public ManualWeather Manual => _manual;

    /// <summary>Wetter für (Standortkennung, virtuelle Zeit, Lauf); Zeiten sind UTC.</summary>
    /// <exception cref="KeyNotFoundException">Unbekannte Standortkennung.</exception>
    public WeatherData Get(string locationKey, DateTime simTimeUtc, int runId)
    {
        if (!LocationCatalog.TryGet(locationKey, out var location))
            throw new KeyNotFoundException($"Unbekannter Standort: '{locationKey}'.");

        var utc = DateTime.SpecifyKind(simTimeUtc, DateTimeKind.Utc);
        var hourStart = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, 0, 0, DateTimeKind.Utc);
        var year = GetYear(location, runId, hourStart.Year);
        int index = IndexOfHour(year, hourStart);
        var hour = year.Hours[index];

        double clearSky = hour.Solar.ClearSkyWhPerM2;
        double irradiation = hour.Solar.GhiWhPerM2;
        double wind = hour.Wind.Speed;
        double sunHeight = SunHeightDegrees(hourStart, location.LatitudeDegrees, location.LongitudeDegrees);
        if (sunHeight <= 0)
            irradiation = 0;

        double cloud = CloudCover(year, index);
        double temperature = TemperatureC(location, runId, hourStart, cloud);
        double precipitation = PrecipitationMmPerHour(location, runId, hourStart, cloud, wind);

        // Manuelle Übersteuerung (Toggle) wirkt auf alle Standorte.
        if (_manual.IsActive)
        {
            double usedCloud = _manual.CloudCoverPercent ?? cloud;
            if (_manual.CloudCoverPercent is not null && sunHeight > 0)
                irradiation = Math.Max(0, clearSky * (1.0 - 0.8 * usedCloud / 100.0));

            cloud = usedCloud;
            temperature = _manual.TemperatureC ?? temperature;
            precipitation = _manual.PrecipitationMmPerHour ?? precipitation;
            wind = _manual.WindSpeedMs ?? wind;
        }

        var condition = ConditionOf(sunHeight, irradiation, cloud, wind, precipitation, temperature);

        return new WeatherData(
            location.Key,
            hourStart,
            runId,
            Math.Round(sunHeight, 2),
            Math.Round(cloud, 1),
            Math.Round(temperature, 1),
            Math.Round(precipitation, 2),
            Math.Round(wind, 2),
            Math.Round(irradiation, 1),
            condition);
    }

    private CoupledWeatherYear GetYear(WeatherLocation location, int runId, int calendarYear) =>
        _years.GetOrAdd((location.Key, runId, calendarYear), key =>
        {
            var solar = new SolarSite(location.LatitudeDegrees, location.LongitudeDegrees, location.AltitudeMeters, TimeZoneInfo.Utc);
            var windSite = new WindSite(10.0, 0.1);
            int seed = unchecked(runId * 7919 + (int)StableHash($"{key.Location}|{key.Year}") + key.Year);
            return SyntheticWeather.Default.GenerateYear(key.Year, seed, solar, windSite, WindProfile.LogLaw);
        });

    /// <summary>Index der Stunde in der Jahresliste (binäre Suche auf dem Stundenbeginn).</summary>
    private static int IndexOfHour(CoupledWeatherYear year, DateTime hourStartUtc)
    {
        var hours = year.Hours;
        int low = 0;
        int high = hours.Count - 1;
        while (low <= high)
        {
            int middle = (low + high) / 2;
            DateTime start = hours[middle].Start.UtcDateTime;
            if (start == hourStartUtc)
                return middle;

            if (start < hourStartUtc)
                low = middle + 1;
            else
                high = middle - 1;
        }

        return Math.Clamp(low, 0, hours.Count - 1);
    }

    /// <summary>Bedeckungsgrad aus dem Verhältnis von Globalstrahlung zu Klarhimmel-Strahlung.</summary>
    private static double CloudCover(CoupledWeatherYear year, int index)
    {
        var hour = year.Hours[index];
        if (hour.Solar.ClearSkyWhPerM2 > 0.1)
            return Math.Clamp((1.0 - hour.Solar.GhiWhPerM2 / hour.Solar.ClearSkyWhPerM2) * 100.0, 0, 100);

        // Nachts: Bedeckung des letzten Tageslichts übernehmen (deterministisch, nicht zufällig).
        for (int i = index - 1; i >= 0 && i > index - 48; i--)
        {
            var previous = year.Hours[i];
            if (previous.Solar.ClearSkyWhPerM2 > 0.1)
                return Math.Clamp((1.0 - previous.Solar.GhiWhPerM2 / previous.Solar.ClearSkyWhPerM2) * 100.0, 0, 100);
        }

        return 40.0;
    }

    /// <summary>
    /// Temperatur aus Jahresmittel (Höhenlage), Jahresgang, Tagesgang und einem pro Tag und Standort
    /// geseedeten Wärme-/Kälte-Offset. Bewölkung dämpft den Tagesgang.
    /// </summary>
    private static double TemperatureC(WeatherLocation location, int runId, DateTime utc, double cloud)
    {
        double annualMean = 10.0 - 0.0065 * (location.AltitudeMeters - 200.0);
        double seasonal = -9.5 * Math.Cos(2.0 * Math.PI * (utc.DayOfYear - 15) / 365.0);
        double dailyOffset = (Rand01(location.Key, runId, $"{utc:yyyyMMdd}", "temp") - 0.5) * 6.0;
        double amplitude = 5.5 * (1.0 - 0.3 * cloud / 100.0);
        double hour = utc.Hour + utc.Minute / 60.0;
        double diurnal = amplitude * Math.Cos(2.0 * Math.PI * (hour - 15.0) / 24.0);
        return annualMean + seasonal + dailyOffset + diurnal;
    }

    /// <summary>Niederschlagswahrscheinlichkeit aus Bedeckung, Menge aus einem geseedeten Stundenwert.</summary>
    private static double PrecipitationMmPerHour(WeatherLocation location, int runId, DateTime utc, double cloud, double wind)
    {
        double wetness = Math.Clamp((cloud - 55.0) / 45.0, 0, 1);
        double probability = wetness * 0.25;
        if (Rand01(location.Key, runId, $"{utc:yyyyMMddHH}", "rain") >= probability)
            return 0.0;

        double amount = 0.2 + 3.0 * Rand01(location.Key, runId, $"{utc:yyyyMMddHH}", "amount");
        if (cloud >= 95 && wind >= 10)
            amount *= 2.0;

        return amount;
    }

    private static WeatherCondition ConditionOf(
        double sunHeight, double irradiation, double cloud, double wind, double precipitation, double temperature)
    {
        if (sunHeight <= 0 && irradiation <= 0)
            return WeatherCondition.Night;

        if (precipitation > 0 && temperature <= 1.5)
            return WeatherCondition.Snow;

        if (cloud >= 90 && wind >= 14)
            return WeatherCondition.Storm;

        if (cloud >= 80)
            return WeatherCondition.Overcast;

        if (cloud >= 30)
            return WeatherCondition.PartlyCloudy;

        return WeatherCondition.Sunny;
    }

    /// <summary>Sonnenhöhe über dem Horizont in Grad (negative Werte = unter dem Horizont).</summary>
    public static double SunHeightDegrees(DateTime utc, double latitudeDegrees, double longitudeDegrees)
    {
        double declination = 23.44 * Math.Sin(2.0 * Math.PI * (utc.DayOfYear - 81) / 365.0) * Math.PI / 180.0;
        double solarHours = utc.Hour + utc.Minute / 60.0 + longitudeDegrees / 15.0;
        double hourAngle = (solarHours - 12.0) * 15.0 * Math.PI / 180.0;
        double latitude = latitudeDegrees * Math.PI / 180.0;
        double sinAltitude = Math.Sin(latitude) * Math.Sin(declination)
            + Math.Cos(latitude) * Math.Cos(declination) * Math.Cos(hourAngle);

        return Math.Asin(Math.Clamp(sinAltitude, -1.0, 1.0)) * 180.0 / Math.PI;
    }

    /// <summary>Stabiler, prozessunabhängiger Zufallswert in [0, 1) aus Text (FNV-1a).</summary>
    public static double Rand01(string location, int runId, string timeKey, string purpose)
    {
        uint hash = StableHash(string.Create(CultureInfo.InvariantCulture, $"{location}|{runId}|{timeKey}|{purpose}"));
        return hash / 4294967296.0;
    }

    private static uint StableHash(string text)
    {
        const uint offset = 2166136261;
        const uint prime = 16777619;
        uint hash = offset;
        foreach (char c in text)
        {
            hash ^= c;
            hash *= prime;
        }

        return hash;
    }
}
