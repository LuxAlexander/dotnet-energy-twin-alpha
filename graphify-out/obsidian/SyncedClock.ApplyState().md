---
source_file: "src/SimCore.Time/SyncedClock.cs"
type: "code"
community: "MasterClock"
location: "L126"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/MasterClock
---

# SyncedClock.ApplyState()

## Connections
- [[SyncedClock]] - `method` [EXTRACTED]
- [[SyncedClockTests.AsymmetricDelay_ErrorStaysWithinReportedUncertainty()]] - `calls` [INFERRED]
- [[SyncedClockTests.DifferentDeliveryDelays_GiveSameSimTime()]] - `calls` [INFERRED]
- [[SyncedClockTests.LateState_IsAppliedAndReported()]] - `calls` [INFERRED]
- [[SyncedClockTests.MasterLost_FreezesTime_UntilContactReturns()]] - `calls` [INFERRED]
- [[SyncedClockTests.NewDevice_DuringPendingChange_UsesAllCurrentStates()]] - `calls` [INFERRED]
- [[SyncedClockTests.OffsetEstimation_CorrectsDifferentComputerClocks()]] - `calls` [INFERRED]
- [[SyncedClockTests.OldOrDuplicateStates_AreIgnored()]] - `calls` [INFERRED]
- [[SyncedClockTests.RestartedMaster_ReplacesTimeLine()]] - `calls` [INFERRED]
- [[TimeLine.Apply()]] - `calls` [INFERRED]
- [[TimeLine.Prune()]] - `calls` [INFERRED]
- [[TimeState]] - `references` [EXTRACTED]
- [[TimeSyncClient.ApplyStates()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/MasterClock