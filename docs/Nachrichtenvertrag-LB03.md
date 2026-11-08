# Nachrichtenvertrag LB03 – Gerätedaten über MQTT (Entwurf)

**Stand:** 2026-11-08 · **Status:** Entwurf mit den Entscheidungen vom 2026-11-08 (§7), umgesetzt im Gateway
(`src/ModbusRabbitGateway`) · **Verantwortlich:** Susanne Lastowicka
**Schemaversion:** 1

Dieser Vertrag legt fest, was das Modbus–RabbitMQ-Gateway veröffentlicht und was der Viewer erwarten darf.
Er ist Teil der SRS (`docs/SRS-LB03.md`, folgt) und gilt für alle verpflichtenden Feldgeräte (PV, Batterie,
DC-Ladesäule, Kühlhaus).

## 1. Datenweg und Grundsätze

```
Feldgerät (Modbus-Server) → Gateway (Modbus-Client, MQTT-Publisher) → RabbitMQ (MQTT-Plugin, 1883) → Viewer (MQTT-Werkzeug)
```

- **Quelle der Messwerte ist ausschließlich Modbus.** Das Gateway liest je Gerät die Register 0–49 in **einer**
  Leseanfrage und dekodiert sie mit `RegisterPlans.For(Gerätetyp)` und `RegisterCodec` (`src/Modbus.Contracts`).
- **Namen, Einheiten und Bedeutung der Messwerte kommen aus dem Registerplan** des Geräts (Geräte-README).
  Ändert sich ein Registerplan, ändert sich die Nachricht automatisch mit; der Vertrag muss nicht je Gerät
  nachgezogen werden.
- **Virtuelle Zeit und reale Abfragezeit sind getrennte Felder** mit eigenen Namen (`simTime` vs. `polledAtUtc`)
  und werden nie verrechnet.
- Pro Lesevorgang eines Geräts entsteht höchstens **eine** Messwertnachricht. Ohne gültige Lesung gibt es keine
  Messwertnachricht, sondern eine Statusnachricht (§4). Alte Werte erscheinen dadurch nie als aktuelle.

## 2. Topics (MQTT) und Routing in RabbitMQ

| Topic | Inhalt | QoS | Retain | Sender |
|---|---|---|---|---|
| `energytwin/devices/{deviceId}/measurements` | Messwertnachricht (§3), eine je Lesung | 1 | nein | Gateway |
| `energytwin/devices/{deviceId}/status` | Verbindungs- und Dekodierstatus des Geräts (§4) | 1 | ja | Gateway |
| `energytwin/gateway/{gatewayId}/status` | `online`/`offline` des Gateways; `offline` als Last Will | 1 | ja | Gateway / Broker |

- `{deviceId}` ist die konfigurierte Geräte-ID, z. B. `PV-01`, `BAT-01`, `DC-01`, `KH-01`. Erlaubt: Buchstaben `A–Z`/`a–z`, `0–9`, `-`.
- **Abonnement im Viewer:** `energytwin/#` (alles) oder `energytwin/devices/+/measurements` (nur Messwerte).
- **Retained bei RabbitMQ:** gespeicherte Nachrichten bekommen nur Abos mit exakt passendem Topic, nicht Wildcard-Abos.
  Deshalb wiederholt das Gateway die Status-Nachrichten regelmäßig (§4).
- **RabbitMQ intern:** Das MQTT-Plugin legt MQTT-Nachrichten auf den Exchange `amq.topic`; aus `/` im Topic wird `.`
  im Routing Key (`energytwin.devices.DC-01.measurements`). Für jeden MQTT-Abonnenten legt das Plugin automatisch
  eine Queue an und bindet sie. Eigene Exchanges, Queues oder Bindings sind nicht nötig.

## 3. Messwertnachricht

JSON, UTF-8, Zahlen mit Punkt (JSON-Standard, unabhängig von der Kultur).

