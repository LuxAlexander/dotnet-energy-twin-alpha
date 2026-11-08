
> Aktualisierung 2026-11-08: inkrementeller Arbeitsstand einschließlich uncommittierter Änderungen.
> Vorherige unveränderte Quellen wurden erhalten; geänderte Quellen neu extrahiert. Agent-Tokenverbrauch
> wurde nicht gemessen; die generischen Tokenfelder im Bericht sind keine gemessene Gesamtkostenangabe.
> Aktueller Umfang und Integritätsgrenzen: `UPDATE_STATUS.json`. Alte Obsidian-Ausgabe wurde nicht neu exportiert.

# Graph Report - dotnet-energy-twin-alpha  (2026-11-08)

## Corpus Check
- 156 files · ~134,799 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 2183 nodes · 5030 edges · 140 communities (104 shown, 36 thin omitted)
- Extraction: 87% EXTRACTED · 13% INFERRED · 0% AMBIGUOUS · INFERRED: 667 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `cc451f0e`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Geräteprüfungen
- Zeitverteilung
- Simulations-Master
- Modbus-Diagnose
- Zeitverteilung
- Wetter und Klima
- Geräteprüfungen
- MQTT-Gateway
- Modbus-Register
- MQTT-Gateway
- MQTT-Gateway
- MQTT-Gateway
- Virtuelle Zeit
- DC-Ladesäule
- Modbus-Register
- README-Pflege
- PV-Simulation
- Modbus-Register
- Modbus-Register
- Geräteanbindung
- RabbitMQ-Broker
- Modbus-Verbindung
- Wetter und Klima
- Projektübersicht
- Kühlhaus
- Virtuelle Zeit
- Virtuelle Zeit
- Kühlhaus
- Geräteanbindung
- Konfiguration
- Kühlhaus
- Geräteanbindung
- Geräteprüfungen
- PV-Simulation
- Geräteanbindung
- Gateway-Prüfungen
- Geräteprüfungen
- Modbus-Register
- Kühlhaus
- Virtuelle Zeit
- PV-Simulation
- Gateway-Prüfungen
- Batteriesimulation
- DC-Ladesäule
- PV-Simulation
- Wetter und Klima
- Wetter und Klima
- DC-Ladesäule
- Geräteanbindung
- Geräteprüfungen
- Batteriesimulation
- MQTT-Gateway
- MQTT-Gateway
- Geräteprüfungen
- MQTT-Gateway
- Batteriesimulation
- Modbus-Diagnose
- Funktionsnachweise
- Kühlhaus
- Gemeinsame Bausteine
- Geräteprüfungen
- Kühlhaus
- Gemeinsame Bausteine
- Simulationsverträge
- Kühlhaus
- Wetterprüfungen
- Modbus-Register
- Virtuelle Zeit
- DC-Ladesäule
- PV-Simulation
- Batteriesimulation
- DC-Ladesäule
- Simulationsverträge
- Geräteanbindung
- DC-Ladesäule
- Kühlhaus
- Modbus-Verbindung
- Wetter und Klima
- Batteriesimulation
- Wetter und Klima
- PV-Simulation
- DC-Ladesäule
- Batteriesimulation
- Geräteanbindung
- Wetter und Klima
- PV-Simulation
- Wetterprüfungen
- Kühlhaus
- Simulationsverträge
- Funktionsnachweise
- Funktionsnachweise
- DC-Ladesäule
- Batteriesimulation
- PV-Simulation
- PV-Simulation
- Gateway-Prüfungen
- PV-Simulation
- Gateway-Prüfungen
- Zeitverteilung
- Gemeinsame Bausteine
- Gateway-Prüfungen
- Batteriesimulation
- Batteriesimulation
- DC-Ladesäule
- DC-Ladesäule
- DC-Ladesäule
- PV-Simulation
- PV-Simulation
- Modbus-Register
- DC-Ladesäule
- Projektübersicht
- Projektübersicht
- DC-Ladesäule
- PV-Simulation
- README-Pflege
- PV-Simulation
- Projektübersicht
- RabbitMQ-Broker
- DC-Ladesäule
- Entwicklungsjournal
- Funktionsnachweise
- Funktionsnachweise
- Batteriesimulation
- Projektübersicht
- Geräteanbindung
- PV-Simulation
- Entwicklungsjournal
- Entwicklungsjournal
- Projektübersicht
- Projektübersicht
- Funktionsnachweise
- Funktionsnachweise
- Funktionsnachweise
- Funktionsnachweise
- Kühlhaus

## God Nodes (most connected - your core abstractions)
1. `MasterClock` - 50 edges
2. `SyncedClock` - 47 edges
3. `SimulationEngine` - 44 edges
4. `SimulationEngine` - 42 edges
5. `DeviceTimeSource` - 41 edges
6. `Simulator` - 39 edges
7. `SimulationEngine` - 39 edges
8. `Kuehlhaus` - 36 edges
9. `ModbusDeviceServer` - 36 edges
10. `MasterConsole` - 36 edges

