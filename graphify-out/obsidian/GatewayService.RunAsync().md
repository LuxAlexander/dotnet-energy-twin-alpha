---
source_file: "src/ModbusRabbitGateway/GatewayService.cs"
type: "code"
community: "GatewayService"
location: "L28"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/GatewayService
---

# GatewayService.RunAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[GatewayAgainstDeviceTests.Service_Once_PublishesGatewayStatusDeviceStatusAndMeasurements()]] - `calls` [INFERRED]
- [[GatewayService]] - `method` [EXTRACTED]
- [[GatewayService.PollAllAsync()]] - `calls` [EXTRACTED]
- [[GatewayService.PublishGatewayStateAsync()]] - `calls` [EXTRACTED]
- [[MqttPublisherTests.GatewayService_KeepsPollingDuringBrokerOutage_AndReportsOutageOnce()]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/GatewayService