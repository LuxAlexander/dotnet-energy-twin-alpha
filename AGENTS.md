# AGENTS.md – EnergyTwin

## Kontext und Stand

Team Alpha entwickelt ein lokales, verteiltes Energiesystem. Ein Master steuert die virtuelle Zeit; ein Wettersimulator liefert reproduzierbare Wetterdaten. PV-Anlage, Batteriespeicher, DC-Ladesäule und Kühlhaus laufen als getrennte Feldgeräte. Ein Diagnose-Client liest und schreibt Modbus-Register. Gateway und Wettersimulator veröffentlichen Daten über MQTT an RabbitMQ; MQTT Explorer dient als Viewer. Die Programme und ihre Schnittstellen sind umgesetzt. Die vollständige interaktive Vorführung bleibt zu prüfen.

Die zentrale [README](README.md) enthält Architektur, Startreihenfolge und Zuständigkeiten. Die README jedes Projekts ist maßgeblich für Bedienung, Konfiguration, Modellformeln und Schnittstellen. Aktuelle Aufgaben und offene Punkte stehen in [docs/task-status](docs/task-status/); Nachweise in [docs/nachweise](docs/nachweise/).

## Was fertig ist

| Bereich | Stand | Details |
|---|---|---|
| Virtuelle Zeit | Master, Zeitverteilung und Geräteanbindung umgesetzt | [Zeit](src/SimCore.Time/README.md), [Master](src/SimulationMaster/README.md) |
| Wetter | Lokaler HTTP-Dienst; MQTT optional | [Wettersimulator](src/WeatherSimulator/README.md) |
| Feldgeräte | PV, Batterie, Ladesäule und Kühlhaus an Zeit, Wetter und Modbus angebunden | Geräte-READMEs im Projektbaum |
| Modbus | Registerpläne, Geräte-Server und Diagnose-Client umgesetzt | [Verträge](src/Modbus.Contracts/README.md), [Diagnose](src/ModbusDiagnosticClient/README.md) |
| MQTT | Gateway und RabbitMQ-Broker eingerichtet | [Gateway](src/ModbusRabbitGateway/README.md), [Broker](docker/README.md) |
| Viewer | MQTT Explorer eingerichtet und mit Geräte- und Wetterdaten geprüft | [Anleitung](docs/viewer/README.md) |
| Weboberfläche | Blazor mit gruppiertem Instanzstart und Statusampeln auf der Startseite, Live-Karten unter „Short Info“ und Master-Steuerung | [Weboberfläche](src/EnergyTwin.Web/README.md) |

## Arbeitsregeln für Agenten

- Diese Datei vor Arbeiten im Repository lesen. Für Modell, Formeln und Bedienung gelten die jeweiligen Projekt-READMEs; Originalquellen sind maßgeblich, der lokale Graph dient nur zur Navigation.
- Antworten, Dokumentation, Kommentare und UI auf Deutsch. Bezeichner im neuen Code grundsätzlich englisch.
- Kein Internet zur Laufzeit. Wetter lokal und deterministisch berechnen. Keine Zugangsdaten ins Repository; `docker/.env` bleibt lokal, `docker/.env.example` zeigt die Konfiguration. Die Aufgaben-PDF liegt nur lokal im Repo-Root und ist ignoriert; Anforderungen stehen in der [SRS](docs/SRS-LB03.md) und den Aufgabenständen.
- Asynchrone Abläufe mit `async/await`, `CancellationToken` und `Async`-Suffix. Kein `Thread.Sleep`, `.Wait()` oder `.Result`. Die synchrone EasyModbus-Bibliothek ist die akzeptierte Grenze; sie nicht in `Task.Run` verpacken.
- Gerätemodelle enthalten nur ihre spezifische Logik. Gemeinsame Zeit-, Modbus- und Wetteranbindung gehört nach `src/Devices.Common`, allgemeine CSV-, Konsolen- und Wetterbausteine nach `src/Shared`.
- Jede größere Änderung automatisch im Entwicklungsjournal der zuständigen Person dokumentieren; nur deren Journal bearbeiten. Kleine README-, Dokumentations- und Regelkorrekturen benötigen keinen eigenen Journaleintrag. Pro Tag ein Eintrag `## JJJJ-MM-TT – Titel`, weitere Arbeiten als `### Nachtrag JJJJ-MM-TT – Titel`. Felder: `**Done:**`, `**KI:**`, `**Artefact:**`, `**Test:**`, `**Comment:**`. Für Arbeiten an Alexanders Bereich: [alexander-lux.md](docs/development-journal/alexander-lux.md).
- Builds nach fachlichem Bedarf ausführen: Bei reinen Dokumentationsänderungen oder minimalen Änderungen ohne relevante Build-Auswirkungen keinen Build starten, solange kein Commit oder Push ansteht. Ohne Änderungen ist ebenfalls kein Build nötig. Bei buildrelevanten Änderungen sowie vor einem Commit oder Push `dotnet build EnergyTwin.sln` ausführen: null Fehler und null Warnungen. Ein bereits erfolgreicher Build muss ohne weitere buildrelevante Änderungen nicht wiederholt werden. Betroffene READMEs, diese Datei und bei größeren Änderungen das Journal prüfen und nötige Änderungen eintragen. Unit-Tests nur vor einem Commit oder auf ausdrücklichen Wunsch; Testprojekte wegen gemeinsam genutzter TCP-Ports einzeln starten. Ausgeführte und lediglich vorhandene Prüfungen unterscheiden.
- Vor dem Öffnen echter Konsolenfenster oder dem Beenden fremder Prozesse um Erlaubnis fragen. Fremde und lokale Artefakte nicht pauschal bereinigen.
- Commits nur auf Anweisung. Eine vorgegebene Commit-Message wörtlich übernehmen, ohne `Co-Authored-By`; nur auf ausdrücklichen Wunsch pushen.

