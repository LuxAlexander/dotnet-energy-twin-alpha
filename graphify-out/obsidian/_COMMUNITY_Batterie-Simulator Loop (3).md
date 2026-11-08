---
type: community
cohesion: 0.36
members: 9
---

# Batterie-Simulator Loop (3)

**Cohesion:** 0.36 - loosely connected
**Members:** 9 nodes

## Members
- [[ChargingPoint.Start()]] - code - src/Devices/DualPortCharger/Models/ChargingPoint.cs
- [[ChargingPoint.Stop()]] - code - src/Devices/DualPortCharger/Models/ChargingPoint.cs
- [[ConsoleRenderer.PromptAsync()]] - code - src/Devices/DualPortCharger/IO/ConsoleRenderer.cs
- [[InputHandler]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.AskPercentAsync()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.ChangePowerLimit()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.ConnectVehicleAsync()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.HandleAsync()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.ToggleCharging()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Batterie-Simulator_Loop_3
SORT file.name ASC
```

## Connections to other communities
- 7 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 5 edges to [[_COMMUNITY_TimeSyncServer]]
- 4 edges to [[_COMMUNITY_Ladepunkt]]
- 4 edges to [[_COMMUNITY_Gruppe 69]]
- 3 edges to [[_COMMUNITY_Fahrzeug]]
- 2 edges to [[_COMMUNITY_Gruppe 56]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]
- 1 edge to [[_COMMUNITY_Charger-Namespaces]]
- 1 edge to [[_COMMUNITY_DualCharger]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_Zeitsync-Client & Konsole]]

## Top bridge nodes
- [[InputHandler.HandleAsync()]] - degree 12, connects to 6 communities
- [[InputHandler]] - degree 11, connects to 5 communities
- [[ConsoleRenderer.PromptAsync()]] - degree 9, connects to 4 communities
- [[InputHandler.ConnectVehicleAsync()]] - degree 9, connects to 4 communities
- [[InputHandler.AskPercentAsync()]] - degree 5, connects to 2 communities