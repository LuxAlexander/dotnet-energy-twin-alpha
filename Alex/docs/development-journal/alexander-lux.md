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