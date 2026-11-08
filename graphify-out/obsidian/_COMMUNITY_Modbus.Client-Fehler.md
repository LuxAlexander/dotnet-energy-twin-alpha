---
type: community
cohesion: 0.25
members: 9
---

# Modbus.Client-Fehler

**Cohesion:** 0.25 - loosely connected
**Members:** 9 nodes

## Members
- [[Connection_1]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DeviceConnection.cs]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DiagnosticErrorKind]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DiagnosticException]] - code - src/Modbus.Client/DeviceConnection.cs
- [[Kind]] - code - src/Modbus.Client/DeviceConnection.cs
- [[Modbus.Client]] - code - src/Modbus.Client/DeviceConnection.cs
- [[Protocol]] - code - src/Modbus.Client/DeviceConnection.cs
- [[Value]] - code - src/Modbus.Client/DeviceConnection.cs
- [[easymodbus_exceptions]] - concept

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/ModbusClient-Fehler
SORT file.name ASC
```

## Connections to other communities
- 4 edges to [[_COMMUNITY_DeviceConnection (Modbus.Client)]]
- 2 edges to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_Nachrichtentypen (Contract)]]
- 1 edge to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]

## Top bridge nodes
- [[DeviceConnection.cs]] - degree 7, connects to 2 communities
- [[DiagnosticException]] - degree 7, connects to 2 communities
- [[Modbus.Client]] - degree 2, connects to 1 community