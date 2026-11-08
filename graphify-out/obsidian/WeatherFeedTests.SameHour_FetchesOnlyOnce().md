---
source_file: "tests/Devices.Common.Tests/WeatherFeedTests.cs"
type: "code"
community: "WeatherFeed"
location: "L25"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/WeatherFeed
---

# WeatherFeedTests.SameHour_FetchesOnlyOnce()

## Connections
- [[Fact]] - `references` [EXTRACTED]
- [[FakeWeatherService]] - `calls` [EXTRACTED]
- [[Task]] - `references` [EXTRACTED]
- [[WeatherFeed]] - `calls` [INFERRED]
- [[WeatherFeed.UpdateAsync()]] - `calls` [INFERRED]
- [[WeatherFeedTests]] - `method` [EXTRACTED]
- [[WeatherFeedTests.Hour()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/WeatherFeed