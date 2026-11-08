---
type: community
cohesion: 0.11
members: 21
---

# Nachrichtentypen (Contract)

**Cohesion:** 0.11 - loosely connected
**Members:** 21 nodes

## Members
- [[Contract]] - code - src/ModbusRabbitGateway/Messages.cs
- [[Contract.DeviceStatusTopic()]] - code - src/ModbusRabbitGateway/Messages.cs
- [[Contract.FormatSimTime()]] - code - src/ModbusRabbitGateway/Messages.cs
- [[Contract.FormatUtc()]] - code - src/ModbusRabbitGateway/Messages.cs
- [[Contract.GatewayStatusTopic()]] - code - src/ModbusRabbitGateway/Messages.cs
- [[Contract.MeasurementsTopic()]] - code - src/ModbusRabbitGateway/Messages.cs
- [[DeviceEndpoint.cs]] - code - src/ModbusRabbitGateway/DeviceEndpoint.cs
- [[DevicePoller.Status()]] - code - src/ModbusRabbitGateway/DevicePoller.cs
- [[DevicePoller.cs]] - code - src/ModbusRabbitGateway/DevicePoller.cs
- [[DeviceState_2]] - code - src/ModbusRabbitGateway/Messages.cs
- [[GatewayOptions.cs]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[GatewayService.cs]] - code - src/ModbusRabbitGateway/GatewayService.cs
- [[Measurement]] - code - src/ModbusRabbitGateway/Messages.cs
- [[MeasurementMessage]] - code - src/ModbusRabbitGateway/Messages.cs
- [[MessageJson]] - code - src/ModbusRabbitGateway/Messages.cs
- [[Messages.cs]] - code - src/ModbusRabbitGateway/Messages.cs
- [[ModbusRabbitGateway]] - code - src/ModbusRabbitGateway/DeviceEndpoint.cs
- [[PollOutcome]] - code - src/ModbusRabbitGateway/DevicePoller.cs
- [[Program.cs]] - code - src/ModbusRabbitGateway/Program.cs
- [[Quality_1]] - code - src/ModbusRabbitGateway/Messages.cs
- [[Reason]] - code - src/ModbusRabbitGateway/Messages.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Nachrichtentypen_Contract
SORT file.name ASC
```

## Connections to other communities
- 6 edges to [[_COMMUNITY_MessageBuilder]]
- 6 edges to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 6 edges to [[_COMMUNITY_GatewayService]]
- 4 edges to [[_COMMUNITY_Gruppe 19]]
- 4 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 4 edges to [[_COMMUNITY_Gateway-Publisher]]
- 3 edges to [[_COMMUNITY_Status-Wiederholung & RabbitMQ]]
- 3 edges to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 2 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 2 edges to [[_COMMUNITY_JSON-Serialisierung]]
- 2 edges to [[_COMMUNITY_MessageBuilder-Tests]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_Modbus.Client-Fehler]]
- 1 edge to [[_COMMUNITY_DeviceEndpoint]]
- 1 edge to [[_COMMUNITY_Messwert-Schlüssel]]
- 1 edge to [[_COMMUNITY_Gateway-Optionen]]

## Top bridge nodes
- [[MeasurementMessage]] - degree 12, connects to 7 communities
- [[ModbusRabbitGateway]] - degree 11, connects to 4 communities
- [[Contract.FormatUtc()]] - degree 6, connects to 4 communities
- [[Messages.cs]] - degree 9, connects to 3 communities
- [[DevicePoller.Status()]] - degree 7, connects to 3 communities