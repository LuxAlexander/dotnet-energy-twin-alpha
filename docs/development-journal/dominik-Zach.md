# Entwicklungsjournal – Dominik Nachname

## 2026-10-05 – Anforderungen, Brainstorming u Spezifikation

- **Done:** Aufgabenstellung gelesen; Funktionsumfang des Batteriespeichers per Frage-Antwort-Brainstorming festgelegt (28 Entscheidungen); Spezifikation u Projekt-Memory geschrieben.
- **KI:** Claude (Anthropic, claude.ai) => Brainstorming zu Funktionen, Architektur, Auto-Modus, Störungen, CSV u Bedienung; Spezifikation u Entscheidungsprotokoll von der KI formuliert, von mir geprüft u bestätigt. Interface für Geräte (`ISimulatedDevice`) verworfen, erst bei Kopplung; Threads bewusst vermieden.
- **Artefact:** Spezifikation „Batteriespeicher-Simulator“, Projekt-Memory (Entscheidungsprotokoll).
- **Test:** Keine Codetests; 10 Abnahmetests in der Spezifikation definiert.
- **Comment:**
  - Batterie als eigenständiges Gerät, PV u Ladestation bauen die Kollegen
  - Auto-Modus = Tagesprofil mit ±10 % Rauschen statt eigenem PV-Modell
  - Soll/Ist getrennt, Gerät begrenzt selbst (wie BMS)
  - Störungen: Not-Aus u Überhitzung; Pflicht: Zähler, Taper; optional: Wirkungsgrad, Selbstentladung, SOH
  - Next: Projekt anlegen, Batterie-Klasse

## 2026-10-06 – Batterie-Modell

- **Done:** `Enum.cs`, `BatterieConfig`, `Batterie` (SOC, Begrenzung, Taper, Temperatur, Derating, Störungen, Energie- u Zykluszähler) u `DailyProfile` (Tagesprofil mit Rauschen) geschrieben.
- **KI:** Claude => Erklärung der Konzepte u Code-Review, Code selbst geschrieben. Gefundene Fehler: Ergebnis von `Math.Clamp` nicht gespeichert; kWh mit kW verglichen (Division durch Stunden fehlte); Zähler mit `=` statt `+=`; Temperaturformel als Gleichgewicht statt Änderung pro Schritt; Überhitzungs-Trip setzte `ActiveFault` in jedem Schritt neu (Reset wirkungslos); Klammerung der Exp-Funktion im Entladeprofil; Rauschen wechselte jede Minute (Fensternummer wurde nicht gemerkt).
- **Artefact:** `Enum.cs`, `BatterieConfig.cs`, `Batterie.cs`, `DailyProfile.cs`, Screenshots der Konsolentests.
- **Test:** Konsolenschleifen: Laden 50→100 % mit Herunterregeln im letzten Schritt; Entladen bis 10 %; Taper ab 80 %; Derating (HeatFactor kurz auf 0,6); Not-Aus u Reset; Überhitzung: Fault, Cooldown, Freigabe bei ca. 50 °C; Zähler (5 kWh geladen = 0,25 Zyklen bei 10 kWh); Profilkurve in 0,5-h-Schritten.
- **Comment:**
  - Natürliche Überhitzung erreicht wegen Derating nie 60 °C, Störung nur manuell
  - Not-Aus setzt Sollwert 0
  - Zusatzwärme über eigenes Flag (`faultHeatActive`) von der Anzeige-Störung getrennt
  - Standardkapazität von 10 auf 20 kWh, damit das Tagesprofil besser passt
  - Next: Simulator mit Realzeit-Takt

## 2026-10-07 – Simulator, Bedienung, CSV u Dashboard

