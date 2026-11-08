---
source_file: "tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs"
type: "code"
community: "MQTT-Publisher & Zugangsdaten"
location: "L111"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/MQTT-Publisher__Zugangsdaten
---

# MqttPublisherTests.BrokerDown_FailsFast_WaitsForBackoff_ThenReconnectsAndRepublishesStatus()

## Connections
- [[BrokerUnavailableException]] - `references` [EXTRACTED]
- [[Fact]] - `references` [EXTRACTED]
- [[ManualTime.Advance()]] - `calls` [INFERRED]
- [[MqttPublisher.PublishAsync()]] - `calls` [INFERRED]
- [[MqttPublisherTests]] - `method` [EXTRACTED]
- [[MqttPublisherTests.NewPublisher()]] - `calls` [EXTRACTED]
- [[MqttPublisherTests.StartServerAsync()]] - `calls` [EXTRACTED]
- [[MqttPublisherTests.WaitForAsync()]] - `calls` [EXTRACTED]
- [[Task]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/MQTT-Publisher__Zugangsdaten