## Surprising Connections (you probably didn't know these)
- `Schreiben und Prüfen: 200 ms warten, Register 18/19 zurücklesen` --conceptually_related_to--> `ChargerRegisters`  [INFERRED]
  src/ModbusDiagnosticClient/README.md → src/Modbus.Contracts/ChargerRegisters.cs
- `Automatisierte Proben ersetzen keine echte Terminalbedienung` --semantically_similar_to--> `Offene Übergabepunkte nach der Umsetzung`  [INFERRED] [semantically similar]
  .opencode/skills/probelauf/SKILL.md → docs/handoff/2026-11-08-alexander-lb03.md
- `Screenshot einer Wettertabelle: Partly Cloudy (Daylight), 7,6 °C, Einstrahlung 528,1 W/m², Sonnenhöhe 44,68°, Bedeckung 31 %, Wind 6,87 m/s und Niederschlag 0 mm/h; Ort, Zeit, Topic und Live-Verbindung im Bild nicht sichtbar` --semantically_similar_to--> `Wetter-Empfang 2026-11-08: sourceSession, masterSession, runId, seq/messageId, location, modelTimeUtc/simTimeUtc/publishedAtUtc, mode/condition/isDaylight; Messschlüssel temperature_c, irradiation_w_m2, wind_speed_m_s, precipitation_mm_h, cloud_cover_percent und sun_height_degrees`  [INFERRED] [semantically similar]
  docs/screenshots/RabbitMQ/ExampleDisplayOfSOMEWeatherInfoSalzburg.png → docs/nachweise/2026-11-08-wetter-mqtt/mqtt-empfang.txt
- `BatteryModbusTests` --calls--> `DailyProfile`  [INFERRED]
  tests/FieldDevices.Tests/FieldDeviceTests.cs → src/Devices/BatterySimulator/DailyProfile.cs
- `BatteryModbusTests` --calls--> `SimulatorOptions`  [INFERRED]
  tests/FieldDevices.Tests/FieldDeviceTests.cs → src/Devices/BatterySimulator/SimulatorOptions.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **KH-01-Sollwertkette 2026-11-08: Ausgangslesung, bestätigter Schreibzugriff, unabhängige Gegenlesung und Geräteanzeige** — docs_nachweise_2026_11_08_probelauf_skill_diagnose_01_read_ausgangslesung, docs_nachweise_2026_11_08_probelauf_skill_diagnose_02_write_sollwert, docs_nachweise_2026_11_08_probelauf_skill_diagnose_03_read_gegenlesung, docs_nachweise_2026_11_08_probelauf_skill_kuehlhaus_geraeteprotokoll [EXTRACTED 1.00]
- **Drei getrennte eigene Skills: Aufgabenbewertung, Funktionsnachweis, Dokumentationspflege** — skills_readtask_skill_readtask, _opencode_skills_probelauf_skill_probelauf, skills_lets_document_skill_lets_document [EXTRACTED 1.00]
- **Probelauf kombiniert Prüfkriterien, Portmatrix, Fehlerwissen und Nachweisvorlage** — _opencode_skills_probelauf_skill_probelauf, _opencode_skills_probelauf_references_abnahmekriterien_abnahmekriterien, _opencode_skills_probelauf_references_ports_portmatrix, _opencode_skills_probelauf_references_fehlerbilder_fehlerbilder, _opencode_skills_probelauf_templates_nachweis_readme_nachweisvorlage [EXTRACTED 1.00]
- **Wetterstrom koppelt retained Stand, Sitzungsidentität, Dienststatus und Last Will** — docs_wetter_nachrichtenvertrag_lb03_sechs_retained_topics, docs_wetter_nachrichtenvertrag_lb03_sitzungsgültigkeit, docs_wetter_nachrichtenvertrag_lb03_status_und_pause, docs_wetter_nachrichtenvertrag_lb03_last_will [EXTRACTED 1.00]
- **Gerät → gemeinsamer Modbus-Client/Gateway → RabbitMQ → MQTT-Viewer** — src_devices_pvsimulator_readme_pvregister, src_modbus_client_readme_deviceconnection, src_modbusrabbitgateway_readme_gateway, docker_readme_rabbitmq, docs_srs_lb03_viewer_offen [EXTRACTED 1.00]
- **Datierte Nachweise trennen beobachtete Funktion von offenen interaktiven Prüfungen, synthetischen Beispielen und nicht neu gelaufenen Unit-Tests** — docs_nachweise_2026_11_08_docker_alexander_readme_aussagegrenze, docs_nachweise_2026_11_08_gateway_mit_master_readme_nachweisgrenzen, docs_nachweise_2026_11_08_lets_document_readme_aussagegrenze, docs_nachweise_2026_11_08_probelauf_skill_readme_aussagegrenze, docs_nachweise_2026_11_08_wetter_mqtt_readme_aussagegrenze [INFERRED 0.85]

## Communities (140 total, 36 thin omitted)

### Community 0 - "Geräteprüfungen"
Cohesion: 0.07
Nodes (15): BatteryModbus, StatusText, WeatherClient, BaseUrl, ChargerModbus, StatusText, KuehlhausModbus, StatusText (+7 more)

