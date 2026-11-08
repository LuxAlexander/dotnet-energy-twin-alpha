# Archiv: Projekt-README-Abgleich

**Archiviert am:** 2026-11-08
**Ausgangsstand:** f28445b17530868fbf51af7104a0f1ffecf685ef einschließlich vorhandener uncommittierter Änderungen.
Die folgenden Originalabschnitte wurden vor der Bearbeitung gesichert. Links innerhalb der Originale beziehen sich
weiterhin auf ihre jeweilige Quelldatei. Historische Aussagen sind keine aktuellen Projektanweisungen.

---

**Quelle:** `README.md`, Abschnitt „EnergyTwin – verteiltes Energiesystem (LB02/LB03, Team Alpha)“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise; Vom Nutzer bestätigte Wetterzuständigkeit; bestehende Modulzuständigkeiten; Vom Nutzer bestätigte Zuständigkeit nach Fachbereich; Doppelten Wetter-Eintrag mit dem bereits umgesetzten Wetterstrom zusammengeführt; TimeSim bleibt offen
**Aktueller Ort:** [README](../../README.md)

### Unveränderter Originalabschnitt

# EnergyTwin – verteiltes Energiesystem (LB02/LB03, Team Alpha)

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

### Aufgabe 03 – Modbus zu RabbitMQ (neue Module)

Entschieden: Übertragung per **MQTT** (RabbitMQ-MQTT-Plugin, Port 1883); als Viewer dient ein MQTT-Werkzeug
(z. B. MQTT Explorer), ein eigener Viewer ist damit nicht nötig. Leere Felder sind noch nicht zugeteilt.

| Baustein | Verantwortlich | Dokumentation |
|---|---|---|
| Modbus–RabbitMQ-Gateway, `Modbus.Client` | Susanne Lastowicka | [`src/ModbusRabbitGateway/README.md`](src/ModbusRabbitGateway/README.md), [`src/Modbus.Client/README.md`](src/Modbus.Client/README.md) |
| Nachrichtenvertrag (Format, Topics, Qualitätsstatus, Beispielnachricht) | Susanne Lastowicka | [`docs/Nachrichtenvertrag-LB03.md`](docs/Nachrichtenvertrag-LB03.md) (Entwurf) |
| Wetter zusätzlich über MQTT, Klimaprofile Sahara/Antarktis | Alexander Lux | [`Wettervertrag`](docs/Wetter-Nachrichtenvertrag-LB03.md), [`Nachweis`](docs/nachweise/2026-11-08-wetter-mqtt/README.md) |
| Messwerte im Vertrag: DC-Ladesäule | Susanne Lastowicka | |
| Messwerte im Vertrag: PV-Anlage | | |
| Messwerte im Vertrag: Batteriespeicher | | |
| Messwerte im Vertrag: Kühlhaus | | |
| SRS `docs/SRS-LB03.md` | | [`docs/SRS-LB03.md`](docs/SRS-LB03.md) (Entwurf mit Gateway-Teil) |
| RabbitMQ unter Docker (Compose, `.env.example`, Start/Status/Stopp) | Alexander Lux | [`docker/README.md`](docker/README.md), lokaler Lauf mit allen vier Geräten geprüft |
| Viewer: Einrichtung und Anleitung MQTT-Werkzeug | | |
| Funktionsnachweise 1–7 | | [`docs/nachweise/`](docs/nachweise/2026-11-08-gateway-mit-master/README.md) (Lauf mit Ladesäule und Master) |
| Optional: Wetter/TimeSim über RabbitMQ | | |
| Eigener Skill `readTask` | Susanne Lastowicka | [`skills/readTask/README.md`](skills/readTask/README.md) |
| Eigener Skill „Let's Document“ | Alexander Lux | [`skills/lets-document/README.md`](skills/lets-document/README.md), [Anwendungsnachweis](docs/nachweise/2026-11-08-lets-document/README.md) |
| Eigener Skill | Dominik Zach | |

## LB03 starten

Einrichtung und vollständige Startfolge: [`docker/README.md`](docker/README.md).
Nach einmaliger Einrichtung von `docker/.env`, aus dem Repository-Root:

```powershell
docker compose --env-file docker/.env -f docker/docker-compose.yml up -d --wait --wait-timeout 180
.\start-all.ps1 -Autostart -NoClient -WeatherMqtt
# Anschließend in einem weiteren Terminal:
dotnet run --no-build --project src/ModbusRabbitGateway
```

Management: <http://localhost:15672>; MQTT: `127.0.0.1:1883`, Abo `energytwin/#`.
Zugangsdaten stehen ausschließlich in der lokalen `docker/.env`.
`-WeatherMqtt` aktiviert den zusätzlichen Wetterstrom für Salzburg, Wien, Innsbruck, Tamanrasset und Neumayer III.
Geräte verwenden weiterhin HTTP. Für gespeicherte Wetterstände im Viewer die sechs konkreten Topics aus dem
[Wettervertrag](docs/Wetter-Nachrichtenvertrag-LB03.md) abonnieren. Ohne `-WeatherMqtt` bleibt der bisherige Wetterstart erhalten.
Der [Docker-Funktionsnachweis](docs/nachweise/2026-11-08-docker-alexander/README.md) belegt den Empfang aller vier Geräte.

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
  Modbus.Client/           Modbus-TCP-Client (Verbindung, Fehlerübersetzung) für Diagnose-Client und Gateway
  ModbusRabbitGateway/     LB03: Geräte über Modbus lesen, nach Nachrichtenvertrag per MQTT an RabbitMQ veröffentlichen
  Devices/PvSimulator/       PV-Anlage
  Devices/BatterySimulator/  Batteriespeicher
  Devices/DualPortCharger/   DC-Ladesäule mit zwei Ladepunkten
  Devices/KuehlhausSimulator/       Kühlhaus (Industrieverbraucher, viertes Feldgerät)
tests/
  SimCore.Tests/            Zeit, Uhrenabgleich, TCP (54)
  Devices.Common.Tests/     Gerätezeit, Modbus-Server, Registercodec, Wetter-Feed (51)
  FieldDevices.Tests/       PV, Batterie und Kühlhaus: Modbus, Optionen, Standalone-Lauf (62)
  ModbusDiagnosticClient.Tests/  Client gegen echten Modbus-Server (28)
  Weather.Tests/            Reproduzierbarkeit und Plausibilität des Wetters (11)
  ModbusRabbitGateway.Tests/  Gateway: Dekodierung, Qualität, Schlüssel, Optionen, gegen echten Modbus-Server
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

# 5. Modbus–RabbitMQ-Gateway (LB03, liest PV 1502, Batterie 1503, Charger 1504, Kühlhaus 1505 → MQTT 127.0.0.1:1883)
#    Zugangsdaten über ENERGYTWIN_MQTT_USER/ENERGYTWIN_MQTT_PASSWORD oder docker/.env; ohne Broker: --console
dotnet run --project src/ModbusRabbitGateway
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
dotnet test tests/ModbusRabbitGateway.Tests
```

Die Testprojekte werden **einzeln** gestartet: Mehrere binden echte TCP-Ports, deshalb kann `dotnet test EnergyTwin.sln`
wegen der parallelen Ausführung scheitern, obwohl der Code in Ordnung ist. Zusammen sind es 258 Tests (Stand 2026-11-08, alle grün).

Pflicht-Tests laut Aufgabenstellung: Zeitsteuerung des Masters (`tests/SimCore.Tests`) und Reproduzierbarkeit des
Wetters (`tests/Weather.Tests`). Dazu Gerätezeit/Modbus (`Devices.Common.Tests`), PV/Batterie/Kühlhaus
(`FieldDevices.Tests`) und der Diagnose-Client (`ModbusDiagnosticClient.Tests`).

## Entwicklungsjournale

`docs/development-journal/alexander-lux.md`, `dominik-Zach.md`, `susanne-lastowicka.md`.
Teamregeln und Architekturdetails: [`AGENTS.md`](AGENTS.md).

## Bekannte Einschränkungen

- Das Kühlhaus ist das vierte Feldgerät. Das Wetter ist
  für PV, Batterie, Ladesäule und Kühlhaus angebunden; ohne Wettersimulator rechnen sie mit festen Werten
  (PV lokales Wetter, sonst 20 °C, Kühlhaus lokales Tagesprofil).
- Die Registerpläne stehen in den READMEs der Geräte; Änderungen daran mit den jeweiligen Verantwortlichen abstimmen.
- Interaktive Anzeigen wurden bisher überwiegend mit umgeleiteter Ausgabe geprüft; echte Terminalfenster sind zu testen.
- Das Abgabe-Tag `lb02-2026` ist noch nicht gesetzt (setzt Alexander laut `docs/adr/0004-prozess-abgabe.md`).

---

**Quelle:** `src/Devices/BatterySimulator/README.md`, Abschnitt „Batteriespeicher-Simulator“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise; Veraltetes LB01-Pausenmenü durch belegte SettingsMenuAsync-Bedienung ersetzt; Gemeinsame Enum-Legende um bereits implementierten Gerätetyp 5 ergänzt; Registeradressen und Fachwerte unverändert; Weiterhin gültige CSV-Dopplung im maßgeblichen Abschnitt zusammengeführt
**Aktueller Ort:** [README](../../src/Devices/BatterySimulator/README.md)

### Unveränderter Originalabschnitt

# Batteriespeicher-Simulator

Konsolenanwendung in C# (.NET), die einen **Batteriespeicher** als eigenständiges, simuliertes Gerät abbildet. Der Speicher wird mit Leistungsvorgaben geladen und entladen, der Ladezustand (SOC) wird über die Simulationszeit berechnet. Im Gesamtprojekt wird er später mit der PV-Anlage und der Ladestation zusammengeführt. Dieses Programm simuliert das Gerät einzeln.

## Voraussetzungen

- **.NET 10 SDK** (<https://dotnet.microsoft.com/download>). Prüfen mit `dotnet --version`.
- Windows-Konsole (Visual-Studio-Debugging-Konsole, Windows Terminal oder Eingabeaufforderung), mindestens 80 Zeichen breit und 15 Zeilen hoch.
- Die Konsole muss **interaktiv** sein (Tastatureingaben). Die Eingabe darf nicht umgeleitet werden.
- Teil der Solution `EnergyTwin.sln` im Repository-Hauptordner. Das Projekt verweist auf `src/Devices.Common`
  (bringt EasyModbusTCP.NETCore mit, wird beim ersten Build aus NuGet geladen). Der LB01-Code nutzt das noch nicht.

## Starten

Aus dem Repository-Hauptordner:

```
dotnet run --project src/Devices/BatterySimulator -- <Argumente>
```

oder im Projektordner `src/Devices/BatterySimulator`:

```
dotnet run -- <Argumente>
```

Beispiel (im Projektordner):

```
dotnet run -- --id BAT-01 --capacity 10 --start 06:00
```

Die weiteren Beispiele in dieser Datei gehen vom Projektordner aus.

In Visual Studio: `EnergyTwin.sln` öffnen, `BatterySimulator` als Startprojekt festlegen, dann *Projekt → Eigenschaften → Debuggen → Befehlszeilenargumente*, dort die Argumente eintragen (ohne `dotnet run --`).

> **Ab LB02** kommt die Zeit vom Simulations-Master (`--master`). Die Aufrufe unten (LB01) laufen nur noch mit `--standalone`; `--speed`, `--start` und
> `--duration` gelten nur dort, `--start` jetzt mit Datum (`"2026-01-15 06:00"`). Siehe Abschnitt „LB02“ am Ende.

Ohne Argumente (und ohne Master) wartet das Gerät auf den Simulations-Master; mit `--standalone` läuft es mit den Standardwerten auf einer lokalen Uhr.

### Kommandozeilenargumente

| Argument | Bedeutung | Standard |
|---|---|---|
| `--id` | Geräte-ID (auch im CSV-Dateinamen) | BAT-01 |
| `--capacity` | Kapazität in kWh | 20 |
| `--max-charge` | max. Ladeleistung in kW | 5 |
| `--max-discharge` | max. Entladeleistung in kW | 5 |
| `--soc-min` | SOC-Untergrenze in % | 10 |
| `--soc-max` | SOC-Obergrenze in % | 100 |
| `--soc-start` | Start-SOC in % | 50 |
| `--speed` | (standalone) Simulationssekunden pro Realsekunde | 600 |
| `--step` | Schrittweite in Simulationssekunden | 60 |
| `--duration` | (standalone) Simulationsdauer in Stunden; ohne Angabe endlos | endlos |
| `--start` | (standalone) Startzeit `yyyy-MM-dd HH:mm` | heute 00:00 |

Hinweise:

- Dezimalzahlen werden in den Argumenten mit **Punkt** geschrieben (`--capacity 7.5`).
- Ungültige Eingaben beenden das Programm mit einer Fehlermeldung. Geprüft werden: `--speed` und `--step` größer als 0, `--capacity` größer als 0, `--soc-min` kleiner als `--soc-max`.

### Beispielparameter

| Zweck | Aufruf |
|---|---|
| Standardlauf | `dotnet run` |
| Schneller Demolauf (24 h in ca. 24 s) | `dotnet run -- --speed 3600` |
| Kleine Batterie, ganzer Zyklus ab Morgen | `dotnet run -- --capacity 10 --start 06:00 --soc-start 20` |
| Leistungsbegrenzung sichtbar machen | `dotnet run -- --max-charge 2 --max-discharge 2` |
| Feinere Zeitauflösung | `dotnet run -- --step 10 --speed 600` |
| Dauerbetrieb (Tagesprofil wiederholt sich) | `dotnet run -- --duration 0` |
| Zweites Gerät parallel (zweites Terminal) | `dotnet run -- --id BAT-02 --capacity 10` |

## Bedienung

Während der Simulation zeigt ein festes Dashboard Simulationszeit, Tempo, Modus, Gerätezustand, Soll- und Ist-Leistung, SOC (Zahl und Balken), Temperatur, aktive Störung sowie Energie- und Zykluszähler.

### Tasten im laufenden Betrieb

| Taste | Funktion |
|---|---|
| Leertaste | Pause (nur standalone; mit Master pausiert der Master) |
| S | Einstellungen öffnen (löst das Pausenmenü ab) |
| M | Umschalten Auto / Manuell (beim Wechsel auf Manuell wird der Sollwert 0) |
| Pfeil hoch / runter | Sollwert um 0,5 kW erhöhen / senken (nur im Manuell-Modus) |
| 0 | Sollwert auf 0 kW (nur im Manuell-Modus dauerhaft, im Auto-Modus überschreibt das Profil ihn wieder) |
| E | Gerät ein- bzw. ausschalten |
| N | Not-Aus auslösen |
| H | Überhitzung auslösen |
| R | Störung zurücksetzen |
| Q | Beenden |

Zum Beenden immer **Q** drücken (oder die Simulationsdauer ablaufen lassen). Wird der Debugger hart gestoppt, kann das Ende der CSV-Datei fehlen.

### Pausenmenü (LB01) und Einstellungen (LB02)

In LB01 hielt die Leertaste die Simulation an und öffnete ein Menü. Ab LB02 steuert der Master die Pause; die Einstellungen öffnet `S`, die Zeit läuft dabei weiter,
Tempo und Schrittweite sind dort nicht mehr änderbar. Änderbar sind:

1. Sollwert (wechselt dabei in den Manuell-Modus)
2. Maximale Ladeleistung
3. Maximale Entladeleistung
4. SOC-Untergrenze
5. SOC-Obergrenze
6. Simulationsgeschwindigkeit
7. Schrittweite
8. Fortsetzen

Im Menü werden Dezimalzahlen mit der **Schreibweise des Betriebssystems** eingegeben, bei deutschen Windows-Einstellungen also mit **Komma** (`3,5`). Ungültige Werte werden abgelehnt und der alte Wert bleibt bestehen. Die Zeit, die man im Menü verbringt, wird nicht nachgeholt.

## Modellbeschreibung

### Grundprinzip

Vorzeichen: **positive Leistung = Laden, negative Leistung = Entladen.** Das Gerät bekommt einen **Sollwert**, entweder vom Tagesprofil (Auto), per Tasten oder über das Pausenmenü (Manuell). Es liefert die **Ist-Leistung**, also das, was es unter den aktuellen Bedingungen darf. Unzulässige Sollwerte werden nicht abgelehnt, sondern still begrenzt, so wie es ein Batteriemanagementsystem tut. Soll und Ist werden getrennt angezeigt und protokolliert.

### Begrenzung der Leistung

```
erlaubt_laden    = max_laden    × Taper(SOC) × Derating(T)
erlaubt_entladen = max_entladen × Derating(T)

