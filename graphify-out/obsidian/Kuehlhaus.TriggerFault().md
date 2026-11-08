---
source_file: "src/Devices/KuehlhausSimulator/Kuehlhaus.cs"
type: "code"
community: "Kühlhaus-Modell"
location: "L164"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Kühlhaus-Modell
---

# Kuehlhaus.TriggerFault()

## Connections
- [[FaultType_1]] - `references` [EXTRACTED]
- [[Kuehlhaus]] - `method` [EXTRACTED]
- [[Kuehlhaus.UpdateState()]] - `calls` [EXTRACTED]
- [[KuehlhausModbusTests.ResetCommand_ClearsCompressorFailure()]] - `calls` [INFERRED]
- [[KuehlhausModelTests.CompressorFailure_IsAlwaysClearedByReset()]] - `calls` [INFERRED]

#graphify/code #graphify/EXTRACTED #community/Kühlhaus-Modell