### Community 1 - "Zeitverteilung"
Cohesion: 0.07
Nodes (18): ByeMessage, HelloMessage, PingMessage, PongMessage, StatesMessage, SyncMessage, SyncProtocol, MessageChannel (+10 more)

### Community 2 - "Simulations-Master"
Cohesion: 0.06
Nodes (21): KeyResult, Handled, Quit, MasterConsole, HasKeyboard, IsInteractive, Width, PromptKind (+13 more)

### Community 3 - "Modbus-Diagnose"
Cohesion: 0.12
Nodes (4): DiagnosticSession, Connection, Plan, DiagnosticClientTests

### Community 4 - "Zeitverteilung"
Cohesion: 0.11
Nodes (6): SimCore.TimeSync, SimCore.Tests, SimCore.Time, Devices.Common.Tests, SimulationMaster, SimCore.Contracts

### Community 5 - "Wetter und Klima"
Cohesion: 0.06
Nodes (26): Dokumentierter Teststand 258/258; neue Wettertests nur gebaut, Geräte-Nachrichtenvertrag LB03, Schema 1, Entwurf, Gerätequalität: good, uncertain, bad, Geräte-MQTT-Topics und RabbitMQ-Routing, Geräteumschlag trennt simTime, polledAtUtc, seq und publishSeq, SRS LB03: Gateway, Vertrag, Docker-Broker und MQTT-Viewer, Interaktiver MQTT-Viewer und Terminal-Gesamtvorführung offen, Fünf Wettertopics und ein Status-Topic, QoS 1 und retained (+18 more)

### Community 6 - "Geräteprüfungen"
Cohesion: 0.06
Nodes (34): DeviceTimeSourceTests, ModbusDeviceServerTests, Devices.Common.Tests, RegisterCodecTests, StepSchedulerTests, TimeSyncOptionsTests, ClientOptionsTests, DiagnosticClientTests (+26 more)

### Community 7 - "MQTT-Gateway"
Cohesion: 0.10
Nodes (11): DeviceEndpoint, Endpoint, ExpectedInstance, ExpectedType, DevicePoller, Device, State, Tracker (+3 more)

### Community 8 - "Modbus-Register"
Cohesion: 0.14
Nodes (4): ModbusCommand, RegisterCodec, ModbusDeviceServerTests, RegisterCodecTests

### Community 9 - "MQTT-Gateway"
Cohesion: 0.10
Nodes (8): Modbus.Client, ModbusRabbitGateway, Modbus.Contracts, Shared.Data, ModbusDiagnosticClient, AsyncConsole, HasKeyboard, CsvSchema

### Community 10 - "MQTT-Gateway"
Cohesion: 0.11
Nodes (8): BrokerSettings, Endpoint, MqttPublisher, Description, GatewayId, IsConnected, BrokerUnavailableException, MqttPublisherTests

### Community 11 - "MQTT-Gateway"
Cohesion: 0.09
Nodes (15): GatewayService, BrokerDown, DroppedSinceOutage, ConsolePublisher, Description, IMessagePublisher, Description, TeePublisher (+7 more)

### Community 12 - "Virtuelle Zeit"
Cohesion: 0.13
Nodes (7): MonotonicUtcClock, Now, TimeSyncClient, Endpoint, IsConnected, LastError, TimeSyncIntegrationTests

### Community 13 - "DC-Ladesäule"
Cohesion: 0.10
Nodes (18): ConsoleRenderer, IsInteractive, Width, SimulationEngine, Charger, Config, EndTime, IsAutoMode (+10 more)

### Community 14 - "Modbus-Register"
Cohesion: 0.13
Nodes (11): CommonRegisters, Header, RegisterAccess, ReadOnly, ReadWrite, RegisterDefinition, Count, EndAddress (+3 more)

### Community 15 - "README-Pflege"
Cohesion: 0.07
Nodes (19): readTask: lokale Claude-Kopie der Skillbeschreibung, readTask: lokale Claude-Anweisungen, ADR-0013: Wetter-MQTT bestätigt, umgesetzt und funktional geprüft, Wetter-MQTT: Pause, Zeitverlust und Wiederanlauf, Unabhängiger Wetterstrom zusätzlich zu HTTP, ADR-0014: GetStatus verworfen, ADR-0015: Let's Document bestätigt und erstellt, Archivierte PV-README-Originalausschnitte (+11 more)

### Community 16 - "PV-Simulation"
Cohesion: 0.12
Nodes (9): Note, SimulationEngine, ModbusStatus, Mode, Pv, RunId, SimulationTime, Time (+1 more)

### Community 17 - "Modbus-Register"
Cohesion: 0.09
Nodes (15): BatteryRegisters, Plan, ChargerRegisters, Plan, KuehlhausRegisters, Plan, PvRegisters, Plan (+7 more)

### Community 18 - "Modbus-Register"
Cohesion: 0.09
Nodes (28): BatterySimulator, Batterie Kommandozeilenargumente, SOC-Modell Batteriespeicher, Charger Kommandozeilenargumente, DualPortCharger, Gemeinsames Leistungsbudget der Ladepunkte, KuehlhausSimulator, Kuehlhaus Registerplan 20-31 (+20 more)

