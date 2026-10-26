# Entwicklungsjournal – Alexander Lux

<!--
Verpflichtende Tags pro Eintrag:

- Done: Was wurde bearbeitet und welches Ergebnis liegt vor?
- KI: Werkzeug, Modell, Einsatzform und Umgang mit dem Ergebnis; bei keiner KI-Nutzung: keine.
- Artefact: Betroffene Dateien, CSV/Kurve, Screenshot, Dokumentation oder andere Ergebnisse.

Optionale Tags bei Relevanz:
- Comment: Entscheidung, Problem, Erkenntnis oder nächster Schritt.
- Test: Durchgeführter manueller oder automatisierter Test.
-->

## 2026-10-03 – Refactor in saubere Architektur

- **Done:** Aufestzen eines `PvSimulator` in klassenbasierte Architektur: `PvSystem` (IDevice), `SimulationEngine`, `SimulationConfig`, `IrradiationCalculator`, `ConsoleDisplay`, `CsvExporter`. CLI-Argumente für Speed/Step/Peak/Output. Auto- & manueller Modus mit Tastensteuerung.
- **KI:** OpenCode mit Qwen 3.8 als CLI-Agent; Projektstruktur implementieren nach konkreten Anweisungen, manuell angepasst: Klimafaktoren, Balkenbreite, CLI-Hilfe, Energieberechnung.
- **Artefact:** `src/EnergySimulator/` (File Structure), `dotnet-energy-sim-lb01.sln`.
- **Test:** Build erfolgreich (`dotnet build`), 24h-Simulation durchgelaufen, CSV mit 96 Zeilen erstellt.
- **Comment:** Simulation startet mit aktueller Uhrzeit, Zeitskala wählbar während der Simulation über Tasten 5–9. 

## 2026-10-04 – Nahtlose Box-Grenzen, Nachtzustand, Robustheit

- **Done:** Konsolen-Layout auf segmentbasiertes Padding umgestellt (Breite wird aus sichtbarer Zeichenzahl berechnet, ANSI-Farbcodes zählen nicht mit) – Rahmen jetzt nahtlos. Wetterzustand `Night` ergänzt. `Console.Clear` gegen Fehler bei umgeleiteter Konsole abgesichert. Emoji aus dem Rahmen entfernt (Doppelbreite in Terminals).
- **KI:** OpenCode mit Qwen 3.8; Layout-Neubau mit Breitenberechnung, Nachtzustand, Refresh-Entkopplung, Robustheits-Fixes.
- **Artefact:** `Output/ConsoleDisplay.cs`, `Output/BarChart.cs`, `Devices/WeatherCondition.cs`, `Devices/IrradiationCalculator.cs`, `Devices/PvSystem.cs`, `Simulation/SimulationEngine.cs`, `Data/CsvExporter.cs`, `README.md`.
- **Test:** Build fehlerfrei. Frame-Weiterleitung in Datei und Breitenmessung: alle 433 Box-Zeilen exakt 72 Zeichen breit, kein Versatz. Nacht-automatik um 21:30 Uhr korrekt erkannt.
- **Comment:**

## 2026-10-05 – Zwei gefundene Modellfehler

- **Done:** Bug Fixes: Außentemperatur Zuweisung (per Step).Außerdem Sommer-Hitzewelle von +8 auf +12 °C angehoben
- **KI:** keine
- **Artefact:** `Devices/PvFaultModel.cs`, `README.md`.
- **Test:** Summer-Start `2026-07-15`, drei Läufe: Außentemperatur nun 22,4–38,3 °C über den Tag (vorher konstant 12,0) und über beide Läufe identisch; Maximalwerte der Modultemperatur 70,2 °C und 78,5 °C, mit Fehlerzeilen. Wetter bleibt zufallsabhängig, deshalb trifft der Schutz nicht in jedem Lauf auf.
- **Comment:**

## 2026-10-06 – Unbegrenzte Laufzeit, Jahreszeiten und Störungsmodell

