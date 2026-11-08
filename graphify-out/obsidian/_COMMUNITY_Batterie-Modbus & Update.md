---
type: community
cohesion: 0.06
members: 87
---

# Batterie-Modbus & Update

**Cohesion:** 0.06 - loosely connected
**Members:** 87 nodes

## Members
- [[Action]] - code
- [[Batterie.Update()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[BatteryModbus]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[BatteryModbus.Dispose()]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[BatteryModbus.ProcessCommands()]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[BatteryModbus.Publish()]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[BatteryModbus.Start()]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[BatteryModbusTests]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[BatteryModbusTests.Dispose()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[BatteryModbusTests.FaultsAndSwitch_AreReportedAndResettable()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[BatteryModbusTests.Header_ShowsChargingAsPositivePower_AndBatteryValues()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[BatteryModbusTests.ModeRegister_SwitchesBackToAuto()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[BatteryModbusTests.Setpoint_BeyondConfiguredPower_IsRejected()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[BatteryModbusTests.Setpoint_WithinConfiguredPower_IsAccepted_AndSwitchesToManual()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ChargerModbus]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[ChargerModbus.Publish()]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[ChargerModbus.Start()]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[ClientCount]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[DiagnosticClientTests.StartServer()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[HeaderValues]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[IDisposable]] - code
- [[IsListening]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[KuehlhausModbus]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[KuehlhausModbus.Dispose()]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[KuehlhausModbus.ProcessCommands()]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[KuehlhausModbus.Publish()]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[KuehlhausModbus.Start()]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[KuehlhausModbusTests]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.Dispose()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.DoorWrite_OpensAndClosesTheDoor()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.PowerSwitch_TurnsPlantOffAndOn()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.Publish_RunningCompressor_SetsRunningStateAndPower()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.Publish_WritesHeaderAndSpecificRegisters()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.ResetCommand_ClearsCompressorFailure()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.SetpointWrite_IsApplied()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModbusTests.SetpointWrite_OutOfRange_IsRejectedAndOldValueKept()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[LastWriteText]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[Lock]] - code
- [[MasterLost]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[ModbusDeviceServer]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.Commit()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.MapStatus()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.OnClientsChanged()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.OnHoldingRegistersChanged()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.ParseInstance()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.Publish()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.SetErrorRegisters()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.Start()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusServer]] - code
- [[ModbusTestBase]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.Connect()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.Dispose()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.FreePort()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.Read()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.ReadFloat()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.WaitUntil()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ModbusTestBase.WriteFloat()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[NoTime_1]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[Options_1]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[Options_2]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[Plan]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[PvModbus]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[PvModbus.Dispose()]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[PvModbus.ProcessCommands()]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[PvModbus.Publish()]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[PvModbus.Start()]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[PvModbusTests]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbusTests.Dispose()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbusTests.Header_ShowsGenerationAsNegativePower_AndPvSpecificValues()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbusTests.OnOffAndReset_AreApplied()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbusTests.PowerLimit_OutOfRange_IsRejected_AndDeviceKeepsValue()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbusTests.PowerLimit_ValidValue_LimitsFeedInAtNextStep()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbusTests.Registers_StayReadable_WhileTimeIsPaused()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvSystem.StepContinuous()]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[RegisterWriter]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[RegisterWriter.SetFloat()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[RegisterWriter.SetUInt16()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[RegisterWriter.SetUInt32()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[Standalone_1]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[StatusText_2]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[StatusText_1]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[StatusText_3]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[StatusText_4]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[StatusText_5]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[Synced]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[TimeStatus]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[ValueValidator]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Batterie-Modbus__Update
SORT file.name ASC
```

## Connections to other communities
- 18 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 15 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 14 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 11 edges to [[_COMMUNITY_WeatherFeed]]
- 9 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 8 edges to [[_COMMUNITY_DeviceTimeSource]]
- 5 edges to [[_COMMUNITY_Schreibprüfung]]
- 5 edges to [[_COMMUNITY_Gruppe 81]]
- 5 edges to [[_COMMUNITY_Gruppe 57]]
- 4 edges to [[_COMMUNITY_Kühlhaus-Modell]]
- 4 edges to [[_COMMUNITY_PvFaultModel]]
- 4 edges to [[_COMMUNITY_TimeSyncOptions Tests]]
- 3 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 3 edges to [[_COMMUNITY_Gruppe 48]]
- 3 edges to [[_COMMUNITY_Gruppe 64]]
- 3 edges to [[_COMMUNITY_Batterie-Modell]]
- 3 edges to [[_COMMUNITY_Gruppe 43]]
- 3 edges to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 3 edges to [[_COMMUNITY_Gruppe 44]]
- 3 edges to [[_COMMUNITY_PvSystem]]
- 3 edges to [[_COMMUNITY_PV-Namespaces]]
- 2 edges to [[_COMMUNITY_Registerpläne]]
- 2 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 2 edges to [[_COMMUNITY_Gruppe 52]]
- 2 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 2 edges to [[_COMMUNITY_DeviceConnection]]
- 2 edges to [[_COMMUNITY_DualCharger]]
- 2 edges to [[_COMMUNITY_Modbus-Tests]]
- 2 edges to [[_COMMUNITY_PV-Engine & Fehler]]
- 2 edges to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_Gruppe 55]]
- 1 edge to [[_COMMUNITY_Gruppe 51]]
- 1 edge to [[_COMMUNITY_CsvLogger]]
- 1 edge to [[_COMMUNITY_Gruppe 88]]
- 1 edge to [[_COMMUNITY_Gruppe 71]]
- 1 edge to [[_COMMUNITY_Gruppe 41]]
- 1 edge to [[_COMMUNITY_Gruppe 58]]
- 1 edge to [[_COMMUNITY_Gruppe 67]]
- 1 edge to [[_COMMUNITY_Gruppe 89]]
- 1 edge to [[_COMMUNITY_Gruppe 49]]
- 1 edge to [[_COMMUNITY_TimeSyncServer]]
- 1 edge to [[_COMMUNITY_TimeState & TimeLine]]
- 1 edge to [[_COMMUNITY_MasterClock]]
- 1 edge to [[_COMMUNITY_SyncedClock]]

## Top bridge nodes
- [[ModbusDeviceServer]] - degree 37, connects to 9 communities
- [[BatteryModbusTests]] - degree 17, connects to 6 communities
- [[PvSystem.StepContinuous()]] - degree 10, connects to 6 communities
- [[ModbusTestBase]] - degree 17, connects to 5 communities
- [[BatteryModbus]] - degree 14, connects to 5 communities