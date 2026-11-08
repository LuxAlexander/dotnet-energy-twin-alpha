---
type: community
cohesion: 0.14
members: 27
---

# Modbus-Tests

**Cohesion:** 0.14 - loosely connected
**Members:** 27 nodes

## Members
- [[BatteryModbus.cs]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[ChargerModbus.cs]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[ChargerRegisters.cs]] - code - src/Modbus.Contracts/ChargerRegisters.cs
- [[Devices.Common.Modbus]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[DiagnosticClientTests.cs]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[FieldDeviceTests.cs]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[GatewayAgainstDeviceTests.cs]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[KuehlhausModbus.cs]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[KuehlhausRegisters.cs]] - code - src/Modbus.Contracts/KuehlhausRegisters.cs
- [[Modbus.Contracts]] - code - src/Modbus.Contracts/BatteryRegisters.cs
- [[ModbusDeviceServer.cs]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServerTests.cs]] - code - tests/Devices.Common.Tests/ModbusDeviceServerTests.cs
- [[ModbusDiagnosticClient.Tests]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[ModbusOptions.cs]] - code - src/Devices.Common/Modbus/ModbusOptions.cs
- [[MqttPublisherTests.cs]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[PvEngineTests]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvModbus.cs]] - code - src/Devices/PvSimulator/Simulation/PvModbus.cs
- [[PvRegisters.cs]] - code - src/Modbus.Contracts/PvRegisters.cs
- [[RegisterDefinition.cs]] - code - src/Modbus.Contracts/RegisterDefinition.cs
- [[TimeSyncClient.cs]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncServer.cs]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[easymodbus]] - concept
- [[modbus_contracts_batteryregisters]] - concept
- [[system_collections_concurrent]] - concept
- [[system_net]] - concept
- [[system_net_sockets]] - concept
- [[system_threading_channels]] - concept

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Modbus-Tests
SORT file.name ASC
```

## Connections to other communities
- 16 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 14 edges to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 13 edges to [[_COMMUNITY_Gruppe 19]]
- 6 edges to [[_COMMUNITY_PV-Namespaces]]
- 5 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 4 edges to [[_COMMUNITY_Registerpläne]]
- 3 edges to [[_COMMUNITY_Schreibprüfung]]
- 3 edges to [[_COMMUNITY_Charger-Namespaces]]
- 3 edges to [[_COMMUNITY_JSON-Serialisierung]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 2 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 2 edges to [[_COMMUNITY_RegisterCodec & Befehle (2)]]
- 2 edges to [[_COMMUNITY_Modbus.Client-Fehler]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 1 edge to [[_COMMUNITY_ArgsParser (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 48]]
- 1 edge to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_Gruppe 57]]
- 1 edge to [[_COMMUNITY_Gruppe 24]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_Gruppe 89]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 102]]
- 1 edge to [[_COMMUNITY_Gruppe 105]]
- 1 edge to [[_COMMUNITY_Batterie-Modell]]
- 1 edge to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]
- 1 edge to [[_COMMUNITY_Gruppe 82]]
- 1 edge to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]

## Top bridge nodes
- [[FieldDeviceTests.cs]] - degree 21, connects to 12 communities
- [[Modbus.Contracts]] - degree 20, connects to 6 communities
- [[DiagnosticClientTests.cs]] - degree 10, connects to 5 communities
- [[Devices.Common.Modbus]] - degree 18, connects to 4 communities
- [[ModbusDeviceServer.cs]] - degree 12, connects to 4 communities