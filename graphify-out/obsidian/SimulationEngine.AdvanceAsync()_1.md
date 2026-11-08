---
source_file: "src/Devices/DualPortCharger/Simulation/SimulationEngine.cs"
type: "code"
community: "Batterie-Simulator Loop"
location: "L163"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Batterie-Simulator_Loop
---

# SimulationEngine.AdvanceAsync()

## Connections
- [[AutoModeController.Step()]] - `calls` [INFERRED]
- [[CancellationToken]] - `references` [EXTRACTED]
- [[CsvLogger_1]] - `references` [EXTRACTED]
- [[CsvLogger.FlushAsync()_1]] - `calls` [INFERRED]
- [[CsvLogger.WriteAsync()]] - `calls` [INFERRED]
- [[DualCharger.Tick()]] - `calls` [EXTRACTED]
- [[SimulationEngine_1]] - `method` [EXTRACTED]
- [[SimulationEngine.RunAsync()]] - `calls` [EXTRACTED]
- [[StepScheduler.Update()]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]
- [[WeatherFeed.UpdateAsync()]] - `calls` [INFERRED]

#graphify/code #graphify/EXTRACTED #community/Batterie-Simulator_Loop