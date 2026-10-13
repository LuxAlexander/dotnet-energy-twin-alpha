# AGENTS.md – dotnet-energy-twin-alpha

**Stand:** 2026-10-13 · **Phase:** LB01 abgeschlossen (3 Einzelsimulatoren), LB02-Projektstruktur angelegt (`EnergyTwin.sln`, `src/`, `tests/`), noch kein LB02-Code

Pflichtlektüre für jeden Agenten in diesem Repo. Sie fasst die drei LB01-Projekte und den LB02-Plan
zusammen, damit nicht alles neu analysiert werden muss. Details:
- Modell und Bedienung je Gerät → jeweilige `README.md` (Quelle der Wahrheit für Formeln).
- Ursprüngliches LB02-Planungsdokument von Alex (Stand 2026-10-10, englisch) → `AGENTS1.md`.
  Diese Datei übernimmt dessen Inhalt und gleicht ihn mit dem tatsächlichen Code hier ab.

---

## 1. Kontext

- „.NET-Programmierung 2026“, Team **Alpha**, 3 Personen. Sprache für UI, Kommentare, Doku, CSV: **Deutsch**.
- **LB01:** Jede Person hat ein Gerät als eigene Konsolen-App simuliert (C#, `net10.0`, `Nullable` und
  `ImplicitUsings` an, keine NuGet-Pakete). Es gibt keine gemeinsame Solution und keine Tests.
- **LB02** („Vom einzelnen Simulator zum verteilten Teamsystem“): mehrere getrennte Programme,
  gekoppelt über eine verteilte virtuelle Zeit, einen Wetterdienst und Modbus TCP. Abgabe als Git-Tag `lb02-2026` (Moodle).

Die LB01-Projekte wurden am 2026-10-13 per `git mv` (Historie bleibt) in die LB02-Geräteprojekte verschoben.
Die Ordner `Alex/`, `Domi/`, `Susi/` und deren `.csproj`/`.sln` gibt es nicht mehr.

| Jetzt in | Gerät (LB01) | Autor/in | früher | Namespace (unverändert) | Einstieg |
|---|---|---|---|---|---|
| `src/Devices/PvSimulator/` | PV-Anlage | Alexander Lux | `Alex/src/EnergySimulator` | `EnergySimulator.*` | `Program.cs` (Top-Level) |
| `src/Devices/BatterySimulator/` | Batteriespeicher | Dominik Zach | `Domi/Batterie` | `Batterie_Sim` | `main.cs` (Top-Level) |
| `src/Devices/DualPortCharger/` | DC-Ladesäule, 2 Ladepunkte | Susanne Lastowicka | `Susi/src/DualChargerSim` | `DualChargerSim.*` | `Program.cs` (`Main`) |

Jedes Geräteprojekt enthält den LB01-Code, die LB01-`README.md` (Startbefehle, Pfade und Projektstruktur an das neue Layout angepasst,
LB02-Gerüst als Abschnitt am Ende). Die Assembly-Namen sind jetzt `PvSimulator`, `BatterySimulator` und `DualPortCharger`.

**Alle Dokumente liegen zentral in `docs/`** (die Geräteordner enthalten nur Code, `.csproj` und `README.md`):

```
docs/
├── data/                          ALLE CSV-Ausgaben (Standard-Ausgabeordner aller Geräte) + Beispiel-CSVs
├── development-journal/           alexander-lux.md, dominik-Zach.md, susanne-lastowicka.md (in LB02 weiterführen)
├── devices/DualPortCharger/       design-notes.md, simulationsablauf.md (Susi)
├── screenshots/<Gerät>/           PvSimulator/, BatterySimulator/ (bild.png), DualPortCharger/
└── register-plans/                Registerpläne pro Gerät (leer)
```

