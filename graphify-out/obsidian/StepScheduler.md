---
source_file: "src/Devices.Common/StepScheduler.cs"
type: "code"
community: "StepScheduler"
location: "L30"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/StepScheduler
---

# StepScheduler

## Connections
- [[DateTime]] - `references` [EXTRACTED]
- [[MaxCatchUpSteps]] - `defines` [EXTRACTED]
- [[ModelTime]] - `defines` [EXTRACTED]
- [[RunId_2]] - `defines` [EXTRACTED]
- [[SimulationEngine]] - `references` [EXTRACTED]
- [[SimulationEngine_1]] - `references` [EXTRACTED]
- [[SimulationEngine_2]] - `references` [EXTRACTED]
- [[Simulator]] - `references` [EXTRACTED]
- [[Step]] - `defines` [EXTRACTED]
- [[StepScheduler.Update()]] - `method` [EXTRACTED]
- [[StepScheduler.cs]] - `contains` [EXTRACTED]
- [[StepSchedulerTests.ClockSlightlyBehindModel_WaitsInsteadOfGoingBack()]] - `calls` [INFERRED]
- [[StepSchedulerTests.FirstUpdate_StartsAtClockTime_WithoutSteps()]] - `calls` [INFERRED]
- [[StepSchedulerTests.InvalidStep_IsRejected()]] - `calls` [INFERRED]
- [[StepSchedulerTests.NewRun_RestartsAtNewTime_EvenIfEarlier()]] - `calls` [INFERRED]
- [[StepSchedulerTests.OnlyFullStepsAreDue_ModelLagsAtMostOneStep()]] - `calls` [INFERRED]
- [[StepSchedulerTests.SameTimeline_GivesSameSteps_RegardlessOfPollingRate()]] - `calls` [INFERRED]
- [[StepSchedulerTests.TooManyDueSteps_AreLimited_AndReported()]] - `calls` [INFERRED]
- [[TimeSpan]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/StepScheduler