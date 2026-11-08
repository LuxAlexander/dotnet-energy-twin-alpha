---
type: community
cohesion: 0.18
members: 17
---

# PV-Namespaces

**Cohesion:** 0.18 - loosely connected
**Members:** 17 nodes

## Members
- [[ConsoleDisplay.cs]] - code - src/Devices/PvSimulator/Output/ConsoleDisplay.cs
- [[CsvExporter.cs]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[EnergySimulator.Data]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[EnergySimulator.Devices]] - code - src/Devices/PvSimulator/Devices/FaultType.cs
- [[EnergySimulator.Output]] - code - src/Devices/PvSimulator/Output/ConsoleDisplay.cs
- [[EnergySimulator.Simulation]] - code - src/Devices/PvSimulator/Simulation/ArgsParser.cs
- [[Fault]] - code - src/Devices/PvSimulator/Devices/InverterStatus.cs
- [[InverterStatus]] - code - src/Devices/PvSimulator/Devices/InverterStatus.cs
- [[InverterStatus.cs]] - code - src/Devices/PvSimulator/Devices/InverterStatus.cs
- [[Ok]] - code - src/Devices/PvSimulator/Devices/InverterStatus.cs
- [[PvFaultModel.cs]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvSimulatorProgram.cs]] - code - src/Devices/PvSimulator/Program.cs
- [[PvSimulatorSimulationSimulationConfig.cs]] - code - src/Devices/PvSimulator/Simulation/SimulationConfig.cs
- [[PvSimulatorSimulationSimulationEngine.cs]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[PvSystem.cs]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[Shared.Weather]] - code - src/Shared/Weather/IrradiationCalculator.cs
- [[SimulationArgsParser.cs]] - code - src/Devices/PvSimulator/Simulation/ArgsParser.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/PV-Namespaces
SORT file.name ASC
```

## Connections to other communities
- 6 edges to [[_COMMUNITY_Modbus-Tests]]
- 4 edges to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 4 edges to [[_COMMUNITY_Gruppe 19]]
- 3 edges to [[_COMMUNITY_Shared Konsolenausgabe]]
- 2 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 2 edges to [[_COMMUNITY_Gruppe 38]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop (2)]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (4)]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_PvSystem]]
- 1 edge to [[_COMMUNITY_PvFaultModel]]
- 1 edge to [[_COMMUNITY_ArgsParser (2)]]
- 1 edge to [[_COMMUNITY_ArgsParser]]
- 1 edge to [[_COMMUNITY_Gruppe 102]]
- 1 edge to [[_COMMUNITY_Gruppe 91]]
- 1 edge to [[_COMMUNITY_Gruppe 67]]
- 1 edge to [[_COMMUNITY_Gruppe 101]]

## Top bridge nodes
- [[PvSimulatorSimulationSimulationEngine.cs]] - degree 10, connects to 4 communities
- [[SimulationArgsParser.cs]] - degree 5, connects to 4 communities
- [[EnergySimulator.Devices]] - degree 10, connects to 3 communities
- [[PvSystem.cs]] - degree 5, connects to 3 communities
- [[PvSimulatorSimulationSimulationConfig.cs]] - degree 4, connects to 3 communities