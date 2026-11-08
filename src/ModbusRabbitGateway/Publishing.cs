namespace ModbusRabbitGateway;

/// <summary>Ziel der Nachrichten. Heute die Konsole (Probebetrieb ohne Broker), im nächsten Schritt MQTT.</summary>
public interface IMessagePublisher
{
    /// <summary>Kurzer Name für die Anzeige, z. B. „Konsole“ oder „MQTT 127.0.0.1:1883“.</summary>
    string Description { get; }

    /// <param name="retain">Bei MQTT: der Broker hebt die letzte Nachricht für neue Abonnenten auf (Status-Topics).</param>
    Task PublishAsync(string topic, string payload, bool retain, CancellationToken cancellationToken);
}

/// <summary>Probebetrieb ohne Broker: schreibt Topic und JSON auf die Konsole (nur mit <c>--json</c>, sonst still).</summary>
public sealed class ConsolePublisher(TextWriter output, bool showPayload) : IMessagePublisher
{
    public string Description => showPayload ? "Konsole (JSON)" : "Konsole (nur Übersicht, --json zeigt die Nachrichten)";

    public async Task PublishAsync(string topic, string payload, bool retain, CancellationToken cancellationToken)
    {
        if (!showPayload)
            return;

        await output.WriteLineAsync($"→ {topic}{(retain ? " (retained)" : "")}".AsMemory(), cancellationToken);
        await output.WriteLineAsync(payload.AsMemory(), cancellationToken);
    }
}
