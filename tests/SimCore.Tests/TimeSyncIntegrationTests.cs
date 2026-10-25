using System.Net;
using System.Net.Sockets;
using System.Text;
using SimCore.Contracts;
using SimCore.Time;
using SimCore.TimeSync;

namespace SimCore.Tests;

/// <summary>
/// Master und Simulatoren über echtes TCP auf localhost mit der realen Uhr.
/// Die Tests warten mit Task.Delay auf Zustände (höchstens <see cref="WaitLimit"/>).
/// </summary>
public sealed class TimeSyncIntegrationTests : IAsyncLifetime
{
    private static readonly DateTime SimStart = new(2026, 4, 1, 12, 0, 0);
    private static readonly TimeSpan WaitLimit = TimeSpan.FromSeconds(10);
    private const double Speed = 600;

    // Toleranz für diese Tests: 10 ms Echtzeit · Tempo (der dokumentierte Vorschlag).
    private static readonly TimeSpan Tolerance = TimeSpan.FromMilliseconds(10) * Speed;

    private readonly CancellationTokenSource _stop = new();
    private readonly List<Task> _running = [];
    private readonly List<string> _serverLog = [];
    private MasterClock _master = null!;
    private TimeSyncServer _server = null!;

    public Task InitializeAsync()
    {
        _master = new MasterClock(TimeProvider.System, SimStart, Speed);
        _server = new TimeSyncServer(_master, port: 0, IPAddress.Loopback, TimeSpan.FromMilliseconds(200));
        _server.Log += (_, m) => { lock (_serverLog) _serverLog.Add(m); };
        _server.Start();
        _running.Add(_server.RunAsync(_stop.Token));
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _stop.CancelAsync();
        await Task.WhenAll(_running);
        _stop.Dispose();
    }

    [Fact]
    public async Task TwoSimulators_FollowStartPauseSpeedAndRestart()
    {
        var a = StartSimulator("PV-01");
        var b = StartSimulator("BAT-01");
        await WaitUntilAsync(() => a.Status == SyncStatus.Synced && b.Status == SyncStatus.Synced);

        _master.Resume();
        await WaitUntilAsync(() => a.Mode == SimMode.Running && b.Mode == SimMode.Running);
        AssertWithinTolerance(a, b);

        _master.SetSpeed(1200);
        await WaitUntilAsync(() => a.Speed == 1200 && b.Speed == 1200);
        AssertWithinTolerance(a, b);

        _master.Pause();
        await WaitUntilAsync(() => a.Mode == SimMode.Paused && b.Mode == SimMode.Paused);
        var paused = _master.Now;
        Assert.Equal(paused, a.Now);                              // in der Pause exakt gleich
        Assert.Equal(paused, b.Now);

        var winter = new DateTime(2026, 1, 15, 6, 0, 0);
        _master.Restart(winter, startPaused: true);
        await WaitUntilAsync(() => a.RunId == 2 && b.RunId == 2);
        Assert.Equal(winter, a.Now);
        Assert.Equal(winter, b.Now);
    }

    [Fact]
    public async Task Server_ListsSimulators_WithReportedDeviation()
    {
        var a = StartSimulator("DC-01");
        _master.Resume();
        await WaitUntilAsync(() => _server.Clients.SingleOrDefault()?.Deviation is not null);

        var info = _server.Clients.Single();
        Assert.Equal("DC-01", info.ClientId);
        Assert.Equal("Test", info.ClientKind);
        Assert.Equal(ClientConnectionState.Connected, info.State);
        Assert.True(info.Deviation!.Value.Duration() <= Tolerance, $"Abweichung {info.Deviation}");
        Assert.True(a.RoundTrip < TimeSpan.FromMilliseconds(50), $"RTT {a.RoundTrip}");
    }

    [Fact]
    public async Task SimulatorStarted_BeforeMaster_ConnectsLater()
    {
        // Eigener Master auf freiem Port, der erst nach dem Simulator startet.
        int port = FreePort();
        var clock = new SyncedClock(TimeProvider.System);
        var client = new TimeSyncClient(clock, "127.0.0.1", port, "WIND-01", "Test",
            reconnectDelay: TimeSpan.FromMilliseconds(100));
        string? error = null;
        client.Log += (_, m) => error ??= m;
        _running.Add(client.RunAsync(_stop.Token));

        await WaitUntilAsync(() => client.LastError is not null);
        Assert.Equal(SyncStatus.WaitingForMaster, clock.Status);
        Assert.Contains("nicht erreichbar", error);

        var lateMaster = new MasterClock(TimeProvider.System, SimStart, Speed);
        var lateServer = new TimeSyncServer(lateMaster, port, IPAddress.Loopback, TimeSpan.FromMilliseconds(200));
        _running.Add(lateServer.RunAsync(_stop.Token));

        await WaitUntilAsync(() => clock.Status == SyncStatus.Synced);
        Assert.Equal(lateMaster.Now, clock.Now);
    }