- **Done:** `Simulator` (Realzeit-Takt, Pause, Auto/Manuell, Tasten, Pausenmenü), `CsvLogger`, `ConsoleDashboard`, Kommandozeilenargumente, README.
- **KI:** Claude => Anleitung u Code-Review; `Program.cs` (Argumente) u README von der KI erstellt, von mir geprüft. Gefundene Fehler: Zeitkonto beschleunigte sich selbst (`lastRealSeconds` falsch benutzt); Pause holte die Pausenzeit nach; Operator-Vorrang `&&`/`||` bei der Schleifenbedingung; `=` statt `==` bei der E-Taste; Beschriftungen in den CSV-Datenzeilen; `internal` statt `public`; `NullReference` weil Dashboard nicht übergeben; fester SOC-Balken.
- **Artefact:** `Simulator.cs`, `CsvLogger.cs`, `ConsoleDashboard.cs`, `Program.cs`, `README.md`, CSV-Beispieldatei, Screenshots des Dashboards.
- **Test:** Manuell: 24-h-Lauf (nachts 0 kW, mittags voll, abends Entladen); Pause 20 s ohne Zeitsprung; Tasten M, Pfeile, 0, E, N, H, R, Q; Pausenmenü: Max-Ladeleistung 2 => Ist < Soll, SOC-Max 80, ungültige Eingaben; CSV: 13 Spalten, Punkt als Dezimalzeichen. Argumente u Mehrfachstart noch zu testen.
- **Comment:**
  - Pausenmenü mit `ReadLine`, keine Threads; Menüzeit wird nicht nachgeholt
  - Dezimalkomma im Menü (Landeseinstellung), Punkt in Argumenten u CSV
  - Offen: Zielframework auf .NET 10 prüfen, Ereignisliste u Zusammenfassung optional
  - Next: Argumente u Mehrfachstart testen, Kopplung mit PV u Ladestation abstimmen, optional Wirkungsgrad

## 2026-10-26 – Kühlhaus als viertes Feldgerät (LB02, Schritte 1–9)

