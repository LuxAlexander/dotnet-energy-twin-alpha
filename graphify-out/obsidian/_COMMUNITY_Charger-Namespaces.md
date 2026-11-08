---
type: community
cohesion: 0.30
members: 12
---

# Charger-Namespaces

**Cohesion:** 0.30 - loosely connected
**Members:** 12 nodes

## Members
- [[DualChargerSim]] - code - src/Devices/DualPortCharger/Program.cs
- [[DualChargerSim.IO]] - code - src/Devices/DualPortCharger/IO/ArgsParser.cs
- [[DualChargerSim.Models]] - code - src/Devices/DualPortCharger/Models/ChargingPoint.cs
- [[DualChargerSim.Simulation]] - code - src/Devices/DualPortCharger/Simulation/AutoModeController.cs
- [[DualPortChargerIOArgsParser.cs]] - code - src/Devices/DualPortCharger/IO/ArgsParser.cs
- [[DualPortChargerIOConsoleRenderer.cs]] - code - src/Devices/DualPortCharger/IO/ConsoleRenderer.cs
- [[DualPortChargerIOCsvLogger.cs]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[DualPortChargerIOInputHandler.cs]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[DualPortChargerProgram.cs]] - code - src/Devices/DualPortCharger/Program.cs
- [[DualPortChargerSimulationAutoModeController.cs]] - code - src/Devices/DualPortCharger/Simulation/AutoModeController.cs
- [[DualPortChargerSimulationSimulationEngine.cs]] - code - src/Devices/DualPortCharger/Simulation/SimulationEngine.cs
- [[Program_2]] - code - src/Devices/DualPortCharger/Program.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Charger-Namespaces
SORT file.name ASC
```

## Connections to other communities
- 7 edges to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 7 edges to [[_COMMUNITY_Gruppe 19]]
- 3 edges to [[_COMMUNITY_Modbus-Tests]]
- 2 edges to [[_COMMUNITY_Gruppe 37]]
- 2 edges to [[_COMMUNITY_Fahrzeug]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop (3)]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]
- 1 edge to [[_COMMUNITY_Gruppe 69]]
- 1 edge to [[_COMMUNITY_Ladepunkt]]
- 1 edge to [[_COMMUNITY_Gruppe 108]]
- 1 edge to [[_COMMUNITY_DualCharger]]

## Top bridge nodes
- [[DualChargerSim.Models]] - degree 12, connects to 5 communities
- [[DualPortChargerIOArgsParser.cs]] - degree 8, connects to 4 communities
- [[DualPortChargerIOConsoleRenderer.cs]] - degree 7, connects to 3 communities
- [[DualPortChargerSimulationSimulationEngine.cs]] - degree 7, connects to 3 communities
- [[DualPortChargerIOInputHandler.cs]] - degree 6, connects to 3 communities