    [Fact]
    public async Task MasterShutdown_IsReported_AndSimulatorStopsTime()
    {
        int port = FreePort();
        using var masterStop = new CancellationTokenSource();
        var master = new MasterClock(TimeProvider.System, SimStart, Speed);
        var server = new TimeSyncServer(master, port, IPAddress.Loopback, TimeSpan.FromMilliseconds(200));
        var serverTask = server.RunAsync(masterStop.Token);

        var clock = new SyncedClock(TimeProvider.System, heartbeatTimeout: TimeSpan.FromMilliseconds(600));
        var client = new TimeSyncClient(clock, "127.0.0.1", port, "PV-02", "Test",
            reconnectDelay: TimeSpan.FromMilliseconds(100));
        var log = new List<string>();
        client.Log += (_, m) => { lock (log) log.Add(m); };
        _running.Add(client.RunAsync(_stop.Token));

        master.Resume();
        await WaitUntilAsync(() => clock.Mode == SimMode.Running);

        await masterStop.CancelAsync();
        await serverTask;

        await WaitUntilAsync(() => clock.Status == SyncStatus.MasterLost);
        Assert.Equal(SimMode.Paused, clock.Mode);
        var frozen = clock.Now;
        await Task.Delay(100);
        Assert.Equal(frozen, clock.Now);
        lock (log)
            Assert.Contains(log, m => m.Contains("Simulations-Master beendet"));
    }

    [Fact]
    public async Task InvalidClient_IsRejected_WithVisibleMessage()
    {
        using var tcp = new TcpClient();
        await tcp.ConnectAsync(_server.LocalEndPoint);
        var stream = tcp.GetStream();
        await stream.WriteAsync(Encoding.UTF8.GetBytes("{\"type\":\"hello\",\"clientId\":\"X\",\"clientKind\":\"Y\",\"protocolVersion\":99}\n"));

        using var reader = new StreamReader(stream);
        string? answer = await reader.ReadLineAsync().WaitAsync(WaitLimit);

        Assert.NotNull(answer);
        Assert.IsType<ByeMessage>(SyncProtocol.Deserialize(answer));
        await WaitUntilAsync(() => { lock (_serverLog) return _serverLog.Any(m => m.Contains("Protokollversion 99")); });
    }

    [Fact]
    public async Task Garbage_DisconnectsOnlyThatClient()
    {
        var good = StartSimulator("BAT-02");
        await WaitUntilAsync(() => good.Status == SyncStatus.Synced);

        using var tcp = new TcpClient();
        await tcp.ConnectAsync(_server.LocalEndPoint);
        var stream = tcp.GetStream();
        await stream.WriteAsync(Encoding.UTF8.GetBytes(SyncProtocol.Serialize(new HelloMessage("BAD", "Test", 1)) + "\nkein json\n"));

        await WaitUntilAsync(() => _server.Clients.Any(c => c.ClientId == "BAD" && c.State == ClientConnectionState.Disconnected));
        lock (_serverLog)
            Assert.Contains(_serverLog, m => m.Contains("Protokollfehler"));
        Assert.Equal(ClientConnectionState.Connected, _server.Clients.Single(c => c.ClientId == "BAT-02").State);
    }

    private SyncedClock StartSimulator(string id)
    {
        var clock = new SyncedClock(TimeProvider.System);
        var client = new TimeSyncClient(clock, "127.0.0.1", _server.LocalEndPoint.Port, id, "Test");
        _running.Add(client.RunAsync(_stop.Token));
        return clock;
    }

    private void AssertWithinTolerance(params SyncedClock[] clocks)
    {
        foreach (var clock in clocks)
        {
            var deviation = (clock.Now - _master.Now).Duration();
            Assert.True(deviation <= Tolerance, $"Abweichung {deviation} > {Tolerance}");
        }
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow + WaitLimit;
        while (!condition())
        {
            if (DateTime.UtcNow > deadline)
                throw new TimeoutException("Bedingung wurde nicht erreicht.");
            await Task.Delay(20);
        }
    }

    private static int FreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
