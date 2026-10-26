# AGENTS.md – dotnet-energy-twin-alpha

**Stand:** 2026-10-26 · **Phase:** LB01 abgeschlossen (3 Einzelsimulatoren), LB02-Projektstruktur angelegt (`EnergyTwin.sln`, `src/`, `tests/`),
Zeitlogik + TCP-Zeitverteilung umgesetzt (`SimCore.*`, 54 Tests), `SimulationMaster` lauffähig, Zeitanbindung der Geräte und
Modbus-Server-Baustein in `Devices.Common` (51 Tests), **Charger, PV, Batterie und Kühlhaus an Master und Modbus angebunden**
(`FieldDevices.Tests` 62), **Diagnose-Client** (28 Tests), **Wettersimulator mit gemeinsamem Wetterbezug von PV, Batterie und Charger**
(`Weather.Tests` 11); **viertes Feldgerät Kühlhaus** (`Devices/KuehlhausSimulator`) ist als LB02-Feldgerät **fertig** (Modell, Registerplan, Engine,
Modbus, Wetter, Anzeige/Tasten, 28 Tests, README) – damit ist
„+ ein viertes Feldgerät“ erfüllt, Wind wird nicht umgesetzt (Platzhalterprojekt am 2026-10-26 gelöscht); zusammen 206 Tests

Pflichtlektüre für jeden Agenten in diesem Repo. Sie fasst die drei LB01-Projekte und den LB02-Plan
zusammen, damit nicht alles neu analysiert werden muss. Details:
- Modell und Bedienung je Gerät → jeweilige `README.md` (Quelle der Wahrheit für Formeln).
- Historisches LB02-Planungsdokument von Alex (Stand 2026-10-10, englisch) →
  `docs/development-journal/AGENTS1-archiv-2026-10-10.md`. Es ist **obsolet** (nur Archiv); sein Inhalt ist
  vollständig in diese Datei übergegangen und mit dem tatsächlichen Code abgeglichen.

---

## 0. Übergabe – hier weitermachen (Stand 2026-10-26, Susanne Lastowicka)

Dieser Abschnitt reicht, um an einem neuen Tag ohne Vorwissen weiterzuarbeiten. §1–§8 enthalten die Details.

### 0.1 Arbeitsregeln für Agenten (von Alexander festgelegt, immer einhalten)
- **Entwicklungsjournal:** Jede Änderung wird automatisch in `docs/development-journal/` unter der jeweiligen Person eingetragen.
  Pro Tag gibt es einen Eintrag `## JJJJ-MM-TT – Titel`, weitere Arbeiten am selben Tag kommen als `### Nachtrag JJJJ-MM-TT – Titel` dazu.
  Felder: `**Done:**`, `**KI:**` (z. B. „Claude Opus 5.5 => …“), `**Artefact:**`, `**Test:**`, `**Comment:**`.
  **Nur diese Datei bearbeiten**, nie die Journale der anderen Personen. Maßgeblich ist das Journal der Person, in deren
  Arbeitsstand gerade gearbeitet wird (am 2026-10-26 war das Dominiks Gerät: Eintrag in `dominik-Zach.md`).
- **Commits nur auf Anweisung.** Die Commit-Message der Userin wird **wörtlich** übernommen, **ohne** `Co-Authored-By`-Zeile.
  Nicht pushen, außer es wird ausdrücklich verlangt.
- Die PDF der Aufgabenstellung (`dotNET Programmierung 2026 - Aufgabe 02.pdf`) liegt nur lokal im Repo-Root und ist in `.gitignore`.
  Für Anforderungen genügt §2.
- Antworten, Doku, Kommentare und UI auf **Deutsch**.
- Nach jeder Änderung `dotnet build EnergyTwin.sln` (0 Warnungen) und die betroffenen Tests ausführen. Dann READMEs, diese Datei
  (Stand-Kopf, §0, §3, §6) und das Journal nachziehen.

### 0.2 Git-Stand
`main`, lokal, **nicht gepusht**. Commits: `81e78a7` „Modbus, Diagnose-Client, PV und Batterie an Master“, `70711c8` „Aufgabenstellung-PDF in .gitignore“,
`676502f` „Master Clock und Charger Zeit von Sim Master“.
`SimCore.Contracts/WeatherData.cs`, das neue NuGet-Paket WeatherSynth, `Weather.Tests`, die Wind-Platzhalterdatei, geänderte READMEs, `AGENTS.md` und Journal.
Vor dem Commit `dotnet build EnergyTwin.sln` und alle fünf Testprojekte laufen lassen (Commit nur auf Anweisung, Message wörtlich, ohne Co-Author).

### 0.3 Was fertig ist