### Community 19 - "Geräteanbindung"
Cohesion: 0.09
Nodes (14): ModbusDeviceServer, ClientCount, IsListening, LastWriteText, Options, Plan, StatusText, ValueValidator (+6 more)

### Community 20 - "RabbitMQ-Broker"
Cohesion: 0.07
Nodes (26): create_host_path, RABBITMQ_DEFAULT_PASS, RABBITMQ_DEFAULT_USER, interval, retries, start_period, test, timeout (+18 more)

### Community 21 - "Modbus-Verbindung"
Cohesion: 0.14
Nodes (13): DeviceConnection, Endpoint, Host, IsConnected, Port, TimeoutMs, UnitId, DiagnosticErrorKind (+5 more)

### Community 22 - "Wetter und Klima"
Cohesion: 0.19
Nodes (9): ManualWeather, CloudCoverPercent, IsActive, PrecipitationMmPerHour, SelectedLocation, TemperatureC, WindSpeedMs, ManualWeatherState (+1 more)

### Community 23 - "Projektübersicht"
Cohesion: 0.08
Nodes (26): Abgabe: annotierter Tag lb03-2026, Projektanweisungen AGENTS.md/CLAUDE.md, Protokollwahl AMQP oder MQTT begruenden, Bewertungskriterien (Datenweg, Vertrag, Docker, Robustheit, KI-Arbeitsweise), Datenweg Geraetesimulator -> Gateway -> RabbitMQ -> Viewer, Aufgabe 03: Von Modbus zu RabbitMQ (Aufgabenblatt), Eigener Skill pro Person (mit Anwendungsnachweis), Entwicklungsjournal fortfuehren (+18 more)

### Community 24 - "Kühlhaus"
Cohesion: 0.18
Nodes (8): ModbusDiagnosticClient.Tests, Devices.Common, KuehlhausSimulator.IO, Devices.Common.Modbus, Devices.Common.Weather, KuehlhausSimulator.Simulation, Batterie_Sim, FieldDevices.Tests

### Community 25 - "Virtuelle Zeit"
Cohesion: 0.12
Nodes (9): TimeState, Current, Now, TimeLine, Latest, States, ManualTime, ManualTimeProvider (+1 more)

### Community 26 - "Virtuelle Zeit"
Cohesion: 0.17
Nodes (9): MasterClock, CurrentStates, Latest, LeadTime, MasterNow, Mode, RunId, Speed (+1 more)

### Community 27 - "Kühlhaus"
Cohesion: 0.10
Nodes (16): InputHandler, SimulationEngine, Config, DoorClosesAt, EndTime, IsAutoMode, IsFinished, IsRunning (+8 more)

### Community 28 - "Geräteanbindung"
Cohesion: 0.11
Nodes (12): DeviceTimeSource, Clock, HasTime, IsRunning, IsStandalone, MasterSession, Options, Status (+4 more)

### Community 29 - "Konfiguration"
Cohesion: 0.09
Nodes (6): Shared.Config, WeatherSimulator.Publishing, OutputPaths, CsvDirectory, RepositoryRoot, Program

### Community 30 - "Kühlhaus"
Cohesion: 0.20
Nodes (6): CsvLogger, FilePath, CsvExporter, FilePath, CsvFileWriter, FilePath

### Community 31 - "Geräteanbindung"
Cohesion: 0.19
Nodes (11): WeatherUnavailableException, WeatherFeed, Last, Location, Status, IWeatherService, WeatherData, FakeWeatherService (+3 more)

### Community 34 - "Geräteanbindung"
Cohesion: 0.17
Nodes (12): StepScheduler, MaxCatchUpSteps, ModelTime, RunId, Step, StepUpdate, StepUpdateKind, Advance (+4 more)

### Community 35 - "Gateway-Prüfungen"
Cohesion: 0.15
Nodes (3): MqttCredentials, MqttCredentials, MqttCredentialsTests

### Community 36 - "Geräteprüfungen"
Cohesion: 0.13
Nodes (12): Microsoft.NET.Sdk, EasyModbusTCP.NETCore, Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk, coverlet.collector (+4 more)

### Community 37 - "Modbus-Register"
Cohesion: 0.12
Nodes (12): HeaderValues, RegisterWriter, OperatingState, Fault, Off, Running, Standby, TimeStatus (+4 more)

### Community 38 - "Kühlhaus"
Cohesion: 0.11
Nodes (16): Kuehlhaus, ActiveFault, AlarmTempC, CompressorOn, CoolingPowerKW, DoorOpen, ElectricPowerKW, EnergyKWh (+8 more)

### Community 39 - "Virtuelle Zeit"
Cohesion: 0.14
Nodes (18): SyncedClock, HeartbeatTimeout, LastLateness, LocalNow, MasterNow, MasterSession, Mode, Now (+10 more)

### Community 40 - "PV-Simulation"
Cohesion: 0.10
Nodes (21): PvSystem, ActiveFault, AgingLossPercent, AmbientTemperatureC, CapacityPercent, DailyEnergyKwh, EfficiencyPercent, ExternalWeather (+13 more)

