---
source_file: "tests/FieldDevices.Tests/FieldDeviceTests.cs"
type: "code"
community: "Batterie-Modbus & Update (2)"
location: "L52"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Batterie-Modbus__Update_2
---

# ModbusTestBase.WriteFloat()

## Connections
- [[BatteryModbusTests.Setpoint_BeyondConfiguredPower_IsRejected()]] - `calls` [EXTRACTED]
- [[BatteryModbusTests.Setpoint_WithinConfiguredPower_IsAccepted_AndSwitchesToManual()]] - `calls` [EXTRACTED]
- [[KuehlhausModbusTests.SetpointWrite_IsApplied()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.SetpointWrite_OutOfRange_IsRejectedAndOldValueKept()]] - `calls` [INFERRED]
- [[ModbusTestBase]] - `method` [EXTRACTED]
- [[PvModbusTests.PowerLimit_OutOfRange_IsRejected_AndDeviceKeepsValue()]] - `calls` [EXTRACTED]
- [[PvModbusTests.PowerLimit_ValidValue_LimitsFeedInAtNextStep()]] - `calls` [EXTRACTED]
- [[RegisterCodec.EncodeFloat32()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Batterie-Modbus__Update_2