---
type: community
cohesion: 0.09
members: 50
---

# Sync-Nachrichten

**Cohesion:** 0.09 - loosely connected
**Members:** 50 nodes

## Members
- [[ByeMessage]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[CancellationTokenSource]] - code
- [[Channel]] - code
- [[Channel_1]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[ClientConnection_1]] - code
- [[ClientConnection]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[ClientConnection.MarkClosed()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[ClientConnection.Report()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[ClientConnection.Touch()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[ClientId]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[Clients]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[DeviceTimeSourceTests.WithMaster_FollowsMasterClock()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[HeartbeatInterval_1]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[HelloMessage]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[IPEndPoint]] - code
- [[IsOpen]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[JsonSerializerOptions]] - code
- [[LocalEndPoint]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[MasterConsole.ShutdownAsync()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MemberData]] - code
- [[MessageChannel.ReceiveAsync()]] - code - src/SimCore.TimeSync/MessageChannel.cs
- [[PingMessage]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[PongMessage]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[Program.Main()]] - code - src/SimulationMaster/Program.cs
- [[StatesMessage]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[SyncMessage]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[SyncMessages.cs]] - code - src/SimCore.Contracts/SyncMessages.cs
- [[SyncProtocol]] - code - src/SimCore.Contracts/SyncProtocol.cs
- [[SyncProtocol.Deserialize()]] - code - src/SimCore.Contracts/SyncProtocol.cs
- [[SyncProtocol.IsInvalid()]] - code - src/SimCore.Contracts/SyncProtocol.cs
- [[SyncProtocol.Serialize()]] - code - src/SimCore.Contracts/SyncProtocol.cs
- [[SyncProtocolTests]] - code - tests/SimCore.Tests/SyncProtocolTests.cs
- [[SyncProtocolTests.Message_IsReadableJson()]] - code - tests/SimCore.Tests/SyncProtocolTests.cs
- [[SyncProtocolTests.Message_SurvivesRoundTrip()]] - code - tests/SimCore.Tests/SyncProtocolTests.cs
- [[SyncProtocolTests.Messages()]] - code - tests/SimCore.Tests/SyncProtocolTests.cs
- [[TcpListener]] - code
- [[TheoryData]] - code
- [[TimeSyncIntegrationTests.InitializeAsync()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncServer]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.AcceptLoopAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.BroadcastLoopAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.HandleClientAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.HeartbeatLoopAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.IsConnectionError()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.ReceiveHelloAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.Register()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.Report()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.RunAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.SendToAllAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.Start()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Sync-Nachrichten
SORT file.name ASC
```

## Connections to other communities
- 24 edges to [[_COMMUNITY_TimeSyncServer]]
- 15 edges to [[_COMMUNITY_Sync-Nachrichten (2)]]
- 9 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 8 edges to [[_COMMUNITY_MasterClock]]
- 8 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 7 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 4 edges to [[_COMMUNITY_Gruppe 65]]
- 3 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 3 edges to [[_COMMUNITY_Gruppe 48]]
- 2 edges to [[_COMMUNITY_Gruppe 52]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource (2)]]
- 2 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 2 edges to [[_COMMUNITY_JSON-Serialisierung]]
- 2 edges to [[_COMMUNITY_Gruppe 23]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_Aufgabe 03 & Nachweise]]
- 1 edge to [[_COMMUNITY_Funktionsnachweis mit Master]]
- 1 edge to [[_COMMUNITY_Gruppe 44]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 1 edge to [[_COMMUNITY_DeviceConnection (Modbus.Client)]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop (2)]]
- 1 edge to [[_COMMUNITY_Nachrichtentypen (Contract)]]

## Top bridge nodes
- [[TimeSyncServer]] - degree 39, connects to 11 communities
- [[DeviceTimeSourceTests.WithMaster_FollowsMasterClock()]] - degree 11, connects to 6 communities
- [[TimeSyncServer.HandleClientAsync()]] - degree 18, connects to 4 communities
- [[ClientConnection]] - degree 15, connects to 4 communities
- [[Program.Main()]] - degree 12, connects to 4 communities