- **Done:** 24-Stunden-Grenze entfernt – `SimulationClock` läuft unbegrenzt weiter, rollt bei Mitternacht in den nächsten Tag und setzt nur die Tagesenergie zurück; Datum, Uhrzeit und Jahreszeit werden durchgehend angezeigt. `Season` eingeführt (Monat → Jahreszeit) mit Einstrahlungsfaktor, Sonnenaufgang/-untergang, Umgebungstemperatur und Schneewahrscheinlichkeit. Störungsmodell `PvFaultModel` ergänzt: Modultemperatur mit thermischer Trägheit, Netzspannung, Verschmutzung, Alterung. Überhitzung (70 °C / Reset 55 °C) löst automatisch aus, Netzspannung außerhalb 200–250 V trennt den Wechselrichter, Verschmutzung und Alterung reduzieren den Ertrag dauerhaft. Neue Tasten `T` (Überhitzung erzwingen), `G` (Netzspannungsstörung), `C` (Module reinigen). Zustände `Snow` und `Night` ins Wettermodell aufgenommen. CSV um Datum, Jahreszeit, Temperaturen, Netzspannung, Verschmutzung und Fehlerursache erweitert. CLI-Option `--start` ergänzt, um gezielt Winter oder Sommer zu starten.
- **KI:** OpenCode mit Qwen 3.8; `Season` und `PvFaultModel`, Integration in Engine, PV-System und CSV, Tastenbelegung, CLI-Option.
- **Artefact:** `Simulation/SimulationClock.cs`, `Simulation/SimulationConfig.cs`, `Simulation/Season.cs`, `Output/ConsoleDisplay.cs`, `Devices/PvFaultModel.cs`, `Devices/PvSystem.cs`, `Devices/WeatherCondition.cs`, `Devices/FaultType.cs`, `Simulation/SimulationEngine.cs`, `Output/ConsoleDisplay.cs`, `Data/CsvExporter.cs`, `Program.cs`.
- **Test:** Build fehlerfrei. Herbst-Lauf über Mitternacht: Einstrahlung peakte exakt bei 600 W/m² (1000 × 0.6), Datum rollte auf den Folgetag, Nachtphase korrekt erkannt. Winter-Start `2026-01-15`: 3 von 11 CSV-Zeilen mit Zustand SCHNEE, Peak 1.68 kW. Sommer-Start `2026-07-15`: Peak 8.96 kW, Modultemperatur 70.1 °C → `ÜBERHITZUNG` ausgelöst, Leistung 0 kW, um 16:01 nach Abkühlung auf 49.4 °C selbsttätig zurückgesetzt. Netzspannung pendelt zwischen ca. 206 V und 238 V und dippt abends gelegentlich unter 200 V.
- **Comment:** CSV-Flush-Intervall von 20 auf 5 Zeilen gesenkt. Verschmutzung wirkt mit 0.6 %/Tag, Alterung mit 0.04 %/Tag.

- **Comment:** Bug KW Anzeige des Vortags, dynamic Link Libs, Historische Wetterdaten Österreich, config files, sharing of weather <-> time <-> PV, same with others
    Simulation, erreichen durch IP und Port, könnte man Zeit rekursiv durch andere Simulatoren erhalten (Zeit->PV->Batterie->usw), synchronisieren every x time, gemeinsamer File, Echtzeit, 4 Simulator, Bibliotheken für Wetter (Einstrahlung), Modbus Assembly, zur Uhrzeit Speed umstellen, Modbus Standard UINT16, 24H Cache, Unbekannte Zeit -> Asynchron <- Modbus Communication

## 2026-10-26 – Wettersimulator und Wetteranbindung der PV

