---
source_file: "docs/SRS-LB03.md"
type: "document"
community: "SRS Gateway-Anforderungen"
tags:
  - graphify/document
  - graphify/EXTRACTED
  - community/SRS_Gateway-Anforderungen
---

# SRS LB03 - Von Modbus zu RabbitMQ

## Connections
- [[AGENTS.md Stand LB03 (258 Tests, Gateway 5252)]] - `references` [EXTRACTED]
- [[AK-01 Build 0 Warnungen, alle Tests gruen (258258)]] - `references` [EXTRACTED]
- [[AK-02 RabbitMQ aus Compose ohne Zugangsdaten im Repo (offen Passwort fest in Compose)]] - `references` [EXTRACTED]
- [[AK-03 Alle vier Geraete liefern an Viewer (nur Ladesaeule nachgewiesen)]] - `references` [EXTRACTED]
- [[AK-04 Messwertnachricht enthaelt DeviceId, Zeit, Sequenz, Qualitaet, Einheit]] - `references` [EXTRACTED]
- [[AK-05 Sollwert in Simulator, Modbus und Nachricht sichtbar]] - `references` [EXTRACTED]
- [[AK-06 Pause simTime steht, paused; kein Zeitsprung]] - `references` [EXTRACTED]
- [[AK-07 Geraeteausfall - offline, Wiederanlauf ohne Gateway-Neustart]] - `references` [EXTRACTED]
- [[AK-08 Broker-Ausfall kein Absturz, Wiederverbindung]] - `references` [EXTRACTED]
- [[AK-09 Ohne Master nicht good (badmasterLost)]] - `references` [EXTRACTED]
- [[AK-10 Spaeter Viewer mit Wildcard sieht Ausfall nach 30 s]] - `references` [EXTRACTED]
- [[AK-11 Alle Programme kontrolliert beendbar (teilweise)]] - `references` [EXTRACTED]
- [[AK-12 Diagnose-Client funktioniert weiter]] - `references` [EXTRACTED]
- [[Entscheidung MQTT (2026-11-08)]] - `references` [EXTRACTED]
- [[FA-BR-01 RabbitMQ aus Compose, Image-Version, Ports 1883567215672]] - `references` [EXTRACTED]
- [[FA-BR-02 Zugangsdaten nur aus docker.env, .env.example]] - `references` [EXTRACTED]
- [[FA-BR-03 StartStatusStopp dokumentiert]] - `references` [EXTRACTED]
- [[FA-GW-01 Geraete ueber IDHostPortUnit-ID (--device)]] - `references` [EXTRACTED]
- [[FA-GW-02 Register 0-49 in einer Anfrage, nach Registerplan dekodieren]] - `references` [EXTRACTED]
- [[FA-GW-03 Pruefen, ob richtiges Geraet antwortet]] - `references` [EXTRACTED]
- [[FA-GW-04 Intervall konfigurierbar (--interval)]] - `references` [EXTRACTED]
- [[FA-GW-05 simTime aus Geraet, polledAtUtc getrennt]] - `references` [EXTRACTED]
- [[FA-GW-06 Qualitaet ableiten, nie veraltet als good]] - `references` [EXTRACTED]
- [[FA-GW-07 In der Pause weiter veroeffentlichen (goodpaused)]] - `references` [EXTRACTED]
- [[FA-GW-08 Verbindungs-Dekodierfehler sichtbar, keine Messwerte]] - `references` [EXTRACTED]
- [[FA-GW-09 Status bei Aenderung und alle 30 s (--status-repeat)]] - `references` [EXTRACTED]
- [[FA-GW-10 Kontrolliert beenden, offline, Last Will]] - `references` [EXTRACTED]
- [[FA-GW-11 Ohne Broker betreibbar (--console)]] - `references` [EXTRACTED]
- [[FA-VW-01 Empfang aller Geraete ueber RabbitMQ, Abo energytwin]] - `references` [EXTRACTED]
- [[FA-VW-02 Anzeige DeviceId, Simzeit, Messwerte, seq, Qualitaet]] - `references` [EXTRACTED]
- [[FA-VW-03 Verbindung und Abo reproduzierbar dokumentiert]] - `references` [EXTRACTED]
- [[Funktionsnachweis mit Master (docsnachweise2026-11-08-gateway-mit-master)]] - `references` [EXTRACTED]
- [[Nachrichtenvertrag LB03 (Schemaversion 1)]] - `references` [EXTRACTED]
- [[Nicht zugeteilt SRS, BrokerDocker, Viewer, Messwerte PVBatterieKuehlhaus, Nachweise, Skills AlexanderDominik]] - `references` [EXTRACTED]
- [[Offene Punkte (BrokerViewer-Zustaendigkeit, Teamzustimmung, Gesamtlauf 4 Geraete)]] - `references` [EXTRACTED]
- [[SRS vor Implementierung (docsSRS-LB03.md)]] - `implements` [EXTRACTED]
- [[Verhalten bei Fehlern (offline, decodeError, Broker nicht erreichbar)]] - `references` [EXTRACTED]
- [[Ziel und Umfang (InfluxDBRESTGrafana nicht im Umfang)]] - `references` [EXTRACTED]

#graphify/document #graphify/EXTRACTED #community/SRS_Gateway-Anforderungen