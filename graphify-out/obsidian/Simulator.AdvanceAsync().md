---
source_file: "src/Devices/BatterySimulator/Simulator.cs"
type: "code"
community: "CsvLogger"
location: "L140"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/CsvLogger
---

# Simulator.AdvanceAsync()

## Connections
- [[Batterie.SetPower()]] - `calls` [INFERRED]
- [[Batterie.Update()]] - `calls` [INFERRED]
- [[CancellationToken]] - `references` [EXTRACTED]
- [[CsvLogger]] - `references` [EXTRACTED]
- [[CsvLogger.FlushAsync()]] - `calls` [INFERRED]
- [[CsvLogger.LogAsync()]] - `calls` [INFERRED]
- [[DailyProfile.GetSetpoint()]] - `calls` [INFERRED]
- [[Simulator]] - `method` [EXTRACTED]
- [[Simulator.RunAsync()]] - `calls` [EXTRACTED]
- [[StepScheduler.Update()]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]
- [[WeatherFeed.UpdateAsync()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/CsvLogger