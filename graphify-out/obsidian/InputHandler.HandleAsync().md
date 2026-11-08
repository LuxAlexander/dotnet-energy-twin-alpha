---
source_file: "src/Devices/DualPortCharger/IO/InputHandler.cs"
type: "code"
community: "Batterie-Simulator Loop (3)"
location: "L50"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Batterie-Simulator_Loop_3
---

# InputHandler.HandleAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[ConsoleKeyInfo]] - `references` [EXTRACTED]
- [[DeviceTimeSource.ChangeSpeed()]] - `calls` [INFERRED]
- [[DeviceTimeSource.TogglePause()]] - `calls` [INFERRED]
- [[InputHandler]] - `method` [EXTRACTED]
- [[InputHandler.ChangePowerLimit()]] - `calls` [EXTRACTED]
- [[InputHandler.ConnectVehicleAsync()]] - `calls` [EXTRACTED]
- [[InputHandler.HandlePendingKeysAsync()]] - `calls` [EXTRACTED]
- [[InputHandler.ToggleCharging()]] - `calls` [EXTRACTED]
- [[SimulationEngine.Quit()]] - `calls` [INFERRED]
- [[SimulationEngine.VehicleArrives()]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Batterie-Simulator_Loop_3