### Community 41 - "Gateway-Prüfungen"
Cohesion: 0.22
Nodes (7): DeviceTracker, LastChangeAt, LastRunId, LastSeq, LastSimTime, PublishSeq, MessageBuilderTests

### Community 42 - "Batteriesimulation"
Cohesion: 0.13
Nodes (14): Batterie, ActiveFault, ActualKW, AmbientTemperatureC, ChargedKWh, Config, DeviceId, DischargeKWh (+6 more)

### Community 43 - "DC-Ladesäule"
Cohesion: 0.12
Nodes (11): ChargingPoint, AmbientTemperatureC, DeratingFactor, Efficiency, EnergyKWh, IsFree, Number, PowerKW (+3 more)

### Community 44 - "PV-Simulation"
Cohesion: 0.13
Nodes (12): PvFaultModel, ActiveFault, AgingLossPercent, AmbientTemperatureC, GridDisturbanceForced, GridVoltageV, HasFault, InverterFault (+4 more)

### Community 45 - "Wetter und Klima"
Cohesion: 0.17
Nodes (5): LocationCatalog, All, WeatherLocation, WeatherModel, Manual

### Community 46 - "Wetter und Klima"
Cohesion: 0.21
Nodes (6): WeatherMeasurement, WeatherMessage, WeatherMessages, WeatherStatus, WeatherPublisher, StatusText

### Community 47 - "DC-Ladesäule"
Cohesion: 0.12
Nodes (10): DualCharger, AmbientTemperatureC, Id, IsOn, Points, PowerLimitKW, QueueLength, RejectedVehicles (+2 more)

### Community 48 - "Geräteanbindung"
Cohesion: 0.17
Nodes (9): TimeSyncOptions, HasStandaloneSettings, MasterEndpoint, MasterHost, MasterPort, Standalone, StandaloneSpeed, StandaloneStart (+1 more)

### Community 49 - "Geräteprüfungen"
Cohesion: 0.18
Nodes (4): ArgsParser, Program, KuehlhausArgsTests, KuehlhausEngineTests

### Community 50 - "Batteriesimulation"
Cohesion: 0.12
Nodes (15): ControlMode, Auto, Manual, Simulator, IsFinished, Message, ModbusStatus, Mode (+7 more)

### Community 51 - "MQTT-Gateway"
Cohesion: 0.20
Nodes (6): BuildResult, MessageBuilder, QualitySettings, Default, Measurement, MeasurementMessage

### Community 52 - "MQTT-Gateway"
Cohesion: 0.12
Nodes (17): GatewayOptions, BrokerHost, BrokerPort, ConsoleOnly, DefaultDevices, Devices, EnvFile, GatewayId (+9 more)

### Community 54 - "MQTT-Gateway"
Cohesion: 0.17
Nodes (6): Contract, DeviceState, GatewayStatusMessage, MessageJson, Quality, Reason

### Community 55 - "Batteriesimulation"
Cohesion: 0.15
Nodes (11): BatterieConfig, CapacityKWh, DeviceId, Efficiency, MaxChargeKW, MaxDischargeKW, SocMaxPercent, SocMinPercent (+3 more)

### Community 56 - "Modbus-Diagnose"
Cohesion: 0.16
Nodes (9): ClientOptions, Host, IsOneShot, Once, Port, TimeoutMs, UnitId, Writes (+1 more)

### Community 57 - "Funktionsnachweise"
Cohesion: 0.13
Nodes (12): 2026-11-08 empfangene Topics energytwin/devices/PV-01/measurements, energytwin/devices/BAT-01/measurements, energytwin/devices/DC-01/measurements und energytwin/devices/KH-01/measurements: 18/21/19/15 Messwerte, jeweils good/running, RunId 1 und Tempo 60, Geräte-Empfang 2026-11-08: schemaVersion, deviceId/deviceType, simTime/simTimeEpoch, runId, seq, publishSeq, polledAtUtc, quality/qualityReason und measurements mit key/name/address/value/unit sowie optional text; unterschiedliche Modellzeiten sind sichtbar, 2026-11-08: RabbitMQ 4.4.0-management im Compose-Projekt energytwin healthy; lokale Listener 1883/5672/15672 und Anmeldung geprüft; echter Master, Wetter und vier Feldgeräte; Gateway --once Exit-Code 0, 2026-11-08 Ablauf 15:15:56–15:17:23: Start, 200/999-kW-Schreibversuche, Pause/Fortsetzen, harter Geräteausfall und Neustart, RabbitMQ stoppen/starten, Master-Ende und Prüfprozessende, 2026-11-08 Susannes Funktionsnachweis: nur DC-01, Modbus 1504, Gateway, RabbitMQ 4.4.0 und MQTTnet-Testempfänger mit energytwin/#; Skript-Master nutzt MasterClock und TimeSyncServer bei Tempo 60, 2026-11-08 Let's Document: ausdrücklich freigegebener Entwurf aus lokalem ADR-0015; maßgebliche skills/lets-document/SKILL.md; PV-README mit vorherigem Archiv, gebündeltem Anzeigeinhalt und datiertem MQTT-Empfangsnachweis bearbeitet; Code-/Formelblöcke unverändert, Wetter-Empfang 2026-11-08: sourceSession, masterSession, runId, seq/messageId, location, modelTimeUtc/simTimeUtc/publishedAtUtc, mode/condition/isDaylight; Messschlüssel temperature_c, irradiation_w_m2, wind_speed_m_s, precipitation_mm_h, cloud_cover_percent und sun_height_degrees, 2026-11-08 empfangene Wettertopics: energytwin/weather/salzburg/measurements, energytwin/weather/wien/measurements, energytwin/weather/innsbruck/measurements, energytwin/weather/tamanrasset/measurements und energytwin/weather/neumayer/measurements; automatische Werte für Modellstunde 2026-12-21T12:00Z, RunId 1 (+4 more)

