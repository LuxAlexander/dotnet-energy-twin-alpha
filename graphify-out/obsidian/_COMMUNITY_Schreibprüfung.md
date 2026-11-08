---
type: community
cohesion: 0.12
members: 20
---

# Schreibprüfung

**Cohesion:** 0.12 - loosely connected
**Members:** 20 nodes

## Members
- [[Count]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[DiagnosticSession.ResolveTarget()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[EndAddress]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[Float32]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[IsWritable]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[ModbusDeviceServer.Validate()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ReadOnly]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[ReadWrite]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[RegisterAccess]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[RegisterCodecTests.Validate_ChecksRangeTypeAndFiniteness()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[RegisterDefinition]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[RegisterDefinition.Contains()]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[RegisterDefinition.FormatLimit()]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[RegisterDefinition.Validate()]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[RegisterPlan.Find()]] - code - src/Modbus.Contracts/RegisterPlan.cs
- [[RegisterPlan.Get()]] - code - src/Modbus.Contracts/RegisterPlan.cs
- [[RegisterType]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[RegisterWriter.Set()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[UInt16]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[UInt32]] - code - src/Modbus.Contracts/RegisterDefinition.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Schreibprüfung
SORT file.name ASC
```

## Connections to other communities
- 7 edges to [[_COMMUNITY_Gruppe 52]]
- 6 edges to [[_COMMUNITY_RegisterCodec & Befehle (2)]]
- 6 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 4 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 3 edges to [[_COMMUNITY_Registerpläne]]
- 3 edges to [[_COMMUNITY_Modbus-Tests]]
- 2 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 1 edge to [[_COMMUNITY_Gruppe 82]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_Gruppe 23]]

## Top bridge nodes
- [[RegisterDefinition]] - degree 30, connects to 8 communities
- [[ModbusDeviceServer.Validate()]] - degree 8, connects to 5 communities
- [[RegisterWriter.Set()]] - degree 4, connects to 2 communities
- [[RegisterCodecTests.Validate_ChecksRangeTypeAndFiniteness()]] - degree 4, connects to 2 communities
- [[RegisterType]] - degree 6, connects to 1 community