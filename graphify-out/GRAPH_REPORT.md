# Graph Report - dotnet-energy-twin-alpha  (2026-11-08)

## Corpus Check
- 228 files · ~342,832 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 7 file(s) not represented in the graph (top: .props 3, (none) 2, .csv 2)

## Summary
- 2088 nodes · 4967 edges · 132 communities (109 shown, 23 thin omitted)
- Extraction: 86% EXTRACTED · 14% INFERRED · 0% AMBIGUOUS · INFERRED: 692 edges (avg confidence: 0.79)
- Token cost: 227,722 input · 0 output

## Community Hubs (Navigation)
- MQTT-Publisher & Zugangsdaten
- Zeitsync-Client & Konsole
- Sync-Nachrichten
- Devices.Common Tests
- WeatherClient & Anzeige
- Batterie-Simulator Loop
- Diagnose-Client Verbindung
- Shared Konsolenausgabe
- Batterie-Modell
- Batterie-Modbus & Update
- Geräte-READMEs & Modelle
- Zeit-Bibliotheken & Master
- MasterClock
- Batterie-Modbus & Update (2)
- Gateway-Optionen
- Nachrichtenvertrag & Messwerte
- Modbus-Tests
- Batterie-Simulator Loop (2)
- WeatherFeed
- Gruppe 19
- Aufgabenabgleich (readTask)
- Gruppe 21
- PvFaultModel
- Gruppe 23
- Gruppe 24
- PV-Engine & Fehler
- TimeSyncServer
- Ladepunkt
- Kühlhaus-Modell
- Registerpläne
- MessageBuilder-Tests
- Aufgabe 03 & Nachweise
- Nachrichtentypen (Contract)
- PvSystem
- Sync-Nachrichten (2)
- Schreibprüfung
- RegisterCodec & Befehle
- Gruppe 37
- Gruppe 38
- SyncedClock
- TimeState & TimeLine
- DevicePoller & Gerätetests
- DeviceConnection (Modbus.Client)
- Gateway-Publisher
- Gruppe 44
- Fahrzeug
- DualCharger
- RegisterCodec & Befehle (2)
- Gruppe 48
- PV-Namespaces
- JSON-Serialisierung
- Gruppe 51
- Gruppe 52
- DeviceTimeSource
- Messwert-Schlüssel
- Funktionsnachweis mit Master
- Gruppe 56
- Gruppe 57
- StepScheduler
- SRS Gateway-Anforderungen
- DeviceTimeSource (2)
- Gruppe 61
- GatewayService
- Bibliotheks-Namespaces
- Diagnose-Client & Modbus.Client
- Gruppe 65
- Gruppe 66
- Gruppe 67
- Gruppe 68
- Gruppe 69
- Batterie-Modbus & Update (3)
- Gruppe 71
- Gruppe 72
- Batterie-Modbus & Update (4)
- ArgsParser
- Wettermodell & Tests
- Status-Wiederholung & RabbitMQ
- Wettermodell & Tests (2)
- Charger-Namespaces
- Wettermodell & Tests (3)
- Wettermodell & Tests (4)
- Gruppe 81
- Gruppe 82
- Gruppe 83
- Gruppe 84
- RabbitMQ unter Docker
- Gruppe 86
- DeviceEndpoint
- Gruppe 88
- Gruppe 89
- LB03-Projektstand
- Gruppe 91
- Qualitätsstatus & Pause
- Modbus.Client-Fehler
- Wettermodell & Tests (5)
- MessageBuilder
- Gruppe 96
- Batterie-Simulator Loop (3)
- Gruppe 98
- Gruppe 99
- Zuständigkeiten LB03
- Gruppe 101
- Gruppe 102
- OutputPaths
- Gruppe 104
- Gruppe 105
- Gruppe 106
- StepScheduler (2)
- Gruppe 108
- Gruppe 109
- ArgsParser (2)
- Gruppe 111
- Gruppe 112
- Gruppe 113
- Gruppe 114
- Gruppe 115
- Gruppe 116
- Gruppe 117
- Projekt-Doku & Geräte
- Gruppe 119
- Fehlerverhalten Gateway
- readTask-Arbeitsschritte
- Gruppe 122
- Projekt-Doku & Geräte (2)
- Projekt-Doku & Geräte (3)
- Projekt-Doku & Geräte (4)
- Gruppe 126
- Projekt-Doku & Geräte (5)
- Projekt-Doku & Geräte (6)
- Gruppe 129
- Projekt-Doku & Geräte (7)
- Projekt-Doku & Geräte (8)

## God Nodes (most connected - your core abstractions)
1. `MasterClock` - 52 edges
2. `SyncedClock` - 44 edges
3. `SimulationEngine` - 44 edges
4. `ModbusDeviceServer` - 42 edges
5. `SimulationEngine` - 42 edges
6. `Simulator` - 40 edges
7. `TimeSyncServer` - 39 edges
8. `SimulationEngine` - 39 edges
9. `SRS LB03 - Von Modbus zu RabbitMQ` - 38 edges
10. `DeviceTimeSource` - 36 edges

## Surprising Connections (you probably didn't know these)
- `Beobachtung: einmal verlorener Sollwert (Ursache ungeklärt, 410 Versuche ohne Wiederholung)` --conceptually_related_to--> `ModbusDeviceServer`  [AMBIGUOUS]
  docs/nachweise/2026-11-08-gateway-mit-master/README.md → src/Devices.Common/Modbus/ModbusDeviceServer.cs