- **Done:** Mein LB01-Projekt (Kühlhaus, `Domi/Verbraucher`) zum vierten LB02-Feldgerät ausgebaut, in neun Schritten: (1) Projekt in `EnergyTwin.sln` und Referenz auf `Devices.Common`; (2) Modell repariert (`SetDoor(bool)`, exakte Exponentiallösung statt Euler, validierte Solltemperatur −30…0 °C, `AlarmTempC`, `ResetFault`, `TurnOff`, Einheiten im Namen); (3) `DeviceType.IndustrialConsumer = 5` und `KuehlhausRegisters` (20–31: Solltemperatur, Tür, Ein/Aus, Reset); (4) `SimulationConfig`/`SimulationEngine` mit Master-Zeit (`StepScheduler`), festen Schritten, CSV je Lauf, Wetter je Modellstunde und Modbus in jedem Durchlauf, auch in der Pause; (5) `KuehlhausModbus` mit Schreibprüfung; (6) `WeatherClient` (HTTP, 5 s Timeout, deutsches Fehlerverhalten); (7) Automatik (`AutoModeController`, Türöffnungen nach Tageszeit) und lokales Tagesprofil; (8) Anzeige und Tasten (`ConsoleRenderer`, `InputHandler`: `T` Tür, `S` Solltemperatur, `F` Kompressorausfall, `R` Reset, `O` Ein/Aus, `A` Automatik, `P`/`<`/`>` nur standalone, `Q`); (9) 28 Tests und Geräte-README mit Registerplan. Dazu die Anleitung mit Code-Skizzen unter `docs/devices/KuehlhausSimulator/umsetzung-lb02.md`.
- **KI:** opencode (big-pickle) => Analyse meines LB01-Codes und des Charger-Musters, Anleitung, vollständige Umsetzung, Tests, Probeläufe, Dokumentation und Journal.
- **Artefact:** `src/Devices/KuehlhausSimulator/` (`Kuehlhaus.cs`, `KuehlhausConfig.cs`, `Enums.cs`, `Program.cs`, `Simulation/SimulationConfig.cs`, `Simulation/SimulationEngine.cs`, `Simulation/KuehlhausModbus.cs`, `Simulation/WeatherClient.cs`, `Simulation/AutoModeController.cs`, `IO/ArgsParser.cs`, `IO/CsvLogger.cs`, `IO/ConsoleRenderer.cs`, `IO/InputHandler.cs`, `README.md`), `src/Modbus.Contracts/KuehlhausRegisters.cs`, `src/Modbus.Contracts/RegisterPlans.cs`, `tests/FieldDevices.Tests/ConsumerTests.cs`, `EnergyTwin.sln`, `docs/devices/KuehlhausSimulator/umsetzung-lb02.md`, zentrale `README.md`, `AGENTS.md`.
- **Test:** `dotnet build EnergyTwin.sln` 0 Warnungen; `dotnet test tests/FieldDevices.Tests` 62/62 (34 PV/Batterie + 28 Kühlhaus), `SimCore.Tests` 54/54, `Devices.Common.Tests` 44/44, `ModbusDiagnosticClient.Tests` 28/28, `Weather.Tests` 11/11 (zusammen 199). Probeläufe: standalone 24 h mit 289 CSV-Zeilen, 20,7 kWh und 25 Schritten offener Tür; mit Master (x600) synchron, nach Master-Abbruch Zeitstatus 3; Wetter vom Wettersimulator übernommen (5,6 → 4,4 °C in der Nacht); Diagnose-Client: Solltemperatur −20 → −15 °C angenommen, `force 5` abgelehnt (Register 18 = 2, Adresse 20).
- **Comment:**
  - Fehler in der eigenen Anleitung: Der dort vorgeschlagene Reset (`return false`, solange der Raum zu warm ist) sperrt sich selbst aus, weil ohne Kompressor nichts kühlt; umgesetzt ist „Kompressorausfall immer quittieren, Übertemperatur-Alarm erst unter `AlarmTempC − AlarmHystereseK`“.
  - Die Startzeile zeigte „außen −20 °C“ (Konstruktorwert); die Engine setzt die Außentemperatur jetzt mit einem Nullschritt, bevor sie die erste CSV-Zeile schreibt.
  - Anzeige, Tasten und Eingabe der Solltemperatur sind noch nicht in einem echten Terminalfenster geprüft; bei umgeleiteter Ausgabe erscheinen zwei Zeilen je 5 s, Tasten werden ignoriert.
  - Nicht committet.

### Nachtrag 2026-10-26 – Team-Entscheidungen: KuehlhausSimulator, 1,5 kW, Port 1505

