---
type: community
cohesion: 0.22
members: 13
---

# Gruppe 58

**Cohesion:** 0.22 - loosely connected
**Members:** 13 nodes

## Members
- [[AutoModeController_1]] - code - src/Devices/KuehlhausSimulator/Simulation/AutoModeController.cs
- [[AutoModeController.DoorProbability()]] - code - src/Devices/KuehlhausSimulator/Simulation/AutoModeController.cs
- [[AutoModeController.DoorRatePerHour()]] - code - src/Devices/KuehlhausSimulator/Simulation/AutoModeController.cs
- [[AutoModeController.Step()_1]] - code - src/Devices/KuehlhausSimulator/Simulation/AutoModeController.cs
- [[DoorClosesAt]] - code - src/Devices/KuehlhausSimulator/Simulation/AutoModeController.cs
- [[Kuehlhaus.SetDoor()]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[KuehlhausAutoModeTests]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausAutoModeTests.CountOpenSteps()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausAutoModeTests.DoorRate_IsHigherDuringTheDayThanAtNight()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausAutoModeTests.SameSeed_OpensTheDoorTheSameWay()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausSimulatorSimulationAutoModeController.cs]] - code - src/Devices/KuehlhausSimulator/Simulation/AutoModeController.cs
- [[SimulationEngine.ToggleDoor()]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.ToggleMode()]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Gruppe_58
SORT file.name ASC
```

## Connections to other communities
- 3 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 3 edges to [[_COMMUNITY_Kühlhaus-Modell]]
- 3 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 2 edges to [[_COMMUNITY_Gruppe 57]]
- 2 edges to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 2 edges to [[_COMMUNITY_WeatherFeed]]
- 2 edges to [[_COMMUNITY_Gruppe 51]]
- 2 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_Gruppe 59]]
- 1 edge to [[_COMMUNITY_Gruppe 43]]

## Top bridge nodes
- [[AutoModeController_1]] - degree 9, connects to 4 communities
- [[AutoModeController.Step()_1]] - degree 7, connects to 3 communities
- [[Kuehlhaus.SetDoor()]] - degree 6, connects to 3 communities
- [[KuehlhausAutoModeTests.CountOpenSteps()]] - degree 6, connects to 3 communities
- [[SimulationEngine.ToggleDoor()]] - degree 3, connects to 2 communities