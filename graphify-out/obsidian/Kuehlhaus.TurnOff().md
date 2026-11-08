---
source_file: "src/Devices/KuehlhausSimulator/Kuehlhaus.cs"
type: "code"
community: "Gruppe 57"
location: "L172"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Gruppe_57
---

# Kuehlhaus.TurnOff()

## Connections
- [[Kuehlhaus]] - `method` [EXTRACTED]
- [[Kuehlhaus.UpdateCompressor()]] - `calls` [EXTRACTED]
- [[Kuehlhaus.UpdateState()]] - `calls` [EXTRACTED]
- [[KuehlhausModbus.ProcessCommands()]] - `calls` [INFERRED]
- [[KuehlhausModelTests.LargeStep_StaysStableAndDoesNotOvershoot()]] - `calls` [INFERRED]
- [[KuehlhausModelTests.OverTemperature_LatchesAndNeedsCoolDownBeforeReset()]] - `calls` [INFERRED]
- [[KuehlhausModelTests.TurnOff_StopsCompressorAndReportsOffState()]] - `calls` [INFERRED]
- [[KuehlhausModelTests.Update_UsesExactExponentialSolution()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Gruppe_57