| Baustein | Stand | Wichtigste Dateien |
|---|---|---|
| `SimCore.Contracts` | ✅ `TimeState`, `SimMode`, `ISimClock`, `IWeatherService`, `WeatherData`, Sync-Nachrichten und Protokoll | `TimeState.cs`, `SyncMessages.cs`, `SyncProtocol.cs`, `IWeatherService.cs`, `WeatherData.cs` |
| `SimCore.Time` | ✅ `MasterClock`, `SyncedClock`, `TimeLine`, `ClockOffsetEstimator`, `MonotonicUtcClock` | README mit Formeln |
| `SimCore.TimeSync` | ✅ `TimeSyncServer` (Master), `TimeSyncClient` (Gerät), `MessageChannel` | README mit Protokolltabelle |
| `SimulationMaster` | ✅ lauffähig (Optionen `--bind --port --start --speed --lead --heartbeat --tolerance --autostart`) | `Program.cs`, `MasterOptions.cs`, `MasterConsole.cs` |
| `Modbus.Contracts` | ✅ gemeinsamer Registerkopf 0–19, `RegisterDefinition`, `RegisterPlan` (+ Markdown), `RegisterCodec`, `ChargerRegisters`/`PvRegisters`/`BatteryRegisters`, `RegisterPlans.For(DeviceType)` | `CommonRegisters.cs`, README mit Kopftabelle |
| `Devices.Common` | ✅ Zeitanbindung: `DeviceTimeSource`, `StepScheduler`, `TimeSyncOptions`, `AsyncConsole`; ✅ Modbus: `ModbusDeviceServer`, `ModbusOptions`, `HeaderValues`; ✅ Wetter: `WeatherClient`, `WeatherOptions`, `WeatherFeed` | README mit Codemustern |
| `Shared` | ✅ geräteunabhängige Bausteine: `CsvFileWriter`, `BarChart`, `Season`/`SeasonInfo`, `WeatherCondition`, `IrradiationCalculator` | `Data/CsvFileWriter.cs`, `Output/BarChart.cs`, `Weather/*`, README |
| `start-all.ps1` | ✅ baut die Solution und öffnet Wetter, Master, alle vier Feldgeräte und den Diagnose-Client in eigenen Fenstern; wartet auf Ports, überspringt belegte, `-Autostart`, `-NoClient`, `-Speed`, `-WhatIf` | `start-all.ps1`, zentrale `README.md` |
| `DualPortCharger` | ✅ an den Master angebunden, async, CSV-Spalte `Lauf`, Profile „Master“ und „Standalone“; ✅ Modbus-Server mit Registerplan im README; ✅ optional Wetter (Außentemperatur) | `Simulation/SimulationEngine.cs` + `Simulation/ChargerModbus.cs` = **Vorlage für alle Geräte** |
| `ModbusDiagnosticClient` | ✅ Eingabeaufforderung + `--once`/`--write`; Kopf und Geräteregister lesen (Rohwert + Deutung), Sollwert schreiben und beim Gerät prüfen (Register 18/19), `force`, `watch`, Verbindungsabbruch mit Wiederverbindung | `DiagnosticSession.cs`, README |
| Tests | ✅ `SimCore.Tests` 54/54, `Devices.Common.Tests` 51/51, `ModbusDiagnosticClient.Tests` 28/28, `FieldDevices.Tests` 62/62 (PV, Batterie, Kühlhaus), `Weather.Tests` 11/11 – zusammen 206 | – |
| `PvSimulator` | ✅ an Master-Zeit (`StepScheduler`, `--model-step`), Modbus (Registerplan 20–38) und Wettersimulator (`--weather`, `--location`, Abfrage je Modellstunde) angebunden, async, CSV je Lauf; Tasten `Q`/`C`/`R`/`F`/`T`/`G` immer, `P`/`M`/`0–4`/`5–9` nur standalone (mit Master ignoriert und im Footer ausgeblendet, siehe §0.13); Statuszeilen umbrochen, Adresse/Standort farbig; nutzt `WeatherClient` aus `Devices.Common` und die gemeinsamen Bausteine aus `Shared` | `Simulation/SimulationEngine.cs`, `PvModbus.cs`, `Output/ConsoleDisplay.cs` |
| `BatterySimulator` | ✅ an Master-Zeit, Modbus (Registerplan 20–43, Sollwert in konfigurierter Leistung) und optional Wetter (Außentemperatur) angebunden, async, Einstellungsmenü `S` statt Pausenmenü | `Simulator.cs`, `BatteryModbus.cs` |
| `WeatherSimulator` | ✅ HTTP-Dienst (`/weather`, `/locations`), deterministisch aus WeatherSynth + eigenem Modell, 3 Standorte, manuelle Übersteuerung, Fehler 404/400 | `Program.cs`, `Simulation/WeatherModel.cs`, README |
| `Devices/KuehlhausSimulator` (Kühlhaus) | ✅ **fertig (Schritte 1–9, 2026-10-26)**: in der Solution, Modell mit exakter Exponentiallösung (`SetDoor(bool)`, `ResetFault()`, `AlarmTempC`), `DeviceType.IndustrialConsumer = 5`, `KuehlhausRegisters` (20–31), `SimulationConfig`/`SimulationEngine` (Master-Zeit, feste Schritte, CSV je Lauf, Wetter je Modellstunde über `Devices.Common` (§0.14), Modbus auch in der Pause), `KuehlhausModbus`, Automatik (`AutoModeController`), Anzeige/Tasten (`ConsoleRenderer`, `InputHandler`), 28 Tests, Geräte-README mit Registerplan | `Simulation/SimulationEngine.cs`, `Simulation/KuehlhausModbus.cs`, `IO/ConsoleRenderer.cs`, `IO/InputHandler.cs`, `README.md`, Anleitung `docs/devices/KuehlhausSimulator/umsetzung-lb02.md` |
| `WindSimulator` | ❌ entfällt: Platzhalterprojekt am 2026-10-26 gelöscht (Ordner, Solution-Eintrag); `DeviceType.Wind = 4` bleibt im Enum reserviert | – |

### 0.4 Nächste Schritte (Vorschlag, Reihenfolge)
1. **Master und Charger in einem echten Terminal testen** (Anzeige, Tasten, Strg+C). Bisher liefen nur Probeläufe mit umgeleiteter Ausgabe.
   Ablauf: Terminal 1 `dotnet run --project src/SimulationMaster -- --speed 600`, Terminal 2 und 3
   `dotnet run --project src/Devices/DualPortCharger -- --id DC-01` bzw. `--id DC-02`; im Master Leertaste drücken, dann `+`, `1–4`, `Q`.
2. ✅ **Registerplan und Modbus-Server** (2026-10-25): Kopf 0–19, gerätespezifisch 20–49 (Charger 20–37), `double` als 2 Register (float,
   höheres Wort zuerst), Sollwerte ungültig ⇒ **abgelehnt** (Register zurückgesetzt, Register 18/19 melden den Fehler). Der Registerplan steht
   laut Aufgabenstellung im **README des Geräts** (`--register-plan` erzeugt die Tabelle aus dem Code). Ports ab 1502, je Gerät `--modbus-port`.
   Grenze der Bibliothek: Sie übernimmt Schreibzugriffe vor der Prüfung, daher keine Modbus-Ausnahmeantwort. Zu klären: Kopf (0–19) mit Alexander
   und Dominik abstimmen, bevor sie ihre Pläne anlegen.
3. ✅ **Modbus-Diagnose-Client** (2026-10-25): Plan des Geräts über `RegisterPlans.For(DeviceType)`; für PV/Batterie/Wind den Plan dort eintragen
   (Definition wie `ChargerRegisters` in `Modbus.Contracts`), sonst zeigt der Client nur den Kopf. Bedienung: README des Clients.
4. ✅ **PV und Batterie** (2026-10-25) nach dem Charger-Muster umgestellt (Zeit, async, Modbus). Mit Alexander und Dominik abstimmen: geänderte Optionen
   (PV: `--timescale` entfällt, `--start` mit Uhrzeit, neu `--id`, `--model-step`, `--seed`; Batterie: `--duration` Standard endlos, `--start` mit Datum),
   CSV je Lauf mit neuem Namen, Einstellungsmenü `S` statt Pausenmenü, Registerpläne (Vorzeichen: PV negativ). Wetter der PV noch lokal.
   Das sind die Projekte von Alexander bzw. Dominik, deshalb vorher abstimmen.
5. ✅ **Wettersimulator und Wetteranbindung der PV** (2026-10-26): HTTP-Dienst auf 8080, deterministisch aus WeatherSynth 0.3.0 plus eigenem Modell,
   `WeatherData` in `SimCore.Contracts`, PV fragt je Modellstunde ab (`--weather`, `--location`), sichtbarer Fehler nach 3 Fehlversuchen, `Weather.Tests` 11. Details §0.8.
   Noch offen: **Wind** (Zeit + Modbus nach dem Muster von `PvModbus`/`BatteryModbus`, Plan in `Modbus.Contracts`, in `RegisterPlans.For` eintragen;
   Wetterbezug analog zu Batterie/Charger); Diagnose-Client ist fertig (siehe 3.).