- `Befund: einmal verlorener Sollwert, in 410 Wiederholungen nicht reproduzierbar` --references--> `ModbusDeviceServer`  [AMBIGUOUS]
  docs/development-journal/susanne-lastowicka.md → src/Devices.Common/Modbus/ModbusDeviceServer.cs
- `Befund: einmal verlorener Sollwert, in 410 Wiederholungen nicht reproduzierbar` --references--> `DualCharger`  [AMBIGUOUS]
  docs/development-journal/susanne-lastowicka.md → src/Devices/DualPortCharger/Models/DualCharger.cs
- `Beobachtung: Neustart der Ladesäule setzt Leistungsgrenze auf 150 kW zurück` --conceptually_related_to--> `DualCharger`  [INFERRED]
  docs/nachweise/2026-11-08-gateway-mit-master/README.md → src/Devices/DualPortCharger/Models/DualCharger.cs
- `AK-12: Diagnose-Client funktioniert weiter` --references--> `DiagnosticSession`  [INFERRED]
  docs/SRS-LB03.md → src/ModbusDiagnosticClient/DiagnosticSession.cs

## Import Cycles
- None detected.

## Communities (132 total, 23 thin omitted)

### Community 0 - "MQTT-Publisher & Zugangsdaten"
Cohesion: 0.07
Nodes (17): ModbusRabbitGateway.Tests (52 Tests), MQTTnet 5.0.1 (+ MQTTnet.Server als Test-Broker), Bug: MQTTnet 5 meldet abgelehnte Anmeldung per Ergebniscode statt Exception (behoben), MQTT-Broker im Testprozess (MQTTnet.Server, kein Docker), Broker-Ausfall: einmal gemeldet, Backoff 1–30 s, Messwerte verworfen, Status nachgeholt, Last Will, Zugangsdaten-Reihenfolge: --mqtt-user → Umgebungsvariablen → docker/.env; Passwort nie als Option, Gateway-Konfiguration (--device, --interval, --broker, --gateway-id ...), MqttCredentials (+9 more)

### Community 1 - "Zeitsync-Client & Konsole"
Cohesion: 0.07
Nodes (20): KeyResult, Handled, Quit, MasterConsole, HasKeyboard, IsInteractive, Width, PromptKind (+12 more)

### Community 2 - "Sync-Nachrichten"
Cohesion: 0.09
Nodes (16): ByeMessage, HelloMessage, PingMessage, PongMessage, StatesMessage, SyncMessage, SyncProtocol, ClientConnection (+8 more)

### Community 3 - "Devices.Common Tests"
Cohesion: 0.06
Nodes (44): DeviceTimeSourceTests, ModbusDeviceServerTests, Devices.Common.Tests, RegisterCodecTests, StepSchedulerTests, TimeSyncOptionsTests, ClientOptionsTests, DiagnosticClientTests (+36 more)

### Community 4 - "WeatherClient & Anzeige"
Cohesion: 0.08
Nodes (20): ConsoleRenderer, IsInteractive, Width, InputHandler, Program, SimulationEngine, Config, DoorClosesAt (+12 more)

### Community 5 - "Batterie-Simulator Loop"
Cohesion: 0.11
Nodes (6): CsvLogger, FilePath, WeatherUnavailableException, CsvLogger, FilePath, WeatherData

### Community 6 - "Diagnose-Client Verbindung"
Cohesion: 0.11
Nodes (4): DiagnosticSession, Connection, Plan, DiagnosticClientTests

### Community 7 - "Shared Konsolenausgabe"
Cohesion: 0.10
Nodes (8): Shared.Output, ConsoleDisplay, SimulationMode, Auto, Manual, Paused, BarChart, ConsoleText

### Community 8 - "Batterie-Modell"
Cohesion: 0.09
Nodes (18): Batterie, ActiveFault, ActualKW, AmbientTemperatureC, ChargedKWh, Config, DeviceId, DischargeKWh (+10 more)

### Community 9 - "Batterie-Modbus & Update"
Cohesion: 0.11
Nodes (14): HeaderValues, ModbusDeviceServer, ClientCount, IsListening, LastWriteText, Options, Plan, StatusText (+6 more)

### Community 10 - "Geräte-READMEs & Modelle"
Cohesion: 0.08
Nodes (31): BatterySimulator, Batterie Kommandozeilenargumente, SOC-Modell Batteriespeicher, Charger Kommandozeilenargumente, DualPortCharger, Gemeinsames Leistungsbudget der Ladepunkte, KuehlhausSimulator, Kuehlhaus Registerplan 20-31 (+23 more)

### Community 11 - "Zeit-Bibliotheken & Master"
Cohesion: 0.13
Nodes (7): SimCore.TimeSync, SimCore.Tests, SimCore.Time, Devices.Common.Tests, SimulationMaster, SimCore.Contracts, Program

### Community 12 - "MasterClock"
Cohesion: 0.15
Nodes (11): MasterClock, Current, CurrentStates, Latest, LeadTime, MasterNow, Mode, Now (+3 more)

### Community 13 - "Batterie-Modbus & Update (2)"
Cohesion: 0.19
Nodes (4): KuehlhausModbusTests, BatteryModbusTests, ModbusTestBase, Options

### Community 14 - "Gateway-Optionen"
Cohesion: 0.09
Nodes (18): GatewayOptions, BrokerHost, BrokerPort, ConsoleOnly, DefaultDevices, Devices, EnvFile, GatewayId (+10 more)

