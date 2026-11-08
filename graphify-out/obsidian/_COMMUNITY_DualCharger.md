---
type: community
cohesion: 0.13
members: 18
---

# DualCharger

**Cohesion:** 0.13 - loosely connected
**Members:** 18 nodes

## Members
- [[AmbientTemperatureC_2]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[ChargerModbus.ProcessCommands()]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[DualCharger]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.ConnectWaitingVehicles()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.Enqueue()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.ProcessQueue()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.SwitchOff()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.SwitchOn()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.cs]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[Id]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[IsOn_1]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[Points]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[PowerLimitKW]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[QueueLength]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[RejectedVehicles]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[SimulationEngine.VehicleArrives()]] - code - src/Devices/DualPortCharger/Simulation/SimulationEngine.cs
- [[TotalEnergyKWh]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[TotalPowerKW]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/DualCharger
SORT file.name ASC
```

## Connections to other communities
- 6 edges to [[_COMMUNITY_Gruppe 51]]
- 4 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 3 edges to [[_COMMUNITY_WeatherFeed]]
- 3 edges to [[_COMMUNITY_Fahrzeug]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_Gruppe 41]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle]]
- 1 edge to [[_COMMUNITY_Charger-Namespaces]]
- 1 edge to [[_COMMUNITY_Ladepunkt]]
- 1 edge to [[_COMMUNITY_Register-Dekodierung]]
- 1 edge to [[_COMMUNITY_Gruppe 54]]

## Top bridge nodes
- [[DualCharger]] - degree 28, connects to 9 communities
- [[DualCharger.Enqueue()]] - degree 6, connects to 3 communities
- [[ChargerModbus.ProcessCommands()]] - degree 5, connects to 3 communities
- [[SimulationEngine.VehicleArrives()]] - degree 4, connects to 3 communities
- [[DualCharger.ProcessQueue()]] - degree 4, connects to 2 communities