### Community 58 - "Kühlhaus"
Cohesion: 0.25
Nodes (3): ConsoleRenderer, IsInteractive, Width

### Community 59 - "Gemeinsame Bausteine"
Cohesion: 0.18
Nodes (9): IrradiationCalculator, WeatherCondition, Night, Overcast, PartlyCloudy, Snow, Storm, Sunny (+1 more)

### Community 61 - "Kühlhaus"
Cohesion: 0.14
Nodes (14): SimulationConfig, CoolingPowerKW, DeviceId, Duration, DurationHours, IsShortRun, Modbus, OutputDirectory (+6 more)

### Community 62 - "Gemeinsame Bausteine"
Cohesion: 0.22
Nodes (6): Season, Autumn, Spring, Summer, Winter, SeasonInfo

### Community 63 - "Simulationsverträge"
Cohesion: 0.19
Nodes (9): Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk, coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio (+1 more)

### Community 64 - "Kühlhaus"
Cohesion: 0.15
Nodes (10): KuehlhausSimulator, DeviceState, Cooling, Fault, Idle, Off, FaultType, CompressorFailure (+2 more)

### Community 66 - "Modbus-Register"
Cohesion: 0.15
Nodes (11): Befehle read/header/watch/write/force/raw/info/connect, Fehlerarten VERBINDUNGS-/PROTOKOLL-/WERTEFEHLER, Exit-Codes 0/1/2/64, Schreiben und Prüfen: 200 ms warten, Register 18/19 zurücklesen, DeviceType, Battery, Charger, IndustrialConsumer, PvPlant (+3 more)

### Community 67 - "Virtuelle Zeit"
Cohesion: 0.23
Nodes (6): ClockOffsetEstimator, HasSample, Offset, RoundTrip, Uncertainty, ClockOffsetEstimatorTests

### Community 68 - "DC-Ladesäule"
Cohesion: 0.15
Nodes (13): SimulationConfig, DeviceId, Duration, DurationHours, IsShortRun, Modbus, OutputDirectory, PowerLimitKW (+5 more)

### Community 69 - "PV-Simulation"
Cohesion: 0.15
Nodes (13): SimulationConfig, DeviceId, Location, Modbus, ModelStep, ModelStepMinutes, OutputDirectory, PeakPowerKw (+5 more)

### Community 70 - "Batteriesimulation"
Cohesion: 0.24
Nodes (3): ConsoleDashboard, BelowStatus, IsInteractive

### Community 72 - "Simulationsverträge"
Cohesion: 0.17
Nodes (7): WeatherCondition, Night, Overcast, PartlyCloudy, Snow, Storm, Sunny

### Community 73 - "Geräteanbindung"
Cohesion: 0.26
Nodes (4): WeatherOptions, Location, Specified, Url

### Community 74 - "DC-Ladesäule"
Cohesion: 0.17
Nodes (8): Vehicle, BatteryCapacityKWh, HasDamagedInlet, IsFull, MaxChargePowerKW, ModelName, SocPercent, TargetSocPercent

### Community 75 - "Kühlhaus"
Cohesion: 0.17
Nodes (11): KuehlhausConfig, AlarmHystereseK, AlarmTempC, COP, DeviceId, HystereseK, KaelteleistungKW, SollTempC (+3 more)

### Community 76 - "Modbus-Verbindung"
Cohesion: 0.21
Nodes (7): EasyModbusTCP.NETCore, Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk, MQTTnet, Microsoft.NET.Sdk, Microsoft.NET.Sdk

### Community 77 - "Wetter und Klima"
Cohesion: 0.17
Nodes (11): LastRequest, CloudCoverPercent, Condition, Count, IrradiationWm2, Location, PrecipitationMmPerHour, SimTimeUtc (+3 more)

### Community 79 - "Wetter und Klima"
Cohesion: 0.20
Nodes (5): Weather.Tests, WeatherSimulator.Simulation, ClimateProfile, Antarctic, Sahara

### Community 80 - "PV-Simulation"
Cohesion: 0.25
Nodes (3): EnergySimulator.Data, EnergySimulator.Simulation, EnergySimulator.Output

### Community 81 - "DC-Ladesäule"
Cohesion: 0.35
Nodes (4): DualChargerSim, DualChargerSim.Models, DualChargerSim.Simulation, DualChargerSim.IO

