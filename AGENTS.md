# AGENTS.md – dotnet-energy-twin-alpha

**Stand:** 2026-10-27 · **Phase:** LB02-Endstand: alle Programme fertig und an Master, Modbus und Wettersimulator angebunden; viertes Feldgerät = Kühlhaus (Wind entfallen); zusammen 206 Tests (54 + 51 + 28 + 62 + 11); PV-CSV-Fehler behoben (de-AT Dezimalkomma) und CSV-/Konsolen-Helfer nach `src/Shared` ausgelagert (§0.6a im Archiv).

Pflichtlektüre für jeden Agenten in diesem Repo. Sie fasst die drei LB01-Projekte und den LB02-Plan
zusammen, damit nicht alles neu analysiert werden muss. Details:
- Modell und Bedienung je Gerät → jeweilige `README.md` (Quelle der Wahrheit für Formeln).
- Historisches LB02-Planungsdokument von Alex (Stand 2026-10-10, englisch) →
  `docs/development-journal/AGENTS1-archiv-2026-10-10.md`. Es ist **obsolet** (nur Archiv); sein Inhalt ist
  vollständig in diese Datei übergegangen und mit dem tatsächlichen Code abgeglichen.

---

## 0. Übergabe – hier weitermachen (Stand 2026-10-27)

Dieser Abschnitt reicht, um an einem neuen Tag ohne Vorwissen weiterzuarbeiten. §1–§8 enthalten die Details.

### 0.1 Arbeitsregeln für Agenten (von Alexander festgelegt, immer einhalten)
- **Entwicklungsjournal:** Jede Änderung wird automatisch in `docs/development-journal/` unter der jeweiligen Person eingetragen.
  Pro Tag gibt es einen Eintrag `## JJJJ-MM-TT – Titel`, weitere Arbeiten am selben Tag kommen als `### Nachtrag JJJJ-MM-TT – Titel` dazu.
  Felder: `**Done:**`, `**KI:**` (z. B. „Claude Opus 5.5 => …“), `**Artefact:**`, `**Test:**`, `**Comment:**`.
  **Nur diese Datei bearbeiten**, nie die Journale der anderen Personen. Maßgeblich ist das Journal der Person, in deren
  Arbeitsstand gerade gearbeitet wird (aktuell: PV → `alexander-lux.md`).
- **Commits nur auf Anweisung.** Die Commit-Message der Userin wird **wörtlich** übernommen, **ohne** `Co-Authored-By`-Zeile.
  Nicht pushen, außer es wird ausdrücklich verlangt.
- Die PDF der Aufgabenstellung (`dotNET Programmierung 2026 - Aufgabe 02.pdf`) liegt nur lokal im Repo-Root und ist in `.gitignore`.
  Für Anforderungen genügt §2.
- Antworten, Doku, Kommentare und UI auf **Deutsch**.
- **Tests (Claude AI):** Testläufe nur **vor einem Commit** oder wenn **ausdrücklich gewünscht**;
  nach jeder Änderung `dotnet build EnergyTwin.sln` (0 Warnungen). Dann READMEs, diese Datei
  (Stand-Kopf, §0, §3, §6) und das Journal nachziehen.

### 0.2 Git-Stand
`main`,(Commit nur auf Anweisung, Message wörtlich, ohne Co-Author).

### 0.3 Was fertig ist

