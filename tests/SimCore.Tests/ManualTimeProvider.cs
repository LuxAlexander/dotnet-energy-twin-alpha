namespace SimCore.Tests;

/// <summary>Reale Uhr zum Weiterdrehen per Hand (statt FakeTimeProvider, ohne zusätzliches NuGet-Paket).</summary>
internal sealed class ManualTimeProvider(DateTimeOffset startUtc) : TimeProvider
{
    private DateTimeOffset _utcNow = startUtc;
    private long _timestamp;

    public override DateTimeOffset GetUtcNow() => _utcNow;
    public override long GetTimestamp() => _timestamp;
    public override long TimestampFrequency => TimeSpan.TicksPerSecond;

    /// <summary>Lässt reale Zeit vergehen (Systemuhr und Stoppuhr).</summary>
    public void Advance(TimeSpan delta)
    {
        _utcNow += delta;
        _timestamp += delta.Ticks;
    }

    /// <summary>Verstellt nur die Systemuhr (z. B. Zeitumstellung), die Stoppuhr läuft unverändert.</summary>
    public void JumpWallClock(TimeSpan delta) => _utcNow += delta;
}
