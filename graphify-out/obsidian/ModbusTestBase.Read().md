---
source_file: "tests/FieldDevices.Tests/FieldDeviceTests.cs"
type: "code"
community: "Batterie-Modbus & Update"
location: "L44"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Batterie-Modbus__Update
---

# ModbusTestBase.Read()

## Connections
- [[BatteryModbusTests.FaultsAndSwitch_AreReportedAndResettable()]] - `calls` [EXTRACTED]
- [[BatteryModbusTests.Header_ShowsChargingAsPositivePower_AndBatteryValues()]] - `calls` [EXTRACTED]
- [[BatteryModbusTests.ModeRegister_SwitchesBackToAuto()]] - `calls` [EXTRACTED]
- [[BatteryModbusTests.Setpoint_BeyondConfiguredPower_IsRejected()]] - `calls` [EXTRACTED]
- [[BatteryModbusTests.Setpoint_WithinConfiguredPower_IsAccepted_AndSwitchesToManual()]] - `calls` [EXTRACTED]
- [[KuehlhausModbusTests.DoorWrite_OpensAndClosesTheDoor()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.PowerSwitch_TurnsPlantOffAndOn()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.Publish_RunningCompressor_SetsRunningStateAndPower()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.Publish_WritesHeaderAndSpecificRegisters()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.ResetCommand_ClearsCompressorFailure()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.SetpointWrite_IsApplied()]] - `calls` [INFERRED]
- [[KuehlhausModbusTests.SetpointWrite_OutOfRange_IsRejectedAndOldValueKept()]] - `calls` [INFERRED]
- [[ModbusTestBase]] - `method` [EXTRACTED]
- [[ModbusTestBase.ReadFloat()]] - `calls` [EXTRACTED]
- [[PvModbusTests.Header_ShowsGenerationAsNegativePower_AndPvSpecificValues()]] - `calls` [EXTRACTED]
- [[PvModbusTests.OnOffAndReset_AreApplied()]] - `calls` [EXTRACTED]
- [[PvModbusTests.PowerLimit_OutOfRange_IsRejected_AndDeviceKeepsValue()]] - `calls` [EXTRACTED]
- [[PvModbusTests.PowerLimit_ValidValue_LimitsFeedInAtNextStep()]] - `calls` [EXTRACTED]
- [[PvModbusTests.Registers_StayReadable_WhileTimeIsPaused()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Batterie-Modbus__Update