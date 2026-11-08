---
source_file: "src/Devices/DualPortCharger/Simulation/SimulationEngine.cs"
type: "code"
community: "Batterie-Simulator Loop"
location: "L100"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Batterie-Simulator_Loop
---

# SimulationEngine.RunAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[ChargerModbus.Dispose()]] - `calls` [INFERRED]
- [[ChargerModbus.ProcessCommands()]] - `calls` [INFERRED]
- [[ChargerModbus.Publish()]] - `calls` [INFERRED]
- [[ChargerModbus.Start()]] - `calls` [INFERRED]
- [[ConsoleRenderer.DrawAsync()]] - `calls` [INFERRED]
- [[ConsoleRenderer.InitializeAsync()]] - `calls` [INFERRED]
- [[ConsoleRenderer.ShutdownAsync()]] - `calls` [INFERRED]
- [[CsvLogger.CreateAsync()_1]] - `calls` [EXTRACTED]
- [[DeviceTimeSource.RunAsync()]] - `calls` [EXTRACTED]
- [[InputHandler.HandlePendingKeysAsync()]] - `calls` [INFERRED]
- [[InputHandler.WaitForQuitAsync()]] - `calls` [INFERRED]
- [[Program.Main()]] - `calls` [INFERRED]
- [[SimulationEngine]] - `method` [EXTRACTED]
- [[SimulationEngine.AdvanceAsync()]] - `calls` [EXTRACTED]
- [[Task]] - `references` [EXTRACTED]
- [[WeatherFeed.Dispose()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Batterie-Simulator_Loop