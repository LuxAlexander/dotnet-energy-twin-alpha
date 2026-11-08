---
type: community
cohesion: 0.11
members: 23
---

# Schreibprüfung

**Cohesion:** 0.11 - loosely connected
**Members:** 23 nodes

## Members
- [[BatteryModbus.ValidateValue()]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[Count]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[DiagnosticSession.Format()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.RegisterCodecEquals()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[EndAddress]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[Float32]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[IsWritable]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[ModbusDeviceServer.Validate()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ReadOnly]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[ReadWrite]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[RegisterAccess]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[RegisterCodec.Encode()]] - code - src/Modbus.Contracts/RegisterCodec.cs
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
- 9 edges to [[_COMMUNITY_Register-Dekodierung]]
- 7 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 7 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 5 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 3 edges to [[_COMMUNITY_Charger-Namespaces]]
- 3 edges to [[_COMMUNITY_Registerpläne]]
- 2 edges to [[_COMMUNITY_Gruppe 48]]
- 2 edges to [[_COMMUNITY_DeviceConnection]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_Master-Zeit Tests]]

## Top bridge nodes
- [[RegisterDefinition]] - degree 31, connects to 8 communities
- [[ModbusDeviceServer.Validate()]] - degree 8, connects to 5 communities
- [[RegisterCodec.Encode()]] - degree 6, connects to 2 communities
- [[DiagnosticSession.RegisterCodecEquals()]] - degree 4, connects to 2 communities
- [[RegisterCodecTests.Validate_ChecksRangeTypeAndFiniteness()]] - degree 4, connects to 2 communities