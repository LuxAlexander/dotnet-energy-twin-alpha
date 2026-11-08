---
source_file: "src/Devices/DualPortCharger/Models/DualCharger.cs"
type: "code"
community: "DualCharger"
location: "L72"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/DualCharger
---

# DualCharger.Tick()

## Connections
- [[ChargingPoint.GetDemandKW()]] - `calls` [INFERRED]
- [[ChargingPoint.Update()]] - `calls` [INFERRED]
- [[DateTime]] - `references` [EXTRACTED]
- [[DualCharger]] - `method` [EXTRACTED]
- [[DualCharger.AllocatePower()]] - `calls` [EXTRACTED]
- [[DualCharger.ProcessQueue()]] - `calls` [EXTRACTED]
- [[SimulationEngine.AdvanceAsync()_1]] - `calls` [EXTRACTED]
- [[TimeSpan]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/DualCharger