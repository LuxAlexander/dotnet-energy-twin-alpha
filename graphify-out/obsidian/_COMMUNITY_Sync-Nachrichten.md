---
type: community
cohesion: 0.12
members: 41
---

# Sync-Nachrichten

**Cohesion:** 0.12 - loosely connected
**Members:** 41 nodes

## Members
- [[ByeMessage]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[ClientConnection.Report()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[Endpoint_1]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[HelloMessage]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[IsConnected_1]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[LastError]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[MemberData]] - code
- [[MessageChannel]] - code - src/SimCore.TimeSync/MessageChannel.cs
- [[MessageChannel.ReceiveAsync()]] - code - src/SimCore.TimeSync/MessageChannel.cs
- [[MessageChannel.SendAsync()]] - code - src/SimCore.TimeSync/MessageChannel.cs
- [[PingMessage]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[PongMessage]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[RemoteEndPoint]] - code - src/SimCore.TimeSync/MessageChannel.cs
- [[SemaphoreSlim]] - code
- [[StatesMessage]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[StreamReader]] - code
- [[SyncMessage]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[SyncMessages.cs]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[SyncProtocol.Deserialize()]] - code - src/SimCore.Contracts/SyncProtocol.cs
- [[SyncProtocol.Serialize()]] - code - src/SimCore.Contracts/SyncProtocol.cs
- [[SyncProtocolTests]] - code - tests/SimCore.Tests/SyncProtocolTests.cs
- [[SyncProtocolTests.Message_IsReadableJson()]] - code - tests/SimCore.Tests/SyncProtocolTests.cs
- [[SyncProtocolTests.Message_SurvivesRoundTrip()]] - code - tests/SimCore.Tests/SyncProtocolTests.cs
- [[SyncProtocolTests.Messages()]] - code - tests/SimCore.Tests/SyncProtocolTests.cs
- [[TcpClient]] - code
- [[TheoryData]] - code
- [[TimeSyncClient]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.ApplyStates()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.IgnoreEndAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.PingLoopAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.ReceiveLoopAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.Report()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.RunAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.RunConnectionAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.SendByeAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncClient.SendPingAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncServer.HandleClientAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.ReceiveHelloAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.Register()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.Report()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[UTF8Encoding]] - code

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Sync-Nachrichten
SORT file.name ASC
```

## Connections to other communities
- 22 edges to [[_COMMUNITY_TimeSyncServer]]
- 21 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 11 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 4 edges to [[_COMMUNITY_CsvLogger]]
- 4 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 3 edges to [[_COMMUNITY_TimeSyncOptions Tests]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource]]
- 2 edges to [[_COMMUNITY_WeatherFeed]]
- 2 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 2 edges to [[_COMMUNITY_MasterClock]]
- 1 edge to [[_COMMUNITY_Gruppe 53]]
- 1 edge to [[_COMMUNITY_Register-Dekodierung]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests]]
- 1 edge to [[_COMMUNITY_SyncedClock]]
- 1 edge to [[_COMMUNITY_Gruppe 51]]
- 1 edge to [[_COMMUNITY_Master-Zeit Tests]]

## Top bridge nodes
- [[TimeSyncClient]] - degree 22, connects to 7 communities
- [[TimeSyncServer.HandleClientAsync()]] - degree 18, connects to 4 communities
- [[MessageChannel]] - degree 20, connects to 3 communities
- [[StatesMessage]] - degree 10, connects to 3 communities
- [[SyncProtocol.Deserialize()]] - degree 6, connects to 3 communities