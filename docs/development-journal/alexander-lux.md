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

## 2026-10-27 – PV-CSV: Dezimalkomma statt Punkt, Wetter-Quellliste, AGENTS.md gekürzt

- **Done:**
  - PV-CSV von Punkt auf **de-AT Dezimalkomma** umgestellt: Excel las Zahlen wie `1.66` als Datum („Jan 66").
    Datum und Uhrzeit bleiben InvariantCulture/ISO. CSV-/Konsolen-Helfer nach `src/Shared` ausgelagert (`CsvFormat`,
    `CsvSchema` mit Grundspalten `Gerät;Datum;Zeit;Lauf`, `ConsoleText.Fit`); PV-CSV jetzt 19 Spalten mit erster
    Spalte `Gerät` = „PV-Anlage". Die Helfer stehen Charger/Batterie/Kühlhaus bereit, dort noch nicht umgestellt.
  - Formeln des Wettersimulators auf **Quellen geprüft** und in dessen README dokumentiert: belegte Formeln
    (Sonnenhöhe Cooper/Spencer/Duffie & Beckman, ISA-Lapse-Rate, WeatherSynth-DWD-Daten, LogLaw/IEC,
    Beaufort-Schwelle 14 m/s, Rain-Snow-Übergang 0–2 °C, Okta) getrennt von eigener Heuristik (Jahresmittel/
    Jahresgang, Tages-Offset ±3 K, Regenskala, Dämpfungsfaktor, Nachtwolken 40 %), ehrlich als „nicht kalibriert"
    markiert.
  - **AGENTS.md von 788 auf 410 Zeilen gekürzt:** Sitzungsprotokolle §0.7–§0.15, LB01-Migrationsplan und
    LB01-Projektdetails komplett in das neue Archiv `docs/development-journal/AGENTS2-archiv-2026-10-27.md`
    ausgelagert; in der AGENTS nur noch Verweise. §0.1 um Regel ergänzt: **Tests nur vor einem Commit oder
    wenn ausdrücklich gewünscht**. Archiv folgt dem Muster von `AGENTS1-archiv-2026-10-10.md`.
- **KI:** opencode · GLM 5.3 Flash => Ursachensuche, PV-Umstellung, Auslagerung nach `Shared`, Formel-quellen-Analyse, AGENTS-Kürzung per Skript, Doku und Journale nachgezogen.
- **Artefact:** `src/Devices/PvSimulator/Data/CsvExporter.cs`, `src/Devices/PvSimulator/README.md`, `src/Shared/Data/CsvFormat.cs`, `src/Shared/Data/CsvSchema.cs`, `src/Shared/Output/ConsoleText.cs`, `src/Shared/README.md`, `src/WeatherSimulator/README.md`, `AGENTS.md`, neu `docs/development-journal/AGENTS2-archiv-2026-10-27.md`, `tests/FieldDevices.Tests/FieldDeviceTests.cs`.
- **Test:** `dotnet build EnergyTwin.sln` 0 Warnungen; `dotnet test tests/FieldDevices.Tests` 62/62 (PV-Test prüft jetzt Gerät-Grundspalten und Startzeit).
- **Comment:** Damit schreibt die PV wie Charger und Kühlhaus de-AT; die Batterie schreibt weiterhin mit Punkt (gleiche Excel-Falle dort noch offen, §0.4).

## 2026-11-08 – LB03: Docker-Broker und Datenweg aller vier Geräte

- **Done:** Verantwortung für die Broker-Einrichtung übernommen. Compose auf RabbitMQ 4.4.0-management festgelegt; feste Zugangsdaten entfernt, ignorierte lokale `.env` mit zufälligem Passwort angelegt, `.env.example` ergänzt. Echte Konfigurationsdatei und Plugin-Datei nur lesend eingebunden, fehlende Mount-Dateien werden nicht als Ordner angelegt. Lokale Portbindung, benanntes Datenvolume, stabiler Hostname, Healthcheck und Start-/Stopp-Anleitung ergänzt. Docker Desktop und Broker gestartet, Management-Anmeldung sowie MQTT-Empfang aller vier Feldgeräte über echten Master und Gateway geprüft. Zuständigkeiten, SRS, zentrale README, Gateway-README und AGENTS aktualisiert; alten Aufgabenabgleich als historisch gekennzeichnet.
- **KI:** Codex (GPT-6) => Konfiguration, lokaler Probelauf, Dokumentation und Nachweis.
- **Artefact:** `docker/docker-compose.yml`, `docker/.env.example`, `docker/rabbitmq.conf`, `docker/enabled_plugins`, `docker/README.md`, `docs/nachweise/2026-11-08-docker-alexander/`, `README.md`, `src/ModbusRabbitGateway/README.md`, `docs/SRS-LB03.md`, `docs/task-status/aufgabe-03.md`, `AGENTS.md`.
- **Test:** Compose-Konfiguration gültig; `up -d --wait` erfolgreich, Broker healthy; MQTT/AMQP/HTTP-Listener vorhanden; authentifizierte Management-API meldet 4.4.0. `dotnet build EnergyTwin.sln`: 0 Fehler, 0 Warnungen. MQTT-Testempfänger erhält PV-01 (18), BAT-01 (21), DC-01 (19), KH-01 (15 Messwerte), jeweils good/running mit virtueller Zeit, Sequenz und Einheiten. Gateway `--once` Exit-Code 0. Keine Unit-Tests ausgeführt.
- **Comment:** Broker bleibt aktiv; eigene temporäre Simulator-/Master-/Wetter-Prozesse nach der Prüfung beendet. Interaktiver Viewer, Tastenbedienung, geordnetes Beenden aller Programme und vollständiger neuer Aufgabenabgleich bleiben offen. Bestehender Graph wurde nicht neu aufgebaut. Alte Zugangsdaten sind aus der aktuellen Compose-Datei entfernt; die Git-Historie wurde nicht verändert. Kein Commit, kein Push.

### Nachtrag 2026-11-08 – Wetter über RabbitMQ: Umfang geklärt

- **Done:** Im grill-with-docs-Interview bestätigt: Alexander bleibt für den Broker zuständig. Wetterdaten sollen zusätzlich für den Viewer veröffentlicht werden; die Feldgeräte bleiben beim HTTP-Wetterbezug. Diese Umfangsentscheidung in ADR-0013 gespeichert; Auslöser, Zeitquelle, Nachricht und Fehlerverhalten bleiben Gegenstand weiterer Interviewrunden.
- **KI:** Codex (GPT-6.1) => Interview mit grilling und domain-modeling, Faktenabgleich des Wetterdienstes, ADR.
- **Artefact:** `docs/adr/0013-wetter-rabbitmq-sichtbarkeit.md`, `AGENTS.md`, dieses Journal.
- **Test:** Nur Planungsdokumentation geändert; dotnet build EnergyTwin.sln --no-restore: 0 Fehler, 0 Warnungen. Keine Unit-Tests ausgeführt.
- **Comment:** Noch keine Wetter-MQTT-Implementierung. ADRs bleiben gemäß ADR-0004 lokale Arbeitsunterlagen. Umsetzung erst nach abschließender Bestätigung des gemeinsam erarbeiteten Entwurfs.

### Nachtrag 2026-11-08 – Wetterstrom und zusätzliche Klimastandorte bestätigt

- **Done:** Interviewentscheidungen in ADR-0013 ergänzt: unabhängiger Wetterstrom zur Master-Zeit, alle vorhandenen Größen und Standorte sowie zwei zusätzliche Standorte (Sahara und antarktische Forschungsstation). Konkrete Orte, Klimaprofile und Veröffentlichungsrhythmus sind noch offen. Österreichisch geprägte Temperatur-/Niederschlagsformeln als Modellgrenze dokumentiert.
- **KI:** Codex (GPT-6.1) => Faktenprüfung, Fortsetzung des Interviews und ADR-Fortschreibung.
- **Artefact:** `docs/adr/0013-wetter-rabbitmq-sichtbarkeit.md`, dieses Journal.
- **Test:** Nur Dokumentation geändert; dotnet build EnergyTwin.sln --no-restore: 0 Fehler, 0 Warnungen. Keine Unit-Tests ausgeführt.
- **Comment:** Standorterweiterung ist beauftragt, wird nach abgeschlossener Entwurfsabstimmung umgesetzt. Keine zusätzlichen Standorte im Code angelegt.

### Nachtrag 2026-11-08 – Wetterorte und Veröffentlichungsrhythmus festgelegt

- **Done:** Tamanrasset und Neumayer III als neue Referenzorte bestätigt. Ziel möglichst realitätsnaher Klimaprofile bei überschaubarem Änderungsaufwand dokumentiert. MQTT veröffentlicht jede reale Sekunde den neuesten stündlichen Wetterstand aller Standorte; keine Nachlieferung übersprungener Modellstunden. Grenzen der mitteleuropäischen WeatherSynth-Defaults festgehalten.
- **KI:** Codex (GPT-6.1) => Interviewfortsetzung und ADR-Pflege.
- **Artefact:** `docs/adr/0013-wetter-rabbitmq-sichtbarkeit.md`, dieses Journal.
- **Test:** Dokumentationsänderung; dotnet build EnergyTwin.sln --no-restore: 0 Fehler, 0 Warnungen. Keine Unit-Tests ausgeführt.
- **Comment:** Manuelle Übersteuerung und Verhalten bei Pause/Verbindungsunterbrechungen noch abzustimmen. Kein Implementierungsbeginn.

### Nachtrag 2026-11-08 – Manuelle Standortwahl und Pausenmeldungen abgestimmt

- **Done:** Gezielt standortbezogene manuelle Wetterübersteuerung und MQTT-Wiederanlauf ohne Nachlieferung bestätigt. Nutzerwunsch einmaliger paused-/continued-Meldungen in ADR-0013 aufgenommen. Details zu dienstweitem Status, Messwerten während Pause und Master-Verlust bleiben offen. Begriffe Wetterstrom, Klimaprofil und standortbezogene Übersteuerung im bestehenden docs/glossary.md ergänzt.
- **KI:** Codex (GPT-6.1) => Interview und ADR-Fortschreibung.
- **Artefact:** `docs/adr/0013-wetter-rabbitmq-sichtbarkeit.md`, dieses Journal.
- **Test:** Dokumentationsänderung; Build mit 0 Fehlern/0 Warnungen geprüft. Keine Unit-Tests ausgeführt.
- **Comment:** Bisher globale manuelle Übersteuerung wird im Entwurf standortbezogen. Kein Implementierungsbeginn; Klärung der Pause vor Umsetzung.

### Nachtrag 2026-11-08 – Wetterstatus bei Pause und Master-Verlust präzisiert

- **Done:** Einmalige dienstweite paused-/continued-Zustandsmeldungen bestätigt; regelmäßige Wetterveröffentlichung stoppt in Pause. Manuelle Standortänderungen werden in Pause einmal mit unveränderter Modellzeit und manual-Kennzeichnung gesendet. Ohne gültige Master-Zeit keine neuen Wetterwerte; noTime/masterLost und Wiederanlaufzustand festgelegt. ADR-0013 fortgeschrieben.
- **KI:** Codex (GPT-6.1) => Interview und ADR-Pflege.
- **Artefact:** `docs/adr/0013-wetter-rabbitmq-sichtbarkeit.md`, dieses Journal.
- **Test:** Dokumentationsänderung; dotnet build EnergyTwin.sln --no-restore: 0 Fehler, 0 Warnungen. Keine Unit-Tests ausgeführt.
- **Comment:** Noch offen: später gestarteter Viewer, MQTT-Aktivierung und manuelle Vorgaben bei neuer RunId. Kein Implementierungsbeginn.

### Nachtrag 2026-11-08 – Wetter-MQTT: Interview abgeschlossen, Gesamtentwurf zur Bestätigung

- **Done:** Letzte Interviewentscheidungen bestätigt: gespeicherte Wetterstände und Status für späteren Viewer, MQTT ausdrücklich über geplantes --mqtt aktivieren, manuelle Vorgaben bei neuer RunId zurücksetzen. ADR-0013 zum konsistenten Gesamtentwurf mit Nachrichten-/Statusregeln und Akzeptanzkriterien zusammengeführt. Technische Ausgestaltung und initiale pausierte Momentaufnahme ausdrücklich zur Gesamtfreigabe aufgenommen. Planungsstand in AGENTS, Wetter-README und SRS abgeglichen.
- **KI:** Codex (GPT-6.1) => Entwurfskonsolidierung, Prüfkriterien und Dokumentation.
- **Artefact:** `docs/adr/0013-wetter-rabbitmq-sichtbarkeit.md`, `AGENTS.md`, `src/WeatherSimulator/README.md`, `docs/SRS-LB03.md`, dieses Journal.
- **Test:** Dokumentationsänderung; dotnet build EnergyTwin.sln --no-restore: 0 Fehler, 0 Warnungen. Keine Unit-Tests ausgeführt.
- **Comment:** Gesamtentwurf wartet auf ausdrückliche abschließende Bestätigung gemäß grilling. Keine Implementierung vor dieser Bestätigung. Konkrete Klimaparameter/Quellen werden im genehmigten Umfang bei der Umsetzung recherchiert.

### Nachtrag 2026-11-08 – Wetter-MQTT und Klimaprofile umgesetzt

- **Done:** Nach ausdrücklichem Umsetzungsauftrag den bestätigten Wetterentwurf realisiert. Neue Standorte Tamanrasset und Neumayer III mit recherchierten Temperatur-/Niederschlagskenngrößen bzw. ausdrücklich dokumentierten synthetischen Näherungen. Österreichische numerische Modellbasis beibehalten. Übersteuerung threadsicher je ausgewähltem Standort; L wählt Standort, neue RunId/Master-Sitzung löscht Vorgaben. Optionaler --mqtt-Ausgang mit Master-Zeit, UTC-Nachrichten, Retain/QoS 1, einmaligen paused/continued/noTime/masterLost-Meldungen, Änderungen in Pause, automatischem Wiederanlauf ohne historischen Puffer und offline/Last Will. Gemeinsame Zugangsdatenauflösung nach Shared.Config ausgelagert, Gateway-Fassade kompatibel. MasterSession lesend verfügbar gemacht. Startskript um -WeatherMqtt erweitert. Wettervertrag, Klimadatenquellen, Grenzen, Anleitungen und Nachweise ergänzt; ADR-0013 als bestätigt/umgesetzt markiert.
- **KI:** Codex (GPT-6.1) => Umsetzung, Quellenrecherche (JMA/BAS/AWI/pvlib), funktionaler MQTT-/HTTP-Probelauf und Dokumentation.
- **Artefact:** `src/WeatherSimulator/` (Program, Simulation, Publishing), `src/Shared.Config/MqttCredentials.cs`, Gateway-Zugangsdaten-Fassade, MasterSession-Eigenschaften in SimCore.Time/Devices.Common, `start-all.ps1`, `tests/Weather.Tests/RegionalWeatherTests.cs`, angepasster bestehender Übersteuerungs-Prüffall, `docs/Wetter-Nachrichtenvertrag-LB03.md`, `docs/nachweise/2026-11-08-wetter-mqtt/`, READMEs, SRS, AGENTS, lokale ADRs.
- **Test:** Abschließender Solution-Build mit 0 Fehlern/0 Warnungen; Startskript inklusive -WeatherMqtt mit -WhatIf geprüft. Isolierter RabbitMQ 4.4.0 auf 1884: fünf Orte, Stundenkonstanz, Polartag/-nacht, Jahreszeiten, Schnee bei Nacht, Sahara-Jahresniederschlag 63,0 mm für Lauf 7; noTime, Pause, gezielter manueller Wert, Retain-Einstieg, continued, neuer Lauf, Broker-Ausfall/Wiederanlauf, Master-Verlust, neue MasterSession bei gleicher RunId und offline erfolgreich geprüft. Tatsächlich gestartete Wetter-/Master-Programme: HTTP und MQTT je sechs numerische Werte für alle fünf Orte identisch, HTTP 404/400 korrekt, Last Will nach Prozessabbruch, HTTP ohne --mqtt funktionsfähig. 7 neue Unit-Prüffälle gebaut; keine Unit-Tests ausgeführt.
- **Comment:** Kein wissenschaftlich kalibriertes Wettermodell; vereinfachte regionale Profile und Grenzen im Wetter-README. Interaktive Bedienung im Terminal und MQTT-Viewer bleiben live zu zeigen. Eigene Prüfprozesse beendet, temporärer Broker nach ID-Prüfung entfernt; Projektbroker bleibt healthy. Kein Commit/Push. Bestehende Journale anderer Personen unverändert.

### Nachtrag 2026-11-08 – PV-Hinweisbox umbrechen und GetStatus-Interview beginnen

- **Done:** Ursache der überbreiten Master-Fehlermeldung gefunden: gelbe Benachrichtigung wurde direkt als eine Row ausgegeben, während Statuszeilen bereits Wrap nutzten. Hinweisbox auf denselben Info-/Wrap-Pfad umgestellt, vollständiger Text bleibt mehrzeilig innerhalb des Rahmens. PV-README und AGENTS ergänzt. Interview zum eigenen GetStatus-Skill vorbereitet; Überschneidung mit Susannes readTask und Dominiks probelauf anhand vorhandener Skills geprüft. Lokalen ADR-0014-Entwurf mit vorgegebenen Statuskategorien und noch offenen Abgrenzungsentscheidungen gespeichert.
- **KI:** Codex (GPT-6.1) => PV-Korrektur, Anzeigeprüfung, Faktenaufnahme für grill-with-docs und ADR-Entwurf.
- **Artefact:** `src/Devices/PvSimulator/Output/ConsoleDisplay.cs`, PV-README, `AGENTS.md`, lokale Arbeitsunterlage `docs/adr/0014-getstatus-skill.md`, dieses Journal.
- **Test:** `dotnet build EnergyTwin.sln --no-restore`: 0 Fehler, 0 Warnungen. Ausgabeprüfung mit dem Originalfehler und einer 180 Zeichen langen ungetrennten Zeichenfolge: alle Rahmenzeilen genau 76 Zeichen, vollständiger Text erhalten. Keine Unit-Tests ausgeführt.
- **Comment:** Eigener GetStatus-Skill noch nicht implementiert; Interview und Bestätigung des Gesamtentwurfs folgen. Bestehende nicht zugehörige Änderungen und Screenshots unverändert gelassen. Kein Commit/Push.

### Nachtrag 2026-11-08 – Eigener Skill: Let's Document statt GetStatus

- **Done:** Auf Nutzerwunsch GetStatus verworfen (zu ähnlich zu readTask). Neuen Skill Let's Document als Dokumentationspflege abgegrenzt: README-Lücken ergänzen, alte/verworfene Informationen archivieren und Abschnitte zusammenfassen. ADR-0014 als verworfen markiert, ADR-0015-Entwurf angelegt und AGENTS-Übergabe aktualisiert. Interview zu Umfang, Archivierung und Verdichtung begonnen.
- **KI:** Codex (GPT-6.1) => grill-with-docs-Fortsetzung, Quellenaufnahme und ADR-Entwurf.
- **Artefact:** lokale `docs/adr/0014-getstatus-skill.md` und `0015-lets-document-skill.md`, `AGENTS.md`, dieses Journal.
- **Test:** Nur Planungsdokumentation geändert; dotnet build EnergyTwin.sln --no-restore: 0 Fehler, 0 Warnungen. Keine Unit-Tests ausgeführt.
- **Comment:** Noch kein Skill implementiert; bestehender GetStatus-Entwurf bleibt nachvollziehbar als verworfen erhalten. Keine Änderung an readTask oder probelauf.

### Nachtrag 2026-11-08 – Let's Document: Umfang und Archivierung bestätigt

- **Done:** Erste Interviewentscheidungen in ADR-0015 festgehalten: gezielte betroffene READMEs, ausdrücklich auch eine einzelne README per Pfad; vollständiger Durchgang nur auf Auftrag. Alte Abschnitte vollständig unter docs/archive mit Herkunft, Datum und Grund erhalten. Unklare Aussagen markieren. Erklärungen und Dopplungen verdichten, Formeln/Registerpläne/Start/Bedienung/Einschränkungen vollständig erhalten. Begriffe Archivierung und Dokumentationslücke im bestehenden Glossar ergänzt.
- **KI:** Codex (GPT-6.1) => Interview und lokale Entscheidungsdokumentation.
- **Artefact:** lokale `docs/adr/0015-lets-document-skill.md`, `docs/glossary.md`, dieses Journal.
- **Test:** Planungsdokumentation geändert; dotnet build EnergyTwin.sln --no-restore: 0 Fehler, 0 Warnungen. Keine Unit-Tests ausgeführt.
- **Comment:** Skill noch nicht implementiert. Belegregeln, Widersprüche, Ausführungsgrenzen und Ablage noch abzustimmen.

### Nachtrag 2026-11-08 – Let's Document: Belege und gezielte User-Einbindung festgelegt

- **Done:** Zweite Interviewrunde in ADR-0015 festgehalten: belegbare Ergänzungen, fachliche Widersprüche offenlegen, User gezielt wie bei Grill Me einbinden; keine Tests/Programmstarts ohne Zustimmung. Allgemeine Wiederverwendbarkeit bestätigt. Gesamtentwurf um vorgeschlagene Ablage, Beispielaufrufe, Abschlussbericht und Prüfkriterien ergänzt.
- **KI:** Codex (GPT-6.1) => Interviewkonsolidierung und Gesamtentwurf.
- **Artefact:** lokale `docs/adr/0015-lets-document-skill.md`, dieses Journal.
- **Test:** Dokumentationsänderung; vorgeschriebener Solution-Build: 0 Fehler, 0 Warnungen. Keine Unit-Tests oder Simulatoren gestartet.
- **Comment:** Skill noch nicht erstellt. Gesamtentwurf wartet auf abschließende Bestätigung gemäß grilling; danach Erstellung und eng begrenzter Dokumentations-Beispielaufruf.

### Nachtrag 2026-11-08 – Let's Document erstellt, installiert und angewendet

- **Done:** Nach ausdrücklicher Freigabe den eigenen Skill Let's Document unter skills/lets-document erstellt: kurzer Einstieg, bedarfsweise Archivregeln, Codex-Metadaten und LB03-Beschreibung mit Aufruf/Ergebnis. Einzeldatei-/Modul-/Gesamtscope, belegte Ergänzungen, gezielte User-Fragen wie Grill Me, Schutz fachlicher Details und Zustimmungspflicht für Tests/Programmstarts umgesetzt. Maßgebliche Repository-Fassung lokal für Codex installiert; drei installierte Dateien bytegleich. Begrenzter PV-README-Aufruf durchgeführt: Einleitung aktualisiert, Anzeigehinweis im Ausgabeabschnitt gebündelt, vorhandenen datierten Empfangsnachweis mit Grenzen ergänzt; Originalausschnitte vorher archiviert. Skill in zentraler README registriert, ADR-0015 als bestätigt/erstellt markiert.
- **KI:** Codex (GPT-6.1) => Skill-Erstellung, formale/statische Prüfung, README-Beispiel und unabhängiger Dokumentationsdurchgang mit separatem Agenten auf isolierten Beispieldateien.
- **Artefact:** `skills/lets-document/` (SKILL.md, agents/openai.yaml, references/archive.md, README.md), lokale Installation, `docs/archive/2026-11-08-pv-readme.md`, `docs/nachweise/2026-11-08-lets-document/README.md`, PV-README, zentrale README, AGENTS, lokaler ADR-0015, dieses Journal.
- **Test:** Skill-Validator für Repository und Installation: Skill is valid. Drei Skill-Dateien bytegleich; 14 relative Dateiverweise statisch gültig. Alle vorhandenen PV-Code-/Formelblöcke unverändert. Isolierter Dokumentationsdurchgang: veraltete Option korrigiert/archiviert, Nachweisalter und neuen Testbestand getrennt, Formelkonflikt dem User vorgelegt, unbestätigten Plan erhalten und Einzeldateigrenze eingehalten. Keine Unit-Tests, Simulatoren oder Dienste gestartet; abschließender vorgeschriebener Solution-Build: 0 Fehler, 0 Warnungen.
- **Comment:** Der isolierte Formelkonflikt gehört ausschließlich zum synthetischen Beispiel, nicht zum EnergyTwin-Modell. Andere READMEs wurden im Einzeldatei-Beispiel nicht verändert; zentrale README/Skill-README sind Bestandteil des separaten Erstellungsauftrags. README im Skillordner dient der vorgeschriebenen LB03-Dokumentation und wird nicht als zusätzliche Skill-Anweisung installiert. Kein Commit/Push.

### Nachtrag 2026-11-08 – Graphmodell aktualisieren und Übergabe vorbereiten

- **Done:** Kompakte Übergabe mit Zuständigkeiten, umgesetzten Wetter-/PV-/Skill-Änderungen, Belegen, offenen Prüfungen, Startbefehlen und lokalen ADRs angelegt. Laufzustand rein lesend erfasst: Projektbroker healthy, keine Listener der Simulatoren/Master/Wetter auf den Standardports. AGENTS-Einstieg auf die aktuelle Übergabe ausgerichtet. Inkrementelle Graphaktualisierung mit portablen Quellpfaden und Integritätsprüfung wird im selben Arbeitsschritt abgeschlossen; Einzelheiten stehen anschließend in graphify-out/UPDATE_STATUS.json.
- **KI:** Codex (GPT-6.1) => Übergabedokumentation und Graphify-Aktualisierung.
- **Artefact:** `docs/handoff/2026-11-08-alexander-lb03.md`, `AGENTS.md`, `graphify-out/`, dieses Journal.
- **Test:** Statische Graph-/Quellenprüfung und vorgeschriebener Solution-Build im Übergabeschritt; keine Unit-Tests oder Simulatoren starten. Letzter dokumentierter Build 0 Fehler/0 Warnungen; erweiterter Wetter-Testbestand weiterhin nicht neu ausgeführt.
- **Comment:** Kein Commit/Push/Tag. Grafische Dateien sind generiert; Graph-Warnungen und verbleibende Einschränkungen werden im Aktualisierungsstatus dokumentiert. Ignorierte lokale ADRs/Glossar bei Rechnerwechsel separat verfügbar machen.

### Nachtrag 2026-11-08 – Lokale Agent- und Graphordner ignorieren

- **Done:** graphify-out, .claude und .opencode in .gitignore aufgenommen; vorhandene Dateien aus der Git-Verfolgung genommen, lokale Inhalte erhalten. probelauf samt Referenzen, Skript und Vorlage unverändert nach skills/probelauf kopiert; bestehende readTask-Kopie mit .claude bytegleich geprüft.
- **KI:** Codex => Repository-Ablage bereinigt.
- **Artefact:** .gitignore, skills/probelauf/, Git-Index, dieses Journal.
- **Test:** Sieben probelauf-Dateien bytegleich; readTask-Kopie bytegleich; Ignore-Regeln greifen und keine Dateien dieser drei Ordner mehr im Git-Index. Solution-Build erfolgreich: 0 Fehler, 0 Warnungen (erster eingeschränkter Lauf abgebrochen, Wiederholung erfolgreich). Keine Tests oder Programme gestartet.
- **Comment:** Keine Quelldateien der Simulatoren geändert; kein Commit/Push. Lokales Graphmodell bleibt nutzbar, bildet diese Ablageänderung noch nicht ab.

### Nachtrag 2026-11-08 – Graph-Ignore in GitHub Desktop korrigiert

- **Done:** Abweichende Ignore-Regel .graphify-out/ wieder auf /graphify-out/ korrigiert und erneut versionierte lokale Agent-/Graphordner aus dem Index genommen. Lokale Dateien bleiben erhalten.
- **KI:** Codex => Git-Status analysiert und Ignore-Verhalten korrigiert.
- **Artefact:** .gitignore, Git-Index, dieses Journal.
- **Test:** Ignore-Regeln und leeren Indexumfang für die drei Ordner geprüft. Keine Codeänderung, Tests oder Programmstarts.
- **Comment:** GitHub Desktop zeigt die Entfernung bisher versionierter Dateien einmalig bis zum nächsten Commit. Kein Commit/Push durch den Agenten.

### Nachtrag 2026-11-08 – Alle Projekt-READMEs mit Let's Document abgleichen

- **Done:** Nach ausdrücklicher Umfangsentscheidung alle 27 Projekt-READMEs in Root, docker, src, tests und skills gelesen und gezielt mit Code/Konfiguration/Nachweisen abgeglichen; 17 davon korrigiert oder ergänzt, bereits passende Inhalte erhalten. Konfiguration mit lokaler .env, tatsächliche Abhängigkeiten, Geräte- und Demoports, Standalone-Beispiele, Batterie-Einstellungen/CSV, fünf Wetterorte, Zustandspriorität/Rundung, MQTT-Nachweise und lokale Skill-Aktivierung aktualisiert. CSV- und Wetter-Dopplungen gebündelt. readTask-Beispiel als historisch gekennzeichnet. SimCore-Verträge und Tests nach Nutzerantwort fachlich zugeordnet: Alexander Wetter, Susanne Zeit/übrige Bausteine. SimCore.Tests enthält derzeit keine Wetterfälle. Letzte Testläufe von aktuellem Prüffallbestand getrennt; keine neue Laufbehauptung.
- **KI:** Codex => Let's Document, gezielte Zuständigkeitsfragen, Quellenvergleich, Archivierung und statische Prüfung.
- **Artefact:** 17 Projekt-READMEs sowie Navigationslinks im probelauf-Nachweis und Broker-Start in dessen README-Vorlage; docs/archive/2026-11-08-readme-abgleich.md mit 29 unveränderten Originalabschnitten und Herkunft/Grund/Ziel/Ausgangscommit; dieses Journal; AGENTS-Hinweis.
- **Test:** Relative Dateiverweise und Überschriften statisch geprüft; vorhandene Codeblöcke und Registerzeilen mit gesichertem Ausgangstext verglichen. Änderungen an vorhandenen Blöcken betreffen ausschließlich Projektbaum und zwei ungültige Startbeispiele; Formelblöcke erhalten. Registeränderungen ausschließlich Ergänzung Gerätetyp 5 in drei Kopflegenden. 33 README-Dateien statisch berücksichtigt (27 Projektbeschreibungen, 5 datierte Nachweise, 1 Vorlage), keine fehlenden relativen Linkziele oder Überschriften. Solution-Build erfolgreich: 0 Fehler, 0 Warnungen. Keine Unit-Tests, Probeläufe oder Dienste gestartet.
- **Comment:** docs/devices/DualPortCharger/design-notes.md fehlt im aktuellen Checkout; Verweis als fehlend gekennzeichnet, keine Verwerfung behauptet. Übrige offene Team-/LB03-Zuständigkeiten nicht erfunden. Lokaler Graph ist gegenüber dieser README-Pflege nicht neu indiziert. Bestehende Nachweisaussagen und Archivtexte bleiben datierte Belege; im probelauf-Nachweis nur Navigationslinks zur versionierten Skill-Ablage berichtigt. Keine neue Aufgabenbewertung. Kein Commit/Push.