**CSV-Ausgabeort:** `Shared.Config.OutputPaths.CsvDirectory` sucht vom Programmordner aufwärts nach `EnergyTwin.sln`
und liefert `<Repo>/docs/data`, egal ob aus dem Repo-Root, dem Projektordner oder Visual Studio (`bin/…`) gestartet
wird. Liegt das Programm außerhalb des Repos, wird `./docs/data` verwendet. PV (`-o`) und Charger (`--output`) können den Ordner
weiterhin überschreiben, die Batterie hat dafür keine Option. Neue Programme sollen denselben Standard verwenden.

---

## 2. LB02 – Anforderungen (Kurzfassung aus der Aufgabenstellung)

**Programme (≥ 4 Arten):**
1. **Simulations-Master** – besitzt die virtuelle Zeit des Gesamtsystems: Neustart, Pause, Fortsetzen, Tempo ändern, Statusanzeige.
2. **Wettersimulator** – eigenständig, liefert Wetter für (Ort, virtuelle Zeit), deterministisch und reproduzierbar.
3. **Feldgeräte** – PV, Batterie, Dual-Port-Charger + ein viertes (Wind **oder** Industrieverbraucher; geplant: Wind).
4. **Modbus-Diagnose-Client** – liest gemeinsamen Registerkopf und gerätespezifische Register, schreibt einen Sollwert, zeigt Fehler an.

**Muss vorführbar sein:** Alle Programme getrennt gestartet → virtuelle Zeiten stimmen innerhalb einer Toleranz ·
Tempowechsel im Betrieb ohne Auseinanderlaufen · globale Pause, wobei die Modbus-Server erreichbar bleiben und den letzten Stand zeigen ·
Fortsetzen ohne Zeitsprung · Neustart mit neuer virtueller Zeit, den alle übernehmen · gleicher (Ort, Zeit) → gleiches Wetter ·
Client liest Register und schreibt einen Sollwert, der im Simulator sichtbar wird · kontrolliertes Beenden aller Programme.

**Fehlerbehandlung:** Master/Simulator kurz nicht erreichbar → sichtbarer Fehler, kein stilles Auseinanderlaufen ·
ungültige oder fehlende Wetterantwort → sichtbarer Fehler · Modbus-Verbindungsabbruch wird behandelt ·
ungültiger oder unerlaubter Schreibwert → abgelehnt mit Fehler · Beenden durch den Benutzer → geordnetes Herunterfahren.

**Tests (Pflicht):** Zeitsteuerung des Masters (mind. Pause/Fortsetzen/Tempo/Neustart) und Reproduzierbarkeit des Wetters.

**Doku (Pflicht):** zentrale `README.md` (Architektur, Startreihenfolge, Module, **Zuständigkeiten**) · `README.md` pro Projekt
(Zweck, Verantwortliche/r, Konfiguration, Bedienung, Schnittstellen, Tests, bekannte Probleme) · **Registerplan pro Gerät**
(Host, Port, Unit-ID, Geräte-ID, Adresse, Typ, Datentyp, Skalierung, Zugriff, gültiger Bereich, Bedeutung) ·
`AGENTS.md` · `.env.example` oder Beispielkonfiguration · Entwicklungsjournale.

### Harte Regeln (nie verletzen)
- **Kein Internet zur Laufzeit** – Wetter wird lokal und deterministisch berechnet, keine APIs.
- **Keine Secrets im Repo** – nur `.env.example`, `.env` nie committen.
- **async/await durchgängig:** kein `Thread.Sleep`, kein `.Wait()`/`.Result`, `CancellationToken` bei langen
  Operationen, Suffix `Async` an async-Methoden.
- **EasyModbusTCP.NETCore ist synchron** – akzeptierte Bibliotheksgrenze, **nicht** in `Task.Run` verpacken.
- **Build mit 0 Fehlern und 0 Warnungen.**

---

## 3. LB02 – Architektur (Entscheidungen aus `AGENTS1.md`)

- **Bibliotheken (ohne Zyklen):** `SimCore.Contracts` (Interfaces + DTOs: `ITimeProvider`, `IWeatherService`, `WeatherData`,
  `TimeState`, `DeviceConfig`; **keine** Abhängigkeiten) ← `SimCore.Time` (virtuelle Zeit, `TimeControlCmd`) ←
  `Modbus.Contracts` (Registeradressen, `RegisterMap`, `DeviceId`, `RegisterPlan`) ← `Devices.*` (+ EasyModbusTCP.NETCore).