| Feld | Typ | Pflicht | Bedeutung |
|---|---|---|---|
| `schemaVersion` | Zahl | ja | Version dieses Vertrags, derzeit `1` |
| `deviceId` | Text | ja | Eindeutige Geräte-ID aus der Gateway-Konfiguration (§5) |
| `deviceType` | Text | ja | Aus Register 0: `PvPlant`, `Battery`, `Charger`, `IndustrialConsumer` |
| `simTime` | Text | ja | **Virtuelle Simulationszeit** des gelesenen Zustands aus Register 8–9, ISO 8601 **ohne** Zeitzone, Sekundenauflösung, z. B. `2026-01-15T06:00:00`. Die Modellzeit ist im Gesamtsystem zonenlos definiert (wie im Master), deshalb kein `Z` und kein Offset |
| `simTimeEpoch` | Zahl | ja | Derselbe Zeitpunkt als Rohwert (Sekunden seit 1970-01-01 00:00 Modellzeit), eindeutig sortierbar |
| `runId` | Zahl | ja | Lauf-Nummer aus Register 10; ändert sich bei jedem Neustart im Master. `simTime` ist nur innerhalb eines Laufs monoton |
| `seq` | Zahl | ja | **Gerätesequenz** aus Register 11 (0–65535, Überlauf auf 0). Zählt bei jeder neuen Modellzeit hoch; gleiche `runId` + gleiche `seq` = gleicher Gerätezustand |
| `publishSeq` | Zahl | ja | **Nachrichtenzähler des Gateways** je Gerät, beginnt beim Gateway-Start bei 1, ohne Überlauf (64 Bit). Lücken zeigen verlorene Nachrichten |
| `polledAtUtc` | Text | ja | **Reale Abfragezeit** (Wanduhr des Gateways) in UTC, ISO 8601 mit `Z` und Millisekunden. Nur zur Diagnose, **nie** als Messzeitpunkt verwenden |
| `quality` | Text | ja | `good`, `uncertain` oder `bad` (§3.1) |
| `qualityReason` | Text | ja | Grund in Kurzform (§3.1) |
| `measurements` | Liste | ja | Messwerte (§3.2) |

### 3.1 Qualitätsstatus

Das Gateway leitet die Qualität aus den Kopfregistern 12 (Zeitstatus), 13 (Simulation läuft) und 11 (Sequenz) ab.

| `quality` | `qualityReason` | Bedingung |
|---|---|---|
| `good` | `running` | Zeitstatus = synchron (1), Simulation läuft (1), `seq` ändert sich regelmäßig (nicht `stale`) |
| `good` | `paused` | Zeitstatus = synchron (1), Simulation läuft = 0: globale Pause, Werte sind der letzte gültige Stand. **Es wird in der Pause weiter in jedem Intervall veröffentlicht** (Entscheidung 2026-11-08) |
| `uncertain` | `standalone` | Zeitstatus = Standalone (2): Gerät läuft ohne Master, Zeit nicht mit dem Gesamtsystem abgestimmt |
| `uncertain` | `stale` | Simulation läuft = 1, aber Lauf und `seq` sind unverändert, obwohl rechnerisch mehr als `--stale-after` Modellzeit (Standard 60 min) vergangen ist: reale Zeit seit der letzten Änderung × Tempo (Register 14–15), mindestens 3 Abfrageintervalle. So passt die Grenze bei jedem Tempo zu den Schrittweiten der Geräte (1 s bis einige Minuten) |
| `uncertain` | `invalidValue` | Sonst `good`, aber ein Register enthält keine endliche Zahl (`value` = `null`) |
| `bad` | `masterLost` | Zeitstatus = Master verloren (3): Gerätezeit steht, Werte nicht aktuell |
| `bad` | `noTime` | Zeitstatus = keine Zeit (0) oder `simTimeEpoch` = 0 |

Reihenfolge der Prüfung: `noTime`, `masterLost`, `stale`, `standalone`, `invalidValue`, `paused`/`running`.

Eine Gerätestörung (Betriebszustand 3) ist **kein** Qualitätsproblem: Sie ist ein gültiger Messwert und steht in
`measurements` (`Betriebszustand` = `Störung`, `Fehlercode`).

### 3.2 Messwerte

Jeder Eintrag entspricht einem Register(paar) des Registerplans:

| Feld | Typ | Bedeutung |
|---|---|---|
| `key` | Text | **Stabiler Schlüssel** aus dem Namen: Kleinbuchstaben, Umlaute umschrieben, sonst nur `a–z`, `0–9`, `_` (`Punkt 1: SOC` → `punkt_1_soc`). Gedacht als späterer InfluxDB-Feldname |
| `name` | Text | Name aus dem Registerplan, z. B. `Wirkleistung`, `Punkt 1: SOC` (Anzeige) |
| `address` | Zahl | Modbus-Startadresse, zur Rückverfolgung zum Registerplan |
| `value` | Zahl oder `null` | Dekodierter Wert in der angegebenen Einheit (Float32 auf 3 Nachkommastellen gerundet); `null`, wenn das Register keine endliche Zahl enthält |
| `unit` | Text | Einheit aus dem Registerplan (`kW`, `kWh`, `%`, `°C`, …); leer bei Zuständen und Zählern ohne Einheit |
| `text` | Text | Nur bei Aufzählungen: Klartext laut Registerplan, z. B. `lädt`; sonst nicht vorhanden |

**Enthalten sind:** aus dem Kopf `Betriebszustand`, `Fehlercode`, `Wirkleistung`, `Kumulierte Energie`, `Tempo`,
`Ein/Aus`, `Letzter Schreibfehler` und dessen Adresse (Register 18/19, Entscheidung 2026-11-08: macht abgelehnte
Sollwerte im Viewer sichtbar); dazu **alle** gerätespezifischen Register 20–49 des Plans.
**Nicht enthalten sind:** Register, die schon im Umschlag stehen (Gerätetyp, Geräte-Nummer, Modellzeit, Lauf,
Sequenz, Zeitstatus, Simulation läuft) und der Reset-Befehl (Register 17, liest immer 0).

**Schlüssel festgehalten:** Die Schlüssel aller Pläne gibt `dotnet run --project src/ModbusRabbitGateway -- --keys` aus;
sie stehen in `tests/ModbusRabbitGateway.Tests/measurement-keys.txt`. Ein Test schlägt an, wenn sich ein Schlüssel
ändert (z. B. weil ein Register umbenannt wurde). Dann bewusst entscheiden und die Datei nachziehen.

Vorzeichen der Wirkleistung wie im Registerkopf: positiv = Aufnahme, negativ = Abgabe (PV negativ).

### 3.3 Beispielnachricht (DC-Ladesäule)

Topic: `energytwin/devices/DC-01/measurements`

```json
{
  "schemaVersion": 1,
  "deviceId": "DC-01",
  "deviceType": "Charger",
  "simTime": "2026-01-15T06:00:00",
  "simTimeEpoch": 1768456800,
  "runId": 1,
  "seq": 412,
  "publishSeq": 87,
  "polledAtUtc": "2026-11-08T14:32:05.123Z",
  "quality": "good",
  "qualityReason": "running",
  "measurements": [
    { "key": "betriebszustand", "name": "Betriebszustand", "address": 2, "value": 2, "unit": "", "text": "Betrieb" },
    { "key": "fehlercode", "name": "Fehlercode", "address": 3, "value": 0, "unit": "" },
    { "key": "wirkleistung", "name": "Wirkleistung", "address": 4, "value": 187.5, "unit": "kW" },
    { "key": "kumulierte_energie", "name": "Kumulierte Energie", "address": 6, "value": 1243.82, "unit": "kWh" },
    { "key": "tempo", "name": "Tempo", "address": 14, "value": 600, "unit": "x" },
    { "key": "ein_aus", "name": "Ein/Aus", "address": 16, "value": 1, "unit": "", "text": "ein" },
    { "key": "letzter_schreibfehler", "name": "Letzter Schreibfehler", "address": 18, "value": 0, "unit": "", "text": "angenommen" },
    { "key": "adresse_des_letzten_schreibzugriffs", "name": "Adresse des letzten Schreibzugriffs", "address": 19, "value": 20, "unit": "" },
    { "key": "leistungsgrenze_der_saeule", "name": "Leistungsgrenze der Säule", "address": 20, "value": 200, "unit": "kW" },
    { "key": "punkt_1_zustand", "name": "Punkt 1: Zustand", "address": 22, "value": 2, "unit": "", "text": "lädt" },
    { "key": "punkt_1_leistung", "name": "Punkt 1: Leistung", "address": 23, "value": 150, "unit": "kW" },
    { "key": "punkt_1_soc", "name": "Punkt 1: SOC", "address": 25, "value": 63.4, "unit": "%" },
    { "key": "punkt_1_temperatur", "name": "Punkt 1: Temperatur", "address": 27, "value": 41.2, "unit": "°C" },
    { "key": "punkt_2_zustand", "name": "Punkt 2: Zustand", "address": 29, "value": 2, "unit": "", "text": "lädt" },
    { "key": "punkt_2_leistung", "name": "Punkt 2: Leistung", "address": 30, "value": 37.5, "unit": "kW" },
    { "key": "punkt_2_soc", "name": "Punkt 2: SOC", "address": 32, "value": 88.1, "unit": "%" },
    { "key": "punkt_2_temperatur", "name": "Punkt 2: Temperatur", "address": 34, "value": 35, "unit": "°C" },
    { "key": "warteschlange", "name": "Warteschlange", "address": 36, "value": 1, "unit": "Fahrzeuge" },
    { "key": "abgewiesene_fahrzeuge", "name": "Abgewiesene Fahrzeuge", "address": 37, "value": 0, "unit": "Fahrzeuge" }
  ]
}
```