- **Done:** Die drei Team-Entscheidungen umgesetzt: (1) Projekt von `Verbraucher` in **`KuehlhausSimulator`** umbenannt (Ordner, `.csproj`, Namespace, Solution-Eintrag, Testprojekt-Referenz, Doku-Ordner `docs/devices/KuehlhausSimulator/`, Registerplan-Name in `KuehlhausRegisters.Plan`), damit es wie `PvSimulator`, `BatterySimulator` und `DualPortCharger` heißt; (2) `ZusatzwaermeKW` von 3 kW auf **1,5 kW** gesenkt (offene Tür: Gleichgewicht Außen + 30 K statt + 60 K, Zeitkonstante τ = 20 h unverändert); (3) Modbus-Standardport auf **1505** (1502 = PV/Charger, 1503 = Batterie, 1504 = Charger in der Demo). Nachgezogen: Geräte-README (Registerplan, Modelltabelle, „Bekannte Probleme“), zentrale `README.md` (Startbefehle und Porthinweis), `AGENTS.md` (§0.3, §0.4, §0.9, §3, §6), Diagnose-Client-README (Plan-Liste, Porttabelle mit Kühlhaus und `write Solltemperatur -15`), Anleitung `umsetzung-lb02.md`.
- **KI:** opencode (big-pickle) => Umbenennung, Anpassung aller Verweise und Texte, Testkorrektur, Build und Testlauf.
- **Artefact:** `src/Devices/KuehlhausSimulator/` (umbenannt), `EnergyTwin.sln`, `tests/FieldDevices.Tests/FieldDevices.Tests.csproj`, `src/Devices/KuehlhausSimulator/Simulation/SimulationConfig.cs`, `KuehlhausConfig.cs`, `src/Modbus.Contracts/KuehlhausRegisters.cs`, `tests/FieldDevices.Tests/ConsumerTests.cs`, `README.md`, `AGENTS.md`, `src/ModbusDiagnosticClient/README.md`, `docs/devices/KuehlhausSimulator/umsetzung-lb02.md`.
- **Test:** `dotnet build EnergyTwin.sln` 0 Warnungen; `dotnet test tests/FieldDevices.Tests` 62/62; `--register-plan` zeigt Port 1505, Gerätetyp 5 und den Schreibbereich −30…0 °C.
- **Comment:**
  - Der Hysterese-Test war auf den alten Türverlust kalibriert (er prüfte die Temperatur **nach** dem Schritt und erlaubte 1,9 K Überschwingen; bei 1,5 kW sind es nur noch 1,8 K). Er prüft jetzt die Temperatur **vor** dem Schritt, also den tatsächlichen Schaltpunkt, und ist damit vom Türverlust unabhängig.
  - Die Umbenennung erfolgte mit `Rename-Item`, nicht mit `git mv`; in Git erscheinen die Dateien deshalb als gelöscht und neu.
  - Nicht committet.

### Nachtrag 2026-10-26 – Kühlhaus auf die gemeinsamen Bausteine umgestellt

- **Done:** Als letztes Feldgerät hatte mein Kühlhaus noch eigene Kopien der Wetterlogik. Jetzt nutzt es – wie PV, Batterie und Ladesäule – `Devices.Common` und `Shared`: eigener `Simulation/WeatherClient.cs` gelöscht (80 Zeilen, inklusive zweiter `WeatherUnavailableException`), stattdessen `WeatherClient`/`WeatherFeed`/`WeatherOptions` aus `Devices.Common` (Abfrage höchstens je Modellstunde, Retry 10 s, letztes Wetter bleibt, Meldung nach 3 Fehlversuchen), `--weather`/`--location` über `WeatherOptions.TryApply` mit `WithDefaultUrl(standalone)`, und der CSV-Grundwriter `Shared.Data.CsvFileWriter` (Datei, Kopfzeile, BOM, Maskierung) statt eigenem `StreamWriter`. Entfallen sind `_lastWeather`, `_weatherFailures`, `_nextWeatherAttempt`, `_weatherErrorReported`, `RetryInterval`, `German(...)`, `CurrentOutsideTemperature` sowie `DefaultWeatherUrl`, `WeatherUrl` und `Location` in `SimulationConfig`; die Engine holt die Außentemperatur nur noch über eine Methode. Rund 170 Zeilen Duplikat weniger, keine Zeile Fachlogik verloren, Bedienung unverändert.
- **KI:** opencode (big-pickle) => Analyse der Doppelungen, Umstellung auf die gemeinsamen Bausteine, Anpassung von Tests und Doku, Build und Probelauf.
- **Artefact:** `src/Devices/KuehlhausSimulator/KuehlhausSimulator.csproj` (`ProjectReference Shared`), `Simulation/SimulationConfig.cs`, `Simulation/SimulationEngine.cs`, `IO/ArgsParser.cs`, `IO/CsvLogger.cs`; gelöscht: `Simulation/WeatherClient.cs`; `tests/FieldDevices.Tests/ConsumerTests.cs`; `README.md` (Gerät), zentrale `README.md`, `AGENTS.md` (§0.3, §0.9, §0.10, §0.11, §0.14), Anleitung `docs/devices/KuehlhausSimulator/umsetzung-lb02.md`.
- **Test:** `dotnet build EnergyTwin.sln` 0 Warnungen; `dotnet test tests/FieldDevices.Tests` 62/62. Probelauf mit Wettersimulator: Kühlhaus standalone mit `--weather http://127.0.0.1:8080`, Spalte `Aussenquelle` = `Wettersimulator`, Außentemperatur 5,6 → 4,9 °C, genau eine Abfrage je Modellstunde; `--weather off` bleibt beim lokalen Tagesprofil (`WeatherStatus` = „aus (--weather off), lokales Tagesprofil“); `--help` zeigt den gemeinsamen Wetterblock.
- **Comment:**
  - Einzige sichtbare Änderung: Die Fehlerzeile sagt bei fehlender Antwort jetzt wie bei den anderen Geräten „… – n. Fehlversuch, letztes Wetter bleibt“ statt „(lokales Profil)“; die Startzeile nennt den Standort („warte auf salzburg …“) statt der URL.
  - Der zweite Typ `WeatherUnavailableException` ist damit weg – es gilt nur noch der aus `Devices.Common.Weather`.
  - Die 16 CSV-Spalten und das de-AT-Zahlenformat bleiben unverändert, damit das Abgabeformat gleich ist.
  - Nicht committet.

