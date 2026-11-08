---
source_file: "src/Devices/KuehlhausSimulator/IO/InputHandler.cs"
type: "code"
community: "Batterie-Simulator Loop"
location: "L50"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Batterie-Simulator_Loop
---

# InputHandler.HandleAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[ConsoleKeyInfo]] - `references` [EXTRACTED]
- [[DeviceTimeSource.ChangeSpeed()]] - `calls` [INFERRED]
- [[DeviceTimeSource.TogglePause()]] - `calls` [INFERRED]
- [[InputHandler_1]] - `method` [EXTRACTED]
- [[InputHandler.AskSetpointAsync()]] - `calls` [EXTRACTED]
- [[InputHandler.HandlePendingKeysAsync()_1]] - `calls` [EXTRACTED]
- [[SimulationEngine.Quit()_1]] - `calls` [INFERRED]
- [[SimulationEngine.ToggleDoor()]] - `calls` [INFERRED]
- [[SimulationEngine.ToggleMode()]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Batterie-Simulator_Loop