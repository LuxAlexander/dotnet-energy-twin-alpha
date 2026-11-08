---
type: community
cohesion: 0.21
members: 22
---

# Bibliotheks-Namespaces

**Cohesion:** 0.21 - loosely connected
**Members:** 22 nodes

## Members
- [[BatteryModbus.cs]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[BatterySimulatorArgsParser.cs]] - code - src/Devices/BatterySimulator/ArgsParser.cs
- [[ConsumerTests.cs]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[Devices.Common]] - code - src/Devices.Common/AsyncConsole.cs
- [[Devices.Common.Modbus]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[Devices.Common.Weather]] - code - src/Devices.Common/Weather/WeatherClient.cs
- [[DualPortChargerSimulationSimulationConfig.cs]] - code - src/Devices/DualPortCharger/Simulation/SimulationConfig.cs
- [[KuehlhausModbus.cs]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[KuehlhausSimulator.IO]] - code - src/Devices/KuehlhausSimulator/IO/ArgsParser.cs
- [[KuehlhausSimulator.Simulation]] - code - src/Devices/KuehlhausSimulator/Simulation/AutoModeController.cs
- [[KuehlhausSimulatorIOArgsParser.cs]] - code - src/Devices/KuehlhausSimulator/IO/ArgsParser.cs
- [[KuehlhausSimulatorIOConsoleRenderer.cs]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[KuehlhausSimulatorIOInputHandler.cs]] - code - src/Devices/KuehlhausSimulator/IO/InputHandler.cs
- [[KuehlhausSimulatorProgram.cs]] - code - src/Devices/KuehlhausSimulator/Program.cs
- [[KuehlhausSimulatorSimulationSimulationConfig.cs]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationConfig.cs
- [[KuehlhausSimulatorSimulationSimulationEngine.cs]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[PvSimulatorSimulationSimulationConfig.cs]] - code - src/Devices/PvSimulator/Simulation/SimulationConfig.cs
- [[Shared.Config]] - code - src/Shared.Config/OutputPaths.cs
- [[SimulationArgsParser.cs]] - code - src/Devices/PvSimulator/Simulation/ArgsParser.cs
- [[Simulator.cs]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[SimulatorOptions.cs]] - code - src/Devices/BatterySimulator/SimulatorOptions.cs
- [[modbus_contracts_batteryregisters]] - concept

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Bibliotheks-Namespaces
SORT file.name ASC
```

## Connections to other communities
- 17 edges to [[_COMMUNITY_Charger-Namespaces]]
- 12 edges to [[_COMMUNITY_Modbus-Tests]]
- 8 edges to [[_COMMUNITY_Gruppe 40]]
- 7 edges to [[_COMMUNITY_PV-Namespaces]]
- 4 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 3 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 3 edges to [[_COMMUNITY_Gruppe 45]]
- 2 edges to [[_COMMUNITY_Gruppe 53]]
- 2 edges to [[_COMMUNITY_Gruppe 50]]
- 2 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 2 edges to [[_COMMUNITY_Gruppe 58]]
- 2 edges to [[_COMMUNITY_ArgsParser]]
- 1 edge to [[_COMMUNITY_StepScheduler]]
- 1 edge to [[_COMMUNITY_WeatherFeed]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_Gruppe 75]]
- 1 edge to [[_COMMUNITY_Gruppe 44]]
- 1 edge to [[_COMMUNITY_Gruppe 71]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_Gruppe 73]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_CsvLogger]]
- 1 edge to [[_COMMUNITY_Gruppe 46]]
- 1 edge to [[_COMMUNITY_Gruppe 92]]
- 1 edge to [[_COMMUNITY_Gruppe 57]]

## Top bridge nodes
- [[Devices.Common]] - degree 27, connects to 7 communities
- [[ConsumerTests.cs]] - degree 12, connects to 7 communities
- [[Devices.Common.Weather]] - degree 15, connects to 6 communities
- [[Devices.Common.Modbus]] - degree 18, connects to 4 communities
- [[Simulator.cs]] - degree 6, connects to 4 communities