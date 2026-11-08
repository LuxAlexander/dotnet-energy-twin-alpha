using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace ModbusRabbitGateway.Tests;

/// <summary>
/// <see cref="MqttPublisher"/> gegen einen MQTT-Broker im Testprozess (MQTTnet.Server statt RabbitMQ, damit die Tests
/// ohne Docker laufen). Geprüft wird das Verhalten aus dem Nachrichtenvertrag: QoS 1, Retain nur für Status,
/// Anmeldung, Ausfall ohne Abbruch, Wiederanlauf mit nachgeholten Status-Nachrichten.
/// </summary>
public sealed class MqttPublisherTests : IAsyncLifetime
{
    private const string User = "gateway";
    private const string Password = "testpasswort";

    private readonly int _port = FreePort();
    private readonly ManualTime _time = new(new DateTimeOffset(2026, 11, 8, 12, 0, 0, TimeSpan.Zero));
    private readonly ConcurrentQueue<(string Topic, string Payload, bool Retain, MqttQualityOfServiceLevel Qos)> _received = new();
    private MqttServer? _server;
    private MqttPublisher? _publisher;

    public async Task InitializeAsync() => _server = await StartServerAsync();

    public async Task DisposeAsync()
    {
        if (_publisher is not null)
            await _publisher.DisposeAsync();
        if (_server is not null)
        {
            await _server.StopAsync();
            _server.Dispose();
        }
    }

    private async Task<MqttServer> StartServerAsync()
    {
        var factory = new MqttServerFactory();
        var options = factory.CreateServerOptionsBuilder().WithDefaultEndpoint().WithDefaultEndpointPort(_port).Build();
        var server = factory.CreateMqttServer(options);
        server.ValidatingConnectionAsync += e =>
        {
            if (e.UserName != User || e.Password != Password)
                e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            return Task.CompletedTask;
        };
        server.InterceptingPublishAsync += e =>
        {
            var m = e.ApplicationMessage;
            _received.Enqueue((m.Topic, Encoding.UTF8.GetString(m.Payload), m.Retain, m.QualityOfServiceLevel));
            return Task.CompletedTask;
        };
        await server.StartAsync();
        return server;
    }

    private MqttPublisher NewPublisher(string password = Password) =>
        _publisher = new MqttPublisher(
            new BrokerSettings("127.0.0.1", _port, "GW-TEST", new MqttCredentials(User, password, "Test")), "GW-TEST", _time);

    private static int FreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private async Task WaitForAsync(Func<bool> condition)
    {
        for (int i = 0; i < 100 && !condition(); i++)
            await Task.Delay(20);
        Assert.True(condition(), "Bedingung nicht innerhalb von 2 s erfüllt");
    }

    [Fact]
    public async Task Publish_SendsWithQos1_RetainOnlyForStatus()
    {
        var publisher = NewPublisher();

        await publisher.PublishAsync("energytwin/devices/DC-01/status", "{\"state\":\"online\"}", retain: true, CancellationToken.None);
        await publisher.PublishAsync("energytwin/devices/DC-01/measurements", "{\"seq\":1}", retain: false, CancellationToken.None);
        await WaitForAsync(() => _received.Count >= 2);

        var messages = _received.ToArray();
        Assert.Equal(("energytwin/devices/DC-01/status", true, MqttQualityOfServiceLevel.AtLeastOnce),
            (messages[0].Topic, messages[0].Retain, messages[0].Qos));
        Assert.Equal(("energytwin/devices/DC-01/measurements", "{\"seq\":1}", false, MqttQualityOfServiceLevel.AtLeastOnce),
            (messages[1].Topic, messages[1].Payload, messages[1].Retain, messages[1].Qos));
        Assert.True(publisher.IsConnected);
    }

    [Fact]
    public async Task WrongPassword_IsBrokerUnavailable_WithReadableMessage()
    {
        var publisher = NewPublisher(password: "falsch");

        var ex = await Assert.ThrowsAsync<BrokerUnavailableException>(() =>
            publisher.PublishAsync("t", "x", retain: false, CancellationToken.None));

        Assert.Contains($"Keine Verbindung zu Broker 127.0.0.1:{_port}", ex.Message);
        Assert.Contains("Anmeldung abgelehnt", ex.Message);
        Assert.DoesNotContain("falsch", ex.Message);                      // Passwort nie in der Meldung
        Assert.False(publisher.IsConnected);
    }