### Community 15 - "Nachrichtenvertrag & Messwerte"
Cohesion: 0.10
Nodes (26): Nachrichtenformat: DeviceId, virtuelle Zeit, Sequenz, Qualitaet, Messwerte, Virtuelle Zeit vs. reale Abfragezeit nicht verwechseln, AK-04: Messwertnachricht enthaelt DeviceId, Zeit, Sequenz, Qualitaet, Einheit, FA-GW-05: simTime aus Geraet, polledAtUtc getrennt, FA-VW-01: Empfang aller Geraete ueber RabbitMQ, Abo energytwin/#, RabbitMQ: Exchange amq.topic, Routing Key mit Punkt, Beispielnachricht DC-01, Nachrichtenvertrag LB03 (Schemaversion 1) (+18 more)

### Community 16 - "Modbus-Tests"
Cohesion: 0.14
Nodes (4): ModbusDiagnosticClient.Tests, Devices.Common.Modbus, Modbus.Contracts, PvEngineTests

### Community 17 - "Batterie-Simulator Loop (2)"
Cohesion: 0.13
Nodes (6): CsvLogger, FilePath, CsvExporter, FilePath, CsvFileWriter, FilePath

### Community 18 - "WeatherFeed"
Cohesion: 0.13
Nodes (13): WeatherFeed, Last, Location, Status, WeatherOptions, Location, Specified, Url (+5 more)

### Community 20 - "Aufgabenabgleich (readTask)"
Cohesion: 0.09
Nodes (25): Aufgabenabgleich Aufgabe 03 – Von Modbus zu RabbitMQ (Stand 2026-11-08), Hinweise: rabbitmq.conf leerer Ordner, Passwort im Repo, graphify-out versioniert, AGENTS.md veraltet, Vorgeschlagene Reihenfolge: SRS → Broker → Gateway → Publisher/Robustheit → Viewer → Skills → Doku → Nachweise → Abgabe, A1 Datenweg Gerät → Modbus → Gateway → RabbitMQ → Viewer (nicht erfüllt), A12 Nachrichtenvertrag mit Topics und Beispielnachricht (nicht erfüllt), A13 Virtuelle Zeit und Abfragezeit getrennt (nicht erfüllt), A14 Viewer zeigt alle Geräte (nicht erfüllt), A15 AGENTS.md aktuell, Regeln für generierte Dateien (teilweise) (+17 more)

### Community 21 - "Gruppe 21"
Cohesion: 0.10
Nodes (17): ConsoleDashboard, BelowStatus, IsInteractive, ControlMode, Auto, Manual, Simulator, IsFinished (+9 more)

### Community 22 - "PvFaultModel"
Cohesion: 0.11
Nodes (12): PvFaultModel, ActiveFault, AgingLossPercent, AmbientTemperatureC, GridDisturbanceForced, GridVoltageV, HasFault, InverterFault (+4 more)

### Community 24 - "Gruppe 24"
Cohesion: 0.10
Nodes (20): BatterieConfig, CapacityKWh, DeviceId, Efficiency, MaxChargeKW, MaxDischargeKW, SocMaxPercent, SocMinPercent (+12 more)

### Community 25 - "PV-Engine & Fehler"
Cohesion: 0.16
Nodes (9): Note, SimulationEngine, ModbusStatus, Mode, Pv, RunId, SimulationTime, Time (+1 more)

### Community 26 - "TimeSyncServer"
Cohesion: 0.20
Nodes (3): AsyncConsole, HasKeyboard, TimeSyncIntegrationTests

### Community 27 - "Ladepunkt"
Cohesion: 0.12
Nodes (11): ChargingPoint, AmbientTemperatureC, DeratingFactor, Efficiency, EnergyKWh, IsFree, Number, PowerKW (+3 more)

### Community 28 - "Kühlhaus-Modell"
Cohesion: 0.11
Nodes (16): Kuehlhaus, ActiveFault, AlarmTempC, CompressorOn, CoolingPowerKW, DoorOpen, ElectricPowerKW, EnergyKWh (+8 more)

### Community 29 - "Registerpläne"
Cohesion: 0.10
Nodes (17): BatteryRegisters, Plan, DeviceType, Battery, Charger, IndustrialConsumer, PvPlant, Wind (+9 more)

### Community 30 - "MessageBuilder-Tests"
Cohesion: 0.21
Nodes (7): DeviceTracker, LastChangeAt, LastRunId, LastSeq, LastSimTime, PublishSeq, MessageBuilderTests

### Community 31 - "Aufgabe 03 & Nachweise"
Cohesion: 0.13
Nodes (21): Abgabe: annotierter Tag lb03-2026, Bewertungskriterien (Datenweg, Vertrag, Docker, Robustheit, KI-Arbeitsweise), Datenweg Geraetesimulator -> Gateway -> RabbitMQ -> Viewer, Aufgabe 03: Von Modbus zu RabbitMQ (Aufgabenblatt), Mindestanforderungen (.NET 10, async, sichtbare Fehler, keine Secrets), Nachweis 1: RabbitMQ aus versionierter Compose-Konfiguration starten, Nachweis 2: TimeSim, WeatherSim und Geraetesimulatoren starten, Nachweis 3: Gateway und Viewer starten, Nachrichten aller Geraete sichtbar (+13 more)

