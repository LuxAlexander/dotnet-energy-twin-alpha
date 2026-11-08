---
type: community
cohesion: 0.18
members: 20
---

# Sync-Nachrichten (2)

**Cohesion:** 0.18 - loosely connected
**Members:** 20 nodes

## Members
- [[ConcurrentDictionary]] - code
- [[Endpoint]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[IsConnected]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[LastError]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[MessageChannel]] - code - src/SimCore.TimeSync/MessageChannel.cs
- [[MessageChannel.SendAsync()]] - code - src/SimCore.TimeSync/MessageChannel.cs
- [[RemoteEndPoint]] - code - src/SimCore.TimeSync/MessageChannel.cs
- [[SemaphoreSlim]] - code
- [[StreamReader]] - code
- [[TcpClient]] - code
- [[TimeSyncClient]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.ApplyStates()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.IgnoreEndAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.PingLoopAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.ReceiveLoopAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.Report()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.RunConnectionAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.SendByeAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.SendPingAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[UTF8Encoding]] - code

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Sync-Nachrichten_2
SORT file.name ASC
```

## Connections to other communities
- 15 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 13 edges to [[_COMMUNITY_TimeSyncServer]]
- 6 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 2 edges to [[_COMMUNITY_Batterie-Simulator Loop (2)]]
- 2 edges to [[_COMMUNITY_MasterClock]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 1 edge to [[_COMMUNITY_SyncedClock]]
- 1 edge to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]
- 1 edge to [[_COMMUNITY_TimeState & TimeLine]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource (2)]]

## Top bridge nodes
- [[TimeSyncClient]] - degree 22, connects to 7 communities
- [[MessageChannel]] - degree 20, connects to 4 communities
- [[TimeSyncClient.ReceiveLoopAsync()]] - degree 9, connects to 4 communities
- [[TimeSyncClient.RunConnectionAsync()]] - degree 12, connects to 3 communities
- [[MessageChannel.SendAsync()]] - degree 10, connects to 3 communities