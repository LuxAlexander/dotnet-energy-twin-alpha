---
source_file: "tests/SimCore.Tests/SyncedClockTests.cs"
type: "code"
community: "MasterClock"
location: "L6"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/MasterClock
---

# SyncedClockTests

## Connections
- [[DateTime]] - `references` [EXTRACTED]
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[ManualTimeProvider_1]] - `references` [EXTRACTED]
- [[SyncedClockTests.AdvanceBoth()]] - `method` [EXTRACTED]
- [[SyncedClockTests.AssertAllEqual()]] - `method` [EXTRACTED]
- [[SyncedClockTests.AsymmetricDelay_ErrorStaysWithinReportedUncertainty()]] - `method` [EXTRACTED]
- [[SyncedClockTests.BeforeFirstState_WaitsForMaster()]] - `method` [EXTRACTED]
- [[SyncedClockTests.Deliver()]] - `method` [EXTRACTED]
- [[SyncedClockTests.DifferentDeliveryDelays_GiveSameSimTime()]] - `method` [EXTRACTED]
- [[SyncedClockTests.LateState_IsAppliedAndReported()]] - `method` [EXTRACTED]
- [[SyncedClockTests.MasterLost_FreezesTime_UntilContactReturns()]] - `method` [EXTRACTED]
- [[SyncedClockTests.NewDevice_DuringPendingChange_UsesAllCurrentStates()]] - `method` [EXTRACTED]
- [[SyncedClockTests.OffsetEstimation_CorrectsDifferentComputerClocks()]] - `method` [EXTRACTED]
- [[SyncedClockTests.OldOrDuplicateStates_AreIgnored()]] - `method` [EXTRACTED]
- [[SyncedClockTests.PauseAndRestart_ReachAllDevicesAtTheSameTime()]] - `method` [EXTRACTED]
- [[SyncedClockTests.RestartedMaster_ReplacesTimeLine()]] - `method` [EXTRACTED]
- [[SyncedClockTests.cs]] - `contains` [EXTRACTED]
- [[TimeSpan]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/MasterClock