- **Zeitverteilung:** Master = TCP-Server, Geräte = TCP-Clients; Master pusht `TimeState` bei Änderung + Heartbeat.
  Nicht erreichbare Geräte werden als offline markiert, die anderen laufen weiter.
- **Wetter:** reine Funktion von (Ort, virtuelles Datum/Stunde), Seed aus Tagesindex + Ort (Muster: Alex' `DailyOffsetC`).
  Antwort `WeatherData { IrradiationWm2, SunHeight, CloudCover, TemperatureC, WindSpeedMs, Condition }`.
  Basis: Alex' `Season` + `IrradiationCalculator`, erweitert um Wind.
- **Async:** `async Task StartAsync(CancellationToken ct)` mit `await Task.Delay(…, ct)`; Tastatur über eine async-Task.
- **Modbus:** Jedes Geräte-Programm hostet einen eigenen `EasyModbus.ModbusServer` (Ports ab 1502). Holding Registers:
  0–9 gemeinsamer Kopf, 10–29 gerätespezifisch, ein schreibbarer Sollwert. `double` → 2 Register, IEEE-754 float,
  Big-Endian-Wortreihenfolge. Der Server bleibt während der Pause erreichbar.
- **Zuständigkeiten laut `AGENTS1.md`:** A = Alexander (Master, `SimCore.Time`, `SimCore.Contracts`), B = Wettersimulator,
  C = alle 4 Feldgeräte + Diagnose-Client + `Modbus.Contracts`. **B und C sind dort nicht namentlich zugeordnet**
  (siehe offene Fragen).

### Entscheidung 2026-10-13: ein Projekt pro Programm, eine Solution

**Jedes Programm ist ein eigenes Exe-Projekt, gemeinsamer Code liegt in Klassenbibliotheken, alles in `EnergyTwin.sln`.**
Nicht gewählt: ein Projekt mit mehreren Startdateien. In .NET hat ein Projekt genau einen Einstiegspunkt
(sonst CS0017/CS8802); die Alternative wäre eine Exe mit Unterbefehlen.
Gründe für die Entscheidung:
- LB02 verlangt getrennte Programme, „sinnvoll geschnittene Assemblies“ und README und Zuständigkeit pro Projekt.
- Abhängigkeiten bleiben gezielt: nur Geräte und der Diagnose-Client bekommen EasyModbus.
- Der Compiler erzwingt die Schichten, weil keine Zyklen möglich sind.
- Weniger Merge-Konflikte im Team, Tests referenzieren gezielt einzelne Bibliotheken.
- Mit `dotnet build EnergyTwin.sln` baut trotzdem alles auf einmal.
- Starten: später ein Skript `start-all.ps1` bzw. „Mehrere Startprojekte“ in VS/Rider.

### Angelegte Struktur (Projektdateien, README-Gerüste; C#-Code bisher nur der verschobene LB01-Code in PV/Batterie/Charger)

```
EnergyTwin.sln                     Solution (Ordner src, src/Devices, tests)
Directory.Packages.props           zentrale NuGet-Versionen (EasyModbusTCP.NETCore 1.0.0, xUnit 2.9.3, Test.Sdk 17.14.1)
src/Directory.Build.props          net10.0, Nullable, ImplicitUsings, TreatWarningsAsErrors=true
                                   (gilt jetzt auch für den verschobenen LB01-Code → Warnungen brechen den Build)
tests/Directory.Build.props        importiert src/…, IsPackable=false
src/
  SimCore.Contracts/       lib   keine Abhängigkeiten
  SimCore.Time/            lib   → Contracts
  Modbus.Contracts/        lib   → Contracts
  Shared.Config/           lib   → Contracts                (in AGENTS1.md: Shared/DeviceConfig); enthält OutputPaths
  Devices.Common/          lib   → Contracts, Time, Modbus.Contracts, Shared.Config + EasyModbusTCP.NETCore
  SimulationMaster/        exe   → Contracts, Time, Shared.Config
  WeatherSimulator/        exe   → Contracts, Shared.Config
  ModbusDiagnosticClient/  exe   → Contracts, Modbus.Contracts, Shared.Config + EasyModbusTCP.NETCore
  Devices/PvSimulator/        exe → Devices.Common   enthält LB01-Code von Alex
  Devices/BatterySimulator/   exe → Devices.Common   enthält LB01-Code von Domi
  Devices/DualPortCharger/    exe → Devices.Common   enthält LB01-Code von Susi
  Devices/WindSimulator/      exe → Devices.Common   leer
tests/
  SimCore.Tests/           xUnit → SimCore.Time
  Weather.Tests/           xUnit → WeatherSimulator (Exe-Referenz; ggf. Wetterlogik später in eine lib auslagern)
docs/                      zentrale Doku und CSV-Ausgaben (Aufbau siehe §1)
```
Jedes Projekt hat eine `README.md` als Gerüst (Zweck, Verantwortlich, Basis aus LB01, Konfiguration, Bedienung,
Schnittstellen, Tests, Bekannte Probleme). Noch offen: zentrale `README.md`, `.env.example`, `start-all.ps1`.
Neue NuGet-Pakete nur in `Directory.Packages.props` versionieren, im `.csproj` ohne `Version`.

### Offene Fragen (müssen im Team entschieden werden)

| # | Frage | Tendenz laut `AGENTS1.md` |
|---|---|---|
| 1 | Transport Wettersimulator | HTTP |
| 2 | Transport Zeitverteilung | TCP-Push |
| 3 | Drift-Toleranz | 250 ms Sim-Zeit |
| 4 | Registeradressen je Gerät | 0–9 Kopf, 10–29 gerätespezifisch |
| 5 | Wie finden Geräte den Master | Konfigurationsdatei (IP:Port) |
| 6 | Ort fürs Wetter | ein globaler Ort |
| 7 | Komplexität des Batteriemodells | einfaches SOC-Modell |
| 8 | Was lädt der Charger | „Batterie aus PV-Überschuss“ |

---

## 4. Was aus LB01 wohin wandert

| LB02-Baustein | Aus LB01 übernehmen | Anpassen |
|---|---|---|
| `SimCore.Time` / Master | Alex `SimulationClock` (`DateTime`, Tageswechsel); Domis **Zeitkonto mit festen Schritten** (`Simulator.Run`); Pause ohne Zeitsprung (Alex/Domi setzen `_lastTick` bzw. `lastRealSeconds` beim Fortsetzen neu); Susis Tempo-Begrenzung 1–100 000 | `Thread.Sleep` → `Task.Delay`; Zeit kommt vom Master statt lokal |
| `WeatherSimulator` | Alex `Season`/`SeasonInfo`, `IrradiationCalculator`, `WeatherCondition`, `PvFaultModel.AmbientTemperature` + `DailyOffsetC` | `UpdateAutoWeather`/`RandomWeather` sind heute **zeit-geseedet** (`new Random()`) → deterministisch aus (Ort, Tag, Stunde); Wind neu |
| PV-Gerät | Alex `PvSystem`, `PvFaultModel` (ohne Wetter und Umgebungstemperatur) | Wetter per Anfrage; `IDevice` an Contracts anpassen; `InverterStatus.cs` ist toter Code |
| Batterie-Gerät | Domi `Batterie` (kennt weder Konsole noch Uhr), `BatterieConfig`, `DeviceState`, `FaultType` | `SetPower(kW)` = schreibbarer Modbus-Sollwert; `Update(dtSeconds)` → `TimeSpan` |
| Charger-Gerät | Susi `DualCharger`, `ChargingPoint`, `Vehicle`, `VehicleCatalog`, `AutoModeController` | `PowerLimitKW` (50–350, Clamp) eignet sich als Sollwert; ungültige Schreibwerte **ablehnen** statt clampen; Umgebungstemperatur 20 °C → aus dem Wetter |
| Gemeinsame Hilfen (`Shared`/`Devices.Common`) | Thermik: Susis exakte Exponentiallösung + Derating + Hysterese (alle drei haben das Muster); `BatteryPack` + Ladekurve (Domi linear, Susi exponentiell); Energiezähler; CSV-Writer (Susi: StreamWriter, AutoFlush, BOM); Konsolen-UI (Alex' differentieller Renderer, `BarChart`; Susis `Fit`/`Prompt`); `ArgsParser` (Susi: Validierung, Punkt und Komma) | einheitliche Einheiten (`KW`/`KWh`, Δt als `TimeSpan`), ein Zahlenformat für CSV, eine Vorzeichenregel für Leistung, ein geseedetes `Random` |

---

## 5. Die drei LB01-Projekte im Detail

Dateipfade in diesem Abschnitt sind relativ zum jeweiligen Geräteordner unter `src/Devices/`.

### Alex – PV-Anlage (`src/Devices/PvSimulator/`)
Start: `dotnet run --project src/Devices/PvSimulator -- [-s 60] [-t 15] [-r 400] [-p 10.0] [-o docs/data] [-d 2026-01-15]`
(`-s` Zeitraffer, `-t` CSV-Intervall in min, `-r` Bild-Refresh ms, `-p` Nennleistung kW, `-o` CSV-Ordner, `-d` Startdatum).
Läuft **endlos**, um 00:00 wird die Tagesenergie zurückgesetzt. `ENERGYSIM_RENDER_LOG=<datei>` protokolliert das Rendering.

- `Simulation/SimulationEngine.cs` – Schleife im 80-ms-Takt, **kontinuierlich** (Δt = Realzeit × Zeitraffer, in Stunden),
  Tasten, Auto-Wetter, CSV-Intervall, Meldungen (4 s).
- `Simulation/SimulationClock.cs` (`DateTime`, `Advance(realMs)` meldet Tageswechsel), `Season.cs` (Jahreszeit aus Monat:
  Einstrahlungsfaktor, Sonnenauf-/untergang, Grundtemperatur, Schneewahrscheinlichkeit), `SimulationMode.cs`
  (`Auto | Manual | Paused`), `SimulationConfig.cs`.
- `Devices/IDevice.cs` – `Name, IsEnabled, PowerKw, DailyEnergyKwh, TotalEnergyKwh, StepContinuous(now, simHours), ResetDailyEnergy()`.
- `Devices/PvSystem.cs` – Leistung = Peak · min(1, E/1000) · max(0.1, 1 − Verschmutzung − Alterung).
- `Devices/IrradiationCalculator.cs` – Halbsinus zwischen Auf- und Untergang × Wetterfaktor × Jahreszeitfaktor.
- `Devices/PvFaultModel.cs` – Modultemperatur (Gleichgewicht = Außen + 0.04·E, τ 0.8 h / 2.5 h, Tages-Hitzewelle aus dem Datum),
  Netzspannung (230 V − Last + Einspeisung + Rauschen, gültig 200–250 V), Verschmutzung/Alterung,
  Störungen: Inverter, Überhitzung (≥ 70 °C, Reset < 55 °C), Netzspannung.
- `Devices/WeatherCondition.cs`, `FaultType.cs` (`None, Inverter, Overheat, GridVoltage`), `InverterStatus.cs` (unbenutzt).
- `Data/CsvExporter.cs` – schreibt `docs/data/pv-simulation.csv`, `;`, InvariantCulture (Punkt), 16 Spalten.
  Der Dateiname ist fest, deshalb **überschreibt jeder Lauf die versionierte Beispiel-CSV**.
- `Output/ConsoleDisplay.cs` – differentielles, flackerfreies Rendering mit ANSI-Farben. Box-Breite 76 Zeichen
  (`InnerWidth = 70`, `BorderWidth = 72`, nicht ohne Test ändern), Fallback bei umgeleiteter Ausgabe, `Release()` am Ende.
  `Output/BarChart.cs` – `Split(value, max, width)` → (`█…`, `░…`).

Tasten: `P` Pause, `M` Auto/Manuell, `Q` Ende, `C` Module reinigen, `R` Refresh-Rate, `F` Inverterfehler, `T` Überhitzung,
`G` Netzstörung, `0–4` Wetter (manuell), `5–9` Zeitraffer x1/x15/x60/x300/x1800.

### Domi – Batteriespeicher (`src/Devices/BatterySimulator/`)
Start: `dotnet run --project src/Devices/BatterySimulator -- --id BAT-01 --capacity 20 --max-charge 5 --max-discharge 5 --soc-min 10
--soc-max 100 --soc-start 50 --speed 600 --step 60 --duration 24 --start 06:00`
(`--speed` Sim-Sekunden je Real-Sekunde, `--step` **Sekunden**, `--duration` h mit `0` = endlos, `--start` **Uhrzeit**).
Argumente mit Dezimalpunkt, im Pausenmenü im Format des Betriebssystems (Komma).

Alle Dateien liegen flach im Projektordner (Block-Namespaces `namespace Batterie_Sim { }`):
- `Batterie.cs` – Gerätephysik. API: `SetPower(kW)`, `Update(dtSeconds)`, `TriggerFault`, `ResetFault`, `TurnOn/Off`.
  **Vorzeichen: + = Laden, − = Entladen.** Die Ist-Leistung ist der Sollwert, begrenzt auf [−maxEntl·Derating, +maxLaden·Taper·Derating];
  die SOC-Grenzen werden exakt eingehalten. Taper ab 80 % SOC linear auf 10 %.
  Temperatur (Euler): ΔT/min = 0.06·|P| − 0.02·(T−20) + Störungswärme; Derating 45→60 °C, Abschaltung 60 °C,
  Freigabe ≤ 50 °C (`Cooldown`). Zähler für geladen/entladen und Vollzyklen.
- `BatterieConfig.cs` (`Efficiency` unbenutzt; die doppelte Eigenschaft `Batterie.BatterieId` wurde entfernt), `Enum.cs` (`DeviceState`: Off/Standby/Charging/Discharging/Fault/Cooldown;
  `ControlMode`: Auto/Manual; `FaultType`: None/EmergencyStop/Overheat).
- `DailyProfile.cs` – Auto-Sollwert: 07–17 Uhr Laden (Sinus), 18–23 Uhr Entladen (Spitze 19:30), ±10 % Rauschen je 5 min.
- `Simulator.cs` – 100-ms-Schleife mit **Zeitkonto** (Realzeit × Speed, pro voller Schrittweite ein `Step()`).
  Sim-Zeit als `TimeSpan` ohne Datum. Tasten + Pausenmenü mit `ReadLine`.
- `CsvLogger.cs` – `docs/data/battery_<ID>_<Datum>.csv`, `;`, Punkt. `ConsoleDashboard.cs` – `SetCursorPosition(0,0)`, Balken `#`/`-`.
- Beispiel-CSV: `docs/data/battery_BAT-01_2026-10-07.csv`. Screenshot: `docs/screenshots/BatterySimulator/bild.png`.

Tasten: Leertaste Pause/Menü, `M` Auto/Manuell, `↑/↓` Sollwert ±0.5 kW, `0` Sollwert 0, `E` Ein/Aus, `N` Not-Aus,
`H` Überhitzung, `R` Störung zurücksetzen, `Q` Ende.

### Susi – Dual-Port-DC-Charger (`src/Devices/DualPortCharger/`)
Start: `dotnet run --project src/Devices/DualPortCharger -- --id DC-01 --power 150 --step 5 --speed 120 --duration 24 --seed 42`
(`--power` 50–350 kW, `--step` **Minuten**, `--speed` Faktor, `--seed` macht Läufe reproduzierbar). Ungültige Argumente → Hilfetext, Exit-Code 1.

- `Models/DualCharger.cs` – gemeinsames Budget. `AllocatePower` gibt jedem Punkt zuerst bis zur Hälfte, den Rest bekommt der andere.
  Warteschlange max. 5 min, danach „abgewiesen“; Ein/Aus; `Tick(now, dt)`.
- `Models/ChargingPoint.cs` – Zustandsautomat `Available → Connected → Charging → Finished`, `Faulted` aus jedem Zustand
  (Guard-Methoden liefern `bool`). η = 0.95. Temperatur exakt exponentiell: T_ziel = 20 + 35·(P/150)², τ 10 min
  (30 min bei Kühlungsdefekt). Derating 60→80 °C, Störung ≥ 80 °C, Reset manuell < 60 °C oder automatisch < 50 °C.
- `Models/Vehicle.cs` – Kapazität, SOC, Ziel-SOC; Ladekurve ab 80 % exponentiell bis 10 %; `AddEnergy` hört beim Ziel-SOC auf.
  `VehicleCatalog.cs` – 7 Modelle (inkl. LKW und defektem Kia EV6 ohne Derating).
- `Simulation/SimulationEngine.cs` – feste Schritte, Echtzeit-Wartezeit = Schritt/Tempo, Tastatur-Polling alle 50 ms,
  Sim-Zeit als `DateTime`. `AutoModeController.cs` – Ankünfte als Rate je Tageszeit (p = 1 − e^(−Rate·Δt)), Verweilzeiten.
  `SimulationConfig.cs` (init-only, Min-/Max-Tempo).
- `IO/ArgsParser.cs` (`ArgumentException`, Punkt **und** Komma), `IO/CsvLogger.cs` (`docs/data/<ID>_<yyyy-MM-dd_HHmmss>.csv`, `;`,
  **de-AT Dezimalkomma**, UTF-8 mit BOM, AutoFlush), `IO/ConsoleRenderer.cs` (flackerfrei, `Fit`/`Bar` `█░`, `Prompt`,
  Fallback bei Umleitung), `IO/InputHandler.cs` (Taste → Engine, Rückmeldung in `Message`).
- `Properties/launchSettings.json` – Startprofil ohne `--output` (schreibt also nach `docs/data`).
- Doku: `docs/devices/DualPortCharger/`, Screenshots: `docs/screenshots/DualPortCharger/`. Alte CSV-Läufe liegen in `docs/data/` (über `*.csv` ignoriert).

Tasten: `P` Pause, `A` Auto an/aus, `N` Fahrzeug kommt (manuell), `1/2` Punkt wählen, `C` Anstecken (Dialog), `D` Abstecken,
`S` Start/Stop, `F` Störung, `R` Reset, `+/-` Limit ±10 kW, `</>` Tempo ÷2/×2, `O` Säule ein/aus, `Q` Ende.

### Unterschiede, die bei der Zusammenführung zählen

| Thema | Alex (PV) | Domi (Batterie) | Susi (Charger) |
|---|---|---|---|
| Zeitschritt | kontinuierlich, Δt in **h** (double) | fest, Δt in **s** (double) | fest, Δt als **TimeSpan** |
| Sim-Zeit | `DateTime`, endlos | `TimeSpan` (nur Uhrzeit) | `DateTime`, mit Ende |
| Warten | `Thread.Sleep(80)` | `Thread.Sleep(100)` | `Thread.Sleep` (≤ 50 ms) |
| Modus | `SimulationMode` Auto/Manual/Paused | `ControlMode` + `IsPaused` | `bool IsAutoMode` + `IsPaused` |
| Störung | `FaultType` (eigene Werte) | `FaultType` (**gleicher Name, andere Werte**) | `ChargingPointState.Faulted` |
| Temperatur | exponentielle Annäherung | Euler-Schritt | exakte Exponentiallösung |
| CSV-Zahlen | Punkt (Invariant) | Punkt (Invariant) | Komma (de-AT), BOM |
| Einheiten | `PowerKw`, `Kwh` | `ActualKW`, `KWh` | `PowerKW`, `KWh` |
| Zufall | `new Random()` | `new Random()` im Profil | ein `Random`, `--seed` |
| Tastenkonflikte | `P` Pause, `M` Modus, `R` Refresh, Ziffern = Wetter/Tempo | Leertaste Pause, `M` Modus, `R` Reset | `P` Pause, `A` Modus, `R` Reset, Ziffern = Punkt |

---

## 6. Build-Status (geprüft am 2026-10-13, .NET SDK 10)

`dotnet build EnergyTwin.sln` (nach dem Verschieben):

| Projekt | Ergebnis |
|---|---|
| alle Bibliotheken, Testprojekte | ✅ bauen |
| `Devices/PvSimulator` | ✅ baut, Probelauf ok (Anzeige, Leistung, CSV) |
| `Devices/DualPortCharger` | ✅ baut, Probelauf ok (2 h Sim-Zeit, CSV mit 26 Zeilen) |
| `Devices/BatterySimulator` | ✅ baut (Nullable-Warnungen am 2026-10-13 behoben) |
| `SimulationMaster`, `WeatherSimulator`, `ModbusDiagnosticClient`, `Devices/WindSimulator` | ❌ `CS5001`, noch kein `Program.cs` (erwartet) |

Die Probeläufe liefen mit umgeleiteter Ausgabe. Die Konsolenausgabe von Alex formatiert Zahlen mit Komma, die CSV mit Punkt.

## 7. Bekannte Probleme

- **`.gitignore`:** Die Regel `output/` wurde in Commit `0667108` entfernt (damit Alex' `Output/` versioniert wird).
  Stattdessen ignoriert sie jetzt `*.csv` (noch nicht committet). Damit werden neue CSV-Läufe in `docs/data/` nicht versioniert.
  Die beiden Beispiel-CSVs (`docs/data/pv-simulation.csv`, `docs/data/battery_BAT-01_2026-10-07.csv`) sind bereits
  versioniert und bleiben es. Änderungen daran (z. B. durch einen PV-Lauf) erscheinen in `git status`.
- Alex `CsvExporter` hält die ganze CSV im `StringBuilder` und schreibt alle 5 Zeilen die **komplette Datei** neu.
  Bei Endlosbetrieb wächst das ohne Grenze. (`AGENTS1.md` nennt das „inkrementell“, das stimmt nicht.)
- Alex: Die Meldung „kWh Vortag“ beim Tageswechsel zeigt `TimeOfDay.TotalHours` statt der Tagesenergie.
  `InverterStatus.cs` ist toter Code.
- Alex' Auto-Wetter ist zeit-geseedet und damit **nicht** reproduzierbar (für LB02 ändern).
- Domi: Die README nennt als `--duration`-Standard 24, der Code 0 (endlos). `BatterieConfig.Efficiency` wird nicht benutzt.

## 8. Konventionen und Stolperfallen

- `net10.0`, `Nullable` + `ImplicitUsings` an, **0 Warnungen**. Bezeichner englisch (Ausnahme: Domi `Batterie*`),
  Texte deutsch.
- Gerätelogik bleibt frei von Konsole, Uhr und Netzwerk (Vorbild: `Batterie`, `DualCharger`). Ein-/Ausgabe in eigenen Klassen.
- Umgeleitete Konsole ≠ echtes Terminal: Cursor- und Puffer-Funktionen brauchen einen Fallback (Alex und Susi haben einen).
  Immer auch in einem echten Terminalfenster testen; ein NullRef beim ersten Diff-Frame endet mit Exit-Code -1073741819.
- Reproduzierbarkeit: `Random` aus Datum/Ort/Seed erzeugen, nie aus der Uhrzeit.
- Umgebungsvariablen nur für optionale Entwickler-Features (Muster: `ENERGYSIM_RENDER_LOG`).
- Bei Modelländerungen die Formeln in der jeweiligen `README.md` mitpflegen. Bei Architekturentscheidungen
  §3 dieser Datei aktualisieren und einen Eintrag im Entwicklungsjournal machen.
