using SimCore.Contracts;
using SimCore.Time;

namespace SimCore.Tests;

public class SyncedClockTests
{
    private static readonly DateTimeOffset RealStart = new(2026, 10, 25, 8, 0, 0, TimeSpan.Zero);
    private static readonly DateTime SimStart = new(2026, 4, 1, 12, 0, 0);
    private static readonly TimeSpan Lead = TimeSpan.FromMilliseconds(200);

    // Lang genug, dass Tests ohne Heartbeats nicht in "Master verloren" laufen.
    private static readonly TimeSpan NoTimeout = TimeSpan.FromHours(1);

    // Gleicher Rechner: Master und Geräte teilen dieselbe Uhr.
    private readonly ManualTimeProvider _time = new(RealStart);

    [Fact]
    public void BeforeFirstState_WaitsForMaster()
    {
        var device = new SyncedClock(_time);

        Assert.Equal(SyncStatus.WaitingForMaster, device.Status);
        Assert.Equal(DateTime.MinValue, device.Now);
        Assert.Equal(SimMode.Paused, device.Mode);
    }

    [Fact]
    public void DifferentDeliveryDelays_GiveSameSimTime()
    {
        var master = new MasterClock(_time, SimStart, 60, Lead);
        var fast = new SyncedClock(_time, NoTimeout);
        var slow = new SyncedClock(_time, NoTimeout);
        fast.ApplyState(master.Latest);
        slow.ApplyState(master.Latest);

        // Resume: Gerät A sofort, Gerät B 150 ms später (unter der Vorlaufzeit).
        var resume = master.Resume();
        fast.ApplyState(resume);
        _time.Advance(TimeSpan.FromMilliseconds(150));
        slow.ApplyState(resume);
        _time.Advance(TimeSpan.FromSeconds(5));
        AssertAllEqual(master, fast, slow);

        // Tempowechsel: diesmal ist A das langsame Gerät.
        var speed = master.SetSpeed(600);
        slow.ApplyState(speed);
        _time.Advance(TimeSpan.FromMilliseconds(190));
        fast.ApplyState(speed);
        AssertAllEqual(master, fast, slow);
        _time.Advance(TimeSpan.FromSeconds(3));
        AssertAllEqual(master, fast, slow);

        Assert.Equal(TimeSpan.Zero, fast.LastLateness);
        Assert.Equal(TimeSpan.Zero, slow.LastLateness);
    }

    [Fact]
    public void PauseAndRestart_ReachAllDevicesAtTheSameTime()
    {
        var master = new MasterClock(_time, SimStart, 60, Lead);
        var a = new SyncedClock(_time, NoTimeout);
        var b = new SyncedClock(_time, NoTimeout);
        Deliver(master.Latest, a, b);
        Deliver(master.Resume(), a, b);
        _time.Advance(TimeSpan.FromSeconds(10));

        Deliver(master.Pause(), a, b);
        _time.Advance(TimeSpan.FromSeconds(30));
        AssertAllEqual(master, a, b);
        Assert.Equal(SimMode.Paused, a.Mode);

        var winter = new DateTime(2026, 1, 15, 6, 0, 0);
        Deliver(master.Restart(winter), a, b);
        _time.Advance(Lead);
        Assert.Equal(winter, a.Now);
        Assert.Equal(winter, b.Now);
        Assert.Equal(2, a.RunId);
    }

    [Fact]
    public void LateState_IsAppliedAndReported()
    {
        var master = new MasterClock(_time, SimStart, 60, Lead);
        var device = new SyncedClock(_time);
        device.ApplyState(master.Latest);

        var resume = master.Resume();
        _time.Advance(TimeSpan.FromMilliseconds(300));           // 100 ms nach dem Umschaltzeitpunkt
        device.ApplyState(resume);

        Assert.Equal(TimeSpan.FromMilliseconds(100), device.LastLateness);
        Assert.Equal(master.Now, device.Now);                     // danach wieder exakt auf der Geraden
    }

    [Fact]
    public void OldOrDuplicateStates_AreIgnored()
    {
        var master = new MasterClock(_time, SimStart, 60, Lead);
        var device = new SyncedClock(_time);
        var first = master.Latest;
        var second = master.Resume();

        Assert.True(device.ApplyState(second));
        Assert.False(device.ApplyState(second));                  // Heartbeat
        Assert.False(device.ApplyState(first));                   // veraltet
    }

    [Fact]
    public void NewDevice_DuringPendingChange_UsesAllCurrentStates()
    {
        var master = new MasterClock(_time, SimStart, 60, Lead);
        master.Resume();
        _time.Advance(TimeSpan.FromSeconds(10));
        master.Pause();                                           // angekündigt, noch nicht wirksam

        var device = new SyncedClock(_time);
        foreach (var state in master.CurrentStates)
            device.ApplyState(state);

        Assert.Equal(master.Now, device.Now);
        Assert.Equal(SimMode.Running, device.Mode);
        _time.Advance(Lead);
        Assert.Equal(master.Now, device.Now);
        Assert.Equal(SimMode.Paused, device.Mode);
    }

