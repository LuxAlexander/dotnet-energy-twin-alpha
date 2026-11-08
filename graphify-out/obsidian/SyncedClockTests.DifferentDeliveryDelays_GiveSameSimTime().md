---
source_file: "tests/SimCore.Tests/SyncedClockTests.cs"
type: "code"
community: "MasterClock"
location: "L28"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/MasterClock
---

# SyncedClockTests.DifferentDeliveryDelays_GiveSameSimTime()

## Connections
- [[Fact]] - `references` [EXTRACTED]
- [[ManualTimeProvider.Advance()_1]] - `calls` [INFERRED]
- [[MasterClock]] - `calls` [INFERRED]
- [[MasterClock.Resume()]] - `calls` [INFERRED]
- [[MasterClock.SetSpeed()]] - `calls` [INFERRED]
- [[SyncedClock]] - `calls` [INFERRED]
- [[SyncedClock.ApplyState()]] - `calls` [INFERRED]
- [[SyncedClockTests]] - `method` [EXTRACTED]
- [[SyncedClockTests.AssertAllEqual()]] - `calls` [EXTRACTED]

#graphify/code #graphify/INFERRED #community/MasterClock