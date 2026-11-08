---
type: community
cohesion: 0.15
members: 14
---

# Diagnose-Client & Modbus.Client

**Cohesion:** 0.15 - loosely connected
**Members:** 14 nodes

## Members
- [[A10 Gateway-Anforderungen (nicht erfüllt; wiederverwendbar DeviceConnection, RegisterPlans, RegisterCodec)]] - document - docs/task-status/aufgabe-03.md
- [[A11 Daten nur über Modbus, Diagnose-Client bleibt nutzbar (erfüllt)]] - document - docs/task-status/aufgabe-03.md
- [[Befehle readheaderwatchwriteforcerawinfoconnect]] - document - src/ModbusDiagnosticClient/README.md
- [[DeviceConnection am 2026-11-08 aus dem Diagnose-Client verschoben (git mv)]] - document - src/Modbus.Client/README.md
- [[EasyModbusTCP.NETCore synchron (Bibliotheksgrenze, Banner)]] - document - src/Modbus.Client/README.md
- [[Empfehlung Gateway als eigenes Projekt mit gemeinsamer Modbus-Bibliothek]] - document - docs/task-status/aufgabe-03.md
- [[Empfehlung MQTT statt AMQP]] - document - docs/task-status/aufgabe-03.md
- [[Fehlerarten VERBINDUNGS-PROTOKOLL-WERTEFEHLER, Exit-Codes 01264]] - document - src/ModbusDiagnosticClient/README.md
- [[Modbus.Client (gemeinsamer Modbus-TCP-Client)]] - document - src/Modbus.Client/README.md
- [[ModbusDiagnosticClient (Diagnose-Konsole)]] - document - src/ModbusDiagnosticClient/README.md
- [[Offene Fragen ans Team (AMQPMQTT, Gateway-Projekt, Zuständigkeit, Pause-Veröffentlichung, Skills)]] - document - docs/task-status/aufgabe-03.md
- [[RegisterPlans]] - code - src/Modbus.Contracts/RegisterPlans.cs
- [[RegisterPlans.cs]] - code - src/Modbus.Contracts/RegisterPlans.cs
- [[Schreiben und Prüfen 200 ms warten, Register 1819 zurücklesen]] - document - src/ModbusDiagnosticClient/README.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Diagnose-Client__ModbusClient
SORT file.name ASC
```

## Connections to other communities
- 3 edges to [[_COMMUNITY_DeviceConnection (Modbus.Client)]]
- 3 edges to [[_COMMUNITY_Aufgabenabgleich (readTask)]]
- 2 edges to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 2 edges to [[_COMMUNITY_Gateway-Publisher]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_Registerpläne]]
- 1 edge to [[_COMMUNITY_MessageBuilder]]
- 1 edge to [[_COMMUNITY_SRS Gateway-Anforderungen]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_Modbus.Client-Fehler]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 1 edge to [[_COMMUNITY_Funktionsnachweis mit Master]]
- 1 edge to [[_COMMUNITY_DevicePoller & Gerätetests]]

## Top bridge nodes
- [[Modbus.Client (gemeinsamer Modbus-TCP-Client)]] - degree 8, connects to 4 communities
- [[RegisterPlans]] - degree 7, connects to 4 communities
- [[A10 Gateway-Anforderungen (nicht erfüllt; wiederverwendbar DeviceConnection, RegisterPlans, RegisterCodec)]] - degree 4, connects to 3 communities
- [[ModbusDiagnosticClient (Diagnose-Konsole)]] - degree 9, connects to 2 communities
- [[Schreiben und Prüfen 200 ms warten, Register 1819 zurücklesen]] - degree 3, connects to 2 communities