- **Done:** Wettersimulator als eigenständiges Programm umgesetzt und die PV-Anlage daran angebunden. Zuerst die Aufgabenstellung (PDF) erneut mit dem Stand verglichen, die Anforderungen an das Wetter (Ort, virtuelle Zeit, Reproduzierbarkeit, sichtbarer Fehler) gesammelt und in einem Arbeitsgespräch Festlegungen getroffen; die Entscheidungen stehen als lokale ADRs in `docs/adr/` (bewusst nicht versioniert) und in `docs/glossary.md`.
  - `SimCore.Contracts`: `WeatherData` (Standort, UTC-Zeit, Lauf, Sonnenhöhe, Bedeckung, Temperatur, Niederschlag, Wind, Strahlung, Zustand) und `WeatherCondition`.
  - `WeatherSimulator`: HTTP-Dienst (`GET /weather?location&simTime&run`, `GET /locations`, `GET /`) auf Port 8080 (änderbar mit `--port`), deterministisch aus WeatherSynth 0.3.0 (stündliche Strahlung und Wind, offline) plus eigenem Modell für Temperatur, Bedeckung, Niederschlag, Sonnenhöhe und Zustand. Seed stabil aus Standort, Jahr und Lauf (FNV-1a statt `string.GetHashCode`). Drei Standorte (`salzburg` Standard, `wien`, `innsbruck`); unbekannter Standort 404, ungültige Zeit 400. Anzeige mit Modus, letzter Abfrage und Zähler; Tasten für manuelle Übersteuerung (sonnig, bewölkt, dicht, Gewitter, Schnee, Regen, Temperatur, Wind, zurück auf Automatik) und sauberes Beenden.
  - PV: neuer `WeatherClient` (async, 5 s Zeitlimit, deutsche Fehlermeldungen), Abfrage je Modellstunde, Übernahme von Einstrahlung, Zustand und Außentemperatur (`PvSystem.ApplyExternalWeather`, `PvFaultModel.Step` mit optionaler Außentemperatur), Statuszeile und Anzeigezeile „Wetter“, Fehlerbehandlung (nach 3 Fehlversuchen sichtbar „WETTER: NICHT ERREICHBAR“, letztes Wetter bleibt, neuer Versuch alle 10 s). Neue Optionen `--weather <url|off>` und `--location`; mit Master ist der Wettersimulator Standard, standalone bleibt das lokale Auto-Wetter (dort wirken auch die Wettertasten `0–4`).
  - Tests: `tests/Weather.Tests` mit 11 Tests (Reproduzierbarkeit, Konstanz innerhalb der Stunde, Standort- und Laufunterschiede, Nacht, Sommer/Winter, manuelle Übersteuerung, Plausibilität).
  - Doku: READMEs von `WeatherSimulator` und `Weather.Tests` neu, PV-README (Status, Optionen, Schnittstellen, Wetterabschnitt, bekannte Probleme), zentrale `README.md` (Architektur, Startreihenfolge, Demo, Zuständigkeiten, Schnittstellen, Tests, Konfiguration), `AGENTS.md` und dieses Journal.
- **KI:** opencode (deepseek-v4.1-flash-free) => Anforderungsabgleich mit dem PDF, Struktur und Arbeitsplan, Umsetzung von Wettersimulator, HTTP-Vertrag und PV-Anbindung, Tests, Fehlersuche, Dokumentation.
- **Artefact:** `src/SimCore.Contracts/WeatherData.cs`, `src/WeatherSimulator/` (`Program.cs`, `Simulation/WeatherModel.cs`, `Simulation/LocationCatalog.cs`, `Simulation/ManualWeather.cs`, `README.md`), `src/Devices/PvSimulator/` (`Simulation/WeatherClient.cs`, `Simulation/SimulationEngine.cs`, `Simulation/SimulationConfig.cs`, `Simulation/ArgsParser.cs`, `Devices/PvSystem.cs`, `Devices/PvFaultModel.cs`, `Output/ConsoleDisplay.cs`, `README.md`), `src/Devices/WindSimulator/Program.cs` (Platzhalter), `tests/Weather.Tests/` (`WeatherModelTests.cs`, `README.md`), `Directory.Packages.props` (WeatherSynth 0.3.0), `README.md`, `AGENTS.md`
- **Test:** `dotnet build EnergyTwin.sln`: 0 Warnungen, 0 Fehler. `dotnet test tests/Weather.Tests`: 11/11 grün; `tests/FieldDevices.Tests` 34/34; `tests/Devices.Common.Tests` 44/44; `tests/SimCore.Tests` 54/54; `tests/ModbusDiagnosticClient.Tests` 28/28. Probelauf mit echten Prozessen: Wettersimulator (8080) + Master + PV; PV zeigt „salzburg: BEWÖLKT · 7.1 °C · 2.9 m/s · 166 W/m²“, der Wettersimulator zählt die Abfrage und liefert für dieselbe Stunde identische Werte (165.5 W/m²). Standalone-Lauf der PV schreibt weiterhin CSV.
- **Comment:** Wetter ist eine reine Funktion von (Standort, Stunde, Lauf); WeatherSynth liefert die Strahlungs- und Windbasis, alles andere ist eigenes Modell, damit die Antworten im Team erklärbar bleiben. Das Paket ist AGPL-3.0 (SolarCalculator LGPL, DWD-Daten CC BY 4.0) und wird nur beim Build geladen; zur Laufzeit braucht das System kein Internet. Offen: Wind-Simulator, Wetter für Batterie/Charger, Test der interaktiven Anzeigen im echten Terminal, Abstimmung der Registerpläne und der geänderten Optionen mit Alexander und Dominik, Abgabe-Tag `lb02-2026`. Nicht committet.

