---
type: community
cohesion: 0.15
members: 13
---

# Batterie-Modbus & Update (3)

**Cohesion:** 0.15 - loosely connected
**Members:** 13 nodes

## Members
- [[BaseUrl]] - code - src/Devices.Common/Weather/WeatherClient.cs
- [[BatteryModbus]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[BatteryModbus.Dispose()]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[BatteryModbus.ValidateValue()]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[BatteryModbusTests.Dispose()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[HttpClient]] - code
- [[IDisposable]] - code
- [[KuehlhausModbus.Dispose()]] - code - src/Devices/KuehlhausSimulator/Simulation/KuehlhausModbus.cs
- [[KuehlhausModbusTests.Dispose()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[ModbusTestBase.Dispose()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[StatusText]] - code - src/Devices/BatterySimulator/BatteryModbus.cs
- [[WeatherClient]] - code - src/Devices.Common/Weather/WeatherClient.cs
- [[WeatherClient.Dispose()]] - code - src/Devices.Common/Weather/WeatherClient.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Batterie-Modbus__Update_3
SORT file.name ASC
```

## Connections to other communities
- 7 edges to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 6 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 3 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 3 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 2 edges to [[_COMMUNITY_Batterie-Modell]]
- 2 edges to [[_COMMUNITY_Gruppe 21]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 2 edges to [[_COMMUNITY_WeatherFeed]]
- 2 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 2 edges to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle]]
- 1 edge to [[_COMMUNITY_PV-Engine & Fehler]]
- 1 edge to [[_COMMUNITY_Schreibprüfung]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_JSON-Serialisierung]]
- 1 edge to [[_COMMUNITY_DeviceConnection (Modbus.Client)]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]

## Top bridge nodes
- [[IDisposable]] - degree 14, connects to 9 communities
- [[WeatherClient]] - degree 12, connects to 8 communities
- [[BatteryModbus]] - degree 14, connects to 6 communities
- [[KuehlhausModbus.Dispose()]] - degree 4, connects to 3 communities
- [[BatteryModbus.Dispose()]] - degree 4, connects to 2 communities