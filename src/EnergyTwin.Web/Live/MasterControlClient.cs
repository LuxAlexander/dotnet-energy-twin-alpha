using System.Text.Json;
using MQTTnet;
using MQTTnet.Protocol;
using SimCore.Contracts;

namespace EnergyTwin.Web.Live;

/// <summary>Browserübergreifender Master-Zustand und genau ein ausstehender Bedienauftrag.</summary>
public sealed class MasterControlClient
{
    private readonly object _sync = new();
    private IMqttClient? _client;
    private MasterStatus? _status;
    private DateTimeOffset _receivedAt;
    private MasterCommand? _pending;
    private TaskCompletionSource<MasterCommandResult>? _reply;

    public (MasterStatus? Status, bool Available) Snapshot()
    {
        lock (_sync) return (_status, IsAvailable());
    }

    private bool IsAvailable() => _client?.IsConnected == true && _status is not null
        && DateTimeOffset.UtcNow - _receivedAt < TimeSpan.FromSeconds(30)
        && DateTimeOffset.UtcNow - _status.SentAt < TimeSpan.FromSeconds(30);

    public void SetConnection(IMqttClient? client)
    {
        lock (_sync)
        {
            _client = client;
            _receivedAt = DateTimeOffset.MinValue;
            if (client is null) _reply?.TrySetException(new IOException("MQTT-Verbindung unterbrochen; Ergebnis unklar."));
        }
    }

    public void Receive(MqttApplicationMessage message)
    {
        if (message.Retain) return;
        var payload = message.ConvertPayloadToString();
        if (payload.Length > 16384) return;
        try
        {
            lock (_sync)
            {
                if (message.Topic == MasterControl.StatusTopic)
                {
                    var status = JsonSerializer.Deserialize<MasterStatus>(payload, MasterControl.Json);
                    if (status?.Current is null || status.Latest is null || status.Latest.MasterSession == Guid.Empty
                        || status.Current.MasterSession != status.Latest.MasterSession
                        || !Enum.IsDefined(status.Current.Mode) || !Enum.IsDefined(status.Latest.Mode)
                        || !double.IsFinite(status.Current.Speed) || status.Current.Speed < 1 || status.Current.Speed > 100000
                    || status.SentAt > DateTimeOffset.UtcNow.AddSeconds(1)
                    || DateTimeOffset.UtcNow - status.SentAt > TimeSpan.FromSeconds(30)) return;
                    if (_status?.Latest.MasterSession == status.Latest.MasterSession
                        && (_status.SentAt > status.SentAt || _status.Latest.Sequence > status.Latest.Sequence)) return;
                    _status = status;
                    _receivedAt = DateTimeOffset.UtcNow;
                }
                else if (message.Topic == MasterControl.ResultTopic)
                {
                    var result = JsonSerializer.Deserialize<MasterCommandResult>(payload, MasterControl.Json);
                    if (result is not null && result.CommandId == _pending?.CommandId
                        && result.MasterSession == _pending.MasterSession && result.State is not null)
                        _reply?.TrySetResult(result);
                }
            }
        }
        catch (JsonException) { /* Fremde oder beschädigte Nachrichten ändern keinen Zustand. */ }
    }

    public Task<string> SendAsync(bool running, CancellationToken cancellationToken) =>
        SendControlAsync(running ? "resume" : "pause", null, null, null, cancellationToken);

    public Task<string> SetSpeedAsync(double speed, TimeState expected, CancellationToken cancellationToken) =>
        SendControlAsync("speed", speed, null, expected, cancellationToken);

    public Task<string> RestartAsync(DateTime startTime, TimeState expected, CancellationToken cancellationToken) =>
        SendControlAsync("restart", null, startTime, expected, cancellationToken);

    private async Task<string> SendControlAsync(string action, double? speed, DateTime? startTime,
        TimeState? expected, CancellationToken cancellationToken)
    {
        IMqttClient client;
        MasterCommand command;
        TaskCompletionSource<MasterCommandResult> reply;
        lock (_sync)
        {
            if (!IsAvailable()) return "Der Master wird noch gestartet. Bitte warten Sie und versuchen Sie es erneut.";
            if (_pending is not null) return "Ein Master-Befehl wartet bereits auf Rückmeldung.";
            client = _client!;
            expected ??= _status!.Latest;
            if (expected.MasterSession != _status!.Latest.MasterSession || expected.Sequence != _status.Latest.Sequence)
                return "Master-Zustand wurde inzwischen geändert. Eingabe neu öffnen und aktuellen Stand prüfen.";
            command = new(Guid.NewGuid(), expected.MasterSession, expected.Sequence,
                action, DateTimeOffset.UtcNow.AddSeconds(5), speed, startTime);
            reply = new(TaskCreationOptions.RunContinuationsAsynchronously);
            _pending = command;
            _reply = reply;
        }
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(6));
        try
        {
            await client.PublishAsync(new MqttApplicationMessageBuilder().WithTopic(MasterControl.CommandTopic)
                .WithPayload(JsonSerializer.Serialize(command, MasterControl.Json))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).WithRetainFlag(false).Build(), timeout.Token);
            var result = await reply.Task.WaitAsync(timeout.Token);
            return result.Accepted ? "Master bestätigt: " + result.Message : "Master lehnt ab: " + result.Message;
        }
        catch (Exception)
        {
            return "Der Master antwortet noch nicht. Bitte warten Sie einen Moment und versuchen Sie es erneut.";
        }
        finally
        {
            lock (_sync) { _pending = null; _reply = null; }
        }
    }
}
