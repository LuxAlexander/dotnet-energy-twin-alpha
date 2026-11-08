---
type: community
cohesion: 0.14
members: 18
---

# DualCharger

**Cohesion:** 0.14 - loosely connected
**Members:** 18 nodes

## Members
- [[AmbientTemperatureC_4]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[ChargerModbus.ProcessCommands()]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[DualCharger]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.AllocatePower()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.ConnectWaitingVehicles()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.ProcessQueue()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.SwitchOff()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.SwitchOn()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.Tick()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.cs]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[Id_1]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[IsOn_2]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[Points]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[PowerLimitKW]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[QueueLength]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[RejectedVehicles]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[TotalEnergyKWh_1]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[TotalPowerKW]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/DualCharger
SORT file.name ASC
```

## Connections to other communities
- 3 edges to [[_COMMUNITY_Ladepunkt]]
- 3 edges to [[_COMMUNITY_Fahrzeug]]
- 3 edges to [[_COMMUNITY_Gruppe 65]]
- 3 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 3 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_Funktionsnachweis mit Master]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_Gruppe 71]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop (3)]]
- 1 edge to [[_COMMUNITY_Aufgabe 03 & Nachweise]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle]]
- 1 edge to [[_COMMUNITY_Charger-Namespaces]]

## Top bridge nodes
- [[DualCharger]] - degree 31, connects to 12 communities
- [[DualCharger.Tick()]] - degree 8, connects to 4 communities
- [[ChargerModbus.ProcessCommands()]] - degree 5, connects to 3 communities
- [[DualCharger.ProcessQueue()]] - degree 4, connects to 1 community
- [[DualCharger.ConnectWaitingVehicles()]] - degree 3, connects to 1 community