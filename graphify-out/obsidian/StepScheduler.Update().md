---
source_file: "src/Devices.Common/StepScheduler.cs"
type: "code"
community: "StepScheduler"
location: "L55"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/StepScheduler
---

# StepScheduler.Update()

## Connections
- [[DateTime]] - `references` [EXTRACTED]
- [[SimulationEngine.AdvanceAsync()]] - `calls` [INFERRED]
- [[SimulationEngine.AdvanceAsync()_1]] - `calls` [INFERRED]
- [[SimulationEngine.AdvanceAsync()_2]] - `calls` [INFERRED]
- [[Simulator.AdvanceAsync()]] - `calls` [INFERRED]
- [[StepScheduler]] - `method` [EXTRACTED]
- [[StepSchedulerTests.ClockSlightlyBehindModel_WaitsInsteadOfGoingBack()]] - `calls` [INFERRED]
- [[StepSchedulerTests.FirstUpdate_StartsAtClockTime_WithoutSteps()]] - `calls` [INFERRED]
- [[StepSchedulerTests.NewRun_RestartsAtNewTime_EvenIfEarlier()]] - `calls` [INFERRED]
- [[StepSchedulerTests.OnlyFullStepsAreDue_ModelLagsAtMostOneStep()]] - `calls` [INFERRED]
- [[StepSchedulerTests.SameTimeline_GivesSameSteps_RegardlessOfPollingRate()]] - `calls` [INFERRED]
- [[StepSchedulerTests.TooManyDueSteps_AreLimited_AndReported()]] - `calls` [INFERRED]
- [[StepUpdate]] - `calls` [EXTRACTED]

#graphify/code #graphify/INFERRED #community/StepScheduler