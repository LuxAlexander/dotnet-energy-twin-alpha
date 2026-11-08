---
type: community
cohesion: 0.12
members: 20
---

# SyncedClock

**Cohesion:** 0.12 - loosely connected
**Members:** 20 nodes

## Members
- [[HeartbeatTimeout]] - code - src/SimCore.Time/SyncedClock.cs
- [[LastLateness]] - code - src/SimCore.Time/SyncedClock.cs
- [[LocalNow]] - code - src/SimCore.Time/SyncedClock.cs
- [[MasterLost_1]] - code - src/SimCore.Time/SyncedClock.cs
- [[MasterNow_1]] - code - src/SimCore.Time/SyncedClock.cs
- [[Mode_4]] - code - src/SimCore.Time/SyncedClock.cs
- [[Now_3]] - code - src/SimCore.Time/SyncedClock.cs
- [[OffsetToMaster]] - code - src/SimCore.Time/SyncedClock.cs
- [[RoundTrip_1]] - code - src/SimCore.Time/SyncedClock.cs
- [[RunId_7]] - code - src/SimCore.Time/SyncedClock.cs
- [[SimUncertainty]] - code - src/SimCore.Time/SyncedClock.cs
- [[Speed_2]] - code - src/SimCore.Time/SyncedClock.cs
- [[Status_2]] - code - src/SimCore.Time/SyncedClock.cs
- [[SyncStatus]] - code - src/SimCore.Time/SyncedClock.cs
- [[Synced_1]] - code - src/SimCore.Time/SyncedClock.cs
- [[SyncedClock]] - code - src/SimCore.Time/SyncedClock.cs
- [[SyncedClock.EvaluationTime()]] - code - src/SimCore.Time/SyncedClock.cs
- [[SyncedClock.StateNow()]] - code - src/SimCore.Time/SyncedClock.cs
- [[SyncedClock.StatusCore()]] - code - src/SimCore.Time/SyncedClock.cs
- [[WaitingForMaster]] - code - src/SimCore.Time/SyncedClock.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/SyncedClock
SORT file.name ASC
```

## Connections to other communities
- 12 edges to [[_COMMUNITY_MasterClock]]
- 5 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 4 edges to [[_COMMUNITY_DeviceTimeSource]]
- 3 edges to [[_COMMUNITY_TimeSyncServer]]
- 2 edges to [[_COMMUNITY_Gruppe 63]]
- 2 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_Gruppe 54]]
- 1 edge to [[_COMMUNITY_WeatherFeed]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_Gruppe 51]]

## Top bridge nodes
- [[SyncedClock]] - degree 44, connects to 11 communities
- [[SyncStatus]] - degree 7, connects to 2 communities
- [[SyncedClock.EvaluationTime()]] - degree 4, connects to 1 community
- [[SyncedClock.StateNow()]] - degree 4, connects to 1 community