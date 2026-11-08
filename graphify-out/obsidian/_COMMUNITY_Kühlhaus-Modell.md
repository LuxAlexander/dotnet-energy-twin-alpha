---
type: community
cohesion: 0.11
members: 22
---

# Kühlhaus-Modell

**Cohesion:** 0.11 - loosely connected
**Members:** 22 nodes

## Members
- [[ActiveFault_1]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[AlarmTempC]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[CompressorOn]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[CoolingPowerKW]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[DoorOpen]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[ElectricPowerKW]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[EnergyKWh_1]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[Id_1]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[IsOn_2]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[Kuehlhaus]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[Kuehlhaus.GetCop()]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[Kuehlhaus.TriggerFault()]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[Kuehlhaus.TurnOn()]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[Kuehlhaus.Update()_1]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[Kuehlhaus.UpdateCompressor()]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[Kuehlhaus.UpdateState()]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[OutsideTempC]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[ResetAlarmBelowC]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[RoomTempC]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[RuntimeHours]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[SetpointC]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[State_2]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Kühlhaus-Modell
SORT file.name ASC
```

## Connections to other communities
- 8 edges to [[_COMMUNITY_Gruppe 57]]
- 4 edges to [[_COMMUNITY_Gruppe 50]]
- 4 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 3 edges to [[_COMMUNITY_Gruppe 58]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_Gruppe 59]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]
- 1 edge to [[_COMMUNITY_Gruppe 51]]

## Top bridge nodes
- [[Kuehlhaus]] - degree 36, connects to 7 communities
- [[Kuehlhaus.TriggerFault()]] - degree 5, connects to 3 communities
- [[Kuehlhaus.TurnOn()]] - degree 4, connects to 2 communities
- [[Kuehlhaus.Update()_1]] - degree 5, connects to 1 community
- [[Kuehlhaus.UpdateState()]] - degree 5, connects to 1 community