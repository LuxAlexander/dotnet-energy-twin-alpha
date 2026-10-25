using SimCore.Contracts;

namespace SimCore.Time;

/// <summary>
/// Besitzt die virtuelle Zeit des Gesamtsystems (läuft im Simulations-Master).
/// Jede Änderung erzeugt einen neuen <see cref="TimeState"/>, der erst nach <see cref="LeadTime"/> gilt.
/// Sein <see cref="TimeState.SimAnchor"/> ist die virtuelle Zeit der alten Geraden zu diesem Zeitpunkt,
/// deshalb gibt es bei Pause, Fortsetzen und Tempowechsel keinen Zeitsprung.
/// Threadsicher.
/// </summary>
public sealed class MasterClock : ISimClock
{
    public const double MinSpeed = 1;
    public const double MaxSpeed = 100_000;

    /// <summary>Standard-Vorlaufzeit; muss größer sein als die längste erwartete Nachrichtenlaufzeit zu den Geräten.</summary>
    public static readonly TimeSpan DefaultLeadTime = TimeSpan.FromMilliseconds(200);

    private readonly MonotonicUtcClock _clock;
    private readonly TimeLine _line = new();
    private readonly Lock _lock = new();

    /// <summary>Legt die Uhr pausiert bei <paramref name="startSimTime"/> an (Lauf 1). Gestartet wird mit <see cref="Resume"/>.</summary>
    public MasterClock(TimeProvider timeProvider, DateTime startSimTime, double speed = 1, TimeSpan? leadTime = null)
    {
        ValidateSpeed(speed);
        LeadTime = leadTime ?? DefaultLeadTime;
        if (LeadTime < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(leadTime), "Die Vorlaufzeit darf nicht negativ sein.");

        _clock = new MonotonicUtcClock(timeProvider);
        _line.Apply(new TimeState(Guid.NewGuid(), RunId: 1, Sequence: 1, SimMode.Paused, speed, startSimTime, _clock.Now));
    }

    /// <summary>Wird nach jeder Änderung ausgelöst (zum Verteilen an die Geräte).</summary>
    public event EventHandler<TimeState>? StateChanged;

    public TimeSpan LeadTime { get; }

    /// <summary>Uhrzeit des Masters (UTC, monoton). Bezugsuhr für alle <see cref="TimeState.EffectiveAt"/>.</summary>
    public DateTimeOffset MasterNow => _clock.Now;

    /// <summary>Zuletzt angekündigter Zustand (kann noch in der Zukunft gelten). Wird als Heartbeat verschickt.</summary>
    public TimeState Latest
    {
        get { lock (_lock) return _line.Latest!; }
    }

    /// <summary>
    /// Alle Zustände, die ab jetzt noch gebraucht werden (gültiger + angekündigte).
    /// Ein neu verbundenes Gerät bekommt diese Liste, damit es auch während einer angekündigten Änderung richtig rechnet.
    /// </summary>
    public IReadOnlyList<TimeState> CurrentStates
    {
        get
        {
            lock (_lock)
            {
                _line.Prune(_clock.Now);
                return [.. _line.States];
            }
        }
    }

    public DateTime Now
    {
        get
        {
            lock (_lock)
            {
                var now = _clock.Now;
                return _line.StateAt(now)!.SimTimeAt(now);
            }
        }
    }

    public SimMode Mode => Current.Mode;
    public double Speed => Current.Speed;
    public int RunId => Current.RunId;

    /// <summary>Zustand, der jetzt gilt.</summary>
    public TimeState Current
    {
        get { lock (_lock) return _line.StateAt(_clock.Now)!; }
    }

    /// <summary>Pausiert die Simulation. Ist sie schon (angekündigt) pausiert, passiert nichts.</summary>
    public TimeState Pause() => Change(latest => latest.Mode == SimMode.Paused
        ? null
        : (at, next) => next with { Mode = SimMode.Paused, SimAnchor = latest.SimTimeAt(at) });

    /// <summary>Startet oder setzt die Simulation an derselben virtuellen Zeit fort.</summary>
    public TimeState Resume() => Change(latest => latest.Mode == SimMode.Running
        ? null
        : (at, next) => next with { Mode = SimMode.Running, SimAnchor = latest.SimTimeAt(at) });

    /// <summary>Ändert die Geschwindigkeit ohne Zeitsprung.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Tempo außerhalb von [<see cref="MinSpeed"/>, <see cref="MaxSpeed"/>].</exception>
    public TimeState SetSpeed(double speed)
    {
        ValidateSpeed(speed);
        return Change(latest => latest.Speed == speed
            ? null
            : (at, next) => next with { Speed = speed, SimAnchor = latest.SimTimeAt(at) });
    }

    /// <summary>Neustart mit neuer virtueller Zeit (neuer Lauf, gewollter Sprung).</summary>
    public TimeState Restart(DateTime newSimTime, bool startPaused = false) =>
        Change(latest => (_, next) => next with
        {
            RunId = latest.RunId + 1,
            Mode = startPaused ? SimMode.Paused : SimMode.Running,
            SimAnchor = newSimTime
        });

    /// <param name="plan">Liefert zum zuletzt angekündigten Zustand die Änderung oder <c>null</c>, wenn nichts zu tun ist.</param>
    private TimeState Change(Func<TimeState, Func<DateTimeOffset, TimeState, TimeState>?> plan)
    {
        TimeState next;
        lock (_lock)
        {
            var latest = _line.Latest!;
            var build = plan(latest);
            if (build is null)
                return latest;

            var now = _clock.Now;
            _line.Prune(now);

            // Nie früher als die vorige Ankündigung, damit die Zustände zeitlich geordnet bleiben.
            var effectiveAt = now + LeadTime;
            if (effectiveAt < latest.EffectiveAt)
                effectiveAt = latest.EffectiveAt;

            next = build(effectiveAt, latest with { Sequence = latest.Sequence + 1, EffectiveAt = effectiveAt });
            _line.Apply(next);
        }

        StateChanged?.Invoke(this, next);
        return next;
    }

    private static void ValidateSpeed(double speed)
    {
        if (!double.IsFinite(speed) || speed < MinSpeed || speed > MaxSpeed)
            throw new ArgumentOutOfRangeException(nameof(speed), speed,
                $"Tempo muss zwischen {MinSpeed} und {MaxSpeed} liegen.");
    }
}