| Baustein | Stand | Wichtigste Dateien |
|---|---|---|
| `SimCore.Contracts` | ✅ `TimeState`, `SimMode`, `ISimClock`, `IWeatherService`, `WeatherData`, Sync-Nachrichten und Protokoll | `TimeState.cs`, `SyncMessages.cs`, `SyncProtocol.cs`, `IWeatherService.cs`, `WeatherData.cs` |
| `SimCore.Time` | ✅ `MasterClock`, `SyncedClock`, `TimeLine`, `ClockOffsetEstimator`, `MonotonicUtcClock` | README mit Formeln |
| `SimCore.TimeSync` | ✅ `TimeSyncServer` (Master), `TimeSyncClient` (Gerät), `MessageChannel` | README mit Protokolltabelle |
| `SimulationMaster` | ✅ lauffähig (Optionen `--bind --port --start --speed --lead --heartbeat --tolerance --autostart`) | `Program.cs`, `MasterOptions.cs`, `MasterConsole.cs` |
| `Modbus.Contracts` | ✅ gemeinsamer Registerkopf 0–19, `RegisterDefinition`, `RegisterPlan` (+ Markdown), `RegisterCodec`, `ChargerRegisters`/`PvRegisters`/`BatteryRegisters`, `RegisterPlans.For(DeviceType)` | `CommonRegisters.cs`, README mit Kopftabelle |
| `Devices.Common` | ✅ Zeitanbindung: `DeviceTimeSource`, `StepScheduler`, `TimeSyncOptions`, `AsyncConsole`; ✅ Modbus: `ModbusDeviceServer`, `ModbusOptions`, `HeaderValues`; ✅ Wetter: `WeatherClient`, `WeatherOptions`, `WeatherFeed` | README mit Codemustern |
| `Shared` | ✅ geräteunabhängige Bausteine: `CsvFileWriter`, `CsvFormat`/`CsvSchema`, `BarChart`, `ConsoleText.Fit`, `Season`/`SeasonInfo`, `WeatherCondition`, `IrradiationCalculator`; von der PV genutzt, für die übrigen Geräte bereitgestellt | `Data/*`, `Output/*`, `Weather/*`, README |
| `start-all.ps1` | ✅ baut die Solution und öffnet Wetter, Master, alle vier Feldgeräte und den Diagnose-Client in eigenen Fenstern; wartet auf Ports, überspringt belegte, `-Autostart`, `-NoClient`, `-Speed`, `-WhatIf` | `start-all.ps1`, zentrale `README.md` |
| `DualPortCharger` | ✅ an den Master angebunden, async, CSV-Spalte `Lauf`, Profile „Master“ und „Standalone“; ✅ Modbus-Server mit Registerplan im README; ✅ optional Wetter (Außentemperatur) | `Simulation/SimulationEngine.cs` + `Simulation/ChargerModbus.cs` = **Vorlage für alle Geräte** |
| `ModbusDiagnosticClient` | ✅ Eingabeaufforderung + `--once`/`--write`; Kopf und Geräteregister lesen (Rohwert + Deutung), Sollwert schreiben und beim Gerät prüfen (Register 18/19), `force`, `watch`, Verbindungsabbruch mit Wiederverbindung | `DiagnosticSession.cs`, README |
| Tests | ✅ `SimCore.Tests` 54/54, `Devices.Common.Tests` 51/51, `ModbusDiagnosticClient.Tests` 28/28, `FieldDevices.Tests` 62/62 (PV, Batterie, Kühlhaus), `Weather.Tests` 11/11 – zusammen 206 | – |
| `PvSimulator` | ✅ an Master-Zeit (`StepScheduler`, `--model-step`), Modbus (Registerplan 20–38) und Wettersimulator (`--weather`, `--location`, Abfrage je Modellstunde) angebunden, async, CSV je Lauf; Tasten `Q`/`C`/`R`/`F`/`T`/`G` immer, `P`/`M`/`0–4`/`5–9` nur standalone (mit Master ignoriert und im Footer ausgeblendet, Sitzungsprotokoll §0.13 im Archiv); Statuszeilen umbrochen, Adresse/Standort farbig; nutzt `WeatherClient` aus `Devices.Common` und die gemeinsamen Bausteine aus `Shared` | `Simulation/SimulationEngine.cs`, `PvModbus.cs`, `Output/ConsoleDisplay.cs` |
| `BatterySimulator` | ✅ an Master-Zeit, Modbus (Registerplan 20–43, Sollwert in konfigurierter Leistung) und optional Wetter (Außentemperatur) angebunden, async, Einstellungsmenü `S` statt Pausenmenü | `Simulator.cs`, `BatteryModbus.cs` |
| `WeatherSimulator` | ✅ HTTP-Dienst (`/weather`, `/locations`), deterministisch aus WeatherSynth + eigenem Modell, 3 Standorte, manuelle Übersteuerung, Fehler 404/400 | `Program.cs`, `Simulation/WeatherModel.cs`, README |
| `Devices/KuehlhausSimulator` (Kühlhaus) | ✅ **fertig (Schritte 1–9, 2026-10-26)**: in der Solution, Modell mit exakter Exponentiallösung (`SetDoor(bool)`, `ResetFault()`, `AlarmTempC`), `DeviceType.IndustrialConsumer = 5`, `KuehlhausRegisters` (20–31), `SimulationConfig`/`SimulationEngine` (Master-Zeit, feste Schritte, CSV je Lauf, Wetter je Modellstunde über `Devices.Common` (§0.14 im Archiv), Modbus auch in der Pause), `KuehlhausModbus`, Automatik (`AutoModeController`), Anzeige/Tasten (`ConsoleRenderer`, `InputHandler`), 28 Tests, Geräte-README mit Registerplan | `Simulation/SimulationEngine.cs`, `Simulation/KuehlhausModbus.cs`, `IO/ConsoleRenderer.cs`, `IO/InputHandler.cs`, `README.md`, Anleitung `docs/devices/KuehlhausSimulator/umsetzung-lb02.md` |

### 0.4 Noch offen (Abgabe `lb02-2026`)
1. **Batterie-CSV auf de-AT umstellen** (schreibt weiter Punkt = Excel-Datum-Falle) und `ConsoleText.Fit` in
   Charger/Batterie/Kühlhaus übernehmen – Helfer stehen in `src/Shared` bereit, vorher im Team abstimmen.
2. **Im Team zu klären:** Zuständigkeiten (B/C in §3), Drift-Toleranz (Vorschlag 10 ms · Tempo), ob ein
   Neustart im Master den Gerätezustand zurücksetzt (Charger behält derzeit angesteckte Fahrzeuge).
3. **Wetter-Erweiterung (Idee, freiwillig):** Klimatabelle pro Standort (Sahara/Antarktis/Tropen) statt
   Österreich-kalibrierten Formeln; im Team besprochen, noch nicht umgesetzt.

