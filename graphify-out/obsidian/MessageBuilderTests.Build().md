---
source_file: "tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs"
type: "code"
community: "MessageBuilder-Tests"
location: "L48"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/MessageBuilder-Tests
---

# MessageBuilderTests.Build()

## Connections
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[DeviceTracker]] - `references` [EXTRACTED]
- [[MeasurementMessage]] - `references` [EXTRACTED]
- [[MessageBuilder.Build()]] - `calls` [EXTRACTED]
- [[MessageBuilderTests]] - `method` [EXTRACTED]
- [[MessageBuilderTests.Build_FillsEnvelopeFromHeader_AndKeepsVirtualAndRealTimeApart()]] - `calls` [EXTRACTED]
- [[MessageBuilderTests.Build_MeasurementsComeFromRegisterPlan_WithKeyUnitAndText()]] - `calls` [EXTRACTED]
- [[MessageBuilderTests.Json_IsCamelCase_KeepsUmlautsAndUnits_AndOmitsMissingText()]] - `calls` [EXTRACTED]
- [[MessageBuilderTests.NonFiniteRegister_BecomesNull_AndQualityUncertain()]] - `calls` [EXTRACTED]
- [[MessageBuilderTests.Quality_FollowsTimeStatusAndRunningFlag()]] - `calls` [EXTRACTED]
- [[MessageBuilderTests.Quality_NotStale_AtSpeedOne_WhenDeviceStepIsMinutes()]] - `calls` [EXTRACTED]
- [[MessageBuilderTests.Quality_PausedKeepsPublishing_WithSameSimTime()]] - `calls` [EXTRACTED]
- [[MessageBuilderTests.Quality_Stale_WhenRunningButSequenceStandsLongerThanModelLimit()]] - `calls` [EXTRACTED]
- [[MessageBuilderTests.Restart_NewRunWithSameSequence_CountsAsChange()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/MessageBuilder-Tests