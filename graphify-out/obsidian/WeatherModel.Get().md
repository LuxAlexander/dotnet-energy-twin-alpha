---
source_file: "src/WeatherSimulator/Simulation/WeatherModel.cs"
type: "code"
community: "Wettermodell & Tests (3)"
location: "L25"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Wettermodell__Tests_3
---

# WeatherModel.Get()

## Connections
- [[DateTime]] - `references` [EXTRACTED]
- [[LocationCatalog.TryGet()]] - `calls` [EXTRACTED]
- [[WeatherData]] - `references` [EXTRACTED]
- [[WeatherModel]] - `method` [EXTRACTED]
- [[WeatherModel.CloudCover()]] - `calls` [EXTRACTED]
- [[WeatherModel.ConditionOf()]] - `calls` [EXTRACTED]
- [[WeatherModel.GetYear()]] - `calls` [EXTRACTED]
- [[WeatherModel.IndexOfHour()]] - `calls` [EXTRACTED]
- [[WeatherModel.PrecipitationMmPerHour()]] - `calls` [EXTRACTED]
- [[WeatherModel.SunHeightDegrees()]] - `calls` [EXTRACTED]
- [[WeatherModel.TemperatureC()]] - `calls` [EXTRACTED]
- [[WeatherModelTests.Clear_RemovesManualOverride()]] - `calls` [INFERRED]
- [[WeatherModelTests.DifferentLocations_GiveDifferentWeather()]] - `calls` [INFERRED]
- [[WeatherModelTests.DifferentRuns_GiveDifferentTemperature()]] - `calls` [INFERRED]
- [[WeatherModelTests.ManualOverride_AppliesToAllLocations()]] - `calls` [INFERRED]
- [[WeatherModelTests.MinuteWithinHour_DoesNotChangeWeather()]] - `calls` [INFERRED]
- [[WeatherModelTests.Night_HasNoIrradiationAndNightCondition()]] - `calls` [INFERRED]
- [[WeatherModelTests.RainPreset_ProducesPrecipitation()]] - `calls` [INFERRED]
- [[WeatherModelTests.SameLocationTimeAndRun_ReturnsIdenticalWeather()]] - `calls` [INFERRED]
- [[WeatherModelTests.SummerNoon_IsHigherThanWinterNoon()]] - `calls` [INFERRED]
- [[WeatherModelTests.UnknownLocation_Throws()]] - `calls` [INFERRED]
- [[WeatherModelTests.Values_AreInPlausibleRanges()]] - `calls` [INFERRED]

#graphify/code #graphify/EXTRACTED #community/Wettermodell__Tests_3