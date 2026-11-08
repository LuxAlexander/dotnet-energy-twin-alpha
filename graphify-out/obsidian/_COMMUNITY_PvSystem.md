---
type: community
cohesion: 0.10
members: 21
---

# PvSystem

**Cohesion:** 0.10 - loosely connected
**Members:** 21 nodes

## Members
- [[ActiveFault_1]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[AgingLossPercent]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[AmbientTemperatureC]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[CapacityPercent]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[DailyEnergyKwh]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[EfficiencyPercent]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[ExternalWeather]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[GridVoltageV]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[HasFault]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[IrradiationWm2]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[IsEnabled]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[IsNight]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[ModuleTemperatureC]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[Name]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[PeakPowerKw]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[PowerKw]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[PowerLimitPercent]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[PvSystem]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[SoilingLossPercent]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[TotalEnergyKwh]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[Weather]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/PvSystem
SORT file.name ASC
```

## Connections to other communities
- 3 edges to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 2 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 2 edges to [[_COMMUNITY_Gruppe 38]]
- 2 edges to [[_COMMUNITY_Gruppe 91]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_Gruppe 102]]
- 1 edge to [[_COMMUNITY_PvFaultModel]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop (2)]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]

## Top bridge nodes
- [[PvSystem]] - degree 35, connects to 10 communities