### Community 32 - "Nachrichtentypen (Contract)"
Cohesion: 0.11
Nodes (9): ModbusRabbitGateway, PollOutcome, Contract, DeviceState, Measurement, MeasurementMessage, MessageJson, Quality (+1 more)

### Community 33 - "PvSystem"
Cohesion: 0.10
Nodes (21): PvSystem, ActiveFault, AgingLossPercent, AmbientTemperatureC, CapacityPercent, DailyEnergyKwh, EfficiencyPercent, ExternalWeather (+13 more)

### Community 34 - "Sync-Nachrichten (2)"
Cohesion: 0.18
Nodes (6): MessageChannel, RemoteEndPoint, TimeSyncClient, Endpoint, IsConnected, LastError

### Community 35 - "Schreibprüfung"
Cohesion: 0.12
Nodes (11): RegisterAccess, ReadOnly, ReadWrite, RegisterDefinition, Count, EndAddress, IsWritable, RegisterType (+3 more)

### Community 37 - "Gruppe 37"
Cohesion: 0.12
Nodes (14): ArgsParser, SimulationConfig, DeviceId, Duration, DurationHours, IsShortRun, Modbus, OutputDirectory (+6 more)

### Community 38 - "Gruppe 38"
Cohesion: 0.15
Nodes (9): IrradiationCalculator, WeatherCondition, Night, Overcast, PartlyCloudy, Snow, Storm, Sunny (+1 more)

### Community 39 - "SyncedClock"
Cohesion: 0.11
Nodes (17): SyncedClock, HeartbeatTimeout, LastLateness, LocalNow, MasterNow, Mode, Now, OffsetToMaster (+9 more)

### Community 40 - "TimeState & TimeLine"
Cohesion: 0.20
Nodes (5): TimeState, TimeLine, Latest, States, ManualTime

### Community 41 - "DevicePoller & Gerätetests"
Cohesion: 0.18
Nodes (8): Gefundener Fehler: doppelte offline-Statusnachricht bei wechselndem Fehlertext, Gefundener Fehler: MQTTnet 5 liefert Ergebniscode statt Exception bei abgelehnter Anmeldung, ModbusRabbitGateway.Tests (52/52 am 2026-11-08), DevicePoller, Device, State, Tracker, GatewayAgainstDeviceTests

### Community 42 - "DeviceConnection (Modbus.Client)"
Cohesion: 0.23
Nodes (7): DeviceConnection, Endpoint, Host, IsConnected, Port, TimeoutMs, UnitId

### Community 43 - "Gateway-Publisher"
Cohesion: 0.14
Nodes (16): Geräte-ID-Prüfung: Kürzel → Typ (Reg. 0), Nummer → Reg. 1, sonst decodeError, Bekannte Probleme: keine atomare Lesung, sequentielles Lesen, Compose mit festen Zugangsdaten, Nachrichtenvertrag LB03 (docs/Nachrichtenvertrag-LB03.md), RabbitMQ: retained nur bei exaktem Topic, nicht bei Wildcard-Abos, --status-repeat: Status alle 30 s wiederholen, sinceUtc unverändert, Topics energytwin/devices/{id}/measurements|status, energytwin/gateway/{id}/status, ConsolePublisher, Description (+8 more)

### Community 44 - "Gruppe 44"
Cohesion: 0.17
Nodes (9): TimeSyncOptions, HasStandaloneSettings, MasterEndpoint, MasterHost, MasterPort, Standalone, StandaloneSpeed, StandaloneStart (+1 more)

### Community 45 - "Fahrzeug"
Cohesion: 0.14
Nodes (11): Vehicle, BatteryCapacityKWh, HasDamagedInlet, IsFull, MaxChargePowerKW, ModelName, SocPercent, TargetSocPercent (+3 more)

### Community 46 - "DualCharger"
Cohesion: 0.14
Nodes (10): DualCharger, AmbientTemperatureC, Id, IsOn, Points, PowerLimitKW, QueueLength, RejectedVehicles (+2 more)

### Community 49 - "PV-Namespaces"
Cohesion: 0.18
Nodes (8): EnergySimulator.Data, EnergySimulator.Simulation, EnergySimulator.Devices, EnergySimulator.Output, Shared.Weather, InverterStatus, Fault, Ok

### Community 51 - "Gruppe 51"
Cohesion: 0.15
Nodes (9): Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk, coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio (+1 more)

### Community 52 - "Gruppe 52"
Cohesion: 0.21
Nodes (4): CommonRegisters, Header, RegisterFormatter, RegisterFormatterTests

### Community 53 - "DeviceTimeSource"
Cohesion: 0.12
Nodes (13): DeviceTimeSource, Clock, HasTime, IsRunning, IsStandalone, Options, Status, StatusText (+5 more)

### Community 54 - "Messwert-Schlüssel"
Cohesion: 0.18
Nodes (4): Festgehaltene Schlüsselliste measurement-keys.txt, KeyCatalog, MeasurementKeys, MeasurementKeysTests

### Community 55 - "Funktionsnachweis mit Master"
Cohesion: 0.14
Nodes (16): RabbitMQ 4.4.0 (Docker Compose, MQTT-Plugin), Beobachtung: Neustart der Ladesäule setzt Leistungsgrenze auf 150 kW zurück, Beobachtung: einmal verlorener Sollwert (Ursache ungeklärt, 410 Versuche ohne Wiederholung), Funktionsnachweis 2026-11-08: Ladesäule → Modbus → Gateway → RabbitMQ → Viewer, Nachweis 1: RabbitMQ aus Compose gestartet (erfüllt), Nachweis 2: TimeSim, WeatherSim und Ladesäule DC-01 synchron (erfüllt), Nachweis 3: Gateway und Viewer, Messwerte jede Sekunde (erfüllt), Nachweis 4: Sollwert-Kette 200 kW angenommen, 999 kW abgelehnt (erfüllt) (+8 more)

