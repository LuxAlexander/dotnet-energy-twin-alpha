---
source_file: "tests/SimCore.Tests/TimeSyncIntegrationTests.cs"
type: "code"
community: "TimeSyncServer"
location: "L89"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/TimeSyncServer
---

# TimeSyncIntegrationTests.SimulatorStarted_BeforeMaster_ConnectsLater()

## Connections
- [[Fact]] - `references` [EXTRACTED]
- [[MasterClock]] - `calls` [INFERRED]
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