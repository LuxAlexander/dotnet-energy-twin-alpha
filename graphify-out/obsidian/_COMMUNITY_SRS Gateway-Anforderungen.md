---
type: community
cohesion: 0.22
members: 15
---

# SRS Gateway-Anforderungen

**Cohesion:** 0.22 - loosely connected
**Members:** 15 nodes

## Members
- [[AK-12 Diagnose-Client funktioniert weiter]] - document - docs/SRS-LB03.md
- [[FA-GW-01 Geraete ueber IDHostPortUnit-ID (--device)]] - document - docs/SRS-LB03.md
- [[FA-GW-02 Register 0-49 in einer Anfrage, nach Registerplan dekodieren]] - document - docs/SRS-LB03.md
- [[FA-GW-03 Pruefen, ob richtiges Geraet antwortet]] - document - docs/SRS-LB03.md
- [[FA-GW-04 Intervall konfigurierbar (--interval)]] - document - docs/SRS-LB03.md
- [[FA-GW-06 Qualitaet ableiten, nie veraltet als good]] - document - docs/SRS-LB03.md
- [[FA-GW-07 In der Pause weiter veroeffentlichen (goodpaused)]] - document - docs/SRS-LB03.md
- [[FA-GW-08 Verbindungs-Dekodierfehler sichtbar, keine Messwerte]] - document - docs/SRS-LB03.md
- [[FA-GW-09 Status bei Aenderung und alle 30 s (--status-repeat)]] - document - docs/SRS-LB03.md
- [[FA-GW-10 Kontrolliert beenden, offline, Last Will]] - document - docs/SRS-LB03.md
- [[FA-GW-11 Ohne Broker betreibbar (--console)]] - document - docs/SRS-LB03.md
- [[Gateway-Pflichten (konfigurierbare Geraete, Dekodierung, Intervall, Simzeit, Fehler, Beenden)]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Offene Punkte (BrokerViewer-Zustaendigkeit, Teamzustimmung, Gesamtlauf 4 Geraete)]] - document - docs/SRS-LB03.md
- [[SRS LB03 - Von Modbus zu RabbitMQ]] - document - docs/SRS-LB03.md
- [[Ziel und Umfang (InfluxDBRESTGrafana nicht im Umfang)]] - document - docs/SRS-LB03.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/SRS_Gateway-Anforderungen
SORT file.name ASC
```

## Connections to other communities
- 8 edges to [[_COMMUNITY_Aufgabe 03 & Nachweise]]
- 6 edges to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 5 edges to [[_COMMUNITY_RabbitMQ unter Docker]]
- 4 edges to [[_COMMUNITY_Qualitätsstatus & Pause]]
- 3 edges to [[_COMMUNITY_LB03-Projektstand]]
- 3 edges to [[_COMMUNITY_Zuständigkeiten LB03]]
- 3 edges to [[_COMMUNITY_Status-Wiederholung & RabbitMQ]]
- 2 edges to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 2 edges to [[_COMMUNITY_Gateway-Optionen]]
- 1 edge to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_DeviceEndpoint]]
- 1 edge to [[_COMMUNITY_GatewayService]]
- 1 edge to [[_COMMUNITY_MessageBuilder]]
- 1 edge to [[_COMMUNITY_Fehlerverhalten Gateway]]

## Top bridge nodes
- [[SRS LB03 - Von Modbus zu RabbitMQ]] - degree 38, connects to 8 communities
- [[Gateway-Pflichten (konfigurierbare Geraete, Dekodierung, Intervall, Simzeit, Fehler, Beenden)]] - degree 13, connects to 3 communities
- [[FA-GW-01 Geraete ueber IDHostPortUnit-ID (--device)]] - degree 4, connects to 2 communities
- [[FA-GW-02 Register 0-49 in einer Anfrage, nach Registerplan dekodieren]] - degree 4, connects to 2 communities
- [[FA-GW-06 Qualitaet ableiten, nie veraltet als good]] - degree 4, connects to 2 communities