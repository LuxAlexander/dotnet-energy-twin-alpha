# Wetter-Nachrichtenvertrag LB03 – Version 1

**Stand:** 2026-11-08 · **Verantwortlich:** Alexander Lux

Zusätzlicher Wetterstrom für den Viewer. Die Feldgeräte verwenden weiterhin HTTP. Unabhängig vom Gerätevertrag
in `Nachrichtenvertrag-LB03.md`; Wetter wird direkt vom Wettersimulator veröffentlicht, nicht über Modbus gelesen.

## Topics und Empfang

| Topic | Inhalt |
|---|---|
| `energytwin/weather/salzburg/measurements` | Wetter Salzburg |
| `energytwin/weather/wien/measurements` | Wetter Wien |
| `energytwin/weather/innsbruck/measurements` | Wetter Innsbruck |
| `energytwin/weather/tamanrasset/measurements` | Wetter Tamanrasset (Sahara) |
| `energytwin/weather/neumayer/measurements` | Wetter Neumayer III (Antarktis) |
| `energytwin/weather/status` | Zustand des Wetterdienstes |

MQTT QoS 1 und **Retain** für alle sechs Topics. Eine leere retained Nachricht löscht den gespeicherten Wert;
Viewer müssen diese Löschungen verarbeiten. Für den späteren Einstieg alle sechs Topics **konkret** abonnieren:
Der geprüfte RabbitMQ-Aufbau liefert gespeicherte Nachrichten nicht an das Wildcard-Abo `energytwin/#`.
Live-Nachrichten können weiterhin mit diesem Wildcard-Abo beobachtet werden.

QoS 1 erlaubt doppelte Zustellung. `messageId` identifiziert eine Nachricht; `seq` steigt innerhalb von
`sourceSession` über Wetter- und Statusnachrichten gemeinsam. Lücken sind möglich und kein Fehler.

## Wetterstand

| Feld | Bedeutung |
|---|---|
| `schemaVersion` | `1` |
| `sourceSession` | Kennung des Wetterprozesses |
| `seq`, `messageId` | Zähler und eindeutige Nachrichtenkennung (`sourceSession:seq`) |
| `masterSession`, `runId` | Master-Sitzung und Lauf; erst zusammen eindeutig über Master-Neustarts hinweg |
| `location` | stabile Standortkennung aus der Topictabelle |
| `modelTimeUtc` | Stundenbeginn des berechneten Wetters |
| `simTimeUtc` | aktuelle virtuelle Zeit des Masters beim Erstellen der Nachricht |
| `publishedAtUtc` | reale Zeit beim Erstellen, keine Zustellbestätigung |
| `mode` | `automatic` oder `manual`; auch teilweise Übersteuerung zählt als `manual` |
| `condition` | `Sunny`, `PartlyCloudy`, `Overcast`, `Storm`, `Snow`, `Night` |
| `isDaylight` | Sonnenhöhe > 0; unabhängig vom Wetterzustand, sodass Schnee/Sturm in der Nacht sichtbar bleiben |
| `measurements` | Werte als `{key, value, unit}` gemäß Tabelle |

| key | Einheit | Bedeutung |
|---|---|---|
| `temperature_c` | °C | Lufttemperatur |
| `irradiation_w_m2` | W/m² | Globalstrahlung |
| `wind_speed_m_s` | m/s | Windgeschwindigkeit (Modellreferenz 10 m) |
| `precipitation_mm_h` | mm/h | Niederschlagsrate als Wasseräquivalent, auch bei Schnee |
| `cloud_cover_percent` | % | Bedeckung |
| `sun_height_degrees` | ° | Sonnenhöhe; negativ bei Nacht |

Zeitstempel sind ausdrücklich UTC. Das Wetter bleibt innerhalb derselben Modellstunde konstant, sofern keine
manuelle Änderung erfolgt. Jede reale Sekunde wird der neueste Stand aller fünf Orte veröffentlicht; bei hohem Tempo
übersprungene Modellstunden werden nicht nachgeliefert. HTTP-Abfragen historischer Zeiten verändern den Strom nicht.

## Dienststatus

Gleiche Kopffelder `schemaVersion`, `sourceSession`, `seq`, `messageId`, `masterSession`, `runId`, `publishedAtUtc`;
zusätzlich `status`, `simTimeUtc` (ohne gültige Master-Zeit `null`) und `isLastWill`.

| status | Verhalten |
|---|---|
| `noTime` | Noch keine gültige Master-Zeit; keine Wetterveröffentlichung |
| `continued` | Master läuft, regelmäßige Wetterveröffentlichung aktiv |
| `paused` | Einmaliger Zustandswechsel; regelmäßige Wetterveröffentlichung ausgesetzt |
| `masterLost` | Master nicht mehr synchron erreichbar; keine neuen Wetterwerte |
| `offline` | Wetterprozess geordnet beendet oder MQTT-Verbindung abgebrochen |

Kein periodisches Wiederholen von Statusnachrichten. Der gespeicherte Status ist eine Zustandsaussage;
beispielsweise bleibt `continued` gültig, bis ein anderer Status folgt. Viewer müssen Broker-Verlust selbst anzeigen.
Status nach Wiederverbindung wird erneut übertragen, auch wenn der fachliche Zustand gleich geblieben ist.

Bei Pause bleiben gespeicherte Werte sichtbar. Eine manuelle Änderung sendet einmal den betroffenen Standort mit
unveränderter Modellzeit. Bei initial bereits pausiertem Master, neuer RunId oder Wiederverbindung werden einmal
Anfangsstände bereitgestellt. Neuer Lauf oder neue Master-Sitzung löschen manuelle Vorgaben; ein bloßer Reconnect nicht.

**Gültigkeit gespeicherter Werte:** Nur Wetter mit passender `sourceSession`, `masterSession` und `runId` zum
Dienststatus als aktuellen Stand behandeln. `noTime`, `masterLost` und `offline` machen gespeicherte Messwerte als
aktuellen Stand ungültig. Beim Start/Wechsel werden alte Wetterstände gelöscht und durch neue ersetzt, sobald Zeit gültig ist.
Die sechs MQTT-Nachrichten sind keine atomare Transaktion; Zwischenstände beim Wiederaufbau sind möglich.

**Last Will:** `offline` mit `isLastWill=true` wird bereits beim Verbindungsaufbau vorbereitet und später vom Broker
gesendet. Sein Zeitstempel und Zähler gehören zum Erstellungszeitpunkt, nicht zum Ausfallzeitpunkt. Es darf nicht wegen
einer kleineren Sequenznummer als die zuletzt empfangene Wetternachricht verworfen werden. Empfangszeit separat führen.

## Ausfälle und Beenden

Bei Broker-Ausfall läuft HTTP weiter. MQTT versucht es mit wachsendem Abstand (1–30 reale Sekunden) erneut;
keine historische Warteschlange. Nach Wiederverbindung aktuellen Status und bei gültiger Zeit aktuelle Wetterstände senden.
Fehler und Wiederverbindung erscheinen in der Konsole; Zugangsdaten werden nie ausgegeben.

Beenden mit `Q`/Strg+C sendet nach Möglichkeit `offline` und trennt geordnet. Bei Abbruch übernimmt der Broker das Last Will.
Wenn der Broker selbst ausfällt, kann er währenddessen keine Offline-Meldung zustellen.
