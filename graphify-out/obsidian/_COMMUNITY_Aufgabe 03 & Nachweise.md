---
type: community
cohesion: 0.13
members: 21
---

# Aufgabe 03 & Nachweise

**Cohesion:** 0.13 - loosely connected
**Members:** 21 nodes

## Members
- [[AK-03 Alle vier Geraete liefern an Viewer (nur Ladesaeule nachgewiesen)]] - document - docs/SRS-LB03.md
- [[AK-05 Sollwert in Simulator, Modbus und Nachricht sichtbar]] - document - docs/SRS-LB03.md
- [[AK-06 Pause simTime steht, paused; kein Zeitsprung]] - document - docs/SRS-LB03.md
- [[AK-07 Geraeteausfall - offline, Wiederanlauf ohne Gateway-Neustart]] - document - docs/SRS-LB03.md
- [[AK-08 Broker-Ausfall kein Absturz, Wiederverbindung]] - document - docs/SRS-LB03.md
- [[Abgabe annotierter Tag lb03-2026]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Aufgabe 03 Von Modbus zu RabbitMQ (Aufgabenblatt)]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Befund einmal verlorener Sollwert, in 410 Wiederholungen nicht reproduzierbar]] - document - docs/development-journal/susanne-lastowicka.md
- [[Bewertungskriterien (Datenweg, Vertrag, Docker, Robustheit, KI-Arbeitsweise)]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Datenweg Geraetesimulator - Gateway - RabbitMQ - Viewer]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Funktionsnachweis mit Master (docsnachweise2026-11-08-gateway-mit-master)]] - document - docs/development-journal/susanne-lastowicka.md
- [[Mindestanforderungen (.NET 10, async, sichtbare Fehler, keine Secrets)]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Nachweis 1 RabbitMQ aus versionierter Compose-Konfiguration starten]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Nachweis 2 TimeSim, WeatherSim und Geraetesimulatoren starten]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Nachweis 3 Gateway und Viewer starten, Nachrichten aller Geraete sichtbar]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Nachweis 4 GeraetezustandSollwert aendern, Wirkung in Simulator, Modbus und Nachricht]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Nachweis 5 PauseFortsetzen, richtige virtuelle Zeit, Veroeffentlichung in der Pause dokumentieren]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Nachweis 6 Verbindung zu Geraet und Broker unterbrechen und wiederherstellen]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Optional WetterTimeSim ueber RabbitMQ]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[SRS vor Implementierung (docsSRS-LB03.md)]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Skript-Master statt Master-Konsole (umgeleitetes Terminal)]] - document - docs/development-journal/susanne-lastowicka.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Aufgabe_03__Nachweise
SORT file.name ASC
```

## Connections to other communities
- 8 edges to [[_COMMUNITY_SRS Gateway-Anforderungen]]
- 4 edges to [[_COMMUNITY_Status-Wiederholung & RabbitMQ]]
- 3 edges to [[_COMMUNITY_LB03-Projektstand]]
- 3 edges to [[_COMMUNITY_RabbitMQ unter Docker]]
- 3 edges to [[_COMMUNITY_Qualitätsstatus & Pause]]
- 2 edges to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 2 edges to [[_COMMUNITY_Zuständigkeiten LB03]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_MasterClock]]
- 1 edge to [[_COMMUNITY_DualCharger]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 1 edge to [[_COMMUNITY_Fehlerverhalten Gateway]]

## Top bridge nodes
- [[Aufgabe 03 Von Modbus zu RabbitMQ (Aufgabenblatt)]] - degree 26, connects to 8 communities
- [[Funktionsnachweis mit Master (docsnachweise2026-11-08-gateway-mit-master)]] - degree 12, connects to 3 communities
- [[Befund einmal verlorener Sollwert, in 410 Wiederholungen nicht reproduzierbar]] - degree 5, connects to 3 communities
- [[Skript-Master statt Master-Konsole (umgeleitetes Terminal)]] - degree 3, connects to 2 communities
- [[AK-08 Broker-Ausfall kein Absturz, Wiederverbindung]] - degree 3, connects to 2 communities