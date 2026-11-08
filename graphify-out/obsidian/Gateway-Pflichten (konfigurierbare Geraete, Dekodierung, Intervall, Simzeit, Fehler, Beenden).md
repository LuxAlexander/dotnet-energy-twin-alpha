---
source_file: "dotNET Programmierung 2026 - Aufgabe 03.pdf"
type: "paper"
community: "SRS Gateway-Anforderungen"
tags:
  - graphify/paper
  - graphify/INFERRED
  - community/SRS_Gateway-Anforderungen
---

# Gateway-Pflichten (konfigurierbare Geraete, Dekodierung, Intervall, Simzeit, Fehler, Beenden)

## Connections
- [[Aufgabe 03 Von Modbus zu RabbitMQ (Aufgabenblatt)]] - `references` [EXTRACTED]
- [[FA-GW-01 Geraete ueber IDHostPortUnit-ID (--device)]] - `implements` [INFERRED]
- [[FA-GW-02 Register 0-49 in einer Anfrage, nach Registerplan dekodieren]] - `implements` [INFERRED]
- [[FA-GW-03 Pruefen, ob richtiges Geraet antwortet]] - `implements` [INFERRED]
- [[FA-GW-04 Intervall konfigurierbar (--interval)]] - `implements` [INFERRED]
- [[FA-GW-05 simTime aus Geraet, polledAtUtc getrennt]] - `implements` [INFERRED]
- [[FA-GW-06 Qualitaet ableiten, nie veraltet als good]] - `implements` [INFERRED]
- [[FA-GW-07 In der Pause weiter veroeffentlichen (goodpaused)]] - `implements` [INFERRED]
- [[FA-GW-08 Verbindungs-Dekodierfehler sichtbar, keine Messwerte]] - `implements` [INFERRED]
- [[FA-GW-09 Status bei Aenderung und alle 30 s (--status-repeat)]] - `implements` [INFERRED]
- [[FA-GW-10 Kontrolliert beenden, offline, Last Will]] - `implements` [INFERRED]
- [[FA-GW-11 Ohne Broker betreibbar (--console)]] - `implements` [INFERRED]
- [[ModbusRabbitGateway (Projekt)]] - `implements` [INFERRED]

#graphify/paper #graphify/INFERRED #community/SRS_Gateway-Anforderungen