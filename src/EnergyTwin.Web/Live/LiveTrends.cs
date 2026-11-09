using System.Globalization;
using System.Text.Json;

namespace EnergyTwin.Web.Live;

public sealed record TrendPoint(string RunId, long SimTimeEpoch, string SimTime, int Seq, double Value);

/// <summary>
/// Begrenzter Verlaufsspeicher im Webdienst, browserunabhängig: je Topic und Messwert-Schlüssel
/// ein Ringpuffer ohne dauerhafte Speicherung. Ein Wechsel der Laufkennung beginnt eine neue
/// Kurve; Punkte ohne numerischen Wert werden nicht übernommen, damit ungültige Daten nicht als
/// ununterbrochene gültige Kurve erscheinen.
/// </summary>
public sealed class LiveTrends(int maxPoints)
{
    private readonly object _gate = new();
    private readonly Dictionary<string, Dictionary<string, List<TrendPoint>>> _series = new(StringComparer.Ordinal);

    public void Record(string topic, JsonElement data)
    {
        try
        {
            var parts = topic.Split('/');
            if (parts.Length != 4 || parts[3] != "measurements") return;
            var isDevice = parts[1] == "devices";
            var isWeather = parts[1] == "weather";
            if (!isDevice && !isWeather) return;
            if (!data.TryGetProperty("measurements", out var values) || values.ValueKind != JsonValueKind.Array) return;
            var runId = data.TryGetProperty("runId", out var run) && run.ValueKind is JsonValueKind.String or JsonValueKind.Number ? run.ToString() : "";
            if (runId.Length == 0) return;
            var epoch = isDevice && data.TryGetProperty("simTimeEpoch", out var deviceEpoch) && deviceEpoch.ValueKind == JsonValueKind.Number
                ? deviceEpoch.GetInt64()
                : TryEpoch(data.TryGetProperty("simTimeUtc", out var weather) ? weather.GetString() : null);
            if (epoch is null) return;
            var simTime = isDevice ? data.TryGetProperty("simTime", out var deviceTime) ? deviceTime.GetString() ?? "" : ""
                : data.TryGetProperty("simTimeUtc", out var weatherTime) ? weatherTime.GetString() ?? "" : "";
            var seq = data.TryGetProperty("seq", out var seqElement) && seqElement.ValueKind == JsonValueKind.Number ? seqElement.GetInt32() : 0;
            lock (_gate)
            {
                foreach (var entry in values.EnumerateArray())
                {
                    if (entry.ValueKind != JsonValueKind.Object || !entry.TryGetProperty("key", out var keyElement) ||
                        keyElement.ValueKind != JsonValueKind.String ||
                        !entry.TryGetProperty("value", out var value) || value.ValueKind != JsonValueKind.Number) continue;
                    Append(topic, keyElement.GetString()!, new TrendPoint(runId, epoch.Value, simTime, seq, value.GetDouble()));
                }
            }
        }
        catch (Exception)
        {
            // Verlauf bleibt bewusst ohne Verlaufspunkt; ungültige Nachrichten wertet LiveStore separat.
        }
    }

    /// <summary>Aktuelle Kurve als Kopie; leer, wenn keine Punkte vorliegen.</summary>
    public IReadOnlyList<TrendPoint> Series(string topic, string key)
    {
        lock (_gate) return _series.TryGetValue(topic, out var byKey) && byKey.TryGetValue(key, out var points)
            ? points.ToArray() : Array.Empty<TrendPoint>();
    }

    public (string RunId, int Count) SeriesInfo(string topic, string key)
    {
        lock (_gate) return _series.TryGetValue(topic, out var byKey) && byKey.TryGetValue(key, out var points) && points.Count > 0
            ? (points[^1].RunId, points.Count) : ("", 0);
    }

    private void Append(string topic, string key, TrendPoint point)
    {
        if (!_series.TryGetValue(topic, out var byKey))
            byKey = _series[topic] = new Dictionary<string, List<TrendPoint>>(StringComparer.Ordinal);
        if (!byKey.TryGetValue(key, out var points))
            points = byKey[key] = new List<TrendPoint>();
        if (points.Count > 0 && points[^1].RunId != point.RunId) points.Clear();
        if (points.Count > 0 && points[^1].SimTimeEpoch > point.SimTimeEpoch) return;
        if (points.Count > 0 && points[^1].SimTimeEpoch == point.SimTimeEpoch)
        {
            if (points[^1].Seq == point.Seq) return;
            points[^1] = point;
        }
        else points.Add(point);
        var excess = points.Count - maxPoints;
        if (excess > 0) points.RemoveRange(0, excess);
    }

    private static long? TryEpoch(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        return DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            ? parsed.ToUnixTimeSeconds() : null;
    }
}
