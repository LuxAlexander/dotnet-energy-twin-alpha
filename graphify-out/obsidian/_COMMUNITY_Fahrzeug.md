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
- [[HasDamagedInlet]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[IsFull]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[MaxChargePowerKW]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[ModelName]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[Models]] - code - src/Devices/DualPortCharger/Models/VehicleCatalog.cs
- [[SocPercent_1]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[TargetSocPercent]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[Vehicle_1]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[Vehicle.AddEnergy()]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
- [[Vehicle.GetMaxPowerAtSoc()]] - code - src/Devices/DualPortCharger/Models/Vehicle.cs
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
- 3 edges to [[_COMMUNITY_Gruppe 51]]
- 3 edges to [[_COMMUNITY_DualCharger]]
- 2 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 2 edges to [[_COMMUNITY_Charger-Namespaces]]
- 2 edges to [[_COMMUNITY_Ladepunkt]]
- 1 edge to [[_COMMUNITY_Register-Dekodierung]]
- 1 edge to [[_COMMUNITY_Gruppe 43]]

## Top bridge nodes
- [[VehicleCatalog.CreateRandom()]] - degree 6, connects to 3 communities
- [[Vehicle_1]] - degree 16, connects to 2 communities
- [[ChargingPoint.Connect()]] - degree 3, connects to 2 communities
- [[VehicleCatalog]] - degree 6, connects to 1 community
- [[VehicleCatalog.Create()]] - degree 5, connects to 1 community