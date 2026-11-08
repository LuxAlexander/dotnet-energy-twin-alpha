---
source_file: "tests/SimCore.Tests/MasterClockTests.cs"
type: "code"
community: "Master-Zeit Tests"
location: "L175"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Master-Zeit_Tests
---

# MasterClockTests.EveryChange_IncreasesSequence_AndRaisesEvent()

## Connections
- [[Fact]] - `references` [EXTRACTED]
- [[MasterClock.Pause()]] - `calls` [INFERRED]
- [[MasterClock.Restart()]] - `calls` [INFERRED]
- [[MasterClock.Resume()]] - `calls` [INFERRED]
- [[MasterClock.SetSpeed()]] - `calls` [INFERRED]
- [[MasterClockTests]] - `method` [EXTRACTED]
- [[MasterClockTests.CreateRunning()]] - `calls` [EXTRACTED]

#graphify/code #graphify/INFERRED #community/Master-Zeit_Tests