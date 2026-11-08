---
source_file: "tests/Devices.Common.Tests/ModbusDeviceServerTests.cs"
type: "code"
community: "RegisterCodec & Befehle"
location: "L9"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/RegisterCodec__Befehle
---

# ModbusDeviceServerTests

## Connections
- [[DeviceTimeSource]] - `references` [EXTRACTED]
- [[IDisposable]] - `implements` [EXTRACTED]
- [[List]] - `references` [EXTRACTED]
- [[ModbusClient]] - `references` [EXTRACTED]
- [[ModbusDeviceServer]] - `references` [EXTRACTED]
- [[ModbusDeviceServer.Publish()]] - `calls` [INFERRED]
- [[ModbusDeviceServer.Start()]] - `calls` [INFERRED]
- [[ModbusDeviceServerTests.Dispose()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.FreePort()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.Header()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.Header_IsReadableWithIdentityStateAndTime()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.InvalidValue_IsRejected_AndOldValueStays()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.PartialWriteOfFloat_IsRejected()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.PowerSwitchAndReset_AreWritableCommands()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.Publish_SequenceGrowsOnlyWithNewModelTime()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.Read()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.ReadFloat()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.Registers_StayReadable_WhilePaused_AndKeepLastState()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.Start_OnBusyPort_ThrowsIOException()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.ValidWrite_IsAccepted_AndQueuedForDevice()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.WaitForCommand()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.WriteFloat()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.WriteSpanningReadOnlyAndWritable_IsRejectedCompletely()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.WriteToReadOnlyRegister_IsRejected()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.WriteToUnusedAddress_IsRejected()]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.cs]] - `contains` [EXTRACTED]
- [[ModbusOptions]] - `calls` [INFERRED]
- [[RegisterPlan]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/RegisterCodec__Befehle