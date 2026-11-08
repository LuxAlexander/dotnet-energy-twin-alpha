---
type: community
cohesion: 0.10
members: 43
---

# Diagnose-Client Verbindung

**Cohesion:** 0.10 - loosely connected
**Members:** 43 nodes

## Members
- [[ChargerModbus.Dispose()]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[Connection]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[Connection_1]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DeviceConnection.ReadRegisters()]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[DiagnosticClientTests]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.ConnectionLoss_IsReportedAsConnectionError_AndReconnectsWhenDeviceIsBack()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Execute_ShowsErrorsAsTextInsteadOfThrowing()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Force_InvalidValue_IsRejectedByDevice_AndExplained()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Force_ReadOnlyAndUnusedAddress_AreRejectedByDevice()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.FreePort()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.OnOffReset_ByLabelAndShortcut_ReachTheDevice()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Quit_EndsTheSession()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Read_ShowsDecodedHeaderAndDeviceRegisters_WithRawValues()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Watch_KeepsGoingDuringOutage()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Write_AmbiguousOrUnknownTarget_ThrowsValueError()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Write_InvalidValue_IsRefusedByClientBeforeSending()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Write_ReadOnlyWithoutForce_IsRefusedByClient()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticClientTests.Write_ValidSetpoint_IsAcceptedAndVisibleInDevice()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[DiagnosticErrorKind]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[DiagnosticException]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[DiagnosticSession]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.ConnectAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.DescribeDevice()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.ExecuteAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.ParseValue()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.Prefix()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.PrintAllAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.PrintHeaderAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.PrintInfoAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.PrintRawAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.ReadAll()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.ReadHeader()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.ResolveTarget()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.WatchAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[DiagnosticSession.WriteAsync()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[Kind]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[ModbusDeviceServer.Dispose()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServerTests.Dispose()]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[Plan_5]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[Protocol]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[StringWriter]] - code
- [[TextWriter]] - code
- [[Value]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Diagnose-Client_Verbindung
SORT file.name ASC
```

## Connections to other communities
- 24 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 15 edges to [[_COMMUNITY_DeviceConnection]]
- 11 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 9 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 8 edges to [[_COMMUNITY_Register-Dekodierung]]
- 7 edges to [[_COMMUNITY_Schreibprüfung]]
- 5 edges to [[_COMMUNITY_Registerpläne]]
- 4 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 3 edges to [[_COMMUNITY_Modbus-Tests]]
- 2 edges to [[_COMMUNITY_TimeSyncOptions Tests]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource]]
- 1 edge to [[_COMMUNITY_Charger-Namespaces]]
- 1 edge to [[_COMMUNITY_Gruppe 48]]
- 1 edge to [[_COMMUNITY_CsvLogger]]
- 1 edge to [[_COMMUNITY_Gruppe 51]]
- 1 edge to [[_COMMUNITY_WeatherFeed]]

## Top bridge nodes
- [[DiagnosticSession]] - degree 27, connects to 7 communities
- [[DiagnosticSession.WriteAsync()]] - degree 23, connects to 5 communities
- [[DiagnosticClientTests]] - degree 23, connects to 5 communities
- [[DiagnosticClientTests.ConnectionLoss_IsReportedAsConnectionError_AndReconnectsWhenDeviceIsBack()]] - degree 8, connects to 3 communities
- [[DiagnosticClientTests.Write_InvalidValue_IsRefusedByClientBeforeSending()]] - degree 7, connects to 3 communities