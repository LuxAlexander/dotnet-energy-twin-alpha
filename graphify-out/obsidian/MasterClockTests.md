---
source_file: "tests/SimCore.Tests/MasterClockTests.cs"
type: "code"
community: "Gruppe 23"
location: "L6"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Gruppe_23
---

# MasterClockTests

## Connections
- [[DateTime]] - `references` [EXTRACTED]
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[ManualTimeProvider_1]] - `references` [EXTRACTED]
- [[MasterClockTests.Change_DoesNotTakeEffectBeforeLeadTime()]] - `method` [EXTRACTED]
- [[MasterClockTests.ChangesWithinLeadTime_StayContinuous()]] - `method` [EXTRACTED]
- [[MasterClockTests.CreateRunning()]] - `method` [EXTRACTED]
- [[MasterClockTests.CurrentStates_ContainsPendingChange()]] - `method` [EXTRACTED]
- [[MasterClockTests.EveryChange_IncreasesSequence_AndRaisesEvent()]] - `method` [EXTRACTED]
- [[MasterClockTests.NewClock_IsPausedAtStartTime()]] - `method` [EXTRACTED]
- [[MasterClockTests.Pause_StopsTime_WithoutJump()]] - `method` [EXTRACTED]
- [[MasterClockTests.RepeatedCommand_CreatesNoNewState()]] - `method` [EXTRACTED]
- [[MasterClockTests.Restart_CanStartPaused()]] - `method` [EXTRACTED]
- [[MasterClockTests.Restart_SetsNewTime_ForNewRun()]] - `method` [EXTRACTED]
- [[MasterClockTests.Resume_ContinuesAtSameTime_WithoutJump()]] - `method` [EXTRACTED]
- [[MasterClockTests.Resume_StartsAfterLeadTime_WithConfiguredSpeed()]] - `method` [EXTRACTED]
- [[MasterClockTests.SetSpeed_ChangesRate_WithoutJump()]] - `method` [EXTRACTED]
- [[MasterClockTests.SetSpeed_OutOfRange_IsRejected()]] - `method` [EXTRACTED]
- [[MasterClockTests.SystemClockJump_DoesNotChangeSimTime()]] - `method` [EXTRACTED]
- [[MasterClockTests.cs]] - `contains` [EXTRACTED]
- [[TimeSpan]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Gruppe_23