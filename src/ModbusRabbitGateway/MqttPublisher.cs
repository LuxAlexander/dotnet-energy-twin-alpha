using System.Text;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Protocol;

namespace ModbusRabbitGateway;

/// <summary>Verbindungsdaten zum RabbitMQ-MQTT-Plugin.</summary>
public sealed record BrokerSettings(string Host, int Port, string ClientId, MqttCredentials Credentials)
{
    public const int DefaultPort = 1883;

    public string Endpoint => $"{Host}:{Port}";
}

/// <summary>
/// Veröffentlicht per MQTT (MQTTnet) an RabbitMQ. Verhalten bei Broker-Ausfall (Nachrichtenvertrag §6):
/// <list type="bullet">
/// <item>Keine Endlosschleife und kein Abbruch: Ist der Broker weg, wirft <see cref="PublishAsync"/> sofort
/// <see cref="BrokerUnavailableException"/>; der Aufrufer liest die Geräte weiter.</item>
/// <item>Wiederanlauf mit wachsendem Abstand (1, 2, 4 … höchstens 30 s), ausgelöst durch den nächsten Veröffentlichungsversuch.</item>
/// <item>Messwerte aus der Ausfallzeit werden <b>verworfen</b> (Momentaufnahmen, kein Puffer); die letzten Status-Nachrichten
/// (retained) werden nach dem Wiederverbinden erneut gesendet, damit der Broker den aktuellen Stand kennt.</item>
/// <item>Last Will: bricht die Verbindung ohne geordnetes Beenden ab, setzt der Broker den Gateway-Status auf <c>offline</c>.</item>
/// </list>
/// </summary>
public sealed class MqttPublisher : IMessagePublisher, IAsyncDisposable
{
    private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan MaxBackoff = TimeSpan.FromSeconds(30);

    private readonly BrokerSettings _settings;
    private readonly string _willTopic;
    private readonly TimeProvider _time;
    private readonly IMqttClient _client = new MqttClientFactory().CreateMqttClient();
    private readonly Dictionary<string, string> _retained = new(StringComparer.Ordinal);
    private TimeSpan _backoff = TimeSpan.FromSeconds(1);
    private DateTimeOffset _nextAttempt = DateTimeOffset.MinValue;
    private string? _lastError;

    public MqttPublisher(BrokerSettings settings, string gatewayId, TimeProvider time)
    {
        _settings = settings;
        _willTopic = Contract.GatewayStatusTopic(gatewayId);
        _time = time;
        GatewayId = gatewayId;
    }

    public string GatewayId { get; }

    public string Description => $"MQTT {_settings.Endpoint} ({_settings.Credentials})";

    public bool IsConnected => _client.IsConnected;

