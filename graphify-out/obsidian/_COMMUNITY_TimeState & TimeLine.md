---
type: community
cohesion: 0.16
members: 21
---

# TimeState & TimeLine

**Cohesion:** 0.16 - loosely connected
**Members:** 21 nodes

## Members
- [[Channel_1]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[ClientConnection_1]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[ClientConnection.MarkClosed()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[ClientConnection.Snapshot()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[ClientConnection.Touch()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[ClientId]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[DateTimeOffset]] - code
- [[Guid]] - code
- [[IsOpen]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[Latest_1]] - code - src/SimCore.Time/TimeLine.cs
- [[MasterClock.Change()]] - code - src/SimCore.Time/MasterClock.cs
- [[States]] - code - src/SimCore.Time/TimeLine.cs
- [[TimeLine]] - code - src/SimCore.Time/TimeLine.cs
- [[TimeLine.Apply()]] - code - src/SimCore.Time/TimeLine.cs
- [[TimeLine.Prune()]] - code - src/SimCore.Time/TimeLine.cs
- [[TimeLine.SimTimeAt()]] - code - src/SimCore.Time/TimeLine.cs
- [[TimeLine.StateAt()]] - code - src/SimCore.Time/TimeLine.cs
- [[TimeState]] - code - src/SimCore.Contracts/TimeState.cs
- [[TimeState.SimTimeAt()]] - code - src/SimCore.Contracts/TimeState.cs
- [[TimeState.cs]] - code - src/SimCore.Contracts/TimeState.cs
- [[TimeSyncServer.OnStateChanged()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/TimeState__TimeLine
SORT file.name ASC
```

## Connections to other communities
- 13 edges to [[_COMMUNITY_MasterClock]]
- 11 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 7 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 5 edges to [[_COMMUNITY_SyncedClock]]
- 3 edges to [[_COMMUNITY_WeatherFeed]]
- 2 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 2 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 2 edges to [[_COMMUNITY_TimeSyncServer]]
- 2 edges to [[_COMMUNITY_Gruppe 51]]
- 2 edges to [[_COMMUNITY_Gruppe 54]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource]]
- 2 edges to [[_COMMUNITY_Gruppe 84]]
- 1 edge to [[_COMMUNITY_Gruppe 63]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_Register-Dekodierung]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]

## Top bridge nodes
- [[DateTimeOffset]] - degree 25, connects to 8 communities
- [[TimeState]] - degree 22, connects to 7 communities
- [[TimeLine]] - degree 12, connects to 5 communities
- [[ClientConnection_1]] - degree 15, connects to 4 communities
- [[MasterClock.Change()]] - degree 10, connects to 3 communities