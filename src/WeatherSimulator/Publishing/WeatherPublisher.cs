using Devices.Common;
using MQTTnet;
using MQTTnet.Protocol;
using Shared.Config;
using SimCore.Time;
using WeatherSimulator.Simulation;

namespace WeatherSimulator.Publishing;

/// <summary>Aktueller Wetterstrom; Zustandserkennung unabhängig von MQTT-Verfügbarkeit, kein historischer Puffer.</summary>
public sealed class WeatherPublisher : IAsyncDisposable
{
    private readonly WeatherOptions _options;
    private readonly WeatherModel _model;
    private readonly ManualWeather _manual;
    private readonly DeviceTimeSource _clock;
    private readonly TimeProvider _time;
    private readonly TextWriter _log;
    private readonly IMqttClient _client = new MqttClientFactory().CreateMqttClient();
    private readonly string _session = Guid.NewGuid().ToString("N");
    private readonly Dictionary<string, long> _sentRevisions = new();
    private readonly MqttCredentials _credentials;
    private long _seq;
    private int? _runId;
    private Guid? _masterSession;
    private string? _sentStatus;
    private string _status = "noTime";
    private bool _fullSnapshot = true;
    private bool _clearRetained = true;
    private bool _reportedFailure;
    private DateTimeOffset _nextConnect;
    private DateTimeOffset _nextWeather;
    private int _backoff = 1;

    public WeatherPublisher(WeatherOptions options, WeatherModel model, ManualWeather manual, DeviceTimeSource clock,
        TimeProvider? time = null, TextWriter? log = null)
    {
        _options = options; _model = model; _manual = manual; _clock = clock;
        _time = time ?? TimeProvider.System; _log = log ?? Console.Out;
        _credentials = MqttCredentials.Resolve(null, options.EnvFile, Environment.GetEnvironmentVariable);
    }

    public string StatusText => $"MQTT {(_client.IsConnected ? "verbunden" : "nicht verbunden")} · {_status}";

