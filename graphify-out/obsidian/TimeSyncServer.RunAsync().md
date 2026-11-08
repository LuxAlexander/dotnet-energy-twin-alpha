---
source_file: "src/SimCore.TimeSync/TimeSyncServer.cs"
type: "code"
community: "TimeSyncServer"
location: "L68"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/TimeSyncServer
---

# TimeSyncServer.RunAsync()

## Connections
- [[ByeMessage]] - `calls` [INFERRED]
- [[CancellationToken]] - `references` [EXTRACTED]
- [[DeviceTimeSourceTests.WithMaster_FollowsMasterClock()]] - `calls` [INFERRED]
- [[Program.Main()_2]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]
- [[TimeSyncIntegrationTests.InitializeAsync()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.MasterShutdown_IsReported_AndSimulatorStopsTime()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.SimulatorStarted_BeforeMaster_ConnectsLater()]] - `calls` [INFERRED]
- [[TimeSyncServer]] - `method` [EXTRACTED]
- [[TimeSyncServer.AcceptLoopAsync()]] - `calls` [EXTRACTED]
- [[TimeSyncServer.BroadcastLoopAsync()]] - `calls` [EXTRACTED]
- [[TimeSyncServer.HeartbeatLoopAsync()]] - `calls` [EXTRACTED]
- [[TimeSyncServer.SendToAllAsync()]] - `calls` [EXTRACTED]
- [[TimeSyncServer.Start()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/TimeSyncServer