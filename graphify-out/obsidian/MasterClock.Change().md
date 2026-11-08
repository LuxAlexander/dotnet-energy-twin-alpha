---
source_file: "src/SimCore.Time/MasterClock.cs"
type: "code"
community: "TimeState & TimeLine"
location: "L118"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/TimeState__TimeLine
---

# MasterClock.Change()

## Connections
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[Func]] - `references` [EXTRACTED]
- [[MasterClock]] - `method` [EXTRACTED]
- [[MasterClock.Pause()]] - `calls` [EXTRACTED]
- [[MasterClock.Restart()]] - `calls` [EXTRACTED]
- [[MasterClock.Resume()]] - `calls` [EXTRACTED]
- [[MasterClock.SetSpeed()]] - `calls` [EXTRACTED]
- [[TimeLine.Apply()]] - `calls` [INFERRED]
- [[TimeLine.Prune()]] - `calls` [INFERRED]
- [[TimeState]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/TimeState__TimeLine