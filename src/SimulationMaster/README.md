# SimulationMaster

**Zweck:** Besitzt die virtuelle Zeit des Gesamtsystems und verteilt sie per TCP an alle Simulatoren: Start, Pause,
Fortsetzen, Tempo, Neustart mit neuer Zeit, Statusanzeige mit allen verbundenen Simulatoren.
**Verantwortlich:** Susanne Lastowicka.
**Basis aus LB01:** Idee der Tastensteuerung (Alex `SimulationEngine`), flackerfreie Anzeige (Susi `ConsoleRenderer`).
**Abhängigkeiten:** `SimCore.Contracts`, `SimCore.Time` (`MasterClock`), `SimCore.TimeSync` (`TimeSyncServer`), `Shared.Config`.

## Start

```
dotnet run --project src/SimulationMaster -- [Optionen]
dotnet run --project src/SimulationMaster -- --speed 600 --start "2026-01-15 06:00"
```

Der Master muss **vor** den Simulatoren laufen (startet ein Simulator früher, verbindet er sich alle 2 s neu).
Er startet **pausiert**, damit alle Simulatoren in Ruhe verbinden können; los geht es mit der Leertaste
(oder `--autostart`).

## Konfiguration (Kommandozeile)

| Option | Standard | Bedeutung |
|---|---|---|
| `--bind <ip>` | `127.0.0.1` | Adresse des Zeitservers; `0.0.0.0` für Simulatoren auf anderen Rechnern |
| `--port <n>` | `5100` | TCP-Port (1024–65535) |
| `--start <zeit>` | 15.04. des laufenden Jahres, 06:00 | virtuelle Startzeit, `yyyy-MM-dd HH:mm` |
| `--speed <x>` | `60` | virtuelle Sekunden je realer Sekunde, 1–100 000 (Punkt oder Komma) |
| `--lead <ms>` | `200` | Vorlaufzeit für Änderungen, 10–5000 |
| `--heartbeat <ms>` | `1000` | Heartbeat-Intervall, 100–10 000 |
| `--tolerance <ms>` | `10` | zulässige Abweichung in realen ms; angezeigt als Sim-Toleranz = Wert · Tempo |
| `--autostart` | aus | sofort starten |
| `--help` | | Hilfe |

Ungültige Optionen → Fehlermeldung + Hilfe, Exit-Code 1. Port belegt → Fehlermeldung, Exit-Code 2.

## Bedienung

| Taste | Wirkung |
|---|---|
| Leertaste / `P` | Pause ↔ Fortsetzen (an derselben virtuellen Zeit) |
| `+` / `-` | Tempo verdoppeln / halbieren (Grenzen 1 und 100 000) |
| `T` | Tempo eingeben (Enter = übernehmen, Esc = Abbruch) |
| `N` | Neustart mit eingegebener Zeit `yyyy-MM-dd HH:mm` |
| `1`–`4` | Neustart am 15.04./15.07./15.10./15.01. 06:00 (Frühling/Sommer/Herbst/Winter) im aktuellen Sim-Jahr |
| `Q` / Strg+C | geordnet beenden: `bye` an alle Simulatoren, Verbindungen schließen |

Eingaben werden zeichenweise gelesen, die Anzeige läuft währenddessen weiter. Ungültige Eingaben erscheinen als
`FEHLER: …` in den Meldungen; der Zustand bleibt unverändert.

Anzeige (alle 250 ms): virtuelle Zeit, Zustand, Tempo, Lauf/Sequenz, angekündigte Änderung (mit Restzeit),
Zeitserver-Adresse, Liste der Simulatoren mit gemeldeter virtueller Zeit, **gemessener Abweichung Δ**, maximaler
Unsicherheit (RTT/2 · Tempo), Status und ✓/✗ gegen die Toleranz, die letzten 6 Meldungen.
Bei umgeleiteter Ausgabe: alle 5 s eine Statuszeile plus Meldungen.

## Schnittstellen
- **Angeboten:** Zeitserver nach dem Protokoll in `src/SimCore.TimeSync/README.md` (TCP, JSON-Zeilen, Port 5100).
- **Benötigt:** keine.

## Tests
Die Zeitlogik und der Server sind in `tests/SimCore.Tests` getestet (`MasterClockTests`, `TimeSyncIntegrationTests`).
Probelauf am 2026-10-25 mit umgeleiteter Ausgabe und einem PowerShell-TCP-Client: `hello` → `states`, `ping` → `pong`,
Heartbeat jede Sekunde, Abmeldung mit `bye` wird angezeigt.

## Bekannte Probleme
- Die interaktive Anzeige ist noch nicht in einem echten Terminalfenster getestet (nur umgeleitete Ausgabe).
- Strg+C wurde noch nicht manuell geprüft (Beenden über den Abbruch-Token ist durch die Tests abgedeckt).
- Δ enthält die einfache Laufzeit des Pings · Tempo (auf localhost < 1 ms real); die echte Abweichung ist eher kleiner.
