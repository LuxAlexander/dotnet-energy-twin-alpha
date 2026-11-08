---
type: community
cohesion: 0.15
members: 19
---

# Gruppe 38

**Cohesion:** 0.15 - loosely connected
**Members:** 19 nodes

## Members
- [[IrradiationCalculator]] - code - src/Shared/Weather/IrradiationCalculator.cs
- [[IrradiationCalculator.Calculate()]] - code - src/Shared/Weather/IrradiationCalculator.cs
- [[IrradiationCalculator.GetWeatherFactor()]] - code - src/Shared/Weather/IrradiationCalculator.cs
- [[IrradiationCalculator.IsDaylight()]] - code - src/Shared/Weather/IrradiationCalculator.cs
- [[IrradiationCalculator.cs]] - code - src/Shared/Weather/IrradiationCalculator.cs
- [[Night_1]] - code - src/Shared/Weather/WeatherCondition.cs
- [[Overcast_1]] - code - src/Shared/Weather/WeatherCondition.cs
- [[PartlyCloudy_1]] - code - src/Shared/Weather/WeatherCondition.cs
- [[PvSystem.SetWeather()]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[SimulationEngine.RandomWeather()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.UpdateAutoWeather()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[Snow_1]] - code - src/Shared/Weather/WeatherCondition.cs
- [[Storm_1]] - code - src/Shared/Weather/WeatherCondition.cs
- [[Sunny_1]] - code - src/Shared/Weather/WeatherCondition.cs
- [[WeatherCondition_1]] - code - src/Shared/Weather/WeatherCondition.cs
- [[WeatherCondition.cs]] - code - src/Shared/Weather/WeatherCondition.cs
- [[WeatherConditionExtensions]] - code - src/Shared/Weather/WeatherCondition.cs
- [[WeatherConditionExtensions.IsDayWeather()]] - code - src/Shared/Weather/WeatherCondition.cs
- [[WeatherConditionExtensions.ToGerman()]] - code - src/Shared/Weather/WeatherCondition.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Gruppe_38
SORT file.name ASC
```

## Connections to other communities
- 5 edges to [[_COMMUNITY_PV-Engine & Fehler]]
- 4 edges to [[_COMMUNITY_Gruppe 67]]
- 2 edges to [[_COMMUNITY_PvSystem]]
- 2 edges to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]

## Top bridge nodes
- [[SimulationEngine.UpdateAutoWeather()]] - degree 9, connects to 5 communities
- [[WeatherCondition_1]] - degree 15, connects to 2 communities
- [[IrradiationCalculator.Calculate()]] - degree 6, connects to 2 communities
- [[PvSystem.SetWeather()]] - degree 4, connects to 2 communities
- [[SimulationEngine.RandomWeather()]] - degree 4, connects to 2 communities