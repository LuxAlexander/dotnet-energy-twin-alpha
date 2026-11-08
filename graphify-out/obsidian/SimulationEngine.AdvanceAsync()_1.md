---
source_file: "src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs"
type: "code"
community: "Batterie-Simulator Loop"
location: "L206"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Batterie-Simulator_Loop
---

# SimulationEngine.AdvanceAsync()

## Connections
- [[AutoModeController.Step()_1]] - `calls` [INFERRED]
- [[CancellationToken]] - `references` [EXTRACTED]
- [[CsvLogger_2]] - `references` [EXTRACTED]
- [[CsvLogger.FlushAsync()_2]] - `calls` [INFERRED]
- [[CsvLogger.WriteAsync()_1]] - `calls` [INFERRED]
- [[Kuehlhaus.Update()]] - `calls` [INFERRED]
- [[SimulationEngine_1]] - `method` [EXTRACTED]
- [[SimulationEngine.RunAsync()_1]] - `calls` [EXTRACTED]
- [[SimulationEngine.StartRunAsync()]] - `calls` [EXTRACTED]
- [[SimulationEngine.UpdateWeatherAsync()]] - `calls` [EXTRACTED]
- [[StepScheduler.Update()]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Batterie-Simulator_Loop