6. ✅ **Zentrale `README.md`** (Architektur, Startreihenfolge, Demo, Zuständigkeiten, Schnittstellen, Tests, Konfiguration) am 2026-10-26 angelegt;
   **`.env.example` entfällt** (keine Umgebungsvariablen, keine Secrets – nur Kommandozeilenoptionen).
   ✅ **`start-all.ps1`** (2026-10-26): baut, startet Wetter und Master, wartet auf die Ports, öffnet dann alle vier Feldgeräte
   und den Diagnose-Client in eigenen Fenstern (`-Autostart`, `-NoClient`, `-Speed`, `-WhatIf`). Noch offen: Abgabe-Tag `lb02-2026`.
7. ✅ **Kühlhaus als viertes Feldgerät** (`src/Devices/KuehlhausSimulator`, 2026-10-26): alle neun Schritte umgesetzt (Solution, Modell, Registerplan,
   Engine mit Master-Zeit, CSV und Wetter, `KuehlhausModbus`, `ArgsParser`/`CsvLogger`, `ConsoleRenderer`, `InputHandler`, 28 Tests,
   Geräte-README mit Registerplan). Reihenfolge und Code-Skizzen stehen in `docs/devices/KuehlhausSimulator/umsetzung-lb02.md`.
   Damit ist das vierte Feldgerät abgedeckt; Wind bleibt optional. Die Team-Abstimmungen sind am 2026-10-26 entschieden
   (Projektname `KuehlhausSimulator`, `ZusatzwaermeKW` = 1,5 kW, Modbus-Standardport 1505).

**Hinweis Wetter:** PV, Batterie und Charger fragen mit Master standardmäßig den Wettersimulator (`http://127.0.0.1:8080`); standalone oder mit
`--weather off` arbeiten sie ohne HTTP (PV: lokales Auto-Wetter, nur mit `--seed` wiederholbar; Batterie/Charger: feste 20 °C). Wind nutzt das Wetter noch nicht.

**Im Team zu klären:** Zuständigkeiten (B/C in §3), Toleranz (Vorschlag 10 ms · Tempo, offene Frage 3),
ob ein Neustart im Master den Gerätezustand zurücksetzt (Charger behält derzeit angesteckte Fahrzeuge).

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
- **Berechtigungen:** Das Starten echter Konsolenfenster (`Start-Process` ohne `-NoNewWindow`) und das Beenden fremder Prozesse mit `Stop-Process` wurde vom System abgelehnt.
  Probeläufe gehen mit `dotnet run --no-build … &` in der Bash-Shell (Ausgabe in eine Datei) und `taskkill //F //IM <Programm>.exe`; interaktive Terminals muss die Userin selbst testen.
- Ein Master-Probelauf geht ohne Gerät: Ein PowerShell-TCP-Client schickt
  `{"type":"hello","clientId":"X","clientKind":"Test","protocolVersion":1}` als Zeile.

### 0.7 Sitzung 2026-10-25 (abends): Modbus, Diagnose-Client, PV und Batterie (Claude Sonnet 5.5)

Alles unten ist gebaut (0 Warnungen) und getestet.

| Bereich | Neu / geändert | Test |
|---|---|---|
| `Modbus.Contracts` | `CommonRegisters` (Kopf 0–19, `Labels`), `RegisterDefinition` (+ `Validate`, `LabelFor`), `RegisterPlan` (Überschneidungsprüfung, `ToMarkdown`), `RegisterCodec`, `ChargerRegisters`, `PvRegisters`, `BatteryRegisters`, `RegisterPlans.For` | `Devices.Common.Tests` (`RegisterCodecTests`), `FieldDevices.Tests` |
| `Devices.Common/Modbus` | `ModbusDeviceServer` (Start mit Portprüfung, `Publish`, `TryDequeueCommand`, `ValueValidator`, Rückgängigmachen ungültiger Zugriffe), `ModbusOptions`, `HeaderValues`, `RegisterWriter`; `DeviceTimeSource.SetSpeed` | `Devices.Common.Tests` 44 |
| `DualPortCharger` | `ChargerModbus`, Optionen `--modbus-*`, `--unit-id`, `--no-modbus`, `--register-plan`, Anzeige „Modbus“, Registerplan im README | Probelauf mit Master |
| `ModbusDiagnosticClient` | `ClientOptions`, `DeviceConnection` (Fehlerübersetzung, Wiederverbindung), `DiagnosticSession` (read/header/watch/write/force/on/off/reset/raw/info/connect), `RegisterFormatter` | `ModbusDiagnosticClient.Tests` 28 |
| `PvSimulator` | ✅ an Master-Zeit (`StepScheduler`, `--model-step`), Modbus (Registerplan 20–38) und Wettersimulator (`--weather`, `--location`, Abfrage je Modellstunde) angebunden, async, CSV je Lauf | `Simulation/SimulationEngine.cs`, `WeatherClient.cs`, `PvModbus.cs` |
| `BatterySimulator` | Zeit vom Master, async `Simulator`, `BatteryModbus`, `ArgsParser`/`SimulatorOptions`, `CsvLogger` async mit Datum und `Lauf`, Einstellungsmenü `S` statt Pausenmenü, `Batterie.Update(TimeSpan)`, `DailyProfile` mit Seed | `FieldDevices.Tests` |
| Doku | READMEs von `Modbus.Contracts`, `Devices.Common`, Charger, PV, Batterie, Diagnose-Client und drei Testprojekten; Journal (4 Nachträge) | – |

**Geänderte Bedienung:** PV `--timescale` entfällt, `-s/-d` nur noch Kürzel für `--speed/--start` (standalone), `--start` mit Uhrzeit;
Batterie `--duration` Standard endlos, `--start` mit Datum, `S` statt Pausenmenü; neue CSV-Namen.
**Entscheidungen:** Registerkopf 0–19 statt 0–9 (reichte für Pflichtfelder plus Zeit und Steuerung nicht); Registerplan im README des Geräts (verlangt die Aufgabenstellung), `--register-plan`
erzeugt ihn aus dem Code; ungültige Schreibwerte werden abgelehnt (nicht begrenzt), Fehler über Register 18/19.
**Gefundene Fehler:** Modellzeit im Client um bis zu 1 min falsch (`float`-Rundung in `RegisterCodec.Decode`, behoben, Test `Value_ModelTime_IsExactToTheSecond`);
unbehandelter Absturz bei belegtem Modbus-Port (Portprüfung vor dem Start); `ConnectionException` fälschlich als Protokollfehler gemeldet; zufällige Testausfälle durch die späte Meldung von
Schreibzugriffen (Tests warten jetzt).
**Noch offen:** echtes Terminal für Master, Charger, PV, Batterie, Client (interaktive Anzeige, Tasten, Einstellungsmenü); Wind; Wetter; zentrale `README.md`, `.env.example`, `start-all.ps1`;
Abstimmung der Registerpläne und der geänderten Optionen im Team.

