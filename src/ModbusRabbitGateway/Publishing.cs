namespace ModbusRabbitGateway;

/// <summary>Ziel der Nachrichten: MQTT (RabbitMQ) oder im Probebetrieb die Konsole.</summary>
public interface IMessagePublisher
{
    /// <summary>Kurzer Name für die Anzeige, z. B. „Konsole“ oder „MQTT 127.0.0.1:1883“.</summary>
    string Description { get; }

    /// <param name="retain">Bei MQTT: der Broker hebt die letzte Nachricht für neue Abonnenten auf (Status-Topics).</param>
    /// <exception cref="BrokerUnavailableException">Nachricht konnte nicht übertragen werden.</exception>
    Task PublishAsync(string topic, string payload, bool retain, CancellationToken cancellationToken);
}

/// <summary>Broker nicht erreichbar oder Übertragung fehlgeschlagen; die Meldung ist für die Anzeige gedacht.</summary>
public sealed class BrokerUnavailableException(string message, Exception? inner = null) : Exception(message, inner);

/// <summary>Probebetrieb ohne Broker oder Zusatzausgabe (<c>--json</c>): schreibt Topic und JSON auf die Konsole.</summary>
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

/// <summary>Schickt jede Nachricht an mehrere Ziele, z. B. MQTT und zusätzlich die Konsole (<c>--json</c>).</summary>
public sealed class TeePublisher(IMessagePublisher primary, IMessagePublisher secondary) : IMessagePublisher
{
    public string Description => $"{primary.Description} + {secondary.Description}";

    public async Task PublishAsync(string topic, string payload, bool retain, CancellationToken cancellationToken)
    {
        await secondary.PublishAsync(topic, payload, retain, cancellationToken);   // Anzeige auch bei Broker-Ausfall
        await primary.PublishAsync(topic, payload, retain, cancellationToken);
    }
}
