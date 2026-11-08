---
source_file: "tests/FieldDevices.Tests/FieldDeviceTests.cs"
type: "code"
community: "Batterie-Modbus & Update"
location: "L161"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Batterie-Modbus__Update
---

# PvModbusTests.Registers_StayReadable_WhileTimeIsPaused()

## Connections
- [[DeviceTimeSource.TogglePause()]] - `calls` [EXTRACTED]
- [[Fact]] - `references` [EXTRACTED]
- [[ModbusTestBase.Read()]] - `calls` [EXTRACTED]
- [[PvModbus.Publish()]] - `calls` [INFERRED]
- [[PvModbusTests]] - `method` [EXTRACTED]
- [[RegisterCodec.DecodeUInt32()]] - `calls` [EXTRACTED]
- [[RegisterCodec.FromSeconds()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Batterie-Modbus__Update