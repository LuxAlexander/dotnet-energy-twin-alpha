---
type: community
cohesion: 0.22
members: 9
---

# Qualitätsstatus & Pause

**Cohesion:** 0.22 - loosely connected
**Members:** 9 nodes

## Members
- [[AK-09 Ohne Master nicht good (badmasterLost)]] - document - docs/SRS-LB03.md
- [[AK-11 Alle Programme kontrolliert beendbar (teilweise)]] - document - docs/SRS-LB03.md
- [[Entscheidung in der Pause weiter veroeffentlichen (paused)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Nachweis 7 Alle Anwendungen und Broker kontrolliert beenden]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Qualitaetsstatus gooduncertainbad]] - document - docs/Nachrichtenvertrag-LB03.md
- [[bad masterLost  noTime]] - document - docs/Nachrichtenvertrag-LB03.md
- [[good running  paused]] - document - docs/Nachrichtenvertrag-LB03.md
- [[stale-Regel reale Zeit x Tempo  --stale-after (60 min Modellzeit)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[uncertain standalone  stale  invalidValue]] - document - docs/Nachrichtenvertrag-LB03.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Qualitätsstatus__Pause
SORT file.name ASC
```

## Connections to other communities
- 4 edges to [[_COMMUNITY_SRS Gateway-Anforderungen]]
- 3 edges to [[_COMMUNITY_Aufgabe 03 & Nachweise]]
- 3 edges to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource]]
- 1 edge to [[_COMMUNITY_MessageBuilder]]

## Top bridge nodes
- [[Qualitaetsstatus gooduncertainbad]] - degree 8, connects to 4 communities
- [[Entscheidung in der Pause weiter veroeffentlichen (paused)]] - degree 4, connects to 3 communities
- [[Nachweis 7 Alle Anwendungen und Broker kontrolliert beenden]] - degree 4, connects to 1 community
- [[AK-09 Ohne Master nicht good (badmasterLost)]] - degree 3, connects to 1 community
- [[AK-11 Alle Programme kontrolliert beendbar (teilweise)]] - degree 2, connects to 1 community