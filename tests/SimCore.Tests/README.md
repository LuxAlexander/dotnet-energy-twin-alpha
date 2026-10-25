# SimCore.Tests

**Zweck:** Pflicht-Tests der Zeitsteuerung (Pause, Fortsetzen, Tempo, Neustart), der Zeitsynchronisation der Geräte und des TCP-Transports.
**Verantwortlich:** _offen_ (wie `SimCore.Time`). Am 2026-10-25 von Susanne Lastowicka erstellt.
**Basis aus LB01:** –

## Ausführen

```
dotnet test tests/SimCore.Tests
```

Die Tests der Zeitlogik warten nie real: `ManualTimeProvider` ist eine Test-Uhr, die per `Advance(…)` weitergedreht wird
(`JumpWallClock(…)` verstellt nur die Systemuhr). Dadurch sind alle Ergebnisse exakt und reproduzierbar.
Nur `TimeSyncIntegrationTests` laufen mit der realen Uhr über TCP (ca. 4 s) und warten per `Task.Delay` auf Zustände, höchstens 10 s.

## Tests

| Datei | Prüft |
|---|---|
| `MasterClockTests` | Start pausiert · Fortsetzen erst nach Vorlaufzeit · Pause ohne Sprung · Fortsetzen an derselben Zeit · Tempowechsel ohne Sprung · Neustart mit neuer Zeit und neuem Lauf (auch pausiert) · ungültiges Tempo abgelehnt · doppelte Befehle ohne neuen Zustand · Sequenz steigt · Befehle innerhalb der Vorlaufzeit bleiben stetig · Systemuhr-Sprung ohne Wirkung · `CurrentStates` enthält angekündigte Änderung |
| `SyncedClockTests` | Unterschiedliche Laufzeiten ⇒ gleiche Zeit bei allen Geräten · Pause und Neustart wirken gleichzeitig · verspätete Nachricht wird übernommen und gemeldet · alte/doppelte Zustände ignoriert · neues Gerät während einer angekündigten Änderung · neu gestarteter Master ersetzt den Verlauf · Master verloren ⇒ Zeit steht, danach wieder synchron · Uhrenabgleich korrigiert 5 s Uhrenunterschied · bei asymmetrischer Laufzeit bleibt der Fehler innerhalb `SimUncertainty` |
| `SyncProtocolTests` | Alle Nachrichtenarten überstehen Hin- und Rückweg · JSON ist lesbar (`type`, Enum als Text) · ungültige Nachrichten (kein JSON, unbekannter Typ, fehlende Felder, Tempo 0, unbekannter Modus) werden abgelehnt |
| `TimeSyncIntegrationTests` | Echtes TCP auf localhost, reale Uhr, Tempo 600: zwei Simulatoren folgen Start, Tempowechsel, Pause (exakt gleich) und Neustart innerhalb der Toleranz (10 ms · Tempo) · Master listet Simulatoren mit gemessener Abweichung · Simulator vor dem Master gestartet verbindet sich später · Master beendet ⇒ Meldung und Gerätezeit steht · falsche Protokollversion wird mit `bye` abgelehnt · ungültige Nachricht trennt nur diese Verbindung |
| `ClockOffsetEstimatorTests` | Ohne Messung Offset 0 · kleinste RTT gewinnt · alte Messungen fallen aus dem Fenster · unmögliche Messung abgelehnt |

Ergebnis am 2026-10-25: 54 von 54 bestanden, 5 Wiederholungen ohne Ausfall.

## Bekannte Probleme
- Die Integrationstests brauchen freie Ports auf localhost (werden vom Betriebssystem vergeben).
