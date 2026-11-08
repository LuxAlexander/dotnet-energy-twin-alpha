---
source_file: "tests/Devices.Common.Tests/ModbusDeviceServerTests.cs"
type: "code"
community: "RegisterCodec & Befehle"
location: "L203"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/RegisterCodec__Befehle
---

# ModbusDeviceServerTests.Registers_StayReadable_WhilePaused_AndKeepLastState()

## Connections
- [[DeviceTimeSource.TogglePause()]] - `calls` [INFERRED]
- [[Fact]] - `references` [EXTRACTED]
- [[ModbusDeviceServer.Publish()]] - `calls` [INFERRED]
- [[ModbusDeviceServerTests]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.Header()]] - `calls` [EXTRACTED]
- [[ModbusDeviceServerTests.Read()]] - `calls` [EXTRACTED]
- [[RegisterCodec.DecodeFloat32()]] - `calls` [EXTRACTED]
- [[RegisterCodec.DecodeUInt32()]] - `calls` [EXTRACTED]
- [[RegisterCodec.FromSeconds()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/RegisterCodec__Befehle