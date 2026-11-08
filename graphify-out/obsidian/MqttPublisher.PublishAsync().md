---
source_file: "src/ModbusRabbitGateway/MqttPublisher.cs"
type: "code"
community: "MQTT-Publisher & Zugangsdaten"
location: "L55"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/MQTT-Publisher__Zugangsdaten
---

# MqttPublisher.PublishAsync()

## Connections
- [[BrokerUnavailableException]] - `calls` [INFERRED]
- [[CancellationToken]] - `references` [EXTRACTED]
- [[IMessagePublisher.PublishAsync()]] - `dispatches_to` [INFERRED]
- [[MqttPublisher]] - `method` [EXTRACTED]
- [[MqttPublisher.EnsureConnectedAsync()]] - `calls` [EXTRACTED]
- [[MqttPublisher.ScheduleRetry()]] - `calls` [EXTRACTED]
- [[MqttPublisher.SendAsync()]] - `calls` [EXTRACTED]
- [[MqttPublisherTests.BrokerDown_FailsFast_WaitsForBackoff_ThenReconnectsAndRepublishesStatus()]] - `calls` [INFERRED]
- [[MqttPublisherTests.Publish_SendsWithQos1_RetainOnlyForStatus()]] - `calls` [INFERRED]
- [[MqttPublisherTests.WrongPassword_IsBrokerUnavailable_WithReadableMessage()]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/MQTT-Publisher__Zugangsdaten