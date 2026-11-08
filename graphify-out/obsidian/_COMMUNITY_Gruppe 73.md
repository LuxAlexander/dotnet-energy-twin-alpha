---
type: community
cohesion: 0.25
members: 8
---

# Gruppe 73

**Cohesion:** 0.25 - loosely connected
**Members:** 8 nodes

## Members
- [[CsvFileWriter.cs]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvFormat]] - code - src/Shared/Data/CsvFormat.cs
- [[CsvFormat.Num()]] - code - src/Shared/Data/CsvFormat.cs
- [[CsvFormat.cs]] - code - src/Shared/Data/CsvFormat.cs
- [[CsvSchema]] - code - src/Shared/Data/CsvSchema.cs
- [[CsvSchema.cs]] - code - src/Shared/Data/CsvSchema.cs
- [[KuehlhausSimulatorIOCsvLogger.cs]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[Shared.Data]] - code - src/Shared/Data/CsvFileWriter.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Gruppe_73
SORT file.name ASC
```

## Connections to other communities
- 2 edges to [[_COMMUNITY_Charger-Namespaces]]
- 2 edges to [[_COMMUNITY_CsvLogger]]
- 1 edge to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_Gruppe 40]]

## Top bridge nodes
- [[KuehlhausSimulatorIOCsvLogger.cs]] - degree 4, connects to 3 communities
- [[CsvFileWriter.cs]] - degree 3, connects to 2 communities
- [[Shared.Data]] - degree 5, connects to 1 community
- [[CsvFormat.cs]] - degree 3, connects to 1 community
- [[CsvFormat]] - degree 3, connects to 1 community