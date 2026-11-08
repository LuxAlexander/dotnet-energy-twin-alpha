---
source_file: "src/ModbusRabbitGateway/GatewayService.cs"
type: "code"
community: "GatewayService"
location: "L11"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/GatewayService
---

# GatewayService

## Connections
- [[BrokerDown]] - `defines` [EXTRACTED]
- [[CultureInfo]] - `references` [EXTRACTED]
- [[DevicePoller]] - `references` [EXTRACTED]
- [[DroppedSinceOutage]] - `defines` [EXTRACTED]
- [[FA-GW-10 Kontrolliert beenden, offline, Last Will]] - `implements` [INFERRED]
- [[Funktionsnachweis 2026-11-08 Ladesäule → Modbus → Gateway → RabbitMQ → Viewer]] - `references` [INFERRED]
- [[GatewayAgainstDeviceTests.Service_Once_PublishesGatewayStatusDeviceStatusAndMeasurements()]] - `calls` [INFERRED]
- [[GatewayOptions]] - `references` [EXTRACTED]
- [[GatewayService.DescribeStatus()]] - `method` [EXTRACTED]
- [[GatewayService.PollAllAsync()]] - `method` [EXTRACTED]
- [[GatewayService.PublishGatewayStateAsync()]] - `method` [EXTRACTED]
- [[GatewayService.RunAsync()]] - `method` [EXTRACTED]
- [[GatewayService.Summarize()]] - `method` [EXTRACTED]
- [[GatewayService.TryPublishAsync()]] - `method` [EXTRACTED]
- [[GatewayService.cs]] - `contains` [EXTRACTED]
- [[IMessagePublisher]] - `references` [EXTRACTED]
- [[IReadOnlyList]] - `references` [EXTRACTED]
- [[ModbusRabbitGateway (Modbus → MQTT → RabbitMQ)]] - `references` [EXTRACTED]
- [[ModbusRabbitGateway (Projekt)]] - `references` [INFERRED]
- [[MqttPublisherTests]] - `references` [EXTRACTED]
- [[MqttPublisherTests.GatewayService_KeepsPollingDuringBrokerOutage_AndReportsOutageOnce()]] - `calls` [INFERRED]
- [[Status-Wiederholung alle 30 s (--status-repeat)]] - `implements` [INFERRED]
- [[TextWriter]] - `references` [EXTRACTED]
- [[TimeProvider]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/GatewayService