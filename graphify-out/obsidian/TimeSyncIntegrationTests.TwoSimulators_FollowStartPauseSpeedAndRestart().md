---
source_file: "tests/SimCore.Tests/TimeSyncIntegrationTests.cs"
type: "code"
community: "TimeSyncServer"
location: "L46"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/TimeSyncServer
---

# TimeSyncIntegrationTests.TwoSimulators_FollowStartPauseSpeedAndRestart()

## Connections
- [[Fact]] - `references` [EXTRACTED]
- [[MasterClock.Pause()]] - `calls` [INFERRED]
- [[MasterClock.Restart()]] - `calls` [INFERRED]
- [[MasterClock.Resume()]] - `calls` [INFERRED]
- [[MasterClock.SetSpeed()]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]
- [[TimeSyncIntegrationTests]] - `method` [EXTRACTED]
- [[TimeSyncIntegrationTests.AssertWithinTolerance()]] - `calls` [EXTRACTED]
- [[TimeSyncIntegrationTests.StartSimulator()]] - `calls` [EXTRACTED]
- [[TimeSyncIntegrationTests.WaitUntilAsync()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/TimeSyncServer