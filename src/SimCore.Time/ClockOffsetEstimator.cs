namespace SimCore.Time;

/// <summary>
/// Schätzt den Abstand zwischen lokaler Uhr und Master-Uhr (Cristian-Verfahren, wie NTP vereinfacht).
/// Ablauf: Gerät merkt sich <c>gesendet</c>, Master antwortet mit seiner Uhrzeit, Gerät merkt sich <c>empfangen</c>.
/// <c>Offset = Master − (gesendet + RTT/2)</c>, Fehler höchstens RTT/2.
/// Von den letzten <see cref="WindowSize"/> Messungen gilt die mit der kleinsten Umlaufzeit.
/// </summary>
public sealed class ClockOffsetEstimator
{
    public const int WindowSize = 8;

    private readonly Queue<(TimeSpan Offset, TimeSpan RoundTrip)> _samples = new();

    public bool HasSample => _samples.Count > 0;

    /// <summary>Master-Uhr minus lokale Uhr. Ohne Messung 0 (gleicher Rechner angenommen).</summary>
    public TimeSpan Offset { get; private set; }

    /// <summary>Umlaufzeit der verwendeten Messung.</summary>
    public TimeSpan RoundTrip { get; private set; }

    /// <summary>Größtmöglicher Fehler von <see cref="Offset"/> (= RTT/2).</summary>
    public TimeSpan Uncertainty => RoundTrip / 2;

    /// <returns><c>false</c>, wenn die Messung unmöglich ist (Antwort vor dem Senden).</returns>
    public bool AddSample(DateTimeOffset sentLocal, DateTimeOffset masterTime, DateTimeOffset receivedLocal)
    {
        var roundTrip = receivedLocal - sentLocal;
        if (roundTrip < TimeSpan.Zero)
            return false;

        _samples.Enqueue((masterTime - (sentLocal + roundTrip / 2), roundTrip));
        if (_samples.Count > WindowSize)
            _samples.Dequeue();

        var best = _samples.MinBy(s => s.RoundTrip);
        Offset = best.Offset;
        RoundTrip = best.RoundTrip;
        return true;
    }
}
