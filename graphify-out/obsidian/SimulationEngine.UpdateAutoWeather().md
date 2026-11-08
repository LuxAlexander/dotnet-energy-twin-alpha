---
source_file: "src/Devices/PvSimulator/Simulation/SimulationEngine.cs"
type: "code"
community: "Gruppe 38"
location: "L364"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Gruppe_38
---

# SimulationEngine.UpdateAutoWeather()

## Connections
- [[DateTime]] - `references` [EXTRACTED]
- [[IrradiationCalculator.IsDaylight()]] - `calls` [EXTRACTED]
- [[PvSystem.SetWeather()]] - `calls` [INFERRED]
- [[SeasonInfo.FromMonth()]] - `calls` [EXTRACTED]
- [[SimulationEngine]] - `method` [EXTRACTED]
- [[SimulationEngine.AdvanceAsync()]] - `calls` [EXTRACTED]
- [[SimulationEngine.RandomWeather()]] - `calls` [EXTRACTED]
- [[SimulationEngine.SetNotification()]] - `calls` [EXTRACTED]
- [[TimeSpan]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Gruppe_38