### 0.5 Kurzreferenz Zeit-API (Details in den READMEs)
- **Master:** `new MasterClock(timeProvider, startSimTime, speed = 1, leadTime = null)` (Vorlauf standardmäßig 200 ms) startet pausiert. `Resume()`, `Pause()`, `SetSpeed(x)` (1–100 000),
  `Restart(newSimTime, startPaused)` (neue `RunId`). Dazu `Current`, `CurrentStates`, Event `StateChanged`.
  `TimeSyncServer` verteilt die Zustände.
- **Gerät:** `DeviceTimeSource.Create(config.TimeSync, deviceId, deviceKind)` liefert `RunAsync(ct)`, `HasTime`, `Clock` (`ISimClock`: `Now`, `Mode`,
  `Speed`, `RunId`), `StatusText`, `Log`. `TogglePause`/`ChangeSpeed` wirken nur standalone.
  `new StepScheduler(step).Update(clock.Now, clock.RunId)` liefert `StepUpdate(Kind, FirstStepAt, StepCount, SkippedSteps, ModelTime)`.
- **Protokoll:** TCP, JSON-Zeilen, Port 5100, Version 1: `hello` → `states` (alle gültigen) · `ping`/`pong` · `bye`.
- **Tests:** reale Zeit über `ManualTimeProvider` (`tests/SimCore.Tests`). Heartbeat-Timeout 3 s beachten, sonst meldet
  `SyncedClock` korrekt `MasterLost`.

### 0.5a Kurzreferenz Modbus (Details: `src/Modbus.Contracts/README.md`, `src/Devices.Common/README.md`, Geräte-READMEs)
- **Register:** Holding Registers, 0-basierte Protokolladressen. **0–19 Kopf** (`CommonRegisters`: Typ, Instanz, Zustand, Fehlercode, Leistung Float32, Energie Float32,
  Modellzeit UInt32 = Sekunden seit 1970, Lauf, Sequenz, Zeitstatus, läuft, Tempo, Ein/Aus *(schreibbar)*, Reset *(schreibbar)*, letzter Schreibfehler, dessen Adresse),
  **20–49 gerätespezifisch** (Charger 20–37, PV 20–38, Batterie 20–43). Float32/UInt32: höherwertiges Wort zuerst. Leistung: positiv = Aufnahme (PV negativ).
- **Neues Gerät:** Register in `Modbus.Contracts` (`XyzRegisters.Plan`, in `RegisterPlans.For` eintragen) → `XyzModbus` nach Muster `ChargerModbus` (`Start()` vor Konsole/CSV,
  in **jedem** Schleifendurchlauf `ProcessCommands()` und `Publish(SimulationTime, RunId)`, auch in der Pause) → `ModbusOptions.TryApply` im Argument-Parser →
  `--register-plan` → Plan aus `ToMarkdown()` ins Geräte-README → Tests wie `FieldDevices.Tests`.
- **Schreibprüfung:** `ModbusDeviceServer` prüft gegen den Plan (nur schreibbare, volle Float-Zugriffe, Bereich, NaN) und optional `ValueValidator`. Ungültig ⇒ **rückgängig**,
  Register 18 (1 nicht beschreibbar, 2 ungültiger Wert, 3 unvollständig) und 19 (Adresse), Meldung in der Konsole. Der Client bekommt keine Modbus-Ausnahme (Bibliotheksgrenze).
- **Diagnose-Client:** `dotnet run --project src/ModbusDiagnosticClient -- --port 1502` (`read`, `write <ziel> <wert>`, `force`, `watch`, `raw`, `info`, `--once`, `--write`).
  Plan wird über Register 0 (`RegisterPlans.For`) gewählt.

### 0.6 Stolperfallen beim Testen unter Windows (erlebt am 2026-10-25)
- PowerShell 5.1 `Start-Process -ArgumentList` zerteilt `"2026-01-15 06:00"`. Lösung: ein einziger String mit inneren Anführungszeichen.
- In PowerShell gibt es kein `<`. Eingabe umleiten mit `-RedirectStandardInput`.
- Temporäre Dateien gehören in den Scratchpad-Ordner der Sitzung, nicht nach `$env:TEMP`.
  Größere Python- oder Bash-Skripte als Datei ablegen statt als Heredoc.
- Bei umgeleiteter Ausgabe rendert der Charger nur alle 5 s eine Zeile. Das ist so gewollt und kein Hänger.
- **EasyModbus:** (1) Die Bibliothek bindet den Port in einem eigenen Thread; ein belegter Port beendet das ganze Programm mit einer unbehandelten Exception.
  `ModbusDeviceServer.Start()` prüft den Port deshalb vorher. (2) Sie antwortet auf einen Schreibzugriff, **bevor** die Anwendung ihn sieht: Tests und Clients müssen kurz
  warten (`WaitUntil`, Diagnose-Client 200 ms). (3) Nach `Listen()` indiziert `holdingRegisters[adresse + 1]`; das Ereignis `HoldingRegistersChanged` liefert die 1-basierte Startadresse.
  (4) `ConnectionException` ist ein `ModbusException`; zuerst prüfen. (5) Beim Start druckt die Client-Bibliothek ein Banner auf stdout.
