---
source_file: "tests/FieldDevices.Tests/FieldDeviceTests.cs"
type: "code"
community: "Batterie-Modbus & Update"
location: "L46"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Batterie-Modbus__Update
---

# ModbusTestBase.ReadFloat()

## Connections
- [[BatteryModbusTests.Header_ShowsChargingAsPositivePower_AndBatteryValues()]] - `calls` [EXTRACTED]
- [[BatteryModbusTests.Setpoint_BeyondConfiguredPower_IsRejected()]] - `calls` [EXTRACTED]
- [[BatteryModbusTests.Setpoint_WithinConfiguredPower_IsAccepted_AndSwitchesToManual()]] - `calls` [EXTRACTED]
- [[KuehlhausModbusTests.Publish_RunningCompressor_SetsRunningStateAndPower()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.Publish_WritesHeaderAndSpecificRegisters()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.SetpointWrite_IsApplied()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.SetpointWrite_OutOfRange_IsRejectedAndOldValueKept()]] - `calls` [INFERRED]
- [[ModbusTestBase]] - `method` [EXTRACTED]
- [[ModbusTestBase.Read()]] - `calls` [EXTRACTED]
- [[PvModbusTests.Header_ShowsGenerationAsNegativePower_AndPvSpecificValues()]] - `calls` [EXTRACTED]
- [[PvModbusTests.OnOffAndReset_AreApplied()]] - `calls` [EXTRACTED]
- [[PvModbusTests.PowerLimit_OutOfRange_IsRejected_AndDeviceKeepsValue()]] - `calls` [EXTRACTED]
- [[PvModbusTests.PowerLimit_ValidValue_LimitsFeedInAtNextStep()]] - `calls` [EXTRACTED]
- [[RegisterCodec.DecodeFloat32()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Batterie-Modbus__Update