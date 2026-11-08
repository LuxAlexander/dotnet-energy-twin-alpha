---
source_file: "src/SimCore.Time/MasterClock.cs"
type: "code"
community: "Master-Zeit Tests"
location: "L100"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Master-Zeit_Tests
---

# MasterClock.SetSpeed()

## Connections
- [[DeviceTimeSource.ChangeSpeed()]] - `calls` [INFERRED]
- [[DeviceTimeSource.SetSpeed()]] - `calls` [INFERRED]
- [[MasterClock]] - `method` [EXTRACTED]
- [[MasterClock.Change()]] - `calls` [EXTRACTED]
- [[MasterClock.ValidateSpeed()]] - `calls` [EXTRACTED]
- [[MasterClockTests.Change_DoesNotTakeEffectBeforeLeadTime()]] - `calls` [INFERRED]
- [[MasterClockTests.ChangesWithinLeadTime_StayContinuous()]] - `calls` [INFERRED]
- [[MasterClockTests.CurrentStates_ContainsPendingChange()]] - `calls` [INFERRED]
- [[MasterClockTests.EveryChange_IncreasesSequence_AndRaisesEvent()]] - `calls` [INFERRED]
- [[MasterClockTests.RepeatedCommand_CreatesNoNewState()]] - `calls` [INFERRED]
- [[MasterClockTests.SetSpeed_ChangesRate_WithoutJump()]] - `calls` [INFERRED]
- [[MasterClockTests.SetSpeed_OutOfRange_IsRejected()]] - `calls` [INFERRED]
- [[MasterConsole.ChangeSpeed()]] - `calls` [INFERRED]
- [[SyncedClockTests.DifferentDeliveryDelays_GiveSameSimTime()]] - `calls` [INFERRED]
- [[SyncedClockTests.RestartedMaster_ReplacesTimeLine()]] - `calls` [INFERRED]
- [[TimeState]] - `references` [EXTRACTED]
- [[TimeSyncIntegrationTests.TwoSimulators_FollowStartPauseSpeedAndRestart()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Master-Zeit_Tests