Das Gateway schreibt die Nachricht kompakt in eine Zeile; `--json` zeigt sie so auf der Konsole an.

## 4. Statusnachricht je Gerät

Topic: `energytwin/devices/{deviceId}/status`, retained. Wird bei **jeder Änderung** des Zustands gesendet, nicht bei
jeder Lesung, und zusätzlich alle `--status-repeat` Sekunden (Standard 30) **wiederholt**, damit auch ein später
gestarteter Viewer mit Wildcard-Abo den Zustand sieht; `sinceUtc` bleibt dabei der Beginn des Zustands. Der
Gateway-Status `online` wird ebenso wiederholt. Bei `offline` zählt nur der Zustand (der Fehlertext wechselt zwischen „Lesen abgebrochen“ und „keine
Verbindung“, ohne dass sich etwas ändert); bei `decodeError` auch ein neuer Grund.

```json
{
  "schemaVersion": 1,
  "deviceId": "DC-01",
  "state": "offline",
  "error": "Modbus: Verbindung zu 127.0.0.1:1504 abgelehnt",
  "sinceUtc": "2026-11-08T14:33:10.480Z",
  "lastSimTime": "2026-01-15T06:10:00",
  "lastRunId": 1,
  "lastSeq": 418
}
```

| `state` | Bedeutung |
|---|---|
| `online` | Letzte Lesung und Dekodierung erfolgreich |
| `offline` | Modbus-Verbindung fehlgeschlagen oder abgebrochen; das Gateway versucht es im Abfrageintervall erneut |
| `decodeError` | Antwort gelesen, aber ungültig: falscher Gerätetyp für die konfigurierte ID, unbekannter Registerplan, Geräte-Nummer passt nicht |

`lastSimTime`, `lastRunId` und `lastSeq` beschreiben den letzten **gültig gelesenen** Stand; sie fehlen, wenn es noch
keinen gab.

## 5. Konfiguration des Gateways

| Einstellung | Option | Standard | Bedeutung |
|---|---|---|---|
| Geräte | `--device ID=host:port[/unit]` (mehrfach) | `PV-01=127.0.0.1:1502`, `BAT-01=…:1503`, `DC-01=…:1504`, `KH-01=…:1505` | Geräte-ID, Host, Port, Unit-ID (Standard 1) |
| Abfrageintervall | `--interval <s>` | 1 s (reale Zeit) | Wie oft jedes Gerät gelesen und veröffentlicht wird (auch in der Pause) |
| Grenze für `stale` | `--stale-after <min>` | 60 min Modellzeit | Siehe §3.1 |
| Modbus-Zeitlimit | `--timeout <ms>` | 1000 ms | Je Gerät und Zugriff |
| Gateway-Kennung | `--gateway-id <ID>` | `GW-01` | Für das Gateway-Status-Topic und die MQTT-Client-ID |
| MQTT-Broker | `--broker <host:port>` | `127.0.0.1:1883` | Host und Port des RabbitMQ-MQTT-Plugins |
| MQTT-Zugangsdaten | `--mqtt-user`, `ENERGYTWIN_MQTT_USER`/`ENERGYTWIN_MQTT_PASSWORD`, `--env-file` | `docker/.env` (`RABBITMQ_DEFAULT_USER`/`RABBITMQ_DEFAULT_PASS`) | Nie im Repo, Passwort nie als Option |
| Status-Wiederholung | `--status-repeat <s>` | 30 s | 0 = nur bei Änderung (§4) |
| Probebetrieb | `--console` | aus | Ohne Broker, nur Konsole |

