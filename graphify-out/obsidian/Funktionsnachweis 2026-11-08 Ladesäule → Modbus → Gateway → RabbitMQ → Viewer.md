---
source_file: "docs/nachweise/2026-11-08-gateway-mit-master/README.md"
type: "document"
community: "Funktionsnachweis mit Master"
tags:
  - graphify/document
  - graphify/EXTRACTED
  - community/Funktionsnachweis_mit_Master
---

# Funktionsnachweis 2026-11-08: Ladesäule → Modbus → Gateway → RabbitMQ → Viewer

## Connections
- [[Beobachtung Neustart der Ladesäule setzt Leistungsgrenze auf 150 kW zurück]] - `references` [EXTRACTED]
- [[Beobachtung einmal verlorener Sollwert (Ursache ungeklärt, 410 Versuche ohne Wiederholung)]] - `references` [EXTRACTED]
- [[DualCharger]] - `references` [INFERRED]
- [[GatewayService]] - `references` [INFERRED]
- [[N1 Nachweise 1–7 (nicht erfüllt zum Zeitpunkt des Berichts)]] - `conceptually_related_to` [INFERRED]
- [[Nachweis 1 RabbitMQ aus Compose gestartet (erfüllt)]] - `references` [EXTRACTED]
- [[Nachweis 2 TimeSim, WeatherSim und Ladesäule DC-01 synchron (erfüllt)]] - `references` [EXTRACTED]
- [[Nachweis 3 Gateway und Viewer, Messwerte jede Sekunde (erfüllt)]] - `references` [EXTRACTED]
- [[Nachweis 4 Sollwert-Kette 200 kW angenommen, 999 kW abgelehnt (erfüllt)]] - `references` [EXTRACTED]
- [[Nachweis 5 PauseFortsetzen ohne Zeitsprung, Veröffentlichung goodpaused (erfüllt)]] - `references` [EXTRACTED]
- [[Nachweis 6a Geräteausfall OFFLINE und Wiederanlauf ONLINE (erfüllt)]] - `references` [EXTRACTED]
- [[Nachweis 6b Broker-Ausfall, 10 Nachrichten verworfen, Status nachgeholt (erfüllt)]] - `references` [EXTRACTED]
- [[Nachweis 7 Kontrolliertes Beenden (teilweise, taskkill ohne Terminal)]] - `references` [EXTRACTED]
- [[RabbitMQ 4.4.0 (Docker Compose, MQTT-Plugin)]] - `references` [EXTRACTED]
- [[SRS-LB03 Akzeptanzkriterien (docsSRS-LB03.md)]] - `references` [EXTRACTED]
- [[Skript-Master (MasterClock + TimeSyncServer, fester Pausenplan)]] - `references` [EXTRACTED]
- [[Viewer (Test-Empfänger, MQTTnet, Abo energytwin)]] - `references` [EXTRACTED]
- [[Zwei ungültige Vorläufe (JSON-Reflexion deaktiviert  Port 5100 belegt)]] - `references` [EXTRACTED]

#graphify/document #graphify/EXTRACTED #community/Funktionsnachweis_mit_Master