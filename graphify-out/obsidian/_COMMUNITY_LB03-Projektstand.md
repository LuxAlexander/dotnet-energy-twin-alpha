---
type: community
cohesion: 0.22
members: 10
---

# LB03-Projektstand

**Cohesion:** 0.22 - loosely connected
**Members:** 10 nodes

## Members
- [[AGENTS.md Stand LB03 (258 Tests, Gateway 5252)]] - document - AGENTS.md
- [[AK-01 Build 0 Warnungen, alle Tests gruen (258258)]] - document - docs/SRS-LB03.md
- [[Commit Gateway (450f489), Push gescheitert]] - document - docs/development-journal/susanne-lastowicka.md
- [[Eigener Skill pro Person (mit Anwendungsnachweis)]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Modbus.Client (Bibliothek)]] - document - README.md
- [[ModbusRabbitGateway (Projekt)]] - document - README.md
- [[Projektanweisungen AGENTS.mdCLAUDE]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Skill readTask]] - document - README.md
- [[Susanne Lastowicka]] - document - README.md
- [[readTask-Lauf Aufgabe 03 (28 Anforderungen, docstask-statusaufgabe-03.md)]] - document - docs/development-journal/susanne-lastowicka.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/LB03-Projektstand
SORT file.name ASC
```

## Connections to other communities
- 3 edges to [[_COMMUNITY_Aufgabe 03 & Nachweise]]
- 3 edges to [[_COMMUNITY_SRS Gateway-Anforderungen]]
- 2 edges to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 2 edges to [[_COMMUNITY_Status-Wiederholung & RabbitMQ]]
- 1 edge to [[_COMMUNITY_DeviceConnection (Modbus.Client)]]
- 1 edge to [[_COMMUNITY_GatewayService]]
- 1 edge to [[_COMMUNITY_Zuständigkeiten LB03]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 1 edge to [[_COMMUNITY_RabbitMQ unter Docker]]

## Top bridge nodes
- [[AGENTS.md Stand LB03 (258 Tests, Gateway 5252)]] - degree 6, connects to 3 communities
- [[readTask-Lauf Aufgabe 03 (28 Anforderungen, docstask-statusaufgabe-03.md)]] - degree 4, connects to 3 communities
- [[ModbusRabbitGateway (Projekt)]] - degree 6, connects to 2 communities
- [[Susanne Lastowicka]] - degree 5, connects to 2 communities
- [[Modbus.Client (Bibliothek)]] - degree 3, connects to 1 community