Ist = Soll, begrenzt auf  −erlaubt_entladen … +erlaubt_laden
```

Zusätzlich gilt:

- Bei SOC an der Obergrenze ist Laden gesperrt, bei SOC an der Untergrenze ist Entladen gesperrt.
- Innerhalb eines Schritts wird nur so viel Energie umgesetzt, wie bis zur Grenze noch hineinpasst (bzw. entnommen werden kann). Der SOC überschreitet die Grenzen deshalb auch bei großen Schrittweiten nicht.
- Bei Aus, Not-Aus oder Überhitzungsabschaltung ist die Ist-Leistung 0.

### Ladezustand (SOC)

```
SOC(neu) = SOC + Ist-Leistung × Δt[h] / Kapazität[kWh] × 100
```

### Leistungsreduzierung bei hohem SOC (Taper)

Nur beim Laden: Bis 80 % SOC volle Leistung, danach sinkt die erlaubte Ladeleistung linear auf 10 % der Maximalleistung bei der SOC-Obergrenze. Die Ladekurve läuft dadurch flach aus.

### Temperatur und Derating

Die Temperatur ändert sich pro Simulationsminute um

```
ΔT = ( 0,06 × |Ist-Leistung|  −  0,02 × (T − T_außen)  +  Störungswärme ) × Δt[min]
```

`T_außen` ist standardmäßig 20 °C. Mit Wetterbezug (siehe LB02) kommt sie je Modellstunde vom Wettersimulator. Bei dauerhaft 5 kW und 20 °C pendelt sich die Temperatur bei etwa 35 °C ein.

| Temperatur | Verhalten |
|---|---|
| unter 45 °C | volle Leistung |
| 45 bis 60 °C | Leistung sinkt linear von 100 % auf 0 % (Derating, gilt für Laden und Entladen) |
| ab 60 °C | Abschaltung, Zustand `Fault` |
| nach Reset bis 50 °C | Zustand `Cooldown`, gesperrt |
| 50 °C oder darunter | wieder freigegeben |

### Gerätezustände

`Off`, `Standby`, `Charging`, `Discharging`, `Fault`, `Cooldown`. Laden und Entladen ergeben sich aus der Ist-Leistung (Totzone ±0,01 kW).

### Störungen

| Störung | Auslösen | Wirkung | Zurücksetzen |
|---|---|---|---|
| Not-Aus | Taste N | Ist-Leistung sofort 0, Sollwert wird 0, Zustand `Fault` | Taste R, danach `Standby` |
| Überhitzung | Taste H | zusätzliche Wärmequelle (2 °C pro Minute), Derating ab 45 °C, Abschaltung bei 60 °C | Taste R schaltet die Wärmequelle ab, bis 50 °C `Cooldown`, danach frei |

Im Auto-Modus bleibt das Profil während einer Störung aktiv, die Ist-Leistung ist aber 0. Nach dem Reset läuft es an der aktuellen Simulationszeit weiter, in der CSV entsteht dadurch eine sichtbare Lücke. Eine Überhitzung allein durch Last erreicht wegen des Deratings praktisch nie 60 °C. Die manuelle Störung ist der vorgesehene Weg, sie auszulösen.

### Zähler

Geladene und entladene Energie in kWh sowie äquivalente Vollzyklen: `(geladen + entladen) / (2 × Kapazität)`.

### Auto-Modus: Tagesprofil

Das Profil liefert den Sollwert abhängig von der Tageszeit. Es hängt an der Simulationsuhr und wiederholt sich im Dauerbetrieb täglich.

| Zeit | Sollwert |
|---|---|
| 00:00 bis 07:00 | 0 kW |
| 07:00 bis 17:00 | Laden, Sinuskurve mit Maximum um 12:00 |
| 17:00 bis 18:00 | 0 kW |
| 18:00 bis 23:00 | Entladen, Grundlast mit Spitze um 19:30 |
| 23:00 bis 24:00 | 0 kW |

Auf den Sollwert wird ein Zufallsfaktor von ±10 % gelegt, der alle 5 Simulationsminuten neu gewürfelt wird. Das Profil beschreibt nur den Wunsch, die Begrenzungen des Geräts greifen weiterhin. Bei den Standardwerten ist die Batterie deshalb am Vormittag voll und am Abend vor Ende des Profils leer.

### Simulationsuhr

Die Hauptschleife läuft alle etwa 100 ms in Echtzeit. Aus der vergangenen Realzeit und der eingestellten Geschwindigkeit wird Simulationszeit gutgeschrieben, und für jede volle Schrittweite wird ein Rechenschritt ausgeführt. Geschwindigkeit und Schrittweite sind damit unabhängig voneinander einstellbar. Bei Speed 600 und Schritt 60 s entspricht eine Realsekunde zehn Rechenschritten (zehn Simulationsminuten). Während der Pause wird keine Zeit gutgeschrieben.

## CSV-Verlauf

Pro Gerät entsteht eine Datei `battery_<ID>_<Datum>.csv` in `docs/data/` im Repository, egal aus welchem Ordner (Kommandozeile oder Visual Studio) gestartet wird. Es wird **bei jedem Simulationsschritt** eine Zeile geschrieben, mit dem Zustand am Ende des Schritts. Trennzeichen ist das Semikolon, Dezimalzeichen der Punkt.

| Spalte | Bedeutung |
|---|---|
| `SimTime` | Simulationszeit (Tag.Stunde:Minute:Sekunde, z. B. `1.06:15:00`) |
| `DeviceId` | Geräte-ID |
| `Mode` | Gerätezustand (Off, Standby, Charging, Discharging, Fault, Cooldown) |
| `Control` | Auto oder Manual |
| `SetpointKW` / `ActualKW` | Soll- und Ist-Leistung |
| `SOC` | Ladezustand in % |
| `TempC` | Temperatur in °C |
| `Fault` | aktive Störung (None, EmergencyStop, Overheat) |
| `MaxChargeKW` / `MaxDischargeKW` | aktuelle Leistungsgrenzen |
| `SocMin` / `SocMax` | aktuelle SOC-Grenzen |

Die CSV darf während des Laufs nicht in Excel geöffnet sein. Zwei Instanzen mit **derselben ID** am selben Tag blockieren sich gegenseitig, daher pro Instanz eine eigene `--id` verwenden.

## Aufbau

| Klasse | Aufgabe |
|---|---|
| `Batterie` | Physik des Geräts: Begrenzung, SOC, Temperatur, Zustände, Störungen, Zähler |
| `BatterieConfig` | Einstellungen (ID, Kapazität, Leistungen, SOC-Grenzen) |
| `DailyProfile` | Tagesprofil mit Rauschen für den Auto-Modus |
| `Simulator` | Simulationsuhr, Hauptschleife, Tasten, Pausenmenü |
| `CsvLogger` | schreibt den Verlauf |
| `ConsoleDashboard` | feste Statusanzeige |
| `Program` (`main.cs`, Top-Level-Statements) | liest die Argumente und startet die Simulation |

`Batterie` kennt weder Konsole noch Uhr noch Profil und nimmt Leistungsvorgaben ausschließlich über `SetPower(kW)` entgegen. Dadurch kann später ein externer Controller (z. B. bei der Kopplung mit PV-Anlage und Ladestation) den Sollwert vorgeben.

## Bekannte Einschränkungen

- Wirkungsgrad, Selbstentladung, Spannung/Strom und Alterung (SOH) sind nicht modelliert.
- Die Kopplung mit PV-Anlage und Ladestation ist noch nicht umgesetzt. Das Gerät läuft eigenständig.
- Die Eingaben im Pausenmenü werden nur grob geprüft, Dezimalzahlen müssen in der Schreibweise des Betriebssystems eingegeben werden.
- Ein Ereignisprotokoll im Dashboard und eine Zusammenfassung am Programmende gibt es nicht.

![alt text](../../../docs/screenshots/BatterySimulator/bild.png)

## Projektstruktur

Pfade relativ zum Repository-Hauptordner `dotnet-energy-twin-alpha/`:

```
src/Devices/BatterySimulator/
├── BatterySimulator.csproj           # Projekt (Einstellungen aus src/Directory.Build.props)
├── main.cs                           # Einstieg: Argumente lesen, Objekte erzeugen, starten
├── Batterie.cs                       # Gerätephysik
├── BatterieConfig.cs                 # Einstellungen
├── DailyProfile.cs                   # Tagesprofil (Auto-Modus)
├── Simulator.cs                      # Hauptschleife (Zeit vom Master, feste Schritte), Tasten, Einstellungen
├── ArgsParser.cs, SimulatorOptions.cs # Kommandozeile
├── BatteryModbus.cs                  # Modbus: Register veröffentlichen, Schreibzugriffe anwenden
├── CsvLogger.cs                      # CSV-Verlauf (async)
├── ConsoleDashboard.cs               # Statusanzeige
└── Enum.cs                           # DeviceState, ControlMode, FaultType

docs/                                 # zentral im Repository
├── data/battery_<ID>_<Datum>.csv     # CSV-Ausgabe (Beispiel: battery_BAT-01_2026-10-07.csv)
├── screenshots/BatterySimulator/bild.png
└── development-journal/dominik-Zach.md   # Entwicklungsjournal
```

---

## LB02 – Umbau zum Modbus-Feldgerät

**Zweck:** Batteriespeicher als Feldgerät im Gesamtsystem: virtuelle Zeit vom Simulations-Master, Zustand und Sollwert über Modbus TCP.
**Verantwortlich:** Dominik Zach (LB01-Gerät); Umbau auf Master-Zeit und Modbus am 2026-10-25 von Susanne Lastowicka
**Basis aus LB01:** Domi Batterie, BatterieConfig, DailyProfile

### Stand (2026-10-25)

| Schritt | Stand |
|---|---|
| Zeit vom Simulations-Master (`DeviceTimeSource`, `StepScheduler`, feste Schritte `--step`) | ✅ |
| Standalone mit lokaler Uhr (`--standalone`; Leertaste und `<`/`>` wirken nur dort) | ✅ |
| async Schleife, CSV, Tastatur und Einstellungsmenü (kein `Thread.Sleep`, kein `ReadLine`) | ✅ |
| Modbus-Server mit Registerplan, Server bleibt in der Pause erreichbar | ✅ |

### Konfiguration

| Option | Bedeutung | Standard |
|---|---|---|
| `--id`, `--capacity`, `--max-charge`, `--max-discharge`, `--soc-min`, `--soc-max`, `--soc-start` | Gerätedaten wie in LB01 (Tabelle oben) | |
| `--step <s>` | feste Schrittweite in Simulationssekunden | 60 |
| `--seed <zahl>` | Zufalls-Seed für das Tagesprofil | zufällig |
| `--master <host:port>` | Simulations-Master | `127.0.0.1:5100` |
| `--standalone` | ohne Master, lokale Uhr | aus |
| `--start <zeit>` | nur standalone, `yyyy-MM-dd HH:mm` (früher nur Uhrzeit) | heute 00:00 |
| `--speed <x>` | nur standalone, Tempo 1–100 000 | 600 |
| `--duration <h>` | nur standalone, Dauer; ohne Angabe endlos (früher 0 = endlos, Standard 24) | endlos |
| `--modbus-port <port>` | Modbus-TCP-Port, pro Gerät eigener | 1502 |
| `--modbus-bind <ip>` | Adresse des Servers | 127.0.0.1 |
| `--unit-id <n>` | Modbus-Unit-ID | 1 |
| `--no-modbus` | Server nicht starten | aus |
| `--register-plan` | Registerplan als Markdown ausgeben | |
| `--weather <url\|off>` | Wettersimulator; mit Master Standard `http://127.0.0.1:8080`, standalone aus | |
| `--location <ort>` | Standort für die Wetterabfrage (salzburg, wien, innsbruck) | salzburg |

Zusätzlich geprüft: `--max-charge`/`--max-discharge` > 0, `0 ≤ --soc-min < --soc-max ≤ 100`, `--soc-start` dazwischen. Dezimalzahlen mit Punkt **oder** Komma.

### Bedienung / Start

```
dotnet run --project src/SimulationMaster -- --speed 600          # 1. Master
dotnet run --project src/Devices/BatterySimulator -- --id BAT-01 --modbus-port 1503   # 2. Batterie
dotnet run --project src/Devices/BatterySimulator -- --standalone --speed 3600 --start "2026-01-15 06:00" --duration 24   # ohne Master
```

Tasten: Leertaste Pause (nur standalone, sonst Master), `<` / `>` Tempo ÷2 / ×2 (nur standalone), `M`, `↑`/`↓`, `0`, `E`, `N`, `H`, `R`, `Q` wie in LB01.
**`S` öffnet die Einstellungen** unter der Anzeige (löst das Pausenmenü ab): 1 Sollwert, 2 max. Ladeleistung, 3 max. Entladeleistung, 4 SOC-Min, 5 SOC-Max.
Die Zeit läuft währenddessen weiter, die Modbus-Register auch. Tempo und Schrittweite lassen sich nicht mehr im Programm ändern (Tempo: Master, Schritt: `--step`).
Die Anzeige hat neue Zeilen „Zeitquelle“, „Modbus“, „Wetter“ und „Meldung“. Bei umgeleiteter Ausgabe erscheint alle 5 s eine Statuszeile.

Der Wetterbezug ist optional: Mit Master fragt die Batterie standardmäßig den Wettersimulator je Modellstunde und übernimmt dessen Temperatur als
Außentemperatur; standalone oder mit `--weather off` rechnet sie mit festen 20 °C. Fehlt der Wettersimulator, bleibt das letzte Wetter aktiv und nach
3 Fehlversuchen erscheint „WETTER: NICHT ERREICHBAR“ in der Meldungszeile (neue Versuche alle 10 s).

### Modbus-Registerplan

Holding Registers (Funktionscode 03 lesen, 06/16 schreiben). Standard: Host `127.0.0.1`, Port `1502`, Unit-ID `1` – alle drei sind konfigurierbar.
Adressen sind Protokolladressen (0-basiert, Referenzadresse = 40001 + Adresse). Pro Register Big-Endian;
mehrteilige Werte (`UInt32`, `Float32`) belegen zwei Register, das höherwertige Wort steht zuerst (Float32 = IEEE 754, Byte-Folge ABCD).
Mehrteilige Werte müssen mit **einem** Schreibzugriff (FC16) auf beide Register geschrieben werden.

| Adresse | Ref. | Name | Typ | Regs | Einheit | Zugriff | Schreibbereich | Bedeutung / Fehlerverhalten |
|---|---|---|---|---|---|---|---|---|
| 0 | 40001 | Gerätetyp | UInt16 | 1 |  | lesen | – | 1 PV, 2 Batterie, 3 Charger, 4 Wind |
| 1 | 40002 | Geräte-Nummer | UInt16 | 1 |  | lesen | – | Zahl am Ende der Geräte-ID (DC-02 → 2), 0 wenn keine |
| 2 | 40003 | Betriebszustand | UInt16 | 1 |  | lesen | – | 0 Aus, 1 Bereit/Leerlauf, 2 Betrieb, 3 Störung |
| 3 | 40004 | Fehlercode | UInt16 | 1 |  | lesen | – | 0 = kein Fehler, sonst gerätespezifisch (siehe Registerplan des Geräts) |
| 4–5 | 40005 | Wirkleistung | Float32 | 2 | kW | lesen | – | Aktuelle Wirkleistung; positiv = Aufnahme, negativ = Abgabe |
| 6–7 | 40007 | Kumulierte Energie | Float32 | 2 | kWh | lesen | – | Energiezähler seit Programmstart (Bedeutung je Gerät im Registerplan) |
| 8–9 | 40009 | Modellzeit | UInt32 | 2 | s | lesen | – | Virtuelle Zeit des dargestellten Zustands, Sekunden seit 1970-01-01 00:00 (ohne Zeitzone); 0 = noch keine Zeit |
| 10 | 40011 | Lauf | UInt16 | 1 |  | lesen | – | Lauf-Nummer; ändert sich bei jedem Neustart im Master (Modellzeit springt dann) |
| 11 | 40012 | Sequenznummer | UInt16 | 1 |  | lesen | – | Zählt bei jeder neuen Modellzeit hoch (Überlauf bei 65535); gleiche Nummer = gleicher Zustand |
| 12 | 40013 | Zeitstatus | UInt16 | 1 |  | lesen | – | 0 keine Zeit, 1 synchron mit Master, 2 Standalone, 3 Master verloren (Zeit steht) |
| 13 | 40014 | Simulation läuft | UInt16 | 1 |  | lesen | – | 1 = virtuelle Zeit läuft, 0 = pausiert oder keine Zeit; bei Pause zeigen die Register den letzten Zustand |
| 14–15 | 40015 | Tempo | Float32 | 2 | x | lesen | – | Simulationsgeschwindigkeit gegenüber Echtzeit |
| 16 | 40017 | Ein/Aus | UInt16 | 1 |  | lesen/schreiben | 0 … 1 | Lesen: 1 = Gerät ein. Schreiben: 0 aus, 1 ein |
| 17 | 40018 | Fehler zurücksetzen | UInt16 | 1 |  | lesen/schreiben | 1 … 1 | Schreiben von 1 setzt Störungen zurück, sofern das Gerät es zulässt; liest immer 0 |
| 18 | 40019 | Letzter Schreibfehler | UInt16 | 1 |  | lesen | – | 0 = letzter Schreibzugriff angenommen, 1 nicht beschreibbar, 2 ungültiger Wert, 3 Wert unvollständig geschrieben |
| 19 | 40020 | Adresse des letzten Schreibzugriffs | UInt16 | 1 |  | lesen | – | Erste Adresse des letzten Schreibzugriffs (bei Fehler: die fehlerhafte Adresse) |
| 20–21 | 40021 | Leistungssollwert | Float32 | 2 | kW | lesen/schreiben | -1000 … 1000 | Sollwert der Leistung: positiv laden, negativ entladen. Gültig von −max. Entladeleistung bis +max. Ladeleistung (Register 37 und 35, Standard ±5 kW), sonst abgelehnt (Letzter Schreibfehler = 2). Ein Schreibzugriff schaltet in den Betriebsmodus Manuell |
| 22 | 40023 | Betriebsmodus | UInt16 | 1 |  | lesen/schreiben | 0 … 1 | 0 Auto (Tagesprofil bestimmt den Sollwert), 1 Manuell (Sollwert aus Register 20) |
| 23–24 | 40024 | Ladezustand (SOC) | Float32 | 2 | % | lesen | – | Ladezustand, zwischen SOC-Min und SOC-Max begrenzt |
| 25–26 | 40026 | Temperatur | Float32 | 2 | °C | lesen | – | Derating ab 45 °C, Abschaltung bei 60 °C (Fehlercode 2), Freigabe ≤ 50 °C |
| 27–28 | 40028 | Geladene Energie | Float32 | 2 | kWh | lesen | – | Seit Programmstart (entspricht der Energie im Kopf) |
| 29–30 | 40030 | Entladene Energie | Float32 | 2 | kWh | lesen | – | Seit Programmstart |
| 31–32 | 40032 | Vollzyklen | Float32 | 2 |  | lesen | – | Äquivalente Vollzyklen |
| 33–34 | 40034 | Kapazität | Float32 | 2 | kWh | lesen | – | Konfigurierte Kapazität |
| 35–36 | 40036 | Max. Ladeleistung | Float32 | 2 | kW | lesen | – | Konfiguriert (--max-charge) |
| 37–38 | 40038 | Max. Entladeleistung | Float32 | 2 | kW | lesen | – | Konfiguriert (--max-discharge) |
| 39–40 | 40040 | SOC-Minimum | Float32 | 2 | % | lesen | – | Konfiguriert (--soc-min) |
| 41–42 | 40042 | SOC-Maximum | Float32 | 2 | % | lesen | – | Konfiguriert (--soc-max) |
| 43 | 40044 | Gerätezustand | UInt16 | 1 |  | lesen | – | 0 aus, 1 Bereitschaft, 2 lädt, 3 entlädt, 4 Störung, 5 Abkühlung (Fehlercode im Kopf: 0 keiner, 1 Not-Aus, 2 Überhitzung, 3 Abkühlung nach Überhitzung) |

Besonderheiten des Batteriespeichers:
- **Leistung:** positiv = Laden (Aufnahme), negativ = Entladen (Abgabe). **Energie** im Kopf = geladene Energie, die entladene steht in Register 29–30.
- **Betriebszustand:** Aus, Bereitschaft, Betrieb (lädt/entlädt), Störung (Fault und Abkühlung).
- **Fehlercode:** 0 keiner, 1 Not-Aus, 2 Überhitzung, 3 Abkühlung nach Überhitzung (keine aktive Störung, Gerät sperrt bis ≤ 50 °C).
- **Sollwert (20–21):** gültig von −max. Entladeleistung bis +max. Ladeleistung (konfiguriert, Registerplan nennt nur die absolute Grenze ±1000 kW);
  alles außerhalb wird abgelehnt (Register 18 = 2), kein stilles Begrenzen. Ein Schreibzugriff schaltet in den Modus **Manuell**
  (im Auto-Modus würde das Tagesprofil den Wert sofort wieder überschreiben). Zurück in Auto: Register 22 = 0.
