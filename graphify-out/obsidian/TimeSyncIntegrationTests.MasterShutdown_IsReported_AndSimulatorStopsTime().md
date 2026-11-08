---
source_file: "tests/SimCore.Tests/TimeSyncIntegrationTests.cs"
type: "code"
community: "TimeSyncServer"
location: "L113"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/TimeSyncServer
---

# TimeSyncIntegrationTests.MasterShutdown_IsReported_AndSimulatorStopsTime()

## Connections
- [[Fact]] - `references` [EXTRACTED]
- [[MasterClock]] - `calls` [INFERRED]
- [[MasterClock.Resume()]] - `calls` [INFERRED]
- [[SyncedClock]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]
- [[TimeSyncClient]] - `calls` [INFERRED]
- [[TimeSyncClient.RunAsync()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests]] - `method` [EXTRACTED]
- [[TimeSyncIntegrationTests.FreePort()]] - `calls` [EXTRACTED]
- [[TimeSyncIntegrationTests.WaitUntilAsync()]] - `calls` [EXTRACTED]
- [[TimeSyncServer]] - `calls` [INFERRED]
- [[TimeSyncServer.RunAsync()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/TimeSyncServer