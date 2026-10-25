namespace SimCore.Time;

/// <summary>
/// UTC-Uhr, die nicht springt: Startzeit aus <see cref="TimeProvider.GetUtcNow"/>, danach nur noch die
/// monotone Stoppuhr (<see cref="TimeProvider.GetTimestamp"/>). Umstellungen der Systemuhr verändern
/// die virtuelle Zeit dadurch nicht.
/// </summary>
public sealed class MonotonicUtcClock
{
    private readonly TimeProvider _timeProvider;
    private readonly DateTimeOffset _startUtc;
    private readonly long _startTimestamp;

    public MonotonicUtcClock(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        _startUtc = timeProvider.GetUtcNow();
        _startTimestamp = timeProvider.GetTimestamp();
    }

    public DateTimeOffset Now => _startUtc + _timeProvider.GetElapsedTime(_startTimestamp);
}