- **Rechnen:** Ein `switch`-Ausdruck mit gemischten Typen (`ushort`, `uint`, `float`) rechnet intern mit `float`; Sekunden seit 1970 gehen dabei verloren (Fehler bis ~1 min).
  In `RegisterCodec.Decode` ist jeder Zweig `double`.
- **Dateien bearbeiten:** Die LB01-Dateien haben teils BOM und/oder CRLF; Python-Änderungen mit `newline=''` lesen und schreiben, sonst ändern sich alle Zeilenenden.
  In der Bash-Shell sind Heredocs mit vielen Sonderzeichen unzuverlässig: Skripte als Datei anlegen (Write-Werkzeug) und ausführen.
- **Berechtigungen:** Das Starten echter Konsolenfenster (`Start-Process` ohne `-NoNewWindow`) und das Beenden fremder Prozesse mit `Stop-Process`, soll zuerst angefragt werden.
  Probeläufe gehen mit `dotnet run --no-build … &` in der Bash-Shell (Ausgabe in eine Datei) und `taskkill //F //IM <Programm>.exe`; interaktive Terminals muss die Userin selbst testen.
- Ein Master-Probelauf geht ohne Gerät: Ein PowerShell-TCP-Client schickt
  `{"type":"hello","clientId":"X","clientKind":"Test","protocolVersion":1}` als Zeile.

### 0.6a Sitzungsarchive
Sitzungsprotokolle der Umbauten (Modbus/Diagnose-Client, Wettersimulator, Kühlhaus, gemeinsames Wetter,
`Shared`, `start-all.ps1`, PV-Statuszeilen, PV-CSV) sind archiviert:
`docs/development-journal/AGENTS2-archiv-2026-10-27.md` (dort §0.7–§0.15); Details auch in den
Entwicklungsjournalen unter `docs/development-journal/`.

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
└── register-plans/                (leer) – der Registerplan steht laut Aufgabenstellung im README des Geräts
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

## 3. LB02 – Architektur (Entscheidungen, fortgeschrieben bis 2026-10-26)

- **Bibliotheken (ohne Zyklen):** `SimCore.Contracts` (Interfaces + DTOs: `ISimClock` (statt des geplanten `ITimeProvider`), `IWeatherService`, `WeatherData`,
  `TimeState`, `IWeatherService`, `WeatherData`, `DeviceConfig`; **keine** Abhängigkeiten) ← `SimCore.Time` (virtuelle Zeit) ← `SimCore.TimeSync` (TCP-Zeitverteilung) ←
  `Modbus.Contracts` (Registeradressen, `RegisterMap`, `DeviceId`, `RegisterPlan`) ← `Devices.*` (+ EasyModbusTCP.NETCore).
- **Zeitverteilung:** Master = TCP-Server, Geräte = TCP-Clients; Master pusht `TimeState` bei Änderung + Heartbeat.
  Verfahren (Gerade + Vorlaufzeit + Uhrenabgleich) siehe „Entscheidung 2026-10-25“ unten.
  Nicht erreichbare Geräte werden als offline markiert, die anderen laufen weiter.
- **Wetter (umgesetzt 2026-10-26):** reine Funktion von (Ort, Stunde, Lauf), HTTP `GET /weather` auf Port 8080.
  WeatherSynth 0.3.0 liefert stündliche Strahlung und Wind (offline), Temperatur/Bedeckung/Niederschlag/Sonnenhöhe/Zustand
  sind eigenes Modell. Seed stabil aus Standort, Jahr und Lauf (FNV-1a). Antwort `WeatherData` (inkl. `PrecipitationMmPerHour`).
  Standorte `salzburg` (Standard), `wien`, `innsbruck`; PV, Batterie und Charger fragen je Modellstunde über den gemeinsamen
  `WeatherFeed` (`Devices.Common`), Fehler nach 3 Fehlversuchen sichtbar. Basis: Alex' `Season` + `IrradiationCalculator`, erweitert um Wind.
- **Async:** `async Task StartAsync(CancellationToken ct)` mit `await Task.Delay(…, ct)`; Tastatur über eine async-Task.
- **Modbus (umgesetzt 2026-10-25):** Jedes Geräte-Programm hostet einen eigenen `EasyModbus.ModbusServer` (Ports ab 1502, `--modbus-port`).
  Holding Registers: **0–19 gemeinsamer Kopf** (Typ, Instanz, Zustand, Fehlercode, Leistung, Energie, Modellzeit, Lauf, Sequenz, Zeitstatus,
  läuft, Tempo, Ein/Aus, Reset, letzter Schreibfehler/-adresse), **20–49 gerätespezifisch** inkl. Sollwert (ursprünglich 0–9/10–29 geplant, zu eng).
  `double` → 2 Register, IEEE-754 float, höherwertiges Wort zuerst, `UInt32` für die Modellzeit (Sekunden seit 1970). Leistung: positiv = Aufnahme.
  Der Server bleibt während der Pause und bei `MasterLost` erreichbar. Schreibprüfung gegen den `RegisterPlan`; ungültig ⇒ zurückgesetzt und
  Fehler in Register 18/19. Details: `src/Modbus.Contracts/README.md`, `src/Devices.Common/README.md`, Registerplan im Geräte-README.
