---
type: community
cohesion: 0.17
members: 12
---

# Wettermodell & Tests (4)

**Cohesion:** 0.17 - loosely connected
**Members:** 12 nodes

## Members
- [[ManualWeather.ApplyPreset()]] - code - src/WeatherSimulator/Simulation/ManualWeather.cs
- [[Night]] - code - src/SimCore.Contracts/WeatherData.cs
- [[Overcast]] - code - src/SimCore.Contracts/WeatherData.cs
- [[PartlyCloudy]] - code - src/SimCore.Contracts/WeatherData.cs
- [[SimulationEngine.German()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[Snow]] - code - src/SimCore.Contracts/WeatherData.cs
- [[Storm]] - code - src/SimCore.Contracts/WeatherData.cs
- [[Sunny]] - code - src/SimCore.Contracts/WeatherData.cs
- [[WeatherCondition]] - code - src/SimCore.Contracts/WeatherData.cs
- [[WeatherData.cs]] - code - src/SimCore.Contracts/WeatherData.cs
- [[WeatherFeed.Describe()]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[WeatherModel.ConditionOf()]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Wettermodell__Tests_4
SORT file.name ASC
```

## Connections to other communities
- 4 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_WeatherFeed]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (3)]]
- 1 edge to [[_COMMUNITY_Zeit-Bibliotheken & Master]]

## Top bridge nodes
- [[WeatherCondition]] - degree 13, connects to 2 communities
- [[WeatherFeed.Describe()]] - degree 3, connects to 2 communities
- [[SimulationEngine.German()]] - degree 3, connects to 2 communities
- [[WeatherData.cs]] - degree 3, connects to 2 communities
- [[WeatherModel.ConditionOf()]] - degree 3, connects to 2 communities