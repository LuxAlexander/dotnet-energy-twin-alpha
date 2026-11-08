---
type: community
cohesion: 0.07
members: 57
---

# Wettermodell & Tests

**Cohesion:** 0.07 - loosely connected
**Members:** 57 nodes

## Members
- [[All]] - code - src/WeatherSimulator/Simulation/LocationCatalog.cs
- [[CloudCoverPercent_1]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[ConcurrentDictionary]] - code
- [[CoupledWeatherYear]] - code
- [[IsActive]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[KeyNotFoundException]] - code
- [[LocationCatalog]] - code - src/WeatherSimulator/Simulation/LocationCatalog.cs
- [[LocationCatalog.TryGet()]] - code - src/WeatherSimulator/Simulation/LocationCatalog.cs
- [[LocationCatalog.cs]] - code - src/WeatherSimulator/Simulation/LocationCatalog.cs
- [[Manual_2]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[ManualWeather]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[ManualWeather.ApplyPreset()]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[ManualWeather.Clear()]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[ManualWeather.SetRain()]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[ManualWeather.SetTemperature()]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[ManualWeather.SetWind()]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[ManualWeather.cs]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[Night_1]] - code - src/SimCore.Contracts/WeatherData.cs
- [[Overcast_1]] - code - src/SimCore.Contracts/WeatherData.cs
- [[PartlyCloudy_1]] - code - src/SimCore.Contracts/WeatherData.cs
- [[PrecipitationMmPerHour_1]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[SimulationEngine.German()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[Snow_1]] - code - src/SimCore.Contracts/WeatherData.cs
- [[Storm_1]] - code - src/SimCore.Contracts/WeatherData.cs
- [[Sunny_1]] - code - src/SimCore.Contracts/WeatherData.cs
- [[TemperatureC_2]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[Weather.Tests]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherCondition_1]] - code - src/SimCore.Contracts/WeatherData.cs
- [[WeatherData.cs]] - code - src/SimCore.Contracts/WeatherData.cs
- [[WeatherLocation]] - code - src/WeatherSimulator/Simulation/LocationCatalog.cs
- [[WeatherModel]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.CloudCover()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.ConditionOf()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.Get()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.GetYear()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.IndexOfHour()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.PrecipitationMmPerHour()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.Rand01()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.StableHash()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.SunHeightDegrees()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.TemperatureC()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModelTests]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.Clear_RemovesManualOverride()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.DifferentLocations_GiveDifferentWeather()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.DifferentRuns_GiveDifferentTemperature()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.ManualOverride_AppliesToAllLocations()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.MinuteWithinHour_DoesNotChangeWeather()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.NewModel()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.Night_HasNoIrradiationAndNightCondition()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.RainPreset_ProducesPrecipitation()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.SameLocationTimeAndRun_ReturnsIdenticalWeather()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.SummerNoon_IsHigherThanWinterNoon()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.UnknownLocation_Throws()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.Values_AreInPlausibleRanges()]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherModelTests.cs]] - code - tests/Weather.Tests/WeatherModelTests.cs
- [[WeatherSimulator.Simulation]] - code - src/WeatherSimulator/Simulation/LocationCatalog.cs
- [[WindSpeedMs_1]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Wettermodell__Tests
SORT file.name ASC
```

## Connections to other communities
- 11 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 10 edges to [[_COMMUNITY_WeatherFeed]]
- 4 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_Gruppe 53]]
- 1 edge to [[_COMMUNITY_Register-Dekodierung]]

## Top bridge nodes
- [[WeatherCondition_1]] - degree 13, connects to 2 communities
- [[WeatherSimulator.Simulation]] - degree 5, connects to 2 communities
- [[SimulationEngine.German()]] - degree 3, connects to 2 communities
- [[WeatherData.cs]] - degree 3, connects to 2 communities
- [[WeatherModel.Get()]] - degree 22, connects to 1 community