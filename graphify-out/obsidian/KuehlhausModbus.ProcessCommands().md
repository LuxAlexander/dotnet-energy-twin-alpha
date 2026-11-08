---
source_file: "src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs"
type: "code"
community: "Batterie-Modbus & Update"
location: "L38"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Batterie-Modbus__Update
---

# KuehlhausModbus.ProcessCommands()

## Connections
- [[Kuehlhaus.ResetFault()]] - `calls` [INFERRED]
- [[Kuehlhaus.SetDoor()]] - `calls` [INFERRED]
- [[Kuehlhaus.TurnOff()]] - `calls` [INFERRED]
- [[Kuehlhaus.TurnOn()]] - `calls` [INFERRED]
- [[KuehlhausModbus]] - `method` [EXTRACTED]
- [[KuehlhausModbusTests.DoorWrite_OpensAndClosesTheDoor()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.PowerSwitch_TurnsPlantOffAndOn()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.ResetCommand_ClearsCompressorFailure()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.SetpointWrite_IsApplied()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.SetpointWrite_OutOfRange_IsRejectedAndOldValueKept()]] - `calls` [INFERRED]
- [[ModbusDeviceServer.TryDequeueCommand()]] - `calls` [INFERRED]
- [[SimulationEngine.RunAsync()_1]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Batterie-Modbus__Update