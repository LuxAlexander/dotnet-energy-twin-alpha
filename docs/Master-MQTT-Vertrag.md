# Master-Steuerung über MQTT

Stand: 2026-11-10. Umsetzung: Pause/Fortsetzen, Tempo, neuer Lauf und Master-Status; keine Gerätebefehle.
Gemeinsame DTOs: `src/SimCore.Contracts/MasterControl.cs`. JSON verwendet camelCase-Felder und
Enum-Namen `Paused`/`Running`. Zeitstempel im ISO-8601-Format; reale Fristen in UTC.

| Topic | Absender → Empfänger | Inhalt |
|---|---|---|
| `energytwin/master/status` | Master → Web | `sentAt`, `simTime`, `current`, `latest` |
| `energytwin/master/commands` | Web → Master | `commandId`, `masterSession`, `expectedSequence`, `action`, `expiresAt`, optional `speed`, `startTime` |
| `energytwin/master/results` | Master → Web | `commandId`, `masterSession`, `accepted`, `message`, `state` |

Alle Nachrichten: QoS 1, Retain aus. Clients verwenden saubere Sitzungen ohne dauerhafte Offline-Warteschlange.
Ein Master je Broker/Topic-Baum wird vorausgesetzt. Keine zusätzlichen Feldgeräte-MQTT-Verbindungen.

`current` ist der wirksame `TimeState`, `latest` der zuletzt angekündigte, eventuell noch zukünftige Zustand.
`simTime` ist die beim Versand berechnete virtuelle Zeit. Status folgt jede reale Sekunde, auch während Pause.
Die Oberfläche extrapoliert keine globale Zeit aus Gerätedaten. Ohne frischen Status (vier Sekunden) sperrt sie
Befehle. Bei Reconnect müssen neue Nachrichten eintreffen; Retain-Nachrichten bestätigen keinen Live-Status.

`action` erlaubt `pause`, `resume`, `speed` und `restart`. `speed` verlangt `speed` (1–100000) ohne `startTime`;
`restart` verlangt `startTime` ohne `speed`; `pause` und `resume` verlangen beides nicht. Ein Tempowechsel ändert
den Modus nicht, eine pausierte Simulation bleibt pausiert. `restart` erhöht den Lauf, startet sofort und lässt
Gerätezustände unverändert. Jahreszeiten-Vorlagen sind reine Eingabehilfen der Oberfläche und kein eigener Befehl.
Der Webdienst vergibt eine UUID und fünf Sekunden Gültigkeit.
Der Master akzeptiert höchstens zehn Sekunden Restgültigkeit. Abgelaufene, unbekannte oder auf eine andere
Sitzung/Sequenz bezogene Befehle werden abgelehnt. Sitzung und Sequenz werden innerhalb derselben
MasterClock-Sperre wie die Änderung geprüft, auch gegenüber Konsolenzugriffen.

Doppelte identische Befehle erhalten innerhalb ihrer Gültigkeit dasselbe Ergebnis; ein doppelt zugestellter
Neustart erhält das gespeicherte Ergebnis und löst keinen zweiten Lauf aus. Dieselbe Kennung mit
anderem Inhalt wird abgelehnt. Der Cache hält maximal 1024 gültige Ergebnisse; weitere Befehle werden
abgelehnt. Nach Ablauf wird der Eintrag verworfen, ein späteres Duplikat ist dann abgelaufen. Master-Neustarts
ändern die Sitzung, sodass alte Befehle weiterhin nicht übernommen werden. Keine automatische Wiederholung
im Webdienst. Ein Auftrag gleichzeitig pro Webdienst; andere Webdienste werden durch die Zustandsprüfung
abgesichert.

`accepted: true` bestätigt die Annahme und liefert den angekündigten Zustand mit `effectiveAt` zurück.
Die tatsächliche Anzeige folgt weiterhin dem Status. Nach sechs Sekunden ohne eindeutige Rückmeldung
oder bei Transportfehler lautet das Ergebnis ausdrücklich unklar; der Befehl könnte umgesetzt worden sein.
Ein Broker-Publish allein gilt niemals als Master-Bestätigung.

Grenzen: lokaler vertrauenswürdiger Broker und gemeinsame reale Uhr werden vorausgesetzt. Keine
Benutzerberechtigungen oder kryptografische Absenderprüfung im Nutzdatenvertrag. Gerätesynchronisation,
Wetter- und Gerätebedienung sind nicht Teil dieser Erweiterung.
