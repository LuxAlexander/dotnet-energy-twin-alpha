using SimCore.Contracts;
using SimCore.Time;

namespace SimCore.Tests;

public class MasterClockTests
{
    private static readonly DateTimeOffset RealStart = new(2026, 10, 25, 8, 0, 0, TimeSpan.Zero);
    private static readonly DateTime SimStart = new(2026, 4, 1, 12, 0, 0);
    private static readonly TimeSpan Lead = TimeSpan.FromMilliseconds(200);

    private readonly ManualTimeProvider _time = new(RealStart);

    private MasterClock CreateRunning(double speed = 60)
    {
        var clock = new MasterClock(_time, SimStart, speed, Lead);
        clock.Resume();
        _time.Advance(Lead);
        return clock;
    }

    [Fact]
    public void NewClock_IsPausedAtStartTime()
    {
        var clock = new MasterClock(_time, SimStart, 60, Lead);

        _time.Advance(TimeSpan.FromMinutes(10));

        Assert.Equal(SimMode.Paused, clock.Mode);
        Assert.Equal(SimStart, clock.Now);
        Assert.Equal(1, clock.RunId);
    }

    [Fact]
    public void Resume_StartsAfterLeadTime_WithConfiguredSpeed()
    {
        var clock = new MasterClock(_time, SimStart, 60, Lead);

        clock.Resume();
        _time.Advance(Lead);
        Assert.Equal(SimStart, clock.Now);
        Assert.Equal(SimMode.Running, clock.Mode);

        _time.Advance(TimeSpan.FromSeconds(10));
        Assert.Equal(SimStart.AddMinutes(10), clock.Now);
    }

    [Fact]
    public void Change_DoesNotTakeEffectBeforeLeadTime()
    {
        var clock = CreateRunning(speed: 60);

        clock.SetSpeed(600);
        _time.Advance(Lead / 2);

        Assert.Equal(60, clock.Speed);
        Assert.Equal(SimStart.AddSeconds(0.1 * 60), clock.Now);
    }

    [Fact]
    public void Pause_StopsTime_WithoutJump()
    {
        var clock = CreateRunning(speed: 60);
        _time.Advance(TimeSpan.FromSeconds(10));                  // Sim: +10 min

        clock.Pause();
        _time.Advance(Lead);                                      // läuft bis zur Pause noch 0,2 s · 60 = 12 s
        var pausedAt = SimStart.AddMinutes(10).AddSeconds(12);
        Assert.Equal(pausedAt, clock.Now);
        Assert.Equal(SimMode.Paused, clock.Mode);

        _time.Advance(TimeSpan.FromHours(1));
        Assert.Equal(pausedAt, clock.Now);
    }

    [Fact]
    public void Resume_ContinuesAtSameTime_WithoutJump()
    {
        var clock = CreateRunning(speed: 60);
        clock.Pause();
        _time.Advance(Lead);
        var pausedAt = clock.Now;
        _time.Advance(TimeSpan.FromMinutes(30));

        clock.Resume();
        _time.Advance(Lead);
        Assert.Equal(pausedAt, clock.Now);

        _time.Advance(TimeSpan.FromSeconds(1));
        Assert.Equal(pausedAt.AddMinutes(1), clock.Now);
    }

    [Fact]
    public void SetSpeed_ChangesRate_WithoutJump()
    {
        var clock = CreateRunning(speed: 60);
        _time.Advance(TimeSpan.FromSeconds(10));

        var state = clock.SetSpeed(600);
        var expectedAtSwitch = SimStart.AddMinutes(10).AddSeconds(12);
        Assert.Equal(expectedAtSwitch, state.SimAnchor);

        _time.Advance(Lead);
        Assert.Equal(expectedAtSwitch, clock.Now);               // kein Sprung beim Umschalten
        Assert.Equal(600, clock.Speed);

        _time.Advance(TimeSpan.FromSeconds(1));
        Assert.Equal(expectedAtSwitch.AddMinutes(10), clock.Now);
    }

