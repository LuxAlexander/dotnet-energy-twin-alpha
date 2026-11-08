---
type: community
cohesion: 0.16
members: 22
---

# PV-Engine & Fehler

**Cohesion:** 0.16 - loosely connected
**Members:** 22 nodes

## Members
- [[ConsoleKey]] - code
- [[ModbusStatus]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[Mode_1]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[Note_1]] - code
- [[Note]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[Pv]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[RunId_1]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.CleanModules()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.CycleRefresh()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.Draw()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.HandleKey()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.SetNotification()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.SetWeather()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.StatusLine()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.ToggleFault()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.ToggleGrid()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.ToggleMode()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.ToggleOverheat()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationTime]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[Time]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[WeatherStatus]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/PV-Engine__Fehler
SORT file.name ASC
```

## Connections to other communities
- 11 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 5 edges to [[_COMMUNITY_PvFaultModel]]
- 5 edges to [[_COMMUNITY_Gruppe 38]]
- 2 edges to [[_COMMUNITY_Shared Konsolenausgabe]]
- 2 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource (2)]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]
- 1 edge to [[_COMMUNITY_PvSystem]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]
- 1 edge to [[_COMMUNITY_Gruppe 104]]
- 1 edge to [[_COMMUNITY_ArgsParser]]
- 1 edge to [[_COMMUNITY_StepScheduler]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (4)]]
- 1 edge to [[_COMMUNITY_Gruppe 67]]

## Top bridge nodes
- [[SimulationEngine]] - degree 42, connects to 17 communities
- [[SimulationEngine.HandleKey()]] - degree 13, connects to 3 communities
- [[SimulationEngine.Draw()]] - degree 4, connects to 3 communities
- [[SimulationEngine.SetNotification()]] - degree 14, connects to 2 communities
- [[SimulationEngine.SetWeather()]] - degree 5, connects to 1 community