---
type: community
cohesion: 0.25
members: 19
---

# TimeSyncOptions Tests

**Cohesion:** 0.25 - loosely connected
**Members:** 19 nodes

## Members
- [[ArgsParserTests]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ArgsParserTests.Battery_InvalidConfig_Throws()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ArgsParserTests.Battery_StandaloneOnlyOptions_RequireStandalone()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ArgsParserTests.Pv_InvalidValues_Throw()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ArgsParserTests.Pv_StandaloneOptions_RequireStandalone()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[ArgumentException]] - code
- [[ClientOptionsTests.Invalid_Throws()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[InlineData]] - code
- [[InvalidDataException]] - code
- [[KuehlhausArgsTests.InvalidSetpoint_IsRejected()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[SyncProtocolTests.InvalidMessage_IsRejected()]] - code - tests/SimCore.Tests/SyncProtocolTests.cs
- [[Theory]] - code
- [[TimeSyncOptionsTests]] - code - tests/Devices.Common.Tests/TimeSyncOptionsTests.cs
- [[TimeSyncOptionsTests.Apply()]] - code - tests/Devices.Common.Tests/TimeSyncOptionsTests.cs
- [[TimeSyncOptionsTests.Defaults_UseLocalMaster()]] - code - tests/Devices.Common.Tests/TimeSyncOptionsTests.cs
- [[TimeSyncOptionsTests.InvalidStandaloneValues_AreRejected()]] - code - tests/Devices.Common.Tests/TimeSyncOptionsTests.cs
- [[TimeSyncOptionsTests.Master_InvalidEndpoint_IsRejected()]] - code - tests/Devices.Common.Tests/TimeSyncOptionsTests.cs
- [[TimeSyncOptionsTests.Master_ParsesHostAndPort()]] - code - tests/Devices.Common.Tests/TimeSyncOptionsTests.cs
- [[TimeSyncOptionsTests.Standalone_TakesStartAndSpeed_WithCommaOrDot()]] - code - tests/Devices.Common.Tests/TimeSyncOptionsTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/TimeSyncOptions_Tests
SORT file.name ASC
```

## Connections to other communities
- 4 edges to [[_COMMUNITY_Gruppe 55]]
- 4 edges to [[_COMMUNITY_Gruppe 45]]
- 4 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 4 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 3 edges to [[_COMMUNITY_Gruppe 75]]
- 3 edges to [[_COMMUNITY_ArgsParser]]
- 3 edges to [[_COMMUNITY_Gruppe 42]]
- 3 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 3 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 2 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]

## Top bridge nodes
- [[Theory]] - degree 16, connects to 5 communities
- [[InlineData]] - degree 15, connects to 4 communities
- [[ArgumentException]] - degree 12, connects to 3 communities
- [[ArgsParserTests]] - degree 7, connects to 3 communities
- [[TimeSyncOptionsTests]] - degree 8, connects to 2 communities