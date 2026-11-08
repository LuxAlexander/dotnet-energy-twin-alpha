---
source_file: "tests/SimCore.Tests/MasterClockTests.cs"
type: "code"
community: "Master-Zeit Tests"
location: "L111"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Master-Zeit_Tests
---

# MasterClockTests.Restart_SetsNewTime_ForNewRun()

## Connections
- [[Fact]] - `references` [EXTRACTED]
- [[ManualTimeProvider.Advance()_1]] - `calls` [INFERRED]
- [[MasterClock.Restart()]] - `calls` [INFERRED]
- [[MasterClockTests]] - `method` [EXTRACTED]
- [[MasterClockTests.CreateRunning()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Master-Zeit_Tests