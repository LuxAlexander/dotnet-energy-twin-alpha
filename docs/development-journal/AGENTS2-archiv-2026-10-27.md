# AGENTS2 – Archiv der AGENTS.md (Stand 2026-10-27)

Am 2026-10-27 aus `AGENTS.md` ausgelagert, um die Pflichtlektüre zu kürzen.
Inhalt unverändert (Sitzungsprotokolle, LB01-Migrationsplan §4, LB01-Projektdetails §5).
Der aktuelle Zustand steht in `AGENTS.md` (§0.3); die laufende Dokumentation bleibt in den
Entwicklungsjournalen und den Projekt-READMEs.

---

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

### 0.15 Sitzung 2026-10-27: PV-CSV-Fehler behoben, CSV-/Konsolen-Helfer nach `Shared` ausgelagert

| Bereich | Neu / geändert |
|---|---|
| `src/Shared` | neu: `Data/CsvFormat.cs` (Zahlen de-AT/Dezimalkomma via `Num(format)`, InvariantCulture für ISO-Datum/Zeit), `Data/CsvSchema.cs` (Grundspalten `Gerät;Datum;Zeit;Lauf` – `Gerät` ist der Anlagenname), `Output/ConsoleText.cs` (`Fit`) |
| `PvSimulator` | CSV erst de-AT statt Punkt (Excel las `1.66` als „Jan 66“), jetzt 19 Spalten: erste Spalte `Gerät` („PV-Anlage“), `Lauf` vor die gerätespezifischen Spalten; Kopf aus `CsvSchema.BaseHeader`, Zahlen über `CsvFormat.Num`; Test `Pv_RunsStandalone_WritesCsvAndStopsOnCancel` angepasst |
| Andere Geräte | **unverändert:** Batterie (schreibt weiter Punkt), Charger, Kühlhaus, Diagnose-Client, Master – die Helfer stehen bereit (siehe `src/Shared/README.md`); Umstellung nur bei Bedarf, vorher im Team abstimmen |

**Entscheidung:** Zahlenformat zentral in `Shared.Data.CsvFormat`, damit die Excel-Datum-Falle (heutiger
Fehler) nicht erneut pro Gerät auftritt; die Grundspalten sind bewusst schmal (`Gerät;Datum;Zeit;Lauf`),
damit bestehende Spaltenpläne der Geräte nicht brechen. `ConsoleText.Fit` ist die Sammelstelle für die
in Charger/Batterie/Kühlhaus je duplizierte Zeilenkürzung (dort noch nicht ersetzt).
**Test:** `dotnet build EnergyTwin.sln` 0 Warnungen; `dotnet test tests/FieldDevices.Tests` 62/62.

---

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
- `Data/CsvExporter.cs` – schreibt `docs/data/<ID>_<Datum>_<Uhrzeit>.csv`, `;`, de-AT (Dezimalkomma, seit 2026-10-27 –
  Punkt-Werte wie `1.66` wurden in Excel als Datum gelesen), 18 Spalten, nutzt `Shared.Data.CsvFileWriter`.
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
| CSV-Zahlen | Komma (de-AT, seit 2026-10-27) | Punkt (Invariant) | Komma (de-AT), BOM |
| Einheiten | `PowerKw`, `Kwh` | `ActualKW`, `KWh` | `PowerKW`, `KWh` |
| Zufall | `new Random()` | `new Random()` im Profil | ein `Random`, `--seed` |
| Tastenkonflikte | `P` Pause, `M` Modus, `R` Refresh, Ziffern = Wetter/Tempo | Leertaste Pause, `M` Modus, `R` Reset | `P` Pause, `A` Modus, `R` Reset, Ziffern = Punkt |


