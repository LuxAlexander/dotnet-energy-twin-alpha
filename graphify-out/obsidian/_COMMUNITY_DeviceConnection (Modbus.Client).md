---
type: community
cohesion: 0.23
members: 18
---

# DeviceConnection (Modbus.Client)

**Cohesion:** 0.23 - loosely connected
**Members:** 18 nodes

## Members
- [[DeviceConnection]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DeviceConnection.Close()]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DeviceConnection.Connect()]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DeviceConnection.Describe()]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DeviceConnection.Dispose()]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DeviceConnection.IsConnectionProblem()]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DeviceConnection.IsModbusProblem()]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DeviceConnection.ReadRegisters()]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DeviceConnection.Translate()]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DeviceConnection.Write()]] - code - src/Modbus.Client/DeviceConnection.cs
- [[DevicePoller.Dispose()]] - code - src/ModbusRabbitGateway/DevicePoller.cs
- [[Endpoint_1]] - code - src/Modbus.Client/DeviceConnection.cs
- [[Exception]] - code
- [[Host_1]] - code - src/Modbus.Client/DeviceConnection.cs
- [[IsConnected_1]] - code - src/Modbus.Client/DeviceConnection.cs
- [[Port_3]] - code - src/Modbus.Client/DeviceConnection.cs
- [[TimeoutMs_1]] - code - src/Modbus.Client/DeviceConnection.cs
- [[UnitId_2]] - code - src/Modbus.Client/DeviceConnection.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/DeviceConnection_ModbusClient
SORT file.name ASC
```

## Connections to other communities
- 4 edges to [[_COMMUNITY_Modbus.Client-Fehler]]
- 3 edges to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]
- 3 edges to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 1 edge to [[_COMMUNITY_Status-Wiederholung & RabbitMQ]]
- 1 edge to [[_COMMUNITY_LB03-Projektstand]]
- 1 edge to [[_COMMUNITY_Messwert-Schlüssel]]

## Top bridge nodes
- [[DeviceConnection]] - degree 24, connects to 7 communities
- [[Exception]] - degree 8, connects to 4 communities
- [[DeviceConnection.Connect()]] - degree 7, connects to 1 community
- [[DeviceConnection.Translate()]] - degree 7, connects to 1 community
- [[DeviceConnection.ReadRegisters()]] - degree 6, connects to 1 community