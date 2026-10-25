namespace SimCore.TimeSync;

/// <summary>Verbindungszustand eines Simulators aus Sicht des Masters.</summary>
public enum ClientConnectionState
{
    /// <summary>Verbunden, meldet sich regelmäßig.</summary>
    Connected,

    /// <summary>Verbunden, aber länger als das Client-Timeout keine Nachricht.</summary>
    NotResponding,

    /// <summary>Verbindung beendet (geordnet oder abgebrochen).</summary>
    Disconnected
}

/// <summary>Momentaufnahme eines Simulators für die Anzeige im Master.</summary>
/// <param name="SimTime">Zuletzt gemeldete virtuelle Zeit des Simulators.</param>
/// <param name="Deviation">Gemeldete Zeit minus Master-Zeit beim Empfang (enthält die Laufzeit · Tempo).</param>
/// <param name="SimUncertainty">Vom Simulator gemeldete größtmögliche Abweichung (RTT/2 · Tempo).</param>
public sealed record TimeSyncClientInfo(
    string ClientId,
    string ClientKind,
    string RemoteEndPoint,
    ClientConnectionState State,
    DateTimeOffset LastSeen,
    DateTime? SimTime,
    TimeSpan? Deviation,
    TimeSpan? SimUncertainty,
    string? SyncStatus);
