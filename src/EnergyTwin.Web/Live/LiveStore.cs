using System.Text.Json;

namespace EnergyTwin.Web.Live;

public sealed record LiveMessage(string Topic, JsonElement Data, DateTimeOffset ReceivedAt, long Generation);

/// <summary>Begrenzter, browserunabhängiger Speicher der letzten Nachrichten.</summary>
public sealed class LiveStore
{
    private readonly object _gate = new();
    private readonly Dictionary<string, LiveMessage> _messages = new(StringComparer.Ordinal);
    private long _generation;
    private bool _connected;
    private string _connection = "Verbindung wird aufgebaut";
    private int _rejected;

    /// <summary>Callback für gültige, angenommene Nachrichten; außerhalb des Sperre aufrufen.</summary>
    public Action<string, JsonElement>? OnValidMessage { get; set; }

    public (bool Connected, string Connection, long Generation, int Rejected, LiveMessage[] Messages) Snapshot()
    {
        lock (_gate) return (_connected, _connection, _generation, _rejected, _messages.Values.OrderBy(m => m.Topic).ToArray());
    }

    public void SetConnection(bool connected, string description)
    {
        lock (_gate)
        {
            if (connected && !_connected) _generation++;
            _connected = connected;
            _connection = description;
        }
    }

    public void Receive(string topic, string payload)
    {
        lock (_gate)
        {
            if (payload.Length == 0) { _messages.Remove(topic); return; }
            try
            {
                if (payload.Length > 262144) throw new JsonException();
                using var document = JsonDocument.Parse(payload, new JsonDocumentOptions { MaxDepth = 16 });
                var data = document.RootElement;
                var parts = topic.Split('/');
                if (parts.Length < 3 || parts[0] != "energytwin" ||
                    !data.TryGetProperty("schemaVersion", out var version) || version.GetInt32() != 1)
                    throw new JsonException();
                if (parts[1] == "devices" && (parts.Length != 4 || Text(data, "deviceId") != parts[2])) throw new JsonException();
                if (parts[^1] == "measurements" && (!data.TryGetProperty("measurements", out var values) || values.ValueKind != JsonValueKind.Array)) throw new JsonException();
                if (parts[^1] == "measurements")
                {
                    foreach (var value in data.GetProperty("measurements").EnumerateArray())
                    {
                        if (value.ValueKind != JsonValueKind.Object || !value.TryGetProperty("key", out var key) || key.ValueKind != JsonValueKind.String ||
                            !value.TryGetProperty("value", out var number) || number.ValueKind is not (JsonValueKind.Number or JsonValueKind.Null)) throw new JsonException();
                    }
                    if (parts[1] == "devices" && new[] { "simTime", "runId", "seq", "quality", "qualityReason" }.Any(key => !data.TryGetProperty(key, out _))) throw new JsonException();
                }
                if (!_messages.ContainsKey(topic) && _messages.Count >= 512) throw new JsonException();
                _messages[topic] = new(topic, data.Clone(), DateTimeOffset.UtcNow, _generation);
                OnValidMessage?.Invoke(topic, data);
            }
            catch (Exception ex) when (ex is JsonException or InvalidOperationException or FormatException or OverflowException)
            {
                _rejected++;
            }
        }
    }

    public static string Text(JsonElement data, string key) => data.TryGetProperty(key, out var value) ? value.ToString() : "—";
}

