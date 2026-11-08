# Modbus.Client

**Zweck:** gemeinsamer Modbus-TCP-Client für den Diagnose-Client und das Modbus–RabbitMQ-Gateway: Verbindung aufbauen,
Holding Registers lesen und schreiben, Bibliotheksfehler in verständliche deutsche Meldungen übersetzen.
**Verantwortlich:** Susanne Lastowicka
**Basis:** `DeviceConnection` aus `src/ModbusDiagnosticClient` (LB02), am 2026-11-08 hierher verschoben (`git mv`, Historie erhalten).
**Abhängigkeiten:** `Modbus.Contracts`, EasyModbusTCP.NETCore.

## Schnittstellen

| Typ | Bedeutung |
|---|---|
| `DeviceConnection(host, port, unitId, timeoutMs)` | `Connect()`, `ReadRegisters(start, count)`, `Write(definition, value)`, `Close()`; nach einem Verbindungsfehler verbindet der nächste Zugriff neu |
| `DiagnosticException` | Fehler mit `Kind`: `Connection` (nicht erreichbar/abgebrochen), `Protocol` (Modbus-Ausnahme, ungültige Antwort), `Value` (Eingabe ungültig) |

Der Name `DiagnosticException` stammt aus dem Diagnose-Client und wurde beim Verschieben bewusst beibehalten.

## Bekannte Probleme

- EasyModbusTCP.NETCore ist synchron (Bibliotheksgrenze der Aufgabe, nicht in `Task.Run` verpackt).
- Die Bibliothek schreibt beim Anlegen eines Clients ein Banner auf die Konsole.

## Tests

Indirekt über `tests/ModbusDiagnosticClient.Tests` und `tests/ModbusRabbitGateway.Tests` (beide gegen einen echten Modbus-Server).
