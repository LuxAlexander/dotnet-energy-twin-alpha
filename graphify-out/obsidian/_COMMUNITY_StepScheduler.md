---
type: community
cohesion: 0.25
members: 15
---

# StepScheduler

**Cohesion:** 0.25 - loosely connected
**Members:** 15 nodes

## Members
- [[ArgumentOutOfRangeException]] - code
- [[MaxCatchUpSteps]] - code - src/Devices.Common/StepScheduler.cs
- [[ModelTime]] - code - src/Devices.Common/StepScheduler.cs
- [[RunId_2]] - code - src/Devices.Common/StepScheduler.cs
- [[Step]] - code - src/Devices.Common/StepScheduler.cs
- [[StepScheduler]] - code - src/Devices.Common/StepScheduler.cs
- [[StepScheduler.Update()]] - code - src/Devices.Common/StepScheduler.cs
- [[StepSchedulerTests]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.ClockSlightlyBehindModel_WaitsInsteadOfGoingBack()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.FirstUpdate_StartsAtClockTime_WithoutSteps()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.InvalidStep_IsRejected()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.NewRun_RestartsAtNewTime_EvenIfEarlier()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.OnlyFullStepsAreDue_ModelLagsAtMostOneStep()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.SameTimeline_GivesSameSteps_RegardlessOfPollingRate()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.TooManyDueSteps_AreLimited_AndReported()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/StepScheduler
SORT file.name ASC
```

## Connections to other communities
- 8 edges to [[_COMMUNITY_Gruppe 23]]
- 3 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 3 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 2 edges to [[_COMMUNITY_Gruppe 65]]
- 2 edges to [[_COMMUNITY_StepScheduler (2)]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_Gruppe 21]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]
- 1 edge to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 72]]

## Top bridge nodes
- [[StepScheduler]] - degree 19, connects to 7 communities
- [[StepScheduler.Update()]] - degree 13, connects to 4 communities
- [[StepSchedulerTests]] - degree 10, connects to 3 communities
- [[ArgumentOutOfRangeException]] - degree 3, connects to 2 communities
- [[StepSchedulerTests.ClockSlightlyBehindModel_WaitsInsteadOfGoingBack()]] - degree 4, connects to 1 community