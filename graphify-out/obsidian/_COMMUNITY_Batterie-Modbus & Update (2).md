---
type: community
cohesion: 0.19
members: 28
---

# Batterie-Modbus & Update (2)

**Cohesion:** 0.19 - loosely connected
**Members:** 28 nodes

## Members
- [[Batterie.Update()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[BatteryModbus.Publish()]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[BatteryModbus.Start()]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[BatteryModbusTests]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[BatteryModbusTests.FaultsAndSwitch_AreReportedAndResettable()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[BatteryModbusTests.Header_ShowsChargingAsPositivePower_AndBatteryValues()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[BatteryModbusTests.ModeRegister_SwitchesBackToAuto()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[BatteryModbusTests.Setpoint_BeyondConfiguredPower_IsRejected()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[BatteryModbusTests.Setpoint_WithinConfiguredPower_IsAccepted_AndSwitchesToManual()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[KuehlhausModbus.ProcessCommands()]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[KuehlhausModbusTests]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.DoorWrite_OpensAndClosesTheDoor()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.PowerSwitch_TurnsPlantOffAndOn()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.Publish_RunningCompressor_SetsRunningStateAndPower()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.Publish_WritesHeaderAndSpecificRegisters()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.ResetCommand_ClearsCompressorFailure()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.SetpointWrite_IsApplied()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.SetpointWrite_OutOfRange_IsRejectedAndOldValueKept()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[ModbusClient]] - code
- [[ModbusTestBase]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.Connect()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.FreePort()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.Read()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.ReadFloat()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.WaitUntil()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.WriteFloat()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[Options_1]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbusTests.Header_ShowsGenerationAsNegativePower_AndPvSpecificValues()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Batterie-Modbus__Update_2
SORT file.name ASC
```

## Connections to other communities
- 15 edges to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 12 edges to [[_COMMUNITY_Gruppe 23]]
- 10 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 8 edges to [[_COMMUNITY_Batterie-Modell]]
- 7 edges to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 4 edges to [[_COMMUNITY_Gruppe 24]]
- 4 edges to [[_COMMUNITY_Gruppe 72]]
- 3 edges to [[_COMMUNITY_Kühlhaus-Modell]]
- 3 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 2 edges to [[_COMMUNITY_Modbus-Tests]]
- 2 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 2 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 2 edges to [[_COMMUNITY_Gruppe 48]]
- 2 edges to [[_COMMUNITY_RegisterCodec & Befehle (2)]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 1 edge to [[_COMMUNITY_Gruppe 21]]
- 1 edge to [[_COMMUNITY_Gruppe 44]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource]]
- 1 edge to [[_COMMUNITY_Gruppe 89]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]
- 1 edge to [[_COMMUNITY_Gruppe 109]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 1 edge to [[_COMMUNITY_DeviceConnection (Modbus.Client)]]

## Top bridge nodes
- [[BatteryModbusTests]] - degree 17, connects to 6 communities
- [[ModbusTestBase]] - degree 17, connects to 6 communities
- [[KuehlhausModbus.ProcessCommands()]] - degree 12, connects to 6 communities
- [[KuehlhausModbusTests]] - degree 15, connects to 5 communities
- [[BatteryModbus.Publish()]] - degree 11, connects to 4 communities