---

### 0.8 Sitzung 2026-10-26: Wettersimulator und Wetteranbindung der PV (opencode/deepseek-v4.1)

Alles unten ist gebaut (0 Warnungen) und getestet, aber **nicht committet**.

| Bereich | Neu / geändert | Test |
|---|---|---|
| `SimCore.Contracts` | `WeatherData` (Standort, UTC-Zeit, Lauf, Sonnenhöhe, Bedeckung, Temperatur, Niederschlag, Wind, Strahlung, Zustand) und `WeatherCondition` | `Weather.Tests` |
| `WeatherSimulator` | Minimal-API (`/weather?location&simTime&run`, `/locations`, `/`), `--port` (8080), deterministisches Modell: WeatherSynth 0.3.0 (stündliche Strahlung/Wind) + eigene Temperatur/Bedeckung/Niederschlag/Sonnenhöhe/Zustand, Seed FNV-1a aus (Standort, Jahr, Lauf), Standorte salzburg/wien/innsbruck, 404/400, Anzeige mit letzter Abfrage, Tasten für manuelle Übersteuerung | `Weather.Tests` 11, Probelauf mit PV |
| `Devices.Common` | `AsyncConsole` wird vom Wetter-Programm mitbenutzt (keine Änderung nötig) | – |
| `PvSimulator` | `WeatherClient` (async, 5 s Timeout, deutsche Fehler), Abfrage je Modellstunde, `PvSystem.ApplyExternalWeather`, `PvFaultModel.Step(..., ambientOverrideC)`, Anzeigezeile „Wetter“, Fehler nach 3 Fehlversuchen sichtbar, letztes Wetter bleibt, Retry 10 s; Optionen `--weather <url\|off>`, `--location`; mit Master Standard-URL, standalone lokal; Wettertasten `0–4` nur lokal | `FieldDevices.Tests` 34, Probelauf |
| `tests/Weather.Tests` | 11 Tests (Reproduzierbarkeit, Stunde konstant, Standorte/Läufe verschieden, Nacht, Sommer/Winter, manuelle Übersteuerung, Plausibilität) | – |
| Doku | READMEs von `WeatherSimulator`, `Weather.Tests`, PV (Optionen/Status/Schnittstellen/Wetterabschnitt), zentrale `README.md`, `AGENTS.md`, Journal | – |

**Entscheidungen:** HTTP als Transport (ADR-0001/0007); WeatherSynth nur als Strahlungs-/Windbasis, Rest eigenes Modell (ADR-0009);
Standortkatalog mit `salzburg` als Standard, unbekannte Kennung 404 (ADR-0006/0010); Nieder­schlag als zusätzliches Feld;
Zeit als UTC in Code und Schnittstelle, Ortszeit nur in der Anzeige (ADR-0012); Fehlerverhalten 5 s Timeout, 3 Fehlversuche, Retry 10 s (ADR-0008).
Die ADRs liegen bewusst **nicht im Repo** (`docs/adr/`, `docs/glossary.md` sind ignoriert).

**Gefundene Punkte:** Der Standalone-Lauf der PV darf nicht standardmäßig den Wettersimulator fragen (er hängt sonst am ersten
Verbindungsversuch, der bestehende Test `Pv_RunsStandalone_WritesCsvAndStopsOnCancel` schlug fehl); deshalb ist die URL nur mit Master vorbelegt.
PowerShell-`Get-Content`/`Set-Content` beschädigt UTF-8 ohne BOM – PvSystem/PvFaultModel/ArgsParser/SimulationConfig mussten zurückgesetzt und
mit Python `newline=''` neu bearbeitet werden.

**Noch offen:** Wind; echte Terminalfenster für Wetter, PV und die übrigen Geräte; `start-all.ps1`;
Abstimmung der Wetter-Optionen und Registerpläne mit Alexander und Dominik; Abgabe-Tag `lb02-2026`.

---

### 0.9 Sitzung 2026-10-26: Kühlhaus als viertes Feldgerät, Schritte 1–9 (opencode/big-pickle)

Das Projekt `src/Devices/KuehlhausSimulator` (Kühlhaus, Industrieverbraucher) ist jetzt ein **fertiges LB02-Feldgerät** mit
allen neun Schritten: Solution, Modell, Registerplan, Engine mit Master-Zeit und CSV, Modbus-Server, Wetteranbindung,
Anzeige/Tasten, 28 Tests und Geräte-README.
Die vollständige Anleitung mit Code-Skizzen steht in `docs/devices/KuehlhausSimulator/umsetzung-lb02.md`.

| Bereich | Neu / geändert | Test |
|---|---|---|
| `EnergyTwin.sln` | `KuehlhausSimulator` unter dem Solution-Ordner `Devices` eingetragen | `dotnet build EnergyTwin.sln` |
| `src/Devices/KuehlhausSimulator/KuehlhausSimulator.csproj` | nur noch `OutputType` + `ProjectReference Devices.Common`; Rest kommt aus `src/Directory.Build.props` | – |
| `Kuehlhaus.cs` | `SetDoor(bool)` (behebt die Build-Fehler), **exakte Exponentiallösung** statt Euler, validierte Solltemperatur (−30…0 °C), `Id`, `OutsideTempC`, `RuntimeHours`, `CoolingPowerKW`, `ResetAlarmBelowC`, boolesches `ResetFault()`, Config-Prüfung im Konstruktor | 0 Warnungen; Zahlen unverändert (1,95 K/h ohne Kühlung, 4,9 K/h mit Tür) |
| `KuehlhausConfig.cs` | Einheiten im Namen: `WaermekapazitaetKWhProK`, `WaermedurchgangKWProK`, `KaelteleistungKW`, `ZusatzwaermeKW`, neu `AlarmHystereseK`; `DeviceId` Standard `KH-01` | – |
| `Emums.cs` | → `Enums.cs` umbenannt (Namespace `KuehlhausSimulator`) | – |
| `Simulation/SimulationConfig.cs` | Optionen (Soll, Kälteleistung, Schritt, Dauer, Seed, Zeit, Modbus, `WeatherOptions`), `DefaultModbusPort = 1505`, `ToModelConfig()` | – |
| `Simulation/SimulationEngine.cs` | Schleife mit `StepScheduler`, `Started`/`Restarted`, CSV je Lauf, Wetter je Modellstunde, Modbus in **jedem** Durchlauf (auch Pause), lokales Tagesprofil 5–23 °C; bis Schritt 7 Statuszeile je Modellschritt statt Anzeige | 24 h standalone: 289 Zeilen, 20,7 kWh, 25 Schritte Tür offen; mit Master synchron (x600), nach Master-Abbruch Register 12 = 3 |
| `Simulation/KuehlhausModbus.cs` | Solltemperatur, Tür, Ein/Aus, Reset; Register 20–31 veröffentlichen; Fehlercode 1/2, Betriebszustand aus Kompressor | Diagnose-Client: Sollwert `-20 → -15 °C` angenommen, `force 5` abgelehnt (Register 18 = 2, Adresse 20) |
| `Simulation/WeatherClient.cs` | HTTP-Client wie bei der PV (`/weather?location…`), deutsche Fehler, 5 s Timeout – **am 2026-10-26 gelöscht**, ersetzt durch `Devices.Common.Weather` (§0.14) | Probelauf mit Wettersimulator: 5,6 → 4,4 °C in der Nacht übernommen |
| `Simulation/AutoModeController.cs` | Türöffnungen nach Tageszeit (Rate je Stunde, 1–10 min offen), geseedet | 24-h-Lauf: 25 Schritte offen |
| `IO/ArgsParser.cs`, `IO/CsvLogger.cs` | Optionen (`--id --setpoint --cooling --step --seed --output --weather --location --duration`), CSV mit 16 Spalten (de-AT, BOM, Spalte `Lauf`) | `--help`, Probeläufe |
| `Program.cs` | LB01-Testmenü **ersetzt**: Hilfe, `--register-plan` (Port 1505), Strg+C geordnet, `IOException` → Exit 1 | `dotnet run -- --register-plan` |
| `Modbus.Contracts` | `DeviceType.IndustrialConsumer = 5`, Labels/Beschreibung; neu `KuehlhausRegisters.cs` (20–31); `RegisterPlans.For`; README nachgezogen | `FieldDevices.Tests` 34/34 |

