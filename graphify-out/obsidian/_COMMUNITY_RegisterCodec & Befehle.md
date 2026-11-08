---
type: community
cohesion: 0.14
members: 34
---

# RegisterCodec & Befehle

**Cohesion:** 0.14 - loosely connected
**Members:** 34 nodes

## Members
- [[IOException]] - code
- [[ModbusCommand]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.TryDequeueCommand()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServerTests]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.FreePort()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.Header()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.Header_IsReadableWithIdentityStateAndTime()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.InvalidValue_IsRejected_AndOldValueStays()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.PartialWriteOfFloat_IsRejected()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.PowerSwitchAndReset_AreWritableCommands()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.Publish_SequenceGrowsOnlyWithNewModelTime()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.Read()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.ReadFloat()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.Registers_StayReadable_WhilePaused_AndKeepLastState()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.Start_OnBusyPort_ThrowsIOException()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.ValidWrite_IsAccepted_AndQueuedForDevice()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.WaitForCommand()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.WriteFloat()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.WriteSpanningReadOnlyAndWritable_IsRejectedCompletely()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.WriteToReadOnlyRegister_IsRejected()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.WriteToUnusedAddress_IsRejected()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[RegisterCodec]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterCodec.DecodeFloat32()]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterCodec.DecodeUInt32()]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterCodec.EncodeFloat32()]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterCodec.EncodeUInt32()]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterCodec.FromSeconds()]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterCodec.ToSeconds()]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterCodec.cs]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterCodecTests]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[RegisterCodecTests.Float32_IsBigEndianWithHighWordFirst()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[RegisterCodecTests.ModelTime_RoundTripsThroughSeconds()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[RegisterCodecTests.Plan_RejectsOverlapsAndRangeViolations()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[RegisterCodecTests.UInt32_RoundTrips()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/RegisterCodec__Befehle
SORT file.name ASC
```

## Connections to other communities
- 18 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 14 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 7 edges to [[_COMMUNITY_Schreibprüfung]]
- 7 edges to [[_COMMUNITY_Register-Dekodierung]]
- 4 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 4 edges to [[_COMMUNITY_WeatherFeed]]
- 3 edges to [[_COMMUNITY_Modbus-Tests]]
- 3 edges to [[_COMMUNITY_TimeSyncOptions Tests]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource]]
- 2 edges to [[_COMMUNITY_Registerpläne]]
- 1 edge to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 1 edge to [[_COMMUNITY_DualCharger]]
- 1 edge to [[_COMMUNITY_Gruppe 64]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_DeviceConnection]]

## Top bridge nodes
- [[ModbusDeviceServerTests]] - degree 28, connects to 8 communities
- [[RegisterCodecTests.Plan_RejectsOverlapsAndRangeViolations()]] - degree 5, connects to 4 communities
- [[ModbusDeviceServer.TryDequeueCommand()]] - degree 15, connects to 3 communities
- [[RegisterCodec]] - degree 10, connects to 3 communities
- [[ModbusDeviceServerTests.Registers_StayReadable_WhilePaused_AndKeepLastState()]] - degree 9, connects to 3 communities