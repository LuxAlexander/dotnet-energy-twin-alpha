---
source_file: "tests/SimCore.Tests/MasterClockTests.cs"
type: "code"
community: "Master-Zeit Tests"
location: "L14"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Master-Zeit_Tests
---

# MasterClockTests.CreateRunning()

## Connections
- [[ManualTimeProvider.Advance()_1]] - `calls` [INFERRED]
- [[MasterClock]] - `references` [EXTRACTED]
- [[MasterClock.Resume()]] - `calls` [INFERRED]
- [[MasterClockTests]] - `method` [EXTRACTED]
- [[MasterClockTests.Change_DoesNotTakeEffectBeforeLeadTime()]] - `calls` [EXTRACTED]
- [[MasterClockTests.ChangesWithinLeadTime_StayContinuous()]] - `calls` [EXTRACTED]
- [[MasterClockTests.CurrentStates_ContainsPendingChange()]] - `calls` [EXTRACTED]
- [[MasterClockTests.EveryChange_IncreasesSequence_AndRaisesEvent()]] - `calls` [EXTRACTED]
- [[MasterClockTests.Pause_StopsTime_WithoutJump()]] - `calls` [EXTRACTED]
- [[MasterClockTests.RepeatedCommand_CreatesNoNewState()]] - `calls` [EXTRACTED]
- [[MasterClockTests.Restart_CanStartPaused()]] - `calls` [EXTRACTED]
- [[MasterClockTests.Restart_SetsNewTime_ForNewRun()]] - `calls` [EXTRACTED]
- [[MasterClockTests.Resume_ContinuesAtSameTime_WithoutJump()]] - `calls` [EXTRACTED]
- [[MasterClockTests.SetSpeed_ChangesRate_WithoutJump()]] - `calls` [EXTRACTED]
- [[MasterClockTests.SetSpeed_OutOfRange_IsRejected()]] - `calls` [EXTRACTED]
- [[MasterClockTests.SystemClockJump_DoesNotChangeSimTime()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Master-Zeit_Tests