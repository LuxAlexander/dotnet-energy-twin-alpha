---
source_file: "tests/SimCore.Tests/TimeSyncIntegrationTests.cs"
type: "code"
community: "TimeSyncServer"
location: "L177"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/TimeSyncServer
---

# TimeSyncIntegrationTests.StartSimulator()

## Connections
- [[SyncedClock]] - `references` [EXTRACTED]
- [[TimeSyncClient]] - `calls` [INFERRED]
- [[TimeSyncClient.RunAsync()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests]] - `method` [EXTRACTED]
- [[TimeSyncIntegrationTests.Garbage_DisconnectsOnlyThatClient()]] - `calls` [EXTRACTED]
- [[TimeSyncIntegrationTests.Server_ListsSimulators_WithReportedDeviation()]] - `calls` [EXTRACTED]
- [[TimeSyncIntegrationTests.TwoSimulators_FollowStartPauseSpeedAndRestart()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/TimeSyncServer