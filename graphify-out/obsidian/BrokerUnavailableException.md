---
source_file: "src/ModbusRabbitGateway/Publishing.cs"
type: "code"
community: "MQTT-Publisher & Zugangsdaten"
location: "L15"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/MQTT-Publisher__Zugangsdaten
---

# BrokerUnavailableException

## Connections
- [[Broker-Ausfall einmal gemeldet, Backoff 1–30 s, Messwerte verworfen, Status nachgeholt, Last Will]] - `references` [INFERRED]
- [[Exception]] - `inherits` [EXTRACTED]
- [[FlakyPublisher.PublishAsync()]] - `calls` [INFERRED]
- [[MqttPublisher.EnsureConnectedAsync()]] - `calls` [INFERRED]
- [[MqttPublisher.PublishAsync()]] - `calls` [INFERRED]
- [[MqttPublisherTests.BrokerDown_FailsFast_WaitsForBackoff_ThenReconnectsAndRepublishesStatus()]] - `references` [EXTRACTED]
- [[MqttPublisherTests.WrongPassword_IsBrokerUnavailable_WithReadableMessage()]] - `references` [EXTRACTED]
- [[Publishing.cs]] - `contains` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/MQTT-Publisher__Zugangsdaten