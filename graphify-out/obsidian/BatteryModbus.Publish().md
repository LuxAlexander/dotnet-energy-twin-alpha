---
source_file: "src/Devices/BatterySimulator/BatteryModbus.cs"
type: "code"
community: "Batterie-Modbus & Update (2)"
location: "L98"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Batterie-Modbus__Update_2
---

# BatteryModbus.Publish()

## Connections
- [[BatteryModbus]] - `method` [EXTRACTED]
- [[BatteryModbus.Start()]] - `calls` [EXTRACTED]
- [[BatteryModbusTests.FaultsAndSwitch_AreReportedAndResettable()]] - `calls` [INFERRED]
- [[BatteryModbusTests.Header_ShowsChargingAsPositivePower_AndBatteryValues()]] - `calls` [INFERRED]
- [[BatteryModbusTests.ModeRegister_SwitchesBackToAuto()]] - `calls` [INFERRED]
- [[BatteryModbusTests.Setpoint_BeyondConfiguredPower_IsRejected()]] - `calls` [INFERRED]
- [[BatteryModbusTests.Setpoint_WithinConfiguredPower_IsAccepted_AndSwitchesToManual()]] - `calls` [INFERRED]
- [[DateTime]] - `references` [EXTRACTED]
- [[HeaderValues]] - `calls` [INFERRED]
- [[ModbusDeviceServer.Publish()]] - `calls` [INFERRED]
- [[Simulator.RunAsync()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Batterie-Modbus__Update_2