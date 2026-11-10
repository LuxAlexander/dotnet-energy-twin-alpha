using SimCore.Contracts;
using SimCore.Time;

namespace SimCore.Tests;

/// <summary>Prüft den atomaren Befehlsweg des Masters gegen Sitzung und Sequenz.</summary>
public class MasterControlTests
{
    private static readonly DateTimeOffset RealStart = new(2026, 10, 25, 8, 0, 0, TimeSpan.Zero);
    private static readonly DateTime SimStart = new(2026, 4, 1, 12, 0, 0);
    private static readonly TimeSpan Lead = TimeSpan.FromMilliseconds(200);

    private static MasterClock CreateRunning(ManualTimeProvider time)
    {
        var clock = new MasterClock(time, SimStart, 60, Lead);
        clock.Resume();
        time.Advance(Lead);
        return clock;
    }

    private static MasterCommand Command(MasterClock clock, string action, double? speed = null,
        DateTime? start = null, Guid? session = null, long? sequence = null) =>
        new(Guid.NewGuid(), session ?? clock.Latest.MasterSession, sequence ?? clock.Latest.Sequence,
            action, DateTimeOffset.UtcNow.AddSeconds(5), speed, start);

    [Fact]
    public void PauseAndResume_AreApplied()
    {
        var time = new ManualTimeProvider(RealStart);
        var clock = CreateRunning(time);

        Assert.True(clock.TryApplyControl(Command(clock, "pause"), out var paused));
        Assert.Equal(SimMode.Paused, paused.Mode);

        Assert.True(clock.TryApplyControl(Command(clock, "resume"), out var resumed));
        Assert.Equal(SimMode.Running, resumed.Mode);
    }

    [Fact]
    public void SpeedChange_KeepsPausedMode()
    {
        var time = new ManualTimeProvider(RealStart);
        var clock = new MasterClock(time, SimStart, 60, Lead);
        Assert.Equal(SimMode.Paused, clock.Latest.Mode);

        Assert.True(clock.TryApplyControl(Command(clock, "speed", speed: 600), out var state));

        Assert.Equal(600, state.Speed);
        Assert.Equal(SimMode.Paused, state.Mode);
        Assert.Equal(SimMode.Paused, clock.Latest.Mode);
    }

    [Fact]
    public void Restart_StartsNewRunImmediately()
    {
        var time = new ManualTimeProvider(RealStart);
        var clock = CreateRunning(time);
        var winter = new DateTime(2026, 1, 15, 6, 0, 0);

        Assert.True(clock.TryApplyControl(Command(clock, "restart", start: winter), out var state));

        Assert.Equal(2, state.RunId);
        Assert.Equal(SimMode.Running, state.Mode);
        Assert.Equal(winter, state.SimAnchor);
    }

    [Fact]
    public void StaleSequence_IsRejectedWithoutChange()
    {
        var time = new ManualTimeProvider(RealStart);
        var clock = CreateRunning(time);
        var command = Command(clock, "pause");
        clock.SetSpeed(120);

        Assert.False(clock.TryApplyControl(command, out var state));

        Assert.Equal(SimMode.Running, state.Mode);
        Assert.Equal(120, clock.Latest.Speed);
        Assert.Equal(clock.Latest.Sequence, state.Sequence);
    }

    [Fact]
    public void WrongSession_IsRejected()
    {
        var time = new ManualTimeProvider(RealStart);
        var clock = CreateRunning(time);
        var before = clock.Latest;

        Assert.False(clock.TryApplyControl(Command(clock, "restart", start: SimStart, session: Guid.NewGuid()), out _));

        Assert.Same(before, clock.Latest);
        Assert.Equal(1, clock.Latest.RunId);
    }

    [Fact]
    public void InvalidAction_ThrowsAndKeepsState()
    {
        var time = new ManualTimeProvider(RealStart);
        var clock = CreateRunning(time);
        var before = clock.Latest;

        Assert.Throws<ArgumentException>(() => clock.TryApplyControl(Command(clock, "explode"), out _));

        Assert.Same(before, clock.Latest);
    }

    [Fact]
    public void SpeedOutOfRange_ThrowsAndKeepsState()
    {
        var time = new ManualTimeProvider(RealStart);
        var clock = CreateRunning(time);
        var before = clock.Latest;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            clock.TryApplyControl(Command(clock, "speed", speed: 0), out _));

        Assert.Same(before, clock.Latest);
    }

    [Theory]
    [InlineData("pause", 10.0, null)]
    [InlineData("resume", null, "2026-01-15")]
    [InlineData("speed", null, "2026-01-15")]
    [InlineData("restart", 60.0, null)]
    public void MismatchedParameters_AreRejected(string action, double? speed, string? start)
    {
        var time = new ManualTimeProvider(RealStart);
        var clock = CreateRunning(time);
        var before = clock.Latest;
        var startTime = start is null ? (DateTime?)null : DateTime.Parse(start);

        Assert.Throws<ArgumentException>(() =>
            clock.TryApplyControl(Command(clock, action, speed: speed, start: startTime), out _));

        Assert.Same(before, clock.Latest);
    }
}
