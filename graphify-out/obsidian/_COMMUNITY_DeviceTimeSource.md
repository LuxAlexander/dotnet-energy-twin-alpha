---
type: community
cohesion: 0.12
members: 17
---

# DeviceTimeSource

**Cohesion:** 0.12 - loosely connected
**Members:** 17 nodes

## Members
- [[Clock]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource.FormatSpan()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource.SetSpeed()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource.Speed()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[HasTime]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[IsRunning]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[IsStandalone]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[MasterLost]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[ModbusDeviceServer.MapStatus()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[NoTime]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[Options_2]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[Standalone]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[Status_1]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[StatusText_5]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[Synced]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[TimeStatus]] - code - src/Modbus.Contracts/CommonRegisters.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/DeviceTimeSource
SORT file.name ASC
```

## Connections to other communities
- 5 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 4 edges to [[_COMMUNITY_DeviceTimeSource (2)]]
- 2 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 2 edges to [[_COMMUNITY_PV-Engine & Fehler]]
- 2 edges to [[_COMMUNITY_Gruppe 88]]
- 2 edges to [[_COMMUNITY_SyncedClock]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 1 edge to [[_COMMUNITY_Gruppe 82]]
- 1 edge to [[_COMMUNITY_Qualitätsstatus & Pause]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle]]
- 1 edge to [[_COMMUNITY_MasterClock]]
- 1 edge to [[_COMMUNITY_Gruppe 21]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 44]]
- 1 edge to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]
- 1 edge to [[_COMMUNITY_Gruppe 23]]

## Top bridge nodes
- [[DeviceTimeSource]] - degree 36, connects to 18 communities
- [[TimeStatus]] - degree 7, connects to 2 communities
- [[DeviceTimeSource.SetSpeed()]] - degree 4, connects to 2 communities
- [[ModbusDeviceServer.MapStatus()]] - degree 4, connects to 1 community
- [[DeviceTimeSource.Speed()]] - degree 3, connects to 1 community