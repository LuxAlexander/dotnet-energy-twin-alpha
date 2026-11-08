---
type: community
cohesion: 0.15
members: 28
---

# MasterClock

**Cohesion:** 0.15 - loosely connected
**Members:** 28 nodes

## Members
- [[Current]] - code - src/SimCore.Time/MasterClock.cs
- [[CurrentStates]] - code - src/SimCore.Time/MasterClock.cs
- [[Latest]] - code - src/SimCore.Time/MasterClock.cs
- [[LeadTime]] - code - src/SimCore.Time/MasterClock.cs
- [[MasterClock]] - code - src/SimCore.Time/MasterClock.cs
- [[MasterClock.Resume()]] - code - src/SimCore.Time/MasterClock.cs
- [[MasterClock.ValidateSpeed()]] - code - src/SimCore.Time/MasterClock.cs
- [[MasterNow]] - code - src/SimCore.Time/MasterClock.cs
- [[Mode_3]] - code - src/SimCore.Time/MasterClock.cs
- [[Now_1]] - code - src/SimCore.Time/MasterClock.cs
- [[RunId_6]] - code - src/SimCore.Time/MasterClock.cs
- [[Speed_1]] - code - src/SimCore.Time/MasterClock.cs
- [[SyncedClock.AddSyncSample()]] - code - src/SimCore.Time/SyncedClock.cs
- [[SyncedClock.ApplyState()]] - code - src/SimCore.Time/SyncedClock.cs
- [[SyncedClockTests]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.AdvanceBoth()]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.AssertAllEqual()]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.AsymmetricDelay_ErrorStaysWithinReportedUncertainty()]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.BeforeFirstState_WaitsForMaster()]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.Deliver()]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.DifferentDeliveryDelays_GiveSameSimTime()]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.LateState_IsAppliedAndReported()]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.MasterLost_FreezesTime_UntilContactReturns()]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.NewDevice_DuringPendingChange_UsesAllCurrentStates()]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.OffsetEstimation_CorrectsDifferentComputerClocks()]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.OldOrDuplicateStates_AreIgnored()]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.PauseAndRestart_ReachAllDevicesAtTheSameTime()]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[SyncedClockTests.RestartedMaster_ReplacesTimeLine()]] - code - tests/SimCore.Tests/SyncedClockTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/MasterClock
SORT file.name ASC
```

## Connections to other communities
- 33 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 13 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 12 edges to [[_COMMUNITY_TimeSyncServer]]
- 12 edges to [[_COMMUNITY_SyncedClock]]
- 5 edges to [[_COMMUNITY_DeviceTimeSource]]
- 4 edges to [[_COMMUNITY_Gruppe 84]]
- 3 edges to [[_COMMUNITY_Gruppe 51]]
- 2 edges to [[_COMMUNITY_Gruppe 63]]
- 2 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 2 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 2 edges to [[_COMMUNITY_WeatherFeed]]
- 2 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 1 edge to [[_COMMUNITY_Gruppe 54]]
- 1 edge to [[_COMMUNITY_Register-Dekodierung]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]

## Top bridge nodes
- [[MasterClock]] - degree 50, connects to 11 communities
- [[MasterClock.Resume()]] - degree 25, connects to 5 communities
- [[SyncedClockTests]] - degree 18, connects to 5 communities
- [[SyncedClock.AddSyncSample()]] - degree 6, connects to 4 communities
- [[SyncedClock.ApplyState()]] - degree 13, connects to 3 communities