### Community 56 - "Gruppe 56"
Cohesion: 0.12
Nodes (15): SimulationEngine, Charger, Config, EndTime, IsAutoMode, IsFinished, IsRunning, Message (+7 more)

### Community 57 - "Gruppe 57"
Cohesion: 0.16
Nodes (9): ClientOptions, Host, IsOneShot, Once, Port, TimeoutMs, UnitId, Writes (+1 more)

### Community 58 - "StepScheduler"
Cohesion: 0.25
Nodes (6): StepScheduler, MaxCatchUpSteps, ModelTime, RunId, Step, StepSchedulerTests

### Community 59 - "SRS Gateway-Anforderungen"
Cohesion: 0.22
Nodes (15): Gateway-Pflichten (konfigurierbare Geraete, Dekodierung, Intervall, Simzeit, Fehler, Beenden), AK-12: Diagnose-Client funktioniert weiter, SRS LB03 - Von Modbus zu RabbitMQ, FA-GW-01: Geraete ueber ID/Host/Port/Unit-ID (--device), FA-GW-02: Register 0-49 in einer Anfrage, nach Registerplan dekodieren, FA-GW-03: Pruefen, ob richtiges Geraet antwortet, FA-GW-04: Intervall konfigurierbar (--interval), FA-GW-06: Qualitaet ableiten, nie veraltet als good (+7 more)

### Community 60 - "DeviceTimeSource (2)"
Cohesion: 0.20
Nodes (5): MonotonicUtcClock, Now, DeviceTimeSourceTests, ManualTimeProvider, TimestampFrequency

### Community 61 - "Gruppe 61"
Cohesion: 0.22
Nodes (3): ArgsParser, KuehlhausArgsTests, KuehlhausEngineTests

### Community 62 - "GatewayService"
Cohesion: 0.21
Nodes (6): GatewayService, BrokerDown, DroppedSinceOutage, FlakyPublisher, Description, Down

### Community 63 - "Bibliotheks-Namespaces"
Cohesion: 0.30
Nodes (5): Devices.Common, KuehlhausSimulator.IO, Devices.Common.Weather, KuehlhausSimulator.Simulation, FieldDevices.Tests

### Community 64 - "Diagnose-Client & Modbus.Client"
Cohesion: 0.15
Nodes (13): Empfehlung: Gateway als eigenes Projekt mit gemeinsamer Modbus-Bibliothek, Empfehlung: MQTT statt AMQP, Offene Fragen ans Team (AMQP/MQTT, Gateway-Projekt, Zuständigkeit, Pause-Veröffentlichung, Skills), A10 Gateway-Anforderungen (nicht erfüllt; wiederverwendbar: DeviceConnection, RegisterPlans, RegisterCodec), A11 Daten nur über Modbus, Diagnose-Client bleibt nutzbar (erfüllt), EasyModbusTCP.NETCore synchron (Bibliotheksgrenze, Banner), Modbus.Client (gemeinsamer Modbus-TCP-Client), DeviceConnection am 2026-11-08 aus dem Diagnose-Client verschoben (git mv) (+5 more)

### Community 65 - "Gruppe 65"
Cohesion: 0.23
Nodes (6): AutoModeController, ClientConnectionState, Connected, Disconnected, NotResponding, TimeSyncClientInfo

### Community 66 - "Gruppe 66"
Cohesion: 0.14
Nodes (14): SimulationConfig, CoolingPowerKW, DeviceId, Duration, DurationHours, IsShortRun, Modbus, OutputDirectory (+6 more)

### Community 67 - "Gruppe 67"
Cohesion: 0.22
Nodes (6): Season, Autumn, Spring, Summer, Winter, SeasonInfo

### Community 68 - "Gruppe 68"
Cohesion: 0.15
Nodes (10): KuehlhausSimulator, DeviceState, Cooling, Fault, Idle, Off, FaultType, CompressorFailure (+2 more)

### Community 69 - "Gruppe 69"
Cohesion: 0.28
Nodes (3): ConsoleRenderer, IsInteractive, Width

### Community 70 - "Batterie-Modbus & Update (3)"
Cohesion: 0.15
Nodes (4): BatteryModbus, StatusText, WeatherClient, BaseUrl

### Community 71 - "Gruppe 71"
Cohesion: 0.23
Nodes (6): ClockOffsetEstimator, HasSample, Offset, RoundTrip, Uncertainty, ClockOffsetEstimatorTests

### Community 73 - "Batterie-Modbus & Update (4)"
Cohesion: 0.31
Nodes (3): PvModbus, StatusText, PvModbusTests

### Community 74 - "ArgsParser"
Cohesion: 0.15
Nodes (13): SimulationConfig, DeviceId, Location, Modbus, ModelStep, ModelStepMinutes, OutputDirectory, PeakPowerKw (+5 more)

### Community 75 - "Wettermodell & Tests"
Cohesion: 0.22
Nodes (6): ManualWeather, CloudCoverPercent, IsActive, PrecipitationMmPerHour, TemperatureC, WindSpeedMs

