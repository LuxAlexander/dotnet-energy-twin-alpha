---
type: community
cohesion: 0.22
members: 11
---

# RabbitMQ unter Docker

**Cohesion:** 0.22 - loosely connected
**Members:** 11 nodes

## Members
- [[dot-env.example statt versionierter Zugangsdaten]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[AK-02 RabbitMQ aus Compose ohne Zugangsdaten im Repo (offen Passwort fest in Compose)]] - document - docs/SRS-LB03.md
- [[Befund Passwort fest in Compose, rabbitmq.conf leerer Ordner, Image ohne Version]] - document - docs/development-journal/susanne-lastowicka.md
- [[Entscheidung MQTT (2026-11-08)]] - document - docs/SRS-LB03.md
- [[FA-BR-01 RabbitMQ aus Compose, Image-Version, Ports 1883567215672]] - document - docs/SRS-LB03.md
- [[FA-BR-02 Zugangsdaten nur aus docker.env, .env.example]] - document - docs/SRS-LB03.md
- [[FA-BR-03 StartStatusStopp dokumentiert]] - document - docs/SRS-LB03.md
- [[Offene Frage 9 AMQP oder MQTT - MQTT entschieden]] - document - AGENTS.md
- [[Protokollwahl AMQP oder MQTT begruenden]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[RabbitMQ unter Docker (Compose, Image-Version, Ports, Plugins)]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[dockerdocker-compose.yml (RabbitMQ)]] - document - README.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/RabbitMQ_unter_Docker
SORT file.name ASC
```

## Connections to other communities
- 5 edges to [[_COMMUNITY_SRS Gateway-Anforderungen]]
- 3 edges to [[_COMMUNITY_Aufgabe 03 & Nachweise]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 1 edge to [[_COMMUNITY_Zuständigkeiten LB03]]
- 1 edge to [[_COMMUNITY_LB03-Projektstand]]

## Top bridge nodes
- [[FA-BR-02 Zugangsdaten nur aus docker.env, .env.example]] - degree 5, connects to 2 communities
- [[RabbitMQ unter Docker (Compose, Image-Version, Ports, Plugins)]] - degree 6, connects to 1 community
- [[Entscheidung MQTT (2026-11-08)]] - degree 4, connects to 1 community
- [[dockerdocker-compose.yml (RabbitMQ)]] - degree 4, connects to 1 community
- [[dot-env.example statt versionierter Zugangsdaten]] - degree 3, connects to 1 community