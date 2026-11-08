---
source_file: "src/Devices.Common/Weather/WeatherFeed.cs"
type: "code"
community: "Batterie-Simulator Loop"
location: "L44"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Batterie-Simulator_Loop
---

# WeatherFeed.UpdateAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[DateTime]] - `references` [EXTRACTED]
- [[IWeatherService.GetAsync()]] - `calls` [INFERRED]
- [[SimulationEngine.AdvanceAsync()_1]] - `calls` [INFERRED]
- [[SimulationEngine.UpdateWeatherAsync()_1]] - `calls` [INFERRED]
- [[Simulator.AdvanceAsync()]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]
- [[WeatherData]] - `references` [EXTRACTED]
- [[WeatherFeed]] - `method` [EXTRACTED]
- [[WeatherFeed.Describe()]] - `calls` [EXTRACTED]
- [[WeatherFeedTests.Failure_KeepsLastWeatherAndUpdatesStatus()]] - `calls` [INFERRED]
- [[WeatherFeedTests.NextHourAndOtherRun_FetchAgain()]] - `calls` [INFERRED]
- [[WeatherFeedTests.SameHour_FetchesOnlyOnce()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Batterie-Simulator_Loop