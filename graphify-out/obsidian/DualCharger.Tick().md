---
source_file: "src/Devices/DualPortCharger/Models/DualCharger.cs"
type: "code"
community: "Gruppe 51"
location: "L72"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Gruppe_51
---

# DualCharger.Tick()

## Connections
- [[ChargingPoint.GetDemandKW()]] - `calls` [INFERRED]
- [[ChargingPoint.Update()]] - `calls` [INFERRED]
- [[DateTime]] - `references` [EXTRACTED]
- [[DualCharger]] - `method` [EXTRACTED]
- [[DualCharger.AllocatePower()]] - `calls` [EXTRACTED]
- [[DualCharger.ProcessQueue()]] - `calls` [EXTRACTED]
- [[SimulationEngine.AdvanceAsync()]] - `calls` [EXTRACTED]
- [[TimeSpan]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Gruppe_51