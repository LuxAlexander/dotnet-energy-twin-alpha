using System.Text.Json;
using System.Text.Json.Serialization;
using SimCore.Contracts;
using WeatherSimulator.Simulation;

namespace WeatherSimulator.Publishing;

public sealed record WeatherMeasurement(string Key, double Value, string Unit);

/// <summary>Eigener Wettervertrag; Zeitstempel UTC, Wetterstunde und aktuelle Master-Zeit getrennt.</summary>
public sealed record WeatherMessage(int SchemaVersion, string SourceSession, long Seq, string MessageId,
    string Location, int RunId, DateTime ModelTimeUtc, DateTime SimTimeUtc, DateTimeOffset PublishedAtUtc,
    string Mode, string Condition, bool IsDaylight, IReadOnlyList<WeatherMeasurement> Measurements, Guid? MasterSession);

public sealed record WeatherStatus(int SchemaVersion, string SourceSession, long Seq, string MessageId,
    string Status, int? RunId, DateTime? SimTimeUtc, DateTimeOffset PublishedAtUtc, bool IsLastWill = false, Guid? MasterSession = null);

public static class WeatherMessages
{
    public const string StatusTopic = "energytwin/weather/status";
    public static string Topic(string location) => $"energytwin/weather/{location}/measurements";
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    { Converters = { new JsonStringEnumConverter() } };

    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Json);

    public static WeatherMessage Create(WeatherData data, DateTime simTime, ManualWeatherState manual,
        string session, long sequence, DateTimeOffset published, Guid? masterSession = null) => new(1, session, sequence, $"{session}:{sequence}",
        data.Location, data.RunId, data.SimTimeUtc, DateTime.SpecifyKind(simTime, DateTimeKind.Utc), published,
        manual.IsActive ? "manual" : "automatic", data.Condition.ToString(), data.SunHeightDegrees > 0,
        [new("temperature_c", data.TemperatureC, "°C"), new("irradiation_w_m2", data.IrradiationWm2, "W/m²"),
         new("wind_speed_m_s", data.WindSpeedMs, "m/s"), new("precipitation_mm_h", data.PrecipitationMmPerHour, "mm/h"),
         new("cloud_cover_percent", data.CloudCoverPercent, "%"), new("sun_height_degrees", data.SunHeightDegrees, "°")], masterSession);
}
