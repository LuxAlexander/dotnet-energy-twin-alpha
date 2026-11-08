---
type: community
cohesion: 0.09
members: 32
---

# Batterie-Modell

**Cohesion:** 0.09 - loosely connected
**Members:** 32 nodes

## Members
- [[ActiveFault_2]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[ActualKW]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[AmbientTemperatureC_1]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.CalculatePower()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.GetDerateFactor()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.GetTaperFactor()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.ResetFault()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.SetPower()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.TriggerFault()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.TurnOff()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.TurnOn()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.Update()_1]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.UpdateState()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Batterie.UpdateTemperature()]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[BatteryModbus.ProcessCommands()]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[ChargedKWh]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Config]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[DeviceId]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[DischargeKWh]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[EmergencyStop]] - code - src/Devices/BatterySimulator/Enum.cs
- [[EquivalentCycles]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[FaultType_2]] - code - src/Devices/BatterySimulator/Enum.cs
- [[IsOn_1]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[None_4]] - code - src/Devices/BatterySimulator/Enum.cs
- [[Overheat_1]] - code - src/Devices/BatterySimulator/Enum.cs
- [[SetpointKW]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[Simulator.HandleKey()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[Simulator.SetSetpoint()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[SocPercent]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[State_1]] - code - src/Devices/BatterySimulator/Batterie.cs
- [[TemperaturC]] - code - src/Devices/BatterySimulator/Batterie.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Batterie-Modell
SORT file.name ASC
```

## Connections to other communities
- 8 edges to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 5 edges to [[_COMMUNITY_Gruppe 21]]
- 5 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 3 edges to [[_COMMUNITY_Gruppe 24]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 2 edges to [[_COMMUNITY_Gruppe 19]]
- 2 edges to [[_COMMUNITY_DeviceTimeSource (2)]]
- 2 edges to [[_COMMUNITY_TimeSyncServer]]
- 1 edge to [[_COMMUNITY_Gruppe 106]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle]]
- 1 edge to [[_COMMUNITY_Zeitsync-Client & Konsole]]

## Top bridge nodes
- [[Batterie]] - degree 36, connects to 7 communities
- [[Simulator.HandleKey()]] - degree 13, connects to 5 communities
- [[BatteryModbus.ProcessCommands()]] - degree 11, connects to 4 communities
- [[Batterie.SetPower()]] - degree 8, connects to 4 communities
- [[FaultType_2]] - degree 7, connects to 2 communities