# EnergyTwin – verteiltes Energiesystem (LB02, Team Alpha)

Simulation eines Energiesystems aus mehreren getrennten Programmen: ein Simulations-Master gibt die virtuelle Zeit vor,
ein Wettersimulator liefert deterministisches Wetter, vier Feldgeräte (PV, Batterie, DC-Ladesäule, Kühlhaus) simulieren
Anlagen, ein Modbus-Diagnose-Client liest und schreibt Register. Alles läuft lokal, ohne Internet zur Laufzeit.

## Zuständigkeiten

| Baustein | Verantwortlich | Dokumentation |
|---|---|---|
| Simulations-Master, `SimCore.Contracts`, `SimCore.Time`, `SimCore.TimeSync` | Susanne Lastowicka | [`src/SimulationMaster/README.md`](src/SimulationMaster/README.md), [`src/SimCore.Time/README.md`](src/SimCore.Time/README.md), [`src/SimCore.TimeSync/README.md`](src/SimCore.TimeSync/README.md) |
| Wettersimulator, PV-Anlage, gemeinsame Bausteine (`Shared`) | Alexander Lux | [`src/WeatherSimulator/README.md`](src/WeatherSimulator/README.md), [`src/Devices/PvSimulator/README.md`](src/Devices/PvSimulator/README.md), [`src/Shared/README.md`](src/Shared/README.md) |
| Batteriespeicher | Dominik Zach | [`src/Devices/BatterySimulator/README.md`](src/Devices/BatterySimulator/README.md) |
| DC-Ladesäule (2 Ladepunkte), Modbus-Diagnose-Client, `Modbus.Contracts`, `Devices.Common` | Susanne Lastowicka | [`src/Devices/DualPortCharger/README.md`](src/Devices/DualPortCharger/README.md), [`src/ModbusDiagnosticClient/README.md`](src/ModbusDiagnosticClient/README.md), [`src/Modbus.Contracts/README.md`](src/Modbus.Contracts/README.md), [`src/Devices.Common/README.md`](src/Devices.Common/README.md) |
| Industrieverbraucher (Kühlhaus, viertes Feldgerät) | Dominik Zach (LB01-Modell und LB02-Umbau) | [`src/Devices/KuehlhausSimulator/README.md`](src/Devices/KuehlhausSimulator/README.md) |

## Aufbau

```
src/
  SimCore.Contracts/       DTOs und Interfaces (WeatherData, IWeatherService, TimeState, ISimClock) – keine Abhängigkeiten
  SimCore.Time/            virtuelle Zeit, Uhrenabgleich (MasterClock, SyncedClock, TimeState als Gerade)
  SimCore.TimeSync/        TCP-Zeitverteilung (JSON-Zeilen, Port 5100)
  Modbus.Contracts/        gemeinsamer Registerkopf 0–19, Registerpläne je Gerät, Codec
  Shared.Config/           Pfade (docs/data), Konfigurationshilfen
  Shared/                  geräteunabhängig: CSV-Writer, Balken, Jahreszeiten/Wetter/Einstrahlung
  Devices.Common/          Zeitanbindung, StepScheduler, Modbus-Server, Wetter-Client/Feed, Konsolenhilfen
  SimulationMaster/        Master: Pause, Tempo, Neustart, Status
  WeatherSimulator/        HTTP-Wetterdienst (deterministisch, WeatherSynth-Basis)
  ModbusDiagnosticClient/  Register lesen/schreiben, Fehler anzeigen
  Devices/PvSimulator/       PV-Anlage
  Devices/BatterySimulator/  Batteriespeicher
  Devices/DualPortCharger/   DC-Ladesäule mit zwei Ladepunkten
  Devices/KuehlhausSimulator/       Kühlhaus (Industrieverbraucher, viertes Feldgerät)
  Devices/WindSimulator/     Platzhalter (Wind noch nicht umgesetzt)
tests/
  SimCore.Tests/            Zeit, Uhrenabgleich, TCP (54)
  Devices.Common.Tests/     Gerätezeit, Modbus-Server, Registercodec, Wetter-Feed (51)
  FieldDevices.Tests/       PV, Batterie und Kühlhaus: Modbus, Optionen, Standalone-Lauf (62)
  ModbusDiagnosticClient.Tests/  Client gegen echten Modbus-Server (28)
  Weather.Tests/            Reproduzierbarkeit und Plausibilität des Wetters (11)
```

