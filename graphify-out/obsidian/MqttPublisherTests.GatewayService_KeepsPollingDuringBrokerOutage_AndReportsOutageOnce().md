---
source_file: "tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs"
type: "code"
community: "GatewayService"
location: "L145"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/GatewayService
---

# MqttPublisherTests.GatewayService_KeepsPollingDuringBrokerOutage_AndReportsOutageOnce()

## Connections
- [[DevicePoller]] - `calls` [INFERRED]
- [[Fact]] - `references` [EXTRACTED]
- [[FlakyPublisher]] - `calls` [EXTRACTED]
- [[GatewayOptions.Parse()]] - `calls` [EXTRACTED]
- [[GatewayService]] - `calls` [INFERRED]
- [[GatewayService.RunAsync()]] - `calls` [INFERRED]
- [[MqttPublisherTests]] - `method` [EXTRACTED]
- [[MqttPublisherTests.CountOf()]] - `calls` [EXTRACTED]
- [[Task]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/GatewayService