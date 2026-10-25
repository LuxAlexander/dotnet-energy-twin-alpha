# SimCore.TimeSync

**Zweck:** TCP-Transport der gemeinsamen virtuellen Zeit: `TimeSyncServer` im Simulations-Master, `TimeSyncClient` in jedem
Simulator (auch im Wettersimulator). Die Zeitlogik selbst liegt in `SimCore.Time`, das Nachrichtenformat in `SimCore.Contracts`.
**Verantwortlich:** _offen_ (wie `SimCore.Time`). Am 2026-10-25 von Susanne Lastowicka umgesetzt.
**Basis aus LB01:** –
**Abhängigkeiten:** `SimCore.Contracts`, `SimCore.Time` (nur .NET-Bibliotheken, kein NuGet).

## Protokoll (Version 1)

TCP, eine JSON-Nachricht pro Zeile (UTF-8 ohne BOM, `\n`), Feldnamen in camelCase, Feld `type` bestimmt die Art.
Standard-Port **5100** (`SyncProtocol.DefaultPort`). Der Master ist Server, die Simulatoren sind Clients.

| Richtung | `type` | Felder | Wann |
|---|---|---|---|
| Sim → Master | `hello` | `clientId`, `clientKind`, `protocolVersion` | erste Nachricht, sonst Trennung nach 5 s |
| Master → Sim | `states` | `states`: Liste von `TimeState` | sofort nach `hello`, bei jeder Änderung, als Heartbeat (1 s) |
| Sim → Master | `ping` | `id`, `simTime`, `simUncertaintySeconds`, `syncStatus` | 5× im Abstand von 100 ms nach dem Verbinden, danach jede Sekunde |
| Master → Sim | `pong` | `id`, `masterTime` | Antwort auf `ping` |
| beide | `bye` | `reason` | geordnetes Beenden, Ablehnung (falsche Version, erste Nachricht kein `hello`) |

`states` enthält immer **alle ab jetzt gültigen** Zustände (gültiger + angekündigte). Deshalb ist die Reihenfolge der
Nachrichten egal und Heartbeat und Änderungsmeldung können sich nicht gegenseitig überholen.

Beispiel (Mitschnitt vom 2026-10-25):

```
→ {"type":"hello","clientId":"PS-01","clientKind":"Smoke","protocolVersion":1}
← {"type":"states","states":[{"masterSession":"a2068931-…","runId":1,"sequence":2,"mode":"Running","speed":600,
   "simAnchor":"2026-01-15T06:00:00","effectiveAt":"2026-10-25T13:52:12.3344133+00:00"}]}
→ {"type":"ping","id":1,"simTime":null,"simUncertaintySeconds":null,"syncStatus":"WaitingForMaster"}
← {"type":"pong","id":1,"masterTime":"2026-10-25T13:52:13.6235877+00:00"}
```

## Verwendung im Simulator

```csharp
var clock = new SyncedClock(TimeProvider.System);
var sync = new TimeSyncClient(clock, host: "127.0.0.1", port: 5100, clientId: "BAT-01", clientKind: "Battery");
sync.Log += (_, message) => ui.AddMessage(message);
var syncTask = sync.RunAsync(ct);          // läuft bis ct, verbindet sich selbst neu

// in der Simulationsschleife:
if (clock.Mode == SimMode.Running) { var now = clock.Now; … }
// Anzeige: clock.Status, clock.SimUncertainty, sync.IsConnected, sync.LastError
```

## Fehlerbehandlung

| Situation | Verhalten |
|---|---|
| Master beim Start des Simulators nicht da | Client meldet „nicht erreichbar“, versucht alle 2 s erneut, `SyncedClock` bleibt `WaitingForMaster`. |
| Verbindung bricht ab / Master beendet (`bye`) | Meldung über `Log`, nach 3 s ohne Nachricht hält die Gerätezeit an (`MasterLost`), Client verbindet sich neu. |
| Simulator antwortet nicht mehr | Master zeigt ihn nach 3 s als „KEINE ANTWORT“, bei Verbindungsende als „GETRENNT“. Die anderen laufen weiter. |
| Ungültige Nachricht (kein JSON, unbekannter Typ, Tempo ≤ 0, …) | `InvalidDataException` → nur diese Verbindung wird mit Meldung „Protokollfehler“ getrennt. |
| Falsche Protokollversion | Master antwortet mit `bye` (Grund) und trennt. |
| Zwei Simulatoren mit gleicher Kennung | Neue Verbindung ersetzt die alte in der Liste, Warnmeldung im Master. |
| Senden an einen Simulator schlägt fehl | Nur diese Verbindung wird geschlossen; gesendet wird parallel, ein langsamer Simulator hält niemanden auf. |
| Beenden des Masters | `bye` an alle, dann Verbindungen schließen. |
| Beenden eines Simulators | `bye` an den Master (max. 500 ms warten). |

## Konfiguration
- `TimeSyncServer(clock, port, address = 127.0.0.1, heartbeatInterval = 1 s, clientTimeout = 3 s)`. Port 0 = frei wählen (Tests).
- `TimeSyncClient(clock, host, port, clientId, clientKind, pingInterval = 1 s, reconnectDelay = 2 s)`.

## Tests
`tests/SimCore.Tests`: `SyncProtocolTests` (Serialisierung, ungültige Nachrichten), `TimeSyncIntegrationTests`
(echtes TCP auf localhost: zwei Simulatoren folgen Start/Pause/Tempo/Neustart, Abweichung in der Master-Liste,
Simulator vor dem Master gestartet, Master beendet, falsche Version, Müll trennt nur eine Verbindung).

## Bekannte Probleme
- Keine Verschlüsselung/Authentifizierung (nur für lokales Labornetz gedacht, Standard-Bind 127.0.0.1).
- Die Zeilenlänge wird erst nach dem Lesen geprüft; eine Gegenstelle ohne Zeilenende könnte Speicher belegen.
