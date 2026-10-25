using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimCore.Contracts;

/// <summary>Kennwerte und (De-)Serialisierung des Zeitsynchronisations-Protokolls.</summary>
public static class SyncProtocol
{
    /// <summary>Protokollversion; der Master lehnt Simulatoren mit anderer Version ab.</summary>
    public const int Version = 1;

    /// <summary>Standard-TCP-Port des Simulations-Masters.</summary>
    public const int DefaultPort = 5100;

    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>Eine Nachricht als einzeilige JSON-Zeichenkette.</summary>
    public static string Serialize(SyncMessage message) => JsonSerializer.Serialize(message, Options);

    /// <exception cref="InvalidDataException">Keine gültige Nachricht (kein JSON, unbekannter Typ, fehlende Felder).</exception>
    public static SyncMessage Deserialize(string line)
    {
        SyncMessage? message;
        try
        {
            message = JsonSerializer.Deserialize<SyncMessage>(line, Options);
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            throw new InvalidDataException($"Ungültige Nachricht: {ex.Message}", ex);
        }

        return message switch
        {
            null => throw new InvalidDataException("Leere Nachricht."),
            HelloMessage { ClientId: null or "" } => throw new InvalidDataException("hello ohne clientId."),
            StatesMessage { States: null } => throw new InvalidDataException("states ohne Liste."),
            StatesMessage s when s.States.Any(IsInvalid) => throw new InvalidDataException("states mit ungültigem Zeitzustand."),
            _ => message
        };
    }

    private static bool IsInvalid(TimeState? state) =>
        state is null || !double.IsFinite(state.Speed) || state.Speed <= 0 || !Enum.IsDefined(state.Mode);
}
