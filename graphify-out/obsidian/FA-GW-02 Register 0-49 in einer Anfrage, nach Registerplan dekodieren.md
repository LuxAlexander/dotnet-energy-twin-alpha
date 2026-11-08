---
source_file: "docs/SRS-LB03.md"
type: "document"
community: "SRS Gateway-Anforderungen"
tags:
  - graphify/document
  - graphify/EXTRACTED
  - community/SRS_Gateway-Anforderungen
---

# FA-GW-02: Register 0-49 in einer Anfrage, nach Registerplan dekodieren

## Connections
- [[DevicePoller]] - `implements` [INFERRED]
- [[Gateway-Pflichten (konfigurierbare Geraete, Dekodierung, Intervall, Simzeit, Fehler, Beenden)]] - `implements` [INFERRED]
- [[RegisterPlans]] - `references` [EXTRACTED]
- [[SRS LB03 - Von Modbus zu RabbitMQ]] - `references` [EXTRACTED]

#graphify/document #graphify/EXTRACTED #community/SRS_Gateway-Anforderungen