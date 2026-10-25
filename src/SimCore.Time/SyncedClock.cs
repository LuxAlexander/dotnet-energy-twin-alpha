using SimCore.Contracts;

namespace SimCore.Time;

/// <summary>Verbindungszustand eines Geräts zum Master.</summary>
public enum SyncStatus
{
    /// <summary>Noch kein <see cref="TimeState"/> empfangen.</summary>
    WaitingForMaster,

    /// <summary>Zeit folgt dem Master.</summary>
    Synced,

    /// <summary>Länger als das Heartbeat-Timeout nichts vom Master gehört: die Zeit steht.</summary>
    MasterLost
}

/// <summary>
/// Virtuelle Zeit auf Geräteseite. Rechnet aus den empfangenen <see cref="TimeState"/>s und der geschätzten
/// Master-Uhr selbst. Die Laufzeit der Nachrichten verändert die Zeit nicht, solange sie unter der Vorlaufzeit
/// des Masters bleibt. Threadsicher.
/// </summary>
public sealed class SyncedClock : ISimClock
{
    public static readonly TimeSpan DefaultHeartbeatTimeout = TimeSpan.FromSeconds(3);

    private readonly MonotonicUtcClock _clock;
    private readonly TimeLine _line = new();
    private readonly ClockOffsetEstimator _offset = new();
    private readonly Lock _lock = new();
    private DateTimeOffset? _lastContactLocal;

    public SyncedClock(TimeProvider timeProvider, TimeSpan? heartbeatTimeout = null)
    {
        _clock = new MonotonicUtcClock(timeProvider);
        HeartbeatTimeout = heartbeatTimeout ?? DefaultHeartbeatTimeout;
        if (HeartbeatTimeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(heartbeatTimeout), "Das Heartbeat-Timeout muss positiv sein.");
    }

    public TimeSpan HeartbeatTimeout { get; }

    /// <summary>Lokale Uhr (UTC, monoton). Für die Zeitstempel beim Uhrenabgleich verwenden.</summary>
    public DateTimeOffset LocalNow => _clock.Now;

    /// <summary>Geschätzte Uhrzeit des Masters.</summary>
    public DateTimeOffset MasterNow
    {
        get { lock (_lock) return _clock.Now + _offset.Offset; }
    }

    /// <summary>Master-Uhr minus lokale Uhr.</summary>
    public TimeSpan OffsetToMaster
    {
        get { lock (_lock) return _offset.Offset; }
    }

    /// <summary>Umlaufzeit der besten Abgleichsmessung.</summary>
    public TimeSpan RoundTrip
    {
        get { lock (_lock) return _offset.RoundTrip; }
    }

    /// <summary>
    /// Größtmögliche Abweichung der virtuellen Zeit zum Master aus der Ungenauigkeit des Uhrenabgleichs (RTT/2 · Tempo).
    /// Dient als Nachweis, dass die Toleranz eingehalten wird.
    /// </summary>
    public TimeSpan SimUncertainty
    {
        get
        {
            lock (_lock)
            {
                var state = StateNow();
                return state is { Mode: SimMode.Running }
                    ? TimeSpan.FromTicks((long)(_offset.Uncertainty.Ticks * state.Speed))
                    : TimeSpan.Zero;
            }
        }
    }

    /// <summary>
    /// Um wie viel (Master-Uhr) der zuletzt übernommene Zustand zu spät kam. Größer 0 heißt: die Nachricht
    /// war langsamer als die Vorlaufzeit, die virtuelle Zeit hat um <c>Verspätung · Tempo</c> gesprungen.
    /// </summary>
    public TimeSpan LastLateness { get; private set; }

    public SyncStatus Status
    {
        get { lock (_lock) return StatusCore(); }
    }

    /// <summary>
    /// Aktuelle virtuelle Zeit. Vor dem ersten Zustand <see cref="DateTime.MinValue"/>.
    /// Bei <see cref="SyncStatus.MasterLost"/> bleibt sie beim Stand des Timeouts stehen.
    /// </summary>
    public DateTime Now
    {
        get { lock (_lock) return StateNow() is { } s ? s.SimTimeAt(EvaluationTime()) : DateTime.MinValue; }
    }

    /// <summary>Betriebszustand; bei <see cref="SyncStatus.MasterLost"/> immer <see cref="SimMode.Paused"/>.</summary>
    public SimMode Mode
    {
        get
        {
            lock (_lock)
                return StatusCore() == SyncStatus.Synced ? StateNow()!.Mode : SimMode.Paused;
        }
    }

    public double Speed
    {
        get { lock (_lock) return StateNow()?.Speed ?? 0; }
    }

    public int RunId
    {
        get { lock (_lock) return StateNow()?.RunId ?? 0; }
    }

    /// <summary>
    /// Übernimmt einen Zustand vom Master (Änderung oder Heartbeat). Jede Nachricht zählt als Lebenszeichen.
    /// </summary>
    /// <returns><c>true</c>, wenn der Zustand neu war.</returns>
    public bool ApplyState(TimeState state)
    {
        lock (_lock)
        {
            _lastContactLocal = _clock.Now;
            if (!_line.Apply(state))
                return false;

            var masterNow = _clock.Now + _offset.Offset;
            LastLateness = state.EffectiveAt < masterNow ? masterNow - state.EffectiveAt : TimeSpan.Zero;
            _line.Prune(masterNow);
            return true;
        }
    }

    /// <summary>Messung für den Uhrenabgleich (Ping an den Master, Antwort mit dessen <c>MasterNow</c>).</summary>
    public bool AddSyncSample(DateTimeOffset sentLocal, DateTimeOffset masterTime, DateTimeOffset receivedLocal)
    {
        lock (_lock)
        {
            if (!_offset.AddSample(sentLocal, masterTime, receivedLocal))
                return false;
            _lastContactLocal = _clock.Now;
            return true;
        }
    }

    private SyncStatus StatusCore()
    {
        if (_lastContactLocal is not { } lastContact || _line.Latest is null)
            return SyncStatus.WaitingForMaster;
        return _clock.Now - lastContact > HeartbeatTimeout ? SyncStatus.MasterLost : SyncStatus.Synced;
    }

    /// <summary>Master-Uhrzeit, für die gerechnet wird: jetzt, bei verlorenem Master der Zeitpunkt des Timeouts.</summary>
    private DateTimeOffset EvaluationTime()
    {
        var local = StatusCore() == SyncStatus.MasterLost
            ? _lastContactLocal!.Value + HeartbeatTimeout
            : _clock.Now;
        return local + _offset.Offset;
    }

    private TimeState? StateNow() => _line.StateAt(EvaluationTime());
}