### Community 82 - "Batteriesimulation"
Cohesion: 0.20
Nodes (4): FaultType, EmergencyStop, None, Overheat

### Community 83 - "Geräteanbindung"
Cohesion: 0.24
Nodes (7): ModbusOptions, Bind, BindAddress, Disabled, Port, PrintRegisterPlan, UnitId

### Community 84 - "Wetter und Klima"
Cohesion: 0.20
Nodes (7): WeatherOptions, BrokerHost, BrokerPort, EnvFile, Mqtt, Port, TimeSync

### Community 85 - "PV-Simulation"
Cohesion: 0.22
Nodes (5): EnergySimulator.Devices, Shared.Weather, InverterStatus, Fault, Ok

### Community 86 - "Wetterprüfungen"
Cohesion: 0.20
Nodes (8): WeatherSynth, MQTTnet, Microsoft.NET.Sdk, coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 87 - "Kühlhaus"
Cohesion: 0.31
Nodes (3): AutoModeController, DoorClosesAt, KuehlhausAutoModeTests

### Community 88 - "Simulationsverträge"
Cohesion: 0.20
Nodes (8): ISimClock, Mode, Now, RunId, Speed, SimMode, Paused, Running

### Community 89 - "Funktionsnachweise"
Cohesion: 0.22
Nodes (8): probelauf: Dominiks reproduzierbarer Funktionsnachweis, Probelauf-Abnahmekriterien A, Z und N, 17 erlebte Fehlerbilder beim Probelauf, Portmatrix und Startbefehle, probelauf: Ablauf und Aktionsgrenzen, Drei begrenzte Probelaufumfänge, Vorlage für Funktionsnachweise, Dominiks Entwicklungsjournal

### Community 90 - "Funktionsnachweise"
Cohesion: 0.22
Nodes (6): 2026-11-08 KH-01-Prüfablauf: Start 15:55:19, Port nach etwa 1,2 Sekunden erreichbar, Lesen/Schreiben/Gegenlesen, Gerät um 15:56:08 beendet, 2026-11-08 KH-01 Ausgangslesung: Typ IndustrialConsumer 5, Instanz/Unit 1, Standalone laufend x600, Kopf 0–19 und Geräteregister 20–31; Solltemperatur Register 20–21 bei −20 °C, Schreibfehler 0, 2026-11-08 Diagnose-Client meldet angenommenen KH-01-Sollwertwechsel −20 °C → −15 °C und Gerätewert −15 °C, 2026-11-08 KH-01 Gegenlesung bestätigt Solltemperatur −15 °C in Register 20–21, Schreibfehler 0 und letzte Schreibadresse 20; Modellzeit inzwischen 00:25, 2026-11-08 Kühlhaus-Protokoll: standalone x600; Modbus-Solltemperatur −15 °C angenommen; Türereignis und Kühlung sichtbar; Ende nach acht Modellstunden mit 2,53 kWh und CSV-Pfad; umgeleitete Statuszeile am Ende abgeschnitten, 2026-11-08 Dominiks probelauf-Selbsttest Umfang A: KH-01 standalone, Tempo 600, acht Modellstunden, Wetter off, Seed 42, Modbus 1505; A1–A4 und A7 dokumentiert; ausdrücklich kein LB02-/LB03-Gesamtnachweis

### Community 92 - "Batteriesimulation"
Cohesion: 0.22
Nodes (9): SimulatorOptions, Config, DurationHours, Modbus, Seed, Step, StepSeconds, TimeSync (+1 more)

### Community 93 - "PV-Simulation"
Cohesion: 0.22
Nodes (6): IDevice, DailyEnergyKwh, IsEnabled, Name, PowerKw, TotalEnergyKwh

### Community 98 - "PV-Simulation"
Cohesion: 0.29
Nodes (6): FaultType, GridVoltage, Inverter, None, Overheat, FaultTypeExtensions

### Community 100 - "Zeitverteilung"
Cohesion: 0.29
Nodes (5): ClientConnectionState, Connected, Disconnected, NotResponding, TimeSyncClientInfo

### Community 101 - "Gemeinsame Bausteine"
Cohesion: 0.29
Nodes (3): Shared.Output, BarChart, ConsoleText

### Community 102 - "Gateway-Prüfungen"
Cohesion: 0.29
Nodes (6): MQTTnet.Server, coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 104 - "Batteriesimulation"
Cohesion: 0.29
Nodes (7): DeviceState, Charging, Cooldown, Discharging, Fault, Off, Standby

### Community 106 - "DC-Ladesäule"
Cohesion: 0.29
Nodes (6): ChargingPointState, Available, Charging, Connected, Faulted, Finished

### Community 107 - "DC-Ladesäule"
Cohesion: 0.43
Nodes (3): VehicleCatalog, Models, VehicleModel

### Community 108 - "PV-Simulation"
Cohesion: 0.33
Nodes (6): PV Statuspanel (Leistung 1,37 kW, Herbst, Auto, x15, bewoelkt), Bild-Aktualisierung (R ruhiger / schneller) 3000 ms, PV Simulator Screenshot: Bild-Aktualisierung 3000 ms, Wartung: Module reinigen (Taste C), Verschmutzung auf 0,0 %, PV Statuspanel sonnig, 4,51 kW, x1800, PV Simulator Screenshot: Module gereinigt

