---
type: community
cohesion: 0.23
members: 18
---

# Register-Dekodierung

**Cohesion:** 0.23 - loosely connected
**Members:** 18 nodes

## Members
- [[CommonRegisters.HeaderAt()]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[DiagnosticSession.Summary()]] - code - src/ModbusDiagnosticClient/DiagnosticSession.cs
- [[IEnumerable]] - code
- [[IReadOnlyList]] - code
- [[ReadOnlySpan]] - code
- [[RegisterCodec.Decode()]] - code - src/Modbus.Contracts/RegisterCodec.cs
- [[RegisterDefinition.LabelFor()]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[RegisterFormatter]] - code - src/ModbusDiagnosticClient/RegisterFormatter.cs
- [[RegisterFormatter.Number()]] - code - src/ModbusDiagnosticClient/RegisterFormatter.cs
- [[RegisterFormatter.Raw()]] - code - src/ModbusDiagnosticClient/RegisterFormatter.cs
- [[RegisterFormatter.Short()]] - code - src/ModbusDiagnosticClient/RegisterFormatter.cs
- [[RegisterFormatter.Table()]] - code - src/ModbusDiagnosticClient/RegisterFormatter.cs
- [[RegisterFormatter.Value()]] - code - src/ModbusDiagnosticClient/RegisterFormatter.cs
- [[RegisterFormatterTests]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[RegisterFormatterTests.Raw_ShowsHexWords()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[RegisterFormatterTests.Value_ModelTimeZero_MeansNoTime()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[RegisterFormatterTests.Value_ModelTime_IsExactToTheSecond()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[RegisterFormatterTests.Value_UsesLabelUnitAndScale()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Register-Dekodierung
SORT file.name ASC
```

## Connections to other communities
- 9 edges to [[_COMMUNITY_Schreibprüfung]]
- 8 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 7 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 4 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 3 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 2 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 2 edges to [[_COMMUNITY_Gruppe 48]]
- 1 edge to [[_COMMUNITY_DualCharger]]
- 1 edge to [[_COMMUNITY_Fahrzeug]]
- 1 edge to [[_COMMUNITY_Registerpläne]]
- 1 edge to [[_COMMUNITY_Gruppe 42]]
- 1 edge to [[_COMMUNITY_Charger-Namespaces]]
- 1 edge to [[_COMMUNITY_CsvLogger]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_TimeSyncServer]]
- 1 edge to [[_COMMUNITY_MasterClock]]
- 1 edge to [[_COMMUNITY_TimeState & TimeLine]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]

## Top bridge nodes
- [[IReadOnlyList]] - degree 19, connects to 14 communities
- [[RegisterFormatter.Value()]] - degree 12, connects to 3 communities
- [[CommonRegisters.HeaderAt()]] - degree 7, connects to 3 communities
- [[RegisterCodec.Decode()]] - degree 11, connects to 2 communities
- [[RegisterFormatter.Short()]] - degree 9, connects to 2 communities