Abhängigkeitsrichtung: `SimCore.Contracts` ← `SimCore.Time` ← `SimCore.TimeSync` ← `Modbus.Contracts` ← `Devices.*`.
Keine Zyklen; nur Geräte und der Diagnose-Client verwenden EasyModbusTCP.NETCore.

## Startreihenfolge

Alle Befehle aus dem Repository-Hauptordner (`dotnet-energy-twin-alpha/`). Jedes Programm in einem eigenen Terminal.

**Ein Befehl für alles** (Windows, PowerShell 5.1 oder 7):

```powershell
# baut die Solution und öffnet sieben Fenster: Wetter, Master, PV, Batterie, Ladesäule, Kühlhaus, Diagnose-Client
.\start-all.ps1

# Alternativ mit sofort laufendem Master und ohne Diagnose-Client
.\start-all.ps1 -Autostart -NoClient
```

Das Skript baut zuerst, wartet auf Wetter (8080) und Master (5100) und startet dann die Geräte. Ohne
`-Autostart` startet der Master pausiert; die Leertaste im Master-Fenster setzt die Zeit in Gang. Ist ein Port
bereits belegt, wird das zugehörige Programm nicht doppelt gestartet. Wenn die Ausführung von Skripten gesperrt ist:

```powershell
powershell -ExecutionPolicy Bypass -File .\start-all.ps1
```

Manuell (ohne Skript), jeweils in einem eigenen Terminal:

```bash
# 1. Wettersimulator (HTTP 8080)
dotnet run --project src/WeatherSimulator

# 2. Simulations-Master (TCP 5100, startet pausiert)
dotnet run --project src/SimulationMaster -- --speed 600

# 3. Feldgeräte (Modbus-Ports ab 1502, je Gerät eigener Port)
dotnet run --project src/Devices/PvSimulator -- --id PV-01
dotnet run --project src/Devices/BatterySimulator -- --id BAT-01 --modbus-port 1503
dotnet run --project src/Devices/DualPortCharger -- --id DC-01 --modbus-port 1504
dotnet run --project src/Devices/KuehlhausSimulator -- --id KH-01

# 4. Modbus-Diagnose-Client
dotnet run --project src/ModbusDiagnosticClient -- --port 1502
```

Die Standardports sind 1502 (PV und Charger), 1503 (Batterie) und 1505 (Kühlhaus). In der Demo mit allen Geräten
gleichzeitig bekommt der Charger 1504, sonst kollidiert er mit der PV.

Im Master mit **Leertaste** starten; danach `+`/`-` für das Tempo, `1`–`4` für Jahreszeiten-Neustart, `Q` zum Beenden.
Die Geräte verbinden sich beim Start mit `127.0.0.1:5100` und wiederholen das automatisch, solange der Master fehlt.

### Demo-Ablauf

1. Wetter und Master starten, Master mit Leertaste fortsetzen.
2. Feldgeräte starten: Anzeige zeigt „synchron“, Tempo und Modellzeit des Masters.
3. Im Master Tempo ändern (`+`/`-`) und pausieren – alle Geräte folgen ohne Zeitsprung, Modbus bleibt erreichbar.
4. Client: `read`, `write leistungsgrenze 60`, im PV-Gerät prüfen; `watch` zeigt wechselnde Werte.
5. Master `N` oder `1`–`4`: alle Geräte übernehmen die neue virtuelle Zeit (neuer Lauf).
6. Wettersimulator beenden ⇒ PV, Batterie, Ladesäule und Kühlhaus melden nach 3 Fehlversuchen sichtbar „WETTER: NICHT ERREICHBAR“, letztes
   Wetter bleibt.
