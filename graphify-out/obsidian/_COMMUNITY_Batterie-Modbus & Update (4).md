---
type: community
cohesion: 0.31
members: 13
---

# Batterie-Modbus & Update (4)

**Cohesion:** 0.31 - loosely connected
**Members:** 13 nodes

## Members
- [[PvModbus]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[PvModbus.Dispose()]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[PvModbus.ProcessCommands()]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[PvModbus.Publish()]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[PvModbus.Start()]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[PvModbusTests]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbusTests.Dispose()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbusTests.OnOffAndReset_AreApplied()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbusTests.PowerLimit_OutOfRange_IsRejected_AndDeviceKeepsValue()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbusTests.PowerLimit_ValidValue_LimitsFeedInAtNextStep()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbusTests.Registers_StayReadable_WhileTimeIsPaused()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvSystem.StepContinuous()]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[StatusText_4]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Batterie-Modbus__Update_4
SORT file.name ASC
```

## Connections to other communities
- 15 edges to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 5 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 5 edges to [[_COMMUNITY_PvFaultModel]]
- 5 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 3 edges to [[_COMMUNITY_PvSystem]]
- 3 edges to [[_COMMUNITY_Gruppe 23]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 2 edges to [[_COMMUNITY_Modbus-Tests]]
- 2 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 2 edges to [[_COMMUNITY_Gruppe 48]]
- 2 edges to [[_COMMUNITY_RegisterCodec & Befehle (2)]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_Gruppe 67]]
- 1 edge to [[_COMMUNITY_Gruppe 38]]
- 1 edge to [[_COMMUNITY_Gruppe 91]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource (2)]]

## Top bridge nodes
- [[PvModbus]] - degree 14, connects to 7 communities
- [[PvSystem.StepContinuous()]] - degree 10, connects to 7 communities
- [[PvModbusTests]] - degree 16, connects to 5 communities
- [[PvModbusTests.Registers_StayReadable_WhileTimeIsPaused()]] - degree 7, connects to 4 communities
- [[PvModbus.Publish()]] - degree 10, connects to 3 communities