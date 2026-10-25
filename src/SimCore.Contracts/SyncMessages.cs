using System.Text.Json.Serialization;

namespace SimCore.Contracts;

/// <summary>
/// Nachrichten der Zeitsynchronisation zwischen Simulations-Master (TCP-Server) und Simulatoren (TCP-Clients).
/// Übertragung: eine JSON-Nachricht pro Zeile (UTF-8, <c>\n</c>), Feld <c>type</c> bestimmt die Art.
/// Serialisierung über <see cref="SyncProtocol"/>.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(HelloMessage), "hello")]
[JsonDerivedType(typeof(StatesMessage), "states")]
[JsonDerivedType(typeof(PingMessage), "ping")]
[JsonDerivedType(typeof(PongMessage), "pong")]
[JsonDerivedType(typeof(ByeMessage), "bye")]
public abstract record SyncMessage;

/// <summary>Simulator → Master, erste Nachricht nach dem Verbindungsaufbau.</summary>
/// <param name="ClientId">Gerätekennung, z. B. <c>BAT-01</c>.</param>
/// <param name="ClientKind">Art des Programms, z. B. <c>Battery</c> oder <c>Weather</c>.</param>
/// <param name="ProtocolVersion">Muss <see cref="SyncProtocol.Version"/> entsprechen.</param>
public sealed record HelloMessage(string ClientId, string ClientKind, int ProtocolVersion) : SyncMessage;

/// <summary>
/// Master → Simulator: alle ab jetzt gültigen Zeitzustände (gültiger + angekündigte), aufsteigend nach Sequenz.
/// Wird nach dem Hello, bei jeder Änderung und als Heartbeat (Standard: jede Sekunde) gesendet.
/// </summary>
public sealed record StatesMessage(IReadOnlyList<TimeState> States) : SyncMessage;

/// <summary>
/// Simulator → Master: Messung für den Uhrenabgleich, zugleich Statusmeldung des Simulators.
/// Der Simulator merkt sich seine lokale Sendezeit selbst unter <paramref name="Id"/>.
/// </summary>
/// <param name="SimTime">Virtuelle Zeit des Simulators beim Senden (für die Anzeige im Master).</param>
/// <param name="SimUncertaintySeconds">Größtmögliche Abweichung zum Master in Sim-Sekunden (RTT/2 · Tempo).</param>
/// <param name="SyncStatus">Verbindungszustand aus Sicht des Simulators (<c>Synced</c>, <c>MasterLost</c>, …).</param>
public sealed record PingMessage(long Id, DateTime? SimTime, double? SimUncertaintySeconds, string? SyncStatus) : SyncMessage;

/// <summary>Master → Simulator: Antwort auf <see cref="PingMessage"/> mit der Uhrzeit des Masters.</summary>
public sealed record PongMessage(long Id, DateTimeOffset MasterTime) : SyncMessage;

/// <summary>Beide Richtungen: geordnetes Trennen (Programmende oder abgelehnte Verbindung), mit Grund.</summary>
public sealed record ByeMessage(string Reason) : SyncMessage;
