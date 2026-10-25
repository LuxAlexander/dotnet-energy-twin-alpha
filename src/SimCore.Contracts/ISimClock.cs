namespace SimCore.Contracts;

/// <summary>
/// Lesender Zugriff auf die gemeinsame virtuelle Zeit. Wird vom Master (<c>MasterClock</c>) und von den
/// Geräten (<c>SyncedClock</c>) implementiert. Bewusst nicht <c>ITimeProvider</c> genannt, um Verwechslungen
/// mit <see cref="TimeProvider"/> (reale Uhr) zu vermeiden.
/// </summary>
public interface ISimClock
{
    /// <summary>Aktuelle virtuelle Zeit.</summary>
    DateTime Now { get; }

    /// <summary>Aktueller Betriebszustand.</summary>
    SimMode Mode { get; }

    /// <summary>Aktuelle Geschwindigkeit (virtuelle Sekunden je realer Sekunde).</summary>
    double Speed { get; }

    /// <summary>Aktueller Simulationslauf (steigt bei jedem Neustart).</summary>
    int RunId { get; }
}