**Entscheidungen:** Gerätetyp 5 = Industrieverbraucher (Wind bleibt 4); Modbus-Standardport **1505** (1502 = PV/Charger, 1503 = Batterie,
1504 = Charger in der Demo);
Fehlercode im Kopf (1 Kompressorausfall, 2 Übertemperatur); Wirkleistung positiv; Wetter nur mit Master vorbelegt,
standalone lokales Tagesprofil (wie bei der PV, damit Standalone-Läufe nicht am ersten Verbindungsversuch hängen).
**Gefundener Fehler in der eigenen Anleitung:** Der dort vorgeschlagene Reset (`return false`, solange der Raum zu warm ist) sperrt sich selbst aus –
ohne Kompressor kühlt nichts, der Raum wird nur wärmer. Umgesetzt ist deshalb: Kompressorausfall immer quittieren,
Übertemperatur-Alarm erst unter `AlarmTempC − AlarmHystereseK`. Die Anleitung ist korrigiert.
**Kleinere Korrekturen beim Testen:** Die Startzeile zeigte „außen −20 °C“ (Konstruktorwert); die Engine setzt die
Außentemperatur jetzt mit einem Nullschritt, bevor sie die erste CSV-Zeile schreibt. Die Wetterzeile sagt bei fehlender
Antwort und ohne letztes Wetter „(lokales Profil)“ statt „letztes Wetter bleibt“.
**Nachtrag: Schritte 7–9 (2026-10-26):** `IO/ConsoleRenderer.cs` (flackerfrei, Balken Soll → Alarm, Zeilen für Wetter,
Tür, Modbus und Meldung, `PromptAsync`, bei umgeleiteter Ausgabe zwei Zeilen je 5 s) und `IO/InputHandler.cs`
(Tasten `T` Tür, `S` Solltemperatur, `F` Kompressorausfall, `R` Reset, `O` Ein/Aus, `A` Automatik, `P`/`<`/`>` nur
standalone, `Q`). `T` schaltet zuerst auf Handbetrieb, damit die Automatik die Tür nicht wieder schließt.
Die Engine zeichnet interaktiv sofort bei Tastenänderung, schreibt am Ende eine Zusammenfassung und wartet auf `Q`;
`Kuehlhaus.AlarmTempC` und `TurnOff` (Leistung sofort 0) ergänzt. `tests/FieldDevices.Tests/ConsumerTests.cs`:
28 Tests (Modell, Modbus, Optionen, Engine/CSV, Anzeige, Automatik), `FieldDevices.Tests` 62/62.
Geräte-README mit Registerplan, zentrale `README.md` und dieser Abschnitt nachgezogen.
**Kleinere Korrekturen dabei:** Hysterese und Abschalten werden vor dem Temperaturschritt geprüft (Tests erlauben
deshalb ~0,2 K Überschwingen); die CSV-Zeit ist ein Feld (`yyyy-MM-dd HH:mm`), nicht Datum und Uhrzeit getrennt.
**Nachtrag: Team-Entscheidungen umgesetzt (2026-10-26):** Das Projekt heißt jetzt **`KuehlhausSimulator`**
(Ordner `src/Devices/KuehlhausSimulator`, `KuehlhausSimulator.csproj`, Namespace `KuehlhausSimulator`, Solution-Eintrag,
Tests, README, `AGENTS.md`, Anleitung und Doku-Ordner `docs/devices/KuehlhausSimulator/`), damit es wie
`PvSimulator`/`BatterySimulator`/`DualPortCharger` benannt ist. `ZusatzwaermeKW` ist von 3 kW auf **1,5 kW** gesenkt
(Gleichgewicht bei offener Tür: Außen + 30 K statt + 60 K; die Zeitkonstante τ = 20 h bleibt). Der Modbus-Standardport ist
**1505** (1502 = PV/Charger, 1503 = Batterie, 1504 = Charger in der Demo), damit alle vier Geräte gleichzeitig laufen können.
Registerplan-Name in `KuehlhausRegisters.Plan` ist `KuehlhausSimulator`. Nachgezogen: `EnergyTwin.sln`,
`tests/FieldDevices.Tests/FieldDevices.Tests.csproj`, `SimulationConfig`, `KuehlhausConfig`, `ConsumerTests`, Geräte-README,
zentrales README, Diagnose-Client-README (Plan und Port 1505) und die Anleitung. `dotnet build EnergyTwin.sln` 0 Warnungen,
`FieldDevices.Tests` 62/62. Der Hysterese-Test prüft die Temperatur jetzt **vor** dem Schritt (den echten Schaltpunkt),
weil seine alte 1,9-K-Toleranz auf den Türverlust von 3 kW kalibriert war. Journal-Eintrag steht im Journal von Dominik
(`docs/development-journal/dominik-Zach.md`).
**Noch offen:** echte Terminalfenster (Anzeige, Tasten, `S`-Eingabe) für Kühlhaus, Master, PV, Batterie, Charger, Client und
Wetter; `start-all.ps1`; Wind; Abgabe-Tag `lb02-2026`.

