using System.Globalization;
using System.Text.Json;
using EnergyTwin.Web.Presentation;

namespace EnergyTwin.Web.Live;

public sealed record LiveMeasurement(string Key, string Name, double? Value, string Unit, string Address, string Text);

public sealed record DeviceLive(
    DateTimeOffset ReceivedAt, long Generation, string SimTime, string RunId, int Seq,
    string Quality, string QualityReason, string State, IReadOnlyList<LiveMeasurement> Values);

public sealed record WeatherLive(string Location, DateTimeOffset ReceivedAt, long Generation, string Mode,
    string Condition, string SimTime, string RunId, int Seq, IReadOnlyList<LiveMeasurement> Values);

/// <summary>Hilfsmethoden zum Überführen gültiger Live-Nachrichten in Ansichtsdaten je Modulart.</summary>
public static class LiveView
{
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");

    public static DeviceLive? Device(LiveMessage? measurements, LiveMessage? status)
    {
        if (measurements is null) return null;
        var data = measurements.Data;
        if (!data.TryGetProperty("measurements", out var values) || values.ValueKind != JsonValueKind.Array) return null;
        var state = status is null ? "unbekannt" : Text(status.Data, "state");
        var list = new List<LiveMeasurement>();
        foreach (var entry in values.EnumerateArray())
        {
            if (entry.ValueKind != JsonValueKind.Object || !entry.TryGetProperty("key", out var key)) continue;
            double? number = entry.TryGetProperty("value", out var value) && value.ValueKind == JsonValueKind.Number ? value.GetDouble() : null;
            list.Add(new(Text(entry, "key"), Text(entry, "name"), number, Text(entry, "unit"),
                entry.TryGetProperty("address", out var address) && address.ValueKind == JsonValueKind.Number ? address.GetInt32().ToString(German) : "—", Text(entry, "text")));
        }
        return new(measurements.ReceivedAt, measurements.Generation, Text(data, "simTime"), Text(data, "runId"),
            Num(data, "seq"), Text(data, "quality"), Text(data, "qualityReason"), state, list.AsReadOnly());
    }

    public static WeatherLive? Weather(LiveMessage? measurements)
    {
        if (measurements is null) return null;
        var data = measurements.Data;
        if (!data.TryGetProperty("measurements", out var values) || values.ValueKind != JsonValueKind.Array) return null;
        var list = new List<LiveMeasurement>();
        foreach (var entry in values.EnumerateArray())
        {
            if (entry.ValueKind != JsonValueKind.Object || !entry.TryGetProperty("key", out var key)) continue;
            double? number = entry.TryGetProperty("value", out var value) && value.ValueKind == JsonValueKind.Number ? value.GetDouble() : null;
            list.Add(new(Text(entry, "key"), Text(entry, "name"), number, Text(entry, "unit"), "—", Text(entry, "text")));
        }
        return new(Text(data, "location"), measurements.ReceivedAt, measurements.Generation, Text(data, "mode"),
            Text(data, "condition"), Text(data, "simTimeUtc"), Text(data, "runId"), Num(data, "seq"), list.AsReadOnly());
    }

    public static LiveMeasurement? Find(DeviceLive? device, string key) =>
        device?.Values.FirstOrDefault(value => value.Key == key);

    public static double? Measurement(IReadOnlyList<LiveMeasurement> values, string key) =>
        values.FirstOrDefault(value => value.Key == key)?.Value;

    /// <summary>Kennzahlen der Übersicht je Modulart; Werte aus der aktuellsten Gerätenachricht.</summary>
    public static Metric[] DeviceMetrics(ModuleKind kind, DeviceLive? device)
    {
        if (device is null) return [];
        Metric Live(string key, string label, string unit, string tone, string description) =>
            new(label, Format(Find(device, key)?.Value), unit, tone, description);
        return kind switch
        {
            ModuleKind.Pv =>
            [
                Live("wirkleistung", "Wirkleistung", "kW", "green", "Negativ: Einspeisung"),
                Live("einstrahlung", "Einstrahlung", "W/m²", "gold", "Wetter am Standort"),
                Live("modultemperatur", "Modultemperatur", "°C", "orange", "Live")
            ],
            ModuleKind.Battery =>
            [
                Live("ladezustand_soc", "Ladezustand", "%", "green", "SOC"),
                Live("wirkleistung", "Wirkleistung", "kW", "blue", "Positiv: Laden · Negativ: Entladen"),
                Live("kapazitaet", "Kapazität", "kWh", "green", "Live")
            ],
            ModuleKind.Charger =>
            [
                Live("wirkleistung", "Gesamtleistung", "kW", "blue", "Beide Anschlüsse"),
                Live("punkt_1_leistung", "Leistung Anschluss 1", "kW", "blue", "Live"),
                Live("punkt_2_leistung", "Leistung Anschluss 2", "kW", "blue", "Live")
            ],
            ModuleKind.ColdStore =>
            [
                Live("raumtemperatur", "Innentemperatur", "°C", "blue", "Live"),
                Live("solltemperatur", "Solltemperatur", "°C", "orange", "Sollwert"),
                Live("kaelteleistung", "Kälteleistung", "kW", "purple", "Live")
            ],
            _ => []
        };
    }

    /// <summary>Verlaufsreihen je Modulart: Messwert-Schlüssel, Farbklassen und Beschriftung für den Chart.</summary>
    public static (string Key, string Label, string Class)[] ChartSeries(ModuleKind kind) => kind switch
    {
        ModuleKind.Pv => [("wirkleistung", "Wirkleistung", "chart-line")],
        ModuleKind.Battery => [("wirkleistung", "Lade-/Entladeleistung", "chart-line")],
        ModuleKind.ColdStore => [("raumtemperatur", "Ist-Temperatur", "chart-line"), ("solltemperatur", "Solltemperatur", "chart-target")],
        _ => []
    };

    public static string Unit(ModuleKind kind) => kind is ModuleKind.ColdStore ? "°C" : "kW";
    public static string ChartTitle(ModuleKind kind) => kind switch
    {
        ModuleKind.Battery => "Lade- und Entladeleistung",
        ModuleKind.ColdStore => "Ist- und Solltemperatur",
        _ => "PV-Leistungsverlauf"
    };
    public static string ChartDescription(ModuleKind kind) => kind switch
    {
        ModuleKind.Battery => "Positiv: Laden · Negativ: Entladen.",
        ModuleKind.ColdStore => "Durchgezogen: Ist · gestrichelt: Soll.",
        _ => "Negative Wirkleistung bedeutet Einspeisung."
    };

    public static string Format(double? value) => value is null ? "—" : string.Create(German, $"{value:0.#}");

    public static string FormatTime(DateTimeOffset value) => value.ToString("HH:mm:ss", German);

    public static string ShortTime(string simTime)
    {
        var index = simTime.IndexOf('T');
        return index < 0 ? simTime : simTime[(index + 1)..];
    }

    private static string Text(JsonElement data, string key) => data.TryGetProperty(key, out var value) ? value.ToString() : "—";
    private static int Num(JsonElement data, string key) => data.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.Number ? value.GetInt32() : 0;
}
