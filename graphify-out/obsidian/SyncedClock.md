---
source_file: "src/SimCore.Time/SyncedClock.cs"
type: "code"
community: "SyncedClock"
location: "L23"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/SyncedClock
---

# SyncedClock

## Connections
- [[ClockOffsetEstimator]] - `references` [EXTRACTED]
- [[DateTime]] - `references` [EXTRACTED]
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[DeviceTimeSource]] - `references` [EXTRACTED]
- [[DeviceTimeSource.Create()]] - `calls` [INFERRED]
- [[HeartbeatTimeout]] - `defines` [EXTRACTED]
- [[ISimClock]] - `implements` [EXTRACTED]
- [[LastLateness]] - `defines` [EXTRACTED]
- [[LocalNow]] - `defines` [EXTRACTED]
- [[Lock]] - `references` [EXTRACTED]
- [[MasterNow_1]] - `defines` [EXTRACTED]
- [[Mode_2]] - `defines` [EXTRACTED]
- [[MonotonicUtcClock]] - `references` [EXTRACTED]
- [[Now_2]] - `defines` [EXTRACTED]
- [[OffsetToMaster]] - `defines` [EXTRACTED]
- [[RoundTrip]] - `defines` [EXTRACTED]
- [[RunId_3]] - `defines` [EXTRACTED]
- [[SimMode]] - `references` [EXTRACTED]
- [[SimUncertainty]] - `defines` [EXTRACTED]
- [[Speed_3]] - `defines` [EXTRACTED]
- [[Status_2]] - `defines` [EXTRACTED]
- [[SyncStatus]] - `references` [EXTRACTED]
- [[SyncedClock.AddSyncSample()]] - `method` [EXTRACTED]
- [[SyncedClock.ApplyState()]] - `method` [EXTRACTED]
- [[SyncedClock.EvaluationTime()]] - `method` [EXTRACTED]
- [[SyncedClock.StateNow()]] - `method` [EXTRACTED]
- [[SyncedClock.StatusCore()]] - `method` [EXTRACTED]
- [[SyncedClock.cs]] - `contains` [EXTRACTED]
- [[SyncedClockTests.AsymmetricDelay_ErrorStaysWithinReportedUncertainty()]] - `calls` [INFERRED]
- [[SyncedClockTests.BeforeFirstState_WaitsForMaster()]] - `calls` [INFERRED]
- [[SyncedClockTests.DifferentDeliveryDelays_GiveSameSimTime()]] - `calls` [INFERRED]
- [[SyncedClockTests.LateState_IsAppliedAndReported()]] - `calls` [INFERRED]
- [[SyncedClockTests.MasterLost_FreezesTime_UntilContactReturns()]] - `calls` [INFERRED]
- [[SyncedClockTests.NewDevice_DuringPendingChange_UsesAllCurrentStates()]] - `calls` [INFERRED]
- [[SyncedClockTests.OffsetEstimation_CorrectsDifferentComputerClocks()]] - `calls` [INFERRED]
- [[SyncedClockTests.OldOrDuplicateStates_AreIgnored()]] - `calls` [INFERRED]
- [[SyncedClockTests.PauseAndRestart_ReachAllDevicesAtTheSameTime()]] - `calls` [INFERRED]
- [[SyncedClockTests.RestartedMaster_ReplacesTimeLine()]] - `calls` [INFERRED]
- [[TimeLine]] - `references` [EXTRACTED]
- [[TimeSpan]] - `references` [EXTRACTED]
- [[TimeSyncClient]] - `references` [EXTRACTED]
- [[TimeSyncIntegrationTests.MasterShutdown_IsReported_AndSimulatorStopsTime()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.SimulatorStarted_BeforeMaster_ConnectsLater()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.StartSimulator()]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/SyncedClock