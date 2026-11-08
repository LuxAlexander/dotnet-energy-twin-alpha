---
source_file: "src/SimCore.TimeSync/TimeSyncClient.cs"
type: "code"
community: "Sync-Nachrichten"
location: "L12"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Sync-Nachrichten
---

# TimeSyncClient

## Connections
- [[ConcurrentDictionary]] - `references` [EXTRACTED]
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[DeviceTimeSource]] - `references` [EXTRACTED]
- [[DeviceTimeSource.Create()]] - `calls` [INFERRED]
- [[Endpoint_1]] - `defines` [EXTRACTED]
- [[IsConnected_1]] - `defines` [EXTRACTED]
- [[LastError]] - `defines` [EXTRACTED]
- [[SyncedClock]] - `references` [EXTRACTED]
- [[TimeSpan]] - `references` [EXTRACTED]
- [[TimeSyncClient.ApplyStates()]] - `method` [EXTRACTED]
- [[TimeSyncClient.IgnoreEndAsync()]] - `method` [EXTRACTED]
- [[TimeSyncClient.PingLoopAsync()]] - `method` [EXTRACTED]
- [[TimeSyncClient.ReceiveLoopAsync()]] - `method` [EXTRACTED]
- [[TimeSyncClient.Report()]] - `method` [EXTRACTED]
- [[TimeSyncClient.RunAsync()]] - `method` [EXTRACTED]
- [[TimeSyncClient.RunConnectionAsync()]] - `method` [EXTRACTED]
- [[TimeSyncClient.SendByeAsync()]] - `method` [EXTRACTED]
- [[TimeSyncClient.SendPingAsync()]] - `method` [EXTRACTED]
- [[TimeSyncClient.cs]] - `contains` [EXTRACTED]
- [[TimeSyncIntegrationTests.MasterShutdown_IsReported_AndSimulatorStopsTime()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.SimulatorStarted_BeforeMaster_ConnectsLater()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.StartSimulator()]] - `calls` [INFERRED]

#graphify/code #graphify/EXTRACTED #community/Sync-Nachrichten