---
type: community
cohesion: 0.23
members: 12
---

# Status-Wiederholung & RabbitMQ

**Cohesion:** 0.23 - loosely connected
**Members:** 12 nodes

## Members
- [[AK-10 Spaeter Viewer mit Wildcard sieht Ausfall nach 30 s]] - document - docs/SRS-LB03.md
- [[Befund Meldung Anmeldung abgelehnt irrefuehrend bei Container-Neustart]] - document - docs/development-journal/susanne-lastowicka.md
- [[Befund retained nicht an Wildcard-Abos - Status-Wiederholung]] - document - docs/development-journal/susanne-lastowicka.md
- [[Bug doppelte offline-Statusnachricht bei wechselndem Fehlertext (behoben in DevicePoller)]] - document - docs/development-journal/susanne-lastowicka.md
- [[DeviceStatusMessage]] - code - src/ModbusRabbitGateway/Messages.cs
- [[Entwicklungsjournal fortfuehren]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Journal Susanne 2026-11-08 (LB03)]] - document - docs/development-journal/susanne-lastowicka.md
- [[Probelauf gegen RabbitMQ 4.4.0 unter Docker]] - document - docs/development-journal/susanne-lastowicka.md
- [[RabbitMQ liefert retained nicht an Wildcard-Abos]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Status-Wiederholung alle 30 s (--status-repeat)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Statusnachricht je Geraet (onlineofflinedecodeError)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Topic energytwindevices{deviceId}status (retained)]] - document - docs/Nachrichtenvertrag-LB03.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Status-Wiederholung__RabbitMQ
SORT file.name ASC
```

## Connections to other communities
- 4 edges to [[_COMMUNITY_Aufgabe 03 & Nachweise]]
- 3 edges to [[_COMMUNITY_Nachrichtentypen (Contract)]]
- 3 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 3 edges to [[_COMMUNITY_SRS Gateway-Anforderungen]]
- 3 edges to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 2 edges to [[_COMMUNITY_GatewayService]]
- 2 edges to [[_COMMUNITY_LB03-Projektstand]]
- 1 edge to [[_COMMUNITY_DeviceConnection (Modbus.Client)]]
- 1 edge to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 1 edge to [[_COMMUNITY_OutputPaths]]
- 1 edge to [[_COMMUNITY_Fehlerverhalten Gateway]]

## Top bridge nodes
- [[Journal Susanne 2026-11-08 (LB03)]] - degree 13, connects to 5 communities
- [[Statusnachricht je Geraet (onlineofflinedecodeError)]] - degree 6, connects to 3 communities
- [[Probelauf gegen RabbitMQ 4.4.0 unter Docker]] - degree 5, connects to 2 communities
- [[Status-Wiederholung alle 30 s (--status-repeat)]] - degree 5, connects to 2 communities
- [[DeviceStatusMessage]] - degree 5, connects to 2 communities