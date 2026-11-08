---
source_file: "src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs"
type: "code"
community: "Batterie-Simulator Loop (2)"
location: "L249"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Batterie-Simulator_Loop_2
---

# SimulationEngine.StartRunAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[CsvLogger_2]] - `references` [EXTRACTED]
- [[CsvLogger.FlushAsync()_2]] - `calls` [INFERRED]
- [[CsvLogger.WriteAsync()_1]] - `calls` [INFERRED]
- [[DateTime]] - `references` [EXTRACTED]
- [[Kuehlhaus.Update()_1]] - `calls` [INFERRED]
- [[SimulationEngine_2]] - `method` [EXTRACTED]
- [[SimulationEngine.AdvanceAsync()_2]] - `calls` [EXTRACTED]
- [[SimulationEngine.UpdateWeatherAsync()_1]] - `calls` [EXTRACTED]
- [[Task]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Batterie-Simulator_Loop_2