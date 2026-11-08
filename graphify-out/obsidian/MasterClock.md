---
source_file: "src/SimCore.Time/MasterClock.cs"
type: "code"
community: "MasterClock"
location: "L12"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/MasterClock
---

# MasterClock

## Connections
- [[Current]] - `defines` [EXTRACTED]
- [[CurrentStates]] - `defines` [EXTRACTED]
- [[DateTime]] - `references` [EXTRACTED]
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[DeviceTimeSource]] - `references` [EXTRACTED]
- [[DeviceTimeSource.Create()]] - `calls` [INFERRED]
- [[DeviceTimeSourceTests.WithMaster_FollowsMasterClock()]] - `calls` [INFERRED]
- [[IReadOnlyList]] - `references` [EXTRACTED]
- [[ISimClock]] - `implements` [EXTRACTED]
- [[Latest]] - `defines` [EXTRACTED]
- [[LeadTime]] - `defines` [EXTRACTED]
- [[Lock]] - `references` [EXTRACTED]
- [[MasterClock.Change()]] - `method` [EXTRACTED]
- [[MasterClock.Pause()]] - `method` [EXTRACTED]
- [[MasterClock.Restart()]] - `method` [EXTRACTED]
- [[MasterClock.Resume()]] - `method` [EXTRACTED]
- [[MasterClock.SetSpeed()]] - `method` [EXTRACTED]
- [[MasterClock.ValidateSpeed()]] - `method` [EXTRACTED]
- [[MasterClock.cs]] - `contains` [EXTRACTED]
- [[MasterClockTests.CreateRunning()]] - `references` [EXTRACTED]
- [[MasterClockTests.NewClock_IsPausedAtStartTime()]] - `calls` [INFERRED]
- [[MasterClockTests.Resume_StartsAfterLeadTime_WithConfiguredSpeed()]] - `calls` [INFERRED]
- [[MasterConsole]] - `references` [EXTRACTED]
- [[MasterNow]] - `defines` [EXTRACTED]
- [[Mode_3]] - `defines` [EXTRACTED]
- [[MonotonicUtcClock]] - `references` [EXTRACTED]
- [[Now_1]] - `defines` [EXTRACTED]
- [[Program.Main()_2]] - `calls` [INFERRED]
- [[RunId_6]] - `defines` [EXTRACTED]
- [[SimMode]] - `references` [EXTRACTED]
- [[Speed_1]] - `defines` [EXTRACTED]
- [[SyncedClockTests.AssertAllEqual()]] - `references` [EXTRACTED]
- [[SyncedClockTests.AsymmetricDelay_ErrorStaysWithinReportedUncertainty()]] - `calls` [INFERRED]
- [[SyncedClockTests.DifferentDeliveryDelays_GiveSameSimTime()]] - `calls` [INFERRED]
- [[SyncedClockTests.LateState_IsAppliedAndReported()]] - `calls` [INFERRED]
- [[SyncedClockTests.MasterLost_FreezesTime_UntilContactReturns()]] - `calls` [INFERRED]
- [[SyncedClockTests.NewDevice_DuringPendingChange_UsesAllCurrentStates()]] - `calls` [INFERRED]
- [[SyncedClockTests.OffsetEstimation_CorrectsDifferentComputerClocks()]] - `calls` [INFERRED]
- [[SyncedClockTests.OldOrDuplicateStates_AreIgnored()]] - `calls` [INFERRED]
- [[SyncedClockTests.PauseAndRestart_ReachAllDevicesAtTheSameTime()]] - `calls` [INFERRED]
- [[SyncedClockTests.RestartedMaster_ReplacesTimeLine()]] - `calls` [INFERRED]
- [[TimeLine]] - `references` [EXTRACTED]
- [[TimeLine.Apply()]] - `calls` [INFERRED]
- [[TimeSpan]] - `references` [EXTRACTED]
- [[TimeState]] - `references` [EXTRACTED]
- [[TimeSyncIntegrationTests]] - `references` [EXTRACTED]
- [[TimeSyncIntegrationTests.InitializeAsync()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.MasterShutdown_IsReported_AndSimulatorStopsTime()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.SimulatorStarted_BeforeMaster_ConnectsLater()]] - `calls` [INFERRED]
- [[TimeSyncServer]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/MasterClock