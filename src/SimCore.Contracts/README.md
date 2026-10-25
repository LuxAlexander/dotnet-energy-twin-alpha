# SimCore.Contracts

**Zweck:** Gemeinsame Verträge (Interfaces und DTOs) aller Programme. Keine Abhängigkeiten, keine Logik außer reinen Formeln.
**Verantwortlich:** _offen_ (laut `AGENTS1.md` Alexander Lux). Zeitvertrag am 2026-10-25 von Susanne Lastowicka umgesetzt.
**Basis aus LB01:** –

## Inhalt

| Typ | Art | Bedeutung |
|---|---|---|
| `TimeState` | record | Vertrag der virtuellen Zeit (siehe unten) |
| `SimMode` | enum | `Paused`, `Running` |
| `SyncMessage` (`HelloMessage`, `StatesMessage`, `PingMessage`, `PongMessage`, `ByeMessage`) | records | Nachrichten der Zeitsynchronisation, Ablauf siehe `src/SimCore.Time/README.md` und `src/SimCore.TimeSync/README.md` |
| `SyncProtocol` | static | Protokollversion (1), Standard-Port 5100, JSON-(De-)Serialisierung mit Prüfung (`InvalidDataException`) |
| `ISimClock` | interface | Lesender Zugriff auf die virtuelle Zeit (`Now`, `Mode`, `Speed`, `RunId`). Ersetzt das in `AGENTS1.md` geplante `ITimeProvider` (Verwechslungsgefahr mit `System.TimeProvider`). |

Noch offen: `IWeatherService`, `WeatherData`, `DeviceConfig`.

## Zeitvertrag `TimeState`

Ein `TimeState` beschreibt eine **Gerade**, keinen Zeitpunkt:

```
ab Master-Uhrzeit EffectiveAt gilt:
  Running:  SimZeit = SimAnchor + (MasterZeit − EffectiveAt) · Speed
  Paused:   SimZeit = SimAnchor
```

| Feld | Typ | Bedeutung |
|---|---|---|
| `MasterSession` | `Guid` | Kennung des Master-Prozesses. Neue Kennung (Master neu gestartet) ⇒ Gerät verwirft den alten Verlauf. |
| `RunId` | `int` | Simulationslauf, steigt bei jedem Neustart (auch Teil des Wetter-Seeds). |
| `Sequence` | `long` | Steigt bei jeder Änderung. Ältere oder gleiche Werte werden ignoriert (Heartbeat). |
| `Mode` | `SimMode` | Läuft / pausiert. |
| `Speed` | `double` | Virtuelle Sekunden je realer Sekunde (1 … 100 000). |
| `SimAnchor` | `DateTime` | Virtuelle Zeit zum Zeitpunkt `EffectiveAt`. |
| `EffectiveAt` | `DateTimeOffset` | Uhrzeit des Masters (UTC), ab der der Zustand gilt; bei Änderungen *jetzt + Vorlaufzeit*. |

`TimeState.SimTimeAt(masterTime)` berechnet die Formel. Master und Geräte verwenden genau diese Methode.

## Konfiguration / Bedienung
Keine (Bibliothek).

## Tests
Indirekt über `tests/SimCore.Tests`.

## Bekannte Probleme
- Keine. Das Transportformat ist seit 2026-10-25 festgelegt: JSON-Zeilen über TCP (`SyncProtocol`).