- **Zuständigkeiten:** siehe README der jeweiligen Projekte.

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
- Starten: opzional ein Skript `start-all.ps1` bzw. „Mehrere Startprojekte“ in VS/Rider.

### Angelegte Struktur (siehe §0.3)

```
EnergyTwin.sln                     Solution (Ordner src, src/Devices, tests)
Directory.Packages.props           zentrale NuGet-Versionen (EasyModbusTCP.NETCore 1.0.0, xUnit 2.9.3, Test.Sdk 17.14.1)
src/Directory.Build.props          net10.0, Nullable, ImplicitUsings, TreatWarningsAsErrors=true
                                   (gilt jetzt auch für den verschobenen LB01-Code → Warnungen brechen den Build)
tests/Directory.Build.props        importiert src/…, IsPackable=false
src/
  SimCore.Contracts/       lib   keine Abhängigkeiten
  SimCore.Time/            lib   → Contracts                (Zeitlogik, kein Netzwerk)
  SimCore.TimeSync/        lib   → Contracts, Time          (TCP: TimeSyncServer/TimeSyncClient, seit 2026-10-25)
  Modbus.Contracts/        lib   → Contracts                (Registerkopf, RegisterPlan, RegisterCodec; seit 2026-10-25)
  Shared.Config/           lib   → Contracts                (ursprünglich geplant: Shared/DeviceConfig); enthält OutputPaths
  Shared/                  lib   keine Abhängigkeiten        (CSV-Writer, CsvFormat/CsvSchema, Balken, Fit, Jahreszeiten/Wetter/Einstrahlung; seit 2026-10-26)
  Devices.Common/          lib   → Contracts, Time, TimeSync, Modbus.Contracts, Shared.Config + EasyModbusTCP.NETCore (Zeit + Modbus-Server)
  SimulationMaster/        exe   → Contracts, Time, TimeSync, Shared.Config   (lauffähig seit 2026-10-25)
  WeatherSimulator/        exe   → Contracts, Shared.Config, Devices.Common + WeatherSynth (HTTP-Wetterdienst)
  ModbusDiagnosticClient/  exe   → Contracts, Modbus.Contracts, Shared.Config + EasyModbusTCP.NETCore   (lauffähig seit 2026-10-25)
  Devices/PvSimulator/        exe → Devices.Common, Shared   enthält LB01-Code von Alex (seit 2026-10-25 mit Master-Zeit und Modbus)
  Devices/BatterySimulator/   exe → Devices.Common   enthält LB01-Code von Domi (seit 2026-10-25 mit Master-Zeit und Modbus)
  Devices/DualPortCharger/    exe → Devices.Common   enthält LB01-Code von Susi
  Devices/KuehlhausSimulator/ exe → Devices.Common   Kühlhaus (4. Feldgerät), Stand 2026-10-26: Modell, Registerplan, Engine, Modbus, Wetter, Anzeige/Tasten
tests/
  SimCore.Tests/           xUnit → SimCore.Time, SimCore.TimeSync
  Devices.Common.Tests/    xUnit → Devices.Common          (seit 2026-10-25)
  ModbusDiagnosticClient.Tests/ xUnit → ModbusDiagnosticClient, Devices.Common   (seit 2026-10-25)
  FieldDevices.Tests/      xUnit → PvSimulator, BatterySimulator, Devices.Common         (seit 2026-10-25)
  Weather.Tests/           xUnit → WeatherSimulator (11 Tests, Modell direkt)
docs/                      zentrale Doku und CSV-Ausgaben (Aufbau siehe §1)
```
Jedes Projekt hat eine `README.md` als Gerüst (Zweck, Verantwortlich, Basis aus LB01, Konfiguration, Bedienung,
Schnittstellen, Tests, Bekannte Probleme).Es existiert eine zentrale `README.md` im root Folder.

### Entscheidung 2026-10-25: Zeitsynchronisation über Gerade, Vorlaufzeit und Uhrenabgleich

Umgesetzt in `SimCore.Contracts` (`TimeState`, `SimMode`, `ISimClock`) und `SimCore.Time` (`MasterClock`, `SyncedClock`,
`TimeLine`, `ClockOffsetEstimator`, `MonotonicUtcClock`), getestet in `tests/SimCore.Tests`. Details: `src/SimCore.Time/README.md`.

- **Problem:** Eine mitgeschickte Uhrzeit ist beim Empfänger um die Laufzeit `d` alt, der Fehler ist `d · Tempo`
  und bei jedem Gerät anders.
- **`TimeState` = Gerade:** `{MasterSession, RunId, Sequence, Mode, Speed, SimAnchor, EffectiveAt}`.
  Ab Master-Uhrzeit `EffectiveAt` gilt `Sim = SimAnchor + (Master − EffectiveAt) · Speed` (pausiert: `SimAnchor`).
  Jedes Gerät rechnet selbst (`TimeState.SimTimeAt`), die Laufzeit der Nachricht spielt keine Rolle.
