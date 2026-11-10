using System.Text.Json;
using MQTTnet;
using MQTTnet.Protocol;
using Shared.Config;
using SimCore.Contracts;
using SimCore.Time;

namespace SimulationMaster;

/// <summary>Optionale MQTT-Bedienung; Zeitserver und Konsole laufen bei Broker-Ausfall weiter.</summary>
public sealed class MasterMqttService(MasterClock clock, MasterOptions options, Action<string> log)
{
    private readonly Dictionary<Guid, (MasterCommand Command, MasterCommandResult Result)> _results = new();

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var delay = 1;
        while (!cancellationToken.IsCancellationRequested)
        {
            using var client = new MqttClientFactory().CreateMqttClient();
            client.ApplicationMessageReceivedAsync += async args =>
            {
                if (args.ApplicationMessage.Retain || args.ApplicationMessage.Topic != MasterControl.CommandTopic) return;
                var payload = args.ApplicationMessage.ConvertPayloadToString();
                if (payload.Length > 4096) return;
                try
                {
                    var command = JsonSerializer.Deserialize<MasterCommand>(payload, MasterControl.Json);
                    if (command is null || command.CommandId == Guid.Empty) return;
                    var result = Handle(command);
                    await PublishAsync(client, MasterControl.ResultTopic, result, cancellationToken);
                }
                catch (JsonException) { log("Ungültiger MQTT-Masterbefehl verworfen."); }
                catch (Exception) { log("MQTT-Befehlsrückmeldung fehlgeschlagen; Ergebnis für Absender unklar."); }
            };
            try
            {
                var credentials = MqttCredentials.Resolve(null, options.MqttEnvFile ?? Path.Combine(
                    OutputPaths.RepositoryRoot ?? Environment.CurrentDirectory, "docker", ".env"), Environment.GetEnvironmentVariable);
                var connection = new MqttClientOptionsBuilder().WithTcpServer(options.MqttHost, options.MqttPort)
                    .WithClientId($"energytwin-master-{clock.Latest.MasterSession:N}").WithCleanSession()
                    .WithCredentials(credentials.User, credentials.Password).Build();
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeout.CancelAfter(TimeSpan.FromSeconds(10));
                var connected = await client.ConnectAsync(connection, timeout.Token);
                if (connected.ResultCode != MqttClientConnectResultCode.Success) throw new InvalidOperationException();
                var subscribed = await client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f.WithTopic(MasterControl.CommandTopic)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)).Build(), timeout.Token);
                if (subscribed.Items.Any(item => (int)item.ResultCode >= 128)) throw new InvalidOperationException();
                log("MQTT-Mastersteuerung verbunden.");
                delay = 1;
                while (client.IsConnected)
                {
                    var now = clock.MasterNow;
                    var current = clock.Current;
                    await PublishAsync(client, MasterControl.StatusTopic,
                        new MasterStatus(DateTimeOffset.UtcNow, current.SimTimeAt(now), current, clock.Latest), cancellationToken);
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { break; }
            catch (Exception) { log("MQTT nicht verfügbar; Zeitserver läuft weiter. Neuversuch folgt."); }
            try { await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken); }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { break; }
            delay = Math.Min(delay * 2, 30);
        }
    }

    internal MasterCommandResult Handle(MasterCommand command)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var id in _results.Where(pair => pair.Value.Command.ExpiresAt < now).Select(pair => pair.Key).ToArray())
            _results.Remove(id);
        var state = clock.Latest;
        MasterCommandResult Reject(string message) => new(command.CommandId, state.MasterSession, false, message, state);
        if (command.ExpiresAt <= now || command.ExpiresAt > now.AddSeconds(10)) return Reject("Befehl abgelaufen oder Gültigkeit unzulässig.");
        if (_results.TryGetValue(command.CommandId, out var previous))
            return previous.Command == command ? previous.Result : Reject("Befehlskennung bereits anders verwendet.");
        if (_results.Count >= 1024) return Reject("Zu viele Befehle; später erneut versuchen.");
        MasterCommandResult result;
        try
        {
            if (!clock.TryApplyControl(command, out state))
                result = Reject("Master-Zustand wurde inzwischen geändert; aktuellen Stand prüfen.");
            else result = new(command.CommandId, state.MasterSession, true,
                "Vom Master angenommen; wirksam ab " + state.EffectiveAt.ToString("O"), state);
        }
        catch (ArgumentException)
        {
            result = Reject("Ungültige Aktion oder Parameter. Tempo: 1–100000; neuer Lauf benötigt eine Startzeit.");
        }
        _results.Add(command.CommandId, (command, result));
        return result;
    }

    private static async Task PublishAsync<T>(IMqttClient client, string topic, T value, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(3));
        await client.PublishAsync(new MqttApplicationMessageBuilder().WithTopic(topic)
            .WithPayload(JsonSerializer.Serialize(value, MasterControl.Json))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).WithRetainFlag(false).Build(), timeout.Token);
    }
}
