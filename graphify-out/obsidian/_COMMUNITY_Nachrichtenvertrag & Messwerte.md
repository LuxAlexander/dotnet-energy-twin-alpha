---
type: community
cohesion: 0.10
members: 27
---

# Nachrichtenvertrag & Messwerte

**Cohesion:** 0.10 - loosely connected
**Members:** 27 nodes

## Members
- [[AK-04 Messwertnachricht enthaelt DeviceId, Zeit, Sequenz, Qualitaet, Einheit]] - document - docs/SRS-LB03.md
- [[Beispielnachricht DC-01]] - document - docs/Nachrichtenvertrag-LB03.md
- [[ChargerRegisters]] - code - src/Modbus.Contracts/ChargerRegisters.cs
- [[ChargerRegisters.CreatePlan()]] - code - src/Modbus.Contracts/ChargerRegisters.cs
- [[Entscheidung Registerplaene gelten als bestaetigt]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Entscheidung Schreibfehler-Register 1819 mitschicken]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Entscheidung deutsche Messwertnamen + stabiler key]] - document - docs/Nachrichtenvertrag-LB03.md
- [[FA-GW-05 simTime aus Geraet, polledAtUtc getrennt]] - document - docs/SRS-LB03.md
- [[FA-VW-01 Empfang aller Geraete ueber RabbitMQ, Abo energytwin]] - document - docs/SRS-LB03.md
- [[GatewayStatusMessage]] - code - src/ModbusRabbitGateway/Messages.cs
- [[Last Will Gateway offline]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Messwert-Eintrag (key, name, address, value, unit, text)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Messwert-Schluessel key (ASCII snake_case, InfluxDB-Feldname)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Messwertnachricht]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Nachrichtenformat DeviceId, virtuelle Zeit, Sequenz, Qualitaet, Messwerte]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[Nachrichtenvertrag LB03 (Schemaversion 1)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Plan_2]] - code - src/Modbus.Contracts/ChargerRegisters.cs
- [[RabbitMQ Exchange amq.topic, Routing Key mit Punkt]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Topic energytwindevices{deviceId}measurements (QoS 1)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Topic energytwingateway{gatewayId}status (Last Will)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[Virtuelle Zeit vs. reale Abfragezeit nicht verwechseln]] - paper - dotNET Programmierung 2026 - Aufgabe 03.pdf
- [[measurement-keys.txt (Schluesseltest)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[polledAtUtc (reale Abfragezeit, nur Diagnose)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[publishSeq Nachrichtenzaehler des Gateways (Luecken = Verlust)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[runId (Register 10)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[seq Geraetesequenz (Register 11)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[simTime  simTimeEpoch (virtuelle Zeit, Register 8-9)]] - document - docs/Nachrichtenvertrag-LB03.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Nachrichtenvertrag__Messwerte
SORT file.name ASC
```

## Connections to other communities
- 6 edges to [[_COMMUNITY_SRS Gateway-Anforderungen]]
- 3 edges to [[_COMMUNITY_Gruppe 52]]
- 3 edges to [[_COMMUNITY_Nachrichtentypen (Contract)]]
- 3 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 3 edges to [[_COMMUNITY_Qualitätsstatus & Pause]]
- 3 edges to [[_COMMUNITY_Status-Wiederholung & RabbitMQ]]
- 2 edges to [[_COMMUNITY_Registerpläne]]
- 2 edges to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]
- 2 edges to [[_COMMUNITY_Aufgabe 03 & Nachweise]]
- 2 edges to [[_COMMUNITY_LB03-Projektstand]]
- 2 edges to [[_COMMUNITY_Fehlerverhalten Gateway]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle (2)]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_GatewayService]]
- 1 edge to [[_COMMUNITY_Messwert-Schlüssel]]
- 1 edge to [[_COMMUNITY_MessageBuilder]]
- 1 edge to [[_COMMUNITY_Zuständigkeiten LB03]]

## Top bridge nodes
- [[Nachrichtenvertrag LB03 (Schemaversion 1)]] - degree 19, connects to 7 communities
- [[Messwertnachricht]] - degree 12, connects to 3 communities
- [[ChargerRegisters]] - degree 6, connects to 3 communities
- [[Last Will Gateway offline]] - degree 4, connects to 3 communities
- [[GatewayStatusMessage]] - degree 4, connects to 3 communities