- **Vorlaufzeit 200 ms:** Jede Änderung gilt erst ab *jetzt + 200 ms*, `SimAnchor` = Wert der alten Geraden dort.
  Alle schalten gleichzeitig und ohne Sprung um. Verspätete Nachrichten werden übernommen, der Sprung wird als
  `LastLateness` gemeldet. Einziger gewollter Sprung: `Restart` (neue `RunId`).
- **Uhrenabgleich (Cristian):** Ping/Pong, `Offset = Master − (t0 + RTT/2)`, Fehler ≤ RTT/2, beste von 8 Messungen.
  Alle Uhren sind monoton (Systemuhr-Umstellung ohne Wirkung). Reale Uhr immer über `System.TimeProvider` (testbar).
- **Master startet pausiert**, `Resume()` startet. Tempo 1 … 100 000, ungültige Werte ⇒ `ArgumentOutOfRangeException`.
- **Neue Geräte** bekommen `MasterClock.CurrentStates` (inkl. angekündigter Änderungen).
- **Master nicht erreichbar:** nach 3 s ohne Nachricht `SyncStatus.MasterLost`, Gerätezeit steht (`Mode = Paused`),
  Modbus bleibt lesbar. Beim nächsten Heartbeat springt das Gerät sichtbar auf die Master-Zeit.
  Neu gestarteter Master (neue `MasterSession`) ersetzt den Verlauf.
- **Toleranz (Vorschlag):** 10 ms reale Zeit ⇒ zulässige Sim-Abweichung = 10 ms · Tempo; Nachweis über
  `SyncedClock.SimUncertainty` (= RTT/2 · Tempo) in der Geräteanzeige.
- **Transport (umgesetzt 2026-10-25, `SimCore.TimeSync`):** TCP, eine JSON-Nachricht pro Zeile, Port 5100.
  `hello` (Sim→Master) · `states` (Master→Sim: **alle** ab jetzt gültigen Zustände; nach `hello`, bei Änderung, Heartbeat 1 s) ·
  `ping`/`pong` (Uhrenabgleich, 5× schnell nach dem Verbinden, dann 1 s; `ping` meldet auch Sim-Zeit und Unsicherheit) ·
  `bye` (geordnetes Beenden/Ablehnung). Protokollversion 1, ungültige Nachrichten trennen nur die betroffene Verbindung.
  Simulator: `TimeSyncClient` verbindet sich alle 2 s neu. Master: listet Simulatoren mit gemessener Abweichung Δ, ✓/✗ gegen Toleranz,
  „KEINE ANTWORT“ nach 3 s, „GETRENNT“ bei Verbindungsende. Details: `src/SimCore.TimeSync/README.md`.
- **`SimulationMaster` (2026-10-25):** startet pausiert; Leertaste/P Pause↔Fortsetzen, `+`/`-` Tempo ×2/÷2, `T` Tempo eingeben,
  `N` Neustart mit Zeit, `1–4` Neustart Frühling/Sommer/Herbst/Winter, `Q`/Strg+C beenden (`bye` an alle).
  Ein eigenes `TimeControlCmd` ist damit nicht nötig (Bedienung nur im Master). Details: `src/SimulationMaster/README.md`.
- **Geräte (2026-10-25, `Devices.Common`):** `DeviceTimeSource` (mit Master: `SyncedClock` + `TimeSyncClient`; `--standalone`:
  lokale `MasterClock`), `StepScheduler` (fragt die Uhr und rechnet die fälligen **festen** Schritte; nie selbst hochzählen,
  nie `Schritt/Tempo` warten; Neustart = neue `RunId` ⇒ Modellzeit springt, Zustand bleibt), `TimeSyncOptions`
  (`--master host:port`, `--standalone`, `--start`, `--speed`; die letzten beiden nur standalone), `AsyncConsole`
  (Eingabe ohne `ReadLine`). Pause/Tempo-Tasten der Geräte wirken nur standalone. Bei `MasterLost` rechnet das Gerät nicht weiter.
  Muster: `src/Devices/DualPortCharger/Simulation/SimulationEngine.cs`, Anleitung: `src/Devices.Common/README.md`.

### Offene Fragen (müssen im Team entschieden werden)