Prüfung beim Lesen: Register 0 (Gerätetyp) und 1 (Geräte-Nummer) müssen zur konfigurierten ID passen
(`DC-01` ⇒ Typ 3, Nummer 1). Sonst `decodeError`, keine Messwertnachricht.

## 6. Bekannte Einschränkungen und Wiederanlauf

- **Broker-Ausfall:** Das Gateway liest weiter und meldet den Ausfall einmal sichtbar. Messwerte aus der Ausfallzeit
  werden verworfen (kein Puffer; erkennbar an Lücken in `publishSeq`). Neuversuch beim nächsten Veröffentlichen mit
  wachsendem Abstand (1 … 30 s). Nach dem Wiederverbinden werden die letzten Status-Nachrichten (retained) nachgeholt.
- **Last Will:** `sinceUtc` in der Last-Will-Nachricht ist der Zeitpunkt des Verbindungsaufbaus (der Broker sendet sie
  unverändert).

- **Keine atomare Lesung:** Die Register 0–49 werden in einer Anfrage gelesen, das Gerät kann sie aber währenddessen
  aktualisieren (EasyModbus ist nicht transaktional). Einzelne Werte einer Nachricht können daher aus zwei
  aufeinanderfolgenden Schritten stammen. `simTime`/`seq` gehören zu dem Stand, den Register 8–11 bei der Lesung
  zeigten. Mögliche Abschwächung: Register 8–11 nach der Lesung erneut lesen und die Nachricht verwerfen, wenn sich
  `seq` geändert hat.
- **Sekundenauflösung:** Die Modellzeit im Register hat nur ganze Sekunden.
- **Bei hohem Tempo** liegen zwischen zwei Lesungen viele Modellschritte; das Gateway sieht nur Stichproben, keinen
  lückenlosen Verlauf.
- **Geräte werden nacheinander gelesen** (EasyModbus ist synchron). Ein nicht erreichbares Gerät kostet je Intervall
  bis zu `--timeout`; die Lesungen der anderen Geräte verschieben sich entsprechend, gehen aber nicht verloren.
- **`seq`-Überlauf** nach 65535; für die Erkennung „unverändert“ ohne Bedeutung, für Sortierungen `simTimeEpoch` verwenden.

## 7. Entscheidungen (2026-11-08, Susanne Lastowicka)

1. **Veröffentlichung in der Pause:** ja, weiter in jedem Intervall mit `quality = good`, `qualityReason = paused`.
   Der Viewer sieht so, dass das System lebt und welche virtuelle Zeit gilt (Nachweis 5 der Aufgabe).
2. **Messwertnamen:** deutsch. InfluxDB erlaubt UTF-8 in Feldnamen, Englisch ist also nicht nötig. Weil Anzeigenamen
   Leerzeichen, `:` und Umlaute enthalten und sich leicht ändern, gibt es zusätzlich den stabilen Schlüssel `key`
   (ASCII, `snake_case`, deutsch), festgehalten durch einen Test (§3.2).
3. **Schreibfehler-Register 18/19:** werden mitgeschickt.
4. **Registerpläne:** gelten als bestätigt (Charger durch Susanne; PV, Batterie und Kühlhaus werden wie bestätigt
   behandelt). Änderungen an einem Plan ändern die Nachricht automatisch mit; neue oder umbenannte Schlüssel zeigt der
   Schlüsseltest an.
