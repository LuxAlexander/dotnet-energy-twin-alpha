---
source_file: "tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs"
type: "code"
community: "MQTT-Publisher & Zugangsdaten"
location: "L60"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/MQTT-Publisher__Zugangsdaten
---

# MqttPublisherTests.NewPublisher()

## Connections
- [[BrokerSettings]] - `calls` [INFERRED]
- [[MqttCredentials]] - `calls` [INFERRED]
- [[MqttPublisher]] - `references` [EXTRACTED]
- [[MqttPublisherTests]] - `method` [EXTRACTED]
- [[MqttPublisherTests.BrokerDown_FailsFast_WaitsForBackoff_ThenReconnectsAndRepublishesStatus()]] - `calls` [EXTRACTED]
- [[MqttPublisherTests.Publish_SendsWithQos1_RetainOnlyForStatus()]] - `calls` [EXTRACTED]
- [[MqttPublisherTests.WrongPassword_IsBrokerUnavailable_WithReadableMessage()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/MQTT-Publisher__Zugangsdaten