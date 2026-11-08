---
type: community
cohesion: 0.08
members: 42
---

# WeatherClient & Anzeige

**Cohesion:** 0.08 - loosely connected
**Members:** 42 nodes

## Members
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
- [[ConsoleRenderer.InitializeAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.PromptAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.ShutdownAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[CultureInfo]] - code
- [[DoorClosesAt_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[EndTime_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[InputHandler_1]] - code - src/Devices/KuehlhausSimulator/IO/InputHandler.cs
- [[InputHandler.AskSetpointAsync()]] - code - src/Devices/KuehlhausSimulator/IO/InputHandler.cs
- [[InputHandler.HandleAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/InputHandler.cs
- [[InputHandler.HandlePendingKeysAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/InputHandler.cs
- [[IsAutoMode_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[IsFinished_2]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[IsInteractive_3]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[IsRunning_2]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[KuehlhausEngineTests.Kuehlhaus_RunsStandalone_WritesCsvAndStopsOnCancel()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[Message_2]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[ModbusStatus_3]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[OutsideSource]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[Program_1]] - code - src/Devices/KuehlhausSimulator/Program.cs
- [[Program.Main()_1]] - code - src/Devices/KuehlhausSimulator/Program.cs
- [[Room]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[RunId_7]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine_2]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.LocalOutsideTemperature()]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.Quit()_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.RunAsync()_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.TrySetSetpoint()]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationTime_3]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[Time_3]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[WeatherStatus_3]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[Width_2]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/WeatherClient__Anzeige
SORT file.name ASC
```

## Connections to other communities
- 12 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 11 edges to [[_COMMUNITY_TimeSyncServer]]
- 7 edges to [[_COMMUNITY_Batterie-Simulator Loop (2)]]
- 5 edges to [[_COMMUNITY_Gruppe 61]]
- 4 edges to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 4 edges to [[_COMMUNITY_Gruppe 109]]
- 3 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 3 edges to [[_COMMUNITY_Gruppe 52]]
- 3 edges to [[_COMMUNITY_DeviceTimeSource (2)]]
- 2 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource]]
- 2 edges to [[_COMMUNITY_Gruppe 65]]
- 2 edges to [[_COMMUNITY_Gruppe 21]]
- 2 edges to [[_COMMUNITY_Gruppe 69]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 2 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 1 edge to [[_COMMUNITY_WeatherFeed]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_StepScheduler]]
- 1 edge to [[_COMMUNITY_Kühlhaus-Modell]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_Gruppe 66]]
- 1 edge to [[_COMMUNITY_Gruppe 104]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_Gruppe 101]]
- 1 edge to [[_COMMUNITY_Gruppe 81]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 23]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_GatewayService]]

## Top bridge nodes
- [[SimulationEngine_2]] - degree 44, connects to 15 communities
- [[CultureInfo]] - degree 18, connects to 12 communities
- [[SimulationEngine.RunAsync()_1]] - degree 18, connects to 6 communities
- [[InputHandler.HandleAsync()_1]] - degree 11, connects to 5 communities
- [[InputHandler_1]] - degree 9, connects to 3 communities