    public async Task RunAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                await TickAsync(ct);
                await Task.Delay(TimeSpan.FromMilliseconds(80), _time, ct);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { }
        finally
        {
            if (_client.IsConnected)
            {
                using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                try { await SendAsync(WeatherMessages.StatusTopic, StatusJson("offline", false), deadline.Token); }
                catch (Exception ex) when (ex is not OutOfMemoryException)
                { await _log.WriteLineAsync("MQTT: offline konnte nicht bestätigt werden; Last Will bleibt maßgeblich."); }
            }
        }
    }

    /// <summary>Ein Verarbeitungsschritt; auch in Pause wird Master- und Broker-Zustand geprüft.</summary>
    public async Task TickAsync(CancellationToken ct)
    {
        bool valid = _clock.Status == SyncStatus.Synced;
        string status = !valid ? (_clock.HasTime ? "masterLost" : "noTime") : _clock.IsRunning ? "continued" : "paused";
        if (valid && (_runId != _clock.Clock.RunId || _masterSession != _clock.MasterSession))
        {
            _manual.ClearAll();
            _runId = _clock.Clock.RunId;
            _masterSession = _clock.MasterSession;
            _fullSnapshot = _clearRetained = true;
            _sentStatus = null;
        }
        if (_status != status)
        {
            bool recovered = _status is "noTime" or "masterLost";
            _status = status;
            _sentStatus = null;
            if (valid && (status == "continued" || recovered)) _fullSnapshot = true;
            if (!valid) _clearRetained = true;
            await _log.WriteLineAsync($"Wetterstatus: {status}");
        }

        if (!_client.IsConnected && _time.GetUtcNow() < _nextConnect) return;
        try
        {
            if (!_client.IsConnected)
            {
                using var deadline = CancellationTokenSource.CreateLinkedTokenSource(ct);
                deadline.CancelAfter(TimeSpan.FromSeconds(3));
                var builder = new MqttClientOptionsBuilder().WithTcpServer(_options.BrokerHost, _options.BrokerPort)
                    .WithClientId("EnergyTwin-Weather").WithCleanSession().WithTimeout(TimeSpan.FromSeconds(3))
                    .WithWillTopic(WeatherMessages.StatusTopic).WithWillPayload(StatusJson("offline", false, isLastWill: true))
                    .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).WithWillRetain();
                if (_credentials.User is { } user) builder.WithCredentials(user, _credentials.Password ?? "");
                var connected = await _client.ConnectAsync(builder.Build(), deadline.Token);
                if (connected.ResultCode != MqttClientConnectResultCode.Success)
                    throw new IOException($"Anmeldung abgelehnt: {connected.ResultCode}");
                _backoff = 1;
                _fullSnapshot = _clearRetained = true;
                _sentStatus = null;
                await _log.WriteLineAsync(_reportedFailure ? "MQTT wieder verbunden; aktueller Wetterstand wird gesendet." : "MQTT verbunden.");
                _reportedFailure = false;
            }

            // Alte Messwerte entfernen, bevor ein neuer Dienst/Lauf oder ein ungültiger Zeitstatus sichtbar wird.
            if (_clearRetained)
            {
                foreach (var location in LocationCatalog.All)
                    await SendAsync(WeatherMessages.Topic(location.Key), "", ct);
                _clearRetained = false;
            }
            if (_sentStatus != status)
            {
                await SendAsync(WeatherMessages.StatusTopic, StatusJson(status, valid), ct);
                _sentStatus = status;
            }
            if (!valid || _clock.Status != SyncStatus.Synced || _clock.IsRunning != (status == "continued")) return;

            bool regular = status == "continued" && _time.GetUtcNow() >= _nextWeather;
            if (regular || _fullSnapshot)
            {
                foreach (var location in LocationCatalog.All) await PublishLocationAsync(location.Key, ct);
                _fullSnapshot = false;
                _nextWeather = _time.GetUtcNow().AddSeconds(1);
            }
            else
            {
                foreach (var location in LocationCatalog.All)
                    if (_manual.Get(location.Key).Revision != _sentRevisions.GetValueOrDefault(location.Key))
                        await PublishLocationAsync(location.Key, ct);
            }
        }
        catch (Exception ex) when (ex is not OutOfMemoryException && !ct.IsCancellationRequested)
        {
            // Keine Zugangsdaten/Exception-Payload ausgeben; insbesondere kein Passwort.
            if (!_reportedFailure)
                await _log.WriteLineAsync("MQTT nicht erreichbar oder Anmeldung fehlgeschlagen; HTTP bleibt verfügbar, Wiederverbindung läuft.");
            _reportedFailure = true;
            _fullSnapshot = true;
            _nextConnect = _time.GetUtcNow().AddSeconds(_backoff);
            _backoff = Math.Min(30, _backoff * 2);
            if (_client.IsConnected)
            {
                using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                try { await _client.DisconnectAsync(new MqttClientDisconnectOptionsBuilder().Build(), deadline.Token); }
                catch (Exception disconnectError) when (disconnectError is not OutOfMemoryException) { }
            }
        }
    }

    private async Task PublishLocationAsync(string key, CancellationToken ct)
    {
        // Eine Modellabfrage und ihre Herkunft verwenden dieselbe unveränderliche manuelle Momentaufnahme.
        int runId = _clock.Clock.RunId;
        if (_clock.Status != SyncStatus.Synced || runId != _runId || _clock.MasterSession != _masterSession) return;
        var manual = _manual.Get(key);
        var simTime = _clock.Clock.Now;
        var data = _model.Get(key, simTime, runId, manual);
        // Aufwendige Jahreserzeugung kann einen Master-Wechsel überlappen: alte Vorgaben nicht in den neuen Lauf senden.
        if (_clock.Status != SyncStatus.Synced || _clock.Clock.RunId != runId || _clock.MasterSession != _masterSession) return;
        var message = WeatherMessages.Create(data, simTime, manual, _session, ++_seq, _time.GetUtcNow(), _masterSession);
        await SendAsync(WeatherMessages.Topic(key), WeatherMessages.Serialize(message), ct);
        _sentRevisions[key] = manual.Revision;
    }

    private string StatusJson(string status, bool valid, bool isLastWill = false)
    {
        long sequence = ++_seq;
        return WeatherMessages.Serialize(new WeatherStatus(1, _session, sequence, $"{_session}:{sequence}", status,
            _runId, valid ? DateTime.SpecifyKind(_clock.Clock.Now, DateTimeKind.Utc) : null, _time.GetUtcNow(), isLastWill, _masterSession));
    }

    private async Task SendAsync(string topic, string json, CancellationToken ct)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(ct);
        deadline.CancelAfter(TimeSpan.FromSeconds(3));
        var result = await _client.PublishAsync(new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(json)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).WithRetainFlag().Build(), deadline.Token);
        if (!result.IsSuccess) throw new IOException($"MQTT-Nachricht abgelehnt: {result.ReasonCode}");
    }

    public async ValueTask DisposeAsync()
    {
        if (_client.IsConnected)
        {
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            try { await _client.DisconnectAsync(new MqttClientDisconnectOptionsBuilder().Build(), deadline.Token); }
            catch (Exception ex) when (ex is not OutOfMemoryException) { }
        }
        _client.Dispose();
    }
}