    public async Task PublishAsync(string topic, string payload, bool retain, CancellationToken cancellationToken)
    {
        if (retain)
            _retained[topic] = payload;                           // auch bei Ausfall merken: wird nach dem Wiederverbinden nachgeholt

        await EnsureConnectedAsync(topic, cancellationToken);
        try
        {
            await SendAsync(topic, payload, retain, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            ScheduleRetry();
            throw new BrokerUnavailableException($"Senden an {_settings.Endpoint} fehlgeschlagen: {ex.Message}", ex);
        }
    }

    private async Task EnsureConnectedAsync(string currentTopic, CancellationToken cancellationToken)
    {
        if (_client.IsConnected)
            return;

        var now = _time.GetUtcNow();
        if (now < _nextAttempt)
            throw new BrokerUnavailableException(
                $"Broker {_settings.Endpoint} nicht erreichbar ({_lastError}); nächster Versuch in {(_nextAttempt - now).TotalSeconds:0} s.");

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(ConnectTimeout);
        MqttClientConnectResult result;
        try
        {
            result = await _client.ConnectAsync(BuildOptions(now), timeout.Token);
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            _lastError = ex switch
            {
                OperationCanceledException => $"keine Antwort innerhalb {ConnectTimeout.TotalSeconds:0} s",
                // RabbitMQ lehnt falsche Zugangsdaten ab, indem es die Verbindung schließt. Genauso sieht es aus, wenn der
                // Container gerade (neu) startet: die Docker-Portweiterleitung nimmt an und schließt sofort. Beides nennen.
                MqttConnectingFailedException failed when IsAuthenticationProblem(failed)
                    => $"Verbindung bei der Anmeldung geschlossen – startet der Broker gerade, oder sind die Zugangsdaten falsch? ({_settings.Credentials})",
                _ => ex.Message,
            };
            ScheduleRetry();
            throw new BrokerUnavailableException($"Keine Verbindung zu Broker {_settings.Endpoint}: {_lastError}", ex);
        }

        // MQTTnet 5 wirft bei Ablehnung (z. B. falsches Passwort) nicht, sondern meldet den Grund im Ergebnis.
        if (result.ResultCode != MqttClientConnectResultCode.Success)
        {
            _lastError = result.ResultCode is MqttClientConnectResultCode.BadUserNameOrPassword or MqttClientConnectResultCode.NotAuthorized
                ? $"Anmeldung abgelehnt ({result.ResultCode}) – Zugangsdaten prüfen: {_settings.Credentials}"
                : $"Verbindung abgelehnt ({result.ResultCode})";
            ScheduleRetry();
            throw new BrokerUnavailableException($"Keine Verbindung zu Broker {_settings.Endpoint}: {_lastError}");
        }

        _backoff = TimeSpan.FromSeconds(1);
        _lastError = null;

        // Status-Nachrichten aus der Ausfallzeit nachholen (die aktuelle Nachricht sendet der Aufrufer gleich selbst)
        try
        {
            foreach (var (topic, payload) in _retained.Where(r => r.Key != currentTopic).ToList())
                await SendAsync(topic, payload, retain: true, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            ScheduleRetry();
            throw new BrokerUnavailableException($"Senden an {_settings.Endpoint} fehlgeschlagen: {ex.Message}", ex);
        }
    }

    /// <summary>MQTTnet 5.0.1 legt den Ablehnungscode nicht in der Ausnahme offen; erkennbar ist er nur an der Meldung.</summary>
    private static bool IsAuthenticationProblem(MqttConnectingFailedException ex) =>
        ex.Message.Contains("authenticat", StringComparison.OrdinalIgnoreCase);

    private void ScheduleRetry()
    {
        _nextAttempt = _time.GetUtcNow() + _backoff;
        _backoff = TimeSpan.FromTicks(Math.Min(_backoff.Ticks * 2, MaxBackoff.Ticks));
    }

    private MqttClientOptions BuildOptions(DateTimeOffset now)
    {
        var will = new GatewayStatusMessage(Contract.SchemaVersion, GatewayId, "offline", Contract.FormatUtc(now));
        var builder = new MqttClientOptionsBuilder()
            .WithTcpServer(_settings.Host, _settings.Port)
            .WithClientId(_settings.ClientId)
            .WithCleanSession()
            .WithTimeout(ConnectTimeout)
            .WithWillTopic(_willTopic)
            .WithWillPayload(Encoding.UTF8.GetBytes(MessageJson.Serialize(will)))
            .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithWillRetain();

        if (_settings.Credentials.User is { } user)
            builder.WithCredentials(user, _settings.Credentials.Password ?? "");
        return builder.Build();
    }

    private Task SendAsync(string topic, string payload, bool retain, CancellationToken cancellationToken)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(Encoding.UTF8.GetBytes(payload))
            .WithContentType("application/json")
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(retain)
            .Build();
        return _client.PublishAsync(message, cancellationToken);
    }

    /// <summary>Geordnet trennen: kein Last Will (den Status <c>offline</c> hat der Gateway-Dienst vorher selbst gesendet).</summary>
    public async ValueTask DisposeAsync()
    {
        if (_client.IsConnected)
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            try
            {
                await _client.DisconnectAsync(new MqttClientDisconnectOptionsBuilder().Build(), timeout.Token);
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                // Verbindung war bereits weg; beim Beenden nichts mehr zu tun
            }
        }
        _client.Dispose();
    }
}