    [Fact]
    public void Restart_SetsNewTime_ForNewRun()
    {
        var clock = CreateRunning(speed: 60);
        _time.Advance(TimeSpan.FromSeconds(10));
        var winter = new DateTime(2026, 1, 15, 6, 0, 0);

        clock.Restart(winter);
        _time.Advance(Lead / 2);
        Assert.Equal(1, clock.RunId);                             // alter Lauf bis zum Umschaltzeitpunkt

        _time.Advance(Lead / 2);
        Assert.Equal(2, clock.RunId);
        Assert.Equal(winter, clock.Now);
        Assert.Equal(SimMode.Running, clock.Mode);

        _time.Advance(TimeSpan.FromSeconds(1));
        Assert.Equal(winter.AddMinutes(1), clock.Now);
    }

    [Fact]
    public void Restart_CanStartPaused()
    {
        var clock = CreateRunning();
        var winter = new DateTime(2026, 1, 15, 6, 0, 0);

        clock.Restart(winter, startPaused: true);
        _time.Advance(Lead + TimeSpan.FromMinutes(5));

        Assert.Equal(SimMode.Paused, clock.Mode);
        Assert.Equal(winter, clock.Now);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.5)]
    [InlineData(-1)]
    [InlineData(100_001)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void SetSpeed_OutOfRange_IsRejected(double speed)
    {
        var clock = CreateRunning();
        var before = clock.Latest;

        Assert.Throws<ArgumentOutOfRangeException>(() => clock.SetSpeed(speed));
        Assert.Same(before, clock.Latest);
    }

    [Fact]
    public void RepeatedCommand_CreatesNoNewState()
    {
        var clock = CreateRunning();
        int events = 0;
        clock.StateChanged += (_, _) => events++;

        var first = clock.Pause();
        var second = clock.Pause();
        clock.SetSpeed(clock.Latest.Speed);

        Assert.Same(first, second);
        Assert.Equal(1, events);
    }

    [Fact]
    public void EveryChange_IncreasesSequence_AndRaisesEvent()
    {
        var clock = CreateRunning();
        var received = new List<TimeState>();
        clock.StateChanged += (_, s) => received.Add(s);

        clock.SetSpeed(120);
        clock.Pause();
        clock.Resume();
        clock.Restart(SimStart);

        Assert.Equal(4, received.Count);
        Assert.True(received.Zip(received.Skip(1)).All(p => p.Second.Sequence == p.First.Sequence + 1));
    }

    [Fact]
    public void ChangesWithinLeadTime_StayContinuous()
    {
        var clock = CreateRunning(speed: 60);

        clock.Pause();
        _time.Advance(Lead / 4);
        clock.Resume();                                           // überholt die noch nicht wirksame Pause
        _time.Advance(Lead / 4);
        clock.SetSpeed(120);

        var before = clock.Now;
        _time.Advance(TimeSpan.FromSeconds(2));
        var after = clock.Now;

        Assert.Equal(SimMode.Running, clock.Mode);
        Assert.Equal(120, clock.Speed);
        // Kein Sprung: Fortschritt höchstens Tempo 120 über 2 s.
        Assert.InRange(after - before, TimeSpan.Zero, TimeSpan.FromSeconds(2 * 120));
    }

    [Fact]
    public void SystemClockJump_DoesNotChangeSimTime()
    {
        var clock = CreateRunning(speed: 60);
        var before = clock.Now;

        _time.JumpWallClock(TimeSpan.FromHours(1));

        Assert.Equal(before, clock.Now);
    }

    [Fact]
    public void CurrentStates_ContainsPendingChange()
    {
        var clock = CreateRunning();
        clock.SetSpeed(120);

        var states = clock.CurrentStates;

        Assert.Equal(2, states.Count);
        Assert.Equal(120, states[^1].Speed);
    }
}
