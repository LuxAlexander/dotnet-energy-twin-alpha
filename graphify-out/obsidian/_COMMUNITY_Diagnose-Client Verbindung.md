---
type: community
cohesion: 0.11
members: 37
---

# Diagnose-Client Verbindung

**Cohesion:** 0.11 - loosely connected
**Members:** 37 nodes

## Members
- [[ChargerModbus.Dispose()]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[Connection]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticClientTests]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.ConnectionLoss_IsReportedAsConnectionError_AndReconnectsWhenDeviceIsBack()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Dispose()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Execute_ShowsErrorsAsTextInsteadOfThrowing()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Force_InvalidValue_IsRejectedByDevice_AndExplained()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Force_ReadOnlyAndUnusedAddress_AreRejectedByDevice()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.FreePort()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.OnOffReset_ByLabelAndShortcut_ReachTheDevice()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Quit_EndsTheSession()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Read_ShowsDecodedHeaderAndDeviceRegisters_WithRawValues()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Watch_KeepsGoingDuringOutage()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Write_AmbiguousOrUnknownTarget_ThrowsValueError()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Write_ReadOnlyWithoutForce_IsRefusedByClient()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Write_ValidSetpoint_IsAcceptedAndVisibleInDevice()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticSession]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.ConnectAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.DescribeDevice()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.ExecuteAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.Format()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.ParseValue()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.Prefix()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.PrintAllAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.PrintHeaderAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.PrintInfoAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.PrintRawAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.ReadAll()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.ReadHeader()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.RegisterCodecEquals()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.WatchAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.WriteAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[ModbusDeviceServer.Dispose()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServerTests.Dispose()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[Plan_5]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[RegisterPlan.ToMarkdown()]] - code - src/Modbus.Contracts/RegisterPlan.cs
- [[StringWriter]] - code

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Diagnose-Client_Verbindung
SORT file.name ASC
```

## Connections to other communities
- 19 edges to [[_COMMUNITY_TimeSyncServer]]
- 11 edges to [[_COMMUNITY_Gruppe 23]]
- 8 edges to [[_COMMUNITY_Gruppe 52]]
- 6 edges to [[_COMMUNITY_Schreibprüfung]]
- 5 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 5 edges to [[_COMMUNITY_Registerpläne]]
- 4 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 3 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 3 edges to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 2 edges to [[_COMMUNITY_Gruppe 48]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource]]
- 1 edge to [[_COMMUNITY_Gruppe 82]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]
- 1 edge to [[_COMMUNITY_Gateway-Publisher]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]
- 1 edge to [[_COMMUNITY_Gruppe 19]]
- 1 edge to [[_COMMUNITY_SRS Gateway-Anforderungen]]
- 1 edge to [[_COMMUNITY_Funktionsnachweis mit Master]]
- 1 edge to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle (2)]]

## Top bridge nodes
- [[DiagnosticSession]] - degree 29, connects to 10 communities
- [[DiagnosticClientTests]] - degree 22, connects to 6 communities
- [[DiagnosticSession.WriteAsync()]] - degree 20, connects to 6 communities
- [[ModbusDeviceServer.Dispose()]] - degree 10, connects to 3 communities
- [[DiagnosticSession.WatchAsync()]] - degree 7, connects to 3 communities