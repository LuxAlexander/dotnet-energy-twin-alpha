---
type: community
cohesion: 0.18
members: 18
---

# DevicePoller & Gerätetests

**Cohesion:** 0.18 - loosely connected
**Members:** 18 nodes

## Members
- [[Device]] - code - src/ModbusRabbitGateway/DevicePoller.cs
- [[DevicePoller]] - code - src/ModbusRabbitGateway/DevicePoller.cs
- [[DevicePoller.Poll()]] - code - src/ModbusRabbitGateway/DevicePoller.cs
- [[DeviceTimeSource_2]] - code
- [[GatewayAgainstDeviceTests]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[GatewayAgainstDeviceTests.DeviceLost_GivesOfflineStatusWithoutMeasurements_ThenRecovers()]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[GatewayAgainstDeviceTests.Dispose()]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[GatewayAgainstDeviceTests.FreePort()]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[GatewayAgainstDeviceTests.NewPoller()]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[GatewayAgainstDeviceTests.Poll_ReadsDeviceOverModbus_AndBuildsMessage()]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[GatewayAgainstDeviceTests.StartServer()]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[GatewayAgainstDeviceTests.Status_IsRepeatedAfterInterval_WithOriginalSince_ForWildcardSubscribers()]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[Gefundener Fehler MQTTnet 5 liefert Ergebniscode statt Exception bei abgelehnter Anmeldung]] - document - tests/ModbusRabbitGateway.Tests/README.md
- [[Gefundener Fehler doppelte offline-Statusnachricht bei wechselndem Fehlertext]] - document - tests/ModbusRabbitGateway.Tests/README.md
- [[ModbusDeviceServer_2]] - code
- [[ModbusRabbitGateway.Tests (5252 am 2026-11-08)]] - document - tests/ModbusRabbitGateway.Tests/README.md
- [[State_3]] - code - src/ModbusRabbitGateway/DevicePoller.cs
- [[Tracker]] - code - src/ModbusRabbitGateway/DevicePoller.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/DevicePoller__Gerätetests
SORT file.name ASC
```

## Connections to other communities
- 6 edges to [[_COMMUNITY_Nachrichtentypen (Contract)]]
- 5 edges to [[_COMMUNITY_Gateway-Publisher]]
- 4 edges to [[_COMMUNITY_GatewayService]]
- 3 edges to [[_COMMUNITY_Gruppe 23]]
- 3 edges to [[_COMMUNITY_DeviceConnection (Modbus.Client)]]
- 3 edges to [[_COMMUNITY_DeviceEndpoint]]
- 3 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 2 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 2 edges to [[_COMMUNITY_SRS Gateway-Anforderungen]]
- 2 edges to [[_COMMUNITY_MessageBuilder-Tests]]
- 2 edges to [[_COMMUNITY_MessageBuilder]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]
- 1 edge to [[_COMMUNITY_Status-Wiederholung & RabbitMQ]]
- 1 edge to [[_COMMUNITY_Funktionsnachweis mit Master]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_Gateway-Optionen]]
- 1 edge to [[_COMMUNITY_Messwert-Schlüssel]]
- 1 edge to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]

## Top bridge nodes
- [[DevicePoller]] - degree 27, connects to 13 communities
- [[GatewayAgainstDeviceTests]] - degree 18, connects to 6 communities
- [[ModbusRabbitGateway.Tests (5252 am 2026-11-08)]] - degree 10, connects to 6 communities
- [[DevicePoller.Poll()]] - degree 9, connects to 4 communities
- [[GatewayAgainstDeviceTests.NewPoller()]] - degree 6, connects to 2 communities