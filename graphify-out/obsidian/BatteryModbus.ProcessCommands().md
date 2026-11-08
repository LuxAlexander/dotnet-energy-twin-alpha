---
source_file: "src/Devices/BatterySimulator/BatteryModbus.cs"
type: "code"
community: "Batterie-Modell"
location: "L54"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Batterie-Modell
---

# BatteryModbus.ProcessCommands()

## Connections
- [[Batterie.ResetFault()]] - `calls` [INFERRED]
- [[Batterie.SetPower()]] - `calls` [INFERRED]
- [[Batterie.TurnOff()]] - `calls` [INFERRED]
- [[Batterie.TurnOn()]] - `calls` [INFERRED]
- [[BatteryModbus]] - `method` [EXTRACTED]
- [[BatteryModbusTests.FaultsAndSwitch_AreReportedAndResettable()]] - `calls` [INFERRED]
- [[BatteryModbusTests.ModeRegister_SwitchesBackToAuto()]] - `calls` [INFERRED]
- [[BatteryModbusTests.Setpoint_BeyondConfiguredPower_IsRejected()]] - `calls` [INFERRED]
- [[BatteryModbusTests.Setpoint_WithinConfiguredPower_IsAccepted_AndSwitchesToManual()]] - `calls` [INFERRED]
- [[ModbusDeviceServer.TryDequeueCommand()]] - `calls` [INFERRED]
- [[Simulator.RunAsync()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Batterie-Modell