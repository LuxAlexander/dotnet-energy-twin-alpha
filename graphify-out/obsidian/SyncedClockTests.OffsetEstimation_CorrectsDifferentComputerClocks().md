---
source_file: "tests/SimCore.Tests/SyncedClockTests.cs"
type: "code"
community: "MasterClock"
location: "L163"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/MasterClock
---

# SyncedClockTests.OffsetEstimation_CorrectsDifferentComputerClocks()

## Connections
- [[Fact]] - `references` [EXTRACTED]
- [[ManualTimeProvider_1]] - `calls` [INFERRED]
- [[MasterClock]] - `calls` [INFERRED]
- [[MasterClock.Resume()]] - `calls` [INFERRED]
- [[SyncedClock]] - `calls` [INFERRED]
- [[SyncedClock.AddSyncSample()]] - `calls` [INFERRED]
- [[SyncedClock.ApplyState()]] - `calls` [INFERRED]
- [[SyncedClockTests]] - `method` [EXTRACTED]
- [[SyncedClockTests.AdvanceBoth()]] - `calls` [EXTRACTED]

#graphify/code #graphify/INFERRED #community/MasterClock