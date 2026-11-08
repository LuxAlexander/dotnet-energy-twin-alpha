---
type: community
cohesion: 0.07
members: 39
---

# WeatherClient & Anzeige

**Cohesion:** 0.07 - loosely connected
**Members:** 39 nodes

## Members
- [[BaseUrl]] - code - src/Devices.Common/Weather/WeatherClient.cs
- [[Config_3]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[ConsoleRenderer_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.Bar()_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.BelowStatus()_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.BuildLines()_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.BuildStatusLines()]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.Cut()]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.DescribeDoor()]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.DrawAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.F()_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.Fit()_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.PromptAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.ShutdownAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[DoorClosesAt_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[EndTime_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[FormattableString]] - code
- [[HttpClient]] - code
- [[IsAutoMode_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[IsFinished_2]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[IsInteractive_2]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[IsRunning_2]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[JsonSerializerOptions]] - code
- [[Message_2]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[ModbusStatus_2]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[OutsideSource]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[Room]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[RunId_3]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.Quit()_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.TrySetSetpoint()]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationTime_2]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SyncProtocol]] - code - src/SimCore.Contracts/SyncProtocol.cs
- [[SyncProtocol.IsInvalid()]] - code - src/SimCore.Contracts/SyncProtocol.cs
- [[Time_2]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[WeatherClient]] - code - src/Devices.Common/Weather/WeatherClient.cs
- [[WeatherClient.Dispose()]] - code - src/Devices.Common/Weather/WeatherClient.cs
- [[WeatherStatus_2]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[Width_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/WeatherClient__Anzeige
SORT file.name ASC
```

## Connections to other communities
- 18 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 5 edges to [[_COMMUNITY_WeatherFeed]]
- 3 edges to [[_COMMUNITY_CsvLogger]]
- 3 edges to [[_COMMUNITY_Gruppe 58]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_Gruppe 53]]
- 2 edges to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 2 edges to [[_COMMUNITY_Register-Dekodierung]]
- 2 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_StepScheduler]]
- 1 edge to [[_COMMUNITY_Gruppe 44]]
- 1 edge to [[_COMMUNITY_Gruppe 41]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_Gruppe 66]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_Gruppe 45]]
- 1 edge to [[_COMMUNITY_Kühlhaus-Modell]]
- 1 edge to [[_COMMUNITY_Gruppe 46]]
- 1 edge to [[_COMMUNITY_Gruppe 59]]
- 1 edge to [[_COMMUNITY_Gruppe 51]]
- 1 edge to [[_COMMUNITY_TimeState & TimeLine]]

## Top bridge nodes
- [[SimulationEngine_1]] - degree 44, connects to 12 communities
- [[WeatherClient]] - degree 12, connects to 6 communities
- [[ConsoleRenderer_1]] - degree 18, connects to 3 communities
- [[ConsoleRenderer.BuildLines()_1]] - degree 8, connects to 2 communities
- [[ConsoleRenderer.PromptAsync()_1]] - degree 8, connects to 2 communities