    [Fact]
    public void RestartedMaster_ReplacesTimeLine()
    {
        var oldMaster = new MasterClock(_time, SimStart, 60, Lead);
        var device = new SyncedClock(_time);
        device.ApplyState(oldMaster.Resume());
        device.ApplyState(oldMaster.SetSpeed(120));

        var newMaster = new MasterClock(_time, new DateTime(2026, 7, 1), 1, Lead);  // beginnt wieder bei Sequence 1

        Assert.True(device.ApplyState(newMaster.Latest));
        Assert.Equal(newMaster.Now, device.Now);
    }

    [Fact]
    public void MasterLost_FreezesTime_UntilContactReturns()
    {
        var master = new MasterClock(_time, SimStart, 60, Lead);
        var device = new SyncedClock(_time, TimeSpan.FromSeconds(3));
        device.ApplyState(master.Resume());                       // letzter Kontakt; Start nach 0,2 s Vorlauf

        _time.Advance(TimeSpan.FromSeconds(10));                  // keine Heartbeats mehr
        Assert.Equal(SyncStatus.MasterLost, device.Status);
        Assert.Equal(SimMode.Paused, device.Mode);
        var frozen = device.Now;
        Assert.Equal(SimStart.AddSeconds((3 - 0.2) * 60), frozen); // Stand beim Timeout (3 s nach letztem Kontakt)

        _time.Advance(TimeSpan.FromSeconds(10));
        Assert.Equal(frozen, device.Now);

        device.ApplyState(master.Latest);                         // Heartbeat kommt wieder
        Assert.Equal(SyncStatus.Synced, device.Status);
        Assert.Equal(master.Now, device.Now);
    }

    [Fact]
    public void OffsetEstimation_CorrectsDifferentComputerClocks()
    {
        // Gerät auf einem anderen Rechner: dessen Systemuhr geht 5 s vor.
        var deviceTime = new ManualTimeProvider(RealStart + TimeSpan.FromSeconds(5));
        var master = new MasterClock(_time, SimStart, 60, Lead);
        var device = new SyncedClock(deviceTime, NoTimeout);
        var delay = TimeSpan.FromMilliseconds(10);

        var sent = device.LocalNow;
        AdvanceBoth(deviceTime, delay);
        var masterTime = master.MasterNow;
        AdvanceBoth(deviceTime, delay);
        device.AddSyncSample(sent, masterTime, device.LocalNow);

        Assert.Equal(TimeSpan.FromSeconds(-5), device.OffsetToMaster);
        Assert.Equal(master.MasterNow, device.MasterNow);

        device.ApplyState(master.Resume());
        AdvanceBoth(deviceTime, TimeSpan.FromSeconds(10));
        Assert.Equal(master.Now, device.Now);
    }

    [Fact]
    public void AsymmetricDelay_ErrorStaysWithinReportedUncertainty()
    {
        var deviceTime = new ManualTimeProvider(RealStart);
        var master = new MasterClock(_time, SimStart, 600, Lead);
        var device = new SyncedClock(deviceTime);

        // Hinweg 2 ms, Rückweg 18 ms: schlechtester Fall für die Schätzung.
        var sent = device.LocalNow;
        AdvanceBoth(deviceTime, TimeSpan.FromMilliseconds(2));
        var masterTime = master.MasterNow;
        AdvanceBoth(deviceTime, TimeSpan.FromMilliseconds(18));
        device.AddSyncSample(sent, masterTime, device.LocalNow);

        device.ApplyState(master.Resume());
        AdvanceBoth(deviceTime, TimeSpan.FromSeconds(1));

        var error = (device.Now - master.Now).Duration();
        Assert.Equal(TimeSpan.FromMilliseconds(10), device.RoundTrip / 2);
        Assert.Equal(TimeSpan.FromSeconds(6), device.SimUncertainty);      // 10 ms · 600
        Assert.True(error <= device.SimUncertainty, $"Abweichung {error} > {device.SimUncertainty}");
    }

    private void AdvanceBoth(ManualTimeProvider other, TimeSpan delta)
    {
        _time.Advance(delta);
        other.Advance(delta);
    }

    private static void Deliver(TimeState state, params SyncedClock[] devices)
    {
        foreach (var device in devices)
            device.ApplyState(state);
    }

    private static void AssertAllEqual(MasterClock master, params SyncedClock[] devices)
    {
        foreach (var device in devices)
            Assert.Equal(master.Now, device.Now);
    }
}
