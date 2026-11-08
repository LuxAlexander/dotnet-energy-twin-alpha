---
source_file: "docs/Nachrichtenvertrag-LB03.md"
type: "document"
community: "Nachrichtenvertrag & Messwerte"
tags:
  - graphify/document
  - graphify/EXTRACTED
  - community/Nachrichtenvertrag__Messwerte
---

# Messwertnachricht

## Connections
- [[Beispielnachricht DC-01]] - `implements` [EXTRACTED]
- [[MeasurementMessage]] - `implements` [INFERRED]
- [[Messwert-Eintrag (key, name, address, value, unit, text)]] - `references` [EXTRACTED]
- [[Nachrichtenvertrag LB03 (Schemaversion 1)]] - `references` [EXTRACTED]
- [[Qualitaetsstatus gooduncertainbad]] - `references` [EXTRACTED]
- [[RegisterCodec]] - `references` [EXTRACTED]
- [[Topic energytwindevices{deviceId}measurements (QoS 1)]] - `shares_data_with` [EXTRACTED]
- [[polledAtUtc (reale Abfragezeit, nur Diagnose)]] - `references` [EXTRACTED]
- [[publishSeq Nachrichtenzaehler des Gateways (Luecken = Verlust)]] - `references` [EXTRACTED]
- [[runId (Register 10)]] - `references` [EXTRACTED]
- [[seq Geraetesequenz (Register 11)]] - `references` [EXTRACTED]
- [[simTime  simTimeEpoch (virtuelle Zeit, Register 8-9)]] - `references` [EXTRACTED]

#graphify/document #graphify/EXTRACTED #community/Nachrichtenvertrag__Messwerte