| # | Frage | Stand / Tendenz |
|---|---|---|
| 1 | Transport Wettersimulator | ✅ umgesetzt 2026-10-26: HTTP, JSON, Port 8080 (`WeatherSimulator`) |
| 2 | Transport Zeitverteilung | ✅ umgesetzt 2026-10-25: TCP, JSON-Zeilen, Port 5100 (`SimCore.TimeSync`) |
| 3 | Drift-Toleranz | ~~250 ms Sim-Zeit~~ → Vorschlag 2026-10-25: 10 ms Echtzeit · Tempo (bei Tempo 600 sind 250 ms Sim-Zeit < 0,5 ms Echtzeit, nicht einhaltbar) |
| 4 | Registeradressen je Gerät | ✅ umgesetzt 2026-10-25: 0–19 Kopf, 20–49 gerätespezifisch; Charger, PV, Batterie (Alexander/Dominik informieren) |
| 5 | Wie finden Geräte den Master | Konfigurationsdatei (IP:Port); Standard `127.0.0.1:5100` (`SyncProtocol.DefaultPort`) |
| 6 | Ort fürs Wetter | ✅ umgesetzt 2026-10-26: Standortkatalog, Standard `salzburg` (auch wien, innsbruck) |
| 7 | Komplexität des Batteriemodells | einfaches SOC-Modell |
| 8 | Was lädt der Charger | „Batterie aus PV-Überschuss“ |

---

## 4. Wandern aus LB01 (abgeschlossen)

Die drei LB01-Geräte sind vollständig in die LB02-Projekte integriert (Wander- und passtabelle im
Archiv: `docs/development-journal/AGENTS2-archiv-2026-10-27.md`, dort §4); gemeinsam nutzbare Hilfen
liegen in `src/Shared` und `Devices.Common`.

---

## 5. Die drei LB01-Geräte im Detail

Nur noch historisch – **Quelle der Wahrheit für Modell, Formeln und Bedienung ist je Gerät dessen `README.md`**:
PV → `src/Devices/PvSimulator/README.md`, Batterie → `src/Devices/BatterySimulator/README.md`,
Charger → `src/Devices/DualPortCharger/README.md` (LB01-Startbefehle und Dateilisten im Archiv,
`AGENTS2-archiv-2026-10-27.md`, dort §5). Was bleibt: die Unterschiede-Tabelle, weil gemeinsame
Entscheidungen (Zahlenformat, Tasten, Modus-Begriffe) darauf aufbauen:

### Unterschiede, die noch zählen

| Thema | Alex (PV) | Domi (Batterie) | Susi (Charger) |
|---|---|---|---|
| Einheiten/Casing | `PowerKw`, `Kwh` | `ActualKW`, `KWh` | `PowerKW`, `KWh` |
| CSV-Zahlen | Komma (de-AT) | Punkt (Invariant; Umstellung offen, §0.4) | Komma (de-AT), BOM |

(Die übrigen LB01-Unterschiede – Zeitschritt, Warten, Modus, Störung, Temperatur, Zufall, Tasten – sind in
LB02 überholt; vollständige Tabelle im Archiv, `AGENTS2-archiv-2026-10-27.md`, dort §5.)

---

## 6. Build-Status (zuletzt 2026-10-27, .NET SDK 10)

**Befehle:**
```
dotnet build EnergyTwin.sln                                     # alles bauen
dotnet test tests/SimCore.Tests                                 # Zeit-Tests (inkl. TCP auf localhost)
dotnet run --project src/SimulationMaster -- --speed 600        # 1. Master (Port 5100, startet pausiert)
dotnet test tests/Devices.Common.Tests                          # Geräte-Zeitanbindung + Modbus-Server
dotnet run --project src/Devices/DualPortCharger -- --id DC-01  # 2. Charger (Master 127.0.0.1:5100, Modbus 127.0.0.1:1502)
dotnet run --project src/Devices/DualPortCharger -- --id DC-02 --modbus-port 1503   # zweite Säule
dotnet run --project src/Devices/DualPortCharger -- --register-plan   # Registerplan als Markdown
dotnet run --project src/ModbusDiagnosticClient -- --port 1502          # 3. Diagnose-Client (read, write Leistungsgrenze 200, force 20 999, watch)
dotnet test tests/ModbusDiagnosticClient.Tests                  # Diagnose-Client gegen echten Modbus-Server
dotnet run --project src/Devices/DualPortCharger -- --standalone --speed 300 --duration 24   # ohne Master
dotnet run --project src/Devices/PvSimulator -- --id PV-01     # PV (Master, Modbus 127.0.0.1:1502; --modbus-port für weitere)
dotnet run --project src/WeatherSimulator                         # Wettersimulator (HTTP 8080)
dotnet run --project src/Devices/BatterySimulator -- --id BAT-01 --modbus-port 1503   # Batterie (Modbus 1503)
dotnet test tests/FieldDevices.Tests                            # PV, Batterie, Kühlhaus: Modbus, Optionen, Standalone-Lauf
dotnet test tests/Weather.Tests                                 # Wetter: Reproduzierbarkeit und Plausibilität
dotnet run --project src/Devices/KuehlhausSimulator -- --id KH-01     # Kühlhaus (Master, Modbus 127.0.0.1:1505, Wetter 8080)
dotnet run --project src/Devices/KuehlhausSimulator -- --register-plan       # Registerplan des Kühlhauses als Markdown
dotnet run --project src/Devices/KuehlhausSimulator -- --standalone --speed 20000 --duration 24 --weather off --seed 42   # ohne Master
dotnet run --project src/ModbusDiagnosticClient -- --port 1505 --once                # Kühlhaus-Register lesen
dotnet run --project src/ModbusDiagnosticClient -- --port 1505 --write "Solltemperatur=-15"
```

