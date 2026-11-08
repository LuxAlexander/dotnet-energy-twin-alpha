---
type: community
cohesion: 0.13
members: 27
---

# Batterie-Simulator Loop (2)

**Cohesion:** 0.13 - loosely connected
**Members:** 27 nodes

## Members
- [[CsvExporter]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[CsvExporter.CreateAsync()]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[CsvExporter.DisposeAsync()]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[CsvExporter.FlushAsync()]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[CsvExporter.WriteRowAsync()]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[CsvFileWriter]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvFileWriter.CreateAsync()]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvFileWriter.DisposeAsync()]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvFileWriter.Escape()]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvFileWriter.FlushAsync()]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvFileWriter.WriteRowAsync()]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvLogger_2]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[CsvLogger.CreateAsync()_2]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[CsvLogger.DisposeAsync()]] - code - src/Devices/BatterySimulator/CsvLogger.cs
- [[CsvLogger.DisposeAsync()_2]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[CsvLogger.FlushAsync()_2]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[CsvLogger.Number()_1]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[CsvLogger.WriteAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[FilePath_4]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[FilePath_2]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[FilePath_3]] - code - src/Shared/Data/CsvFileWriter.cs
- [[IAsyncDisposable]] - code
- [[MessageChannel.DisposeAsync()]] - code - src/SimCore.TimeSync/MessageChannel.cs
- [[SimulationEngine.AdvanceAsync()_2]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.StartRunAsync()]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.UpdateWeatherAsync()_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[ValueTask]] - code

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Batterie-Simulator_Loop_2
SORT file.name ASC
```

## Connections to other communities
- 22 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 12 edges to [[_COMMUNITY_TimeSyncServer]]
- 7 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 4 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 2 edges to [[_COMMUNITY_Gruppe 101]]
- 2 edges to [[_COMMUNITY_Sync-Nachrichten (2)]]
- 2 edges to [[_COMMUNITY_Gruppe 72]]
- 2 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_Kühlhaus-Modell]]
- 1 edge to [[_COMMUNITY_PvSystem]]
- 1 edge to [[_COMMUNITY_Gruppe 67]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_StepScheduler]]
- 1 edge to [[_COMMUNITY_Gruppe 109]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]

## Top bridge nodes
- [[SimulationEngine.AdvanceAsync()_2]] - degree 12, connects to 6 communities
- [[CsvExporter.WriteRowAsync()]] - degree 9, connects to 6 communities
- [[SimulationEngine.StartRunAsync()]] - degree 10, connects to 5 communities
- [[CsvLogger.WriteAsync()_1]] - degree 8, connects to 4 communities
- [[SimulationEngine.UpdateWeatherAsync()_1]] - degree 8, connects to 4 communities