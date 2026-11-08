---
source_file: "src/SimCore.Time/ClockOffsetEstimator.cs"
type: "code"
community: "Gruppe 54"
location: "L27"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Gruppe_54
---

# ClockOffsetEstimator.AddSample()

## Connections
- [[ClockOffsetEstimator]] - `method` [EXTRACTED]
- [[ClockOffsetEstimatorTests.ImpossibleSample_IsRejected()]] - `calls` [INFERRED]
- [[ClockOffsetEstimatorTests.OldSamples_LeaveTheWindow()]] - `calls` [INFERRED]
- [[ClockOffsetEstimatorTests.UsesSampleWithSmallestRoundTrip()]] - `calls` [INFERRED]
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[SyncedClock.AddSyncSample()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Gruppe_54