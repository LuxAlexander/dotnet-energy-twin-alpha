---
type: community
cohesion: 0.15
members: 22
---

# StepScheduler

**Cohesion:** 0.15 - loosely connected
**Members:** 22 nodes

## Members
- [[Advance]] - code - src/Devices.Common/StepScheduler.cs
- [[ArgumentOutOfRangeException]] - code
- [[MaxCatchUpSteps]] - code - src/Devices.Common/StepScheduler.cs
- [[ModelTime]] - code - src/Devices.Common/StepScheduler.cs
- [[NoTime]] - code - src/Devices.Common/StepScheduler.cs
- [[Restarted]] - code - src/Devices.Common/StepScheduler.cs
- [[RunId]] - code - src/Devices.Common/StepScheduler.cs
- [[Started]] - code - src/Devices.Common/StepScheduler.cs
- [[Step]] - code - src/Devices.Common/StepScheduler.cs
- [[StepScheduler]] - code - src/Devices.Common/StepScheduler.cs
- [[StepScheduler.Update()]] - code - src/Devices.Common/StepScheduler.cs
- [[StepScheduler.cs]] - code - src/Devices.Common/StepScheduler.cs
- [[StepSchedulerTests]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.ClockSlightlyBehindModel_WaitsInsteadOfGoingBack()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.FirstUpdate_StartsAtClockTime_WithoutSteps()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.InvalidStep_IsRejected()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.NewRun_RestartsAtNewTime_EvenIfEarlier()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.OnlyFullStepsAreDue_ModelLagsAtMostOneStep()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.SameTimeline_GivesSameSteps_RegardlessOfPollingRate()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepSchedulerTests.TooManyDueSteps_AreLimited_AndReported()]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[StepUpdate]] - code - src/Devices.Common/StepScheduler.cs
- [[StepUpdateKind]] - code - src/Devices.Common/StepScheduler.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/StepScheduler
SORT file.name ASC
```

## Connections to other communities
- 8 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 5 edges to [[_COMMUNITY_WeatherFeed]]
- 2 edges to [[_COMMUNITY_Gruppe 51]]
- 2 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 1 edge to [[_COMMUNITY_Gruppe 44]]
- 1 edge to [[_COMMUNITY_Gruppe 41]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_CsvLogger]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_Gruppe 57]]

## Top bridge nodes
- [[StepScheduler]] - degree 19, connects to 6 communities
- [[StepScheduler.Update()]] - degree 13, connects to 3 communities
- [[StepSchedulerTests]] - degree 10, connects to 3 communities
- [[ArgumentOutOfRangeException]] - degree 3, connects to 2 communities
- [[StepScheduler.cs]] - degree 4, connects to 1 community