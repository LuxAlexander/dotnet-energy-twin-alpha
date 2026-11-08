---
type: community
cohesion: 0.14
members: 18
---

# Gateway-Publisher

**Cohesion:** 0.14 - loosely connected
**Members:** 18 nodes

## Members
- [[--status-repeat Status alle 30 s wiederholen, sinceUtc unverändert]] - document - src/ModbusRabbitGateway/README.md
- [[Bekannte Probleme keine atomare Lesung, sequentielles Lesen, Compose mit festen Zugangsdaten]] - document - src/ModbusRabbitGateway/README.md
- [[ConsolePublisher]] - code - src/ModbusRabbitGateway/Publishing.cs
- [[Description_2]] - code - src/ModbusRabbitGateway/Publishing.cs
- [[Description_1]] - code - src/ModbusRabbitGateway/Publishing.cs
- [[Description_3]] - code - src/ModbusRabbitGateway/Publishing.cs
- [[Description_4]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[Geräte-ID-Prüfung Kürzel → Typ (Reg. 0), Nummer → Reg. 1, sonst decodeError]] - document - src/ModbusRabbitGateway/README.md
- [[IMessagePublisher]] - code - src/ModbusRabbitGateway/Publishing.cs
- [[ModbusRabbitGateway (Modbus → MQTT → RabbitMQ)]] - document - src/ModbusRabbitGateway/README.md
- [[Nachrichtenvertrag LB03 (docsNachrichtenvertrag-LB03.md)]] - document - src/ModbusRabbitGateway/README.md
- [[Publishing.cs]] - code - src/ModbusRabbitGateway/Publishing.cs
- [[RabbitMQ retained nur bei exaktem Topic, nicht bei Wildcard-Abos]] - document - src/ModbusRabbitGateway/README.md
- [[RecordingPublisher]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[Sent]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[TeePublisher]] - code - src/ModbusRabbitGateway/Publishing.cs
- [[TextWriter]] - code
- [[Topics energytwindevices{id}measurementsstatus, energytwingateway{id}status]] - document - src/ModbusRabbitGateway/README.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Gateway-Publisher
SORT file.name ASC
```

## Connections to other communities
- 5 edges to [[_COMMUNITY_GatewayService]]
- 5 edges to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 5 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 4 edges to [[_COMMUNITY_Nachrichtentypen (Contract)]]
- 4 edges to [[_COMMUNITY_TimeSyncServer]]
- 2 edges to [[_COMMUNITY_Messwert-Schlüssel]]
- 2 edges to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_MessageBuilder-Tests]]
- 1 edge to [[_COMMUNITY_MessageBuilder]]
- 1 edge to [[_COMMUNITY_Funktionsnachweis mit Master]]
- 1 edge to [[_COMMUNITY_Aufgabenabgleich (readTask)]]
- 1 edge to [[_COMMUNITY_DeviceEndpoint]]

## Top bridge nodes
- [[ModbusRabbitGateway (Modbus → MQTT → RabbitMQ)]] - degree 22, connects to 8 communities
- [[RecordingPublisher]] - degree 7, connects to 4 communities
- [[IMessagePublisher]] - degree 10, connects to 3 communities
- [[Publishing.cs]] - degree 5, connects to 2 communities
- [[--status-repeat Status alle 30 s wiederholen, sinceUtc unverändert]] - degree 3, connects to 2 communities