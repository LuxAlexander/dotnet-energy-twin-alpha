---
source_file: "src/ModbusRabbitGateway/MessageBuilder.cs"
type: "code"
community: "MessageBuilder-Tests"
location: "L17"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/MessageBuilder-Tests
---

# DeviceTracker

## Connections
- [[--status-repeat Status alle 30 s wiederholen, sinceUtc unverändert]] - `references` [AMBIGUOUS]
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[DevicePoller]] - `references` [EXTRACTED]
- [[LastChangeAt]] - `defines` [EXTRACTED]
- [[LastRunId]] - `defines` [EXTRACTED]
- [[LastSeq]] - `defines` [EXTRACTED]
- [[LastSimTime]] - `defines` [EXTRACTED]
- [[MessageBuilder.Build()]] - `references` [EXTRACTED]
- [[MessageBuilder.cs]] - `contains` [EXTRACTED]
- [[MessageBuilderTests.Build()]] - `references` [EXTRACTED]
- [[MessageBuilderTests.Quality_NotStale_AtSpeedOne_WhenDeviceStepIsMinutes()]] - `calls` [INFERRED]
- [[MessageBuilderTests.Quality_PausedKeepsPublishing_WithSameSimTime()]] - `calls` [INFERRED]
- [[MessageBuilderTests.Quality_Stale_WhenRunningButSequenceStandsLongerThanModelLimit()]] - `calls` [INFERRED]
- [[MessageBuilderTests.Restart_NewRunWithSameSequence_CountsAsChange()]] - `calls` [INFERRED]
- [[MessageBuilderTests.WrongDevice_IsDecodeError_AndNoMessage()]] - `calls` [INFERRED]
- [[PublishSeq]] - `defines` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/MessageBuilder-Tests