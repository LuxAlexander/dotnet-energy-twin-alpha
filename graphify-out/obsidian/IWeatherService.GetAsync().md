---
source_file: "src/SimCore.Contracts/IWeatherService.cs"
type: "code"
community: "WeatherFeed"
location: "L10"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/WeatherFeed
---

# IWeatherService.GetAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[DateTime]] - `references` [EXTRACTED]
- [[FakeWeatherService.GetAsync()]] - `dispatches_to` [INFERRED]
- [[IWeatherService]] - `method` [EXTRACTED]
- [[Task]] - `references` [EXTRACTED]
- [[WeatherClient.GetAsync()]] - `dispatches_to` [INFERRED]
- [[WeatherData]] - `references` [EXTRACTED]
- [[WeatherFeed.UpdateAsync()]] - `calls` [INFERRED]

#graphify/code #graphify/EXTRACTED #community/WeatherFeed