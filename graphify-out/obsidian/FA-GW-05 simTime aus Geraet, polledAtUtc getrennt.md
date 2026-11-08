---
source_file: "docs/SRS-LB03.md"
type: "document"
community: "Nachrichtenvertrag & Messwerte"
tags:
  - graphify/document
  - graphify/EXTRACTED
  - community/Nachrichtenvertrag__Messwerte
---

# FA-GW-05: simTime aus Geraet, polledAtUtc getrennt

## Connections
- [[Gateway-Pflichten (konfigurierbare Geraete, Dekodierung, Intervall, Simzeit, Fehler, Beenden)]] - `implements` [INFERRED]
- [[SRS LB03 - Von Modbus zu RabbitMQ]] - `references` [EXTRACTED]
- [[Virtuelle Zeit vs. reale Abfragezeit nicht verwechseln]] - `implements` [INFERRED]
- [[polledAtUtc (reale Abfragezeit, nur Diagnose)]] - `references` [EXTRACTED]
- [[simTime  simTimeEpoch (virtuelle Zeit, Register 8-9)]] - `references` [EXTRACTED]

#graphify/document #graphify/EXTRACTED #community/Nachrichtenvertrag__Messwerte