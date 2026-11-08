# Graph Report - dotnet-energy-twin-alpha  (2026-11-04)

## Corpus Check
- 181 files · ~93,678 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 7 file(s) not represented in the graph (top: .props 3, (none) 2, .csv 2)

## Summary
- 1724 nodes · 4104 edges · 95 communities (79 shown, 16 thin omitted)
- Extraction: 86% EXTRACTED · 14% INFERRED · 0% AMBIGUOUS · INFERRED: 565 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- Batterie-Modbus & Update
- Zeitsync-Client & Konsole
- Wettermodell & Tests
- Batterie-Simulator Loop
- Projekt-Doku & Geräte
- Devices.Common Tests
- Diagnose-Client Verbindung
- Sync-Nachrichten
- WeatherClient & Anzeige
- Geräte-READMEs & Modelle
- RegisterCodec & Befehle
- TimeSyncServer
- Shared Konsolenausgabe
- WeatherFeed
- Zeit-Bibliotheken & Master
- MasterClock
- DeviceTimeSource
- CsvLogger
- Registerpläne
- Master-Zeit Tests
- PV-Engine & Fehler
- Schreibprüfung
- StepScheduler
- Bibliotheks-Namespaces
- Kühlhaus-Modell
- TimeState & TimeLine
- PvSystem
- Batterie-Modell
- Ladepunkt
- PvFaultModel
- ArgsParser
- SyncedClock
- TimeSyncOptions Tests
- Charger-Namespaces
- PV-Namespaces
- DeviceConnection
- Register-Dekodierung
- Fahrzeug
- DualCharger
- Modbus-Tests
- Gruppe 40
- Gruppe 41
- Gruppe 42
- Gruppe 43
- Gruppe 44
- Gruppe 45
- Gruppe 46
- Gruppe 47
- Gruppe 48
- Gruppe 49
- Gruppe 50
- Gruppe 51
- Gruppe 52
- Gruppe 53
- Gruppe 54
- Gruppe 55
- Gruppe 56
- Gruppe 57
- Gruppe 58
- Gruppe 59
- Gruppe 60
- Gruppe 61
- Gruppe 62
- Gruppe 63
- Gruppe 64
- Gruppe 65
- Gruppe 66
- Gruppe 67
- Gruppe 68
- Gruppe 69
- Gruppe 70
- Gruppe 71
- Gruppe 72
- Gruppe 73
- Gruppe 74
- Gruppe 75
- Gruppe 76
- Gruppe 77
- Gruppe 78
- Gruppe 79
- Gruppe 80
- Gruppe 82
- Gruppe 83
- Gruppe 84
- Gruppe 85
- Gruppe 86
- Gruppe 87
- Gruppe 88
- Gruppe 89
- Gruppe 91
- Gruppe 92
- Gruppe 93
- Gruppe 94

## God Nodes (most connected - your core abstractions)
1. `MasterClock` - 50 edges
2. `SimulationEngine` - 44 edges
3. `SyncedClock` - 44 edges
4. `SimulationEngine` - 42 edges
5. `Simulator` - 40 edges
6. `SimulationEngine` - 39 edges
7. `ModbusDeviceServer` - 37 edges
8. `TimeSyncServer` - 37 edges
9. `DeviceTimeSource` - 36 edges
10. `Batterie` - 36 edges

## Surprising Connections (you probably didn't know these)
- `ModbusDeviceServerTests` --calls--> `ModbusOptions`  [INFERRED]
  tests/Devices.Common.Tests/ModbusDeviceServerTests.cs → src/Devices.Common/Modbus/ModbusOptions.cs
- `BatteryModbusTests` --calls--> `TimeSyncOptions`  [INFERRED]
  tests/FieldDevices.Tests/FieldDeviceTests.cs → src/Devices.Common/TimeSyncOptions.cs
- `BatteryModbusTests` --calls--> `DailyProfile`  [INFERRED]
  tests/FieldDevices.Tests/FieldDeviceTests.cs → src/Devices/BatterySimulator/DailyProfile.cs
- `BatteryModbusTests` --calls--> `SimulatorOptions`  [INFERRED]
  tests/FieldDevices.Tests/FieldDeviceTests.cs → src/Devices/BatterySimulator/SimulatorOptions.cs