- **Ein/Aus (16):** wie Taste `E`; Aus setzt den Sollwert auf 0. **Fehler zurücksetzen (17):** wie Taste `R`.
- Fehlerverhalten bei Schreibzugriffen: siehe `src/Devices/DualPortCharger/README.md`, Abschnitt „Fehlerverhalten bei Schreibzugriffen“.

### CSV
`docs/data/battery_<ID>_<Datum>_<Uhrzeit>.csv`, neue Spalte `Lauf` am Ende, `SimTime` jetzt mit Datum (`yyyy-MM-dd HH:mm:ss`). Beim Start und bei jedem
Neustart im Master wird zusätzlich eine Zeile geschrieben.

### Schnittstellen
- **Benötigt:** Simulations-Master (TCP, `127.0.0.1:5100`).
- **Angeboten:** Modbus-TCP-Server (siehe Registerplan), CSV. Prüfen mit `src/ModbusDiagnosticClient`.

### Tests
- `tests/FieldDevices.Tests`: Modbus-Anbindung (Kopf, Vorzeichen, Sollwert innerhalb/außerhalb der konfigurierten Leistung, Modus, Störung, Reset, Ein/Aus),
  Optionen und Prüfungen, `Update(TimeSpan)`, wiederholbares Tagesprofil mit Seed.
- Probelauf 2026-10-25 mit Master (x600): synchron, Sollwert −2,5 kW per Diagnose-Client angenommen, 9 kW abgelehnt, Zurück auf Auto.

### Bekannte Probleme
- Ein Neustart im Master behält den Zustand (SOC, Temperatur, Zähler).
- Die interaktive Anzeige, die Tasten und das Einstellungsmenü sind nur mit umgeleiteter Ausgabe geprüft, noch nicht in einem echten Terminalfenster.
- Die Beispiel-CSV aus LB01 hat noch das alte Format (`SimTime` als Zeitdauer, ohne `Lauf`).

---

**Quelle:** `src/Devices/DualPortCharger/README.md`, Abschnitt „dotnet-energy-sim-lb01 – Dual-Port-Charger“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise; Gemeinsame Enum-Legende um bereits implementierten Gerätetyp 5 ergänzt; Registeradressen und Fachwerte unverändert
**Aktueller Ort:** [README](../../src/Devices/DualPortCharger/README.md)

### Unveränderter Originalabschnitt

# dotnet-energy-sim-lb01 – Dual-Port-Charger

Konsolensimulator einer **öffentlichen DC-Schnellladesäule mit zwei Ladepunkten** und gemeinsamem Leistungsbudget (LB01, .NET Programmierung 2026).

Fahrzeuge kommen an, stecken an, laden nach einer SOC-abhängigen Ladekurve und fahren wieder ab. Die Säule teilt ihre Leistung zwischen den beiden Punkten auf. Die Ladepunkte erwärmen sich und können überhitzen. Jeder Schritt wird in eine CSV-Datei geschrieben.

**Seit LB02 (2026-10-25)** kommt die virtuelle Zeit vom **Simulations-Master** (`src/SimulationMaster`). Pause, Tempo und Neustart
steuert der Master für alle Geräte gemeinsam. Ohne Master läuft der Charger mit `--standalone` auf einer lokalen Uhr (wie in LB01).

---

## Voraussetzungen

- **.NET 10 SDK** (`dotnet --list-sdks` muss eine 10.x-Version zeigen)
- Windows, Linux oder macOS mit einer Konsole, die UTF-8 darstellt (für die Balken `█░`)
- Empfohlen: Konsolenfenster mit mindestens **140 Zeichen** Breite, sonst werden die Zeilen abgeschnitten
- Optional: Rider oder Visual Studio: `EnergyTwin.sln` öffnen, `DualPortCharger` als Startprojekt (Startprofil in `src/Devices/DualPortCharger/Properties/launchSettings.json`)
- Teil der Solution `EnergyTwin.sln` im Repository-Hauptordner. Das Projekt verweist auf `src/Devices.Common`
  (Zeitquelle, Schrittsteuerung, async Konsoleneingabe; bringt auch EasyModbusTCP.NETCore mit, das noch nicht genutzt wird).

## Start

Aus dem Repository-Hauptordner, **zuerst den Master**, dann den Charger (eigenes Konsolenfenster):

```bash
dotnet run --project src/SimulationMaster -- --speed 600      # Fenster 1: Master (startet pausiert, Leertaste = Start)
dotnet run --project src/Devices/DualPortCharger              # Fenster 2: Charger, verbindet sich mit 127.0.0.1:5100
```

Startet der Charger vor dem Master, zeigt er „WARTE AUF MASTER“ und versucht es alle 2 s erneut.
Ohne Master (z. B. zum Testen):

```bash
dotnet run --project src/Devices/DualPortCharger -- --standalone --speed 300 --duration 24
```

Ohne Argumente gelten die Standardwerte (siehe unten). Die Hilfe zeigt `--help`:

```bash
dotnet run --project src/Devices/DualPortCharger -- --help
```

### Kommandozeilenargumente

| Argument | Bedeutung | Standard | Erlaubt |
|---|---|---|---|
| `--id <text>` | Geräte-ID (erscheint in Anzeige und CSV-Dateiname) | `DC-01` | nicht leer |
| `--power <kW>` | Gesamtleistung der Säule | `150` | 50–350 |
| `--step <min>` | Schrittweite = Detailgrad (Rechenintervall, eine CSV-Zeile pro Schritt) | `5` | > 0 |
| `--seed <zahl>` | Zufalls-Seed für wiederholbare Läufe | zufällig | ganze Zahl |
| `--output <ordner>` | Zielordner der CSV (wird angelegt) | `docs/data` im Repo | gültiger Pfad |
| `--master <host:port>` | Simulations-Master | `127.0.0.1:5100` | Port 1–65535 |
| `--standalone` | ohne Master, lokale Uhr | aus | |
| `--start <zeit>` | nur standalone: Startzeit `yyyy-MM-dd HH:mm` | heute 00:00 | |
| `--speed <faktor>` | nur standalone: Tempo gegenüber Echtzeit | `120` | 1–100 000 |
| `--duration <h>` | nur standalone: Simulationsdauer in Stunden | endlos | > 0 (Warnung bei < 24) |
| `--modbus-port <port>` | Port des Modbus-TCP-Servers (jede Säule braucht einen eigenen) | `1502` | 1024–65535 |
| `--modbus-bind <ip>` | Adresse, auf der der Server lauscht | `127.0.0.1` | IP-Adresse (`0.0.0.0` = alle) |
| `--unit-id <n>` | Modbus-Unit-ID | `1` | 1–247 |
| `--no-modbus` | Modbus-Server nicht starten | aus | |
| `--register-plan` | Registerplan als Markdown ausgeben und beenden | | |
| `--weather <url\|off>` | Wettersimulator; mit Master Standard `http://127.0.0.1:8080`, standalone aus | mit Master `http://127.0.0.1:8080` | |
| `--location <ort>` | Standort für die Wetterabfrage | `salzburg` | |

Ungültige Werte oder unbekannte Argumente führen zu einer Fehlermeldung mit Hilfetext (Exit-Code 1).
`--start`, `--speed` und `--duration` ohne `--standalone` sind ein Fehler: Mit Master bestimmt der Master Zeit und Tempo.

**Echtzeit pro Schritt = Schrittweite / Tempo** (Tempo vom Master bzw. `--speed`). Beispiele:

| Schrittweite | Tempo | Echtzeit pro Schritt | 24 h dauern |
|---|---|---|---|
| 5 min | 120x | 2,5 s | 12 min |
| 1 min | 300x | 0,2 s | 4,8 min |
| 15 min | 600x | 1,5 s | 2,4 min |

### Beispielparameter

```bash
# Mit Master (Standard): 150 kW, 5-min-Schritte
dotnet run --project src/Devices/DualPortCharger

# Feiner Verlauf, reproduzierbar, Master auf einem anderen Rechner
dotnet run --project src/Devices/DualPortCharger -- --id DC-01 --power 150 --step 1 --seed 42 --master 192.168.1.20:5100

# Zweite Säule am selben Master (andere ID, anderer Modbus-Port)
dotnet run --project src/Devices/DualPortCharger -- --id DC-02 --power 100 --step 15

# Standalone wie in LB01: 24 h in 4,8 min, reproduzierbar
dotnet run --project src/Devices/DualPortCharger -- --standalone --step 1 --speed 300 --duration 24 --seed 42
```

Mehrere Instanzen mit verschiedenen `--id` können gleichzeitig laufen. Jede schreibt ihre eigene CSV und erscheint
einzeln in der Liste des Masters.

## Bedienung

Die Tasten wirken sofort, auch während die Simulation läuft:

| Taste | Befehl |
|---|---|
| `P` | nur standalone: Pause an/aus (Simulationszeit steht, keine CSV-Zeilen). Mit Master: Pause im Master |
| `A` | Auto-Modus an/aus |
| `N` | Manueller Modus: zufälliges Fahrzeug kommt, steckt am freien Punkt an und lädt sofort; abstecken mit `D` |
| `1` / `2` | Ladepunkt wählen (markiert mit `>`) |
| `C` | Fahrzeug an den gewählten Punkt anstecken: Modell, Start-SOC [20 %] und Ziel-SOC [100 %] eingeben, danach mit `S` starten |
| `D` | Fahrzeug abstecken (nicht bei Störung, das Kabel ist verriegelt) |
| `S` | Laden starten / stoppen |
| `F` | Störung auslösen (Kühlungsdefekt) |
| `R` | Störung zurücksetzen (erst unter 60 °C) |
| `+` / `-` | Gesamtleistung ±10 kW (50–350 kW) |
| `<` / `>` | nur standalone: Tempo halbieren / verdoppeln (1–100 000x). Mit Master: Tempo im Master |
| `O` | Säule ein/aus |
| `Q` | Beenden |

Die Zeile **Meldung** zeigt das Ergebnis jedes Befehls, auch warum ein Befehl gerade nicht möglich ist (z. B. „Reset erst unter 60 °C möglich (aktuell 72 °C)“).

Standalone mit `--duration`: Nach Ablauf bleibt die letzte Anzeige stehen, bis man `Q` drückt. Danach wird der Pfad der
CSV-Datei ausgegeben. `Q` oder Strg+C beenden jederzeit geordnet (Abmeldung beim Master, CSV wird geschlossen).

Die Eingaben beim Anstecken (`C`) werden ohne blockierendes Warten gelesen, `Esc` bricht ab. Während der Eingabe
rechnet das Modell nicht weiter; die fälligen Schritte werden danach nachgeholt (die Zeit kommt ja vom Master).

Die Zeile **Zeitquelle** zeigt den Zustand der Verbindung:

| Anzeige | Bedeutung |
|---|---|
| `WARTE AUF MASTER host:port – <Fehler>` | noch keine Zeit; Wiederverbindung alle 2 s |
| `Master host:port · synchron · LÄUFT · x600 · Lauf 1 · Abweichung ≤ 204 ms (RTT 0,7 ms)` | Normalbetrieb; Abweichung = größtmöglicher Fehler zum Master (RTT/2 · Tempo) |
| `MASTER host:port NICHT ERREICHBAR – Zeit steht, Wiederverbindung läuft` | 3 s nichts vom Master; das Modell hält an |
| `STANDALONE (lokale Uhr, ohne Master) · LÄUFT · x120` | `--standalone` |

Ein **Neustart im Master** (neue Zeit, neuer Lauf) wird übernommen: Die Modellzeit springt auf die neue Zeit,
der Zustand der Säule (Fahrzeuge, Temperaturen, Zähler) bleibt erhalten. Die Meldung zeigt „Neustart durch den Master“.

### Konsolenausgabe

```
=== Dual-Charger DC-01 | AUTO ===
Simulationszeit: So. 2026-10-04 17:30   Lauf: 1   Schritt: 5 min
Zeitquelle:      Master 127.0.0.1:5100 · synchron · LÄUFT · x120 · Lauf 1 · Abweichung ≤ 41 ms (RTT 0,7 ms)

Säule: EIN   Limit: 150 kW   Abgewiesen: 3   Warteschlange: 0
Leistung:   142,0 kW [█████████░]  95 %   Energie: 1 284,3 kWh

>[Punkt 1] CHARGING   VW ID.3               SOC  62,0 % [██████░░░░] Ziel  90 %   120,0 kW   41 °C     612,4 kWh
 [Punkt 2] CHARGING   Renault Zoe           SOC  84,0 % [████████░░] Ziel 100 %    22,0 kW   27 °C     671,9 kWh

Meldung: ...
```

Screenshots aus einem echten Lauf (LB01, noch ohne Zeile „Zeitquelle“):

| Auto-Modus | Manueller Modus |
|---|---|
| ![Auto-Modus](../../../docs/screenshots/DualPortCharger/Automatischer_Modus.png) | ![Manueller Modus](../../../docs/screenshots/DualPortCharger/Manueller_Modus.png) |

| Limit-Erhöhung | Überhitzung |
|---|---|
| ![Limit-Erhöhung](../../../docs/screenshots/DualPortCharger/Limit_Erhöhung.png) | ![Überhitzung](../../../docs/screenshots/DualPortCharger/Überhitzung.png) |

---

## Modellbeschreibung

Ausführliche Herleitung und Begründungen: [`docs/devices/DualPortCharger/design-notes.md`](../../../docs/devices/DualPortCharger/design-notes.md).

### Klassen

```
DualCharger
 ├─ ChargingPoint [1]  ── 0..1 Vehicle
 └─ ChargingPoint [2]  ── 0..1 Vehicle
```

| Ordner | Klasse | Aufgabe |
|---|---|---|
| `Models` | `Vehicle` | Batterie (Kapazität, SOC, Ziel-SOC), Ladekurve |
| | `VehicleCatalog` | Fahrzeugmodelle, zufällige Fahrzeuge |
| | `ChargingPoint` / `ChargingPointState` | Zustandsautomat, Energie, Temperatur je Ladepunkt |
| | `DualCharger` | Leistungsbudget, Aufteilung, Warteschlange, Ein/Aus |
| `Simulation` | `SimulationEngine` | async Hauptschleife: Zeit von `DeviceTimeSource`, feste Schritte über `StepScheduler`, Neustart, CSV |
| | `AutoModeController` | Zufällige Ankünfte und Abfahrten |
| | `SimulationConfig` | Startparameter |
| `IO` | `ConsoleRenderer`, `InputHandler`, `CsvLogger`, `ArgsParser` | Anzeige, Tastatur, CSV, Kommandozeile (alle async) |
| `Devices.Common` | `DeviceTimeSource`, `StepScheduler`, `TimeSyncOptions`, `AsyncConsole` | gemeinsame Zeitanbindung aller Geräte |

`Program.cs` baut nur die Konfiguration und startet die Engine.

### Fahrzeuge

| Modell | Kapazität | Max. DC-Ladeleistung |
|---|---|---|
| Renault Zoe | 52 kWh | 50 kW |
| Fiat 500e | 42 kWh | 85 kW |
| VW ID.3 | 58 kWh | 120 kW |
| Tesla Model 3 | 60 kWh | 170 kW |
| Hyundai Ioniq 5 | 77 kWh | 220 kW |
| Mercedes eActros 600 (LKW) | 621 kWh | 400 kW |
| Kia EV6 (defekt) | 77 kWh | 240 kW |

Gerundete Realwerte. Fahrzeuge über 150 kW zeigen, dass die Säule begrenzt. Der LKW kann mehr aufnehmen, als die ganze Säule liefert. Der **Kia EV6 (defekt)** hat eine beschädigte Ladebuchse und überhitzt bei hoher Leistung (siehe Temperatur).

### Ladekurve (Leistung nicht konstant)

```
SOC ≤ 80 %:  P = P_max
SOC > 80 %:  P = P_max · e^(−k · (SOC − 80)),   k = ln(10) / 20
```

Bei 100 % SOC sind es noch 10 % der Maximalleistung. Ist der Ziel-SOC erreicht, wechselt der Punkt auf `FINISHED`.

### Aufteilung des Leistungsbudgets

Lädt nur ein Punkt, bekommt er das ganze Budget. Laden beide, bekommt jeder zunächst bis zur Hälfte, und was einer nicht braucht, geht an den anderen:

```
150 kW, Bedarf 15 + 150 kW  →  15 + 135 kW   (nicht starr 75 + 75)
```

### Energie und SOC

```
E_netz = P · Δt              (gezählte Energie)
E_akku = E_netz · η,  η = 0,95
ΔSOC   = E_akku / Kapazität · 100 %
```

Beim Erreichen des Ziel-SOC wird die Energie im letzten Schritt passend gekürzt. Die Leistung gilt jeweils für einen ganzen Schritt. Große Schrittweiten sind über 80 % SOC deshalb ungenauer, und darum ist die Schrittweite einstellbar.

### Zustände eines Ladepunkts

```
Available ──C──► Connected ──S──► Charging ──SOC ≥ Ziel──► Finished ──D──► Available
                     ▲                │
                     └──────S─────────┘
Faulted: aus jedem Zustand (F oder automatisch ab 80 °C)
```

`FINISHED` heißt: Das Fahrzeug ist voll, steckt aber noch und blockiert den Punkt. Bei `FAULTED` liefert der Punkt 0 kW, das Fahrzeug bleibt verbunden und der SOC bleibt stehen.

### Temperatur und Störung (Kabel/Stecker)

| Parameter | Wert |
|---|---|
| Umgebung | 20 °C (Standard; mit Wetterbezug vom Wettersimulator) |
| Beharrungstemperatur bei 150 kW | 55 °C |
| Zeitkonstante τ (normal / Kühlungsdefekt) | 10 min / 30 min |
| Derating | ab 60 °C linear bis 0 % bei 80 °C |
| Störung | ab 80 °C automatisch, mit `F` sofort auf 85 °C |
| Reset | manuell (`R`) unter 60 °C, automatisch unter 50 °C |

```
T_ziel = T_außen + 35 °C · (P / 150 kW)²       (Verluste ∝ I²; T_außen standardmäßig 20 °C)
T_neu  = T_ziel + (T_alt − T_ziel) · e^(−Δt / τ)
```

Die exakte Lösung statt eines Euler-Schritts bleibt bei jeder Schrittweite stabil. Bei normalen Fahrzeugen verhindert das Derating eine Störung: Die Leistung geht bei 80 °C auf 0, die Temperatur nähert sich 80 °C nur an. Wurde vor der Störung geladen, lädt der Punkt nach dem Reset weiter.

**Kia EV6 (defekt):** Der Temperatursensor der Ladebuchse ist ausgefallen, es gibt **kein Derating**. Dazu erwärmt der erhöhte Übergangswiderstand den Stecker doppelt so stark (`T_ziel = T_außen + 70 °C · (P / 150 kW)²`). Ab ca. 140 kW steigt die Temperatur über 80 °C und nur die Abschaltung der Säule schützt. Mit 150 kW allein am Punkt ist er nach ca. 20 min `FAULTED`, kühlt ab, setzt sich unter 50 °C zurück und lädt weiter. Teilt er sich die 150 kW mit dem anderen Punkt, bleibt er kühl.

