---
type: community
cohesion: 0.09
members: 25
---

# Aufgabenabgleich (readTask)

**Cohesion:** 0.09 - loosely connected
**Members:** 25 nodes

## Members
- [[A1 Datenweg Gerät → Modbus → Gateway → RabbitMQ → Viewer (nicht erfüllt)]] - document - docs/task-status/aufgabe-03.md
- [[A12 Nachrichtenvertrag mit Topics und Beispielnachricht (nicht erfüllt)]] - document - docs/task-status/aufgabe-03.md
- [[A13 Virtuelle Zeit und Abfragezeit getrennt (nicht erfüllt)]] - document - docs/task-status/aufgabe-03.md
- [[A14 Viewer zeigt alle Geräte (nicht erfüllt)]] - document - docs/task-status/aufgabe-03.md
- [[A15 AGENTS.md aktuell, Regeln für generierte Dateien (teilweise)]] - document - docs/task-status/aufgabe-03.md
- [[A16 Ein Skill pro Person (teilweise nur readTask)]] - document - docs/task-status/aufgabe-03.md
- [[A2 Zwei Simulatoren pro Person im README (erfüllt)]] - document - docs/task-status/aufgabe-03.md
- [[A5 SRS docsSRS-LB03.md (nicht erfüllt)]] - document - docs/task-status/aufgabe-03.md
- [[A7 Compose dokumentiert (teilweise Image ohne Version, rabbitmq.conf leerer Ordner)]] - document - docs/task-status/aufgabe-03.md
- [[A8 AMQP oder MQTT gewählt und begründet (teilweise)]] - document - docs/task-status/aufgabe-03.md
- [[A9 Keine Zugangsdaten versioniert, .env.example (nicht erfüllt)]] - document - docs/task-status/aufgabe-03.md
- [[Aktivierung skillsreadTask nach .claudeskills kopieren]] - document - skills/readTask/README.md
- [[Aufgabenabgleich Aufgabe 03 – Von Modbus zu RabbitMQ (Stand 2026-11-08)]] - document - docs/task-status/aufgabe-03.md
- [[Aufruf readTask Aufgabe 03 (ohne Argument neueste Aufgaben-PDF)]] - document - skills/readTask/SKILL.md
- [[Festes Ausgabeformat des Berichts]] - document - skills/readTask/SKILL.md
- [[G1 Annotierter Tag lb03-2026 (nicht erfüllt)]] - document - docs/task-status/aufgabe-03.md
- [[Grenzen nur lesend, kein BuildTestDockerCommit, keine Secrets, .env nie lesen]] - document - skills/readTask/SKILL.md
- [[Hinweise rabbitmq.conf leerer Ordner, Passwort im Repo, graphify-out versioniert, AGENTS.md veraltet]] - document - docs/task-status/aufgabe-03.md
- [[M1M2 .NET 10 baubar, LB02-Funktionen erhalten (nicht nachgewiesen  erfüllt)]] - document - docs/task-status/aufgabe-03.md
- [[M5 Broker-Ausfall ohne Absturz, Wiederanlauf dokumentiert (nicht erfüllt)]] - document - docs/task-status/aufgabe-03.md
- [[N1 Nachweise 1–7 (nicht erfüllt zum Zeitpunkt des Berichts)]] - document - docs/task-status/aufgabe-03.md
- [[Nutzen prüfbare Checkliste, Code ≠ Nachweis]] - document - skills/readTask/README.md
- [[Skill readTask (Aufgabenblatt lesen, Stand abgleichen, Reihenfolge vorschlagen)]] - document - skills/readTask/SKILL.md
- [[Verantwortlich Susanne Lastowicka]] - document - skills/readTask/README.md
- [[Vorgeschlagene Reihenfolge SRS → Broker → Gateway → PublisherRobustheit → Viewer → Skills → Doku → Nachweise → Abgabe]] - document - docs/task-status/aufgabe-03.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Aufgabenabgleich_readTask
SORT file.name ASC
```

## Connections to other communities
- 3 edges to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]
- 2 edges to [[_COMMUNITY_Funktionsnachweis mit Master]]
- 2 edges to [[_COMMUNITY_readTask-Arbeitsschritte]]
- 2 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 1 edge to [[_COMMUNITY_MessageBuilder]]
- 1 edge to [[_COMMUNITY_Gateway-Publisher]]

## Top bridge nodes
- [[Aufgabenabgleich Aufgabe 03 – Von Modbus zu RabbitMQ (Stand 2026-11-08)]] - degree 24, connects to 2 communities
- [[Skill readTask (Aufgabenblatt lesen, Stand abgleichen, Reihenfolge vorschlagen)]] - degree 9, connects to 1 community
- [[A12 Nachrichtenvertrag mit Topics und Beispielnachricht (nicht erfüllt)]] - degree 2, connects to 1 community
- [[A13 Virtuelle Zeit und Abfragezeit getrennt (nicht erfüllt)]] - degree 2, connects to 1 community
- [[A5 SRS docsSRS-LB03.md (nicht erfüllt)]] - degree 2, connects to 1 community