### Nachtrag 2026-10-26 – Gemeinsamer Wetterbezug für Batterie und Charger

- **Done:** `IWeatherService` in `SimCore.Contracts` angelegt; `WeatherClient` aus der PV nach `Devices.Common/Weather` verschoben (Namespace `Devices.Common.Weather`, implementiert den Vertrag). `WeatherOptions` (`--weather`, `--location`, Standard-URL nur mit Master) und `WeatherFeed` (Stunden-Cache, Retry 10 s, letztes Wetter bleibt, Meldung nach 3 Fehlversuchen) ergänzt. PV auf den gemeinsamen Client umgestellt (Verhalten unverändert). Batterie und Charger übernehmen die Außentemperatur je Modellschritt vom Wetter (`Batterie.AmbientTemperatureC`, `ChargingPoint.AmbientTemperatureC`, `DualCharger.AmbientTemperatureC`), Statuszeile „Wetter“; ohne Wetter weiterhin feste 20 °C. 7 neue Tests in `Devices.Common.Tests` (`WeatherFeedTests`, Optionen). READMEs von Contracts, `Devices.Common`, Batterie, Charger und zentrale `README.md` nachgezogen; `AGENTS.md` §0.3/§0.4/§0.9/§3/§6 aktualisiert. `AGENTS1.md` als obsolet markiert und als Archiv ins Entwicklungsjournal verschoben (siehe voriger Eintrag).
- **KI:** opencode (deepseek-v4.1-flash-free) => Interface/Client/Feed entworfen, PV-Umzug, Batterie- und Charger-Anbindung, Tests, Doku.
- **Artefact:** `src/SimCore.Contracts/IWeatherService.cs`, `src/Devices.Common/Weather/{WeatherClient,WeatherOptions,WeatherFeed}.cs`, `src/Devices/BatterySimulator/{Batterie,Simulator,SimulatorOptions,ArgsParser,ConsoleDashboard}.cs`, `src/Devices/DualPortCharger/{Models/ChargingPoint,Models/DualCharger,Simulation/SimulationConfig,Simulation/SimulationEngine,IO/ArgsParser,IO/ConsoleRenderer}.cs`, `tests/Devices.Common.Tests/WeatherFeedTests.cs`, READMEs, `AGENTS.md`
- **Test:** `dotnet build EnergyTwin.sln` ⇒ 0 Warnungen, 0 Fehler. `dotnet test EnergyTwin.sln` ⇒ 178/178 grün (`Devices.Common.Tests` 51). Probelauf Wettersimulator + Batterie standalone (x600, 1 h) und Charger standalone (x600, 1 h): beide zeigen „Wetter salzburg: NACHT · 5,6 °C …“, der Wettersimulator zählt genau eine Abfrage je Modellstunde.
- **Comment:** Vererbung wäre hier falsch (kein „ist ein“); geteilt wird über die Assembly `Devices.Common` per Komposition. Batterie und Charger bleiben ohne Wettersimulator lauffähig; mit Master ist der Dienst standardmäßig eingetragen.


### Nachtrag 2026-10-26 – Gemeinsame Bausteine nach src/Shared ausgelagert

