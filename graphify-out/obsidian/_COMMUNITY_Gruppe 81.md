---
type: community
cohesion: 0.40
members: 6
---

# Gruppe 81

**Cohesion:** 0.40 - moderately connected
**Members:** 6 nodes

## Members
- [[Batterie.ResetFault()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.SetPower()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.TurnOff()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.TurnOn()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Simulator.HandleKey()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[Simulator.SetSetpoint()]] - code - src/Devices/BatterySimulator/Simulator.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Gruppe_81
SORT file.name ASC
```

## Connections to other communities
- 5 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 5 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 4 edges to [[_COMMUNITY_Batterie-Modell]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource]]
- 2 edges to [[_COMMUNITY_Gruppe 44]]
- 1 edge to [[_COMMUNITY_CsvLogger]]
- 1 edge to [[_COMMUNITY_Gruppe 43]]
- 1 edge to [[_COMMUNITY_Gruppe 88]]

## Top bridge nodes
- [[Batterie.SetPower()]] - degree 8, connects to 5 communities
- [[Simulator.HandleKey()]] - degree 13, connects to 4 communities
- [[Batterie.ResetFault()]] - degree 3, connects to 2 communities
- [[Batterie.TurnOff()]] - degree 3, connects to 2 communities
- [[Batterie.TurnOn()]] - degree 3, connects to 2 communities