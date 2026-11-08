---
type: community
cohesion: 0.12
members: 33
---

# TimeSyncServer

**Cohesion:** 0.12 - loosely connected
**Members:** 33 nodes

## Members
- [[CancellationTokenSource]] - code
- [[Channel]] - code
- [[ClientConnection]] - code
- [[Clients]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[DeviceTimeSourceTests.WithMaster_FollowsMasterClock()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[HeartbeatInterval]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[IAsyncLifetime]] - code
- [[IPEndPoint]] - code
- [[LocalEndPoint]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[Program_2]] - code - src/SimulationMaster/Program.cs
- [[Program.Main()_2]] - code - src/SimulationMaster/Program.cs
- [[TcpListener]] - code
- [[TimeSyncIntegrationTests]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.AssertWithinTolerance()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.DisposeAsync()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.FreePort()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.Garbage_DisconnectsOnlyThatClient()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.InitializeAsync()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.InvalidClient_IsRejected_WithVisibleMessage()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.MasterShutdown_IsReported_AndSimulatorStopsTime()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.Server_ListsSimulators_WithReportedDeviation()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.SimulatorStarted_BeforeMaster_ConnectsLater()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.StartSimulator()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.TwoSimulators_FollowStartPauseSpeedAndRestart()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.WaitUntilAsync()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncServer]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.AcceptLoopAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.BroadcastLoopAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.HeartbeatLoopAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.IsConnectionError()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.RunAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.SendToAllAsync()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[TimeSyncServer.Start()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/TimeSyncServer
SORT file.name ASC
```

## Connections to other communities
- 24 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 22 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 12 edges to [[_COMMUNITY_MasterClock]]
- 10 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 7 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 3 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 3 edges to [[_COMMUNITY_Gruppe 51]]
- 3 edges to [[_COMMUNITY_SyncedClock]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource]]
- 2 edges to [[_COMMUNITY_Shared Konsolenausgabe]]
- 2 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 1 edge to [[_COMMUNITY_Gruppe 55]]
- 1 edge to [[_COMMUNITY_DeviceConnection]]
- 1 edge to [[_COMMUNITY_Register-Dekodierung]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_WeatherFeed]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]

## Top bridge nodes
- [[TimeSyncServer]] - degree 37, connects to 10 communities
- [[TimeSyncIntegrationTests]] - degree 21, connects to 6 communities
- [[TimeSyncIntegrationTests.MasterShutdown_IsReported_AndSimulatorStopsTime()]] - degree 12, connects to 5 communities
- [[DeviceTimeSourceTests.WithMaster_FollowsMasterClock()]] - degree 11, connects to 5 communities
- [[TimeSyncIntegrationTests.SimulatorStarted_BeforeMaster_ConnectsLater()]] - degree 11, connects to 5 communities