- `Journal Alexander Lux` --references--> `PvSimulator`  [INFERRED]
  docs/development-journal/alexander-lux.md → AGENTS.md

## Import Cycles
- None detected.

## Communities (95 total, 16 thin omitted)

### Community 0 - "Batterie-Modbus & Update"
Cohesion: 0.06
Nodes (28): BatteryModbus, StatusText, HeaderValues, ModbusDeviceServer, ClientCount, IsListening, LastWriteText, Options (+20 more)

### Community 1 - "Zeitsync-Client & Konsole"
Cohesion: 0.06
Nodes (25): ClientConnectionState, Connected, Disconnected, NotResponding, TimeSyncClientInfo, KeyResult, Handled, Quit (+17 more)

### Community 2 - "Wettermodell & Tests"
Cohesion: 0.07
Nodes (21): Weather.Tests, WeatherSimulator.Simulation, WeatherCondition, Night, Overcast, PartlyCloudy, Snow, Storm (+13 more)

### Community 3 - "Batterie-Simulator Loop"
Cohesion: 0.10
Nodes (6): AsyncConsole, HasKeyboard, InputHandler, CsvLogger, FilePath, InputHandler

### Community 4 - "Projekt-Doku & Geräte"
Cohesion: 0.06
Nodes (43): AGENTS1 Archiv (obsoletes LB02-Plan), AGENTS2 Archiv (Sitzungsprotokolle), BatterySimulator, Entwicklungsjournal, Devices.Common, ModbusDiagnosticClient, DualPortCharger, EnergyTwin LB02 Projekt (+35 more)

### Community 5 - "Devices.Common Tests"
Cohesion: 0.06
Nodes (44): DeviceTimeSourceTests, ModbusDeviceServerTests, Devices.Common.Tests, RegisterCodecTests, StepSchedulerTests, TimeSyncOptionsTests, ClientOptionsTests, DiagnosticClientTests (+36 more)

### Community 6 - "Diagnose-Client Verbindung"
Cohesion: 0.10
Nodes (10): DiagnosticErrorKind, Connection, Protocol, Value, DiagnosticException, Kind, DiagnosticSession, Connection (+2 more)

### Community 7 - "Sync-Nachrichten"
Cohesion: 0.12
Nodes (13): ByeMessage, HelloMessage, PingMessage, PongMessage, StatesMessage, SyncMessage, MessageChannel, RemoteEndPoint (+5 more)

### Community 8 - "WeatherClient & Anzeige"
Cohesion: 0.07
Nodes (21): WeatherClient, BaseUrl, ConsoleRenderer, IsInteractive, Width, SimulationEngine, Config, DoorClosesAt (+13 more)

### Community 9 - "Geräte-READMEs & Modelle"
Cohesion: 0.08
Nodes (35): BatterySimulator, Batterie Kommandozeilenargumente, SOC-Modell Batteriespeicher, Charger Kommandozeilenargumente, DualPortCharger, Gemeinsames Leistungsbudget der Ladepunkte, Diagnose-Client Befehle (read, write, force, watch), Standardports der Geraete (+27 more)

### Community 10 - "RegisterCodec & Befehle"
Cohesion: 0.14
Nodes (4): ModbusCommand, RegisterCodec, ModbusDeviceServerTests, RegisterCodecTests

### Community 11 - "TimeSyncServer"
Cohesion: 0.12
Nodes (6): TimeSyncServer, Clients, HeartbeatInterval, LocalEndPoint, Program, TimeSyncIntegrationTests

### Community 12 - "Shared Konsolenausgabe"
Cohesion: 0.10
Nodes (8): Shared.Output, ConsoleDisplay, SimulationMode, Auto, Manual, Paused, BarChart, ConsoleText

### Community 13 - "WeatherFeed"
Cohesion: 0.13
Nodes (12): WeatherUnavailableException, WeatherFeed, Last, Location, Status, Note, IWeatherService, WeatherData (+4 more)

### Community 14 - "Zeit-Bibliotheken & Master"
Cohesion: 0.16
Nodes (5): SimCore.TimeSync, SimCore.Tests, SimCore.Time, SimulationMaster, SimCore.Contracts

