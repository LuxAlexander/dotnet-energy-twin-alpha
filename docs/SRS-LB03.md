# SRS LB03 – Von Modbus zu RabbitMQ

**Stand:** 2026-11-08 · **Status:** Entwurf; Abschnitte zu Gateway und Nachrichtenvertrag umgesetzt und geprüft,
Broker von Alexander Lux eingerichtet und geprüft, Viewer noch ohne Zuständigkeit · **Erstellt von:** Susanne Lastowicka (mit Claude Opus 5.5)

Knappe, projektspezifische Anforderungsbeschreibung für Aufgabe 03. Details stehen in den verlinkten Dokumenten und
werden hier nicht wiederholt.

## 1. Ziel und Umfang

Die Gerätedaten der Feldgeräte, die bisher nur über Modbus TCP lesbar waren, werden über einen Nachrichtenbroker
(RabbitMQ unter Docker) verteilt und in einem Viewer sichtbar gemacht. Grundlage für die spätere Speicherung in InfluxDB.

**Im Umfang:** Modbus–RabbitMQ-Gateway, Nachrichtenvertrag, RabbitMQ per Docker Compose, Viewer, Anbindung aller
verpflichtenden Feldgeräte (PV, Batterie, DC-Ladesäule, Kühlhaus).
**Nicht im Umfang:** InfluxDB, REST, Grafana; Containerisierung der eigenen Programme.
**Optionale Erweiterung umgesetzt:** Wetterdaten zusätzlich über RabbitMQ, verantwortlich Alexander Lux;
fünf Standorte, Master-Zeit, manuelle Standortübersteuerung und einmalige Pausen-/Fortsetzungsstatus.
Vertrag: [`Wetter-Nachrichtenvertrag-LB03.md`](Wetter-Nachrichtenvertrag-LB03.md),
Prüfung: [`Wetter-MQTT-Nachweis`](nachweise/2026-11-08-wetter-mqtt/README.md).
Master-Daten über RabbitMQ sind weiterhin nicht geplant. Ein interaktiver Viewer bleibt einzurichten.
**Unverändert erhalten:** virtuelle Zeit (Master), Wettersimulator, Modbus-Server der Geräte, Diagnose-Client.

## 2. Datenweg und beteiligte Anwendungen

```
Feldgerät (Modbus-Server) → Gateway (Modbus-Client, MQTT-Publisher) → RabbitMQ (MQTT-Plugin, 1883) → Viewer (MQTT-Werkzeug)
        ↑ virtuelle Zeit vom Simulations-Master (TCP 5100), Wetter vom Wettersimulator (HTTP 8080)
```

| Anwendung / Baustein | Rolle in LB03 | Verantwortlich | Stand |
|---|---|---|---|
| PV, Batterie, DC-Ladesäule, Kühlhaus | liefern Daten nur über Modbus (unverändert) | siehe zentrale `README.md` | vorhanden |
| `ModbusRabbitGateway` (+ `Modbus.Client`) | liest Modbus, dekodiert, veröffentlicht per MQTT | Susanne Lastowicka | umgesetzt |
| Nachrichtenvertrag | Format, Topics, Qualität | Susanne Lastowicka | umgesetzt, Teamzustimmung offen |
| RabbitMQ unter Docker | Broker mit MQTT-Plugin | Alexander Lux | feste Version, `.env`, Healthcheck und Anleitung umgesetzt; lokaler Lauf geprüft |
| Viewer | Empfang und Anzeige (MQTT-Werkzeug, z. B. MQTT Explorer) | offen | Anleitung offen |

**Protokollwahl: MQTT** (Entscheidung 2026-11-08). Gründe: Das MQTT-Plugin war in der Compose-Datei schon aktiv; ein
fertiges MQTT-Werkzeug kann als Viewer dienen; Topics bilden Gerät und Nachrichtenart direkt ab.

## 3. Funktionale Anforderungen

### 3.1 Gateway (umgesetzt, `src/ModbusRabbitGateway/README.md`)

