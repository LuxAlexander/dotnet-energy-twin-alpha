---
type: community
cohesion: 0.13
members: 27
---

# WeatherFeed

**Cohesion:** 0.13 - loosely connected
**Members:** 27 nodes

## Members
- [[Calls]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[Fail]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[FakeWeatherService]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[IWeatherService]] - code - src/SimCore.Contracts/IWeatherService.cs
- [[IWeatherService.cs]] - code - src/SimCore.Contracts/IWeatherService.cs
- [[Last]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[Location]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[Location_2]] - code - src/Devices.Common/Weather/WeatherOptions.cs
- [[Specified]] - code - src/Devices.Common/Weather/WeatherOptions.cs
- [[Status]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[Url]] - code - src/Devices.Common/Weather/WeatherOptions.cs
- [[WeatherFeed]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[WeatherFeed.cs]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[WeatherFeedTests]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherFeedTests.Failure_KeepsLastWeatherAndUpdatesStatus()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherFeedTests.Hour()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherFeedTests.NextHourAndOtherRun_FetchAgain()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherFeedTests.Options_LocationIsLowercased()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherFeedTests.Options_MasterDefaultsToWeatherSimulator()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherFeedTests.Options_OffStaysOffWithMaster()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherFeedTests.Options_StandaloneHasNoWeather()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherFeedTests.SameHour_FetchesOnlyOnce()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[WeatherOptions]] - code - src/Devices.Common/Weather/WeatherOptions.cs
- [[WeatherOptions.IsOff()]] - code - src/Devices.Common/Weather/WeatherOptions.cs
- [[WeatherOptions.TryApply()]] - code - src/Devices.Common/Weather/WeatherOptions.cs
- [[WeatherOptions.WithDefaultUrl()]] - code - src/Devices.Common/Weather/WeatherOptions.cs
- [[WeatherOptions.cs]] - code - src/Devices.Common/Weather/WeatherOptions.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/WeatherFeed
SORT file.name ASC
```

## Connections to other communities
- 8 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 7 edges to [[_COMMUNITY_Gruppe 23]]
- 3 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 3 edges to [[_COMMUNITY_TimeSyncServer]]
- 2 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 2 edges to [[_COMMUNITY_Gruppe 37]]
- 2 edges to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]
- 1 edge to [[_COMMUNITY_Gruppe 21]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (4)]]
- 1 edge to [[_COMMUNITY_Gruppe 66]]
- 1 edge to [[_COMMUNITY_Gruppe 24]]
- 1 edge to [[_COMMUNITY_Gruppe 61]]
- 1 edge to [[_COMMUNITY_Gruppe 105]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]

## Top bridge nodes
- [[WeatherFeed]] - degree 19, connects to 9 communities
- [[WeatherOptions.TryApply()]] - degree 8, connects to 4 communities
- [[WeatherOptions]] - degree 14, connects to 3 communities
- [[WeatherFeedTests.Failure_KeepsLastWeatherAndUpdatesStatus()]] - degree 7, connects to 3 communities
- [[WeatherFeedTests.NextHourAndOtherRun_FetchAgain()]] - degree 7, connects to 3 communities