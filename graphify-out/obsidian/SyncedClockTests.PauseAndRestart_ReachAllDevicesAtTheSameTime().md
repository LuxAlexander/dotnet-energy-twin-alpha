---
source_file: "tests/SimCore.Tests/SyncedClockTests.cs"
type: "code"
community: "MasterClock"
location: "L58"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/MasterClock
---

# SyncedClockTests.PauseAndRestart_ReachAllDevicesAtTheSameTime()

## Connections
- [[Fact]] - `references` [EXTRACTED]
- [[ManualTimeProvider.Advance()_1]] - `calls` [INFERRED]
- [[MasterClock]] - `calls` [INFERRED]
- [[MasterClock.Pause()]] - `calls` [INFERRED]
- [[MasterClock.Restart()]] - `calls` [INFERRED]
- [[MasterClock.Resume()]] - `calls` [INFERRED]
- [[SyncedClock]] - `calls` [INFERRED]
- [[SyncedClockTests]] - `method` [EXTRACTED]
- [[SyncedClockTests.AssertAllEqual()]] - `calls` [EXTRACTED]
- [[SyncedClockTests.Deliver()]] - `calls` [EXTRACTED]

#graphify/code #graphify/INFERRED #community/MasterClock