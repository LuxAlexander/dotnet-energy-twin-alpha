---
type: community
cohesion: 0.18
members: 18
---

# PV-Namespaces

**Cohesion:** 0.18 - loosely connected
**Members:** 18 nodes

## Members
- [[ConsoleDisplay.cs]] - code - src/Devices/PvSimulator/Output/ConsoleDisplay.cs
- [[CsvExporter.cs]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[EnergySimulator.Data]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[EnergySimulator.Devices]] - code - src/Devices/PvSimulator/Devices/FaultType.cs
- [[EnergySimulator.Output]] - code - src/Devices/PvSimulator/Output/ConsoleDisplay.cs
- [[EnergySimulator.Simulation]] - code - src/Devices/PvSimulator/Simulation/ArgsParser.cs
- [[Fault_2]] - code - src/Devices/PvSimulator/Devices/InverterStatus.cs
- [[FieldDeviceTests.cs]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[FieldDevices.Tests]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[InverterStatus]] - code - src/Devices/PvSimulator/Devices/InverterStatus.cs
- [[InverterStatus.cs]] - code - src/Devices/PvSimulator/Devices/InverterStatus.cs
- [[Ok]] - code - src/Devices/PvSimulator/Devices/InverterStatus.cs
- [[PvEngineTests]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvFaultModel.cs]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvSimulatorProgram.cs]] - code - src/Devices/PvSimulator/Program.cs
- [[PvSimulatorSimulationSimulationEngine.cs]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[PvSystem.cs]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[Shared.Weather]] - code - src/Shared/Weather/IrradiationCalculator.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/PV-Namespaces
SORT file.name ASC
```

## Connections to other communities
- 7 edges to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 4 edges to [[_COMMUNITY_Modbus-Tests]]
- 3 edges to [[_COMMUNITY_Gruppe 40]]
- 3 edges to [[_COMMUNITY_Shared Konsolenausgabe]]
- 3 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_Gruppe 74]]
- 2 edges to [[_COMMUNITY_WeatherFeed]]
- 2 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 1 edge to [[_COMMUNITY_Gruppe 75]]
- 1 edge to [[_COMMUNITY_Gruppe 88]]
- 1 edge to [[_COMMUNITY_Gruppe 73]]
- 1 edge to [[_COMMUNITY_CsvLogger]]
- 1 edge to [[_COMMUNITY_Gruppe 67]]
- 1 edge to [[_COMMUNITY_PvFaultModel]]
- 1 edge to [[_COMMUNITY_PvSystem]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests]]
- 1 edge to [[_COMMUNITY_ArgsParser]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_Charger-Namespaces]]
- 1 edge to [[_COMMUNITY_Gruppe 89]]
- 1 edge to [[_COMMUNITY_Gruppe 49]]
- 1 edge to [[_COMMUNITY_Gruppe 47]]
- 1 edge to [[_COMMUNITY_TimeSyncOptions Tests]]
- 1 edge to [[_COMMUNITY_Gruppe 43]]
- 1 edge to [[_COMMUNITY_Master-Zeit Tests]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop]]

## Top bridge nodes
- [[FieldDeviceTests.cs]] - degree 21, connects to 12 communities
- [[PvSimulatorSimulationSimulationEngine.cs]] - degree 10, connects to 4 communities
- [[EnergySimulator.Devices]] - degree 10, connects to 3 communities
- [[EnergySimulator.Simulation]] - degree 9, connects to 3 communities
- [[Shared.Weather]] - degree 8, connects to 3 communities