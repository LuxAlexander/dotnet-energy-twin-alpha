---
source_file: "src/Devices/PvSimulator/Simulation/SimulationEngine.cs"
type: "code"
community: "WeatherFeed"
location: "L364"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/WeatherFeed
---

# SimulationEngine.UpdateAutoWeather()

## Connections
- [[DateTime]] - `references` [EXTRACTED]
- [[IrradiationCalculator.IsDaylight()]] - `calls` [EXTRACTED]
- [[PvSystem.SetWeather()]] - `calls` [INFERRED]
- [[SeasonInfo.FromMonth()]] - `calls` [EXTRACTED]
- [[SimulationEngine_2]] - `method` [EXTRACTED]
- [[SimulationEngine.AdvanceAsync()_2]] - `calls` [EXTRACTED]
- [[SimulationEngine.RandomWeather()]] - `calls` [EXTRACTED]
- [[SimulationEngine.SetNotification()]] - `calls` [EXTRACTED]
- [[TimeSpan]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/WeatherFeed