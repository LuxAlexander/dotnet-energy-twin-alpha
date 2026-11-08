---
source_file: "tests/SimCore.Tests/SyncedClockTests.cs"
type: "code"
community: "MasterClock"
location: "L209"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/MasterClock
---

# SyncedClockTests.AdvanceBoth()

## Connections
- [[ManualTimeProvider_1]] - `references` [EXTRACTED]
- [[ManualTimeProvider.Advance()_1]] - `calls` [INFERRED]
- [[SyncedClockTests]] - `method` [EXTRACTED]
- [[SyncedClockTests.AsymmetricDelay_ErrorStaysWithinReportedUncertainty()]] - `calls` [EXTRACTED]
- [[SyncedClockTests.OffsetEstimation_CorrectsDifferentComputerClocks()]] - `calls` [EXTRACTED]
- [[TimeSpan]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/MasterClock