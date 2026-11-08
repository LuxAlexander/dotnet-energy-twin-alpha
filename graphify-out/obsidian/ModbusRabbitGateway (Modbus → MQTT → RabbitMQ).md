---
source_file: "src/ModbusRabbitGateway/README.md"
type: "document"
community: "Gateway-Publisher"
tags:
  - graphify/document
  - graphify/EXTRACTED
  - community/Gateway-Publisher
---

# ModbusRabbitGateway (Modbus → MQTT → RabbitMQ)

## Connections
- [[A10 Gateway-Anforderungen (nicht erfüllt; wiederverwendbar DeviceConnection, RegisterPlans, RegisterCodec)]] - `conceptually_related_to` [INFERRED]
- [[Bekannte Probleme keine atomare Lesung, sequentielles Lesen, Compose mit festen Zugangsdaten]] - `references` [EXTRACTED]
- [[Broker-Ausfall einmal gemeldet, Backoff 1–30 s, Messwerte verworfen, Status nachgeholt, Last Will]] - `references` [EXTRACTED]
- [[BrokerSettings]] - `references` [EXTRACTED]
- [[ConsolePublisher]] - `references` [EXTRACTED]
- [[DevicePoller]] - `references` [EXTRACTED]
- [[GatewayService]] - `references` [EXTRACTED]
- [[Geräte-ID-Prüfung Kürzel → Typ (Reg. 0), Nummer → Reg. 1, sonst decodeError]] - `references` [EXTRACTED]
- [[IMessagePublisher]] - `references` [EXTRACTED]
- [[KeyCatalog]] - `references` [EXTRACTED]
- [[MeasurementKeys]] - `references` [EXTRACTED]
- [[MeasurementMessage]] - `references` [EXTRACTED]
- [[MessageBuilder]] - `references` [EXTRACTED]
- [[MessageJson]] - `references` [EXTRACTED]
- [[Modbus.Client (gemeinsamer Modbus-TCP-Client)]] - `references` [EXTRACTED]
- [[ModbusRabbitGateway.Tests (5252 am 2026-11-08)]] - `references` [EXTRACTED]
- [[Nachrichtenvertrag LB03 (docsNachrichtenvertrag-LB03.md)]] - `references` [EXTRACTED]
- [[Optionen --device, --interval, --stale-after, --broker, --status-repeat, --console, --once, --json, --keys]] - `references` [EXTRACTED]
- [[RabbitMQ retained nur bei exaktem Topic, nicht bei Wildcard-Abos]] - `references` [EXTRACTED]
- [[TeePublisher]] - `references` [EXTRACTED]
- [[Topics energytwindevices{id}measurementsstatus, energytwingateway{id}status]] - `references` [EXTRACTED]
- [[Zugangsdaten-Reihenfolge --mqtt-user → Umgebungsvariablen → docker.env; Passwort nie als Option]] - `references` [EXTRACTED]

#graphify/document #graphify/EXTRACTED #community/Gateway-Publisher