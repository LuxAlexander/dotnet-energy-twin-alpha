---
type: community
cohesion: 0.27
members: 13
---

# Gruppe 51

**Cohesion:** 0.27 - loosely connected
**Members:** 13 nodes

## Members
- [[AutoModeController]] - code - src/Devices/DualPortCharger/Simulation/AutoModeController.cs
- [[AutoModeController.GetArrivalProbability()]] - code - src/Devices/DualPortCharger/Simulation/AutoModeController.cs
- [[AutoModeController.GetArrivalRatePerHour()]] - code - src/Devices/DualPortCharger/Simulation/AutoModeController.cs
- [[AutoModeController.HandleDepartures()]] - code - src/Devices/DualPortCharger/Simulation/AutoModeController.cs
- [[AutoModeController.RandomDwellTime()]] - code - src/Devices/DualPortCharger/Simulation/AutoModeController.cs
- [[AutoModeController.Step()]] - code - src/Devices/DualPortCharger/Simulation/AutoModeController.cs
- [[ChargingPoint.GetDemandKW()]] - code - src/Devices/DualPortCharger/Models/ChargingPoint.cs
- [[ChargingPoint.Update()]] - code - src/Devices/DualPortCharger/Models/ChargingPoint.cs
- [[ChargingPoint.UpdateTemperature()]] - code - src/Devices/DualPortCharger/Models/ChargingPoint.cs
- [[Dictionary]] - code
- [[DualCharger.AllocatePower()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[DualCharger.Tick()]] - code - src/Devices/DualPortCharger/Models/DualCharger.cs
- [[TimeSpan]] - code

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Gruppe_51
SORT file.name ASC
```

## Connections to other communities
- 7 edges to [[_COMMUNITY_Ladepunkt]]
- 6 edges to [[_COMMUNITY_WeatherFeed]]
- 6 edges to [[_COMMUNITY_DualCharger]]
- 5 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 5 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 3 edges to [[_COMMUNITY_Fahrzeug]]
- 3 edges to [[_COMMUNITY_TimeSyncServer]]
- 3 edges to [[_COMMUNITY_MasterClock]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource]]
- 2 edges to [[_COMMUNITY_StepScheduler]]
- 2 edges to [[_COMMUNITY_Gruppe 43]]
- 2 edges to [[_COMMUNITY_Gruppe 41]]
- 2 edges to [[_COMMUNITY_Gruppe 58]]
- 2 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 2 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_Gruppe 44]]
- 1 edge to [[_COMMUNITY_Gruppe 71]]
- 1 edge to [[_COMMUNITY_Charger-Namespaces]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_Kühlhaus-Modell]]
- 1 edge to [[_COMMUNITY_Gruppe 46]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]
- 1 edge to [[_COMMUNITY_ArgsParser]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_Gruppe 54]]
- 1 edge to [[_COMMUNITY_SyncedClock]]
- 1 edge to [[_COMMUNITY_Gruppe 84]]

## Top bridge nodes
- [[TimeSpan]] - degree 49, connects to 28 communities
- [[AutoModeController]] - degree 12, connects to 6 communities
- [[AutoModeController.Step()]] - degree 8, connects to 4 communities
- [[DualCharger.Tick()]] - degree 8, connects to 3 communities
- [[ChargingPoint.Update()]] - degree 8, connects to 2 communities