- **Done:** Die PV-Anlage enthält nur noch ihr Gerät, ihre Anzeige und ihre CSV-Spalten. Die wiederverwendbaren Grundlagen liegen jetzt in der neuen Bibliothek `src/Shared`: `Shared.Data.CsvFileWriter` (asynchroner CSV-Grundwriter mit Zeitstempel-Datei, Kopfzeile, Zeilen und Maskierung), `Shared.Output.BarChart`, `Shared.Weather.Season`/`SeasonInfo`, `WeatherCondition` und `IrradiationCalculator`. Die PV referenziert `Shared`, `CsvExporter` nutzt den gemeinsamen Writer und behält seine 18 Spalten, das PV-Layout bleibt in `ConsoleDisplay`.
- **KI:** opencode (deepseek-v4.1-flash-free) => Bestandsaufnahme der PV-Bausteine, Auswahl der generischen Teile, neue Bibliothek, Umzug, Anpassung der Namespaces und Doku.
- **Artefact:** `src/Shared/` (`Shared.csproj`, `Data/CsvFileWriter.cs`, `Output/BarChart.cs`, `Weather/Season.cs`, `Weather/WeatherCondition.cs`, `Weather/IrradiationCalculator.cs`, `README.md`), `src/Devices/PvSimulator/` (`PvSimulator.csproj`, `Data/CsvExporter.cs`, `Devices/PvSystem.cs`, `Devices/PvFaultModel.cs`, `Output/ConsoleDisplay.cs`, `Simulation/SimulationEngine.cs`, `README.md`), `EnergyTwin.sln`, zentrale `README.md`, `AGENTS.md`.
- **Test:** `dotnet build EnergyTwin.sln` ⇒ 0 Warnungen, 0 Fehler. Testprojekte einzeln: `SimCore.Tests` 54/54, `Devices.Common.Tests` 51/51, `ModbusDiagnosticClient.Tests` 28/28, `FieldDevices.Tests` 62/62 (inkl. `Pv_RunsStandalone_WritesCsvAndStopsOnCancel` mit CSV und Energie), `Weather.Tests` 11/11 – zusammen 206/206.
- **Comment:** Eine gemeinsame Bibliothek statt mehrerer kleiner Projekte; nur generische Teile wandern, damit PV-Anzeige und CSV-Abgabeformat unverändert bleiben. Keine neuen NuGet-Pakete. Nicht committet.

### Nachtrag 2026-10-26 – PV-Tasten nur standalone, Doku-Abgleich

- **Done:** Die PV-Tasten `P` (Pause), `M` (Modus), `0–4` (Wetter) und `5–9` (Tempo), wirken nur im Standalone-Betrieb; mit Master werden sie ignoriert und im Footer ausgeblendet. `SimulationMode.Manual` und die Modusanzeige `AUTO`/`MANUELL`/`PAUSE` sind (mit Master nur `AUTO`/`PAUSE`), Auto-Wetter läuft standalone nur im Modus `AUTO`. Statuszeilen umbrechen an ` · ` statt abgeschnitten zu werden; Adresse und Standort sind farbig. Zentrale `README.md` geglättet: Master/SimCore = Alexander, Kühlhaus = Dominik, Testprojekte einzeln starten (Parallelstart kann an echten TCP-Ports scheitern), Abgabe-Tag als offener Punkt vermerkt. Zuständigkeitsangaben in den Modul-READMEs an ADR-0002/0011 angeglichen.
- **KI:** opencode (deepseek-v4.1-flash-free) => Tasten-Gate, Moduslogik, Anzeige, Doku-Abgleich.
- **Artefact:** `src/Devices/PvSimulator/Simulation/SimulationEngine.cs`, `Output/ConsoleDisplay.cs`, `Simulation/SimulationMode.cs`, `Data/CsvExporter.cs`, `Simulation/ArgsParser.cs`, `README.md`, zentrale `README.md`, `AGENTS.md`, Modul-READMEs, dieses Journal.
- **Test:** `dotnet build EnergyTwin.sln` ⇒ 0 Warnungen, 0 Fehler. Testprojekte einzeln: `SimCore.Tests` 54/54, `Devices.Common.Tests` 51/51, `ModbusDiagnosticClient.Tests` 28/28, `FieldDevices.Tests` 62/62, `Weather.Tests` 11/11 – zusammen 206/206. Anzeige- und Tastenprüfung mit temporärem Prüfprogramm: mit Master Footer ohne `P`/`M`/Wetter/Tempo, standalone mit allen Tasten und `MANUELL`.
- **Comment:** Pause, Modus, Wetter und Tempo gehören zur lokalen Uhr und zum lokalen Wetter; mit Master bleiben Zeit und Wetter zentral. Die Doku nennt jetzt die tatsächlichen Zuständigkeiten und den sicheren Testaufruf. Offen bleiben nur echte Terminalfenster und das Tag `lb02-2026`.
