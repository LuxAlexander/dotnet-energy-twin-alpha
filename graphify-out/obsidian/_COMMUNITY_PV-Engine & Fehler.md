---
type: community
cohesion: 0.11
members: 23
---

# PV-Engine & Fehler

**Cohesion:** 0.11 - loosely connected
**Members:** 23 nodes

## Members
- [[ConsoleKey]] - code
- [[ModbusStatus_3]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[Mode_1]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[Note_1]] - code
- [[Pv]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[PvFaultModel.CleanModules()]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvFaultModel.ToggleForceOverheat()]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvFaultModel.ToggleGridDisturbance()]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvFaultModel.ToggleInverterFault()]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[RunId_4]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine_2]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.CleanModules()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.CycleRefresh()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.Draw()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.HandleKey()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.StatusLine()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.ToggleFault()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.ToggleGrid()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.ToggleMode()_1]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.ToggleOverheat()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationTime_3]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[Time_3]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[WeatherStatus_3]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/PV-Engine__Fehler
SORT file.name ASC
```

## Connections to other communities
- 14 edges to [[_COMMUNITY_WeatherFeed]]
- 5 edges to [[_COMMUNITY_PvFaultModel]]
- 5 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 4 edges to [[_COMMUNITY_DeviceTimeSource]]
- 3 edges to [[_COMMUNITY_Gruppe 47]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_StepScheduler]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]
- 1 edge to [[_COMMUNITY_PvSystem]]
- 1 edge to [[_COMMUNITY_ArgsParser]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_CsvLogger]]
- 1 edge to [[_COMMUNITY_Gruppe 43]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests]]
- 1 edge to [[_COMMUNITY_Gruppe 51]]
- 1 edge to [[_COMMUNITY_Gruppe 49]]

## Top bridge nodes
- [[SimulationEngine_2]] - degree 42, connects to 16 communities
- [[SimulationEngine.HandleKey()]] - degree 13, connects to 4 communities
- [[SimulationEngine.Draw()]] - degree 4, connects to 3 communities
- [[PvFaultModel.ToggleInverterFault()]] - degree 3, connects to 2 communities
- [[SimulationEngine.CleanModules()]] - degree 4, connects to 1 community