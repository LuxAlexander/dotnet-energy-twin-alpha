---
type: community
cohesion: 0.21
members: 22
---

# MessageBuilder-Tests

**Cohesion:** 0.21 - loosely connected
**Members:** 22 nodes

## Members
- [[DeviceTracker]] - code - src/ModbusRabbitGateway/MessageBuilder.cs
- [[LastChangeAt]] - code - src/ModbusRabbitGateway/MessageBuilder.cs
- [[LastRunId]] - code - src/ModbusRabbitGateway/MessageBuilder.cs
- [[LastSeq]] - code - src/ModbusRabbitGateway/MessageBuilder.cs
- [[LastSimTime]] - code - src/ModbusRabbitGateway/MessageBuilder.cs
- [[MessageBuilderTests]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.Build()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.Build_FillsEnvelopeFromHeader_AndKeepsVirtualAndRealTimeApart()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.Build_MeasurementsComeFromRegisterPlan_WithKeyUnitAndText()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.ChargerRegisters()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.Json_IsCamelCase_KeepsUmlautsAndUnits_AndOmitsMissingText()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.NonFiniteRegister_BecomesNull_AndQualityUncertain()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.Quality_FollowsTimeStatusAndRunningFlag()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.Quality_NotStale_AtSpeedOne_WhenDeviceStepIsMinutes()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.Quality_PausedKeepsPublishing_WithSameSimTime()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.Quality_Stale_WhenRunningButSequenceStandsLongerThanModelLimit()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.Restart_NewRunWithSameSequence_CountsAsChange()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.SetFloat()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageBuilderTests.WrongDevice_IsDecodeError_AndNoMessage()]] - code - tests/ModbusRabbitGateway.Tests/MessageBuilderTests.cs
- [[MessageJson.Serialize()]] - code - src/ModbusRabbitGateway/Messages.cs
- [[PublishSeq]] - code - src/ModbusRabbitGateway/MessageBuilder.cs
- [[TimeStatus_1]] - code

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/MessageBuilder-Tests
SORT file.name ASC
```

## Connections to other communities
- 9 edges to [[_COMMUNITY_Gruppe 23]]
- 6 edges to [[_COMMUNITY_MessageBuilder]]
- 3 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 2 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 2 edges to [[_COMMUNITY_Gruppe 48]]
- 2 edges to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 2 edges to [[_COMMUNITY_GatewayService]]
- 2 edges to [[_COMMUNITY_Nachrichtentypen (Contract)]]
- 1 edge to [[_COMMUNITY_DeviceEndpoint]]
- 1 edge to [[_COMMUNITY_Gateway-Publisher]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 1 edge to [[_COMMUNITY_JSON-Serialisierung]]
- 1 edge to [[_COMMUNITY_Messwert-Schlüssel]]

## Top bridge nodes
- [[MessageBuilderTests]] - degree 19, connects to 6 communities
- [[DeviceTracker]] - degree 16, connects to 4 communities
- [[MessageBuilderTests.Build()]] - degree 14, connects to 3 communities
- [[MessageJson.Serialize()]] - degree 6, connects to 3 communities
- [[MessageBuilderTests.ChargerRegisters()]] - degree 15, connects to 2 communities