    [Fact]
    public async Task BrokerDown_FailsFast_WaitsForBackoff_ThenReconnectsAndRepublishesStatus()
    {
        var publisher = NewPublisher();
        await publisher.PublishAsync("energytwin/devices/DC-01/status", "online", retain: true, CancellationToken.None);

        var broker = _server!;
        _server = null;
        await broker.StopAsync();                                       // Broker fällt aus
        broker.Dispose();
        await WaitForAsync(() => !publisher.IsConnected);

        // Statuswechsel während des Ausfalls: nicht übertragen, aber gemerkt
        await Assert.ThrowsAsync<BrokerUnavailableException>(() =>
            publisher.PublishAsync("energytwin/devices/BAT-01/status", "offline", retain: true, CancellationToken.None));
        // gleich danach: kein neuer Verbindungsversuch (Wartezeit 1 s), sofortige Meldung
        var waiting = await Assert.ThrowsAsync<BrokerUnavailableException>(() =>
            publisher.PublishAsync("energytwin/devices/DC-01/measurements", "m1", retain: false, CancellationToken.None));
        Assert.Contains("nächster Versuch", waiting.Message);

        _server = await StartServerAsync();                             // Broker wieder da
        while (_received.TryDequeue(out _)) { }
        _time.Advance(TimeSpan.FromSeconds(2));                         // Wartezeit vorbei

        await publisher.PublishAsync("energytwin/devices/DC-01/measurements", "m2", retain: false, CancellationToken.None);
        await WaitForAsync(() => _received.Count >= 3);

        var topics = _received.Select(r => (r.Topic, r.Payload, r.Retain)).ToList();
        Assert.Contains(("energytwin/devices/DC-01/status", "online", true), topics);     // nachgeholt
        Assert.Contains(("energytwin/devices/BAT-01/status", "offline", true), topics);   // aus der Ausfallzeit nachgeholt
        Assert.Contains(("energytwin/devices/DC-01/measurements", "m2", false), topics);
        Assert.DoesNotContain(topics, t => t.Payload == "m1");                            // Messwert aus der Ausfallzeit verworfen
    }

    [Fact]
    public async Task GatewayService_KeepsPollingDuringBrokerOutage_AndReportsOutageOnce()
    {
        var flaky = new FlakyPublisher { Down = true };
        var options = GatewayOptions.Parse(["--device", "DC-01=127.0.0.1:1", "--once", "--timeout", "100"]);
        using var poller = new DevicePoller(options.Devices[0], options.TimeoutMs, options.Quality);
        var log = new StringWriter();
        var service = new GatewayService(options, [poller], flaky, TimeProvider.System, log);

        await service.RunAsync(CancellationToken.None);                 // Broker weg: kein Abbruch
        await service.RunAsync(CancellationToken.None);
        Assert.True(service.BrokerDown);
        Assert.Equal(1, CountOf(log.ToString(), "BROKER NICHT ERREICHBAR"));

        flaky.Down = false;
        await service.RunAsync(CancellationToken.None);

        Assert.False(service.BrokerDown);
        Assert.Contains("BROKER WIEDER VERBUNDEN", log.ToString());
        Assert.Contains("DC-01   OFFLINE", log.ToString());           // Gerätefehler bleiben sichtbar
    }

    private static int CountOf(string text, string part) =>
        (text.Length - text.Replace(part, "").Length) / part.Length;

    private sealed class FlakyPublisher : IMessagePublisher
    {
        public bool Down { get; set; }

        public string Description => "Test";

        public Task PublishAsync(string topic, string payload, bool retain, CancellationToken cancellationToken) =>
            Down ? throw new BrokerUnavailableException("Testausfall") : Task.CompletedTask;
    }

    /// <summary>Reale Zeit zum Weiterstellen (nur <see cref="GetUtcNow"/>, mehr nutzt der Publisher nicht).</summary>
    private sealed class ManualTime(DateTimeOffset start) : TimeProvider
    {
        private DateTimeOffset _now = start;

        public override DateTimeOffset GetUtcNow() => _now;

        public void Advance(TimeSpan by) => _now += by;
    }
}
