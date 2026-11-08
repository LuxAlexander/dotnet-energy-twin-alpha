---
type: community
cohesion: 0.10
members: 21
---

# PvSystem

**Cohesion:** 0.10 - loosely connected
**Members:** 21 nodes

## Members
- [[ActiveFault_3]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[AgingLossPercent_1]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[AmbientTemperatureC_4]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[CapacityPercent]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[DailyEnergyKwh_1]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[EfficiencyPercent]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[ExternalWeather]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[GridVoltageV_1]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[HasFault_1]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[IrradiationWm2]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[IsEnabled_1]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[IsNight]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[ModuleTemperatureC_1]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[Name_1]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[PeakPowerKw]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[PowerKw_2]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[PowerLimitPercent]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[PvSystem]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[SoilingLossPercent_1]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[TotalEnergyKwh_2]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[Weather_3]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/PvSystem
SORT file.name ASC
```

## Connections to other communities
- 3 edges to [[_COMMUNITY_WeatherFeed]]
- 3 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_Gruppe 67]]
- 2 edges to [[_COMMUNITY_Gruppe 47]]
- 1 edge to [[_COMMUNITY_Gruppe 74]]
- 1 edge to [[_COMMUNITY_PvFaultModel]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]

## Top bridge nodes
- [[PvSystem]] - degree 35, connects to 9 communities