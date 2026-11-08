---
source_file: "src/ModbusRabbitGateway/DevicePoller.cs"
type: "code"
community: "DevicePoller & Gerätetests"
location: "L19"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/DevicePoller__Gerätetests
---

# DevicePoller

## Connections
- [[Bug doppelte offline-Statusnachricht bei wechselndem Fehlertext (behoben in DevicePoller)]] - `references` [EXTRACTED]
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[Device]] - `defines` [EXTRACTED]
- [[DeviceConnection]] - `calls` [INFERRED]
- [[DeviceEndpoint]] - `references` [EXTRACTED]
- [[DevicePoller.Dispose()]] - `method` [EXTRACTED]
- [[DevicePoller.Poll()]] - `method` [EXTRACTED]
- [[DevicePoller.Status()]] - `method` [EXTRACTED]
- [[DevicePoller.cs]] - `contains` [EXTRACTED]
- [[DeviceTracker]] - `references` [EXTRACTED]
- [[FA-GW-02 Register 0-49 in einer Anfrage, nach Registerplan dekodieren]] - `implements` [INFERRED]
- [[FA-GW-03 Pruefen, ob richtiges Geraet antwortet]] - `implements` [INFERRED]
- [[GatewayAgainstDeviceTests]] - `references` [INFERRED]
- [[GatewayAgainstDeviceTests.NewPoller()]] - `references` [EXTRACTED]
- [[GatewayAgainstDeviceTests.Status_IsRepeatedAfterInterval_WithOriginalSince_ForWildcardSubscribers()]] - `calls` [INFERRED]
- [[GatewayService]] - `references` [EXTRACTED]
- [[Gefundener Fehler doppelte offline-Statusnachricht bei wechselndem Fehlertext]] - `conceptually_related_to` [INFERRED]
- [[Geräte-ID-Prüfung Kürzel → Typ (Reg. 0), Nummer → Reg. 1, sonst decodeError]] - `references` [INFERRED]
- [[IDisposable]] - `implements` [EXTRACTED]
- [[ModbusRabbitGateway (Modbus → MQTT → RabbitMQ)]] - `references` [EXTRACTED]
- [[MqttPublisherTests.GatewayService_KeepsPollingDuringBrokerOutage_AndReportsOutageOnce()]] - `calls` [INFERRED]
- [[Nachweis 6a Geräteausfall OFFLINE und Wiederanlauf ONLINE (erfüllt)]] - `references` [INFERRED]
- [[PollOutcome]] - `references` [INFERRED]
- [[QualitySettings]] - `references` [EXTRACTED]
- [[State_3]] - `defines` [EXTRACTED]
- [[TimeSpan]] - `references` [EXTRACTED]
- [[Tracker]] - `defines` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/DevicePoller__Gerätetests