### Community 15 - "MasterClock"
Cohesion: 0.15
Nodes (11): MasterClock, Current, CurrentStates, Latest, LeadTime, MasterNow, Mode, Now (+3 more)

### Community 16 - "DeviceTimeSource"
Cohesion: 0.11
Nodes (13): DeviceTimeSource, Clock, HasTime, IsRunning, IsStandalone, Options, Status, StatusText (+5 more)

### Community 17 - "CsvLogger"
Cohesion: 0.11
Nodes (9): CsvLogger, FilePath, CsvLogger, FilePath, Program, CsvExporter, FilePath, CsvFileWriter (+1 more)

### Community 18 - "Registerpläne"
Cohesion: 0.09
Nodes (19): BatteryRegisters, Plan, ChargerRegisters, Plan, DeviceType, Battery, Charger, IndustrialConsumer (+11 more)

### Community 20 - "PV-Engine & Fehler"
Cohesion: 0.11
Nodes (8): SimulationEngine, ModbusStatus, Mode, Pv, RunId, SimulationTime, Time, WeatherStatus

### Community 21 - "Schreibprüfung"
Cohesion: 0.11
Nodes (11): RegisterAccess, ReadOnly, ReadWrite, RegisterDefinition, Count, EndAddress, IsWritable, RegisterType (+3 more)

### Community 22 - "StepScheduler"
Cohesion: 0.15
Nodes (12): StepScheduler, MaxCatchUpSteps, ModelTime, RunId, Step, StepUpdate, StepUpdateKind, Advance (+4 more)

### Community 23 - "Bibliotheks-Namespaces"
Cohesion: 0.21
Nodes (6): Shared.Config, Devices.Common, KuehlhausSimulator.IO, Devices.Common.Modbus, Devices.Common.Weather, KuehlhausSimulator.Simulation

### Community 24 - "Kühlhaus-Modell"
Cohesion: 0.11
Nodes (16): Kuehlhaus, ActiveFault, AlarmTempC, CompressorOn, CoolingPowerKW, DoorOpen, ElectricPowerKW, EnergyKWh (+8 more)

### Community 25 - "TimeState & TimeLine"
Cohesion: 0.16
Nodes (8): TimeState, TimeLine, Latest, States, ClientConnection, Channel, ClientId, IsOpen

### Community 26 - "PvSystem"
Cohesion: 0.10
Nodes (21): PvSystem, ActiveFault, AgingLossPercent, AmbientTemperatureC, CapacityPercent, DailyEnergyKwh, EfficiencyPercent, ExternalWeather (+13 more)

### Community 27 - "Batterie-Modell"
Cohesion: 0.13
Nodes (14): Batterie, ActiveFault, ActualKW, AmbientTemperatureC, ChargedKWh, Config, DeviceId, DischargeKWh (+6 more)

### Community 28 - "Ladepunkt"
Cohesion: 0.12
Nodes (11): ChargingPoint, AmbientTemperatureC, DeratingFactor, Efficiency, EnergyKWh, IsFree, Number, PowerKW (+3 more)

### Community 29 - "PvFaultModel"
Cohesion: 0.13
Nodes (12): PvFaultModel, ActiveFault, AgingLossPercent, AmbientTemperatureC, GridDisturbanceForced, GridVoltageV, HasFault, InverterFault (+4 more)

### Community 30 - "ArgsParser"
Cohesion: 0.12
Nodes (14): ArgsParser, SimulationConfig, DeviceId, Location, Modbus, ModelStep, ModelStepMinutes, OutputDirectory (+6 more)

### Community 31 - "SyncedClock"
Cohesion: 0.12
Nodes (17): SyncedClock, HeartbeatTimeout, LastLateness, LocalNow, MasterNow, Mode, Now, OffsetToMaster (+9 more)

### Community 33 - "Charger-Namespaces"
Cohesion: 0.19
Nodes (6): ModbusDiagnosticClient, DualChargerSim, DualChargerSim.Models, DualChargerSim.Simulation, DualChargerSim.IO, Program

### Community 34 - "PV-Namespaces"
Cohesion: 0.18
Nodes (10): EnergySimulator.Data, EnergySimulator.Simulation, EnergySimulator.Devices, EnergySimulator.Output, FieldDevices.Tests, Shared.Weather, InverterStatus, Fault (+2 more)

