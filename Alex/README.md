# EnergySimulator – PV-Anlage (LB01)

Konsolensimulator für eine PV-Anlage im Rahmen von .NET-Programmierung 2026, LB01.

**Verantwortlich:** Alexander Lux  
**Team:** Alpha

## Voraussetzungen

- [.NET 10 SDK](https://dotnet.microsoft.com/download) – Projekt zielt auf `net10.0`
- Keine zusätzlichen Abhängigkeiten

## Starten

```bash
# Standardstart: aktuelles Datum, x60, CSV alle 15 min
dotnet run --project src/EnergySimulator

# Schneller Lauf
dotnet run --project src/EnergySimulator -- -s 1800

# Winter mit Schnee demonstrieren
dotnet run --project src/EnergySimulator -- -d 2026-01-15 -s 300

# Eigene Parameter
dotnet run --project src/EnergySimulator -- -p 15.5 -t 10

# Hilfe
dotnet run --project src/EnergySimulator -- --help
```

Die Simulation läuft **ohne Zeitlimit** und läuft bei Mitternacht in den nächsten
Tag weiter. Tagesenergie wird um 00:00 zurückgesetzt, die Gesamtenergie läuft durch.

### CLI-Optionen

| Option | Beschreibung | Default |
|--------|-------------|---------|
| `-s, --timescale` | Zeitraffer x1, x15, x60, x300, x1800 … | 60 |
| `-t, --step` | CSV-Aufzeichnungsintervall in Minuten | 15 |
| `-r, --refresh` | Bildschirmaktualisierung in Millisekunden | 400 |
| `-p, --peak` | Nennleistung in kW | 10.0 |
| `-o, --output` | Verzeichnis für CSV-Export | `data` |
| `-d, --start` | Startdatum, z. B. `2026-01-15` | jetzt |

## Bedienung (Tasten)

| Taste | Funktion |
|-------|---------|
| `P` | Pause / Fortsetzen |
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

**Zeitraffer**: `5` x1, `6` x15, `7` x60, `8` x300, `9` x1800

Jede Aktion wird im Display bestätigt.

## Modellbeschreibung

### Uhr und Kalender

Die Simulation startet mit aktuellem Datum und Uhrzeit und läuft unbegrenzt weiter.
Datum, Uhrzeit und Jahreszeit werden fortlaufend angezeigt.

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

- **CSV:** `data/pv-simulation.csv` – 15 Spalten, u. a. Datum, Jahreszeit, Wetter,
  Einstrahlung, Leistung, Tages- und Gesamtenergie, Temperaturen, Netzspannung,
  Verschmutzung, Alterung, Fehlerursache.
- **Screenshots:** `screenshots/`

## Projektstruktur

```
dotnet-energy-sim-lb01/
├── src/EnergySimulator/
│   ├── Program.cs                    # Entry Point, CLI-Parsing
│   ├── Simulation/
│   │   ├── SimulationEngine.cs       # Tick-Loop, Eingaben, Tageswechsel
│   │   ├── SimulationClock.cs        # Datum/Uhr, Zeitraffer, Tageswechsel
│   │   ├── SimulationConfig.cs       # Konfiguration
│   │   ├── SimulationMode.cs         # AUTO / MANUELL / PAUSE
│   │   └── Season.cs                 # Jahreszeiten und Saisonparameter
│   ├── Devices/
│   │   ├── IDevice.cs                # Geräte-Interface
│   │   ├── PvSystem.cs               # PV-Anlage
│   │   ├── PvFaultModel.cs           # Störungen, Temperatur, Spannung, Verschmutzung
│   │   ├── IrradiationCalculator.cs  # Einstrahlung aus Zeit, Wetter, Saison
│   │   ├── WeatherCondition.cs       # Wetterzustände
│   │   ├── FaultType.cs              # Störungsarten
│   │   └── InverterStatus.cs         # Wechselrichter-Status
│   ├── Output/
│   │   ├── ConsoleDisplay.cs         # Rahmen, Farben, Bestätigungen
│   │   └── BarChart.cs               # Balkenberechnung
│   └── Data/
│       └── CsvExporter.cs            # CSV-Export
├── data/                             # CSV-Ausgabe
├── screenshots/                      # Screenshots
└── docs/development-journal/
    └── alexander-lux.md              # Entwicklungsjournal
```

## Annahmen

- Linearer Wirkungsgrad, konstante Fläche, keine Verschattung
- Modultemperatur ohne Wind- und Luftströmungseinfluss
- Verschmutzung wirkt linear auf den Ertrag
- Einstrahlungsmaximum 1000 W/m² als Referenz (klarer Sonnenmittag)
- Tageslänge und Einstrahlung stark vereinfacht über die Jahreszeit skaliert
- Wetterzustände diskret, Übergänge als sprunghaft angenommene Änderung
- .NET 10 als Ziel, gebaut mit SDK 10.0.401