---

### 0.10 Sitzung 2026-10-26: gemeinsamer Wetterbezug für alle Feldgeräte (opencode/deepseek-v4.1)

Gebaut (0 Warnungen) und getestet, **nicht committet**.

| Bereich | Neu / geändert |
|---|---|
| `SimCore.Contracts` | `IWeatherService.GetAsync(location, simTimeUtc, runId, ct)` als Vertrag; Contracts bleibt abhängigkeitsfrei |
| `Devices.Common` | `WeatherClient` (aus `PvSimulator` hierher verschoben, `IWeatherService`, 5 s Timeout, deutsche Fehler), `WeatherOptions` (`--weather`, `--location`, Standard-URL nur mit Master) und `WeatherFeed` (Abfrage höchstens je Modellstunde, Cache, Retry 10 s, letztes Wetter bleibt, Meldung nach 3 Fehlversuchen) |
| `PvSimulator` | nutzt `WeatherClient` aus `Devices.Common`; Verhalten unverändert (lokaler `WeatherClient` gelöscht) |
| `BatterySimulator` | Außentemperatur `AmbientTemperatureC` statt Konstante; je Modellschritt Wetter → Temperaturmodell; Anzeige/Statuszeile „Wetter“; `--weather`, `--location` |
| `DualPortCharger` | `ChargingPoint.AmbientTemperatureC` (Standard 20 °C) und `DualCharger.AmbientTemperatureC`; je Modellschritt Wetter → Wärmeziel; Anzeige/Statuszeile „Wetter“; `--weather`, `--location` |
| Tests | `Devices.Common.Tests` +7 (`WeatherFeedTests`: Stunden-Cache, neuer Lauf, Fehler behält letztes Wetter, Optionen) = 51; gesamt 206 |

**Entscheidungen:** Interface in `SimCore.Contracts`, HTTP-Client und Feed in `Devices.Common` – Code teilen über eine Assembly
(Komposition/Referenz), **nicht** über Vererbung. Batterie und Charger bleiben ohne Wettersimulator lauffähig (`--weather off`,
standalone) und rechnen dann mit 20 °C. Kein neues NuGet-Paket (HttpClient aus der BCL).
Das Kühlhaus (Abschnitt 0.9) brachte noch einen eigenen `WeatherClient` mit; es ist am 2026-10-26 ebenfalls
auf `Devices.Common` umgestellt (§0.14).

**Probelauf (umgeleitete Ausgabe):** Wettersimulator + Batterie standalone (x600, 1 h) und Charger standalone (x600, 1 h, 1-min-Schritte):
beide zeigen „Wetter salzburg: NACHT · 5,6 °C …“, der Wettersimulator zählt genau eine Abfrage je Modellstunde.
`dotnet build EnergyTwin.sln` ⇒ 0 Warnungen; alle fünf Testprojekte zusammen 206/206 (einzeln gestartet).

**Offen:** echte Terminalfenster; Wind (Zeit, Modbus, Wetter);
`start-all.ps1`; Abgabe-Tag `lb02-2026`.

---

### 0.11 Sitzung 2026-10-26: gemeinsame Bausteine aus der PV nach `src/Shared` (opencode/deepseek-v4.1)

Gebaut (0 Warnungen) und getestet, **nicht committet**. Die PV enthält nur noch ihr Gerät, ihre Anzeige und ihre
CSV-Spalten; wiederverwendbare Grundlagen liegen in der neuen Bibliothek `src/Shared`.

| Bereich | Neu / geändert |
|---|---|
| `src/Shared` | neues Projekt ohne Abhängigkeiten (in `EnergyTwin.sln` unter `src`), README |
| `Shared.Data.CsvFileWriter` | asynchroner CSV-Grundwriter: Datei `<Präfix>_<Datum_Uhrzeit>.csv`, Kopfzeile, Zeilen, Maskierung von Trennzeichen/Anführungszeichen/Zeilenumbrüchen, BOM optional |
| `Shared.Output.BarChart` | unverändert aus `PvSimulator/Output` verschoben |
| `Shared.Weather` | `Season`/`SeasonInfo`, `WeatherCondition`, `IrradiationCalculator` aus der PV verschoben |
| `PvSimulator` | `PvSimulator.csproj` referenziert `Shared`; `CsvExporter` nutzt `CsvFileWriter` und behält seine 18 Spalten; `ConsoleDisplay` (PV-Layout) bleibt im Gerät; alle `using` nachgezogen |
| Tests | unverändert 206/206 (54 + 51 + 28 + 62 + 11), einzeln gestartet; `Pv_RunsStandalone_WritesCsvAndStopsOnCancel` prüft weiterhin CSV und Energie |

**Entscheidungen:** Eine gemeinsame Bibliothek `Shared` (nicht mehrere kleine Projekte), weil die Bausteine klein sind
und dieselben Geräte sie zusammen brauchen. Nur generische Teile wandern; PV-Layout und CSV-Schema bleiben in der PV,
damit die Anzeige und das Abgabeformat unverändert bleiben. Keine neuen NuGet-Pakete.

**Noch offen:** echte Terminalfenster; Wind;
Abgabe-Tag `lb02-2026`.

---

### 0.12 Sitzung 2026-10-26: ein Startbefehl für alle Programme (`start-all.ps1`)

Gebaut und mit `-WhatIf` geprüft, **nicht committet**.

| Bereich | Neu / geändert |
|---|---|
| `start-all.ps1` | PowerShell-Skript im Repo-Root: baut `EnergyTwin.sln`, startet Wetter und Master, wartet auf Port 8080/5100, öffnet PV, Batterie, Ladesäule (Port 1504 wegen PV), Kühlhaus und Diagnose-Client in eigenen Fenstern; belegte Ports werden übersprungen; `-Speed`, `-Autostart`, `-NoClient`, `-WhatIf` |
| `README.md` | Startreihenfolge um den Ein-Befehl-Start und den Hinweis auf `ExecutionPolicy Bypass` ergänzt |
| `AGENTS.md` | §0.3/§0.4/§0.11/§0.12 nachgezogen |

**Entscheidungen:** Jedes Programm bleibt in einem eigenen Fenster, damit die interaktiven Anzeigen und Tasten sichtbar
bleiben. Der Master startet ohne `-Autostart` weiterhin pausiert (Demo-Anforderung); `-Autostart` ist nur eine Abkürzung.
Das Skript baut zuerst einmal und startet danach mit `--no-build`, damit parallele `dotnet run` nicht um die Build-Ausgabe
konkurrieren. Belegte Ports werden erkannt und nicht doppelt belegt.

**Noch offen:** das Skript in einem echten Benutzerfenster durchspielen (der Agent darf keine fremden Konsolenfenster
starten); Abgabe-Tag `lb02-2026`.

---

