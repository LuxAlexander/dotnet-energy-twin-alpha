using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Devices.Common;
using Devices.Common.Modbus;
using Modbus.Contracts;

namespace ModbusRabbitGateway.Tests;

/// <summary>Gateway gegen einen echten <see cref="ModbusDeviceServer"/> mit dem Registerplan der Ladesäule.</summary>
public sealed class GatewayAgainstDeviceTests : IDisposable
{
    private static readonly DateTime ModelTime = new(2026, 1, 15, 6, 5, 0);

    private readonly int _port = FreePort();
    private readonly DeviceTimeSource _time = DeviceTimeSource.Create(
        new TimeSyncOptions { Standalone = true, StandaloneStart = ModelTime, StandaloneSpeed = 60 }, "DC-01", "Charger");
    private ModbusDeviceServer _server;

    public GatewayAgainstDeviceTests() => _server = StartServer();

    public void Dispose() => _server.Dispose();

    private ModbusDeviceServer StartServer()
    {
        var server = new ModbusDeviceServer(new ModbusOptions { Port = _port }, ChargerRegisters.Plan, "DC-01");
        server.Start();
        server.Publish(new HeaderValues(OperatingState.Running, 0, 42.5, 123.25, true, ModelTime, 1), _time,
            w => w.SetFloat(ChargerRegisters.PowerLimit, 150));
        return server;
    }

    private static int FreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private DevicePoller NewPoller() =>
        new(new DeviceEndpoint("DC-01", "127.0.0.1", _port), 1500, QualitySettings.Default);

    [Fact]
    public void Poll_ReadsDeviceOverModbus_AndBuildsMessage()
    {
        using var poller = NewPoller();

        var outcome = poller.Poll(DateTimeOffset.UtcNow);

        var message = Assert.IsType<MeasurementMessage>(outcome.Message);
        Assert.Equal("DC-01", message.DeviceId);
        Assert.Equal("2026-01-15T06:05:00", message.SimTime);
        Assert.Equal(("uncertain", "standalone"), (message.Quality, message.QualityReason));   // Testgerät läuft ohne Master
        Assert.Equal(42.5, message.Measurements.Single(m => m.Key == "wirkleistung").Value);
        Assert.Equal(150, message.Measurements.Single(m => m.Key == "leistungsgrenze_der_saeule").Value);
        Assert.Equal(Contract.DeviceState.Online, outcome.StatusChange?.State);
    }

    [Fact]
    public void DeviceLost_GivesOfflineStatusWithoutMeasurements_ThenRecovers()
    {
        using var poller = NewPoller();
        poller.Poll(DateTimeOffset.UtcNow);

        _server.Dispose();
        var lost = poller.Poll(DateTimeOffset.UtcNow);
        var stillLost = poller.Poll(DateTimeOffset.UtcNow);

        Assert.Null(lost.Message);                                     // keine alten Werte als aktuelle
        Assert.Equal(Contract.DeviceState.Offline, lost.StatusChange?.State);
        Assert.Equal("2026-01-15T06:05:00", lost.StatusChange?.LastSimTime);
        Assert.Null(stillLost.StatusChange);                           // Status nur bei Änderung

        _server = StartServer();
        var back = poller.Poll(DateTimeOffset.UtcNow);

        Assert.NotNull(back.Message);
        Assert.Equal(Contract.DeviceState.Online, back.StatusChange?.State);
    }

    [Fact]
    public async Task Service_Once_PublishesGatewayStatusDeviceStatusAndMeasurements()
    {
        var options = GatewayOptions.Parse(["--device", $"DC-01=127.0.0.1:{_port}", "--once"]);
        var publisher = new RecordingPublisher();
        using var poller = NewPoller();
        var log = new StringWriter();

        await new GatewayService(options, [poller], publisher, TimeProvider.System, log).RunAsync(CancellationToken.None);

        Assert.Equal(
        [
            ("energytwin/gateway/GW-01/status", true),
            ("energytwin/devices/DC-01/status", true),
            ("energytwin/devices/DC-01/measurements", false),
            ("energytwin/gateway/GW-01/status", true),
        ], publisher.Sent.Select(s => (s.Topic, s.Retain)));
        Assert.Equal("online", JsonDocument.Parse(publisher.Sent[0].Payload).RootElement.GetProperty("state").GetString());
        Assert.Equal("offline", JsonDocument.Parse(publisher.Sent[^1].Payload).RootElement.GetProperty("state").GetString());
        Assert.Contains("DC-01   Sim 2026-01-15 06:05:00", log.ToString());
    }

    private sealed class RecordingPublisher : IMessagePublisher
    {
        public List<(string Topic, string Payload, bool Retain)> Sent { get; } = [];

        public string Description => "Test";

        public Task PublishAsync(string topic, string payload, bool retain, CancellationToken cancellationToken)
        {
            Sent.Add((topic, payload, retain));
            return Task.CompletedTask;
        }
    }
}
