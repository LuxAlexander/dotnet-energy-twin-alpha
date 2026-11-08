---
source_file: "src/SimCore.Time/SyncedClock.cs"
type: "code"
community: "MasterClock"
location: "L142"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/MasterClock
---

# SyncedClock.AddSyncSample()

## Connections
- [[ClockOffsetEstimator.AddSample()]] - `calls` [INFERRED]
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[SyncedClock]] - `method` [EXTRACTED]
- [[SyncedClockTests.AsymmetricDelay_ErrorStaysWithinReportedUncertainty()]] - `calls` [INFERRED]
- [[SyncedClockTests.OffsetEstimation_CorrectsDifferentComputerClocks()]] - `calls` [INFERRED]
- [[TimeSyncClient.ReceiveLoopAsync()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/MasterClock