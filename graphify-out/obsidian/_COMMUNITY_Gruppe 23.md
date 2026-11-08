---
type: community
cohesion: 0.28
members: 23
---

# Gruppe 23

**Cohesion:** 0.28 - loosely connected
**Members:** 23 nodes

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
- [[TimeSyncIntegrationTests.AssertWithinTolerance()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.TwoSimulators_FollowStartPauseSpeedAndRestart()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Gruppe_23
SORT file.name ASC
```

## Connections to other communities
- 33 edges to [[_COMMUNITY_MasterClock]]
- 12 edges to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 11 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 10 edges to [[_COMMUNITY_TimeSyncServer]]
- 9 edges to [[_COMMUNITY_MessageBuilder-Tests]]
- 8 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 8 edges to [[_COMMUNITY_StepScheduler]]
- 8 edges to [[_COMMUNITY_Wettermodell & Tests (3)]]
- 8 edges to [[_COMMUNITY_Gruppe 72]]
- 7 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 7 edges to [[_COMMUNITY_RegisterCodec & Befehle (2)]]
- 7 edges to [[_COMMUNITY_WeatherFeed]]
- 7 edges to [[_COMMUNITY_Gruppe 61]]
- 7 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 6 edges to [[_COMMUNITY_Gateway-Optionen]]
- 5 edges to [[_COMMUNITY_DeviceTimeSource (2)]]
- 4 edges to [[_COMMUNITY_Gruppe 71]]
- 3 edges to [[_COMMUNITY_Gruppe 119]]
- 3 edges to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 3 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 3 edges to [[_COMMUNITY_Wettermodell & Tests]]
- 3 edges to [[_COMMUNITY_Gruppe 44]]
- 3 edges to [[_COMMUNITY_Gruppe 52]]
- 3 edges to [[_COMMUNITY_Gruppe 57]]
- 3 edges to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 2 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 2 edges to [[_COMMUNITY_Gruppe 65]]
- 2 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 2 edges to [[_COMMUNITY_Registerpläne]]
- 2 edges to [[_COMMUNITY_Gruppe 48]]
- 2 edges to [[_COMMUNITY_Gruppe 24]]
- 2 edges to [[_COMMUNITY_GatewayService]]
- 2 edges to [[_COMMUNITY_Messwert-Schlüssel]]
- 1 edge to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 1 edge to [[_COMMUNITY_SyncedClock]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource]]
- 1 edge to [[_COMMUNITY_Schreibprüfung]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_ArgsParser (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 104]]
- 1 edge to [[_COMMUNITY_Gruppe 109]]
- 1 edge to [[_COMMUNITY_Gruppe 105]]

## Top bridge nodes
- [[Fact]] - degree 168, connects to 35 communities
- [[MasterClockTests]] - degree 20, connects to 5 communities
- [[MasterClock.SetSpeed()]] - degree 17, connects to 5 communities
- [[MasterClock.Pause()]] - degree 13, connects to 4 communities
- [[MasterClock.Restart()]] - degree 10, connects to 4 communities