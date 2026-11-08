---
source_file: "src/Devices.Common/DeviceTimeSource.cs"
type: "code"
community: "DeviceTimeSource"
location: "L94"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/DeviceTimeSource
---

# DeviceTimeSource.TogglePause()

## Connections
- [[DeviceTimeSource]] - `method` [EXTRACTED]
- [[DeviceTimeSourceTests.Standalone_PauseAndSpeed_ChangeLocalClock()]] - `calls` [INFERRED]
- [[DeviceTimeSourceTests.WithMaster_LocalControlsAreRefused()]] - `calls` [INFERRED]
- [[InputHandler.HandleAsync()]] - `calls` [INFERRED]
- [[InputHandler.HandleAsync()_1]] - `calls` [INFERRED]
- [[MasterClock.Pause()]] - `calls` [INFERRED]
- [[MasterClock.Resume()]] - `calls` [INFERRED]
- [[ModbusDeviceServerTests.Registers_StayReadable_WhilePaused_AndKeepLastState()]] - `calls` [INFERRED]
- [[PvModbusTests.Registers_StayReadable_WhileTimeIsPaused()]] - `calls` [EXTRACTED]
- [[SimulationEngine.HandleKey()]] - `calls` [EXTRACTED]
- [[Simulator.HandleKey()]] - `calls` [EXTRACTED]

#graphify/code #graphify/INFERRED #community/DeviceTimeSource