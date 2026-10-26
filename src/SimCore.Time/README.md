# SimCore.Time

**Zweck:** Logik der gemeinsamen virtuellen Zeit für Master und Geräte: Pause, Fortsetzen und Tempowechsel ohne Zeitsprung,
Neustart mit neuer Zeit, Uhrenabgleich, Erkennen eines verlorenen Masters. Kein Netzwerk, keine Konsole.
**Verantwortlich:** Alexander Lux. Am 2026-10-25 von Susanne Lastowicka umgesetzt.
**Basis aus LB01:** Tempo-Grenzen 1–100 000 (Susi); Idee „Pause ohne Zeitsprung“ (Alex/Domi), hier über Anker statt `_lastTick`.
**Abhängigkeiten:** `SimCore.Contracts`.

## Grundidee

Das Problem: Schickt der Master „jetzt ist 12:00“, ist die Nachricht beim Gerät schon um die Laufzeit `d` alt.
Der Fehler ist `d · Tempo` (1 ms bei Tempo 600 = 0,6 s Sim-Zeit) und ist bei jedem Gerät anders.

Die Lösung in drei Bausteinen:

1. **Gerade statt Zeitpunkt:** Der Master verschickt `TimeState` (siehe `SimCore.Contracts`). Jedes Gerät rechnet
   die virtuelle Zeit selbst aus. Die Laufzeit der Nachricht verändert das Ergebnis nicht.
2. **Vorlaufzeit:** Jede Änderung gilt erst ab *jetzt + 200 ms* (`MasterClock.LeadTime`). `SimAnchor` ist die
   virtuelle Zeit der **alten** Geraden zu diesem Zeitpunkt. Alle Geräte schalten gleichzeitig um, ohne Sprung.
   Kommt eine Nachricht zu spät, rechnet das Gerät trotzdem richtig weiter. Der kleine Sprung (`Verspätung · Tempo`)
   wird in `SyncedClock.LastLateness` gemeldet.
3. **Uhrenabgleich (Cristian/NTP):** Gerät sendet Ping (lokal `t0`), Master antwortet mit `MasterNow`, Gerät empfängt (`t1`).
   `Offset = Master − (t0 + RTT/2)`, Fehler ≤ RTT/2. Von den letzten 8 Messungen zählt die mit der kleinsten RTT.
   Auf einem Rechner ist der Offset ≈ 0.

Alle Uhren sind **monoton** (`MonotonicUtcClock`: Start-UTC + Stoppuhr). Eine Umstellung der Systemuhr verändert
die virtuelle Zeit nicht.

## Klassen

| Klasse | Seite | Aufgabe |
|---|---|---|
| `MasterClock` | Master | Besitzt die Zeit. `Resume()` (auch Start), `Pause()`, `SetSpeed(x)`, `Restart(neueZeit, startPaused)`. Event `StateChanged` zum Verteilen, `Latest` als Heartbeat, `CurrentStates` für neu verbundene Geräte, `MasterNow` für Pong-Antworten. Startet **pausiert**. |
| `SyncedClock` | Gerät | `ApplyState(state)` (Änderung oder Heartbeat), `AddSyncSample(t0, masterNow, t1)`, `Now`, `Mode`, `Status`, `SimUncertainty`, `LastLateness`. |
| `TimeLine` | beide | Liste aus gültigem und angekündigten `TimeState`s, wählt den zur Uhrzeit passenden. |
| `ClockOffsetEstimator` | Gerät | Uhrenabgleich (siehe oben). |
| `MonotonicUtcClock` | beide | Sprungfreie UTC-Uhr auf Basis von `TimeProvider`. |

Alle Klassen bekommen die reale Uhr als `System.TimeProvider` (Produktion: `TimeProvider.System`, Tests: eigene Test-Uhr).

## Verhalten in Sonderfällen

| Fall | Verhalten |
|---|---|
| Gleicher Befehl zweimal (z. B. 2× Pause) | Kein neuer Zustand, kein Event. |
| Tempo außerhalb 1 … 100 000, `NaN`, ∞ | `ArgumentOutOfRangeException`, Zustand unverändert. |
| Mehrere Befehle innerhalb der Vorlaufzeit | `EffectiveAt` wird nie früher als beim Vorgänger; die Zeit bleibt stetig. |
| Neues Gerät verbindet sich | Bekommt `CurrentStates` (auch angekündigte Änderungen) und rechnet sofort richtig. |
| Nachricht kommt nach `EffectiveAt` an | Wird übernommen, Sprung = `LastLateness · Tempo`, sichtbar zu melden. |
| Länger als `HeartbeatTimeout` (3 s) nichts vom Master | `Status = MasterLost`, `Mode = Paused`, `Now` bleibt beim Stand des Timeouts stehen. |
| Master wieder erreichbar | Erster Heartbeat ⇒ `Synced`, Gerät springt auf die Master-Zeit (gewollt, sichtbar). |
| Master-Prozess neu gestartet | Neue `MasterSession` ⇒ Gerät verwirft den alten Verlauf. |

## Toleranz (Vorschlag, im Team zu bestätigen)

Abweichung eines Geräts zum Master ≤ `RTT/2 · Tempo` (`SyncedClock.SimUncertainty`), solange Nachrichten innerhalb
der Vorlaufzeit ankommen. Vorschlag: **10 ms reale Zeit**, also zulässige Sim-Abweichung = 10 ms · Tempo
(Tempo 600 ⇒ 6 s). Begründung: Ein Geräteschritt umfasst bei diesem Tempo mehrere Sim-Minuten; Abweichungen von
wenigen Sekunden verändern Leistung und Energie nicht sichtbar. Nachweis: Das Gerät zeigt RTT und `SimUncertainty` an.

## Konfiguration
- `MasterClock`: Startzeit, Tempo (Standard 1), Vorlaufzeit (Standard 200 ms, muss größer als die längste Nachrichtenlaufzeit sein).
- `SyncedClock`: Heartbeat-Timeout (Standard 3 s, also ca. 3 verpasste Heartbeats bei 1 s Takt).

## Bedienung / Start
Bibliothek, wird vom `SimulationMaster` und über `SimCore.TimeSync` von `Devices.Common` verwendet.

## Tests
`tests/SimCore.Tests`: `dotnet test tests/SimCore.Tests` (Zeitlogik: 33 Tests, alle grün am 2026-10-25).

## Bekannte Probleme
- Transport: siehe `SimCore.TimeSync` (seit 2026-10-25).
- Ein `TimeControlCmd` (Befehle von außen an den Master) gibt es noch nicht, der Master ruft die Methoden direkt auf.
