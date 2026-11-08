---
type: community
cohesion: 0.13
members: 20
---

# PvFaultModel

**Cohesion:** 0.13 - loosely connected
**Members:** 20 nodes

## Members
- [[ActiveFault_2]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[AgingLossPercent]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[AmbientTemperatureC_3]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[GridDisturbanceForced]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[GridVoltageV]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[HasFault]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[InverterFault]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[ModuleTemperatureC]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[OverheatForced]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvFaultModel]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvFaultModel.AmbientTemperature()]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvFaultModel.DailyOffsetC()]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvFaultModel.HouseholdLoadFactor()]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvFaultModel.ResetFaults()]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvFaultModel.Step()]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvFaultModel.StepDegradation()]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvFaultModel.StepGridVoltage()]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[PvFaultModel.StepTemperature()]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[SoilingLossPercent]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs
- [[TotalLossPercent]] - code - src/Devices/PvSimulator/Devices/PvFaultModel.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/PvFaultModel
SORT file.name ASC
```

## Connections to other communities
- 5 edges to [[_COMMUNITY_PV-Engine & Fehler]]
- 4 edges to [[_COMMUNITY_WeatherFeed]]
- 4 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 4 edges to [[_COMMUNITY_Gruppe 49]]
- 1 edge to [[_COMMUNITY_Gruppe 74]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_Gruppe 43]]
- 1 edge to [[_COMMUNITY_PvSystem]]

## Top bridge nodes
- [[PvFaultModel]] - degree 31, connects to 7 communities
- [[PvFaultModel.Step()]] - degree 7, connects to 3 communities
- [[PvFaultModel.StepTemperature()]] - degree 6, connects to 2 communities
- [[PvFaultModel.DailyOffsetC()]] - degree 4, connects to 2 communities
- [[PvFaultModel.AmbientTemperature()]] - degree 3, connects to 1 community