---
source_file: "src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs"
type: "code"
community: "Batterie-Simulator Loop"
location: "L141"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Batterie-Simulator_Loop
---

# SimulationEngine.RunAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[ConsoleRenderer.DrawAsync()_1]] - `calls` [INFERRED]
- [[ConsoleRenderer.InitializeAsync()_1]] - `calls` [INFERRED]
- [[ConsoleRenderer.ShutdownAsync()_1]] - `calls` [INFERRED]
- [[CsvLogger.CreateAsync()_2]] - `calls` [EXTRACTED]
- [[DeviceTimeSource.RunAsync()]] - `calls` [EXTRACTED]
- [[InputHandler.HandlePendingKeysAsync()_1]] - `calls` [INFERRED]
- [[InputHandler.WaitForQuitAsync()_1]] - `calls` [INFERRED]
- [[KuehlhausEngineTests.Kuehlhaus_RunsStandalone_WritesCsvAndStopsOnCancel()]] - `calls` [INFERRED]
- [[KuehlhausModbus.Dispose()]] - `calls` [INFERRED]
- [[KuehlhausModbus.ProcessCommands()]] - `calls` [INFERRED]
- [[KuehlhausModbus.Publish()]] - `calls` [INFERRED]
- [[KuehlhausModbus.Start()]] - `calls` [INFERRED]
- [[Program.Main()_1]] - `calls` [INFERRED]
- [[SimulationEngine_1]] - `method` [EXTRACTED]
- [[SimulationEngine.AdvanceAsync()_1]] - `calls` [EXTRACTED]
- [[Task]] - `references` [EXTRACTED]
- [[WeatherFeed.Dispose()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Batterie-Simulator_Loop