using SimCore.Contracts;

namespace SimCore.Time;

/// <summary>
/// Zeitverlauf aus mehreren <see cref="TimeState"/>s: der gerade gültige und bereits angekündigte,
/// deren <see cref="TimeState.EffectiveAt"/> noch in der Zukunft liegt. Wird von Master und Geräten gleich verwendet.
/// Nicht threadsicher, die Aufrufer sperren selbst.
/// </summary>
public sealed class TimeLine
{
    private readonly List<TimeState> _states = [];

    /// <summary>Alle noch benötigten Zustände, aufsteigend nach <see cref="TimeState.Sequence"/>.</summary>
    public IReadOnlyList<TimeState> States => _states;

    /// <summary>Zuletzt angekündigter Zustand (kann noch in der Zukunft gelten).</summary>
    public TimeState? Latest => _states.Count == 0 ? null : _states[^1];

    /// <summary>
    /// Übernimmt einen Zustand. Liefert <c>false</c>, wenn er alt oder doppelt ist (z. B. Heartbeat).
    /// Ein Zustand eines neu gestarteten Masters (andere <see cref="TimeState.MasterSession"/>) ersetzt den ganzen Verlauf.
    /// </summary>
    public bool Apply(TimeState state)
    {
        if (Latest is { } latest)
        {
            if (latest.MasterSession != state.MasterSession)
            {
                _states.Clear();
            }
            else
            {
                if (state.Sequence <= latest.Sequence)
                    return false;
                if (state.EffectiveAt < latest.EffectiveAt)
                    throw new ArgumentException(
                        $"Zustand {state.Sequence} gilt früher ({state.EffectiveAt:O}) als sein Vorgänger ({latest.EffectiveAt:O}).",
                        nameof(state));
            }
        }

        _states.Add(state);
        return true;
    }

    /// <summary>
    /// Zustand, der zur Master-Uhrzeit <paramref name="masterTime"/> gilt. Liegt die Zeit vor dem ersten bekannten
    /// Zustand, wird dieser zurück verlängert.
    /// </summary>
    public TimeState? StateAt(DateTimeOffset masterTime)
    {
        if (_states.Count == 0)
            return null;

        for (int i = _states.Count - 1; i > 0; i--)
        {
            if (_states[i].EffectiveAt <= masterTime)
                return _states[i];
        }
        return _states[0];
    }

    /// <summary>Virtuelle Zeit zur Master-Uhrzeit <paramref name="masterTime"/>.</summary>
    public DateTime? SimTimeAt(DateTimeOffset masterTime) => StateAt(masterTime)?.SimTimeAt(masterTime);

    /// <summary>Entfernt Zustände, die ab <paramref name="masterTime"/> nicht mehr gebraucht werden.</summary>
    public void Prune(DateTimeOffset masterTime)
    {
        int lastDue = _states.FindLastIndex(s => s.EffectiveAt <= masterTime);
        if (lastDue > 0)
            _states.RemoveRange(0, lastDue);
    }
}