### Community 109 - "PV-Simulation"
Cohesion: 0.33
Nodes (6): Simulation beendet: Tagesenergie 0,30 kWh, CSV pv-simulation.csv, Inverter FEHLER, Anlage GESTOERT, Leistung 1,19 kW rot, PV Simulator Screenshot: Inverter-Fehler (LB01), CSV-Pfad data/pv-simulation.csv (teilweise geschwaerzt), Laufzeit-Zusammenfassung 06.10.-08.10.2026, 52,00 kWh, PV Simulator Screenshot: Beendet, CSV gespeichert

### Community 110 - "Modbus-Register"
Cohesion: 0.33
Nodes (4): RegisterType, Float32, UInt16, UInt32

### Community 112 - "Projektübersicht"
Cohesion: 0.33
Nodes (5): coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 113 - "Projektübersicht"
Cohesion: 0.33
Nodes (5): coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 114 - "DC-Ladesäule"
Cohesion: 0.50
Nodes (5): Charger key legend (P/A/1-2/C/D/S/F/R/+-/O/Q), AUTO mode, Punkt 1 CHARGING VW ID.3 120 kW, Limit 150 kW, Charger Screenshot: Automatischer Modus, MANUELL mode: Punkt 1 CONNECTED, Punkt 2 FINISHED, message Laden gestoppt, Charger Screenshot: Manueller Modus

### Community 115 - "PV-Simulation"
Cohesion: 0.70
Nodes (5): Feldgeraete-Fenster: Batterie, Ladesaeule, PV-Anlage, Kuehlhaus, Modbus-Diagnose-Client Fenster (PvPlant Registerkopf), Screenshot: Gesamtsystem mit allen Programmen, Simulations-Master Fenster (x600, Port 5100, 4 Simulatoren verbunden), Wettersimulator Fenster (HTTP 8080, salzburg)

### Community 116 - "README-Pflege"
Cohesion: 0.40
Nodes (4): interface, default_prompt, display_name, short_description

### Community 118 - "PV-Simulation"
Cohesion: 0.40
Nodes (4): SimulationMode, Auto, Manual, Paused

### Community 119 - "Projektübersicht"
Cohesion: 0.80
Nodes (4): Start-Program(), Start-TwinService(), Test-Port(), Wait-Port()

### Community 121 - "DC-Ladesäule"
Cohesion: 0.50
Nodes (4): Raised limit 230 kW, Abgewiesen 8, Charger Screenshot: Limit Erhoehung, Punkt 1 FAULTED Tesla Model 3 62 C, Kuehlungsdefekt 85 C, Charger Screenshot: Ueberhitzung

## Ambiguous Edges - Review These
- `Veraltete Konfigurationsaussage: keine Umgebungsvariablen oder .env nötig` → `MqttCredentials.Resolve: gemeinsame Zugangsdaten für Gateway und Wetter`  [AMBIGUOUS]
  README.md · relation: conceptually_related_to

## Knowledge Gaps
- **652 isolated node(s):** `GridVoltage`, `Inverter`, `None`, `Overheat`, `DoorClosesAt` (+647 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 805 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **36 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Kuehlhaus` connect `Kühlhaus` to `Kühlhaus`, `Geräteprüfungen`, `Kühlhaus`, `Kühlhaus`, `Kühlhaus`, `Geräteprüfungen`, `Kühlhaus`?**
  _High betweenness centrality (0.045) - this node is a cross-community bridge._
- **What connects `GridVoltage`, `Inverter`, `None` to the rest of the system?**
  _652 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Geräteprüfungen` be split into smaller, more focused modules?**
  _Cohesion score 0.07289002557544758 - nodes in this community are weakly interconnected._
- **Why does `ModbusDeviceServer` connect `Geräteanbindung` to `Geräteprüfungen`, `Modbus-Register`, `Gateway-Prüfungen`, `Zeitverteilung`, `Modbus-Diagnose`, `Modbus-Register`, `Modbus-Register`, `Modbus-Register`, `Modbus-Register`, `Geräteanbindung`, `Virtuelle Zeit`, `Geräteanbindung`?**
  _High betweenness centrality (0.044) - this node is a cross-community bridge._
- **Should `Zeitverteilung` be split into smaller, more focused modules?**
  _Cohesion score 0.06784260515603799 - nodes in this community are weakly interconnected._
- **Why does `MasterOptions` connect `Simulations-Master` to `Zeitverteilung`, `DC-Ladesäule`, `Geräteanbindung`?**
  _High betweenness centrality (0.037) - this node is a cross-community bridge._

### Low-confidence Hints
_AMBIGUOUS edges — the extractor was unsure. Verify before acting on these._

- **What is the exact relationship between `Veraltete Konfigurationsaussage: keine Umgebungsvariablen oder .env nötig` and `MqttCredentials.Resolve: gemeinsame Zugangsdaten für Gateway und Wetter`?**
  _Edge tagged AMBIGUOUS (relation: conceptually_related_to) - confidence is low._