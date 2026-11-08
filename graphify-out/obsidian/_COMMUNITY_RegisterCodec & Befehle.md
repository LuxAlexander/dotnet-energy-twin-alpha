---
type: community
cohesion: 0.25
members: 19
---

# RegisterCodec & Befehle

**Cohesion:** 0.25 - loosely connected
**Members:** 19 nodes

## Members
- [[IOException]] - code
- [[ModbusCommand]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.TryDequeueCommand()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServerTests]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.FreePort()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.Header()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.InvalidValue_IsRejected_AndOldValueStays()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.PartialWriteOfFloat_IsRejected()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.PowerSwitchAndReset_AreWritableCommands()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.Publish_SequenceGrowsOnlyWithNewModelTime()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.Read()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.ReadFloat()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.Start_OnBusyPort_ThrowsIOException()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.ValidWrite_IsAccepted_AndQueuedForDevice()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.WaitForCommand()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.WriteFloat()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.WriteSpanningReadOnlyAndWritable_IsRejectedCompletely()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.WriteToReadOnlyRegister_IsRejected()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.WriteToUnusedAddress_IsRejected()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/RegisterCodec__Befehle
SORT file.name ASC
```

## Connections to other communities
- 9 edges to [[_COMMUNITY_RegisterCodec & Befehle (2)]]
- 8 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 8 edges to [[_COMMUNITY_Gruppe 23]]
- 3 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 3 edges to [[_COMMUNITY_Gruppe 48]]
- 2 edges to [[_COMMUNITY_Schreibprüfung]]
- 2 edges to [[_COMMUNITY_Modbus-Tests]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_Registerpläne]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 1 edge to [[_COMMUNITY_Gruppe 89]]
- 1 edge to [[_COMMUNITY_Batterie-Modell]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 1 edge to [[_COMMUNITY_DualCharger]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (2)]]

## Top bridge nodes
- [[ModbusDeviceServerTests]] - degree 28, connects to 10 communities
- [[ModbusDeviceServer.TryDequeueCommand()]] - degree 15, connects to 7 communities
- [[ModbusCommand]] - degree 6, connects to 3 communities
- [[ModbusDeviceServerTests.Header()]] - degree 5, connects to 3 communities
- [[ModbusDeviceServerTests.WriteSpanningReadOnlyAndWritable_IsRejectedCompletely()]] - degree 6, connects to 2 communities