7. Kühlhaus: `T` öffnet die Tür (Temperatur steigt), `S` ändert die Solltemperatur, `F` löst einen Kompressorausfall
   aus, `R` quittiert ihn; der Diagnose-Client `--port 1505` zeigt dieselben Werte.
8. Alle Programme mit `Q` bzw. Strg+C beenden.

## Schnittstellen

| Schnittstelle | Transport | Standard | Details |
|---|---|---|---|
| Zeitverteilung Master → Geräte | TCP, JSON je Zeile | `127.0.0.1:5100` | [`src/SimCore.TimeSync/README.md`](src/SimCore.TimeSync/README.md) |
| Wetter | HTTP GET `/weather` | `http://127.0.0.1:8080` | [`src/WeatherSimulator/README.md`](src/WeatherSimulator/README.md) |
| Geräteregister | Modbus TCP, Holding Registers | `127.0.0.1:1502` ff. | Registerplan im README des jeweiligen Geräts |

**Registerkopf (Adressen 0–19) für alle Geräte:** Typ, Geräte-Nummer, Zustand, Fehlercode, Leistung (Float32, positiv = Aufnahme),
kumulierte Energie, Modellzeit (UInt32, Sekunden seit 1970), Lauf, Sequenz, Zeitstatus, läuft, Tempo, Ein/Aus (schreibbar),
Reset (schreibbar), letzter Schreibfehler und dessen Adresse. Gerätespezifische Register ab 20.
Mehrteilige Werte: zwei Register, höherwertiges Wort zuerst. Details: [`src/Modbus.Contracts/README.md`](src/Modbus.Contracts/README.md).

## Konfiguration

Alle Programme werden über Kommandozeilenoptionen konfiguriert (`--help` zeigt die jeweilige Hilfe). Es gibt **keine
Umgebungsvariablen und keine Secrets**; `.env`-Dateien werden nicht benötigt. CSV-Ausgaben landen standardmäßig in
`docs/data/` (im Repo, unabhängig vom Startordner).

## Tests

```bash
dotnet build EnergyTwin.sln
dotnet test tests/SimCore.Tests
dotnet test tests/Devices.Common.Tests
dotnet test tests/FieldDevices.Tests
dotnet test tests/ModbusDiagnosticClient.Tests
dotnet test tests/Weather.Tests
```

Die Testprojekte werden **einzeln** gestartet: Mehrere binden echte TCP-Ports, deshalb kann `dotnet test EnergyTwin.sln`
wegen der parallelen Ausführung scheitern, obwohl der Code in Ordnung ist. Zusammen sind es 206 Tests.

Pflicht-Tests laut Aufgabenstellung: Zeitsteuerung des Masters (`tests/SimCore.Tests`) und Reproduzierbarkeit des
Wetters (`tests/Weather.Tests`). Dazu Gerätezeit/Modbus (`Devices.Common.Tests`), PV/Batterie/Kühlhaus
(`FieldDevices.Tests`) und der Diagnose-Client (`ModbusDiagnosticClient.Tests`).

## Entwicklungsjournale

`docs/development-journal/alexander-lux.md`, `dominik-Zach.md`, `susanne-lastowicka.md`.
Teamregeln und Architekturdetails: [`AGENTS.md`](AGENTS.md).

## Bekannte Einschränkungen

- Der Wind-Simulator ist noch nicht umgesetzt (Platzhalter); das Kühlhaus ist das vierte Feldgerät. Das Wetter ist
  für PV, Batterie, Ladesäule und Kühlhaus angebunden; ohne Wettersimulator rechnen sie mit festen Werten
  (PV lokales Wetter, sonst 20 °C, Kühlhaus lokales Tagesprofil).
- Die Registerpläne stehen in den READMEs der Geräte; Änderungen daran mit den jeweiligen Verantwortlichen abstimmen.
- Interaktive Anzeigen wurden bisher überwiegend mit umgeleiteter Ausgabe geprüft; echte Terminalfenster sind zu testen.
- Das Abgabe-Tag `lb02-2026` ist noch nicht gesetzt (setzt Alexander laut `docs/adr/0004-prozess-abgabe.md`).
