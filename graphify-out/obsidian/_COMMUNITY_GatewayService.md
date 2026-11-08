---
type: community
cohesion: 0.21
members: 15
---

# GatewayService

**Cohesion:** 0.21 - loosely connected
**Members:** 15 nodes

## Members
- [[BrokerDown]] - code - src/ModbusRabbitGateway/GatewayService.cs
- [[Description_5]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[Down]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[DroppedSinceOutage]] - code - src/ModbusRabbitGateway/GatewayService.cs
- [[FlakyPublisher]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[FlakyPublisher.PublishAsync()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[GatewayAgainstDeviceTests.Service_Once_PublishesGatewayStatusDeviceStatusAndMeasurements()]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[GatewayService]] - code - src/ModbusRabbitGateway/GatewayService.cs
- [[GatewayService.DescribeStatus()]] - code - src/ModbusRabbitGateway/GatewayService.cs
- [[GatewayService.PollAllAsync()]] - code - src/ModbusRabbitGateway/GatewayService.cs
- [[GatewayService.PublishGatewayStateAsync()]] - code - src/ModbusRabbitGateway/GatewayService.cs
- [[GatewayService.RunAsync()]] - code - src/ModbusRabbitGateway/GatewayService.cs
- [[GatewayService.Summarize()]] - code - src/ModbusRabbitGateway/GatewayService.cs
- [[GatewayService.TryPublishAsync()]] - code - src/ModbusRabbitGateway/GatewayService.cs
- [[MqttPublisherTests.GatewayService_KeepsPollingDuringBrokerOutage_AndReportsOutageOnce()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/GatewayService
SORT file.name ASC
```

## Connections to other communities
- 9 edges to [[_COMMUNITY_TimeSyncServer]]
- 6 edges to [[_COMMUNITY_Nachrichtentypen (Contract)]]
- 5 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 5 edges to [[_COMMUNITY_Gateway-Publisher]]
- 5 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 4 edges to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 3 edges to [[_COMMUNITY_Gateway-Optionen]]
- 2 edges to [[_COMMUNITY_Gruppe 23]]
- 2 edges to [[_COMMUNITY_Status-Wiederholung & RabbitMQ]]
- 2 edges to [[_COMMUNITY_MessageBuilder-Tests]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource (2)]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]
- 1 edge to [[_COMMUNITY_TimeState & TimeLine]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_SRS Gateway-Anforderungen]]
- 1 edge to [[_COMMUNITY_Funktionsnachweis mit Master]]
- 1 edge to [[_COMMUNITY_LB03-Projektstand]]
- 1 edge to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]

## Top bridge nodes
- [[GatewayService]] - degree 24, connects to 12 communities
- [[GatewayService.PublishGatewayStateAsync()]] - degree 10, connects to 6 communities
- [[MqttPublisherTests.GatewayService_KeepsPollingDuringBrokerOutage_AndReportsOutageOnce()]] - degree 9, connects to 5 communities
- [[GatewayAgainstDeviceTests.Service_Once_PublishesGatewayStatusDeviceStatusAndMeasurements()]] - degree 8, connects to 5 communities
- [[GatewayService.PollAllAsync()]] - degree 10, connects to 4 communities