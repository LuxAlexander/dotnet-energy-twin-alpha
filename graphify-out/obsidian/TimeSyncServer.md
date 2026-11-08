---
source_file: "src/SimCore.TimeSync/TimeSyncServer.cs"
type: "code"
community: "TimeSyncServer"
location: "L13"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/TimeSyncServer
---

# TimeSyncServer

## Connections
- [[CancellationTokenSource]] - `references` [EXTRACTED]
- [[Channel]] - `references` [EXTRACTED]
- [[ClientConnection]] - `references` [EXTRACTED]
- [[ClientConnection_1]] - `contains` [EXTRACTED]
- [[Clients]] - `defines` [EXTRACTED]
- [[DeviceTimeSourceTests.WithMaster_FollowsMasterClock()]] - `calls` [INFERRED]
- [[Dictionary]] - `references` [EXTRACTED]
- [[HeartbeatInterval]] - `defines` [EXTRACTED]
- [[IPEndPoint]] - `references` [EXTRACTED]
- [[IReadOnlyList]] - `references` [EXTRACTED]
- [[List]] - `references` [EXTRACTED]
- [[LocalEndPoint]] - `defines` [EXTRACTED]
- [[Lock]] - `references` [EXTRACTED]
- [[MasterClock]] - `references` [EXTRACTED]
- [[MasterConsole]] - `references` [EXTRACTED]
- [[Program.Main()_2]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]
- [[TcpListener]] - `references` [EXTRACTED]
- [[TimeSpan]] - `references` [EXTRACTED]
- [[TimeSyncClientInfo]] - `references` [EXTRACTED]
- [[TimeSyncIntegrationTests]] - `references` [EXTRACTED]
- [[TimeSyncIntegrationTests.InitializeAsync()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.MasterShutdown_IsReported_AndSimulatorStopsTime()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.SimulatorStarted_BeforeMaster_ConnectsLater()]] - `calls` [INFERRED]
- [[TimeSyncServer.AcceptLoopAsync()]] - `method` [EXTRACTED]
- [[TimeSyncServer.BroadcastLoopAsync()]] - `method` [EXTRACTED]
- [[TimeSyncServer.HandleClientAsync()]] - `method` [EXTRACTED]
- [[TimeSyncServer.HeartbeatLoopAsync()]] - `method` [EXTRACTED]
- [[TimeSyncServer.IsConnectionError()]] - `method` [EXTRACTED]
- [[TimeSyncServer.OnStateChanged()]] - `method` [EXTRACTED]
- [[TimeSyncServer.ReceiveHelloAsync()]] - `method` [EXTRACTED]
- [[TimeSyncServer.Register()]] - `method` [EXTRACTED]
- [[TimeSyncServer.Report()]] - `method` [EXTRACTED]
- [[TimeSyncServer.RunAsync()]] - `method` [EXTRACTED]
- [[TimeSyncServer.SendToAllAsync()]] - `method` [EXTRACTED]
- [[TimeSyncServer.Start()]] - `method` [EXTRACTED]
- [[TimeSyncServer.cs]] - `contains` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/TimeSyncServer