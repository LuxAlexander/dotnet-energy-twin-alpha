---
source_file: "src/Devices/BatterySimulator/Simulator.cs"
type: "code"
community: "Batterie-Simulator Loop"
location: "L72"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Batterie-Simulator_Loop
---

# Simulator.RunAsync()

## Connections
- [[BatteryModbus.Dispose()]] - `calls` [INFERRED]
- [[BatteryModbus.ProcessCommands()]] - `calls` [INFERRED]
- [[BatteryModbus.Publish()]] - `calls` [INFERRED]
- [[BatteryModbus.Start()]] - `calls` [INFERRED]
- [[CancellationToken]] - `references` [EXTRACTED]
- [[ConsoleDashboard.Initialize()]] - `calls` [INFERRED]
- [[ConsoleDashboard.Render()]] - `calls` [INFERRED]
- [[ConsoleDashboard.Shutdown()]] - `calls` [INFERRED]
- [[ConsoleDashboard.StatusLine()]] - `calls` [EXTRACTED]
- [[CsvLogger.CreateAsync()_1]] - `calls` [EXTRACTED]
- [[DeviceTimeSource.RunAsync()]] - `calls` [EXTRACTED]
- [[Simulator]] - `method` [EXTRACTED]
- [[Simulator.AdvanceAsync()]] - `calls` [EXTRACTED]
- [[Simulator.HandleKeys()]] - `calls` [EXTRACTED]
- [[Simulator.RunPending()]] - `calls` [EXTRACTED]
- [[Task]] - `references` [EXTRACTED]
- [[WeatherFeed.Dispose()]] - `calls` [INFERRED]

#graphify/code #graphify/EXTRACTED #community/Batterie-Simulator_Loop