### Community 35 - "DeviceConnection"
Cohesion: 0.20
Nodes (7): DeviceConnection, Endpoint, Host, IsConnected, Port, TimeoutMs, UnitId

### Community 37 - "Fahrzeug"
Cohesion: 0.14
Nodes (11): Vehicle, BatteryCapacityKWh, HasDamagedInlet, IsFull, MaxChargePowerKW, ModelName, SocPercent, TargetSocPercent (+3 more)

### Community 38 - "DualCharger"
Cohesion: 0.13
Nodes (10): DualCharger, AmbientTemperatureC, Id, IsOn, Points, PowerLimitKW, QueueLength, RejectedVehicles (+2 more)

### Community 39 - "Modbus-Tests"
Cohesion: 0.15
Nodes (3): ModbusDiagnosticClient.Tests, Modbus.Contracts, Devices.Common.Tests

### Community 41 - "Gruppe 41"
Cohesion: 0.12
Nodes (15): SimulationEngine, Charger, Config, EndTime, IsAutoMode, IsFinished, IsRunning, Message (+7 more)

### Community 42 - "Gruppe 42"
Cohesion: 0.16
Nodes (9): ClientOptions, Host, IsOneShot, Once, Port, TimeoutMs, UnitId, Writes (+1 more)

### Community 43 - "Gruppe 43"
Cohesion: 0.16
Nodes (11): BatterieConfig, CapacityKWh, DeviceId, Efficiency, MaxChargeKW, MaxDischargeKW, SocMaxPercent, SocMinPercent (+3 more)

### Community 44 - "Gruppe 44"
Cohesion: 0.13
Nodes (14): ControlMode, Auto, Manual, Simulator, IsFinished, Message, ModbusStatus, Mode (+6 more)

### Community 45 - "Gruppe 45"
Cohesion: 0.22
Nodes (3): ArgsParser, KuehlhausArgsTests, KuehlhausEngineTests

### Community 46 - "Gruppe 46"
Cohesion: 0.14
Nodes (14): SimulationConfig, CoolingPowerKW, DeviceId, Duration, DurationHours, IsShortRun, Modbus, OutputDirectory (+6 more)

### Community 47 - "Gruppe 47"
Cohesion: 0.18
Nodes (8): WeatherCondition, Night, Overcast, PartlyCloudy, Snow, Storm, Sunny, WeatherConditionExtensions

### Community 48 - "Gruppe 48"
Cohesion: 0.14
Nodes (13): CommonRegisters, Header, OperatingState, Fault, Off, Running, Standby, WriteError (+5 more)

### Community 49 - "Gruppe 49"
Cohesion: 0.22
Nodes (6): Season, Autumn, Spring, Summer, Winter, SeasonInfo

### Community 50 - "Gruppe 50"
Cohesion: 0.15
Nodes (10): KuehlhausSimulator, DeviceState, Cooling, Fault, Idle, Off, FaultType, CompressorFailure (+2 more)

### Community 52 - "Gruppe 52"
Cohesion: 0.23
Nodes (4): WeatherOptions, Location, Specified, Url

### Community 54 - "Gruppe 54"
Cohesion: 0.23
Nodes (6): ClockOffsetEstimator, HasSample, Offset, RoundTrip, Uncertainty, ClockOffsetEstimatorTests

### Community 55 - "Gruppe 55"
Cohesion: 0.21
Nodes (8): TimeSyncOptions, HasStandaloneSettings, MasterEndpoint, MasterHost, MasterPort, Standalone, StandaloneSpeed, StandaloneStart

### Community 56 - "Gruppe 56"
Cohesion: 0.15
Nodes (13): SimulationConfig, DeviceId, Duration, DurationHours, IsShortRun, Modbus, OutputDirectory, PowerLimitKW (+5 more)

### Community 58 - "Gruppe 58"
Cohesion: 0.22
Nodes (3): AutoModeController, DoorClosesAt, KuehlhausAutoModeTests

### Community 59 - "Gruppe 59"
Cohesion: 0.17
Nodes (11): KuehlhausConfig, AlarmHystereseK, AlarmTempC, COP, DeviceId, HystereseK, KaelteleistungKW, SollTempC (+3 more)

