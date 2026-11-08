---
source_file: "tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs"
type: "code"
community: "MessageBuilder-Tests"
location: "L14"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/MessageBuilder-Tests
---

# MessageBuilderTests.ChargerRegisters()

## Connections
- [[DateTime]] - `references` [EXTRACTED]
- [[DeviceType_2]] - `references` [EXTRACTED]
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
- [[MessageBuilderTests.SetFloat()]] - `calls` [EXTRACTED]
- [[MessageBuilderTests.WrongDevice_IsDecodeError_AndNoMessage()]] - `calls` [EXTRACTED]
- [[TimeStatus_1]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/MessageBuilder-Tests