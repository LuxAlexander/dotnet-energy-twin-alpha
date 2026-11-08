---
type: community
cohesion: 0.39
members: 12
---

# Wettermodell & Tests (3)

**Cohesion:** 0.39 - loosely connected
**Members:** 12 nodes

## Members
- [[KeyNotFoundException]] - code
- [[WeatherModel.Get()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModelTests]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.DifferentLocations_GiveDifferentWeather()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.DifferentRuns_GiveDifferentTemperature()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.MinuteWithinHour_DoesNotChangeWeather()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.NewModel()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.Night_HasNoIrradiationAndNightCondition()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.SameLocationTimeAndRun_ReturnsIdenticalWeather()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.SummerNoon_IsHigherThanWinterNoon()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.UnknownLocation_Throws()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.Values_AreInPlausibleRanges()]] - code - tests/Weather.Tests/WeatherModelTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Wettermodell__Tests_3
SORT file.name ASC
```

## Connections to other communities
- 9 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 8 edges to [[_COMMUNITY_Gruppe 23]]
- 7 edges to [[_COMMUNITY_Wettermodell & Tests]]
- 2 edges to [[_COMMUNITY_Wettermodell & Tests (5)]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (4)]]

## Top bridge nodes
- [[WeatherModel.Get()]] - degree 22, connects to 5 communities
- [[WeatherModelTests]] - degree 13, connects to 2 communities
- [[WeatherModelTests.NewModel()]] - degree 11, connects to 2 communities
- [[WeatherModelTests.UnknownLocation_Throws()]] - degree 5, connects to 1 community
- [[WeatherModelTests.DifferentLocations_GiveDifferentWeather()]] - degree 4, connects to 1 community