### Community 60 - "Gruppe 60"
Cohesion: 0.18
Nodes (8): Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk, coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 61 - "Gruppe 61"
Cohesion: 0.17
Nodes (11): LastRequest, CloudCoverPercent, Condition, Count, IrradiationWm2, Location, PrecipitationMmPerHour, SimTimeUtc (+3 more)

### Community 62 - "Gruppe 62"
Cohesion: 0.18
Nodes (10): RABBITMQ_DEFAULT_PASS, RABBITMQ_DEFAULT_USER, command, container_name, environment, image, ports, volumes (+2 more)

### Community 63 - "Gruppe 63"
Cohesion: 0.18
Nodes (8): ISimClock, Mode, Now, RunId, Speed, SimMode, Paused, Running

### Community 64 - "Gruppe 64"
Cohesion: 0.24
Nodes (7): ModbusOptions, Bind, BindAddress, Disabled, Port, PrintRegisterPlan, UnitId

### Community 65 - "Gruppe 65"
Cohesion: 0.18
Nodes (8): Microsoft.NET.Sdk, EasyModbusTCP.NETCore, Microsoft.NET.Sdk, coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 66 - "Gruppe 66"
Cohesion: 0.40
Nodes (3): ConsoleRenderer, IsInteractive, Width

### Community 67 - "Gruppe 67"
Cohesion: 0.20
Nodes (6): IDevice, DailyEnergyKwh, IsEnabled, Name, PowerKw, TotalEnergyKwh

### Community 68 - "Gruppe 68"
Cohesion: 0.29
Nodes (5): Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk, Microsoft.NET.Sdk

### Community 69 - "Gruppe 69"
Cohesion: 0.22
Nodes (7): WeatherSynth, Microsoft.NET.Sdk, coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 70 - "Gruppe 70"
Cohesion: 0.28
Nodes (3): ConsoleDashboard, BelowStatus, IsInteractive

### Community 71 - "Gruppe 71"
Cohesion: 0.22
Nodes (9): SimulatorOptions, Config, DurationHours, Modbus, Seed, Step, StepSeconds, TimeSync (+1 more)

### Community 72 - "Gruppe 72"
Cohesion: 0.28
Nodes (9): Deterministic weather (location, time, run seed), Weather HTTP API (/weather, /locations), LB01 Season/IrradiationCalculator basis, Manual weather override keys, WeatherData, WeatherSimulator, Weather.Tests, Weather reproducibility requirement (+1 more)

### Community 73 - "Gruppe 73"
Cohesion: 0.25
Nodes (3): Shared.Data, CsvFormat, CsvSchema

### Community 74 - "Gruppe 74"
Cohesion: 0.29
Nodes (6): FaultType, GridVoltage, Inverter, None, Overheat, FaultTypeExtensions

### Community 76 - "Gruppe 76"
Cohesion: 0.29
Nodes (4): Microsoft.NET.Sdk, EasyModbusTCP.NETCore, Microsoft.NET.Sdk, Microsoft.NET.Sdk

### Community 77 - "Gruppe 77"
Cohesion: 0.29
Nodes (7): DeviceState, Charging, Cooldown, Discharging, Fault, Off, Standby

### Community 78 - "Gruppe 78"
Cohesion: 0.29
Nodes (6): ChargingPointState, Available, Charging, Connected, Faulted, Finished

### Community 79 - "Gruppe 79"
Cohesion: 0.33
Nodes (6): PV Statuspanel (Leistung 1,37 kW, Herbst, Auto, x15, bewoelkt), Bild-Aktualisierung (R ruhiger / schneller) 3000 ms, PV Simulator Screenshot: Bild-Aktualisierung 3000 ms, Wartung: Module reinigen (Taste C), Verschmutzung auf 0,0 %, PV Statuspanel sonnig, 4,51 kW, x1800, PV Simulator Screenshot: Module gereinigt

### Community 80 - "Gruppe 80"
Cohesion: 0.33
Nodes (6): Simulation beendet: Tagesenergie 0,30 kWh, CSV pv-simulation.csv, Inverter FEHLER, Anlage GESTOERT, Leistung 1,19 kW rot, PV Simulator Screenshot: Inverter-Fehler (LB01), CSV-Pfad data/pv-simulation.csv (teilweise geschwaerzt), Laufzeit-Zusammenfassung 06.10.-08.10.2026, 52,00 kWh, PV Simulator Screenshot: Beendet, CSV gespeichert

