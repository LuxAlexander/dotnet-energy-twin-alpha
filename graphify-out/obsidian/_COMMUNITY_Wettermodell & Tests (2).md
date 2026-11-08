---
type: community
cohesion: 0.32
members: 12
---

# Wettermodell & Tests (2)

**Cohesion:** 0.32 - loosely connected
**Members:** 12 nodes

## Members
- [[CoupledWeatherYear]] - code
- [[DateTime]] - code
- [[Manual_1]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.CloudCover()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.GetYear()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.IndexOfHour()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.PrecipitationMmPerHour()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.Rand01()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.StableHash()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.SunHeightDegrees()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[WeatherModel.TemperatureC()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Wettermodell__Tests_2
SORT file.name ASC
```

## Connections to other communities
- 9 edges to [[_COMMUNITY_Wettermodell & Tests (3)]]
- 8 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 4 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 4 edges to [[_COMMUNITY_Gruppe 65]]
- 4 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 4 edges to [[_COMMUNITY_Wettermodell & Tests]]
- 4 edges to [[_COMMUNITY_PvFaultModel]]
- 4 edges to [[_COMMUNITY_Batterie-Simulator Loop (2)]]
- 3 edges to [[_COMMUNITY_RegisterCodec & Befehle (2)]]
- 3 edges to [[_COMMUNITY_Wettermodell & Tests (5)]]
- 3 edges to [[_COMMUNITY_StepScheduler]]
- 3 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 3 edges to [[_COMMUNITY_DualCharger]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 2 edges to [[_COMMUNITY_WeatherFeed]]
- 2 edges to [[_COMMUNITY_PV-Engine & Fehler]]
- 2 edges to [[_COMMUNITY_MasterClock]]
- 2 edges to [[_COMMUNITY_Gruppe 23]]
- 2 edges to [[_COMMUNITY_Gruppe 44]]
- 2 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 2 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 2 edges to [[_COMMUNITY_MessageBuilder-Tests]]
- 1 edge to [[_COMMUNITY_TimeSyncServer]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource (2)]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 19]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (4)]]
- 1 edge to [[_COMMUNITY_StepScheduler (2)]]
- 1 edge to [[_COMMUNITY_SyncedClock]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_Gruppe 21]]
- 1 edge to [[_COMMUNITY_Gruppe 104]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_Gruppe 84]]
- 1 edge to [[_COMMUNITY_Gruppe 88]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_Gruppe 38]]
- 1 edge to [[_COMMUNITY_Fahrzeug]]
- 1 edge to [[_COMMUNITY_Gruppe 109]]
- 1 edge to [[_COMMUNITY_Gruppe 91]]
- 1 edge to [[_COMMUNITY_Nachrichtentypen (Contract)]]
- 1 edge to [[_COMMUNITY_DevicePoller & Gerätetests]]

## Top bridge nodes
- [[DateTime]] - degree 83, connects to 39 communities
- [[WeatherModel]] - degree 19, connects to 5 communities
- [[WeatherModel.GetYear()]] - degree 5, connects to 2 communities
- [[WeatherModel.PrecipitationMmPerHour()]] - degree 5, connects to 2 communities
- [[WeatherModel.TemperatureC()]] - degree 5, connects to 2 communities