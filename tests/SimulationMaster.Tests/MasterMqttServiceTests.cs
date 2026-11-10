using SimCore.Contracts;
using SimCore.Time;

namespace SimulationMaster.Tests;

/// <summary>Prüft Ablauf, Sitzung/Sequenz und den Duplikatcache des MQTT-Befehlswegs ohne Broker.</summary>
public class MasterMqttServiceTests
{
    private static readonly DateTime SimStart = new(2026, 4, 1, 12, 0, 0);
    private static readonly DateTime Winter = new(2026, 1, 15, 6, 0, 0);

    private static (MasterMqttService Service, MasterClock Clock) Create()
    {
        var clock = new MasterClock(TimeProvider.System, SimStart, 60);
        var service = new MasterMqttService(clock, new MasterOptions(), _ => { });
        return (service, clock);
    }

    private static MasterCommand Command(MasterClock clock, string action, double? speed = null,
        DateTime? start = null, Guid? id = null, DateTimeOffset? expiresAt = null,
        Guid? session = null, long? sequence = null) =>
        new(id ?? Guid.NewGuid(), session ?? clock.Latest.MasterSession,
            sequence ?? clock.Latest.Sequence, action,
            expiresAt ?? DateTimeOffset.UtcNow.AddSeconds(5), speed, start);

    [Fact]
    public void DuplicateRestart_DoesNotStartSecondRun()
    {
        var (service, clock) = Create();
        var command = Command(clock, "restart", start: Winter);

        var first = service.Handle(command);
        var duplicate = service.Handle(command);

        Assert.True(first.Accepted);
        Assert.True(duplicate.Accepted);
        Assert.Equal(first, duplicate);
        Assert.Equal(2, clock.Latest.RunId);
        Assert.Equal(Winter, clock.Latest.SimAnchor);
    }

    [Fact]
    public void SameCommandIdWithOtherContent_IsRejected()
    {
        var (service, clock) = Create();
        var command = Command(clock, "restart", start: Winter);
        service.Handle(command);

        var altered = command with { StartTime = new DateTime(2026, 7, 15, 6, 0, 0) };
        var result = service.Handle(altered);

        Assert.False(result.Accepted);
        Assert.Equal(2, clock.Latest.RunId);
        Assert.Equal(Winter, clock.Latest.SimAnchor);
    }

    [Fact]
    public void DuplicateSpeedCommand_IsAppliedOnce()
    {
        var (service, clock) = Create();
        var command = Command(clock, "speed", speed: 600);

        var first = service.Handle(command);
        var duplicate = service.Handle(command);

        Assert.True(first.Accepted);
        Assert.Equal(first, duplicate);
        Assert.Equal(600, clock.Latest.Speed);
        Assert.Equal(first.State.Sequence, clock.Latest.Sequence);
    }

    [Fact]
    public void PauseAndResume_AreApplied()
    {
        var (service, clock) = Create();
        clock.Resume();

        Assert.True(service.Handle(Command(clock, "pause")).Accepted);
        Assert.Equal(SimMode.Paused, clock.Latest.Mode);

        Assert.True(service.Handle(Command(clock, "resume")).Accepted);
        Assert.Equal(SimMode.Running, clock.Latest.Mode);
    }

    [Fact]
    public void StaleSequence_IsRejected()
    {
        var (service, clock) = Create();
        var command = Command(clock, "pause");
        clock.SetSpeed(120);

        var result = service.Handle(command);

        Assert.False(result.Accepted);
        Assert.Equal(120, clock.Latest.Speed);
        Assert.Equal(SimMode.Paused, clock.Latest.Mode);
    }

    [Fact]
    public void WrongSession_IsRejected()
    {
        var (service, clock) = Create();
        var command = Command(clock, "restart", start: Winter, session: Guid.NewGuid());

        var result = service.Handle(command);

        Assert.False(result.Accepted);
        Assert.Equal(1, clock.Latest.RunId);
    }

    [Fact]
    public void ExpiredCommand_IsRejected()
    {
        var (service, clock) = Create();
        var command = Command(clock, "pause", expiresAt: DateTimeOffset.UtcNow.AddSeconds(-1));

        Assert.False(service.Handle(command).Accepted);
    }

    [Fact]
    public void ExcessiveValidity_IsRejected()
    {
        var (service, clock) = Create();
        var command = Command(clock, "pause", expiresAt: DateTimeOffset.UtcNow.AddSeconds(11));

        Assert.False(service.Handle(command).Accepted);
    }

    [Theory]
    [InlineData("explode", null)]
    [InlineData("speed", 0.0)]
    [InlineData("speed", 100001.0)]
    public void InvalidActionOrSpeed_IsRejected(string action, double? speed)
    {
        var (service, clock) = Create();
        var before = clock.Latest;

        var result = service.Handle(Command(clock, action, speed: speed));

        Assert.False(result.Accepted);
        Assert.Same(before, clock.Latest);
    }
}