### Community 76 - "Status-Wiederholung & RabbitMQ"
Cohesion: 0.23
Nodes (12): Entwicklungsjournal fortfuehren, Befund: Meldung Anmeldung abgelehnt irrefuehrend bei Container-Neustart, Bug: doppelte offline-Statusnachricht bei wechselndem Fehlertext (behoben in DevicePoller), Befund: retained nicht an Wildcard-Abos -> Status-Wiederholung, Journal Susanne 2026-11-08 (LB03), Probelauf gegen RabbitMQ 4.4.0 unter Docker, AK-10: Spaeter Viewer mit Wildcard sieht Ausfall nach 30 s, RabbitMQ liefert retained nicht an Wildcard-Abos (+4 more)

### Community 78 - "Charger-Namespaces"
Cohesion: 0.30
Nodes (5): DualChargerSim, DualChargerSim.Models, DualChargerSim.Simulation, DualChargerSim.IO, Program

### Community 80 - "Wettermodell & Tests (4)"
Cohesion: 0.17
Nodes (7): WeatherCondition, Night, Overcast, PartlyCloudy, Snow, Storm, Sunny

### Community 81 - "Gruppe 81"
Cohesion: 0.17
Nodes (11): KuehlhausConfig, AlarmHystereseK, AlarmTempC, COP, DeviceId, HystereseK, KaelteleistungKW, SollTempC (+3 more)

### Community 82 - "Gruppe 82"
Cohesion: 0.17
Nodes (11): OperatingState, Fault, Off, Running, Standby, WriteError, IncompleteValue, InvalidValue (+3 more)

### Community 83 - "Gruppe 83"
Cohesion: 0.20
Nodes (8): Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk, coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 84 - "Gruppe 84"
Cohesion: 0.17
Nodes (11): LastRequest, CloudCoverPercent, Condition, Count, IrradiationWm2, Location, PrecipitationMmPerHour, SimTimeUtc (+3 more)

### Community 85 - "RabbitMQ unter Docker"
Cohesion: 0.22
Nodes (11): Offene Frage 9: AMQP oder MQTT -> MQTT entschieden, Protokollwahl AMQP oder MQTT begruenden, .env.example statt versionierter Zugangsdaten, RabbitMQ unter Docker (Compose, Image-Version, Ports, Plugins), Befund: Passwort fest in Compose, rabbitmq.conf leerer Ordner, Image ohne Version, AK-02: RabbitMQ aus Compose ohne Zugangsdaten im Repo (offen: Passwort fest in Compose), Entscheidung MQTT (2026-11-08), FA-BR-01: RabbitMQ aus Compose, Image-Version, Ports 1883/5672/15672 (+3 more)

### Community 86 - "Gruppe 86"
Cohesion: 0.18
Nodes (10): RABBITMQ_DEFAULT_PASS, RABBITMQ_DEFAULT_USER, command, container_name, environment, image, ports, volumes (+2 more)

### Community 87 - "DeviceEndpoint"
Cohesion: 0.25
Nodes (5): Optionen --device, --interval, --stale-after, --broker, --status-repeat, --console, --once, --json, --keys, DeviceEndpoint, Endpoint, ExpectedInstance, ExpectedType

### Community 88 - "Gruppe 88"
Cohesion: 0.18
Nodes (8): ISimClock, Mode, Now, RunId, Speed, SimMode, Paused, Running

### Community 89 - "Gruppe 89"
Cohesion: 0.24
Nodes (7): ModbusOptions, Bind, BindAddress, Disabled, Port, PrintRegisterPlan, UnitId

### Community 90 - "LB03-Projektstand"
Cohesion: 0.22
Nodes (10): AGENTS.md Stand LB03 (258 Tests, Gateway 52/52), Projektanweisungen AGENTS.md/CLAUDE.md, Eigener Skill pro Person (mit Anwendungsnachweis), Commit Gateway (450f489), Push gescheitert, readTask-Lauf Aufgabe 03 (28 Anforderungen, docs/task-status/aufgabe-03.md), AK-01: Build 0 Warnungen, alle Tests gruen (258/258), Modbus.Client (Bibliothek), ModbusRabbitGateway (Projekt) (+2 more)

### Community 91 - "Gruppe 91"
Cohesion: 0.20
Nodes (6): IDevice, DailyEnergyKwh, IsEnabled, Name, PowerKw, TotalEnergyKwh

### Community 92 - "Qualitätsstatus & Pause"
Cohesion: 0.22
Nodes (9): Nachweis 7: Alle Anwendungen und Broker kontrolliert beenden, AK-09: Ohne Master nicht good (bad/masterLost), AK-11: Alle Programme kontrolliert beendbar (teilweise), Entscheidung: in der Pause weiter veroeffentlichen (paused), bad: masterLost / noTime, good: running / paused, Qualitaetsstatus good/uncertain/bad, uncertain: standalone / stale / invalidValue (+1 more)

### Community 93 - "Modbus.Client-Fehler"
Cohesion: 0.25
Nodes (7): Modbus.Client, DiagnosticErrorKind, Connection, Protocol, Value, DiagnosticException, Kind

### Community 94 - "Wettermodell & Tests (5)"
Cohesion: 0.28
Nodes (5): Weather.Tests, WeatherSimulator.Simulation, LocationCatalog, All, WeatherLocation

### Community 95 - "MessageBuilder"
Cohesion: 0.36
Nodes (4): BuildResult, MessageBuilder, QualitySettings, Default

