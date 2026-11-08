---
type: community
cohesion: 0.22
members: 13
---

# Wettermodell & Tests

**Cohesion:** 0.22 - loosely connected
**Members:** 13 nodes

## Members
- [[CloudCoverPercent]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[IsActive]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[ManualWeather]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[ManualWeather.Clear()]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[ManualWeather.SetRain()]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[ManualWeather.SetTemperature()]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[ManualWeather.SetWind()]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[PrecipitationMmPerHour]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[TemperatureC]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[WeatherModelTests.Clear_RemovesManualOverride()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.ManualOverride_AppliesToAllLocations()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.RainPreset_ProducesPrecipitation()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WindSpeedMs]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Wettermodell__Tests
SORT file.name ASC
```

## Connections to other communities
- 7 edges to [[_COMMUNITY_Wettermodell & Tests (3)]]
- 4 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 3 edges to [[_COMMUNITY_Gruppe 23]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (5)]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (4)]]

## Top bridge nodes
- [[ManualWeather]] - degree 16, connects to 4 communities
- [[WeatherModelTests.Clear_RemovesManualOverride()]] - degree 7, connects to 3 communities
- [[WeatherModelTests.ManualOverride_AppliesToAllLocations()]] - degree 7, connects to 3 communities
- [[WeatherModelTests.RainPreset_ProducesPrecipitation()]] - degree 6, connects to 3 communities