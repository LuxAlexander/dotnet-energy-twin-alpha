---
source_file: "tests/Weather.Tests/WeatherModelTests.cs"
type: "code"
community: "Wettermodell & Tests (3)"
location: "L8"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Wettermodell__Tests_3
---

# WeatherModelTests.NewModel()

## Connections
- [[ManualWeather]] - `calls` [INFERRED]
- [[WeatherModel]] - `references` [EXTRACTED]
- [[WeatherModelTests]] - `method` [EXTRACTED]
- [[WeatherModelTests.DifferentLocations_GiveDifferentWeather()]] - `calls` [EXTRACTED]
- [[WeatherModelTests.DifferentRuns_GiveDifferentTemperature()]] - `calls` [EXTRACTED]
- [[WeatherModelTests.MinuteWithinHour_DoesNotChangeWeather()]] - `calls` [EXTRACTED]
- [[WeatherModelTests.Night_HasNoIrradiationAndNightCondition()]] - `calls` [EXTRACTED]
- [[WeatherModelTests.SameLocationTimeAndRun_ReturnsIdenticalWeather()]] - `calls` [EXTRACTED]
- [[WeatherModelTests.SummerNoon_IsHigherThanWinterNoon()]] - `calls` [EXTRACTED]
- [[WeatherModelTests.UnknownLocation_Throws()]] - `calls` [EXTRACTED]
- [[WeatherModelTests.Values_AreInPlausibleRanges()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Wettermodell__Tests_3