## Projektbaum

```text
EnergyTwin.sln            gemeinsame Solution
README.md                 Architektur, Start, Zuständigkeiten
start-all.ps1             lokaler Start der Programme
src/
  EnergyTwin.Web/         lokale Blazor-Oberfläche (Instanzstart, MQTT und Master-Steuerung)
  SimCore.Contracts/      Zeit- und Wetterverträge
  SimCore.Time/           virtuelle Zeit und Uhrenabgleich
  SimCore.TimeSync/       TCP-Zeitverteilung
  SimulationMaster/       Steuerung der virtuellen Zeit
  WeatherSimulator/       Wetter über HTTP, optional MQTT
  Modbus.Contracts/       Registerpläne und Codec
  Modbus.Client/          Modbus-Client für Diagnose und Gateway
  ModbusDiagnosticClient/ Register lesen und schreiben
  ModbusRabbitGateway/    Modbus-Werte über MQTT veröffentlichen
  Devices.Common/         gemeinsame Geräteanbindung
  Shared.Config/          Pfade und MQTT-Konfiguration
  Shared/                 CSV-, Konsolen- und Wetterbausteine
  Devices/
    PvSimulator/          PV-Anlage
    BatterySimulator/     Batteriespeicher
    DualPortCharger/      DC-Ladesäule
    KuehlhausSimulator/   Industrieverbraucher
tests/                    Testprojekte mit eigenen READMEs
docker/                   RabbitMQ-Compose und Beispielkonfiguration
docs/
  data/                   CSV-Ausgaben und Beispiele
  development-journal/    Arbeitsprotokolle je Person
  devices/                ergänzende Gerätedokumentation
  nachweise/              dokumentierte Probeläufe
  task-status/            Aufgabenstand
  viewer/                 MQTT-Explorer-Anleitung
skills/                   lokale Arbeits-Skills
```

## Wo die Details stehen

- Zeit und Synchronisation: [SimCore.Time](src/SimCore.Time/README.md), [SimCore.TimeSync](src/SimCore.TimeSync/README.md), [SimulationMaster](src/SimulationMaster/README.md).
- Wetter und MQTT: [WeatherSimulator](src/WeatherSimulator/README.md), [Wetter-Nachrichtenvertrag](docs/Wetter-Nachrichtenvertrag-LB03.md).
- Modbus und Geräteanbindung: [Modbus.Contracts](src/Modbus.Contracts/README.md), [Devices.Common](src/Devices.Common/README.md). Die Registerpläne stehen in den jeweiligen Geräte-READMEs.
- Gateway und Broker: [ModbusRabbitGateway](src/ModbusRabbitGateway/README.md), [Nachrichtenvertrag](docs/Nachrichtenvertrag-LB03.md), [docker](docker/README.md).
- Viewer: [Viewer-Anleitung](docs/viewer/README.md). Anforderungen: [SRS](docs/SRS-LB03.md). Historische Entscheidungen und frühere Stände: [docs/archive](docs/archive/) und [docs/development-journal](docs/development-journal/).

