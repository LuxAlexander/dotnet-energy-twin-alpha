# ModbusRabbitGateway

**Zweck:** Modbus–RabbitMQ-Gateway (Aufgabe 03). Liest die Feldgeräte über Modbus TCP, dekodiert Registerkopf und
gerätespezifische Register nach dem Registerplan und baut daraus Nachrichten nach dem
[Nachrichtenvertrag](../../docs/Nachrichtenvertrag-LB03.md).
**Verantwortlich:** Susanne Lastowicka
**Basis:** Modbus-Verbindung aus dem Diagnose-Client (LB02), jetzt in `src/Modbus.Client`.
**Abhängigkeiten:** `Modbus.Contracts` (Registerpläne, Codec), `Modbus.Client` (Verbindung, EasyModbusTCP.NETCore).

**Stand 2026-11-08:** Modbus lesen, dekodieren, Qualitätsstatus und Nachrichten fertig; veröffentlicht wird noch auf
die **Konsole** (Probebetrieb ohne Broker). Nächster Schritt: MQTT-Publisher an RabbitMQ (Port 1883).

## Konfiguration

| Option | Standard | Bedeutung |
|---|---|---|
| `--device ID=host:port[/unit]` | `PV-01=127.0.0.1:1502`, `BAT-01=…:1503`, `DC-01=…:1504`, `KH-01=…:1505` | Gerät anbinden, mehrfach möglich; ersetzt die Standardgeräte |
| `--interval <s>` | 1 | Abfrage- und Veröffentlichungsintervall (reale Zeit), mind. 0,1 |
| `--stale-after <min>` | 60 | Modellzeit ohne neue Gerätesequenz bis Qualität `stale` |
| `--timeout <ms>` | 1000 | Modbus-Zeitlimit je Gerät |
| `--gateway-id <ID>` | `GW-01` | Kennung im Gateway-Status-Topic |
| `--once` | – | jedes Gerät einmal lesen und beenden |
| `--json` | – | vollständige Nachrichten (Topic + JSON) ausgeben |
| `--keys` | – | Messwert-Schlüssel aller Registerpläne ausgeben |

Aus der Geräte-ID prüft das Gateway, ob das richtige Gerät antwortet: Das Kürzel (`PV`, `BAT`, `DC`, `KH`) gibt den
Gerätetyp (Register 0) vor, die Zahl am Ende die Geräte-Nummer (Register 1). Bei Abweichung: `decodeError`, keine
Messwerte. Unbekannte Kürzel werden nicht auf den Typ geprüft.

## Bedienung / Start

```bash
# Geräte wie in der zentralen README starten (Charger auf 1504), dann:
dotnet run --project src/ModbusRabbitGateway

# nur die Ladesäule, einmal lesen, vollständige Nachricht zeigen
dotnet run --project src/ModbusRabbitGateway -- --device DC-01=127.0.0.1:1504 --once --json
```

Je Lesung erscheint eine Übersichtszeile, z. B.

```
DC-01   Sim 2026-11-08 00:15:00  Lauf 1   Seq 5     Nr 1      uncertain/standalone  Wirkleistung 0 kW
BAT-01  OFFLINE: Keine Verbindung zu 127.0.0.1:1503 (Unit 1): connection timed out. … – neuer Versuch im nächsten Intervall
```

Beenden mit `Q` oder Strg+C: Die laufende Runde wird abgebrochen, der Gateway-Status `offline` wird noch gemeldet,
die Verbindungen werden geschlossen.

## Ablauf und Aufbau

| Klasse | Aufgabe |
|---|---|
| `GatewayService` | Hauptschleife mit `PeriodicTimer` (reale Zeit über `TimeProvider`), liest alle Geräte nacheinander, veröffentlicht Messwerte und Statusänderungen, meldet `online`/`offline` des Gateways |
| `DevicePoller` | liest Register 0–49 eines Geräts in einer Anfrage; Verbindungsfehler ⇒ `offline`, falsches Gerät/Protokollfehler ⇒ `decodeError`, jeweils **ohne** Messwertnachricht; Status nur bei Änderung |
| `MessageBuilder` | dekodiert nach `RegisterPlans.For(Typ)`, trennt virtuelle Zeit (`simTime`) und Abfragezeit (`polledAtUtc`), leitet den Qualitätsstatus ab (Vertrag §3.1) |
| `MeasurementKeys`, `KeyCatalog` | stabile Schlüssel je Messwert (`Punkt 1: SOC` → `punkt_1_soc`) |
| `Messages` (`MeasurementMessage`, `DeviceStatusMessage`, `GatewayStatusMessage`, `Contract`, `MessageJson`) | Nachrichten, Topics, Qualitäts- und Statuswerte, JSON (camelCase, UTF-8) |
| `IMessagePublisher`, `ConsolePublisher` | Ziel der Nachrichten; heute Konsole, später MQTT |
| `GatewayOptions`, `DeviceEndpoint` | Kommandozeile, Geräteangaben |

Die Daten kommen ausschließlich über Modbus; der Diagnose-Client bleibt unverändert nutzbar (gleiche
`DeviceConnection`). Wiederanlauf: Nach einem Verbindungsfehler verbindet `DeviceConnection` beim nächsten Lesen selbst
neu; das Gateway versucht es in jedem Intervall, ohne Endlosschleife und ohne Programmabbruch.

## Schnittstellen

- **Gelesen:** Modbus TCP, Holding Registers 0–49 je Gerät (FC03), Registerpläne in den Geräte-READMEs.
- **Veröffentlicht:** Topics `energytwin/devices/{id}/measurements`, `energytwin/devices/{id}/status`,
  `energytwin/gateway/{id}/status`; Format im [Nachrichtenvertrag](../../docs/Nachrichtenvertrag-LB03.md).

## Tests

`dotnet test tests/ModbusRabbitGateway.Tests` – siehe [`tests/ModbusRabbitGateway.Tests/README.md`](../../tests/ModbusRabbitGateway.Tests/README.md).

## Bekannte Probleme

- Noch kein MQTT: Nachrichten gehen nur auf die Konsole.
- Keine atomare Lesung über mehrere Register (EasyModbus); Einzelheiten im Vertrag §6.
- Geräte werden nacheinander gelesen; ein nicht erreichbares Gerät verzögert die Runde um bis zu `--timeout`.
- Die Bibliothek gibt beim Start je Gerät `EasyModbus Client Library Version …` aus; das lässt sich nicht abschalten.
