---
source_file: "tests/Devices.Common.Tests/StepSchedulerTests.cs"
type: "code"
community: "StepScheduler"
location: "L3"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/StepScheduler
---

# StepSchedulerTests

## Connections
- [[DateTime]] - `references` [EXTRACTED]
- [[StepSchedulerTests.ClockSlightlyBehindModel_WaitsInsteadOfGoingBack()]] - `method` [EXTRACTED]
- [[StepSchedulerTests.FirstUpdate_StartsAtClockTime_WithoutSteps()]] - `method` [EXTRACTED]
- [[StepSchedulerTests.InvalidStep_IsRejected()]] - `method` [EXTRACTED]
- [[StepSchedulerTests.NewRun_RestartsAtNewTime_EvenIfEarlier()]] - `method` [EXTRACTED]
- [[StepSchedulerTests.OnlyFullStepsAreDue_ModelLagsAtMostOneStep()]] - `method` [EXTRACTED]
- [[StepSchedulerTests.SameTimeline_GivesSameSteps_RegardlessOfPollingRate()]] - `method` [EXTRACTED]
- [[StepSchedulerTests.TooManyDueSteps_AreLimited_AndReported()]] - `method` [EXTRACTED]
- [[StepSchedulerTests.cs]] - `contains` [EXTRACTED]
- [[TimeSpan]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/StepScheduler