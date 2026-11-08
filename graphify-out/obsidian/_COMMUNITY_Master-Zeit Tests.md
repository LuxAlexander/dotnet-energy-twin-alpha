---
type: community
cohesion: 0.25
members: 24
---

# Master-Zeit Tests

**Cohesion:** 0.25 - loosely connected
**Members:** 24 nodes

## Members
- [[Fact]] - code
- [[ManualTimeProvider.Advance()_1]] - code - tests/SimCore.Tests/ManualTimeProvider.cs
- [[MasterClock.Pause()]] - code - src/SimCore.Time/MasterClock.cs
- [[MasterClock.Restart()]] - code - src/SimCore.Time/MasterClock.cs
- [[MasterClock.SetSpeed()]] - code - src/SimCore.Time/MasterClock.cs
- [[MasterClockTests]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.Change_DoesNotTakeEffectBeforeLeadTime()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.ChangesWithinLeadTime_StayContinuous()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.CreateRunning()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.CurrentStates_ContainsPendingChange()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.EveryChange_IncreasesSequence_AndRaisesEvent()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.NewClock_IsPausedAtStartTime()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.Pause_StopsTime_WithoutJump()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.RepeatedCommand_CreatesNoNewState()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.Restart_CanStartPaused()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.Restart_SetsNewTime_ForNewRun()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.Resume_ContinuesAtSameTime_WithoutJump()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.Resume_StartsAfterLeadTime_WithConfiguredSpeed()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.SetSpeed_ChangesRate_WithoutJump()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.SetSpeed_OutOfRange_IsRejected()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterClockTests.SystemClockJump_DoesNotChangeSimTime()]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[RegisterPlanRequirementTests]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[RegisterPlanRequirementTests.EveryDevicePlan_HasSetpointAndAtLeastThreeSpecificValues()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[RegisterPlanRequirementTests.Markdown_ListsAllRegisters_WithByteAndWordOrder()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Master-Zeit_Tests
SORT file.name ASC
```

## Connections to other communities
- 33 edges to [[_COMMUNITY_MasterClock]]
- 15 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 14 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 11 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 11 edges to [[_COMMUNITY_Wettermodell & Tests]]
- 10 edges to [[_COMMUNITY_TimeSyncServer]]
- 8 edges to [[_COMMUNITY_StepScheduler]]
- 8 edges to [[_COMMUNITY_Gruppe 57]]
- 7 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 7 edges to [[_COMMUNITY_Gruppe 45]]
- 6 edges to [[_COMMUNITY_DeviceTimeSource]]
- 5 edges to [[_COMMUNITY_WeatherFeed]]
- 4 edges to [[_COMMUNITY_TimeSyncOptions Tests]]
- 4 edges to [[_COMMUNITY_Gruppe 52]]
- 4 edges to [[_COMMUNITY_Register-Dekodierung]]
- 4 edges to [[_COMMUNITY_Gruppe 54]]
- 3 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 3 edges to [[_COMMUNITY_Gruppe 42]]
- 3 edges to [[_COMMUNITY_Gruppe 84]]
- 2 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 2 edges to [[_COMMUNITY_Gruppe 58]]
- 2 edges to [[_COMMUNITY_Gruppe 43]]
- 2 edges to [[_COMMUNITY_Gruppe 51]]
- 1 edge to [[_COMMUNITY_Registerpläne]]
- 1 edge to [[_COMMUNITY_Schreibprüfung]]
- 1 edge to [[_COMMUNITY_Gruppe 55]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_ArgsParser]]
- 1 edge to [[_COMMUNITY_Gruppe 75]]
- 1 edge to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]

## Top bridge nodes
- [[Fact]] - degree 139, connects to 24 communities
- [[MasterClockTests]] - degree 20, connects to 5 communities
- [[MasterClock.SetSpeed()]] - degree 17, connects to 5 communities
- [[MasterClock.Pause()]] - degree 13, connects to 5 communities
- [[MasterClock.Restart()]] - degree 10, connects to 5 communities