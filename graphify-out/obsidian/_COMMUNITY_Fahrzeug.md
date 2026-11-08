---
type: community
cohesion: 0.14
members: 18
---

# Fahrzeug

**Cohesion:** 0.14 - loosely connected
**Members:** 18 nodes

## Members
- [[BatteryCapacityKWh]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[ChargingPoint.Connect()]] - code - src/Devices/DualPortCharger/Models/ChargingPoint.cs
- [[DualCharger.Enqueue()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[HasDamagedInlet]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[IsFull]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[MaxChargePowerKW]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[ModelName]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[Models]] - code - src/Devices/DualPortCharger/Models/VehicleCatalog.cs
- [[SimulationEngine.VehicleArrives()]] - code - src/Devices/DualPortCharger/Simulation/SimulationEngine.cs
- [[SocPercent_1]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[TargetSocPercent]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[Vehicle]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[Vehicle.cs]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[VehicleCatalog]] - code - src/Devices/DualPortCharger/Models/VehicleCatalog.cs
- [[VehicleCatalog.Create()]] - code - src/Devices/DualPortCharger/Models/VehicleCatalog.cs
- [[VehicleCatalog.CreateRandom()]] - code - src/Devices/DualPortCharger/Models/VehicleCatalog.cs
- [[VehicleCatalog.cs]] - code - src/Devices/DualPortCharger/Models/VehicleCatalog.cs
- [[VehicleModel]] - code - src/Devices/DualPortCharger/Models/VehicleCatalog.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Fahrzeug
SORT file.name ASC
```

## Connections to other communities
- 4 edges to [[_COMMUNITY_Ladepunkt]]
- 3 edges to [[_COMMUNITY_DualCharger]]
- 3 edges to [[_COMMUNITY_Batterie-Simulator Loop (3)]]
- 2 edges to [[_COMMUNITY_Gruppe 65]]
- 2 edges to [[_COMMUNITY_Charger-Namespaces]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_Gruppe 104]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (2)]]

## Top bridge nodes
- [[DualCharger.Enqueue()]] - degree 6, connects to 3 communities
- [[Vehicle]] - degree 16, connects to 2 communities
- [[VehicleCatalog.CreateRandom()]] - degree 6, connects to 2 communities
- [[SimulationEngine.VehicleArrives()]] - degree 4, connects to 2 communities
- [[ChargingPoint.Connect()]] - degree 3, connects to 2 communities