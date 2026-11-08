---
source_file: "tests/SimCore.Tests/SyncedClockTests.cs"
type: "code"
community: "MasterClock"
location: "L109"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/MasterClock
---

# SyncedClockTests.NewDevice_DuringPendingChange_UsesAllCurrentStates()

## Connections
- [[Fact]] - `references` [EXTRACTED]
- [[ManualTimeProvider.Advance()_1]] - `calls` [INFERRED]
- [[MasterClock]] - `calls` [INFERRED]
- [[MasterClock.Pause()]] - `calls` [INFERRED]
- [[MasterClock.Resume()]] - `calls` [INFERRED]
- [[SyncedClock]] - `calls` [INFERRED]
- [[SyncedClock.ApplyState()]] - `calls` [INFERRED]
- [[SyncedClockTests]] - `method` [EXTRACTED]

#graphify/code #graphify/INFERRED #community/MasterClock