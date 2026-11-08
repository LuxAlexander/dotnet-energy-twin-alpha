---
source_file: "src/SimCore.Time/MasterClock.cs"
type: "code"
community: "Gruppe 23"
location: "L89"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Gruppe_23
---

# MasterClock.Pause()

## Connections
- [[DeviceTimeSource.TogglePause()]] - `calls` [INFERRED]
- [[MasterClock]] - `method` [EXTRACTED]
- [[MasterClock.Change()]] - `calls` [EXTRACTED]
- [[MasterClockTests.ChangesWithinLeadTime_StayContinuous()]] - `calls` [INFERRED]
- [[MasterClockTests.EveryChange_IncreasesSequence_AndRaisesEvent()]] - `calls` [INFERRED]
- [[MasterClockTests.Pause_StopsTime_WithoutJump()]] - `calls` [INFERRED]
- [[MasterClockTests.RepeatedCommand_CreatesNoNewState()]] - `calls` [INFERRED]
- [[MasterClockTests.Resume_ContinuesAtSameTime_WithoutJump()]] - `calls` [INFERRED]
- [[MasterConsole.TogglePause()]] - `calls` [INFERRED]
- [[SyncedClockTests.NewDevice_DuringPendingChange_UsesAllCurrentStates()]] - `calls` [INFERRED]
- [[SyncedClockTests.PauseAndRestart_ReachAllDevicesAtTheSameTime()]] - `calls` [INFERRED]
- [[TimeState]] - `references` [EXTRACTED]
- [[TimeSyncIntegrationTests.TwoSimulators_FollowStartPauseSpeedAndRestart()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Gruppe_23