### Auto-Modus

- **Ankünfte** nach Tageszeit: 00–06 Uhr 0,2/h · 06–09 Uhr 1,5/h · 09–16 Uhr 1,0/h · 16–20 Uhr 2,5/h · 20–24 Uhr 0,8/h. Pro Schritt gilt `p = 1 − e^(−Rate · Δt)`, damit hängt die Zahl der Ankünfte nicht von der Schrittweite ab.
- **Fahrzeug:** zufälliges Modell (auch der defekte Kia EV6), Ankunfts-SOC 10–50 %, Ziel-SOC 80–100 %.
- **Kein Punkt frei:** Das Fahrzeug wartet bis zu 5 min, sonst fährt es weg und zählt als **abgewiesen**. Bei Schrittweiten ≥ 5 min wird die Wartezeit nur schrittweise geprüft.
- **Abfahrt nach `FINISHED`:** 80 % nach 0–5 min, 20 % erst nach 10–40 min (blockiert den Punkt).

### Manueller Modus

- **`N`:** Ein zufälliges Fahrzeug kommt, steckt sofort am ersten freien Punkt an und startet (auch während der Pause). Sind beide Punkte belegt, wartet es bis zu 5 min.
- **Abstecken** nur von Hand mit `D`, es fährt kein Fahrzeug von selbst ab.
- Alternativ `C`: Modell und SOC selbst wählen, mit `S` starten.

### Wichtige Annahmen

- Die Werte sind plausible Annahmen, keine Normwerte. Ziel ist nachvollziehbares, dynamisches Verhalten, kein exaktes physikalisches Modell.
- Die Temperatur gilt für Kabel/Stecker je Ladepunkt, nicht für die Leistungselektronik.
- Der Wirkungsgrad ist konstant (0,95), die Batterietemperatur wird nicht modelliert.
- Die maximale Ladeleistung gehört zum Fahrzeugmodell und ist nicht änderbar. Der SOC wird beim Anstecken festgelegt.
- Eine einzige `Random`-Instanz. Mit `--seed` ist ein Lauf wiederholbar, solange keine manuellen Eingriffe erfolgen.

---

## CSV-Ausgabe

- **Datei:** `docs/data/<ID>_<yyyy-MM-dd_HHmmss>.csv` im Repository (anderer Ordner per `--output`), es wird nichts überschrieben
- **Format:** Semikolon als Trenner, Dezimalkomma (de-AT), UTF-8 mit BOM. Das deutsche Excel öffnet die Datei direkt richtig.
- **Eine Zeile pro Schritt** plus eine Startzeile und eine Zeile bei jedem Neustart. Während einer Pause entstehen keine Zeilen.
- **`Lauf`** (seit LB02): Simulationslauf des Masters; ändert sich bei jedem Neustart mit neuer Zeit (dann springt `Zeit`).

```
Zeit;Lauf;Modus;Saeule_Ein;Gesamtlimit_kW;Gesamtleistung_kW;Energie_kWh;Abgewiesen;Warteschlange;
P1_Zustand;P1_Fahrzeug;P1_SOC;P1_Leistung_kW;P1_Temp_C;P1_Energie_kWh;
P2_Zustand;P2_Fahrzeug;P2_SOC;P2_Leistung_kW;P2_Temp_C;P2_Energie_kWh
```

Änderungen des Limits (`Gesamtlimit_kW`), Störungen (`Px_Zustand = Faulted`, Leistung 0, Temperatur) und der Modus sind damit im Verlauf erkennbar.

---

## Projektstruktur

Pfade relativ zum Repository-Hauptordner `dotnet-energy-twin-alpha/`:

```
src/Devices/DualPortCharger/
├── DualPortCharger.csproj            Projekt (Einstellungen aus src/Directory.Build.props)
├── Program.cs                        Einstieg
├── Models/                           Vehicle, VehicleCatalog, ChargingPoint, ChargingPointState, DualCharger
├── Simulation/                       SimulationEngine, AutoModeController, SimulationConfig, ChargerModbus
├── IO/                               ConsoleRenderer, InputHandler, CsvLogger, ArgsParser
└── Properties/launchSettings.json    Startprofile „Master“ und „Standalone“ für Rider/Visual Studio

docs/                                 zentral im Repository
├── data/                             CSV-Ausgaben (<ID>_<Zeitstempel>.csv)
├── devices/DualPortCharger/          design-notes.md, simulationsablauf.md
├── screenshots/DualPortCharger/      Screenshots
└── development-journal/susanne-lastowicka.md   Entwicklungsjournal
```


---

## Modbus-Server und Registerplan

Jede Säule hostet einen eigenen Modbus-TCP-Server (EasyModbusTCP.NETCore, Baustein `Devices.Common.Modbus`, Anbindung in
`Simulation/ChargerModbus.cs`, Registerdefinition in `src/Modbus.Contracts/ChargerRegisters.cs`). Er startet mit dem Programm, läuft parallel zur Simulation und bleibt bei **Pause** und bei
verlorenem Master erreichbar; die Register zeigen dann den zuletzt berechneten Zustand (Register „Simulation läuft“ = 0).
Zwei Säulen auf einem Rechner brauchen verschiedene Ports (`--id DC-02 --modbus-port 1503`). Ist der Port belegt, endet das
Programm sofort mit einer Fehlermeldung.

### Registerplan

Holding Registers (Funktionscode 03 lesen, 06/16 schreiben). Standard: Host `127.0.0.1`, Port `1502`, Unit-ID `1` – alle drei sind konfigurierbar.
Adressen sind Protokolladressen (0-basiert, Referenzadresse = 40001 + Adresse). Pro Register Big-Endian;
mehrteilige Werte (`UInt32`, `Float32`) belegen zwei Register, das höherwertige Wort steht zuerst (Float32 = IEEE 754, Byte-Folge ABCD).
Mehrteilige Werte müssen mit **einem** Schreibzugriff (FC16) auf beide Register geschrieben werden.

| Adresse | Ref. | Name | Typ | Regs | Einheit | Zugriff | Schreibbereich | Bedeutung / Fehlerverhalten |
|---|---|---|---|---|---|---|---|---|
| 0 | 40001 | Gerätetyp | UInt16 | 1 |  | lesen | – | 1 PV, 2 Batterie, 3 Charger, 4 Wind |
| 1 | 40002 | Geräte-Nummer | UInt16 | 1 |  | lesen | – | Zahl am Ende der Geräte-ID (DC-02 → 2), 0 wenn keine |
| 2 | 40003 | Betriebszustand | UInt16 | 1 |  | lesen | – | 0 Aus, 1 Bereit/Leerlauf, 2 Betrieb, 3 Störung |
| 3 | 40004 | Fehlercode | UInt16 | 1 |  | lesen | – | 0 = kein Fehler, sonst gerätespezifisch (siehe Registerplan des Geräts) |
| 4–5 | 40005 | Wirkleistung | Float32 | 2 | kW | lesen | – | Aktuelle Wirkleistung; positiv = Aufnahme, negativ = Abgabe |
| 6–7 | 40007 | Kumulierte Energie | Float32 | 2 | kWh | lesen | – | Energiezähler seit Programmstart (Bedeutung je Gerät im Registerplan) |
| 8–9 | 40009 | Modellzeit | UInt32 | 2 | s | lesen | – | Virtuelle Zeit des dargestellten Zustands, Sekunden seit 1970-01-01 00:00 (ohne Zeitzone); 0 = noch keine Zeit |
| 10 | 40011 | Lauf | UInt16 | 1 |  | lesen | – | Lauf-Nummer; ändert sich bei jedem Neustart im Master (Modellzeit springt dann) |
| 11 | 40012 | Sequenznummer | UInt16 | 1 |  | lesen | – | Zählt bei jeder neuen Modellzeit hoch (Überlauf bei 65535); gleiche Nummer = gleicher Zustand |
| 12 | 40013 | Zeitstatus | UInt16 | 1 |  | lesen | – | 0 keine Zeit, 1 synchron mit Master, 2 Standalone, 3 Master verloren (Zeit steht) |
| 13 | 40014 | Simulation läuft | UInt16 | 1 |  | lesen | – | 1 = virtuelle Zeit läuft, 0 = pausiert oder keine Zeit; bei Pause zeigen die Register den letzten Zustand |
| 14–15 | 40015 | Tempo | Float32 | 2 | x | lesen | – | Simulationsgeschwindigkeit gegenüber Echtzeit |
| 16 | 40017 | Ein/Aus | UInt16 | 1 |  | lesen/schreiben | 0 … 1 | Lesen: 1 = Gerät ein. Schreiben: 0 aus, 1 ein |
| 17 | 40018 | Fehler zurücksetzen | UInt16 | 1 |  | lesen/schreiben | 1 … 1 | Schreiben von 1 setzt Störungen zurück, sofern das Gerät es zulässt; liest immer 0 |
| 18 | 40019 | Letzter Schreibfehler | UInt16 | 1 |  | lesen | – | 0 = letzter Schreibzugriff angenommen, 1 nicht beschreibbar, 2 ungültiger Wert, 3 Wert unvollständig geschrieben |
| 19 | 40020 | Adresse des letzten Schreibzugriffs | UInt16 | 1 |  | lesen | – | Erste Adresse des letzten Schreibzugriffs (bei Fehler: die fehlerhafte Adresse) |
| 20–21 | 40021 | Leistungsgrenze der Säule | Float32 | 2 | kW | lesen/schreiben | 50 … 350 | Gemeinsames Leistungsbudget beider Ladepunkte (Sollwert). Außerhalb 50–350 kW wird der Schreibzugriff abgelehnt (Letzter Schreibfehler = 2), der alte Wert bleibt |
| 22 | 40023 | Punkt 1: Zustand | UInt16 | 1 |  | lesen | – | 0 frei, 1 angesteckt, 2 lädt, 3 fertig, 4 Störung |
| 23–24 | 40024 | Punkt 1: Leistung | Float32 | 2 | kW | lesen | – | Mittlere Netzleistung des Punkts im letzten Schritt |
| 25–26 | 40026 | Punkt 1: SOC | Float32 | 2 | % | lesen | – | Ladezustand des Fahrzeugs; 0 wenn kein Fahrzeug angesteckt |
| 27–28 | 40028 | Punkt 1: Temperatur | Float32 | 2 | °C | lesen | – | Temperatur von Kabel und Stecker; Derating ab 60 °C, Störung ab 80 °C |
| 29 | 40030 | Punkt 2: Zustand | UInt16 | 1 |  | lesen | – | 0 frei, 1 angesteckt, 2 lädt, 3 fertig, 4 Störung |
| 30–31 | 40031 | Punkt 2: Leistung | Float32 | 2 | kW | lesen | – | Mittlere Netzleistung des Punkts im letzten Schritt |
| 32–33 | 40033 | Punkt 2: SOC | Float32 | 2 | % | lesen | – | Ladezustand des Fahrzeugs; 0 wenn kein Fahrzeug angesteckt |
| 34–35 | 40035 | Punkt 2: Temperatur | Float32 | 2 | °C | lesen | – | Temperatur von Kabel und Stecker; Derating ab 60 °C, Störung ab 80 °C |
| 36 | 40037 | Warteschlange | UInt16 | 1 | Fahrzeuge | lesen | – | Wartende Fahrzeuge (max. 5 min) |
| 37 | 40038 | Abgewiesene Fahrzeuge | UInt16 | 1 | Fahrzeuge | lesen | – | Seit Programmstart weggefahren, weil die Wartezeit abgelaufen ist (Sättigung bei 65535) |

### Bedeutung für die Säule

- **Betriebszustand:** Aus (Säule ausgeschaltet), Störung (mindestens ein Ladepunkt gestört), Betrieb (Leistung > 0), sonst Bereit.
- **Fehlercode:** Bitmaske, Bit 0 = Ladepunkt 1 gestört, Bit 1 = Ladepunkt 2 gestört (3 = beide).
- **Wirkleistung / Energie:** Summe der Netzleistung bzw. der abgegebenen Netzenergie beider Punkte (positiv, die Säule nimmt auf);
  der Energiezähler zählt seit Programmstart und läuft bei einem Neustart im Master weiter.
- **Modellzeit:** Zeit des zuletzt berechneten Schritts; die **Sequenznummer** wächst mit jeder neuen Modellzeit. Zwei Lesezugriffe
  mit gleicher Sequenznummer und gleichem Lauf gehören zum selben Zustand; ändert sich die Nummer zwischen Anfang und Ende
  eines Lesevorgangs, sollte der Client noch einmal lesen (es gibt keine atomare Lesegarantie über mehrere Register).
- **Ein/Aus (16):** schaltet die Säule (wie Taste `O`); Aus stoppt die Ladung, Fahrzeuge bleiben angesteckt.
- **Fehler zurücksetzen (17):** Schreiben von 1 setzt gestörte Punkte zurück, wenn sie unter 60 °C sind (wie Taste `R`).
  Sonst keine Wirkung, die Meldung steht in der Konsole der Säule.
- **Leistungsgrenze (20–21):** Sollwert, 50–350 kW. Er wird sofort übernommen und ist auch in der Pause wirksam.

### Fehlerverhalten bei Schreibzugriffen

Die Bibliothek EasyModbusTCP.NETCore übernimmt einen Schreibzugriff zuerst und meldet ihn erst danach an die Anwendung; das Modbus-Protokoll
bietet dort keinen Weg, vorher mit einer Ausnahmeantwort abzulehnen (die Bibliothek darf laut Aufgabe nicht verändert werden).
Die Säule geht deshalb so vor:

1. Ungültige Zugriffe werden **sofort rückgängig gemacht**, das Register zeigt wieder den alten Wert. Der Zugriff gilt als Ganzes: Ist ein Teil ungültig, wird nichts übernommen.
2. Ungültig sind: nur lesbare oder nicht belegte Adressen (Letzter Schreibfehler = 1), Werte außerhalb des Bereichs, `NaN`/`∞` (= 2), und
   ein Float, von dem nur ein Register geschrieben wird (= 3, Floats immer mit FC16 auf beide Register).
3. Register 18 (Fehlerart) und 19 (Adresse) halten das Ergebnis des **letzten** Schreibzugriffs fest (0 = angenommen); die Konsole zeigt eine Meldung.
   Der Diagnose-Client prüft diese Register nach jedem Schreiben.

### Prüfen mit dem Diagnose-Client

```
dotnet run --project src/Devices/DualPortCharger -- --standalone --speed 600 --modbus-port 1502
```
In einem zweiten Terminal:
```
dotnet run --project src/ModbusDiagnosticClient -- --port 1502
```
(Befehle `read`, `write Leistungsgrenze 200`, `force 20 999` – siehe `src/ModbusDiagnosticClient/README.md`.) Ein beliebiger anderer Modbus-Client geht auch
(Holding Registers ab Adresse 0, Anzahl 38). Den Registerplan als Markdown erzeugt `--register-plan`.

---

## LB02 – Umbau zum Feldgerät im Gesamtsystem

**Zweck:** Dual-Port-DC-Ladesäule als Feldgerät, das der gemeinsamen virtuellen Zeit folgt (mit Modbus-Server).
**Verantwortlich:** Susanne Lastowicka (Gerät aus LB01)
**Basis aus LB01:** Susi DualCharger, ChargingPoint, Vehicle, VehicleCatalog, AutoModeController

### Stand (2026-10-25)

| Schritt | Stand |
|---|---|
| Zeit vom Simulations-Master (`SyncedClock`, `TimeSyncClient`) | ✅ |
| Standalone-Betrieb mit lokaler Uhr (`--standalone`) | ✅ |
| async Schleife, CSV, Tastatur (kein `Thread.Sleep`, kein `ReadLine`) | ✅ |
| Modbus-Server, Registerplan (Kopf 0–19, Säule 20–38; Server bleibt in der Pause erreichbar) | ✅ (2026-10-25) |
| Umgebungstemperatur aus dem Wetter (`--weather`, `--location`; Standard mit Master) | ✅ (2026-10-26) |

### Schnittstellen
- **Benötigt:** Simulations-Master (TCP, Standard `127.0.0.1:5100`, Protokoll in `src/SimCore.TimeSync/README.md`); optional Wettersimulator (HTTP, Standard `127.0.0.1:8080`).
- **Angeboten:** CSV-Datei; Modbus-TCP-Server (Registerplan oben, Standardport 1502); Gegenstück: `src/ModbusDiagnosticClient`.

### Tests
- Der Modbus-Baustein (Registerplan, Lesen, gültige und ungültige Schreibzugriffe, Pause) ist in `tests/Devices.Common.Tests` getestet.
- Die Zeitanbindung ist in `tests/Devices.Common.Tests` getestet (Schrittsteuerung, Optionen, Zeitquelle mit echtem Master über TCP).
- Probeläufe am 2026-10-25 (umgeleitete Ausgabe):
  - Standalone `--start "2026-01-15 00:00" --speed 100000 --duration 24 --seed 42`: endet von selbst mit Exit-Code 0,
    CSV mit 290 Zeilen von 2026-01-15 00:00 bis 2026-01-16 00:00.
  - Master (x600) + DC-01 (5 min) + DC-02 (1 min): beide „synchron“, Abweichung ≤ 0,2 s (Toleranz 6 s), Master listet beide.
    Master hart beendet ⇒ beide melden nach 3 s „MASTER NICHT ERREICHBAR – Zeit steht“.

### Bekannte Probleme
- Mit Master fragt die Säule standardmäßig den Wettersimulator; läuft er nicht, erscheint nach 3 Fehlversuchen „WETTER: NICHT ERREICHBAR“, die Säule rechnet mit dem letzten Wetter (bzw. 20 °C) weiter.
- Modbus-Schreibfehler werden nicht mit einer Modbus-Ausnahmeantwort gemeldet, sondern über Register 18/19 (Grenze der Bibliothek, siehe oben).
- Interaktive Anzeige und Tasten nach dem Umbau noch nicht in einem echten Terminalfenster getestet.
- Ein Neustart im Master behält den Zustand der Säule (Fahrzeuge bleiben angesteckt); ob das gewünscht ist, im Team klären.
- Die Screenshots stammen noch aus LB01.

---

**Quelle:** `src/Devices/PvSimulator/README.md`, Abschnitt „Jahreszeiten“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../src/Devices/PvSimulator/README.md)

### Unveränderter Originalabschnitt

### Jahreszeiten

Die Jahreszeit ergibt sich aus dem Monat und beeinflusst Einstrahlung, Tageslänge,
Umgebungstemperatur und Schneewahrscheinlichkeit:

