---
type: community
cohesion: 0.50
members: 4
---

# Fehlerverhalten Gateway

**Cohesion:** 0.50 - moderately connected
**Members:** 4 nodes

## Members
- [[Bekannte Einschraenkungen (keine atomare Lesung, Sekundenaufloesung, sequentiell)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Broker-Ausfall verwerfen, Neuversuch 1-30 s, Status nachholen]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Keine atomare Lese-Garantie verlangt]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Verhalten bei Fehlern (offline, decodeError, Broker nicht erreichbar)]] - document - docs/SRS-LB03.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Fehlerverhalten_Gateway
SORT file.name ASC
```

## Connections to other communities
- 2 edges to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 1 edge to [[_COMMUNITY_Aufgabe 03 & Nachweise]]
- 1 edge to [[_COMMUNITY_SRS Gateway-Anforderungen]]
- 1 edge to [[_COMMUNITY_Status-Wiederholung & RabbitMQ]]

## Top bridge nodes
- [[Broker-Ausfall verwerfen, Neuversuch 1-30 s, Status nachholen]] - degree 4, connects to 2 communities
- [[Verhalten bei Fehlern (offline, decodeError, Broker nicht erreichbar)]] - degree 3, connects to 2 communities
- [[Bekannte Einschraenkungen (keine atomare Lesung, Sekundenaufloesung, sequentiell)]] - degree 3, connects to 1 community
- [[Keine atomare Lese-Garantie verlangt]] - degree 2, connects to 1 community