### Community 96 - "Gruppe 96"
Cohesion: 0.22
Nodes (7): WeatherSynth, Microsoft.NET.Sdk, coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 98 - "Gruppe 98"
Cohesion: 0.28
Nodes (5): Microsoft.NET.Sdk, EasyModbusTCP.NETCore, Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk

### Community 99 - "Gruppe 99"
Cohesion: 0.28
Nodes (9): Deterministic weather (location, time, run seed), Weather HTTP API (/weather, /locations), LB01 Season/IrradiationCalculator basis, Manual weather override keys, WeatherData, WeatherSimulator, Weather.Tests, Weather reproducibility requirement (+1 more)

### Community 100 - "Zuständigkeiten LB03"
Cohesion: 0.32
Nodes (8): Verantwortung: zwei Simulatoren pro Person, Viewer (DemoApp oder MQTT Explorer), FA-VW-02: Anzeige DeviceId, Simzeit, Messwerte, seq, Qualitaet, FA-VW-03: Verbindung und Abo reproduzierbar dokumentiert, Alexander Lux, Dominik Zach, Nicht zugeteilt: SRS, Broker/Docker, Viewer, Messwerte PV/Batterie/Kuehlhaus, Nachweise, Skills Alexander/Dominik, Zustaendigkeiten Aufgabe 03 (neue Module)

### Community 101 - "Gruppe 101"
Cohesion: 0.25
Nodes (3): Shared.Data, CsvFormat, CsvSchema

### Community 102 - "Gruppe 102"
Cohesion: 0.29
Nodes (6): FaultType, GridVoltage, Inverter, None, Overheat, FaultTypeExtensions

### Community 103 - "OutputPaths"
Cohesion: 0.29
Nodes (4): Shared.Config, OutputPaths, CsvDirectory, RepositoryRoot

### Community 106 - "Gruppe 106"
Cohesion: 0.29
Nodes (7): DeviceState, Charging, Cooldown, Discharging, Fault, Off, Standby

### Community 107 - "StepScheduler (2)"
Cohesion: 0.33
Nodes (6): StepUpdate, StepUpdateKind, Advance, NoTime, Restarted, Started

### Community 108 - "Gruppe 108"
Cohesion: 0.29
Nodes (6): ChargingPointState, Available, Charging, Connected, Faulted, Finished

### Community 111 - "Gruppe 111"
Cohesion: 0.33
Nodes (6): PV Statuspanel (Leistung 1,37 kW, Herbst, Auto, x15, bewoelkt), Bild-Aktualisierung (R ruhiger / schneller) 3000 ms, PV Simulator Screenshot: Bild-Aktualisierung 3000 ms, Wartung: Module reinigen (Taste C), Verschmutzung auf 0,0 %, PV Statuspanel sonnig, 4,51 kW, x1800, PV Simulator Screenshot: Module gereinigt

### Community 112 - "Gruppe 112"
Cohesion: 0.33
Nodes (6): Simulation beendet: Tagesenergie 0,30 kWh, CSV pv-simulation.csv, Inverter FEHLER, Anlage GESTOERT, Leistung 1,19 kW rot, PV Simulator Screenshot: Inverter-Fehler (LB01), CSV-Pfad data/pv-simulation.csv (teilweise geschwaerzt), Laufzeit-Zusammenfassung 06.10.-08.10.2026, 52,00 kWh, PV Simulator Screenshot: Beendet, CSV gespeichert

### Community 113 - "Gruppe 113"
Cohesion: 0.33
Nodes (5): coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 114 - "Gruppe 114"
Cohesion: 0.33
Nodes (5): coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 115 - "Gruppe 115"
Cohesion: 0.50
Nodes (5): Charger key legend (P/A/1-2/C/D/S/F/R/+-/O/Q), AUTO mode, Punkt 1 CHARGING VW ID.3 120 kW, Limit 150 kW, Charger Screenshot: Automatischer Modus, MANUELL mode: Punkt 1 CONNECTED, Punkt 2 FINISHED, message Laden gestoppt, Charger Screenshot: Manueller Modus

### Community 116 - "Gruppe 116"
Cohesion: 0.70
Nodes (5): Feldgeraete-Fenster: Batterie, Ladesaeule, PV-Anlage, Kuehlhaus, Modbus-Diagnose-Client Fenster (PvPlant Registerkopf), Screenshot: Gesamtsystem mit allen Programmen, Simulations-Master Fenster (x600, Port 5100, 4 Simulatoren verbunden), Wettersimulator Fenster (HTTP 8080, salzburg)

### Community 117 - "Gruppe 117"
Cohesion: 0.40
Nodes (3): EasyModbusTCP.NETCore, Microsoft.NET.Sdk, Microsoft.NET.Sdk

### Community 118 - "Projekt-Doku & Geräte"
Cohesion: 0.80
Nodes (4): Start-Program(), Start-TwinService(), Test-Port(), Wait-Port()

### Community 120 - "Fehlerverhalten Gateway"
Cohesion: 0.50
Nodes (4): Keine atomare Lese-Garantie verlangt, Verhalten bei Fehlern (offline, decodeError, Broker nicht erreichbar), Broker-Ausfall: verwerfen, Neuversuch 1-30 s, Status nachholen, Bekannte Einschraenkungen (keine atomare Lesung, Sekundenaufloesung, sequentiell)