## 2026-11-08 – Skill `probelauf` (eigener Skill, Aufgabe 03 A16)

- **Done:** Eigenen Skill für reproduzierbare Funktionsnachweise angelegt, nach der Repo-Konvention wie `readTask` (versioniert unter `skills/`, Aktivierung über `.claude/skills/`). Inhalt: `SKILL.md` mit Aufruf, drei Umfängen (A Gerät, B LB02-Demo, C LB03-Kette), Pflichtlektüre, Regeln vor dem Start, 7-Schritt-Ablauf, Kurz-Prüftabelle und Grenzen; `references/ports.md` (Portmatrix 8080/5100/1502–1505/1883/15672/5672, Startbefehle, Standardwerte); `references/abnahmekriterien.md` (A1–A8, Z1–Z11, N1–N7 aus Aufgabe 03, Registerkopf 0–19, Qualitätswerte); `references/fehlerbilder.md` (17 Fälle mit Ursache/Abhilfe); `templates/nachweis-README.md`; `scripts/pruefe-ports.ps1` (Portcheck, nur lesend). Dazu `README.md` mit Feldtabelle, Nutzen, belegtem Beispielaufruf, Aktivierung, Dateiliste, Quelle und bekannten Grenzen.
- **KI:** opencode (Big Pickle) => Analyse der vorhandenen Nachweise und Stolperfallen, Skill-Texte, Portcheck-Skript, Selbsttest und Nachweisordner.
- **Artefact:** `skills/probelauf/` (`SKILL.md`, `README.md`, `references/ports.md`, `references/abnahmekriterien.md`, `references/fehlerbilder.md`, `templates/nachweis-README.md`, `scripts/pruefe-ports.ps1`), identische Kopie in `.claude/skills/probelauf/`, Nachweis `docs/nachweise/2026-11-08-probelauf-skill/` (`README.md`, `ablauf.txt`, `kuehlhaus.txt`, `diagnose-01-read.txt`, `diagnose-02-write.txt`, `diagnose-03-read.txt`).
- **Test:** `dotnet build EnergyTwin.sln` 0 Fehler/0 Warnungen. `pruefe-ports.ps1` vor und nach dem Lauf: alle Ports frei. Selbsttest Umfang A: Kühlhaus KH-01 standalone, `--speed 600 --duration 8 --weather off --seed 42 --modbus-port 1505`; Port nach 1,2 s offen; `--once` liest Kopf 0–19 (Typ 5, Zeitstatus 2, Tempo 600) und Register 20–31; `--write "Solltemperatur=-15"` wird angenommen, Gegenlesung zeigt −15 °C und Register 19 = 20; Gerät endet nach 8 Modellstunden mit CSV, keine Restprozesse. Testprojekte nicht gestartet (kein Commit geplant, Projektregel in `AGENTS.md` §0.1).
- **Comment:**
  - Die ersten Protokolle waren unbrauchbar: PowerShell-Redirection mit `*>` schreibt UTF-16 und ersetzt Umlaute durch `?`. Mit `Start-Process -RedirectStandardOutput` bleiben die UTF-8-Bytes des Programms unverändert – als Fehlerbild 17 im Skill vermerkt, der Lauf wurde damit wiederholt.
  - `Start-Process -PassThru` lieferte nach `WaitForExit` keinen Exit-Code; der Nachweis stützt sich auf Ausgaben, Portverhalten und Zeitmarken.
  - Der Skill kann Anzeige, Tasten und Strg+C nicht abnehmen; das steht im Nachweis als „nicht geprüft“, ebenso Umfang B und C.
  - `ExitCode`- und Encoding-Eigenheiten sowie die Pflicht zu `.txt`-Protokollen (wegen `*.log` in `.gitignore`) sind im Skill-README dokumentiert.
  - OpenCode hat den Skill nach dem Anlegen selbst erkannt (Eintrag in der Skill-Liste), ohne Neustart.
  - Nicht committet.

