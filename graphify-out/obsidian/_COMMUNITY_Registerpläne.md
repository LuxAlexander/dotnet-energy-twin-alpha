---
type: community
cohesion: 0.10
members: 22
---

# Registerpläne

**Cohesion:** 0.10 - loosely connected
**Members:** 22 nodes

## Members
- [[Battery]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[BatteryRegisters]] - code - src/Modbus.Contracts/BatteryRegisters.cs
- [[BatteryRegisters.cs]] - code - src/Modbus.Contracts/BatteryRegisters.cs
- [[Charger]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[Definitions]] - code - src/Modbus.Contracts/RegisterPlan.cs
- [[DeviceName]] - code - src/Modbus.Contracts/RegisterPlan.cs
- [[DeviceType]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[DeviceType_1]] - code - src/Modbus.Contracts/RegisterPlan.cs
- [[IndustrialConsumer]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[KuehlhausRegisters]] - code - src/Modbus.Contracts/KuehlhausRegisters.cs
- [[KuehlhausRegisters.CreatePlan()]] - code - src/Modbus.Contracts/KuehlhausRegisters.cs
- [[Plan_1]] - code - src/Modbus.Contracts/BatteryRegisters.cs
- [[Plan_3]] - code - src/Modbus.Contracts/KuehlhausRegisters.cs
- [[Plan_4]] - code - src/Modbus.Contracts/PvRegisters.cs
- [[PvPlant]] - code - src/Modbus.Contracts/CommonRegisters.cs
- [[PvRegisters]] - code - src/Modbus.Contracts/PvRegisters.cs
- [[RegisterPlan]] - code - src/Modbus.Contracts/RegisterPlan.cs
- [[RegisterPlanRequirementTests]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[RegisterPlanRequirementTests.EveryDevicePlan_HasSetpointAndAtLeastThreeSpecificValues()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[RegisterPlanRequirementTests.Markdown_ListsAllRegisters_WithByteAndWordOrder()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[RegisterPlans.For()]] - code - src/Modbus.Contracts/RegisterPlans.cs
- [[Wind]] - code - src/Modbus.Contracts/CommonRegisters.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Registerpläne
SORT file.name ASC
```

## Connections to other communities
- 5 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 4 edges to [[_COMMUNITY_Modbus-Tests]]
- 3 edges to [[_COMMUNITY_Schreibprüfung]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 2 edges to [[_COMMUNITY_Gruppe 23]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle]]
- 1 edge to [[_COMMUNITY_Gruppe 82]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_Gruppe 19]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle (2)]]
- 1 edge to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]

## Top bridge nodes
- [[RegisterPlan]] - degree 23, connects to 8 communities
- [[DeviceType]] - degree 9, connects to 2 communities
- [[RegisterPlans.For()]] - degree 5, connects to 2 communities
- [[KuehlhausRegisters]] - degree 4, connects to 1 community
- [[PvRegisters]] - degree 3, connects to 1 community