| ID | Anforderung |
|---|---|
| FA-GW-01 | Geräte über konfigurierbare Geräte-ID, Host, Port und Unit-ID anbinden (`--device ID=host:port[/unit]`, mehrfach). |
| FA-GW-02 | Je Gerät Register 0–49 in einer Anfrage lesen und Kopf sowie gerätespezifische Register nach dem Registerplan (`RegisterPlans.For`) dekodieren. |
| FA-GW-03 | Prüfen, ob das richtige Gerät antwortet (Gerätetyp aus dem ID-Kürzel, Geräte-Nummer aus der ID); sonst Dekodierfehler, keine Messwerte. |
| FA-GW-04 | Abfrage- und Veröffentlichungsintervall konfigurierbar (`--interval`, Standard 1 s; jede gültige Lesung wird veröffentlicht). |
| FA-GW-05 | Als Messzeitpunkt die **aus dem Gerät gelesene** virtuelle Zeit verwenden (`simTime`); die reale Abfragezeit getrennt mitschicken (`polledAtUtc`). |
| FA-GW-06 | Qualitätsstatus aus Zeitstatus, „Simulation läuft“ und Sequenz ableiten; ungültige oder veraltete Werte nie als `good`. |
| FA-GW-07 | In der Pause weiter veröffentlichen, mit `good/paused` und unveränderter `simTime`. |
| FA-GW-08 | Verbindungs- und Dekodierfehler sichtbar melden (Konsole und Status-Topic); dabei keine Messwertnachricht. |
| FA-GW-09 | Status von Gateway und Geräten bei Änderung und zusätzlich alle 30 s (`--status-repeat`) senden. |
| FA-GW-10 | Kontrolliert beenden (`Q`/Strg+C): Gateway-Status `offline` senden, Verbindungen trennen. Bei Absturz setzt der Broker `offline` (Last Will). |
| FA-GW-11 | Ohne Broker betreibbar (`--console`) für Tests und Fehlersuche. |

### 3.2 Viewer (Zuständigkeit offen)

| ID | Anforderung |
|---|---|
| FA-VW-01 | Empfängt die Nachrichten aller verpflichtenden Geräte über RabbitMQ (nicht über Modbus); Abo `energytwin/#`. |
| FA-VW-02 | Zeigt mindestens Geräte-ID, Simulationszeit, Messwerte mit Einheit, Sequenznummer und Qualitätsstatus (im Nachrichteninhalt erkennbar). |
| FA-VW-03 | Verbindungseinstellungen und Abo sind so dokumentiert, dass der Empfang reproduzierbar eingerichtet werden kann. |

### 3.3 Broker (Alexander Lux)

| ID | Anforderung |
|---|---|
| FA-BR-01 | RabbitMQ läuft aus der versionierten Compose-Datei, Image mit genauer Version, Ports 1883 (MQTT), 5672, 15672. |
| FA-BR-02 | Zugangsdaten nur aus `docker/.env` (nicht versioniert); `docker/.env.example` mit Platzhaltern. Das Gateway liest dieselbe Datei. |
| FA-BR-03 | Start, Statuskontrolle und Stoppen sind dokumentiert. |

## 4. Nachrichtenformat

Verbindlich: [`docs/Nachrichtenvertrag-LB03.md`](Nachrichtenvertrag-LB03.md). Kurzfassung:

- Topics: `energytwin/devices/{deviceId}/measurements` (QoS 1), `energytwin/devices/{deviceId}/status` und
  `energytwin/gateway/{gatewayId}/status` (QoS 1, retained). In RabbitMQ: Exchange `amq.topic`, Routing Key mit `.`.
- Messwertnachricht: `deviceId`, `deviceType`, `simTime` (ISO 8601 ohne Zone) + `simTimeEpoch`, `runId`,
  `seq` (Gerätesequenz, Register 11), `publishSeq` (Nachrichtenzähler des Gateways), `polledAtUtc`,
  `quality`/`qualityReason`, `measurements[]` mit `key`, `name`, `address`, `value`, `unit`, `text`.
- Qualität: `good` (running/paused), `uncertain` (standalone/stale/invalidValue), `bad` (masterLost/noTime).
- Messwertnamen deutsch aus dem Registerplan, dazu stabiler ASCII-Schlüssel `key` (späterer InfluxDB-Feldname).

## 5. Konfiguration

| Was | Wie |
|---|---|
| Geräte, Intervalle, Broker | Kommandozeile des Gateways (`--device`, `--interval`, `--stale-after`, `--status-repeat`, `--broker`, `--gateway-id`) |
| Zugangsdaten | `ENERGYTWIN_MQTT_USER`/`ENERGYTWIN_MQTT_PASSWORD` oder `docker/.env` (`RABBITMQ_DEFAULT_USER`/`RABBITMQ_DEFAULT_PASS`); Passwort nie als Option, nie im Repo, nie in der Ausgabe |
| Standardports | Geräte 1502 (PV), 1503 (Batterie), 1504 (Ladesäule in der Demo), 1505 (Kühlhaus); MQTT 1883 |

## 6. Verhalten bei Fehlern