### 0.13 Sitzung 2026-10-26: PV-Tasten nur standalone, bessere Statuszeilen

| Bereich | Neu / geändert |
|---|---|
| `SimulationEngine` | `P` (Pause), `M` (Modus), `0–4` (Wetter) und `5–9` (Tempo) bleiben im Standalone-Betrieb; mit Master werden sie ignoriert. `SimulationMode.Manual` und `_mode` sind wieder da, `Mode` liefert standalone `AUTO`/`MANUELL`/`PAUSE`, mit Master nur `AUTO`/`PAUSE`. Auto-Wetter läuft standalone nur im Modus `AUTO`; ohne Wettersimulator unverändert lokal |
| `ConsoleDisplay` | Statuszeilen `Zeitquelle`, `Modbus`, `Wetter` umbrechen lange Texte statt sie mit `…` abzuschneiden; Adresse bzw. Standort (`Master 127.0.0.1:5100`, `127.0.0.1:1502`, `salzburg`) werden cyan hervorgehoben; Footer zeigt `P`/`M`/Wetter/Tempo nur standalone, mit Master `Q`/`C`/`R`, Störungen und Hinweis auf Master-Steuerung |
| `CsvExporter`, `SimulationMode` | Modusspalte `AUTO`/`MANUELL`/`PAUSE`; `Manual` wieder enthalten |
| `ArgsParser`, README | Tastenhilfe und Bedienungstabellen: `P`/`M`/`0–4`/`5–9` als „nur standalone“ gekennzeichnet, mit Master ausgeblendet |

**Entscheidungen:** Pause, Modus, Wetter und Tempo sind nur sinnvoll, wenn die PV ihre eigene Uhr und ihr lokales
Wetter nutzt (standalone). Mit Master bleiben diese Tasten wirkungslos und werden im Footer ausgeblendet; die Zeit
kommt vom Master, das Wetter vom Wettersimulator. Die Statuszeilen werden an den Trennern ` · ` umbrochen, damit
die Boxbreite 76 Zeichen bleibt und auch bei langen Master-Meldungen nichts abgeschnitten wird.

**Test:** `dotnet build EnergyTwin.sln` ⇒ 0 Warnungen; alle fünf Testprojekte einzeln 206/206
(54 + 51 + 28 + 62 + 11). Anzeige mit einem temporären Prüfprogramm in beiden Modi gerendert: mit Master Footer
ohne `P`/`M`/Wetter/Tempo, standalone mit allen Tasten und `MANUELL`; Zeitquelle zweizeilig ohne Abschneiden
(keine `…`, Zeilenbreite 76), `Master …`, `127.0.0.1:1502` und `salzburg` mit ANSI-Cyan. Echte Terminalfenster
weiterhin offen.

**Noch offen:** Test in einem echten Terminal; Abgabe-Tag `lb02-2026`.

---

### 0.14 Sitzung 2026-10-26: Kühlhaus auf die gemeinsamen Bausteine umgestellt

Gebaut (0 Warnungen) und getestet, **nicht committet**. Das Kühlhaus hatte als letztes Gerät noch eigene Kopien der
Wetterlogik; jetzt nutzt es – wie PV, Batterie und Charger – `Devices.Common` und `Shared`. Verhalten für die
Bedienung unverändert, nur die Fehlerzeile sagt jetzt wie bei den anderen Geräten „letztes Wetter bleibt“.

| Bereich | Neu / geändert |
|---|---|
| `KuehlhausSimulator.csproj` | zusätzlich `ProjectReference Shared` (für den CSV-Grundwriter) |
| `Simulation/WeatherClient.cs` | **gelöscht** (80 Zeilen, inklusive zweiter `WeatherUnavailableException`); es gilt `Devices.Common.Weather.WeatherClient` |
| `Simulation/SimulationConfig.cs` | `DefaultWeatherUrl`, `WeatherUrl` und `Location` entfallen, dafür `WeatherOptions Weather` (Standard-URL nur mit Master) |
| `IO/ArgsParser.cs` | `--weather`/`--location` über `WeatherOptions.TryApply`, Hilfe über `WeatherOptions.Usage`, Ergebnis `weather.WithDefaultUrl(timeSync.Standalone)` (gleiche Regeln wie bei Charger/Batterie) |
| `Simulation/SimulationEngine.cs` | `WeatherFeed` statt eigener Felder (`_lastWeather`, `_weatherFailures`, `_nextWeatherAttempt`, `_weatherErrorReported`, `RetryInterval`, `German(...)`); `UpdateWeatherAsync` liefert nur noch die Außentemperatur, `WeatherStatus`/`OutsideSource` kommen aus dem Feed; `_config` entfernt |
| `IO/CsvLogger.cs` | Datei, Kopfzeile, BOM und Maskierung über `Shared.Data.CsvFileWriter`; die 16 Spalten und das de-AT-Format bleiben |
| Tests | `ConsumerTests` prüfen `config.Weather.Url`/`config.Weather.Location` und `WeatherOptions.DefaultUrl`; unverändert 62/62 |
| Doku | Geräte-README (Wetterabschnitt, Aufbau, gemeinsame Bausteine, „Bekannte Probleme“), Anleitung Schritt 6/7, zentrale `README.md` (keine Änderung nötig), `AGENTS.md`, Journal |

**Entscheidungen:** Kein eigenes Wetter mehr im Gerät – eine Quelle der Wahrheit für Meldungen, Retry und Cache.
Der CSV-Grundwriter wird geteilt, das Gerät behält aber seine Spalten und sein Zahlenformat; damit bleibt das
Abgabeformat identisch. Gelöscht wurden rund 170 Zeilen Duplikat, ohne eine Zeile Fachlogik zu verlieren.

**Test:** `dotnet build EnergyTwin.sln` ⇒ 0 Warnungen; `dotnet test tests/FieldDevices.Tests` 62/62; Probelauf
Wettersimulator + Kühlhaus standalone mit `--weather http://127.0.0.1:8080`: Spalte `Aussenquelle` =
`Wettersimulator`, Außentemperatur 5,6 → 4,9 °C, der Wettersimulator zählt genau eine Abfrage je Modellstunde;
`--weather off` bleibt beim lokalen Tagesprofil (`WeatherStatus` = „aus (--weather off), lokales Tagesprofil“);
`--help` zeigt den gemeinsamen Wetterblock.

**Noch offen:** echte Terminalfenster; Abgabe-Tag `lb02-2026` (Wind ist am 2026-10-26 entfallen).

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
- **Zuständigkeiten (ursprünglicher Plan, umgesetzt; im Team zu bestätigen):** A = Alexander (Master, `SimCore.Time`, `SimCore.Contracts`), B = Wettersimulator,
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

