---
type: community
cohesion: 0.11
members: 31
---

# Batterie-Modbus & Update

**Cohesion:** 0.11 - loosely connected
**Members:** 31 nodes

## Members
- [[Action]] - code
- [[ChargerModbus]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[ChargerModbus.Publish()]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[ChargerModbus.Start()]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[ClientCount]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[DiagnosticClientTests.StartServer()]] - code - tests/ModbusDiagnosticClient.Tests/DiagnosticClientTests.cs
- [[HeaderValues]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[IsListening]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[KuehlhausModbus]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[KuehlhausModbus.Publish()]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[KuehlhausModbus.Start()]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[LastWriteText]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.Commit()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.OnClientsChanged()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.OnHoldingRegistersChanged()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.ParseInstance()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.Publish()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.SetErrorRegisters()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusDeviceServer.Start()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[ModbusServer]] - code
- [[Options]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[Plan]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[RegisterWriter]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[RegisterWriter.SetFloat()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[RegisterWriter.SetUInt16()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[RegisterWriter.SetUInt32()]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[StatusText_1]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[StatusText_2]] - code - src/Devices/DualPortCharger/Simulation/ChargerModbus.cs
- [[StatusText_3]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[ValueValidator]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Batterie-Modbus__Update
SORT file.name ASC
```

## Connections to other communities
- 10 edges to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 8 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 6 edges to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 5 edges to [[_COMMUNITY_Modbus-Tests]]
- 5 edges to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 5 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 5 edges to [[_COMMUNITY_DeviceTimeSource]]
- 4 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 4 edges to [[_COMMUNITY_Schreibprüfung]]
- 3 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 2 edges to [[_COMMUNITY_Gruppe 82]]
- 2 edges to [[_COMMUNITY_Gruppe 89]]
- 2 edges to [[_COMMUNITY_Registerpläne]]
- 2 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 2 edges to [[_COMMUNITY_DualCharger]]
- 2 edges to [[_COMMUNITY_RegisterCodec & Befehle (2)]]
- 2 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 1 edge to [[_COMMUNITY_MasterClock]]
- 1 edge to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 1 edge to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 1 edge to [[_COMMUNITY_Aufgabe 03 & Nachweise]]
- 1 edge to [[_COMMUNITY_Funktionsnachweis mit Master]]
- 1 edge to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_Kühlhaus-Modell]]
- 1 edge to [[_COMMUNITY_WeatherFeed]]
- 1 edge to [[_COMMUNITY_Gruppe 21]]
- 1 edge to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 1 edge to [[_COMMUNITY_Gruppe 72]]

## Top bridge nodes
- [[ModbusDeviceServer]] - degree 42, connects to 17 communities
- [[KuehlhausModbus]] - degree 13, connects to 6 communities
- [[ChargerModbus]] - degree 12, connects to 6 communities
- [[HeaderValues]] - degree 10, connects to 6 communities
- [[Action]] - degree 9, connects to 6 communities