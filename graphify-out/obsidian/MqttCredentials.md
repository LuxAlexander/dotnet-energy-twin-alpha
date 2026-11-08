---
source_file: "src/ModbusRabbitGateway/MqttCredentials.cs"
type: "code"
community: "MQTT-Publisher & Zugangsdaten"
location: "L10"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/MQTT-Publisher__Zugangsdaten
---

# MqttCredentials

## Connections
- [[BrokerSettings]] - `references` [EXTRACTED]
- [[FA-BR-02 Zugangsdaten nur aus docker.env, .env.example]] - `references` [INFERRED]
- [[Gateway-Konfiguration (--device, --interval, --broker, --gateway-id ...)]] - `references` [INFERRED]
- [[Journal Susanne 2026-11-08 (LB03)]] - `references` [EXTRACTED]
- [[MqttCredentials.ReadEnvFile()]] - `method` [EXTRACTED]
- [[MqttCredentials.Resolve()]] - `method` [EXTRACTED]
- [[MqttCredentials.ToString()]] - `method` [EXTRACTED]
- [[MqttCredentials.cs]] - `contains` [EXTRACTED]
- [[MqttCredentialsTests]] - `references` [EXTRACTED]
- [[MqttPublisherTests.NewPublisher()]] - `calls` [INFERRED]
- [[Zugangsdaten-Reihenfolge --mqtt-user → Umgebungsvariablen → docker.env; Passwort nie als Option]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/MQTT-Publisher__Zugangsdaten