### Community 121 - "readTask-Arbeitsschritte"
Cohesion: 0.50
Nodes (4): Statuskategorien erfüllt / teilweise / nicht erfüllt / nicht nachgewiesen, 28 Anforderungen: 3 erfüllt, 6 teilweise, 16 nicht erfüllt, 3 nicht nachgewiesen, Ordnungskriterien: Abhängigkeit, Pflicht, Risiko/Aufwand, Zuständigkeit, Arbeitsschritte: Extrahieren → Abgleichen → Reihenfolge → Berichten

### Community 122 - "Gruppe 122"
Cohesion: 0.50
Nodes (4): Raised limit 230 kW, Abgewiesen 8, Charger Screenshot: Limit Erhoehung, Punkt 1 FAULTED Tesla Model 3 62 C, Kuehlungsdefekt 85 C, Charger Screenshot: Ueberhitzung

### Community 123 - "Projekt-Doku & Geräte (2)"
Cohesion: 0.67
Nodes (3): ModbusDeviceServer, Modbus-im-Gerät Muster, Kühlhaus Umsetzungsanleitung LB02

## Ambiguous Edges - Review These
- `ModbusDeviceServer` → `Befund: einmal verlorener Sollwert, in 410 Wiederholungen nicht reproduzierbar`  [AMBIGUOUS]
  docs/development-journal/susanne-lastowicka.md · relation: references
- `ModbusDeviceServer` → `Beobachtung: einmal verlorener Sollwert (Ursache ungeklärt, 410 Versuche ohne Wiederholung)`  [AMBIGUOUS]
  docs/nachweise/2026-11-08-gateway-mit-master/README.md · relation: conceptually_related_to
- `DualCharger` → `Befund: einmal verlorener Sollwert, in 410 Wiederholungen nicht reproduzierbar`  [AMBIGUOUS]
  docs/development-journal/susanne-lastowicka.md · relation: references
- `DeviceTracker` → `--status-repeat: Status alle 30 s wiederholen, sinceUtc unverändert`  [AMBIGUOUS]
  src/ModbusRabbitGateway/README.md · relation: references
- `Nachweis 7: Alle Anwendungen und Broker kontrolliert beenden` → `Funktionsnachweis mit Master (docs/nachweise/2026-11-08-gateway-mit-master)`  [AMBIGUOUS]
  docs/development-journal/susanne-lastowicka.md · relation: implements
- `Nicht zugeteilt: SRS, Broker/Docker, Viewer, Messwerte PV/Batterie/Kuehlhaus, Nachweise, Skills Alexander/Dominik` → `Alexander Lux`  [AMBIGUOUS]
  README.md · relation: references
- `Nicht zugeteilt: SRS, Broker/Docker, Viewer, Messwerte PV/Batterie/Kuehlhaus, Nachweise, Skills Alexander/Dominik` → `Dominik Zach`  [AMBIGUOUS]
  README.md · relation: references

## Knowledge Gaps
- **617 isolated node(s):** `StatusText`, `ClientCount`, `IsListening`, `LastWriteText`, `Options` (+612 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 699 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **23 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SimulationEngine` connect `WeatherClient & Anzeige` to `Gruppe 65`, `Gruppe 66`, `Batterie-Modbus & Update (3)`, `Gruppe 104`, `Batterie-Modbus & Update`, `Wettermodell & Tests (2)`, `DeviceTimeSource (2)`, `Gruppe 109`, `Batterie-Simulator Loop (2)`, `WeatherFeed`, `Gruppe 81`, `DeviceTimeSource`, `StepScheduler`, `Kühlhaus-Modell`, `Bibliotheks-Namespaces`?**
  _High betweenness centrality (0.058) - this node is a cross-community bridge._
- **What connects `StatusText`, `ClientCount`, `IsListening` to the rest of the system?**
  _617 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `MQTT-Publisher & Zugangsdaten` be split into smaller, more focused modules?**
  _Cohesion score 0.06666666666666667 - nodes in this community are weakly interconnected._
- **Why does `DeviceTimeSource` connect `DeviceTimeSource` to `Sync-Nachrichten (2)`, `RegisterCodec & Befehle`, `WeatherClient & Anzeige`, `Batterie-Modbus & Update (3)`, `Diagnose-Client Verbindung`, `SyncedClock`, `Batterie-Modbus & Update (4)`, `Batterie-Modbus & Update`, `Zeit-Bibliotheken & Master`, `MasterClock`, `Batterie-Modbus & Update (2)`, `Gruppe 44`, `Gruppe 21`, `Gruppe 56`, `PV-Engine & Fehler`, `Batterie-Simulator Loop`, `Gruppe 88`, `DeviceTimeSource (2)`?**
  _High betweenness centrality (0.052) - this node is a cross-community bridge._
- **Should `Zeitsync-Client & Konsole` be split into smaller, more focused modules?**
  _Cohesion score 0.06823529411764706 - nodes in this community are weakly interconnected._

### Low-confidence Hints
_AMBIGUOUS edges — the extractor was unsure. Verify before acting on these._

- **What is the exact relationship between `ModbusDeviceServer` and `Befund: einmal verlorener Sollwert, in 410 Wiederholungen nicht reproduzierbar`?**
  _Edge tagged AMBIGUOUS (relation: references) - confidence is low._
- **What is the exact relationship between `ModbusDeviceServer` and `Beobachtung: einmal verlorener Sollwert (Ursache ungeklärt, 410 Versuche ohne Wiederholung)`?**
  _Edge tagged AMBIGUOUS (relation: conceptually_related_to) - confidence is low._