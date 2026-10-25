namespace SimCore.Contracts;

/// <summary>
/// Vertrag für die gemeinsame virtuelle Zeit. Ein <see cref="TimeState"/> beschreibt keinen Zeitpunkt,
/// sondern eine Gerade: ab der Master-Uhrzeit <see cref="EffectiveAt"/> gilt
/// <c>SimZeit = SimAnchor + (MasterZeit − EffectiveAt) · Speed</c> (bei <see cref="SimMode.Paused"/>: <c>SimAnchor</c>).
/// Der Master setzt <see cref="EffectiveAt"/> bei jeder Änderung um eine Vorlaufzeit in die Zukunft.
/// Dadurch wirkt die Änderung bei allen Geräten gleichzeitig, egal wann die Nachricht ankommt.
/// </summary>
/// <param name="MasterSession">Zufällige Kennung des Master-Prozesses. Ändert sie sich (Master neu gestartet),
/// verwerfen die Geräte ihren bisherigen Zeitverlauf.</param>
/// <param name="RunId">Simulationslauf. Steigt bei jedem Neustart mit neuer virtueller Zeit (auch Teil des Wetter-Seeds).</param>
/// <param name="Sequence">Steigt bei jeder Änderung (über alle Läufe). Ältere oder doppelte Zustände werden ignoriert.</param>
/// <param name="Mode">Läuft oder pausiert ab <see cref="EffectiveAt"/>.</param>
/// <param name="Speed">Virtuelle Sekunden je realer Sekunde.</param>
/// <param name="SimAnchor">Virtuelle Zeit zum Zeitpunkt <see cref="EffectiveAt"/>.</param>
/// <param name="EffectiveAt">Uhrzeit des Masters (UTC), ab der dieser Zustand gilt.</param>
public sealed record TimeState(
    Guid MasterSession,
    int RunId,
    long Sequence,
    SimMode Mode,
    double Speed,
    DateTime SimAnchor,
    DateTimeOffset EffectiveAt)
{
    /// <summary>
    /// Virtuelle Zeit zur Master-Uhrzeit <paramref name="masterTime"/> nach dieser Geraden.
    /// Master und Geräte verwenden genau diese Formel, damit sie bei gleicher Master-Uhrzeit dasselbe Ergebnis liefern.
    /// </summary>
    public DateTime SimTimeAt(DateTimeOffset masterTime)
    {
        if (Mode == SimMode.Paused)
            return SimAnchor;

        long elapsedTicks = (masterTime - EffectiveAt).Ticks;
        return SimAnchor.AddTicks((long)Math.Round(elapsedTicks * Speed));
    }
}