### Community 83 - "Gruppe 83"
Cohesion: 0.33
Nodes (5): coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 85 - "Gruppe 85"
Cohesion: 0.33
Nodes (5): coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 86 - "Gruppe 86"
Cohesion: 0.50
Nodes (5): Charger key legend (P/A/1-2/C/D/S/F/R/+-/O/Q), AUTO mode, Punkt 1 CHARGING VW ID.3 120 kW, Limit 150 kW, Charger Screenshot: Automatischer Modus, MANUELL mode: Punkt 1 CONNECTED, Punkt 2 FINISHED, message Laden gestoppt, Charger Screenshot: Manueller Modus

### Community 87 - "Gruppe 87"
Cohesion: 0.70
Nodes (5): Feldgeraete-Fenster: Batterie, Ladesaeule, PV-Anlage, Kuehlhaus, Modbus-Diagnose-Client Fenster (PvPlant Registerkopf), Screenshot: Gesamtsystem mit allen Programmen, Simulations-Master Fenster (x600, Port 5100, 4 Simulatoren verbunden), Wettersimulator Fenster (HTTP 8080, salzburg)

### Community 88 - "Gruppe 88"
Cohesion: 0.40
Nodes (4): FaultType, EmergencyStop, None, Overheat

### Community 91 - "Gruppe 91"
Cohesion: 0.50
Nodes (4): Raised limit 230 kW, Abgewiesen 8, Charger Screenshot: Limit Erhoehung, Punkt 1 FAULTED Tesla Model 3 62 C, Kuehlungsdefekt 85 C, Charger Screenshot: Ueberhitzung

## Knowledge Gaps
- **545 isolated node(s):** `HasKeyboard`, `Options`, `Clock`, `IsStandalone`, `Status` (+540 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 614 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **16 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SimulationEngine` connect `PV-Engine & Fehler` to `Batterie-Modbus & Update`, `PV-Namespaces`, `Wettermodell & Tests`, `Batterie-Simulator Loop`, `WeatherClient & Anzeige`, `Gruppe 43`, `Shared Konsolenausgabe`, `WeatherFeed`, `Gruppe 47`, `DeviceTimeSource`, `CsvLogger`, `Gruppe 51`, `StepScheduler`, `PvSystem`, `PvFaultModel`, `ArgsParser`?**
  _High betweenness centrality (0.059) - this node is a cross-community bridge._
- **Are the 18 inferred relationships involving `MasterClock` (e.g. with `DeviceTimeSource.Create()` and `TimeLine.Apply()`) actually correct?**
  _`MasterClock` has 18 INFERRED edges - model-reasoned connections that need verification._
- **What connects `HasKeyboard`, `Options`, `Clock` to the rest of the system?**
  _545 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Batterie-Modbus & Update` be split into smaller, more focused modules?**
  _Cohesion score 0.056134723336006415 - nodes in this community are weakly interconnected._
- **Why does `SimulationEngine` connect `WeatherClient & Anzeige` to `Batterie-Modbus & Update`, `Batterie-Simulator Loop`, `WeatherFeed`, `Gruppe 46`, `DeviceTimeSource`, `CsvLogger`, `Gruppe 51`, `StepScheduler`, `Bibliotheks-Namespaces`, `Kühlhaus-Modell`, `Gruppe 58`, `Gruppe 59`?**
  _High betweenness centrality (0.052) - this node is a cross-community bridge._
- **Should `Zeitsync-Client & Konsole` be split into smaller, more focused modules?**
  _Cohesion score 0.05747126436781609 - nodes in this community are weakly interconnected._
- **Why does `Simulator` connect `Gruppe 44` to `Batterie-Modbus & Update`, `Zeitsync-Client & Konsole`, `Batterie-Simulator Loop`, `Gruppe 70`, `Gruppe 71`, `WeatherClient & Anzeige`, `Gruppe 43`, `WeatherFeed`, `DeviceTimeSource`, `CsvLogger`, `Gruppe 81`, `Gruppe 51`, `StepScheduler`, `Bibliotheks-Namespaces`, `Batterie-Modell`?**
  _High betweenness centrality (0.051) - this node is a cross-community bridge._