| Jahreszeit | Monate | Einstrahlungsfaktor | Sonnenaufgang | Sonnenuntergang | Umgebungstemperatur | Schnee-Wahrscheinlichkeit |
|------------|--------|--------------------|---------------|-----------------|----------------------|--------------------------|
| FRÜHLING | 03–05 | 0.75 | 06:00 | 18:00 | 11 °C | 3 % |
| SOMMER | 06–08 | 1.00 | 04:30 | 20:00 | 21 °C | 0 % |
| HERBST | 09–11 | 0.60 | 06:30 | 17:30 | 10 °C | 8 % |
| WINTER | 12–02 | 0.40 | 08:00 | 16:00 | 2 °C | 40 % |


---

**Quelle:** `src/Devices/PvSimulator/README.md`, Abschnitt „Konsolenausgabe“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../src/Devices/PvSimulator/README.md)

### Unveränderter Originalabschnitt

## Konsolenausgabe

Mit exakt ausgerichteten Rahmen:

- Datum, Uhrzeit, Jahreszeit, Modus, Betriebszustand, Zeitraffer
- Zeitquelle, Modbus und Wetter in eigenen, farblich hervorgehobenen Statuszeilen (lange Texte werden umbrochen)
- Tagesfortschritt als Balken über 24 h
- Wirkleistung (kW), Tagesenergie (kWh), Gesamtenergie (kWh)
- Wetter, Einstrahlung (W/m²), Fehlerursache
- Modultemperatur, Außentemperatur, Netzspannung, Verschmutzung, Alterung, Wirkungsgrad
- Auslastung als Balken in %

Die Bildschirmaktualisierung ist von der Simulationsrate entkoppelt und standardmäßig
auf 4 Bilder/s begrenzt, damit alles lesbar bleibt. Per `-r`/`--refresh` konfigurierbar,
per Taste `R` im Betrieb umschaltbar (100 – 3000 ms).

Das Bild wird **differentiell** gezeichnet: Es wird nur beschrieben, was sich seit dem
vorigen Bild tatsächlich geändert hat (Cursor-Sprung + Zeichen bis Zeilenende löschen).
Dadurch entsteht kein Flackern. Bei umgeleiteter Ausgabe (Datei, CI, Pipe) steht keine
Cursor-Steuerung zur Verfügung – dann wird das ganze Bild geschrieben.

Diagnose: `ENERGYSIM_RENDER_LOG=<datei>` protokolliert pro Bild, wie viele Zeilen
wirklich angefasst wurden (z. B. `diff;rows=20;written=4`).
- Gelbe Hinweis-/Fehlermeldungen, einschließlich langer Verbindungsfehler, werden vollständig an Wortgrenzen
  innerhalb der Rahmenbreite umbrochen; ungetrennte lange Zeichenfolgen werden nötigenfalls geteilt.
  Bestätigungsmeldungen verschwinden nach 4 s.

Die frühere Einleitung und der separate datierte Anzeigehinweis sind im
[README-Archiv](../../../docs/archive/2026-11-08-pv-readme.md) mit Herkunft und Änderungsgrund erhalten.


---

**Quelle:** `src/Devices/PvSimulator/README.md`, Abschnitt „Konfiguration“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../src/Devices/PvSimulator/README.md)

### Unveränderter Originalabschnitt

### Konfiguration

| Option | Bedeutung | Standard |
|---|---|---|
| `--id <text>` | Geräte-ID (Master-Anzeige, Modbus-Geräte-Nummer, CSV-Name) | `PV-01` |
| `-p, --peak <kW>` | Nennleistung | 10.0 |
| `-t, --step <min>` | CSV-Intervall | 15 |
| `--model-step <min>` | feste Schrittweite des Modells | 1 |
| `-r, --refresh <ms>` | Bildschirmaktualisierung, mind. 50 | 400 |
| `-o, --output <ordner>` | CSV-Ordner | `docs/data` im Repo |
| `--seed <zahl>` | Zufalls-Seed für Auto-Wetter und Netzrauschen | zufällig |
| `--weather <url\|off>` | Wettersimulator; `off` = lokales Wetter | mit Master `http://127.0.0.1:8080`, standalone `off` |
| `--location <kennung>` | Standort für die Wetterabfrage (`salzburg`, `wien`, `innsbruck`) | `salzburg` |
| `--master <host:port>` | Simulations-Master | `127.0.0.1:5100` |
| `--standalone` | ohne Master, lokale Uhr | aus |
| `-d, --start <zeit>` | nur standalone, `yyyy-MM-dd HH:mm` | heute 00:00 |
| `-s, --speed <x>` | nur standalone, Tempo 1–100 000 | 60 |
| `--modbus-port <port>` | Modbus-TCP-Port, pro Gerät eigener | 1502 |
| `--modbus-bind <ip>` | Adresse des Servers | 127.0.0.1 |
| `--unit-id <n>` | Modbus-Unit-ID | 1 |
| `--no-modbus` | Server nicht starten | aus |
| `--register-plan` | Registerplan als Markdown ausgeben | |

`-s` und `-d` aus LB01 bleiben als Kürzel für `--speed` und `--start`, bedeuten aber nur noch standalone etwas (`--start` jetzt mit Uhrzeit).
Die Option `--timescale` gibt es nicht mehr; mit Master bestimmt der Master das Tempo.


---

**Quelle:** `src/Devices/PvSimulator/README.md`, Abschnitt „Modbus-Registerplan“
**Grund:** Gemeinsame Enum-Legende um bereits implementierten Gerätetyp 5 ergänzt; Registeradressen und Fachwerte unverändert
**Aktueller Ort:** [README](../../src/Devices/PvSimulator/README.md)

### Unveränderter Originalabschnitt

### Modbus-Registerplan

Holding Registers (Funktionscode 03 lesen, 06/16 schreiben). Standard: Host `127.0.0.1`, Port `1502`, Unit-ID `1` – alle drei sind konfigurierbar.
Adressen sind Protokolladressen (0-basiert, Referenzadresse = 40001 + Adresse). Pro Register Big-Endian;
mehrteilige Werte (`UInt32`, `Float32`) belegen zwei Register, das höherwertige Wort steht zuerst (Float32 = IEEE 754, Byte-Folge ABCD).
Mehrteilige Werte müssen mit **einem** Schreibzugriff (FC16) auf beide Register geschrieben werden.

| Adresse | Ref. | Name | Typ | Regs | Einheit | Zugriff | Schreibbereich | Bedeutung / Fehlerverhalten |
|---|---|---|---|---|---|---|---|---|
| 0 | 40001 | Gerätetyp | UInt16 | 1 |  | lesen | – | 1 PV, 2 Batterie, 3 Charger, 4 Wind |
| 1 | 40002 | Geräte-Nummer | UInt16 | 1 |  | lesen | – | Zahl am Ende der Geräte-ID (DC-02 → 2), 0 wenn keine |
| 2 | 40003 | Betriebszustand | UInt16 | 1 |  | lesen | – | 0 Aus, 1 Bereit/Leerlauf, 2 Betrieb, 3 Störung |
| 3 | 40004 | Fehlercode | UInt16 | 1 |  | lesen | – | 0 = kein Fehler, sonst gerätespezifisch (siehe Registerplan des Geräts) |
| 4–5 | 40005 | Wirkleistung | Float32 | 2 | kW | lesen | – | Aktuelle Wirkleistung; positiv = Aufnahme, negativ = Abgabe |
| 6–7 | 40007 | Kumulierte Energie | Float32 | 2 | kWh | lesen | – | Energiezähler seit Programmstart (Bedeutung je Gerät im Registerplan) |
| 8–9 | 40009 | Modellzeit | UInt32 | 2 | s | lesen | – | Virtuelle Zeit des dargestellten Zustands, Sekunden seit 1970-01-01 00:00 (ohne Zeitzone); 0 = noch keine Zeit |
| 10 | 40011 | Lauf | UInt16 | 1 |  | lesen | – | Lauf-Nummer; ändert sich bei jedem Neustart im Master (Modellzeit springt dann) |
| 11 | 40012 | Sequenznummer | UInt16 | 1 |  | lesen | – | Zählt bei jeder neuen Modellzeit hoch (Überlauf bei 65535); gleiche Nummer = gleicher Zustand |
| 12 | 40013 | Zeitstatus | UInt16 | 1 |  | lesen | – | 0 keine Zeit, 1 synchron mit Master, 2 Standalone, 3 Master verloren (Zeit steht) |
| 13 | 40014 | Simulation läuft | UInt16 | 1 |  | lesen | – | 1 = virtuelle Zeit läuft, 0 = pausiert oder keine Zeit; bei Pause zeigen die Register den letzten Zustand |
| 14–15 | 40015 | Tempo | Float32 | 2 | x | lesen | – | Simulationsgeschwindigkeit gegenüber Echtzeit |
| 16 | 40017 | Ein/Aus | UInt16 | 1 |  | lesen/schreiben | 0 … 1 | Lesen: 1 = Gerät ein. Schreiben: 0 aus, 1 ein |
| 17 | 40018 | Fehler zurücksetzen | UInt16 | 1 |  | lesen/schreiben | 1 … 1 | Schreiben von 1 setzt Störungen zurück, sofern das Gerät es zulässt; liest immer 0 |
| 18 | 40019 | Letzter Schreibfehler | UInt16 | 1 |  | lesen | – | 0 = letzter Schreibzugriff angenommen, 1 nicht beschreibbar, 2 ungültiger Wert, 3 Wert unvollständig geschrieben |
| 19 | 40020 | Adresse des letzten Schreibzugriffs | UInt16 | 1 |  | lesen | – | Erste Adresse des letzten Schreibzugriffs (bei Fehler: die fehlerhafte Adresse) |
| 20–21 | 40021 | Leistungsbegrenzung | Float32 | 2 | % | lesen/schreiben | 0 … 100 | Sollwert: begrenzt die Einspeisung auf diesen Anteil der Nennleistung (Einspeisemanagement). Außerhalb 0–100 % wird der Schreibzugriff abgelehnt (Letzter Schreibfehler = 2), der alte Wert bleibt |
| 22–23 | 40023 | Einstrahlung | Float32 | 2 | W/m² | lesen | – | Aktuelle Einstrahlung auf die Module |
| 24–25 | 40025 | Modultemperatur | Float32 | 2 | °C | lesen | – | Überhitzung (Fehlercode 2) ab 70 °C, Freigabe unter 55 °C |
| 26–27 | 40027 | Außentemperatur | Float32 | 2 | °C | lesen | – | Umgebungstemperatur der Anlage |
| 28–29 | 40029 | Netzspannung | Float32 | 2 | V | lesen | – | Netzspannung am Anschlusspunkt; außerhalb 200–250 V Störung (Fehlercode 3) |
| 30–31 | 40031 | Tagesenergie | Float32 | 2 | kWh | lesen | – | Erzeugte Energie seit 00:00 Modellzeit |
| 32–33 | 40033 | Verschmutzung | Float32 | 2 | % | lesen | – | Ertragsverlust durch Verschmutzung (max. 25 %) |
| 34–35 | 40035 | Alterung | Float32 | 2 | % | lesen | – | Ertragsverlust durch Alterung (max. 15 %) |
| 36 | 40037 | Wetter | UInt16 | 1 |  | lesen | – | 0 sonnig, 1 bewölkt, 2 dicht bewölkt, 3 Gewitter, 4 Schnee, 5 Nacht |
| 37–38 | 40038 | Nennleistung | Float32 | 2 | kW | lesen | – | Konfigurierte Nennleistung (--peak) |

Besonderheiten der PV-Anlage:
- **Wirkleistung negativ** = Einspeisung (Vorzeichenregel aller Geräte: positiv = Aufnahme). **Energie** im Kopf = erzeugte Gesamtenergie.
- **Betriebszustand:** Aus (per Modbus/Ein-Aus abgeschaltet), Störung, Betrieb (Leistung > 0), sonst Bereit (z. B. nachts).
- **Fehlercode:** 0 keiner, 1 Wechselrichter, 2 Überhitzung (≥ 70 °C), 3 Netzspannung (außerhalb 200–250 V).
- **Sollwert Leistungsbegrenzung (20–21):** 0–100 % der Nennleistung, wirkt ab dem nächsten Modellschritt. Ungültige Werte werden abgelehnt
  (Register 18 = 2) und die Anlage behält den alten Wert.
- **Fehler zurücksetzen (17):** hebt von Hand ausgelöste Störungen auf (Inverterfehler, erzwungene Überhitzung, erzwungene Netzstörung).
  Eine natürliche Überhitzung geht erst unter 55 °C weg.
- **Modellzeit/Sequenz:** Zeit des zuletzt berechneten Schritts; in der Pause bleiben alle Werte stehen.
- Fehlerverhalten bei Schreibzugriffen (Rückgängigmachen, Register 18/19): siehe `src/Devices/DualPortCharger/README.md`, Abschnitt „Fehlerverhalten bei Schreibzugriffen“.


---

**Quelle:** `src/WeatherSimulator/README.md`, Abschnitt „Schnittstelle“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../src/WeatherSimulator/README.md)

### Unveränderter Originalabschnitt

## Schnittstelle

HTTP-Server auf `http://127.0.0.1:<port>` (Standard 8080).

| Anfrage | Antwort |
|---|---|
| `GET /weather?location=<kennung>&simTime=yyyy-MM-ddTHH:mm:ss&run=<lauf>` | `WeatherData` als JSON |
| `GET /locations` | unterstützte Standorte (Kennung, Name, Koordinaten) |
| `GET /` | Kurzbeschreibung und Endpunkt |

- `location` ist optional, Standard `salzburg`; unbekannte Kennung ⇒ **404**.
- `simTime` ist UTC; fehlend/ungültig ⇒ **400**. Antwortfelder werden auf eine Nachkommastelle gerundet.
- `run` ist die Lauf-Nummer des Masters (Standard 0); sie fließt in den Seed ein, damit ein neuer Lauf neues Wetter ergibt.
- Die Antwort ist innerhalb einer Stunde konstant (Minuten/Sekunden ändern nichts).

Beispielantwort:

```json
{
  "location": "salzburg",
  "simTimeUtc": "2026-06-21T12:00:00Z",
  "runId": 1,
  "sunHeightDegrees": 64.71,
  "cloudCoverPercent": 22.4,
  "temperatureC": 24.3,
  "precipitationMmPerHour": 0.0,
  "windSpeedMs": 3.1,
  "irradiationWm2": 812.7,
  "condition": "PartlyCloudy"
}
```

Zustände: `Sunny`, `PartlyCloudy`, `Overcast`, `Storm`, `Snow`, `Night`.


---

**Quelle:** `src/WeatherSimulator/README.md`, Abschnitt „Modell“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise; Weiterhin gültige Zustandsregel im Modellabschnitt gebündelt
**Aktueller Ort:** [README](../../src/WeatherSimulator/README.md)

### Unveränderter Originalabschnitt

## Modell

- **Basis der österreichischen Standorte:** WeatherSynth 0.3.0 erzeugt für (Standort, Kalenderjahr, Seed) stündliche Werte: Globalstrahlung,
  Klarhimmel-Strahlung und Wind (10 m, logarithmisches Profil). Offline-Bibliothek, DWD-Daten (CC BY 4.0).
- **Seed:** `runId · 7919 + FNV-1a(Standort|Jahr) + Jahr`; kein `string.GetHashCode`, damit der Wert über
  Neustarts und Prozesse hinweg gleich bleibt. Gleicher (Ort, Zeit, Lauf) ⇒ byte-identische Antwort.
- **Strahlung:** Globalstrahlung der Stunde; bei Sonnenhöhe ≤ 0 auf 0 gesetzt. Sonnenhöhe aus Deklination,
  Stundenwinkel und Standortkoordinaten (UTC).
- **Bedeckung:** `(1 − Globalstrahlung / Klarhimmelstrahlung) · 100`, nachts vom letzten Tageslicht übernommen.
- **Temperatur:** Jahresmittel aus Höhenlage (`10 °C − 0.0065 · (Höhe − 200 m)`), Jahresgang, Tagesgang
  (Maximum ~15 Uhr, durch Bewölkung gedämpft) und ein pro Tag und Standort geseedeter Offset von ±3 K.
- **Niederschlag:** Wahrscheinlichkeit wächst mit der Bedeckung; Menge aus einem stundenweise geseedeten Wert
  (0.2–3.2 mm/h, verdoppelt bei dichter Bewölkung und Wind ≥ 10 m/s).
- **Zustand:** `Night` ohne Sonne; `Snow` bei Niederschlag und ≤ 1.5 °C; `Storm` ab 90 % Bedeckung und 14 m/s;
  `Overcast` ab 80 %, `PartlyCloudy` ab 30 %, sonst `Sunny`.
- **Ortszeit:** interne Rechnung UTC; Anzeige zeigt zusätzlich `Europe/Vienna`.

Standorte: `salzburg` (Standard), `wien`, `innsbruck`, `tamanrasset`, `neumayer`.

### Formeln und Quellen

Belegt oder nahe an etablierten Standards:

| Formel | Quelle |
|---|---|
| Sonnenhöhe aus Deklination und Stundenwinkel | Standard-Astronomie (Cooper 1969; Spencer 1971; Duffie & Beckman, „Solar Engineering of Thermal Processes“); identisch mit dem NOAA-Solarrechner |
| Höhen-Abnahme der Temperatur `−0,0065 K/m` | Internationale Standardatmosphäre (ICAO/ISA, Doc 7488) – gemittelte Troposphärenrate |
| Strahlung und Wind (1 h) | NuGet `WeatherSynth` 0.3.0 – DWD-Daten (CC BY 4.0) |
| logarithmisches Windprofil | Monin-Obukhov-Bodenlogarithmengesetz; Standard in der Windenergie (IEC 61400-12-1) |
| Sturm-Schwelle 14 m/s | Beaufortstufe 7/8 (13,9–17,1 m/s) |
| Schnee bei ≤ 1,5 °C | Rain-Snow-Übergang liegt bei nasser Kugel-Temperatur ≈ 0–2 °C (z. B. Jennings et al. 2018) |
| „dicht bewölkt“ ab 80 %, Bedeckungsgrad aus Strahlungsverhältnis | 8/8 Okta = 87,5 %; Zusammenhang Bewölkung ↔ Globalstrahlung z. B. Kasten & Czeplak 1980 |

Eigene einfache Heuristik (deterministisch, offline – entsprechend der Aufgabenstellung, **nicht** aus Literatur kalibriert):

