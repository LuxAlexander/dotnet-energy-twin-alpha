---
source_file: "src/Devices/KuehlhausSimulator/Kuehlhaus.cs"
type: "code"
community: "Gruppe 57"
location: "L110"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Gruppe_57
---

# Kuehlhaus.Update()

## Connections
- [[Kuehlhaus]] - `method` [EXTRACTED]
- [[Kuehlhaus.Update()]] - `calls` [EXTRACTED]
- [[KuehlhausAutoModeTests.CountOpenSteps()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.Publish_RunningCompressor_SetsRunningStateAndPower()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.Publish_WritesHeaderAndSpecificRegisters()]] - `calls` [INFERRED]
- [[KuehlhausModelTests.CompressorFailure_IsAlwaysClearedByReset()]] - `calls` [INFERRED]
- [[KuehlhausModelTests.Compressor_SwitchesWithHysteresisAroundSetpoint()]] - `calls` [INFERRED]
- [[KuehlhausModelTests.ElectricPowerAndEnergy_FollowCoolingPowerAndCop()]] - `calls` [INFERRED]
- [[KuehlhausModelTests.LargeStep_StaysStableAndDoesNotOvershoot()]] - `calls` [INFERRED]
- [[KuehlhausModelTests.OverTemperature_LatchesAndNeedsCoolDownBeforeReset()]] - `calls` [INFERRED]
- [[KuehlhausModelTests.TurnOff_StopsCompressorAndReportsOffState()]] - `calls` [INFERRED]
- [[KuehlhausModelTests.Update_UsesExactExponentialSolution()]] - `calls` [INFERRED]
- [[SimulationEngine.AdvanceAsync()_1]] - `calls` [INFERRED]
- [[SimulationEngine.StartRunAsync()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Gruppe_57