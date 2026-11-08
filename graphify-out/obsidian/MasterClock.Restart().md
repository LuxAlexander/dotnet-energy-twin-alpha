---
source_file: "src/SimCore.Time/MasterClock.cs"
type: "code"
community: "Gruppe 23"
location: "L109"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Gruppe_23
---

# MasterClock.Restart()

## Connections
- [[DateTime]] - `references` [EXTRACTED]
- [[MasterClock]] - `method` [EXTRACTED]
- [[MasterClock.Change()]] - `calls` [EXTRACTED]
- [[MasterClockTests.EveryChange_IncreasesSequence_AndRaisesEvent()]] - `calls` [INFERRED]
- [[MasterClockTests.Restart_CanStartPaused()]] - `calls` [INFERRED]
- [[MasterClockTests.Restart_SetsNewTime_ForNewRun()]] - `calls` [INFERRED]
- [[MasterConsole.Restart()]] - `calls` [INFERRED]
- [[SyncedClockTests.PauseAndRestart_ReachAllDevicesAtTheSameTime()]] - `calls` [INFERRED]
- [[TimeState]] - `references` [EXTRACTED]
- [[TimeSyncIntegrationTests.TwoSimulators_FollowStartPauseSpeedAndRestart()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Gruppe_23