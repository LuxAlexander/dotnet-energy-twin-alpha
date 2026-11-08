# ModbusRabbitGateway

**Zweck:** Modbus–RabbitMQ-Gateway (Aufgabe 03). Liest die Feldgeräte über Modbus TCP, dekodiert Registerkopf und
gerätespezifische Register nach dem Registerplan und veröffentlicht Nachrichten nach dem
[Nachrichtenvertrag](../../docs/Nachrichtenvertrag-LB03.md) per **MQTT** an RabbitMQ (MQTT-Plugin, Port 1883).
**Verantwortlich:** Susanne Lastowicka
**Basis:** Modbus-Verbindung aus dem Diagnose-Client (LB02), jetzt in `src/Modbus.Client`.
**Abhängigkeiten:** `Modbus.Contracts` (Registerpläne, Codec), `Modbus.Client` (Verbindung, EasyModbusTCP.NETCore),
`Shared.Config` (Repo-Ordner für `docker/.env`), MQTTnet 5.0.1 (MQTT-Client).

**Stand 2026-11-08:** vollständiger Datenweg Modbus → MQTT umgesetzt, mit einem MQTT-Broker im Testprozess getestet
und im Probelauf gegen RabbitMQ 4.4.0 (Docker) geprüft. `--console` erlaubt den Betrieb ohne Broker.

## Konfiguration

| Option | Standard | Bedeutung |
|---|---|---|
| `--device ID=host:port[/unit]` | `PV-01=127.0.0.1:1502`, `BAT-01=…:1503`, `DC-01=…:1504`, `KH-01=…:1505` | Gerät anbinden, mehrfach möglich; ersetzt die Standardgeräte |
| `--interval <s>` | 1 | Abfrage- und Veröffentlichungsintervall (reale Zeit), mind. 0,1 |
| `--stale-after <min>` | 60 | Modellzeit ohne neue Gerätesequenz bis Qualität `stale` |
| `--timeout <ms>` | 1000 | Modbus-Zeitlimit je Gerät |
| `--gateway-id <ID>` | `GW-01` | Kennung im Gateway-Status-Topic und MQTT-Client-ID |
| `--broker <host:port>` | `127.0.0.1:1883` | MQTT-Broker (RabbitMQ-MQTT-Plugin) |
| `--mqtt-user <name>` | – | MQTT-Benutzer (sonst Umgebung oder `.env`) |
| `--env-file <pfad>` | `<Repo>/docker/.env` | `.env` mit `RABBITMQ_DEFAULT_USER` / `RABBITMQ_DEFAULT_PASS` |
| `--status-repeat <s>` | 30 | Status-Nachrichten zusätzlich alle n Sekunden wiederholen (0 = nur bei Änderung), siehe unten |
| `--console` | – | Probebetrieb ohne Broker, nur Konsole |
| `--once` | – | jedes Gerät einmal lesen und beenden |
| `--json` | – | vollständige Nachrichten (Topic + JSON) zusätzlich auf der Konsole |
| `--keys` | – | Messwert-Schlüssel aller Registerpläne ausgeben |

### Zugangsdaten

Das Passwort steht nie im Repo und wird nie als Option angenommen (wäre in der Prozessliste sichtbar). Reihenfolge:

1. Benutzer aus `--mqtt-user`,
2. Umgebungsvariablen `ENERGYTWIN_MQTT_USER` und `ENERGYTWIN_MQTT_PASSWORD`,
3. `docker/.env` (bzw. `--env-file`) mit `RABBITMQ_DEFAULT_USER` und `RABBITMQ_DEFAULT_PASS` – dieselbe Datei, aus der
   die Compose-Datei die Broker-Zugangsdaten liest; `*.env` ist in `.gitignore`. Einrichtung: [`docker/README.md`](../../docker/README.md).

Ohne Angaben verbindet sich das Gateway anonym; RabbitMQ lehnt das in der Regel ab.

```powershell
$env:ENERGYTWIN_MQTT_USER = "user"; $env:ENERGYTWIN_MQTT_PASSWORD = "<passwort>"
dotnet run --project src/ModbusRabbitGateway
```

Aus der Geräte-ID prüft das Gateway, ob das richtige Gerät antwortet: Das Kürzel (`PV`, `BAT`, `DC`, `KH`) gibt den
Gerätetyp (Register 0) vor, die Zahl am Ende die Geräte-Nummer (Register 1). Bei Abweichung: `decodeError`, keine
Messwerte. Unbekannte Kürzel werden nicht auf den Typ geprüft.

## Bedienung / Start

```bash
# RabbitMQ und die Geräte wie in der zentralen README starten (Charger auf 1504), dann:
dotnet run --project src/ModbusRabbitGateway

# ohne Broker: nur die Ladesäule, einmal lesen, vollständige Nachricht zeigen
dotnet run --project src/ModbusRabbitGateway -- --console --device DC-01=127.0.0.1:1504 --once --json
```

Je Lesung erscheint eine Übersichtszeile, z. B.

```
DC-01   Sim 2026-11-08 00:15:00  Lauf 1   Seq 5     Nr 1      uncertain/standalone  Wirkleistung 0 kW
BAT-01  OFFLINE: Keine Verbindung zu 127.0.0.1:1503 (Unit 1): connection timed out. … – neuer Versuch im nächsten Intervall
```