### Nachtrag 2026-11-08 – Skill nach `.opencode/skills/` umgehängt (Agenten getrennt)

- **Done:** Der Skill liegt jetzt nur noch in **`.opencode/skills/probelauf/`**; die Kopien in `skills/probelauf/` und `.claude/skills/probelauf/` sind entfallen (Team-Entscheidung: Agenten getrennt halten, damit sich Arbeitsweisen und Widersprüche nicht vermischen). Korrigiert: Aufruf ist **`@probelauf <Thema>`** statt `/probelauf` – OpenCode lädt Skills über die ID (aus dem Ordnernamen), das Skill-Werkzeug kennt keine Parameter, und ein Slash-Command wäre ein eigener Command in `.opencode/commands/`, der bewusst nicht angelegt wurde; Skriptpfad `powershell -File .opencode/skills/probelauf/scripts/pruefe-ports.ps1`; Abschnitt „Ablage und Aktivierung“ im README neu (ID aus Ordnernamen, automatische Erkennung, kein Kopieren und kein Neustart, `.opencode/skills/` hat Vorrang vor `.claude/skills/`); relativer Link auf den Nachweis (eine Ordnerebene tiefer) und die Verweise im Nachweis-README.
- **KI:** opencode (Big Pickle) => Pfade und Aufruftexte angepasst, Duplikat entfernt, Links und Reststellen geprüft.
- **Artefact:** `.opencode/skills/probelauf/` (`SKILL.md`, `README.md`, `references/ports.md`, `references/abnahmekriterien.md`, `references/fehlerbilder.md`, `templates/nachweis-README.md`, `scripts/pruefe-ports.ps1`), `docs/nachweise/2026-11-08-probelauf-skill/README.md`; gelöscht: `skills/probelauf/`, `.claude/skills/probelauf/`.
- **Test:** `git grep "skills/probelauf"` ohne Treffer außer diesem Journal; relative Links im Skill-README und im Nachweis auf Existenz geprüft; OpenCode hat die geänderte `SKILL.md` selbst neu eingelesen (Skill-Liste zeigt die neue Beschreibung mit `@probelauf`) – kein Neustart nötig. Build und Tests nicht nötig (nur Markdown und ein Skriptkommentar).
- **Comment:**
  - `skills/` enthält jetzt nur noch `readTask`; für diesen Skill gibt es genau eine Ablage.
  - Claude Code sieht diesen Skill nicht mehr (`.claude/skills/` enthält nur `readTask`). Umgekehrt liest OpenCode `.claude/skills/` als Kompatibilitätsquelle und zeigt `readTask` weiterhin an; echte Trennung ginge nur über `permissions` (`skill`/`readTask`/`deny`) oder durch Verschieben von `readTask` – nicht beauftragt.
  - Nicht committet.