| Formel | Anmerkung |
|---|---|
| Jahresmittel `10 °C − 0.0065 · (Höhe − 200 m)` | Referenzwert 10 °C ist eine Annahme; trifft zufällig gut auf die Klimanormale von Salzburg (≈ 9,5 °C, 1991–2020, DWD/AustroControl) |
| Jahresgang `−9,5·cos(2π·(Tag−15)/365)` | Amplitude −9,5 K ist gefittet (Jan ≈ 0,5 / Jul ≈ 19,5 °C ≈ Salzburg); keine direkte Kalibrierung |
| Tages-Offset ±3 K aus dem Seed | vereinfachend weißes Rauschen je Tag; reale Wetterlagen wandern über Tage |
| Niederschlagswahrscheinlichkeit `Bedeckung → 0,25` | erfundene Skala, Wüste/Tropen unterschieden das nicht |
| Bewölkungsmäßige Dämpfung des Tagesgangs (Faktor 0,3) | Richtung belegt (Wolken dämpfen die Amplitude), Faktor nicht kalibriert |
| Nachts Bewölkung 40 % | Konvention |

Die österreichischen Standorte behalten diese vereinfachte Modellbasis. WeatherSynth.Default wurde mit Solarwerten
aus Bochum und Windwerten aus Essen-Bredeney gefittet und wird deshalb nicht als Klimabasis auf Wüste/Antarktis übertragen.

### Klimaprofile für Sahara und Antarktis

Die neuen Orte verwenden `ClimateProfile` und dasselbe deterministische Modell für HTTP und MQTT. Keine Laufzeit-Downloads.

| Standort | Referenzgeometrie | Temperaturbasis | Niederschlag |
|---|---|---|---|
| Tamanrasset | 22,80° N, 5,43° E, 1362 m | monatliche JMA-Normalwerte | monatliche JMA-Normalwerte (Summe 56,2 mm/Jahr) |
| Neumayer III | ca. 70,667° S, 8,267° W, nominal 40 m | gerundete saisonale Näherung an BAS-Monatsbeobachtungen | synthetische Annahme 370 mm Wasseräquivalent/Jahr |

Tamanrasset: [JMA ClimatView, Station 60680](https://www.data.jma.go.jp/tcc/tcc/products/climate/climatview/graph_mkhtml.php?d=0&e=0&k=0&m=7&n=60680&r=0&s=1&y=2026).
Verwendet werden die **Normal**-Spalten, keine Beobachtungen einzelner Jahre; Bezugsperiode wird auf der verwendeten
Ansicht nicht ausgewiesen. Temperatur Jan–Dez (°C): 12,8 / 15,3 / 19,2 / 23,8 / 27,7 / 30,0 / 30,1 / 29,1 / 27,9 / 23,8 / 18,2 / 14,4.
Niederschlag (mm): 1,1 / 0,9 / 5,2 / 0,1 / 1,5 / 5,7 / 7,5 / 18,5 / 6,5 / 8,2 / 0,4 / 0,6.

Neumayer: [BAS READER, Monatsbeobachtungen seit 1981](https://legacy.bas.ac.uk/met/READER/surface/Neumayer.All.temperature.html),
[AWI Stationsbeschreibung](https://www.awi.de/en/fleet-stations/stations/neumayer-station-iii.html).
Die verwendete Temperaturkurve ist eine **eigene gerundete Näherung**, kein berechnetes Klimanormalmittel:
−4,2 / −8,0 / −13,0 / −17,5 / −20,5 / −23,5 / −25,0 / −25,0 / −23,0 / −17,5 / −10,0 / −4,8 °C.
Der Referenzpunkt ist fest, obwohl die Station auf dem Schelfeis wandert; die nominale Höhe ist eine Modellannahme.
Die südliche Jahreszeitenlage folgt dieser Monatskurve.

**Formeln:** Zwischen Monatsmitten wird linear interpoliert. Temperatur = Monatskurve + geseedeter Tagesoffset
(Tamanrasset ±3 K, Neumayer ±6 K) + lokaler solarer Tagesgang (Amplitude 7 bzw. 1,5 K, durch Wolken gedämpft).
In Polarnacht entfällt der solare Tagesgang. Bewölkung = regionaler Mittelwert (20 bzw. 70 %) plus geseedete Tagesabweichung
±35 Prozentpunkte, auf 0–100 % begrenzt. Das sind vereinfachende Annahmen, keine angepassten Messverteilungen.

Wind wird als Rayleigh-Verteilung mit Mittel 4,0 m/s (Sahara, **Annahme**) bzw. 8,7 m/s (Neumayer,
[Beobachtungsstudie](https://tc.copernicus.org/articles/15/4745/2021/tc-15-4745-2021.html)) erzeugt.
Stündliche Windwerte sind unabhängig; reale zeitliche Persistenz und Katabatik werden nicht modelliert.
Niederschlag tritt mit Wahrscheinlichkeit `Monatsmenge / (Monatsstunden · 0,5 mm)` auf;
Mengen sind exponentiell mit Mittel 0,5 mm pro nasser Stunde. Damit entspricht die monatliche Menge im Erwartungswert
der Vorgabe, ein einzelner Lauf darf abweichen. Nasse Stunden erhalten mindestens 85 % Bedeckung.
Neumayers Niederschlagsverteilung und beide Wolken-/Tagesgangparameter sind ausdrücklich synthetisch.

Klarhimmelstrahlung: `1098 · cos(Z) · exp(−0,059/cos(Z))` nach
[Haurwitz, Referenzimplementierung pvlib](https://github.com/pvlib/pvlib-python/blob/main/pvlib/clearsky.py);
bei Sonnenhöhe ≤ 0 exakt 0. Wir verwenden die geometrische Sonnenhöhe ohne Refraktionskorrektur.
Wolkendämpfung `1 − 0,75 · (Bedeckung/100)^3,4` ist eine vereinfachte Modellannahme; lokale Aerosole, Staub,
Albedo und Mehrfachreflexion an Schnee werden nicht kalibriert. Keine wissenschaftlich validierte Polareinstrahlung.

Schnee und Sturm haben Vorrang vor `Night`, sodass Dunkelheit das Wetterereignis nicht verdeckt.
MQTT führt zusätzlich `isDaylight`. Diese Änderung betrifft die Zustandsbezeichnung auch an österreichischen Standorten,
deren numerische Wetterbasis sonst unverändert bleibt.


---

**Quelle:** `src/WeatherSimulator/README.md`, Abschnitt „MQTT-Betrieb und Viewer“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../src/WeatherSimulator/README.md)

### Unveränderter Originalabschnitt

## MQTT-Betrieb und Viewer

`--mqtt` aktiviert den zusätzlichen Ausgang und die Master-Anbindung. `--env-file <pfad>` überschreibt den Standard
`docker/.env`. `ENERGYTWIN_MQTT_USER`/`ENERGYTWIN_MQTT_PASSWORD` haben Vorrang. Zugangsdatenauflösung gemeinsam
mit dem Gateway in `Shared.Config`; kein Passwort auf der Kommandozeile.

Einmal pro realer Sekunde neueste Wetterstände aller Orte. Pause: einmal `paused`, keine regelmäßigen Werte;
manuelle Änderungen senden den betroffenen Standort einmal. Fortsetzen: einmal `continued`. Master fehlt/verloren:
`noTime`/`masterLost`, keine neuen Wetterwerte. Neue RunId oder neue Master-Sitzung löschen manuelle Vorgaben.
Broker-Ausfall beeinträchtigt HTTP nicht; automatische Wiederverbindung, keine Nachlieferung alter Messwerte.

MQTT-Viewer: `127.0.0.1:1883`, Zugangsdaten aus `.env`; für gespeicherte Zustände die sechs konkreten Topics aus dem
[Wettervertrag](../../docs/Wetter-Nachrichtenvertrag-LB03.md) abonnieren. Der MQTT-Status muss bei der Gültigkeitsbewertung
gespeicherter Wetterwerte berücksichtigt werden. Es darf nur ein aktiver Wetter-Publisher diese Topics bedienen.

Gemeinsamer Start: `.\start-all.ps1 -Autostart -NoClient -WeatherMqtt` (Broker vorher starten).
Ein bereits laufender Wetterdienst wird vom Startskript übersprungen; für MQTT muss dieser mit `--mqtt` gestartet sein.


---

**Quelle:** `docker/README.md`, Abschnitt „RabbitMQ für LB03“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../docker/README.md)

### Unveränderter Originalabschnitt

# RabbitMQ für LB03

**Verantwortlich:** Alexander Lux · **Stand:** 2026-11-08

Lokaler Nachrichtenbroker für das Modbus–RabbitMQ-Gateway. Docker Desktop muss mit Linux-Containern laufen;
die eigenen .NET-Programme laufen auf dem Host. Beim ersten Start wird das Image aus dem Internet geladen,
der Simulationsbetrieb selbst benötigt kein Internet.

## Einrichtung und Start

Alle folgenden Befehle werden im **Repository-Root** in PowerShell ausgeführt.

1. Docker Desktop starten und auf die laufende Engine warten (`docker info`).
2. Einmalig `Copy-Item docker/.env.example docker/.env` ausführen, sofern noch keine `.env` existiert.
3. In `docker/.env` den Passwort-Platzhalter durch ein eigenes zufälliges Passwort ersetzen.
   Benutzer und Passwort werden vom Broker und Gateway aus derselben Datei gelesen.
   Für identische Auswertung in Compose und Gateway einfache Werte ohne Leerzeichen, `$` oder Inline-Kommentare
   verwenden (z. B. ein zufälliges Hex-Passwort). `.env` ist durch `.gitignore` ausgeschlossen.
4. Konfiguration prüfen und Broker starten:

```powershell
docker compose --env-file docker/.env -f docker/docker-compose.yml config --quiet
docker compose --env-file docker/.env -f docker/docker-compose.yml up -d --wait --wait-timeout 180
docker compose --env-file docker/.env -f docker/docker-compose.yml ps
```

`config --quiet` prüft ohne Zugangsdaten auszugeben. Fehlende oder leere Zugangsdaten brechen die Konfiguration ab.
Die Beispielwerte sind Platzhalter und müssen vor dem Start ersetzt werden.

## Konfiguration und Kontrolle

| Einstellung | Wert |
|---|---|
| Image | `rabbitmq:4.4.0-management` |
| Compose-Projekt | `energytwin` |
| MQTT | `127.0.0.1:1883`, Plugin `rabbitmq_mqtt`, keine anonyme Anmeldung |
| AMQP | `127.0.0.1:5672` |
| Management | <http://localhost:15672>, Plugin `rabbitmq_management` |
| Zugangsdaten | `RABBITMQ_DEFAULT_USER` und `RABBITMQ_DEFAULT_PASS` in `docker/.env` |
| Broker-Konfiguration | `rabbitmq.conf`, nur lesend eingebunden |
| Plugins | `enabled_plugins`, nur lesend eingebunden |
| Daten | benanntes Volume `energytwin_rabbitmq-data`, stabiler Hostname |
| Bereitschaft | laufende RabbitMQ-Anwendung und erreichbare Listener; `up --wait` wartet darauf |

Die Ports sind nur auf diesem Rechner erreichbar. Anmeldung in der Management-Oberfläche mit den Werten aus `.env`.
Für einen MQTT-Viewer: Host `127.0.0.1`, Port `1883`, dieselben Zugangsdaten, kein TLS, Topic `energytwin/#`.
Die Management-Oberfläche ist eine Broker-Verwaltung, kein Ersatz für den Messwert-Viewer.

```powershell
docker compose --env-file docker/.env -f docker/docker-compose.yml exec rabbitmq rabbitmq-diagnostics listeners
docker compose --env-file docker/.env -f docker/docker-compose.yml logs --tail 60 rabbitmq
```

## Simulatoren und Gateway

```powershell
dotnet build EnergyTwin.sln
.\start-all.ps1 -Autostart -NoClient -WeatherMqtt
```

Das vorhandene Startskript öffnet Wetter, Master und alle vier Feldgeräte in eigenen Fenstern.
`-WeatherMqtt` schaltet den zusätzlichen Wetterstrom ein; ohne den Schalter läuft Wetter nur über HTTP.
Für gespeicherte Wetterwerte und Pausenstatus muss der Viewer die sechs konkreten Wettertopics aus dem
[Wettervertrag](../docs/Wetter-Nachrichtenvertrag-LB03.md) abonnieren. Ein bereits laufender Wetterdienst wird
übersprungen und muss für diese Funktion bereits mit `--mqtt` gestartet sein.
Danach in einem weiteren Terminal:

```powershell
dotnet run --no-build --project src/ModbusRabbitGateway
```

Das Gateway verwendet standardmäßig PV 1502, Batterie 1503, Charger 1504 und Kühlhaus 1505 sowie `docker/.env`.
Bereits gesetzte `ENERGYTWIN_MQTT_USER`/`ENERGYTWIN_MQTT_PASSWORD` haben Vorrang vor der Datei.
Zum kurzen Prüfen: `dotnet run --no-build --project src/ModbusRabbitGateway -- --once --json`.
Ohne laufende Geräte sind `offline`-Statusmeldungen zu erwarten.

## Stoppen und erneuter Start

Die Simulatoren und das Gateway mit `Q` bzw. Strg+C beenden, dann:

```powershell
docker compose --env-file docker/.env -f docker/docker-compose.yml stop
# Wieder starten, einschließlich Bereitschaftsprüfung:
docker compose --env-file docker/.env -f docker/docker-compose.yml up -d --wait --wait-timeout 180
# Alternativ Container und Netzwerk entfernen; das Datenvolume bleibt erhalten:
docker compose --env-file docker/.env -f docker/docker-compose.yml down
```

**Persistenz:** Die Default-Zugangsdaten werden nur bei einer leeren Broker-Datenbank angelegt. Eine spätere Änderung
an `.env` ändert bestehende RabbitMQ-Benutzer nicht automatisch. Passwort in RabbitMQ (Management-Oberfläche) und
`.env` gemeinsam ändern. Kein `down -v` für einen normalen Neustart: Das würde die Broker-Daten löschen.

## Fehlerbehebung

- Engine nicht erreichbar: Docker Desktop starten, Linux-Container auswählen, `docker info` prüfen.
- Port belegt: vorhandenen Dienst identifizieren; keine fremden Container automatisch beenden.
- Datei-Mount fehlerhaft: `rabbitmq.conf` und `enabled_plugins` müssen Dateien sein. Compose legt fehlende Pfade
  bewusst nicht als Verzeichnisse an.
- Anmeldung abgelehnt: `.env`, eventuell gesetzte Gateway-Umgebungsvariablen und bestehende Broker-Benutzer prüfen.
- Broker noch nicht bereit: `ps` und `logs` prüfen; Image-Download benötigt beim ersten Start eine Verbindung.

Grundlagen: [RabbitMQ MQTT-Konfiguration](https://www.rabbitmq.com/docs/mqtt),
[Compose-Service-Konfiguration](https://docs.docker.com/reference/compose-file/services/).

---

**Quelle:** `src/Devices.Common/README.md`, Abschnitt „Devices.Common“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise; Vom Nutzer bestätigte fachliche Aufteilung
**Aktueller Ort:** [README](../../src/Devices.Common/README.md)

### Unveränderter Originalabschnitt

# Devices.Common

**LB03-Ergänzung 2026-11-08:** `DeviceTimeSource.MasterSession` stellt die aktuelle Master-Sitzung lesend bereit.
Damit kann der Wetter-Publisher einen vollständig neu gestarteten Master auch bei gleicher RunId erkennen.

**Zweck:** Gemeinsame Bausteine aller Feldgeräte (Charger, PV, Batterie nutzen sie seit 2026-10-25). Enthalten: Anbindung an die gemeinsame virtuelle Zeit,
feste Modellschritte, Konsoleneingabe ohne blockierendes Warten, Modbus-Server-Hosting (Registerkopf, Schreibprüfung) und seit 2026-10-26 der Wetterbezug
(HTTP-Client, Stunden-Cache, Fehlerbehandlung). Geplant: Thermik/Derating, Energiezähler, CSV, Konsolen-UI.
**Verantwortlich:** Susanne Lastowicka Zeit, Alexander Lux Wetter.
**Basis aus LB01:** Susi Pausen-/Tempo-Tasten und Prompt (jetzt async), Domi Zeitkonto mit festen Schritten (jetzt `StepScheduler`).
**Abhängigkeiten:** `SimCore.Contracts`, `SimCore.Time`, `SimCore.TimeSync`, `Modbus.Contracts`, `Shared.Config`, EasyModbusTCP.NETCore.

## Bausteine

| Klasse | Aufgabe |
|---|---|
| `TimeSyncOptions` | Gemeinsame Startoptionen `--master host:port`, `--standalone`, `--start`, `--speed` (`TryApply` für den eigenen `ArgsParser`, Hilfetext `Usage`) |
| `DeviceTimeSource` | Zeitquelle des Geräts: mit Master `SyncedClock` + `TimeSyncClient`, standalone eine lokale `MasterClock`. Liefert `Clock`, `HasTime`, `IsRunning`, `StatusText`, `Log`; `TogglePause`/`ChangeSpeed`/`SetSpeed` nur standalone |
| `StepScheduler` | Rechnet aus der Uhrzeit die fälligen festen Schritte (`Update(now, runId)`), erkennt Neustarts, holt verpasste Schritte nach (höchstens 10 000 auf einmal, Rest wird gemeldet übersprungen) |
| `Modbus.ModbusOptions` | Gemeinsame Startoptionen `--modbus-port`, `--modbus-bind`, `--unit-id`, `--no-modbus`, `--register-plan` (`TryApply`, `Usage`) |
| `Modbus.ModbusDeviceServer` | Modbus-TCP-Server eines Geräts auf Basis eines `RegisterPlan`: `Start()` (prüft vorher, ob der Port frei ist, sonst `IOException`), `Publish(header, time, specific)` schreibt Kopf und gerätespezifische Register, `TryDequeueCommand` liefert angenommene Schreibzugriffe, `StatusText`, `Log`, `Dispose` |
| `Modbus.ModbusDeviceServer.ValueValidator` | Optionale gerätespezifische Prüfung für Werte, deren Grenzen nicht im Registerplan stehen können (z. B. Sollwert innerhalb der konfigurierten Leistung der Batterie) |
| `Modbus.HeaderValues`, `RegisterWriter`, `ModbusCommand` | Werte des Registerkopfs, Schreiber für gerätespezifische Register (prüft Adresse und Typ), angenommener Schreibzugriff |
| `AsyncConsole` | `TryReadKey()` ohne Warten, `ReadLineAsync(max, ct)` mit Echo, Rücktaste und Esc; bei umgeleiteter Eingabe `Console.In.ReadLineAsync` |
| `Weather.WeatherOptions` | Startoptionen `--weather <url\|off>`, `--location <ort>` (`TryApply`, `Usage`); `WithDefaultUrl(standalone)` setzt die Standard-URL nur mit Master |
| `Weather.WeatherClient` | HTTP-Client auf den Wettersimulator, implementiert `IWeatherService`; 5-s-Timeout, deutsche Fehler in `WeatherUnavailableException` |
| `Weather.WeatherFeed` | Wetter je Modellstunde: fragt höchstens einmal pro Stunde, Retry nach 10 s, behält das letzte Wetter, meldet nach 3 Fehlversuchen einmalig sichtbar |

## Verwendung in einem Gerät (Muster: `src/Devices/DualPortCharger/Simulation/SimulationEngine.cs`)

```csharp
var time = DeviceTimeSource.Create(config.TimeSync, deviceId: "BAT-01", deviceKind: "Battery");
time.Log += (_, m) => message = m;
var scheduler = new StepScheduler(TimeSpan.FromSeconds(60));
var syncTask = time.RunAsync(ct);                     // Verbindung zum Master, verbindet sich selbst neu

while (!ct.IsCancellationRequested)
{
    // Tasten: AsyncConsole.TryReadKey(); P und </> an time.TogglePause()/time.ChangeSpeed() weitergeben
    if (time.HasTime)
    {
        var u = scheduler.Update(time.Clock.Now, time.Clock.RunId);
        if (u.Kind == StepUpdateKind.Restarted) { /* Neustart melden */ }
        for (int i = 0; i < u.StepCount; i++)
            device.Step(u.FirstStepAt + scheduler.Step * i, scheduler.Step);
        // u.ModelTime = Zeit des dargestellten Zustands (für Anzeige, CSV, später Modbus-Kopf)
    }
    // Anzeige mit time.StatusText
    await Task.Delay(50, ct);
}
await syncTask;                                        // meldet sich beim Master ab
```

Regeln:
- Das Gerät zählt die Zeit **nie selbst** hoch und wartet nie `Schritt / Tempo` ab. Es fragt die Uhr und rechnet die fälligen Schritte.
  Dadurch bleibt es bei jedem Tempo synchron, auch wenn die Schleife einmal langsamer ist.
- Pause und Tempo bestimmt der Master. Die Geräte-Tasten dafür wirken nur standalone.
- Bei `MasterLost` steht die Uhr, `StepCount` ist dann 0. Das Gerät rechnet nicht weiter, die Anzeige (und später Modbus) bleibt erreichbar.
- Standalone: Die erste Modellzeit ist genau `--start` (siehe Charger).

## Modbus im Gerät (Muster: `src/Devices/DualPortCharger/Simulation/ChargerModbus.cs`; weitere: `PvModbus`, `BatteryModbus`)

1. `RegisterPlan` des Geräts anlegen (Kopf aus `CommonRegisters`, eigene Register ab Adresse 20; `RegisterPlan.ToMarkdown()` erzeugt die Tabelle für die README).
2. `new ModbusDeviceServer(options, plan, deviceId)`, `Start()` **vor** Konsole und CSV (Port belegt ⇒ `IOException` ⇒ Fehlermeldung, Exit-Code 1).
3. In **jedem** Durchlauf der Gerätschleife, auch in der Pause und bei `MasterLost`:
   `TryDequeueCommand` bis leer (Befehle im Simulationsthread auf das Modell anwenden), dann `Publish(...)` mit dem aktuellen Zustand
   (`ModelTime` = Zeit des zuletzt berechneten Schritts). Die Sequenznummer wächst nur mit neuer Modellzeit.
4. Sollwerte, die zurückgelesen werden sollen, im `specific`-Callback mit veröffentlichen.
5. Beim Beenden `Dispose()`.

**Timing:** Die Bibliothek antwortet auf einen Schreibzugriff, bevor die Anwendung ihn sieht; angenommene Befehle kommen deshalb erst im nächsten Schleifendurchlauf
(Tests warten darauf, der Diagnose-Client wartet 200 ms).

Die Prüfung der Schreibzugriffe (Adresse, Bereich, Typ, unvollständige Floats) übernimmt der Server anhand des Registerplans; das Gerät
bekommt nur gültige Werte. Ungültiges wird zurückgesetzt und in den Registern 18/19 vermerkt, weil die Bibliothek erst nach dem Schreiben
meldet (Details im Charger-README). Der Server läuft in Threads der Bibliothek, die Modelllogik bleibt einthreadig.

## Wetter im Gerät (Muster: `src/Devices/BatterySimulator/Simulator.cs`; ebenso Charger, PV)

```csharp
var feed = new WeatherFeed(new WeatherClient(url), location, message => Message = message);
// je Modellschritt vor der Physik:
if (await feed.UpdateAsync(stepStart, runId, ct) is { } weather)
    device.AmbientTemperatureC = weather.TemperatureC;
```

- Die Abfrage erfolgt je Modellstunde, auch wenn die Schrittweite kleiner ist (`WeatherFeed` cached).
- `--weather off` (oder standalone ohne Angabe) ⇒ kein HTTP-Verkehr, das Gerät rechnet mit festen 20 °C.
- Ausfall: letztes Wetter bleibt aktiv; nach 3 Fehlversuchen erscheint „WETTER: NICHT ERREICHBAR“ in der Meldungszeile, neue Versuche alle 10 s.
- Der Client gehört nach `Devices.Common`, nicht in `SimCore.Contracts` (Contracts bleibt abhängigkeitsfrei).

## Konfiguration
Über die Kommandozeile des jeweiligen Geräts (`TimeSyncOptions.Usage`):

| Option | Standard | Bedeutung |
|---|---|---|
| `--master <host:port>` | `127.0.0.1:5100` | Simulations-Master |
| `--standalone` | aus | ohne Master, lokale Uhr |
| `--start <zeit>` | heute 00:00 | nur standalone |
| `--speed <x>` | je Gerät (Charger 120) | nur standalone, 1–100 000 |
| `--modbus-port <port>` | 1502 | Modbus-TCP-Port, 1024–65535, pro Gerät eigener |
| `--modbus-bind <ip>` | 127.0.0.1 | Adresse des Servers (`0.0.0.0` = alle) |
| `--unit-id <n>` | 1 | Modbus-Unit-ID, 1–247 |
| `--no-modbus` | aus | Server nicht starten |
| `--register-plan` | | Registerplan ausgeben und beenden (im Gerät auszuwerten) |
| `--weather <url\|off>` | mit Master `http://127.0.0.1:8080`, standalone aus | Wettersimulator; `off` = feste 20 °C |
| `--location <ort>` | salzburg | Standort für die Wetterabfrage |

## Tests
`tests/Devices.Common.Tests` (51 Tests, alle grün am 2026-10-26; neu: `WeatherFeed` und `WeatherOptions`).

## Bekannte Probleme
- Modbus: Bibliotheksgrenze, Schreibfehler werden nicht als Modbus-Ausnahme gemeldet; die Bibliothek bindet den Port in einem eigenen Thread, deshalb prüft `Start()` vorher selbst (kleines Zeitfenster zwischen Prüfung und Bindung).

---

**Quelle:** `src/Shared/README.md`, Abschnitt „Inhalt“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../src/Shared/README.md)

### Unveränderter Originalabschnitt

## Inhalt

| Namespace | Datei | Zweck |
|---|---|---|
| `Shared.Data` | `Data/CsvFileWriter.cs` | Asynchroner CSV-Writer: Datei mit Zeitstempel anlegen, Kopfzeile schreiben, Zeilen anhängen, Felder maskieren |
| `Shared.Data` | `Data/CsvFormat.cs` | Einheitliches CSV-Zahlenformat: de-AT (Dezimalkomma, Excel liest Punkt-Werte sonst als Datum), InvariantCulture für ISO-Datum/Uhrzeit |
| `Shared.Data` | `Data/CsvSchema.cs` | Gemeinsame Grundspalten der Geräte-CSVs: `Gerät;Datum;Zeit;Lauf`, darunter die gerätespezifischen Spalten |
| `Shared.Output` | `Output/BarChart.cs` | Balken in gefüllten und leeren Teil aufteilen (`Split`) |
| `Shared.Output` | `Output/ConsoleText.cs` | Zeilen auf feste Breite schneiden/füllen (`Fit`) – mehrfach in Geräte-Renderern dupliziert |
| `Shared.Weather` | `Weather/Season.cs` | Jahreszeit aus dem Monat, deutsche Bezeichnung, Tageslänge, Einstrahlungsfaktor, Basistemperatur, Schneewahrscheinlichkeit |
| `Shared.Weather` | `Weather/WeatherCondition.cs` | Lokale Wetterzustände und deutsche Bezeichnung |
| `Shared.Weather` | `Weather/IrradiationCalculator.cs` | Idealisierte Einstrahlung aus Uhrzeit, Wetter und Jahreszeit |

Das lokale Wettermodell ist nur für Standalone-Läufe gedacht. Mit Simulations-Master beziehen die
Geräte das Wetter über `Devices.Common.Weather` vom Wettersimulator.

Die Helfer `CsvFormat`, `CsvSchema` und `ConsoleText.Fit` stehen den Feldgeräten bereit;
die PV nutzt sie bereits. Batterie, Charger und Kühlhaus können bei Bedarf nachziehen –
geändert wurden an ihren Geräten (noch) keine Dateien.


---

**Quelle:** `src/Modbus.Contracts/README.md`, Abschnitt „Bausteine“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../src/Modbus.Contracts/README.md)

### Unveränderter Originalabschnitt

## Bausteine

| Klasse | Aufgabe |
|---|---|
| `CommonRegisters` | Adressen und Definitionen des Kopfs 0–19, Bereichsgrenzen (`SpecificStart` 20, `SpecificEnd` 50) |
| `RegisterDefinition` | Ein Eintrag: Adresse, Name, Typ (`UInt16`, `UInt32`, `Float32`), Zugriff, Einheit, Skalierung, Schreibbereich, `Labels` (Klartext zu Zahlenwerten); `Validate(wert)` |
| `ChargerRegisters`, `PvRegisters`, `BatteryRegisters`, `KuehlhausRegisters` | Adressen und `RegisterPlan` der Geräte (Säule 20–37, PV 20–38, Batterie 20–43, Kühlhaus 20–31); gemeinsam von Gerät und Diagnose-Client benutzt |
| `RegisterPlans` | `For(DeviceType)` liefert den Plan zum Gerätetyp (Charger, PV, Batterie, Industrieverbraucher); neue Geräte (Wind) dort eintragen |
| `RegisterPlan` | Kopf + gerätespezifische Register; prüft Überschneidungen, `Find(adresse)`, `ToMarkdown()` für die Geräte-README |
| `RegisterCodec` | Wert ↔ Register: Float32/UInt32 Big-Endian, höherwertiges Wort zuerst; Modellzeit als Sekunden seit 1970 |
| `DeviceType`, `OperatingState`, `TimeStatus`, `WriteError` | Enums der Kopfregister |


---

**Quelle:** `src/Modbus.Contracts/README.md`, Abschnitt „Schnittstellen“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../src/Modbus.Contracts/README.md)

### Unveränderter Originalabschnitt

## Schnittstellen
Benutzt von `Devices.Common` (Server) und vom `ModbusDiagnosticClient` (Dekodieren, Plan über `RegisterPlans.For`). Neues Gerät: `new RegisterPlan(DeviceType.X, "Name", spezifischeRegister)`.


---

**Quelle:** `src/Modbus.Contracts/README.md`, Abschnitt „Bekannte Probleme“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../src/Modbus.Contracts/README.md)

### Unveränderter Originalabschnitt

## Bekannte Probleme
- Der Kopf muss mit den Geräten von Alexander und Dominik abgestimmt werden, bevor deren Registerpläne entstehen.
- Das Vorzeichen der Wirkleistung (positiv = Aufnahme) ist für Erzeuger (PV, Wind) gewöhnungsbedürftig: dort negative Werte.
- Skalierung (`Scale`) ist vorgesehen, aktuell nutzen alle Register Rohwert = technischer Wert.

---

**Quelle:** `src/SimCore.Contracts/README.md`, Abschnitt „SimCore.Contracts“
**Grund:** Vom Nutzer bestätigte fachliche Aufteilung; Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../src/SimCore.Contracts/README.md)

### Unveränderter Originalabschnitt

# SimCore.Contracts

**Zweck:** Gemeinsame Verträge (Interfaces und DTOs) aller Programme. Keine Abhängigkeiten, keine Logik außer reinen Formeln.
**Verantwortlich:** Alexander Lux / Susanne Lastowicka
**Basis aus LB01:** –

## Inhalt

| Typ | Art | Bedeutung |
|---|---|---|
| `TimeState` | record | Vertrag der virtuellen Zeit (siehe unten) |
| `SimMode` | enum | `Paused`, `Running` |
| `SyncMessage` (`HelloMessage`, `StatesMessage`, `PingMessage`, `PongMessage`, `ByeMessage`) | records | Nachrichten der Zeitsynchronisation, Ablauf siehe `src/SimCore.Time/README.md` und `src/SimCore.TimeSync/README.md` |
| `SyncProtocol` | static | Protokollversion (1), Standard-Port 5100, JSON-(De-)Serialisierung mit Prüfung (`InvalidDataException`) |
| `ISimClock` | interface | Lesender Zugriff auf die virtuelle Zeit (`Now`, `Mode`, `Speed`, `RunId`). Ersetzt das ursprünglich geplante `ITimeProvider` (Verwechslungsgefahr mit `System.TimeProvider`). |
| `WeatherData`, `WeatherCondition` | record, enum | Wetter für (Standort, virtuelle Zeit, Lauf): Sonnenhöhe, Bedeckung, Temperatur, Niederschlag, Wind, Strahlung, Zustand. Alle Zeiten UTC. |
| `IWeatherService` | interface | `GetAsync(location, simTimeUtc, runId, ct)`; implementiert vom `WeatherClient` in `Devices.Common`, damit Geräte nur vom Vertrag abhängen. |

Noch offen: `DeviceConfig`.

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

---

**Quelle:** `src/SimCore.TimeSync/README.md`, Abschnitt „SimCore.TimeSync“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../src/SimCore.TimeSync/README.md)

