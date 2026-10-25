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

# Zweite Säule am selben Master (andere ID)
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
| Umgebung | 20 °C |
| Beharrungstemperatur bei 150 kW | 55 °C |
| Zeitkonstante τ (normal / Kühlungsdefekt) | 10 min / 30 min |
| Derating | ab 60 °C linear bis 0 % bei 80 °C |
| Störung | ab 80 °C automatisch, mit `F` sofort auf 85 °C |
| Reset | manuell (`R`) unter 60 °C, automatisch unter 50 °C |

```
T_ziel = 20 °C + 35 °C · (P / 150 kW)²       (Verluste ∝ I²)
T_neu  = T_ziel + (T_alt − T_ziel) · e^(−Δt / τ)
```

Die exakte Lösung statt eines Euler-Schritts bleibt bei jeder Schrittweite stabil. Bei normalen Fahrzeugen verhindert das Derating eine Störung: Die Leistung geht bei 80 °C auf 0, die Temperatur nähert sich 80 °C nur an. Wurde vor der Störung geladen, lädt der Punkt nach dem Reset weiter.

**Kia EV6 (defekt):** Der Temperatursensor der Ladebuchse ist ausgefallen, es gibt **kein Derating**. Dazu erwärmt der erhöhte Übergangswiderstand den Stecker doppelt so stark (`T_ziel = 20 °C + 70 °C · (P / 150 kW)²`). Ab ca. 140 kW steigt die Temperatur über 80 °C und nur die Abschaltung der Säule schützt. Mit 150 kW allein am Punkt ist er nach ca. 20 min `FAULTED`, kühlt ab, setzt sich unter 50 °C zurück und lädt weiter. Teilt er sich die 150 kW mit dem anderen Punkt, bleibt er kühl.

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
├── Simulation/                       SimulationEngine, AutoModeController, SimulationConfig
├── IO/                               ConsoleRenderer, InputHandler, CsvLogger, ArgsParser
└── Properties/launchSettings.json    Startprofile „Master“ und „Standalone“ für Rider/Visual Studio

docs/                                 zentral im Repository
├── data/                             CSV-Ausgaben (<ID>_<Zeitstempel>.csv)
├── devices/DualPortCharger/          design-notes.md, simulationsablauf.md
├── screenshots/DualPortCharger/      Screenshots
└── development-journal/susanne-lastowicka.md   Entwicklungsjournal
```


---

## LB02 – Umbau zum Feldgerät im Gesamtsystem

**Zweck:** Dual-Port-DC-Ladesäule als Feldgerät, das der gemeinsamen virtuellen Zeit folgt (später mit Modbus-Server).
**Verantwortlich:** Susanne Lastowicka (Gerät aus LB01; im Team noch zu bestätigen)
**Basis aus LB01:** Susi DualCharger, ChargingPoint, Vehicle, VehicleCatalog, AutoModeController

### Stand (2026-10-25)

| Schritt | Stand |
|---|---|
| Zeit vom Simulations-Master (`SyncedClock`, `TimeSyncClient`) | ✅ |
| Standalone-Betrieb mit lokaler Uhr (`--standalone`) | ✅ |
| async Schleife, CSV, Tastatur (kein `Thread.Sleep`, kein `ReadLine`) | ✅ |
| Modbus-Server, Registerplan | offen (nach dem gemeinsamen Registerkopf) |
| Umgebungstemperatur aus dem Wetter | offen |

### Schnittstellen
- **Benötigt:** Simulations-Master (TCP, Standard `127.0.0.1:5100`, Protokoll in `src/SimCore.TimeSync/README.md`).
- **Angeboten:** CSV-Datei; Modbus folgt.

### Tests
- Die Zeitanbindung ist in `tests/Devices.Common.Tests` getestet (Schrittsteuerung, Optionen, Zeitquelle mit echtem Master über TCP).
- Probeläufe am 2026-10-25 (umgeleitete Ausgabe):
  - Standalone `--start "2026-01-15 00:00" --speed 100000 --duration 24 --seed 42`: endet von selbst mit Exit-Code 0,
    CSV mit 290 Zeilen von 2026-01-15 00:00 bis 2026-01-16 00:00.
  - Master (x600) + DC-01 (5 min) + DC-02 (1 min): beide „synchron“, Abweichung ≤ 0,2 s (Toleranz 6 s), Master listet beide.
    Master hart beendet ⇒ beide melden nach 3 s „MASTER NICHT ERREICHBAR – Zeit steht“.

### Bekannte Probleme
- Interaktive Anzeige und Tasten nach dem Umbau noch nicht in einem echten Terminalfenster getestet.
- Ein Neustart im Master behält den Zustand der Säule (Fahrzeuge bleiben angesteckt); ob das gewünscht ist, im Team klären.
- Die Screenshots stammen noch aus LB01.