Beenden mit `Q` oder Strg+C: Die laufende Runde wird abgebrochen, der Gateway-Status `offline` wird noch gemeldet,
die Verbindungen werden geordnet getrennt.

### Broker-Ausfall und Wiederanlauf

| Situation | Verhalten |
|---|---|
| Broker beim Start oder im Betrieb nicht erreichbar | einmal `BROKER NICHT ERREICHBAR: …`, Geräte werden weiter gelesen, Übersichtszeilen tragen `[nicht übertragen]`; kein Abbruch, keine Endlosschleife |
| Neuversuch | beim nächsten Veröffentlichen, mit wachsendem Abstand 1, 2, 4 … höchstens 30 s (dazwischen keine Verbindungsversuche) |
| Broker wieder da | `BROKER WIEDER VERBUNDEN – n Nachricht(en) … nicht übertragen`; die letzten Status-Nachrichten (Gateway und Geräte, retained) werden **nachgeholt** |
| Messwerte aus der Ausfallzeit | **verworfen** (Momentaufnahmen, kein Puffer); im Viewer als Lücke in `publishSeq` erkennbar |
| Falsche Zugangsdaten | RabbitMQ schließt die Verbindung beim Anmelden: `Verbindung bei der Anmeldung geschlossen – startet der Broker gerade, oder sind die Zugangsdaten falsch? (Benutzer … (Passwort aus …))`. Dieselbe Meldung erscheint, solange der Container neu startet (die Docker-Portweiterleitung nimmt an und schließt sofort); beides ist von außen nicht unterscheidbar. Broker mit Ablehnungscode: `Anmeldung abgelehnt (BadUserNameOrPassword) …`. Das Passwort wird nie angezeigt |
| Gateway stirbt ohne geordnetes Beenden | Last Will: der Broker setzt `energytwin/gateway/GW-01/status` auf `offline` |

### Besonderheit RabbitMQ: retained nur bei exaktem Topic

RabbitMQ liefert gespeicherte (retained) Nachrichten nur an Abos, deren Topic **exakt** passt
(`energytwin/devices/BAT-01/status`), nicht an Wildcard-Abos wie `energytwin/#` oder `energytwin/devices/+/status`
(im Probelauf am 2026-11-08 mit RabbitMQ 4.4.0 nachgeprüft). Ein später gestarteter Viewer mit `energytwin/#` sähe
ein ausgefallenes Gerät sonst erst bei der nächsten Zustandsänderung. Deshalb wiederholt das Gateway die
Status-Nachrichten von Gateway und Geräten alle `--status-repeat` Sekunden (Standard 30). `sinceUtc` bleibt dabei der
Beginn des Zustands; auf der Konsole erscheinen nur echte Änderungen.

## Ablauf und Aufbau

| Klasse | Aufgabe |
|---|---|
| `GatewayService` | Hauptschleife mit `PeriodicTimer` (reale Zeit über `TimeProvider`), liest alle Geräte nacheinander, veröffentlicht Messwerte und Statusänderungen, meldet `online`/`offline` des Gateways |
| `DevicePoller` | liest Register 0–49 eines Geräts in einer Anfrage; Verbindungsfehler ⇒ `offline`, falsches Gerät/Protokollfehler ⇒ `decodeError`, jeweils **ohne** Messwertnachricht; Status nur bei Änderung |
| `MessageBuilder` | dekodiert nach `RegisterPlans.For(Typ)`, trennt virtuelle Zeit (`simTime`) und Abfragezeit (`polledAtUtc`), leitet den Qualitätsstatus ab (Vertrag §3.1) |
| `MeasurementKeys`, `KeyCatalog` | stabile Schlüssel je Messwert (`Punkt 1: SOC` → `punkt_1_soc`) |
| `Messages` (`MeasurementMessage`, `DeviceStatusMessage`, `GatewayStatusMessage`, `Contract`, `MessageJson`) | Nachrichten, Topics, Qualitäts- und Statuswerte, JSON (camelCase, UTF-8) |
| `MqttPublisher`, `BrokerSettings` | MQTT (MQTTnet): QoS 1, Retain für Status, Last Will, Wiederanlauf mit Wartezeit, Status nachholen |
| `MqttCredentials` | Zugangsdaten aus Option, Umgebung oder `.env`; Passwort nie in Anzeige oder Kommandozeile |
| `IMessagePublisher`, `ConsolePublisher`, `TeePublisher`, `BrokerUnavailableException` | Ziel der Nachrichten; Konsole für `--console`/`--json` |
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

- RabbitMQ 4.4.0 unter Docker ist geprüft, zuletzt mit allen vier Geräten am 2026-11-08; siehe [Nachweis](../../docs/nachweise/2026-11-08-docker-alexander/README.md). Die interaktive Gesamtvorführung bleibt offen.
- Keine atomare Lesung über mehrere Register (EasyModbus); Einzelheiten im Vertrag §6.
- Geräte werden nacheinander gelesen; ein nicht erreichbares Gerät verzögert die Runde um bis zu `--timeout`.
- Die Bibliothek gibt beim Start je Gerät `EasyModbus Client Library Version …` aus; das lässt sich nicht abschalten.
