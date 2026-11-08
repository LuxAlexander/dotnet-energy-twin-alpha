---
type: community
cohesion: 0.11
members: 26
---

# CsvLogger

**Cohesion:** 0.11 - loosely connected
**Members:** 26 nodes

## Members
- [[CsvExporter]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[CsvExporter.DisposeAsync()]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[CsvFileWriter]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvFileWriter.DisposeAsync()]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvFileWriter.Escape()]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvLogger]] - code - src/Devices/BatterySimulator/CsvLogger.cs
- [[CsvLogger_1]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[CsvLogger.CreateAsync()_1]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[CsvLogger.DisposeAsync()]] - code - src/Devices/BatterySimulator/CsvLogger.cs
- [[CsvLogger.DisposeAsync()_1]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[CsvLogger.DisposeAsync()_2]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[CsvLogger.FlushAsync()]] - code - src/Devices/BatterySimulator/CsvLogger.cs
- [[CsvLogger.LogAsync()]] - code - src/Devices/BatterySimulator/CsvLogger.cs
- [[CsvLogger.Number()]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[CultureInfo]] - code
- [[FilePath]] - code - src/Devices/BatterySimulator/CsvLogger.cs
- [[FilePath_1]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[FilePath_3]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[FilePath_4]] - code - src/Shared/Data/CsvFileWriter.cs
- [[IAsyncDisposable]] - code
- [[MessageChannel.DisposeAsync()]] - code - src/SimCore.TimeSync/MessageChannel.cs
- [[Program_1]] - code - src/Devices/KuehlhausSimulator/Program.cs
- [[Program.Main()_1]] - code - src/Devices/KuehlhausSimulator/Program.cs
- [[Simulator.AdvanceAsync()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[StreamWriter]] - code
- [[ValueTask]] - code

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/CsvLogger
SORT file.name ASC
```

## Connections to other communities
- 26 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 4 edges to [[_COMMUNITY_WeatherFeed]]
- 4 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 3 edges to [[_COMMUNITY_Gruppe 44]]
- 3 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 2 edges to [[_COMMUNITY_Gruppe 43]]
- 2 edges to [[_COMMUNITY_Gruppe 45]]
- 2 edges to [[_COMMUNITY_Gruppe 73]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource]]
- 1 edge to [[_COMMUNITY_StepScheduler]]
- 1 edge to [[_COMMUNITY_Batterie-Modell]]
- 1 edge to [[_COMMUNITY_Gruppe 81]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_Gruppe 70]]
- 1 edge to [[_COMMUNITY_Gruppe 40]]
- 1 edge to [[_COMMUNITY_Gruppe 66]]
- 1 edge to [[_COMMUNITY_Charger-Namespaces]]
- 1 edge to [[_COMMUNITY_Gruppe 41]]
- 1 edge to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_Register-Dekodierung]]
- 1 edge to [[_COMMUNITY_Zeitsync-Client & Konsole]]

## Top bridge nodes
- [[CultureInfo]] - degree 17, connects to 12 communities
- [[Simulator.AdvanceAsync()]] - degree 12, connects to 7 communities
- [[CsvLogger.LogAsync()]] - degree 7, connects to 4 communities
- [[CsvLogger]] - degree 11, connects to 3 communities
- [[CsvExporter]] - degree 9, connects to 3 communities