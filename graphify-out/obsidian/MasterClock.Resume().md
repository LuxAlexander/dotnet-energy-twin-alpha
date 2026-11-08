---
source_file: "src/SimCore.Time/MasterClock.cs"
type: "code"
community: "MasterClock"
location: "L94"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/MasterClock
---

# MasterClock.Resume()

## Connections
- [[DeviceTimeSource.Create()]] - `calls` [INFERRED]
- [[DeviceTimeSource.TogglePause()]] - `calls` [INFERRED]
- [[DeviceTimeSourceTests.WithMaster_FollowsMasterClock()]] - `calls` [INFERRED]
- [[MasterClock]] - `method` [EXTRACTED]
- [[MasterClock.Change()]] - `calls` [EXTRACTED]
- [[MasterClockTests.ChangesWithinLeadTime_StayContinuous()]] - `calls` [INFERRED]
- [[MasterClockTests.CreateRunning()]] - `calls` [INFERRED]
- [[MasterClockTests.EveryChange_IncreasesSequence_AndRaisesEvent()]] - `calls` [INFERRED]
- [[MasterClockTests.Resume_ContinuesAtSameTime_WithoutJump()]] - `calls` [INFERRED]
- [[MasterClockTests.Resume_StartsAfterLeadTime_WithConfiguredSpeed()]] - `calls` [INFERRED]
- [[MasterConsole.TogglePause()]] - `calls` [INFERRED]
- [[Program.Main()_2]] - `calls` [INFERRED]
- [[SyncedClockTests.AsymmetricDelay_ErrorStaysWithinReportedUncertainty()]] - `calls` [INFERRED]
- [[SyncedClockTests.DifferentDeliveryDelays_GiveSameSimTime()]] - `calls` [INFERRED]
- [[SyncedClockTests.LateState_IsAppliedAndReported()]] - `calls` [INFERRED]
- [[SyncedClockTests.MasterLost_FreezesTime_UntilContactReturns()]] - `calls` [INFERRED]
- [[SyncedClockTests.NewDevice_DuringPendingChange_UsesAllCurrentStates()]] - `calls` [INFERRED]
- [[SyncedClockTests.OffsetEstimation_CorrectsDifferentComputerClocks()]] - `calls` [INFERRED]
- [[SyncedClockTests.OldOrDuplicateStates_AreIgnored()]] - `calls` [INFERRED]
- [[SyncedClockTests.PauseAndRestart_ReachAllDevicesAtTheSameTime()]] - `calls` [INFERRED]
- [[SyncedClockTests.RestartedMaster_ReplacesTimeLine()]] - `calls` [INFERRED]
- [[TimeState]] - `references` [EXTRACTED]
- [[TimeSyncIntegrationTests.MasterShutdown_IsReported_AndSimulatorStopsTime()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.Server_ListsSimulators_WithReportedDeviation()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.TwoSimulators_FollowStartPauseSpeedAndRestart()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/MasterClock