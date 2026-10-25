# EnergySimulator – PV-Anlage (LB01, ab LB02 mit Master-Zeit und Modbus)

Konsolensimulator für eine PV-Anlage im Rahmen von .NET-Programmierung 2026, LB01.

**Verantwortlich:** Alexander Lux  
**Team:** Alpha

## Voraussetzungen

- [.NET 10 SDK](https://dotnet.microsoft.com/download) – Projekt zielt auf `net10.0`
- Teil der Solution `EnergyTwin.sln` im Repository-Hauptordner. Das Projekt verweist auf `src/Devices.Common`
  (bringt EasyModbusTCP.NETCore mit, wird beim ersten Build aus NuGet geladen). Der LB01-Code nutzt das noch nicht.

## Starten

Aus dem Repository-Hauptordner:

```bash
# Standardstart (LB02): Zeit vom Simulations-Master, CSV alle 15 min
dotnet run --project src/Devices/PvSimulator

# Ohne Master (wie LB01): schneller Lauf
dotnet run --project src/Devices/PvSimulator -- --standalone -s 1800

# Winter mit Schnee demonstrieren (ohne Master)
dotnet run --project src/Devices/PvSimulator -- --standalone -d "2026-01-15 06:00" -s 300

# Eigene Parameter
dotnet run --project src/Devices/PvSimulator -- -p 15.5 -t 10

# Hilfe
dotnet run --project src/Devices/PvSimulator -- --help
```

Die Simulation läuft **ohne Zeitlimit** und läuft bei Mitternacht in den nächsten
Tag weiter. Tagesenergie wird um 00:00 zurückgesetzt, die Gesamtenergie läuft durch.
Ab LB02 kommen Datum und Tempo vom Simulations-Master; alle Optionen stehen im Abschnitt „LB02“ am Ende.

### CLI-Optionen

| Option | Beschreibung | Default |
|--------|-------------|---------|
| `-s, --speed` | (standalone) Tempo; früher `--timescale` | 60 |
| `-t, --step` | CSV-Aufzeichnungsintervall in Minuten | 15 |
| `-r, --refresh` | Bildschirmaktualisierung in Millisekunden | 400 |
| `-p, --peak` | Nennleistung in kW | 10.0 |
| `-o, --output` | Verzeichnis für CSV-Export | `docs/data` im Repo |
| `-d, --start` | (standalone) Startzeit `yyyy-MM-dd HH:mm` | heute 00:00 |

## Bedienung (Tasten)

| Taste | Funktion |
|-------|---------|
| `P` | Pause / Fortsetzen (nur standalone, sonst steuert der Master) |
| `M` | Modus wechseln (AUTO / MANUELL) |
| `Q` | Beenden |
| `C` | Module reinigen (Verschmutzung zurücksetzen) |
| `R` | Bildschirm ruhiger / schneller (100 – 3000 ms) |

**Störungen**

| Taste | Funktion |
|-------|---------|
| `F` | Wechselrichterfehler auslösen / zurücksetzen |
| `T` | Überhitzung erzwingen / aufheben |
| `G` | Netzspannungsstörung ein-/ausschalten |

**Wetter** (manueller Modus): `0` Nacht, `1` Sonnig, `2` Bewölkt, `3` Dicht bewölkt, `4` Gewitter

**Zeitraffer** (nur standalone): `5` x1, `6` x15, `7` x60, `8` x300, `9` x1800

Jede Aktion wird im Display bestätigt.

## Modellbeschreibung

### Uhr und Kalender

Die Zeit kommt vom Simulations-Master (standalone von einer lokalen Uhr ab `--start`) und läuft unbegrenzt weiter.
Das Modell rechnet in festen Schritten (`--model-step`, Standard 1 min). Datum, Uhrzeit und Jahreszeit werden fortlaufend angezeigt.

### Jahreszeiten

Die Jahreszeit ergibt sich aus dem Monat und beeinflusst Einstrahlung, Tageslänge,
Umgebungstemperatur und Schneewahrscheinlichkeit:

| Jahreszeit | Monate | Einstrahlungsfaktor | Sonnenaufgang | Sonnenuntergang | Umgebungstemperatur | Schnee-Wahrscheinlichkeit |
|------------|--------|--------------------|---------------|-----------------|----------------------|--------------------------|
| FRÜHLING | 03–05 | 0.75 | 06:00 | 18:00 | 11 °C | 3 % |
| SOMMER | 06–08 | 1.00 | 04:30 | 20:00 | 21 °C | 0 % |
| HERBST | 09–11 | 0.60 | 06:30 | 17:30 | 10 °C | 8 % |
| WINTER | 12–02 | 0.40 | 08:00 | 16:00 | 2 °C | 40 % |

### Einstrahlung

Halbsinus über den Tag, skaliert mit Wetterfaktor und Jahreszeit:

```
Grundwert  = 1000 W/m² · sin(π · (t − Aufgang) / (Untergang − Aufgang))
Einstrahlung = Grundwert · Wetterfaktor · Jahreszeitfaktor
```

| Wetter | Faktor |
|--------|--------|
| Sonnig | 1.00 |
| Bewölkt | 0.60 |
| Dicht bewölkt | 0.30 |
| Gewitter | 0.10 |
| Schnee | 0.15 |
| Nacht | 0.00 |

Der Zustand **Nacht** wird im Auto-Modus automatisch gesetzt, sobald die Sonne
untergeht, und bei Sonnenaufgang automatisch wieder auf einen Tageszustand.

### Leistung

```
Leistung = Nennleistung · min(1, Einstrahlung / 1000) · (1 − Verschmutzung − Alterung)
```

Bei Nacht, ausgeschalteter Anlage oder aktiver Störung: **0 kW**.

### Wärmeverhalten der Module

```
Gleichgewichtstemperatur = Außentemperatur + 0.040 K/(W/m²) · Einstrahlung
```

Die Modultemperatur folgt dem Gleichgewicht mit einer Zeitkonstante von 0.8 h bei
Einstrahlung und 2.5 h ohne Sonne – sie läuft der Einstrahlung also sichtbar
hinterher. Im Sommer kommen tägliche Hitzewellen von bis zu +12 °C hinzu, dazu die
Tagesneigung (Tagesminimum am Morgen, Maximum am Nachmittag, Abkühlung über Nacht).
Die Hitzewelle wird aus dem Datum erzeugt, die Temperaturkurve eines Tages ist damit
über mehrere Läufe hinweg gleich; Wetter und Netzrauschen bleiben zufällig.
Ab **70 °C** löst der Wechselrichter den Überhitzungsschutz aus, die Leistung fällt
auf 0 kW. Unter **55 °C** entspannt sich der Schutz selbsttätig.

### Netzspannung

```
Spannung ≈ 230 V − 24 V · Lastfaktor + 9 V · Einspeisung + Rauschen
```

Der Lastfaktor hat Spitzen am Morgen und am Abend. Liegt die Spannung außerhalb
**200–250 V**, trennt der Wechselrichter aus dem Netz.

### Verschmutzung und Alterung

| Effekt | Rate | Grenze | Zurücksetzbar |
|--------|------|--------|---------------|
| Verschmutzung | 0.6 %/Tag | 25 % | ja, mit Taste `C` |
| Alterung | 0.04 %/Tag | 15 % | nein |

### Störungen

| Störung | Auslöser | Wirkung | Auflösung |
|---------|----------|---------|-----------|
| Wechselrichterfehler | Taste `F` | 0 kW | Taste `F` |
| Überhitzung | Modultemperatur ≥ 70 °C | 0 kW | automatisch unter 55 °C |
| Netzspannung | Spannung außerhalb 200–250 V | 0 kW | Rückkehr in den gültigen Bereich |
| Verschmutzung/Alterung | Zeitablauf | Leistungsreduktion | Reinigung bzw. nicht reversibel |

## Konsolenausgabe

Mit exakt ausgerichteten Rahmen:

- Datum, Uhrzeit, Jahreszeit, Modus, Betriebszustand, Zeitraffer
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
- Bestätigungsmeldung der letzten Aktion (verschwindet nach 4 s)

## Artefakte

- **CSV:** `docs/data/<ID>_<Datum>_<Uhrzeit>.csv` im Repository, egal aus welchem Ordner gestartet wird (Ordner per `-o` änderbar) – 18 Spalten, u. a. Datum, Jahreszeit, Wetter,
  Einstrahlung, Leistung, Tages- und Gesamtenergie, Temperaturen, Netzspannung,
  Verschmutzung, Alterung, Fehlerursache, Anlagenstatus, Lauf, Leistungsbegrenzung.
  Die Beispiel-CSV `docs/data/pv-simulation.csv` aus LB01 bleibt unverändert.
- **Screenshots:** [`docs/screenshots/PvSimulator/`](../../../docs/screenshots/PvSimulator/)
- **Entwicklungsjournal:** [`docs/development-journal/alexander-lux.md`](../../../docs/development-journal/alexander-lux.md)

## Projektstruktur

Pfade relativ zum Repository-Hauptordner `dotnet-energy-twin-alpha/`:

```
src/Devices/PvSimulator/
├── PvSimulator.csproj                # Projekt (Einstellungen aus src/Directory.Build.props)
├── Program.cs                        # Entry Point, CLI-Parsing
├── Simulation/
│   ├── SimulationEngine.cs           # Tick-Loop, Eingaben, Tageswechsel
│   ├── SimulationConfig.cs           # Konfiguration
│   ├── SimulationMode.cs             # AUTO / MANUELL / PAUSE
│   └── Season.cs                     # Jahreszeiten und Saisonparameter
├── Devices/
│   ├── IDevice.cs                    # Geräte-Interface
│   ├── PvSystem.cs                   # PV-Anlage
│   ├── PvFaultModel.cs               # Störungen, Temperatur, Spannung, Verschmutzung
│   ├── IrradiationCalculator.cs      # Einstrahlung aus Zeit, Wetter, Saison
│   ├── WeatherCondition.cs           # Wetterzustände
│   ├── FaultType.cs                  # Störungsarten
│   └── InverterStatus.cs             # Wechselrichter-Status (unbenutzt)
├── Output/
│   ├── ConsoleDisplay.cs             # Rahmen, Farben, Bestätigungen
│   └── BarChart.cs                   # Balkenberechnung
└── Data/
    └── CsvExporter.cs                # CSV-Export

docs/                                 # zentral im Repository
├── data/pv-simulation.csv            # CSV-Ausgabe
├── screenshots/PvSimulator/          # Screenshots
└── development-journal/alexander-lux.md   # Entwicklungsjournal
```

## Annahmen

- Linearer Wirkungsgrad, konstante Fläche, keine Verschattung
- Modultemperatur ohne Wind- und Luftströmungseinfluss
- Verschmutzung wirkt linear auf den Ertrag
- Einstrahlungsmaximum 1000 W/m² als Referenz (klarer Sonnenmittag)
- Tageslänge und Einstrahlung stark vereinfacht über die Jahreszeit skaliert
- Wetterzustände diskret, Übergänge als sprunghaft angenommene Änderung
- .NET 10 als Ziel, gebaut mit SDK 10.0.401

---

## LB02 – Umbau zum Modbus-Feldgerät

**Zweck:** PV-Anlage als Feldgerät im Gesamtsystem: virtuelle Zeit vom Simulations-Master, Zustand und Sollwert über Modbus TCP.
**Verantwortlich:** Alexander Lux (LB01-Gerät); Umbau auf Master-Zeit und Modbus am 2026-10-25 von Susanne Lastowicka (mit Alexander abzustimmen)
**Basis aus LB01:** Alex PvSystem, PvFaultModel

### Stand (2026-10-25)

| Schritt | Stand |
|---|---|
| Zeit vom Simulations-Master (`DeviceTimeSource`, `StepScheduler`, feste Schritte `--model-step`, Standard 1 min) | ✅ |
| Standalone mit lokaler Uhr (`--standalone`, Tasten `P`, `5–9` wirken nur dort) | ✅ |
| async Schleife, CSV angehängt statt komplett neu geschrieben (kein `Thread.Sleep`, kein `File.WriteAllText`) | ✅ |
| Modbus-Server mit Registerplan, Server bleibt in der Pause erreichbar | ✅ |
| Wetter vom Wettersimulator | offen (bis dahin das lokale Auto-Wetter) |

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

### Bedienung / Start

```
dotnet run --project src/SimulationMaster -- --speed 600        # 1. Master
dotnet run --project src/Devices/PvSimulator -- --id PV-01      # 2. PV-Anlage (Modbus 1502)
dotnet run --project src/Devices/PvSimulator -- --standalone -s 300 -d "2026-06-21 05:00"   # ohne Master
```
Tasten wie oben; `P` (Pause) und `5–9` (Tempo) wirken nur standalone, mit Master steuert der Master. Die Anzeige hat zwei neue Zeilen
„Zeitquelle“ und „Modbus“. Bei umgeleiteter Ausgabe erscheint alle 5 s eine Statuszeile.

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

### Schnittstellen
- **Benötigt:** Simulations-Master (TCP, `127.0.0.1:5100`); später den Wettersimulator.
- **Angeboten:** Modbus-TCP-Server (siehe Registerplan), CSV-Datei `<ID>_<Datum>_<Uhrzeit>.csv` mit neuen Spalten `Lauf` und `Leistungsbegrenzung_pct`.
  Prüfen mit `src/ModbusDiagnosticClient`.

### Tests
- `tests/FieldDevices.Tests`: Modbus-Anbindung (Kopf, Vorzeichen, Leistungsbegrenzung gültig/ungültig, Ein/Aus, Reset, Pause), Optionen,
  kompletter Standalone-Lauf mit CSV (`PvEngineTests`).
- Probelauf 2026-10-25 mit Master (x600) zusammen mit Batterie und Charger: Zeitstatus „synchron“, drei Simulatoren im Master.

### Bekannte Probleme
- Das Auto-Wetter ist lokal und nur mit `--seed` wiederholbar; es kommt später vom Wettersimulator.
- Die Modultemperatur startet immer bei 12 °C (Einschwingen dauert einige Stunden Modellzeit).
- Ein Neustart im Master behält den Gerätezustand (Energie, Verschmutzung); nur die Tagesenergie wird bei einem Datumswechsel zurückgesetzt.
- Die interaktive Anzeige und die Tasten sind nach dem Umbau nur mit umgeleiteter Ausgabe geprüft, noch nicht in einem echten Terminalfenster.
- Die versionierte Beispiel-CSV `docs/data/pv-simulation.csv` wird nicht mehr überschrieben (neuer Dateiname je Lauf).