`dotnet build EnergyTwin.sln` ⇒ 0 Fehler / 0 Warnungen (zuletzt 2026-10-27 geprüft);
alle fünf Testprojekte grün, zusammen 206/206 (54 + 51 + 28 + 62 + 11, Stand 2026-10-26/27).
Historische Probelauf-Details stehen im Archiv (`AGENTS2-archiv-2026-10-27.md`).

Die Probeläufe liefen mit umgeleiteter Ausgabe. Die Konsolenausgabe von Alex formatiert Zahlen mit Komma, die CSV ebenfalls mit Komma (seit 2026-10-27, Punkt-Werte wurden in Excel als Datum gelesen).

**Testläufe:** `dotnet test EnergyTwin.sln` startet alle Testprojekte **parallel**; mehrere davon binden echte TCP-Ports
(Modbus-Server, Zeitverteilung). Dabei können 1–2 Tests scheitern, obwohl alles in Ordnung ist. Deshalb die Projekte
einzeln starten (`dotnet test tests/<Projekt>`) – so waren am 2026-10-26 alle 206 Tests grün
(54 + 51 + 62 + 11 + 28).

## 7. Bekannte Probleme

- **`.gitignore`:** Die Regel `output/` wurde in Commit `0667108` entfernt (damit Alex' `Output/` versioniert wird).
  Stattdessen ignoriert sie jetzt `*.csv` (committet in `676502f`). Damit werden neue CSV-Läufe in `docs/data/` nicht versioniert.
  Die beiden Beispiel-CSVs (`docs/data/pv-simulation.csv`, `docs/data/battery_BAT-01_2026-10-07.csv`) sind bereits
  versioniert und bleiben es. Änderungen daran (z. B. durch einen PV-Lauf) erscheinen in `git status`.
- ~~Alex `CsvExporter` schrieb die komplette Datei neu~~ und ~~die Meldung „kWh Vortag“ war falsch~~: behoben 2026-10-25 (CSV angehängt, async; Meldung zeigt die Tagesenergie).
  `InverterStatus.cs` ist toter Code.
- Alex' Auto-Wetter ist lokal (mit `--seed` wiederholbar); mit Master fragt die PV den Wettersimulator, standalone bleibt das lokale Wetter.
- Domi: `BatterieConfig.Efficiency` wird nicht benutzt. Das Pausenmenü ist durch das Einstellungsmenü `S` ersetzt.

## 8. Konventionen und Stolperfallen

- `net10.0`, `Nullable` + `ImplicitUsings` an, **0 Warnungen**. Bezeichner englisch (Ausnahme: Domi `Batterie*`),
  Texte deutsch.
- **Trennung Modell ↔ Rahmen:** Die Modellklassen (`Batterie.cs`, `DualCharger.cs`, `PvSystem.cs`/`PvFaultModel.cs`,
  `Kuehlhaus.cs`) enthalten **nur die gerätespezifische Logik** – die Physik und das Verhalten, die dieses Gerät
  ausmachen. Alles, was geräteübergreifend möglich ist, wird ausgelagert und vereinheitlicht: gemeinsame Bausteine
  nach `src/Shared` (CSV-Grundwriter, Zahlenformat, Grundspalten, Konsolen-Helfer, Jahreszeiten/Wetter), Bibliothekslogik
  (Zeit, Modbus, Wetterbezug) nach `Devices.Common`. virtuelle Zeit, Wetter und Ein-/Ausgabe kommen den Modellen als
  Parameter von außen: Konsole, Rendering, `DateTime.Now`/`UtcNow` und Retry-Logik gehören in `Program.cs`, `IO/*`,
  `*Modbus` und die `SimulationEngine`s; die virtuelle Zeit lesen diese über `ISimClock`/`StepScheduler` (§0.5),
  nie die Wanduhr. **Faustregel beim Neuen:** „Gibt es das schon einmal? Dann in `Shared`/`Devices.Common` –
  im Modell bleibt nur das, was nur dieses Gerät kann."
- Umgeleitete Konsole ≠ echtes Terminal: Cursor- und Puffer-Funktionen brauchen einen Fallback (Alex und Susi haben einen).
  Immer auch in einem echten Terminalfenster testen; ein NullRef beim ersten Diff-Frame endet mit Exit-Code -1073741819.
- Reproduzierbarkeit: `Random` aus Datum/Ort/Seed erzeugen, nie aus der Uhrzeit.
- Virtuelle Zeit nur über `ISimClock` lesen (Geräte: `SyncedClock`), nie lokal hochzählen. Die reale Uhr immer als
  `System.TimeProvider` übergeben (nicht `DateTime.Now`/`Stopwatch` direkt), damit Tests mit einer Test-Uhr laufen.
- Umgebungsvariablen nur für optionale Entwickler-Features (Muster: `ENERGYSIM_RENDER_LOG`).
- Bei Modelländerungen die Formeln in der jeweiligen `README.md` mitpflegen. Bei Architekturentscheidungen
  §3 dieser Datei aktualisieren und einen Eintrag im Entwicklungsjournal machen.
