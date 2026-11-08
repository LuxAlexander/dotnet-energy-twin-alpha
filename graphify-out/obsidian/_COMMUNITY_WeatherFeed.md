---
type: community
cohesion: 0.13
members: 31
---

# WeatherFeed

**Cohesion:** 0.13 - loosely connected
**Members:** 31 nodes

## Members
- [[Calls]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[CsvExporter.WriteRowAsync()]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[DateTime]] - code
- [[Fail]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[FakeWeatherService]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[FakeWeatherService.GetAsync()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[IWeatherService]] - code - src/SimCore.Contracts/IWeatherService.cs
- [[IWeatherService.GetAsync()]] - code - src/SimCore.Contracts/IWeatherService.cs
- [[IWeatherService.cs]] - code - src/SimCore.Contracts/IWeatherService.cs
- [[Last]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[Location]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[Note]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[PvSystem.ApplyExternalWeather()]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[SimulationEngine.AdvanceAsync()_2]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.LocalOutsideTemperature()]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.SetNotification()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.UpdateAutoWeather()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.UpdateWeatherAsync()_1]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[Status_1]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[WeatherClient.GetAsync()]] - code - src/Devices.Common/Weather/WeatherClient.cs
- [[WeatherData]] - code - src/SimCore.Contracts/WeatherData.cs
- [[WeatherFeed]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[WeatherFeed.Describe()]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[WeatherFeed.UpdateAsync()]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[WeatherFeed.cs]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[WeatherFeedTests]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherFeedTests.Failure_KeepsLastWeatherAndUpdatesStatus()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherFeedTests.Hour()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherFeedTests.NextHourAndOtherRun_FetchAgain()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherFeedTests.SameHour_FetchesOnlyOnce()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherUnavailableException]] - code - src/Devices.Common/Weather/WeatherClient.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/WeatherFeed
SORT file.name ASC
```

## Connections to other communities
- 29 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 14 edges to [[_COMMUNITY_PV-Engine & Fehler]]
- 11 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 10 edges to [[_COMMUNITY_Wettermodell & Tests]]
- 6 edges to [[_COMMUNITY_Gruppe 51]]
- 5 edges to [[_COMMUNITY_StepScheduler]]
- 5 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 5 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 5 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 4 edges to [[_COMMUNITY_CsvLogger]]
- 4 edges to [[_COMMUNITY_PvFaultModel]]
- 4 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 4 edges to [[_COMMUNITY_Gruppe 52]]
- 3 edges to [[_COMMUNITY_DualCharger]]
- 3 edges to [[_COMMUNITY_PvSystem]]
- 3 edges to [[_COMMUNITY_Gruppe 47]]
- 3 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 2 edges to [[_COMMUNITY_Gruppe 55]]
- 2 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 2 edges to [[_COMMUNITY_Gruppe 44]]
- 2 edges to [[_COMMUNITY_Gruppe 41]]
- 2 edges to [[_COMMUNITY_Gruppe 58]]
- 2 edges to [[_COMMUNITY_Shared Konsolenausgabe]]
- 2 edges to [[_COMMUNITY_Gruppe 49]]
- 2 edges to [[_COMMUNITY_Gruppe 67]]
- 2 edges to [[_COMMUNITY_PV-Namespaces]]
- 2 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 2 edges to [[_COMMUNITY_Gruppe 61]]
- 2 edges to [[_COMMUNITY_MasterClock]]
- 1 edge to [[_COMMUNITY_Gruppe 53]]
- 1 edge to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 1 edge to [[_COMMUNITY_DeviceConnection]]
- 1 edge to [[_COMMUNITY_Gruppe 89]]
- 1 edge to [[_COMMUNITY_Gruppe 63]]
- 1 edge to [[_COMMUNITY_SyncedClock]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_TimeSyncServer]]

## Top bridge nodes
- [[DateTime]] - degree 79, connects to 28 communities
- [[WeatherFeed]] - degree 19, connects to 6 communities
- [[SimulationEngine.AdvanceAsync()_2]] - degree 13, connects to 6 communities
- [[WeatherData]] - degree 15, connects to 5 communities
- [[CsvExporter.WriteRowAsync()]] - degree 9, connects to 5 communities