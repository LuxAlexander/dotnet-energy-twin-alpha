---
source_file: "tests/FieldDevices.Tests/FieldDeviceTests.cs"
type: "code"
community: "Batterie-Modbus & Update"
location: "L173"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Batterie-Modbus__Update
---

# BatteryModbusTests

## Connections
- [[Batterie]] - `references` [EXTRACTED]
- [[BatterieConfig]] - `references` [EXTRACTED]
- [[BatteryModbus]] - `references` [EXTRACTED]
- [[BatteryModbus.Start()]] - `calls` [INFERRED]
- [[BatteryModbusTests.Dispose()]] - `method` [EXTRACTED]
- [[BatteryModbusTests.FaultsAndSwitch_AreReportedAndResettable()]] - `method` [EXTRACTED]
- [[BatteryModbusTests.Header_ShowsChargingAsPositivePower_AndBatteryValues()]] - `method` [EXTRACTED]
- [[BatteryModbusTests.ModeRegister_SwitchesBackToAuto()]] - `method` [EXTRACTED]
- [[BatteryModbusTests.Setpoint_BeyondConfiguredPower_IsRejected()]] - `method` [EXTRACTED]
- [[BatteryModbusTests.Setpoint_WithinConfiguredPower_IsAccepted_AndSwitchesToManual()]] - `method` [EXTRACTED]
- [[DailyProfile]] - `calls` [INFERRED]
- [[FieldDeviceTests.cs]] - `contains` [EXTRACTED]
- [[ModbusTestBase]] - `inherits` [EXTRACTED]
- [[ModbusTestBase.Connect()]] - `calls` [EXTRACTED]
- [[Simulator]] - `references` [EXTRACTED]
- [[SimulatorOptions]] - `calls` [INFERRED]
- [[TimeSyncOptions]] - `calls` [INFERRED]

#graphify/code #graphify/EXTRACTED #community/Batterie-Modbus__Update