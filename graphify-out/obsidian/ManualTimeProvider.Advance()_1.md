---
source_file: "tests/SimCore.Tests/ManualTimeProvider.cs"
type: "code"
community: "Master-Zeit Tests"
location: "L14"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Master-Zeit_Tests
---

# ManualTimeProvider.Advance()

## Connections
- [[ManualTimeProvider_1]] - `method` [EXTRACTED]
- [[MasterClockTests.Change_DoesNotTakeEffectBeforeLeadTime()]] - `calls` [INFERRED]
- [[MasterClockTests.ChangesWithinLeadTime_StayContinuous()]] - `calls` [INFERRED]
- [[MasterClockTests.CreateRunning()]] - `calls` [INFERRED]
- [[MasterClockTests.NewClock_IsPausedAtStartTime()]] - `calls` [INFERRED]
- [[MasterClockTests.Pause_StopsTime_WithoutJump()]] - `calls` [INFERRED]
- [[MasterClockTests.Restart_CanStartPaused()]] - `calls` [INFERRED]
- [[MasterClockTests.Restart_SetsNewTime_ForNewRun()]] - `calls` [INFERRED]
- [[MasterClockTests.Resume_ContinuesAtSameTime_WithoutJump()]] - `calls` [INFERRED]
- [[MasterClockTests.Resume_StartsAfterLeadTime_WithConfiguredSpeed()]] - `calls` [INFERRED]
- [[MasterClockTests.SetSpeed_ChangesRate_WithoutJump()]] - `calls` [INFERRED]
- [[SyncedClockTests.AdvanceBoth()]] - `calls` [INFERRED]
- [[SyncedClockTests.DifferentDeliveryDelays_GiveSameSimTime()]] - `calls` [INFERRED]
- [[SyncedClockTests.LateState_IsAppliedAndReported()]] - `calls` [INFERRED]
- [[SyncedClockTests.MasterLost_FreezesTime_UntilContactReturns()]] - `calls` [INFERRED]
- [[SyncedClockTests.NewDevice_DuringPendingChange_UsesAllCurrentStates()]] - `calls` [INFERRED]
- [[SyncedClockTests.PauseAndRestart_ReachAllDevicesAtTheSameTime()]] - `calls` [INFERRED]
- [[TimeSpan]] - `references` [EXTRACTED]

#graphify/code #graphify/INFERRED #community/Master-Zeit_Tests