---
type: community
cohesion: 0.14
members: 16
---

# Funktionsnachweis mit Master

**Cohesion:** 0.14 - loosely connected
**Members:** 16 nodes

## Members
- [[Beobachtung Neustart der Ladesäule setzt Leistungsgrenze auf 150 kW zurück]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Beobachtung einmal verlorener Sollwert (Ursache ungeklärt, 410 Versuche ohne Wiederholung)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Funktionsnachweis 2026-11-08 Ladesäule → Modbus → Gateway → RabbitMQ → Viewer]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Nachweis 1 RabbitMQ aus Compose gestartet (erfüllt)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Nachweis 2 TimeSim, WeatherSim und Ladesäule DC-01 synchron (erfüllt)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Nachweis 3 Gateway und Viewer, Messwerte jede Sekunde (erfüllt)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Nachweis 4 Sollwert-Kette 200 kW angenommen, 999 kW abgelehnt (erfüllt)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Nachweis 5 PauseFortsetzen ohne Zeitsprung, Veröffentlichung goodpaused (erfüllt)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Nachweis 6a Geräteausfall OFFLINE und Wiederanlauf ONLINE (erfüllt)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Nachweis 6b Broker-Ausfall, 10 Nachrichten verworfen, Status nachgeholt (erfüllt)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Nachweis 7 Kontrolliertes Beenden (teilweise, taskkill ohne Terminal)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[RabbitMQ 4.4.0 (Docker Compose, MQTT-Plugin)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[SRS-LB03 Akzeptanzkriterien (docsSRS-LB03.md)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Skript-Master (MasterClock + TimeSyncServer, fester Pausenplan)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Viewer (Test-Empfänger, MQTTnet, Abo energytwin)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md
- [[Zwei ungültige Vorläufe (JSON-Reflexion deaktiviert  Port 5100 belegt)]] - document - docs/nachweise/2026-11-08-gateway-mit-master/README.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Funktionsnachweis_mit_Master
SORT file.name ASC
```

## Connections to other communities
- 2 edges to [[_COMMUNITY_DualCharger]]
- 2 edges to [[_COMMUNITY_Aufgabenabgleich (readTask)]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_MasterClock]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 1 edge to [[_COMMUNITY_GatewayService]]
- 1 edge to [[_COMMUNITY_MessageBuilder]]
- 1 edge to [[_COMMUNITY_Gateway-Publisher]]
- 1 edge to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]

## Top bridge nodes
- [[Funktionsnachweis 2026-11-08 Ladesäule → Modbus → Gateway → RabbitMQ → Viewer]] - degree 18, connects to 3 communities
- [[Nachweis 4 Sollwert-Kette 200 kW angenommen, 999 kW abgelehnt (erfüllt)]] - degree 3, connects to 2 communities
- [[Skript-Master (MasterClock + TimeSyncServer, fester Pausenplan)]] - degree 3, connects to 2 communities
- [[RabbitMQ 4.4.0 (Docker Compose, MQTT-Plugin)]] - degree 3, connects to 1 community
- [[Beobachtung Neustart der Ladesäule setzt Leistungsgrenze auf 150 kW zurück]] - degree 2, connects to 1 community