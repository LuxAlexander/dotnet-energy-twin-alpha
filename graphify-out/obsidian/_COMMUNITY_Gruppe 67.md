---
type: community
cohesion: 0.20
members: 10
---

# Gruppe 67

**Cohesion:** 0.20 - loosely connected
**Members:** 10 nodes

## Members
- [[DailyEnergyKwh]] - code - src/Devices/PvSimulator/Devices/IDevice.cs
- [[IDevice]] - code - src/Devices/PvSimulator/Devices/IDevice.cs
- [[IDevice.ResetDailyEnergy()]] - code - src/Devices/PvSimulator/Devices/IDevice.cs
- [[IDevice.StepContinuous()]] - code - src/Devices/PvSimulator/Devices/IDevice.cs
- [[IDevice.cs]] - code - src/Devices/PvSimulator/Devices/IDevice.cs
- [[IsEnabled]] - code - src/Devices/PvSimulator/Devices/IDevice.cs
- [[Name]] - code - src/Devices/PvSimulator/Devices/IDevice.cs
- [[PowerKw_1]] - code - src/Devices/PvSimulator/Devices/IDevice.cs
- [[PvSystem.ResetDailyEnergy()]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[TotalEnergyKwh_1]] - code - src/Devices/PvSimulator/Devices/IDevice.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Gruppe_67
SORT file.name ASC
```

## Connections to other communities
- 2 edges to [[_COMMUNITY_PvSystem]]
- 2 edges to [[_COMMUNITY_WeatherFeed]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]

## Top bridge nodes
- [[IDevice.StepContinuous()]] - degree 3, connects to 2 communities
- [[PvSystem.ResetDailyEnergy()]] - degree 3, connects to 2 communities
- [[IDevice]] - degree 9, connects to 1 community
- [[IDevice.cs]] - degree 2, connects to 1 community