### Unveränderter Originalabschnitt

# SimCore.TimeSync

**Zweck:** TCP-Transport der gemeinsamen virtuellen Zeit: `TimeSyncServer` im Simulations-Master, `TimeSyncClient` in jedem
Simulator (auch im Wettersimulator). Die Zeitlogik selbst liegt in `SimCore.Time`, das Nachrichtenformat in `SimCore.Contracts`.
**Verantwortlich:** Susanne Lastowicka
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

---

**Quelle:** `tests/SimCore.Tests/README.md`, Abschnitt „SimCore.Tests“
**Grund:** Vom Nutzer bestätigte Fachzuordnung; tatsächlicher Umfang laut Testdateien
**Aktueller Ort:** [README](../../tests/SimCore.Tests/README.md)

### Unveränderter Originalabschnitt

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

---

**Quelle:** `tests/Devices.Common.Tests/README.md`, Abschnitt „Devices.Common.Tests“
**Grund:** Vom Nutzer bestätigte Modulzuständigkeiten; Historische 44/44-Angabe durch späteren datierten Nachweis ersetzt; spätere Änderung kenntlich gemacht
**Aktueller Ort:** [README](../../tests/Devices.Common.Tests/README.md)

### Unveränderter Originalabschnitt

# Devices.Common.Tests

**Zweck:** Tests der gemeinsamen Gerätebausteine in `src/Devices.Common`.
**Verantwortlich:** _offen_ (wie `Devices.Common`). Am 2026-10-25 von Susanne Lastowicka erstellt.
**Basis aus LB01:** –

## Ausführen

```
dotnet test tests/Devices.Common.Tests
```

## Tests

| Datei | Prüft |
|---|---|
| `StepSchedulerTests` | Erste Zeit ohne Schritt · nur volle Schritte fällig, Modell hinkt höchstens einen Schritt nach · gleiche Schritte unabhängig davon, wie oft abgefragt wird · neuer Lauf springt auf die neue (auch frühere) Zeit · Uhr kurz hinter dem Modell ⇒ warten statt zurückspringen · zu viele fällige Schritte werden begrenzt und gemeldet · ungültige Schrittweite |
| `TimeSyncOptionsTests` | Standard 127.0.0.1:5100 · `--master` mit und ohne Port · ungültige Adressen · standalone mit `--start` und `--speed` (Punkt/Komma) · ungültige Werte · fremde Optionen werden nicht verbraucht |
| `DeviceTimeSourceTests` | Standalone läuft sofort mit lokaler Uhr · Pause/Tempo standalone · mit Master werden lokale Pause/Tempo abgelehnt · mit echtem Master über TCP: Gerät ist synchron, Abweichung innerhalb 10 ms · Tempo |
| `ModbusDeviceServerTests` | Echter Server + Client über TCP: Kopf lesbar (Typ, Instanz, Zustand, Leistung, Energie, Modellzeit, Lauf, Zeitstatus) · Sequenznummer wächst nur mit neuer Modellzeit/Lauf · gültiger Sollwert wird angenommen und als Befehl gereicht · ungültige Werte (zu klein/groß, negativ, NaN, ∞) abgelehnt, alter Wert bleibt, Fehlerregister gesetzt · Schreiben auf nur lesbare/unbelegte Adresse abgelehnt · halber Float abgelehnt · Zugriff über lesbar+schreibbar komplett abgelehnt · Ein/Aus und Reset als Befehle, ungültige Werte dafür abgelehnt · Register in der Pause lesbar mit letztem Stand · belegter Port ⇒ `IOException` |
| `RegisterCodecTests` | Float32/UInt32 Big-Endian, höheres Wort zuerst · Modellzeit hin und zurück · Registerplan lehnt Überschneidungen und falsche Bereiche ab · Bereichsprüfung (Typ, Grenzen, NaN) |

Ergebnis am 2026-10-25: 44 von 44 bestanden.

## Bekannte Probleme
- `DeviceTimeSourceTests.WithMaster_FollowsMasterClock` und die Modbus-Tests brauchen freie Ports auf localhost (die Modbus-Tests suchen sich einen freien Port).

---

**Quelle:** `tests/FieldDevices.Tests/README.md`, Abschnitt „FieldDevices.Tests“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../tests/FieldDevices.Tests/README.md)

### Unveränderter Originalabschnitt

# FieldDevices.Tests

**Zweck:** Tests der Feldgeräte PV-Anlage (`src/Devices/PvSimulator`) und Batteriespeicher (`src/Devices/BatterySimulator`): Modbus-Anbindung, Optionen, Standalone-Lauf.
**Verantwortlich:** Susanne Lastowicka (Umbau auf Master-Zeit und Modbus am 2026-10-25)
**Basis aus LB01:** –

## Ausführen

```
dotnet test tests/FieldDevices.Tests
```

## Tests

