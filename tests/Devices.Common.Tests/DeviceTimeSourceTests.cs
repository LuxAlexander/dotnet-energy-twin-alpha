using System.Net;
using SimCore.Contracts;
using SimCore.Time;
using SimCore.TimeSync;

namespace Devices.Common.Tests;

public class DeviceTimeSourceTests
{
    private static readonly DateTime SimStart = new(2026, 1, 15, 6, 0, 0);

    /// <summary>Reale Uhr zum Weiterdrehen per Hand.</summary>
    private sealed class ManualTimeProvider : TimeProvider
    {
        private long _timestamp;
        public override DateTimeOffset GetUtcNow() => new DateTimeOffset(2026, 10, 25, 8, 0, 0, TimeSpan.Zero).AddTicks(_timestamp);
        public override long GetTimestamp() => _timestamp;
        public override long TimestampFrequency => TimeSpan.TicksPerSecond;
        public void Advance(TimeSpan delta) => _timestamp += delta.Ticks;
    }

    [Fact]
    public void Standalone_RunsImmediately_WithLocalClock()
    {
        var time = new ManualTimeProvider();
        var source = DeviceTimeSource.Create(
            new TimeSyncOptions { Standalone = true, StandaloneStart = SimStart, StandaloneSpeed = 60 }, "DC-01", "Charger", time);

        time.Advance(TimeSpan.FromSeconds(10));

        Assert.True(source.IsStandalone);
        Assert.True(source.IsRunning);
        Assert.Equal(SimStart.AddMinutes(10), source.Clock.Now);
        Assert.Contains("STANDALONE", source.StatusText);
    }

    [Fact]
    public void Standalone_PauseAndSpeed_ChangeLocalClock()
    {
        var time = new ManualTimeProvider();
        var source = DeviceTimeSource.Create(
            new TimeSyncOptions { Standalone = true, StandaloneStart = SimStart, StandaloneSpeed = 60 }, "DC-01", "Charger", time);

        source.ChangeSpeed(2);
        time.Advance(TimeSpan.FromSeconds(1));
        Assert.Equal(SimStart.AddMinutes(2), source.Clock.Now);

        source.TogglePause();
        time.Advance(TimeSpan.FromSeconds(30));
        Assert.False(source.IsRunning);
        Assert.Equal(SimStart.AddMinutes(2), source.Clock.Now);
    }

    [Fact]
    public void WithMaster_LocalControlsAreRefused()
    {
        var source = DeviceTimeSource.Create(new TimeSyncOptions(), "DC-01", "Charger");

        Assert.False(source.IsStandalone);
        Assert.False(source.HasTime);
        Assert.Contains("Master", source.TogglePause());
        Assert.Contains("Master", source.ChangeSpeed(2));
        Assert.Contains("WARTE AUF MASTER 127.0.0.1:5100", source.StatusText);
    }

    [Fact]
    public async Task WithMaster_FollowsMasterClock()
    {
        var master = new MasterClock(TimeProvider.System, SimStart, 600);
        var server = new TimeSyncServer(master, port: 0, IPAddress.Loopback, TimeSpan.FromMilliseconds(200));
        server.Start();
        using var stop = new CancellationTokenSource();
        var serverTask = server.RunAsync(stop.Token);

        var source = DeviceTimeSource.Create(
            new TimeSyncOptions { MasterPort = server.LocalEndPoint.Port }, "DC-01", "Charger");
        var clientTask = source.RunAsync(stop.Token);

        master.Resume();
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!(source.IsRunning && server.Clients.Any(c => c.ClientKind == "Charger")) && DateTime.UtcNow < deadline)
            await Task.Delay(20);

        Assert.True(source.IsRunning);
        Assert.Equal(SyncStatus.Synced, source.Status);
        Assert.Contains("synchron", source.StatusText);
        var deviation = (source.Clock.Now - master.Now).Duration();
        Assert.True(deviation <= TimeSpan.FromMilliseconds(10) * 600, $"Abweichung {deviation}");

        await stop.CancelAsync();
        await Task.WhenAll(serverTask, clientTask);
    }
}
