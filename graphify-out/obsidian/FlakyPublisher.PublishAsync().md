---
source_file: "tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs"
type: "code"
community: "GatewayService"
location: "L176"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/GatewayService
---

# FlakyPublisher.PublishAsync()

## Connections
- [[BrokerUnavailableException]] - `calls` [INFERRED]
- [[CancellationToken]] - `references` [EXTRACTED]
- [[FlakyPublisher]] - `method` [EXTRACTED]
- [[IMessagePublisher.PublishAsync()]] - `dispatches_to` [INFERRED]
- [[Task]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/GatewayService