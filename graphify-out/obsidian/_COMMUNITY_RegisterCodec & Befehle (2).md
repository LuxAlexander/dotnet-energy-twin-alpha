---
type: community
cohesion: 0.22
members: 18
---

# RegisterCodec & Befehle (2)

**Cohesion:** 0.22 - loosely connected
**Members:** 18 nodes

## Members
- [[ModbusDeviceServerTests.Header_IsReadableWithIdentityStateAndTime()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDeviceServerTests.Registers_StayReadable_WhilePaused_AndKeepLastState()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[RegisterCodec]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterCodec.Decode()]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterCodec.DecodeFloat32()]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterCodec.DecodeUInt32()]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterCodec.Encode()]] - code - src/Modbus.Contracts/RegisterCodec.cs
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
- [[RegisterFormatterTests.Value_ModelTime_IsExactToTheSecond()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/RegisterCodec__Befehle_2
SORT file.name ASC
```

## Connections to other communities
- 9 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 9 edges to [[_COMMUNITY_Gruppe 52]]
- 7 edges to [[_COMMUNITY_Gruppe 23]]
- 6 edges to [[_COMMUNITY_Schreibprüfung]]
- 3 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 2 edges to [[_COMMUNITY_Modbus-Tests]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 1 edge to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 1 edge to [[_COMMUNITY_Registerpläne]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 48]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]

## Top bridge nodes
- [[ModbusDeviceServerTests.Registers_StayReadable_WhilePaused_AndKeepLastState()]] - degree 9, connects to 4 communities
- [[RegisterCodecTests.Plan_RejectsOverlapsAndRangeViolations()]] - degree 5, connects to 4 communities
- [[RegisterCodec.Decode()]] - degree 11, connects to 3 communities
- [[RegisterCodec.EncodeFloat32()]] - degree 8, connects to 3 communities
- [[RegisterCodec.FromSeconds()]] - degree 7, connects to 3 communities