| Klasse | Prüft |
|---|---|
| `PvModbusTests` | Kopf mit negativer Leistung (Einspeisung) und PV-Registern · Leistungsbegrenzung gültig (wirkt im nächsten Schritt) und ungültig (abgelehnt, alter Wert) · Störung im Kopf, Reset und Ein/Aus per Modbus · Register in der Pause lesbar |
| `BatteryModbusTests` | Kopf mit positiver Ladeleistung und Batterie-Registern · Sollwert innerhalb der konfigurierten Leistung angenommen (Modus Manuell) · außerhalb/NaN abgelehnt · Modus Auto/Manuell, ungültiger Modus · Not-Aus im Kopf, Reset, Ein/Aus |
| `RegisterPlanRequirementTests` | jeder Gerätplan hat einen schreibbaren Sollwert mit Bereich und mindestens drei gerätespezifische Werte · Markdown nennt Byte-/Wortreihenfolge |
| `ArgsParserTests` | PV: Standard, Kürzel `-s`/`-d`, standalone-Pflicht, ungültige Werte · Batterie: Gerätedaten, Zeit, Modbus, Seed, ungültige Konfiguration, standalone-Pflicht |
| `PvEngineTests` | kompletter Standalone-Lauf mit Zeitraffer: CSV beginnt genau bei `--start`, Energie wächst, Abbruch beendet geordnet |
| `BatteryUnitTests` | `Update(TimeSpan)` = `Update(double)`, Tagesprofil mit Seed wiederholbar |

Ergebnis am 2026-10-25: 34 von 34 bestanden (dreimal wiederholt, ohne Ausfall).

## Bekannte Probleme
- Die Modbus-Tests brauchen freie Ports auf localhost und warten auf Schreibzugriffe, die die Bibliothek erst nach der Antwort meldet (`WaitUntil`).
- Es gibt keinen Lauf des Batterie-Simulators bis zum Ende, weil er die CSV immer nach `docs/data` schreibt.

---

**Quelle:** `skills/probelauf/README.md`, Abschnitt „Nutzen und Wirkung“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../skills/probelauf/README.md)

### Unveränderter Originalabschnitt

## Nutzen und Wirkung

Die Abgabe verlangt keinen Code, sondern einen **vorführbaren** Lauf: Zeitgleichheit, Pause, Tempowechsel, Neustart,
Wetterausfall, Modbus-Schreiben, Geräte- und Broker-Ausfall, geordnetes Beenden. Solche Läufe gehen selten beim ersten
Mal glatt, und die teuersten Fehler sind stille: ein alter Master, der noch Port 5100 hält, ein `start-all.ps1`, das
einen belegten Port überspringt, oder „angenommen“ im Client, obwohl das Gerät den Wert erst im nächsten Durchlauf
sieht. Der Skill macht die Prüfreihenfolge, die typischen Fallen und das Belegformat explizit, damit ein Lauf
wiederholbar ist und ein Nachweis entsteht, der auch einer Prüfung standhält – inklusive der Punkte, die **nicht**
geprüft wurden.

Drei Umfänge, damit der Aufwand zum Ziel passt:

| Umfang | Beteiligte Programme | Belegt |
|---|---|---|
| **A – Gerät** | Wetter (optional), ein Gerät, Diagnose-Client | Registerplan, Sollwert, Wetter, CSV |
| **B – LB02-Demo** | Wetter, Master, vier Geräte, Diagnose-Client | Zeit, Pause, Tempo, Neustart, Wetterausfall |
| **C – LB03-Kette** | wie B + RabbitMQ + Gateway + Viewer | Nachweise 1–7 aus Aufgabe 03 |


---

**Quelle:** `skills/probelauf/README.md`, Abschnitt „Beispielaufruf mit Ergebnis (2026-11-08)“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../skills/probelauf/README.md)

### Unveränderter Originalabschnitt

## Beispielaufruf mit Ergebnis (2026-11-08)

Aufruf: `@probelauf nur Kühlhaus, ohne Master` – Umfang **A**, Kühlhaus KH-01 standalone, Tempo 600, 8 Modellstunden,
`--weather off`, Seed 42, Modbus 1505. Vollständiger Nachweis mit allen Protokollen:
[`docs/nachweise/2026-11-08-probelauf-skill/`](../../../docs/nachweise/2026-11-08-probelauf-skill/README.md). Auszug:

| # | Prüfung | Beobachtung | Ergebnis |
|---|---|---|---|
| A1 | Zeitbezug | Register 12 = 2 (Standalone), 13 = 1 (läuft), Tempo 600 | ✅ |
| A2 | Registerkopf 0–19 | Typ 5 = Industrieverbraucher, Instanz 1, Modellzeit 2026-11-08 00:10 | ✅ |
| A3 | Geräteregister 20–31 | Soll −20 °C, Raum −19,77 °C, außen 7,5 °C, Tür zu, Kompressor aus | ✅ |
| A4 | Sollwert schreiben | „ANGENOMMEN: Solltemperatur −20 °C → −15 °C“; Gegenlesung: Register 20–21 = −15 °C, Register 19 = 20 | ✅ |
| A5/A6 | ungültig / nur lesbar (`force`) | nicht ausgeführt | ⚠️ nicht geprüft |
| A7 | CSV | `KH-01_2026-11-08_155520.csv`, Semikolon, Dezimalkomma, Spalte `Lauf` | ✅ |
| A8 | Wetter | mit `--weather off` nicht Teil des Laufs | ⚠️ nicht geprüft |

Zeitmarken aus `ablauf.txt`: Gerät 15:55:19 gestartet, Port 1505 nach 1,2 s erreichbar, Lesen 15:55:20,
Schreiben 15:55:21, Gegenlesen 15:55:22, Gerät nach 8 Modellstunden um 15:56:08 beendet, alle Ports danach frei.
Der Lauf prüft den **Ablauf**; Master, Broker und echte Tastenbedienung sind ausdrücklich als „nicht geprüft“ vermerkt.


---

**Quelle:** `skills/probelauf/README.md`, Abschnitt „Ablage und Aktivierung“
**Grund:** Versionierte Ablage nach Nutzerauftrag nach skills/ kopiert; lokale Aktivierung und verbliebene Pfadabhängigkeit offengelegt
**Aktueller Ort:** [README](../../skills/probelauf/README.md)

### Unveränderter Originalabschnitt

## Ablage und Aktivierung

Der Skill liegt in **`.opencode/skills/probelauf/`**. Seine ID ist `probelauf` (aus dem Ordnernamen; `name:` im
Frontmatter ist nur das Anzeige-Label). OpenCode findet Projekt-Skills dort automatisch – kein Eintrag in
`opencode.json`, kein Kopieren, kein Neustart: geänderte Skill-Dateien werden neu eingelesen.

Aufruf im Prompt:

- `@probelauf <Thema>` – lädt diesen Skill und übergibt das Thema als Text (das Skill-Werkzeug selbst hat keine
  Parameter), z. B. `@probelauf LB03 Gateway`.
- Ohne Mention: den Auftrag einfach beschreiben; das Modell sieht die `description` und lädt den Skill selbst.

Einen Slash-Command `/probelauf` gibt es bewusst nicht (Commands liegen in `.opencode/commands/` und sind ein
eigenes Konzept). Die Ablage trennt die Agenten: Claude Code liest `.claude/skills/`, OpenCode liest
`.opencode/skills/` – und zusätzlich `.claude/skills/` und `.agents/skills/` als Kompatibilitätsquellen, wobei
`.opencode/skills/` die höhere Priorität hat.


---

**Quelle:** `skills/readTask/README.md`, Abschnitt „Beispielaufruf mit Ergebnis (2026-11-08)“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../skills/readTask/README.md)

### Unveränderter Originalabschnitt

## Beispielaufruf mit Ergebnis (2026-11-08)
Aufruf: `/readTask Aufgabe 03`. Vollständiges Ergebnis: [`docs/task-status/aufgabe-03.md`](../../docs/task-status/aufgabe-03.md)
(28 Anforderungen: 3 erfüllt, 6 teilweise, 16 nicht erfüllt, 3 nicht nachgewiesen). Auszug:

| ID | Anforderung (Seite) | Art | Status | Beleg / Lücke |
|---|---|---|---|---|
| A7 | Compose im Repo, dokumentiert (S. 2) | Pflicht | teilweise | `docker/docker-compose.yml` vorhanden; Image ohne genaue Version, `rabbitmq.conf` ist ein leerer Ordner, keine Start/Stopp-Doku |
| A9 | Keine Zugangsdaten versioniert, `.env.example` (S. 2, 5) | Pflicht | nicht erfüllt | Passwort fest in der Compose-Datei, `.env.example` fehlt |
| A10 | Modbus–RabbitMQ-Gateway (S. 2–3) | Pflicht | nicht erfüllt | kein Gateway-Projekt in `EnergyTwin.sln` |
| A5 | SRS `docs/SRS-LB03.md` (S. 2) | Pflicht | nicht erfüllt | Datei fehlt |
| A16 | Eigener Skill pro Person (S. 4–5) | Pflicht | teilweise | `skills/readTask` vorhanden; Alexander und Dominik offen |

Vorgeschlagene Reihenfolge (Auszug): 1. SRS mit Nachrichtenformat und Akzeptanzkriterien (blockiert alles) →
2. Compose und Broker-Doku abschließen → 3. Gateway → 4. Viewer → 5. Nachweise (Screenshots, Protokolle) →
6. README, `AGENTS.md`, Journale, Abgabe-Tag `lb03-2026`.


---

**Quelle:** `skills/readTask/README.md`, Abschnitt „Aktivierung“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../skills/readTask/README.md)

### Unveränderter Originalabschnitt

## Aktivierung
Claude Code lädt Skills aus `.claude/skills/`. Dieser Ordner `skills/` ist die versionierte Ablage im Repo. Zum Verwenden:
`.claude/skills/readTask` auf `skills/readTask` kopieren (oder verlinken):
```
New-Item -ItemType Directory -Force .claude/skills
Copy-Item -Recurse skills/readTask .claude/skills/readTask
```


---

**Quelle:** `skills/readTask/README.md`, Abschnitt „Quelle und Lizenz“
**Grund:** Belegte Aktualisierung anhand Code, Konfiguration und datierter Nachweise
**Aktueller Ort:** [README](../../skills/readTask/README.md)

### Unveränderter Originalabschnitt

## Quelle und Lizenz
Eigenentwicklung für dieses Projekt, nicht aus einer fremden Vorlage übernommen. Inspiration: die Beschreibung des
Anwendungsfalls in Aufgabe 03 (Roland Graf, 2026). Keine fremde Lizenz zu beachten.


---

**Quelle:** `docs/nachweise/2026-11-08-probelauf-skill/README.md`
**Grund:** Nur Navigationslinks auf die versionierte Skill-Ablage umgestellt; Durchführung und Ergebnisse unverändert
**Ausgangsstand:** wie im Archivkopf, vor dieser Änderung gesichert.
**Aktueller Ort:** [Dokumentation](../../docs/nachweise/2026-11-08-probelauf-skill/README.md)

### Unveränderter Originalabschnitt

# Funktionsnachweis 2026-11-08: Selbsttest des Skills `probelauf` (Kühlhaus, Umfang A)

**Durchgeführt von:** Dominik Zach mit opencode (Modell Big Pickle) – Ablauf skriptgesteuert, Protokolle unverändert.
**Bezug:** Prüfung des Skills [`probelauf`](../../../.opencode/skills/probelauf/README.md), Umfang **A – Gerät**; gleichzeitig
Nachweis der Punkte A1–A4 und A7 der Prüfliste [`probelauf/references/abnahmekriterien.md`](../../../.opencode/skills/probelauf/references/abnahmekriterien.md).
**Kein** LB02- oder LB03-Nachweis – was dieser Lauf nicht zeigen kann, steht unter „Nicht geprüft“.

## Aufbau

| Programm | Start | Protokoll |
|---|---|---|
| Kühlhaus KH-01 (standalone) | `dotnet run --project src/Devices/KuehlhausSimulator -- --id KH-01 --standalone --speed 600 --duration 8 --weather off --seed 42 --modbus-port 1505` | `kuehlhaus.txt` |
| Diagnose-Client, lesen | `dotnet run --project src/ModbusDiagnosticClient -- --port 1505 --once` | `diagnose-01-read.txt` |
| Diagnose-Client, schreiben | `dotnet run --project src/ModbusDiagnosticClient -- --port 1505 --write "Solltemperatur=-15"` | `diagnose-02-write.txt` |
| Diagnose-Client, erneut lesen | wie oben mit `--once` | `diagnose-03-read.txt` |
| Ablauf mit Zeitmarken | – | `ablauf.txt` |

**Besonderheiten des Laufs:** ohne Master (Standalone), Wetter mit `--weather off`, 8 Modellstunden bei Tempo 600
(48 s Echtzeit), Ausgabe umgeleitet in Dateien (kein Terminal). Der Lauf prüft den **Ablauf des Skills**; die
Gerätefunktion selbst ist im Geräte-README und in `tests/FieldDevices.Tests` abgedeckt.

## Ergebnis je Prüfpunkt

| # | Prüfung | Beobachtung | Ergebnis | Beleg |
|---|---|---|---|---|
| A1 | Gerät startet, Zeitbezug | Register 12 = 2 (Standalone), Register 13 = 1 (läuft), Tempo 600 | ✅ | `diagnose-01-read.txt:16-18` |
| A2 | Registerkopf vollständig | Adressen 0–19 gelesen, Typ 5 = Industrieverbraucher, Instanz 1 | ✅ | `diagnose-01-read.txt:5-22` |
| A3 | Geräteregister | 20–31 vollständig: Soll −20 °C, Raum −19,77 °C, außen 7,5 °C, Tür zu, Kompressor aus | ✅ | `diagnose-01-read.txt:23-31` |
| A4 | Gültiger Sollwert | „ANGENOMMEN: Solltemperatur −20 °C → −15 °C. Gerät zeigt jetzt −15 °C.“; zweite Lesung: Register 20–21 = −15 °C, Register 19 = 20; Gerät meldet „Modbus-Schreibzugriff angenommen“ | ✅ | `diagnose-02-write.txt:4`, `diagnose-03-read.txt:22-25`, `kuehlhaus.txt:3-4` |
| A5 | Ungültiger Wert (`force`) | nicht ausgeführt | ⚠️ nicht geprüft | – |
| A6 | Nur-Lese-Register (`force 5` o. ä.) | nicht ausgeführt | ⚠️ nicht geprüft | – |
| A7 | CSV | `docs/data/KH-01_2026-11-08_155520.csv` geschrieben: Semikolon, Dezimalkomma, Spalte `Lauf`, Kopfzeile wie im Geräte-README | ✅ | `kuehlhaus.txt:28` (Pfad), Datei selbst ist über `*.csv` ignoriert und nach dem Test gelöscht |
| A8 | Wetter | mit `--weather off` nicht Teil dieses Laufs | ⚠️ nicht geprüft | – |

## Beobachtungen und Einschränkungen

- **Zeitmarken passen zum Aufbau:** Gerät 15:55:19 gestartet, Port 1505 nach 1,2 s erreichbar, Lesen 15:55:20,
  Schreiben 15:55:21, Gegenlesen 15:55:22, Gerät nach 8 Modellstunden um 15:56:08 beendet (`ablauf.txt`).
- **Modellzeit wandert im Protokoll:** erste Lesung 00:10, Schreiben ~00:15, Gegenlesung 00:25 Modellzeit – bei
  Tempo 600 sind das je Abfrage einige Sekunden Echtzeit. Der Diagnose-Client ist deshalb kein Zeitnormal.
- **Umleitung ist kein Terminal:** Gerät und Client rendern bei umgeleiteter Ausgabe nur alle 5 s eine Zeile; die
  Statuszeile wird nach Spaltenbreite abgeschnitten (`kuehlhaus.txt:22` endet mit „Meldung: Simulatio“). Das ist
  erwartet und kein Fehler (Skill `probelauf`, Fehlerbild 6).
- **Kodierung der Protokolle:** PowerShell-Redirection mit `*>` schreibt UTF-16 und ersetzt Umlaute durch `?`;
  mit `Start-Process -RedirectStandardOutput` bleiben die UTF-8-Bytes des Programms unverändert. Alle Dateien hier
  sind UTF-8 ohne BOM. Der Fall ist als Fehlerbild 17 im Skill vermerkt.
- **`ExitCode` leer:** `Start-Process -PassThru` lieferte in dieser Umgebung nach `WaitForExit` keinen Exit-Code.
  Für den Nachweis unerheblich (Ausgaben und Portverhalten sind belegt), im Skill als Einschränkung genannt.
- **Türverhalten:** Ab 06:40 Modellzeit öffnete das Gerät die Tür (Zufallsereignis aus dem Seed), der Kompressor
  kühlte gegen, Energie 2,53 kWh. Plausibel, nicht Teil der Prüfpunkte.
- Der Portmatrix-Check vor und nach dem Lauf zeigte alle Ports frei; es blieben keine Prozesse zurück.

## Nicht geprüft

- **Umfang B (LB02-Demo):** Zeitgleichheit mit dem Master, Toleranz, Pause, Fortsetzen, Tempowechsel, Neustart,
  Wetterausfall, Wetter-Reproduzierbarkeit, geordnetes Beenden per `Q`.
- **Umfang C (LB03-Kette):** RabbitMQ, Gateway, Viewer, Sollwert über MQTT, Geräte- und Broker-Ausfall,
  falsche Zugangsdaten.
- **Echte Bedienung:** Anzeige und Tasten in einem echten Terminalfenster, Strg+C, Screenshots.
- **Ungültige Schreibwerte** (`force`, Register 18/19) und **Wetter** (`--weather` mit Wettersimulator).

Maßgeblich für die vollständigen Kriterien: [`probelauf/references/abnahmekriterien.md`](../../../.opencode/skills/probelauf/references/abnahmekriterien.md).

---

**Quelle:** `skills/probelauf/templates/nachweis-README.md`
**Grund:** Startbeispiel der Vorlage auf die vorhandene Compose-Datei, lokale .env und Bereitschaftsprüfung ausgerichtet
**Ausgangsstand:** wie im Archivkopf, vor dieser Änderung gesichert.
**Aktueller Ort:** [Dokumentation](../../skills/probelauf/templates/nachweis-README.md)

### Unveränderter Originalabschnitt

## Aufbau

| Programm | Start | Protokoll |
|---|---|---|
| RabbitMQ (Docker, `docker/docker-compose.yml`) | `docker compose up -d` | – |
| Wettersimulator | `dotnet run --project src/WeatherSimulator` | `wetter.txt` |
| Master | `dotnet run --project src/SimulationMaster -- --speed 600` | `master.txt` |
| &lt;Gerät&gt; | `dotnet run --project src/Devices/… -- --id … --modbus-port …` | `&lt;geraet&gt;.txt` |
| Gateway | `dotnet run --project src/ModbusRabbitGateway -- --device …` | `gateway.txt` |
| Viewer | &lt;MQTT Explorer oder Test-Empfänger&gt; | `viewer.txt` |
| Diagnose-Client | `dotnet run --project src/ModbusDiagnosticClient -- --port …` | `diagnose.txt` |
| Ablauf mit Zeitmarken | – | `ablauf.txt` |

**Besonderheiten des Laufs:** &lt;z. B. Skript-Master statt Tastenbedienung, nur ein Gerät, Broker von Hand gestartet&gt;.
**Zugangsdaten:** nur über Umgebungsvariablen bzw. `docker/.env`; in den Protokollen steht kein Passwort.