### Angelegte Struktur (Stand 2026-10-25; LB02-Code in `SimCore.*`, `SimulationMaster`, `Devices.Common`, Charger, siehe §0.3)

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
  Shared/                  lib   keine Abhängigkeiten        (CSV-Writer, Balken, Jahreszeiten/Wetter/Einstrahlung; seit 2026-10-26)
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
Schnittstellen, Tests, Bekannte Probleme). Noch offen: zentrale `README.md`, `.env.example`, `start-all.ps1`.
Neue NuGet-Pakete nur in `Directory.Packages.props` versionieren, im `.csproj` ohne `Version`.

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
- **Noch offen:** Wind auf `Devices.Common` und Modbus (Plan in `Modbus.Contracts` + `RegisterPlans.For`); Wetterbezug analog zu Batterie/Charger.

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
Seit 2026-10-26 liegen `Season`, `WeatherCondition`, `IrradiationCalculator` und `BarChart` in `src/Shared`
(Pfade unten nennen den historischen LB01-Ort).

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

Tasten (LB02): `Q` Ende, `C` Module reinigen, `R` Refresh-Rate, `F` Inverterfehler, `T` Überhitzung, `G` Netzstörung
immer; nur standalone: `P` Pause, `M` Auto/Manuell, `0–4` Wetter und `5–9` Zeitraffer x1/x15/x60/x300/x1800.
Mit Master werden diese Tasten ignoriert und im Footer ausgeblendet; Zeit, Wetter und Tempo kommen dann vom Master
bzw. Wettersimulator (§0.13).

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

> **Seit 2026-10-25 (LB02):** Zeit kommt vom Master (`--master`, Standard 127.0.0.1:5100); der LB01-Aufruf oben geht nur noch
> mit `--standalone` (`--speed`, `--duration`, `--start` sind ohne `--standalone` ein Fehler, `--duration` ist standardmäßig endlos).
> Schleife, CSV und Eingabe sind async (`Thread.Sleep`/`ReadLine`/`Stopwatch` entfernt), CSV hat die neue Spalte `Lauf`,
> `P` und `</>` wirken nur standalone. Die Beschreibung unten ist der LB01-Stand; aktuell: `src/Devices/DualPortCharger/README.md`.

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

## 6. Build-Status (geprüft am 2026-10-13, ergänzt 2026-10-25, .NET SDK 10)

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

`dotnet build EnergyTwin.sln` (nach dem Verschieben):

| Projekt | Ergebnis |
|---|---|
| alle Bibliotheken, Testprojekte | ✅ bauen (2026-10-26 erneut geprüft, 0 Warnungen) |
| `src/Shared` | ✅ baut mit 0 Warnungen (2026-10-26); von der PV referenziert, Verhalten unverändert |
| `tests/SimCore.Tests` | ✅ 54/54 Tests grün (2026-10-25, 5 Wiederholungen ohne Ausfall) |
| `SimulationMaster` | ✅ baut, Probelauf mit umgeleiteter Ausgabe + PowerShell-TCP-Client ok (2026-10-25); echtes Terminal noch ungetestet |
| `Devices/PvSimulator` | ✅ baut; 2026-10-25 mit Master (x600) synchron, Modbus per Diagnose-Client geprüft; 2026-10-26 Wetter vom Wettersimulator übernommen und in der Anzeige sichtbar; standalone unverändert grün |
| `tests/Devices.Common.Tests` | ✅ 51/51 Tests grün (2026-10-26; 24 Zeit, 20 Modbus/Registercodec, 7 Wetter-Feed/-Optionen) |
| `Devices/DualPortCharger` | ✅ baut; 2026-10-26 Wetter übernommen („Wetter salzburg …“); 2026-10-25 mit Master: Master x600 + DC-01 + DC-02 synchron (Abweichung ≤ 0,2 s), Master beendet ⇒ „NICHT ERREICHBAR – Zeit steht“; standalone 24 h endet selbst (CSV 290 Zeilen); Modbus: Kopf und Sollwerte mit eigenem Testclient gelesen/geschrieben, ungültiger Sollwert abgelehnt, belegter Port ⇒ Fehlermeldung (2026-10-25) |
| `Devices/BatterySimulator` | ✅ baut; 2026-10-25 mit Master synchron, Modbus geprüft (Sollwert −2,5 kW angenommen, 9 kW abgelehnt, Modus Auto/Manuell); 2026-10-26 Wetter übernommen („Wetter salzburg …“) |
| `WeatherSimulator` | ✅ baut; Probelauf 2026-10-26: liefert für dieselbe Stunde identische Werte, 404 bei unbekanntem Standort, PV zeigt die gelieferten Werte |
| `tests/Weather.Tests` | ✅ 11/11 (2026-10-26) |
| `Devices/KuehlhausSimulator` (Kühlhaus) | ✅ baut mit 0 Warnungen (2026-10-26): standalone 24 h (289 CSV-Zeilen, 20,7 kWh, Tür 25 Schritte offen), mit Master synchron (x600), Master-Abbruch ⇒ Register 12 = 3, Wetter vom Wettersimulator übernommen, Sollwert per Diagnose-Client `-20 → -15 °C`, `force 5` abgelehnt (Register 18 = 2, Adresse 20); Anzeige, Tasten und Eingabe der Solltemperatur nur mit umgeleiteter Ausgabe geprüft |
| `tests/FieldDevices.Tests` | ✅ 62/62 (2026-10-26; 34 PV/Batterie + 28 Kühlhaus) |
| alle Testprojekte zusammen | ✅ 206/206 (2026-10-26) |

Die Probeläufe liefen mit umgeleiteter Ausgabe. Die Konsolenausgabe von Alex formatiert Zahlen mit Komma, die CSV mit Punkt.

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
- Gerätelogik bleibt frei von Konsole, Uhr und Netzwerk (Vorbild: `Batterie`, `DualCharger`). Ein-/Ausgabe in eigenen Klassen.
- Umgeleitete Konsole ≠ echtes Terminal: Cursor- und Puffer-Funktionen brauchen einen Fallback (Alex und Susi haben einen).
  Immer auch in einem echten Terminalfenster testen; ein NullRef beim ersten Diff-Frame endet mit Exit-Code -1073741819.
- Reproduzierbarkeit: `Random` aus Datum/Ort/Seed erzeugen, nie aus der Uhrzeit.
- Virtuelle Zeit nur über `ISimClock` lesen (Geräte: `SyncedClock`), nie lokal hochzählen. Die reale Uhr immer als
  `System.TimeProvider` übergeben (nicht `DateTime.Now`/`Stopwatch` direkt), damit Tests mit einer Test-Uhr laufen.
- Umgebungsvariablen nur für optionale Entwickler-Features (Muster: `ENERGYSIM_RENDER_LOG`).
- Bei Modelländerungen die Formeln in der jeweiligen `README.md` mitpflegen. Bei Architekturentscheidungen
  §3 dieser Datei aktualisieren und einen Eintrag im Entwicklungsjournal machen.
