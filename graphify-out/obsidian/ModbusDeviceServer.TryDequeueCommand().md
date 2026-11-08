---
source_file: "src/Devices.Common/Modbus/ModbusDeviceServer.cs"
type: "code"
community: "RegisterCodec & Befehle"
location: "L183"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/RegisterCodec__Befehle
---

# ModbusDeviceServer.TryDequeueCommand()

## Connections
- [[BatteryModbus.ProcessCommands()]] - `calls` [INFERRED]
- [[ChargerModbus.ProcessCommands()]] - `calls` [INFERRED]
- [[DiagnosticClientTests.OnOffReset_ByLabelAndShortcut_ReachTheDevice()]] - `calls` [INFERRED]
- [[DiagnosticClientTests.Write_InvalidValue_IsRefusedByClientBeforeSending()]] - `calls` [INFERRED]
- [[DiagnosticClientTests.Write_ValidSetpoint_IsAcceptedAndVisibleInDevice()]] - `calls` [INFERRED]
- [[KuehlhausModbus.ProcessCommands()]] - `calls` [INFERRED]
- [[ModbusCommand]] - `references` [EXTRACTED]
- [[ModbusDeviceServer]] - `method` [EXTRACTED]
- [[ModbusDeviceServerTests.InvalidValue_IsRejected_AndOldValueStays()]] - `calls` [INFERRED]
- [[ModbusDeviceServerTests.PartialWriteOfFloat_IsRejected()]] - `calls` [INFERRED]
- [[ModbusDeviceServerTests.PowerSwitchAndReset_AreWritableCommands()]] - `calls` [INFERRED]
- [[ModbusDeviceServerTests.WaitForCommand()]] - `calls` [INFERRED]
- [[ModbusDeviceServerTests.WriteSpanningReadOnlyAndWritable_IsRejectedCompletely()]] - `calls` [INFERRED]
- [[ModbusDeviceServerTests.WriteToReadOnlyRegister_IsRejected()]] - `calls` [INFERRED]
- [[PvModbus.ProcessCommands()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/RegisterCodec__Befehle