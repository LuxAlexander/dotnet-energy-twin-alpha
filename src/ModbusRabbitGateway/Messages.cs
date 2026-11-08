using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModbusRabbitGateway;

/// <summary>Messwertnachricht nach <c>docs/Nachrichtenvertrag-LB03.md</c> §3 (eine je gültiger Lesung).</summary>
/// <param name="SimTime">Virtuelle Zeit des gelesenen Zustands, ISO 8601 ohne Zeitzone.</param>
/// <param name="PolledAtUtc">Reale Abfragezeit des Gateways (UTC). Nie als Messzeitpunkt verwenden.</param>
public sealed record MeasurementMessage(
    int SchemaVersion,
    string DeviceId,
    string DeviceType,
    string SimTime,
    long SimTimeEpoch,
    int RunId,
    int Seq,
    long PublishSeq,
    string PolledAtUtc,
    string Quality,
    string QualityReason,
    IReadOnlyList<Measurement> Measurements);

/// <param name="Key">Stabiler Schlüssel (<see cref="MeasurementKeys"/>), z. B. für InfluxDB.</param>
/// <param name="Name">Name aus dem Registerplan.</param>
/// <param name="Value"><c>null</c>, wenn das Register keinen endlichen Wert enthält.</param>
/// <param name="Text">Klartext bei Aufzählungen (Zustände), sonst nicht vorhanden.</param>
public sealed record Measurement(
    string Key,
    string Name,
    int Address,
    double? Value,
    string Unit,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Text = null);

/// <summary>Statusnachricht je Gerät (§4), retained, nur bei Änderung.</summary>
public sealed record DeviceStatusMessage(
    int SchemaVersion,
    string DeviceId,
    string State,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Error,
    string SinceUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? LastSimTime,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? LastRunId,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? LastSeq);

/// <summary>Status des Gateways selbst (<c>online</c>/<c>offline</c>), retained; <c>offline</c> auch als Last Will.</summary>
public sealed record GatewayStatusMessage(int SchemaVersion, string GatewayId, string State, string SinceUtc);

/// <summary>Werte und Namen, die der Vertrag festlegt.</summary>
public static class Contract
{
    public const int SchemaVersion = 1;
    public const string TopicRoot = "energytwin";

    public static string MeasurementsTopic(string deviceId) => $"{TopicRoot}/devices/{deviceId}/measurements";
    public static string DeviceStatusTopic(string deviceId) => $"{TopicRoot}/devices/{deviceId}/status";
    public static string GatewayStatusTopic(string gatewayId) => $"{TopicRoot}/gateway/{gatewayId}/status";

    public static class Quality
    {
        public const string Good = "good";
        public const string Uncertain = "uncertain";
        public const string Bad = "bad";
    }

    public static class Reason
    {
        public const string Running = "running";
        public const string Paused = "paused";
        public const string Standalone = "standalone";
        public const string Stale = "stale";
        public const string InvalidValue = "invalidValue";
        public const string MasterLost = "masterLost";
        public const string NoTime = "noTime";
    }

    public static class DeviceState
    {
        public const string Online = "online";
        public const string Offline = "offline";
        public const string DecodeError = "decodeError";
    }

    /// <summary>Reale Zeit in UTC mit Millisekunden und <c>Z</c>.</summary>
    public static string FormatUtc(DateTimeOffset time) =>
        time.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Virtuelle Zeit ohne Zeitzone, Sekundenauflösung.</summary>
    public static string FormatSimTime(DateTime simTime) =>
        simTime.ToString("yyyy-MM-dd'T'HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
}

/// <summary>JSON nach Vertrag: camelCase, UTF-8 ohne Umschreibung von Umlauten und °, kompakt.</summary>
public static class MessageJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static readonly JsonSerializerOptions Indented = new(Options) { WriteIndented = true };

    public static string Serialize<T>(T message, bool indented = false) =>
        JsonSerializer.Serialize(message, indented ? Indented : Options);
}