| Fehler | Verhalten |
|---|---|
| Gerät nicht erreichbar | Status `offline` mit Fehlertext, keine Messwerte; Neuversuch im nächsten Intervall; andere Geräte unbeeinflusst |
| Falsches Gerät / ungültige Antwort | Status `decodeError`, keine Messwerte |
| Master verloren / keine Zeit | Messwerte mit `bad/masterLost` bzw. `bad/noTime` |
| Gerät hängt (Sequenz steht trotz laufender Zeit) | `uncertain/stale` nach 60 min Modellzeit (`--stale-after`) |
| Broker nicht erreichbar | einmalige Meldung, Geräte werden weiter gelesen, Messwerte verworfen (Lücke in `publishSeq`), Neuversuch mit 1–30 s Abstand, danach Status nachgeholt; kein Absturz, keine Endlosschleife |
| Zugangsdaten falsch | sichtbare Meldung ohne Passwort, Neuversuch wie oben |

## 7. Randbedingungen

.NET 10, gemeinsame Solution, 0 Warnungen · async/await und `CancellationToken` durchgängig; EasyModbus ist synchron
(begründete Bibliotheksgrenze) · keine Secrets im Repo · kein Internet zur Laufzeit · keine atomare Lesung über mehrere
Register (Einschränkung im Vertrag §6 dokumentiert).

## 8. Akzeptanzkriterien

| ID | Kriterium | Stand 2026-11-08 | Beleg |
|---|---|---|---|
| AK-01 | Die Solution baut mit 0 Fehlern/0 Warnungen, alle bisherigen Tests bleiben grün. | ✅ 258/258 | Journal Susanne, Nachtrag „Gesamttestlauf“ |
| AK-02 | RabbitMQ startet aus der versionierten Compose-Datei, ohne Zugangsdaten im Repo. | ✅ aktualisierte Compose-Datei lokal geprüft; Zugangsdaten nur in ignorierter `.env` (Änderung noch nicht committet) | `docs/nachweise/2026-11-08-docker-alexander/` |
| AK-03 | Alle vier Pflicht-Geräte liefern Nachrichten an den Viewer. | ⚠️ alle vier mit echtem Master an MQTT-Testempfänger nachgewiesen; interaktiver Viewer noch offen | `docs/nachweise/2026-11-08-docker-alexander/` |
| AK-04 | Jede Messwertnachricht enthält DeviceId, virtuelle Zeit, Sequenz, Qualität und Messwerte mit Einheit. | ✅ | Vertrag, `viewer.txt`, Tests `MessageBuilderTests` |
| AK-05 | Ein über Modbus geschriebener Sollwert ist im Simulator, in den Modbus-Daten und in der nächsten Nachricht sichtbar. | ✅ (ein früherer Verlust nicht reproduzierbar, siehe Nachweis) | Nachweis 4 |
| AK-06 | Während der Pause bleibt `simTime` stehen und die Qualität ist `paused`; nach dem Fortsetzen kein Zeitsprung. | ✅ | Nachweis 5 |
| AK-07 | Bei Geräteausfall erscheinen keine Messwerte, sondern `offline`; nach Wiederkehr geht es ohne Neustart des Gateways weiter. | ✅ | Nachweis 6a, Tests |
| AK-08 | Bei Broker-Ausfall stürzt das Gateway nicht ab, meldet den Ausfall und verbindet sich selbst wieder. | ✅ | Nachweis 6b, `MqttPublisherTests` |
| AK-09 | Ohne Master werden Werte nicht als `good` veröffentlicht. | ✅ `bad/masterLost` nach Master-Ende | Nachweis 7, Tests |
| AK-10 | Ein später gestarteter Viewer mit Wildcard-Abo sieht ausgefallene Geräte spätestens nach 30 s. | ✅ | Probelauf 2026-11-08 (Journal), Test `Status_IsRepeated…` |
| AK-11 | Alle Programme lassen sich kontrolliert beenden. | ⚠️ Gateway/Master belegt; Gesamtvorführung im Terminal offen | Nachweis 7 |
| AK-12 | Der Diagnose-Client aus Aufgabe 02 funktioniert weiter. | ✅ 28/28 Tests, Nachweis 4 | – |

## 9. Offene Punkte

1. Zuständigkeit und Einrichtung des interaktiven Viewers; Broker-Einrichtung durch Alexander abgeschlossen.
2. Zustimmung des Teams zum Nachrichtenvertrag (dann Status „Entwurf“ entfernen).
3. Gesamtlauf mit allen vier Geräten und dem echten Master im Terminal (Screenshots für die Abgabe).
4. Optional: Gateway in `start-all.ps1` aufnehmen.
