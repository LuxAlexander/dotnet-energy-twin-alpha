---
source_file: "tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs"
type: "code"
community: "MessageBuilder-Tests"
location: "L7"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/MessageBuilder-Tests
---

# MessageBuilderTests

## Connections
- [[DateTime]] - `references` [EXTRACTED]
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[DeviceEndpoint]] - `references` [EXTRACTED]
- [[MessageBuilder]] - `references` [EXTRACTED]
- [[MessageBuilderTests.Build()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.Build_FillsEnvelopeFromHeader_AndKeepsVirtualAndRealTimeApart()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.Build_MeasurementsComeFromRegisterPlan_WithKeyUnitAndText()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.ChargerRegisters()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.Json_IsCamelCase_KeepsUmlautsAndUnits_AndOmitsMissingText()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.NonFiniteRegister_BecomesNull_AndQualityUncertain()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.Quality_FollowsTimeStatusAndRunningFlag()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.Quality_NotStale_AtSpeedOne_WhenDeviceStepIsMinutes()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.Quality_PausedKeepsPublishing_WithSameSimTime()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.Quality_Stale_WhenRunningButSequenceStandsLongerThanModelLimit()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.Restart_NewRunWithSameSequence_CountsAsChange()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.SetFloat()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.WrongDevice_IsDecodeError_AndNoMessage()]] - `method` [EXTRACTED]
- [[MessageBuilderTests.cs]] - `contains` [EXTRACTED]
- [[ModbusRabbitGateway.Tests (5252 am 2026-11-08)]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/MessageBuilder-Tests