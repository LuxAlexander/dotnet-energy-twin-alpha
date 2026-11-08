---
type: community
cohesion: 0.15
members: 16
---

# Modbus-Tests

**Cohesion:** 0.15 - loosely connected
**Members:** 16 nodes

## Members
- [[ChargerModbus.cs]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[ChargerRegisters.cs]] - code - src/Modbus.Contracts/ChargerRegisters.cs
- [[DeviceConnection.cs]] - code - src/ModbusDiagnosticClient/DeviceConnection.cs
- [[Devices.Common.Tests]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[DiagnosticClientTests.cs]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[KuehlhausRegisters.cs]] - code - src/Modbus.Contracts/KuehlhausRegisters.cs
- [[Modbus.Contracts]] - code - src/Modbus.Contracts/BatteryRegisters.cs
- [[ModbusDeviceServerTests.cs]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDiagnosticClient.Tests]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[PvModbus.cs]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[PvRegisters.cs]] - code - src/Modbus.Contracts/PvRegisters.cs
- [[StepSchedulerTests.cs]] - code - tests/Devices.Common.Tests/StepSchedulerTests.cs
- [[TimeSyncOptionsTests.cs]] - code - tests/Devices.Common.Tests/TimeSyncOptionsTests.cs
- [[WeatherFeedTests.cs]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[easymodbus]] - concept
- [[easymodbus_exceptions]] - concept

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Modbus-Tests
SORT file.name ASC
```

## Connections to other communities
- 12 edges to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 9 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 7 edges to [[_COMMUNITY_Charger-Namespaces]]
- 5 edges to [[_COMMUNITY_Registerpläne]]
- 4 edges to [[_COMMUNITY_PV-Namespaces]]
- 3 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 3 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_Gruppe 48]]
- 1 edge to [[_COMMUNITY_Gruppe 40]]
- 1 edge to [[_COMMUNITY_DeviceConnection]]
- 1 edge to [[_COMMUNITY_StepScheduler]]
- 1 edge to [[_COMMUNITY_TimeSyncOptions Tests]]
- 1 edge to [[_COMMUNITY_WeatherFeed]]
- 1 edge to [[_COMMUNITY_Gruppe 42]]
- 1 edge to [[_COMMUNITY_Register-Dekodierung]]

## Top bridge nodes
- [[Modbus.Contracts]] - degree 21, connects to 8 communities
- [[DiagnosticClientTests.cs]] - degree 10, connects to 6 communities
- [[DeviceConnection.cs]] - degree 8, connects to 4 communities
- [[ModbusDeviceServerTests.cs]] - degree 8, connects to 3 communities
- [[ChargerModbus.cs]] - degree 7, connects to 3 communities