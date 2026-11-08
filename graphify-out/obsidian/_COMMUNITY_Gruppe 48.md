---
type: community
cohesion: 0.28
members: 17
---

# Gruppe 48

**Cohesion:** 0.28 - loosely connected
**Members:** 17 nodes

## Members
- [[ArgsParserTests]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ArgsParserTests.Battery_InvalidConfig_Throws()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ArgsParserTests.Battery_StandaloneOnlyOptions_RequireStandalone()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ArgsParserTests.Pv_InvalidValues_Throw()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ArgsParserTests.Pv_StandaloneOptions_RequireStandalone()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ArgumentException]] - code
- [[ClientOptionsTests.Invalid_Throws()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Write_InvalidValue_IsRefusedByClientBeforeSending()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[GatewayOptionsTests.InvalidValues_AreRejectedWithGermanMessage()]] - code - tests/ModbusRabbitGateway.Tests/GatewayOptionsTests.cs
- [[InlineData]] - code
- [[InvalidDataException]] - code
- [[KuehlhausArgsTests.InvalidSetpoint_IsRejected()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[MeasurementKeysTests.From_MakesAsciiSnakeCase()]] - code - tests/ModbusRabbitGateway.Tests/MeasurementKeysTests.cs
- [[SyncProtocolTests.InvalidMessage_IsRejected()]] - code - tests/SimCore.Tests/SyncProtocolTests.cs
- [[Theory]] - code
- [[TimeSyncOptionsTests.InvalidStandaloneValues_AreRejected()]] - code - tests/Devices.Common.Tests/TimeSyncOptionsTests.cs
- [[TimeSyncOptionsTests.Master_InvalidEndpoint_IsRejected()]] - code - tests/Devices.Common.Tests/TimeSyncOptionsTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Gruppe_48
SORT file.name ASC
```

## Connections to other communities
- 6 edges to [[_COMMUNITY_Gruppe 44]]
- 5 edges to [[_COMMUNITY_Gateway-Optionen]]
- 4 edges to [[_COMMUNITY_Gruppe 61]]
- 3 edges to [[_COMMUNITY_ArgsParser (2)]]
- 3 edges to [[_COMMUNITY_Gruppe 105]]
- 3 edges to [[_COMMUNITY_Gruppe 57]]
- 3 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 3 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 2 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 2 edges to [[_COMMUNITY_Gruppe 23]]
- 2 edges to [[_COMMUNITY_MessageBuilder-Tests]]
- 2 edges to [[_COMMUNITY_Messwert-Schlüssel]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle (2)]]
- 1 edge to [[_COMMUNITY_TimeSyncServer]]

## Top bridge nodes
- [[Theory]] - degree 19, connects to 7 communities
- [[InlineData]] - degree 18, connects to 6 communities
- [[ArgumentException]] - degree 16, connects to 4 communities
- [[ArgsParserTests]] - degree 7, connects to 3 communities
- [[DiagnosticClientTests.Write_InvalidValue_IsRefusedByClientBeforeSending()]] - degree 6, connects to 3 communities