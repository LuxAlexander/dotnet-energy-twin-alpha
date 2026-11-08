---
type: community
cohesion: 0.20
members: 18
---

# DeviceConnection

**Cohesion:** 0.20 - loosely connected
**Members:** 18 nodes

## Members
- [[DeviceConnection]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[DeviceConnection.Close()]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[DeviceConnection.Connect()]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[DeviceConnection.Describe()]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[DeviceConnection.Dispose()]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[DeviceConnection.IsConnectionProblem()]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[DeviceConnection.IsModbusProblem()]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[DeviceConnection.Translate()]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[DeviceConnection.Write()]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[DiagnosticClientTests.Dispose()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[Endpoint]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[Exception]] - code
- [[Host_1]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[IsConnected]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[ModbusClient]] - code
- [[Port_2]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[TimeoutMs_1]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[UnitId_2]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/DeviceConnection
SORT file.name ASC
```

## Connections to other communities
- 15 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_Schreibprüfung]]
- 1 edge to [[_COMMUNITY_WeatherFeed]]
- 1 edge to [[_COMMUNITY_TimeSyncServer]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle]]

## Top bridge nodes
- [[DeviceConnection]] - degree 21, connects to 3 communities
- [[Exception]] - degree 7, connects to 3 communities
- [[DeviceConnection.Write()]] - degree 8, connects to 2 communities
- [[ModbusClient]] - degree 3, connects to 2 communities
- [[DeviceConnection.Connect()]] - degree 7, connects to 1 community