# Wetter über MQTT – Funktionsnachweis 2026-11-08

**Verantwortlich:** Alexander Lux · **Durchführung:** Codex (GPT-6.1)
**Vertrag:** [`Wetter-Nachrichtenvertrag-LB03.md`](../../Wetter-Nachrichtenvertrag-LB03.md)

## Aufbau

Isolierter temporärer RabbitMQ-4.4.0-Broker auf MQTT-Port 1884, dieselben Plugin-/Konfigurationsdateien und lokalen
Zugangsdaten wie beim Projektbroker. Der laufende Projektbroker auf 1883 wurde nicht unterbrochen.
Die Prüfung verwendet echte `MasterClock`-/`TimeSyncServer`-Klassen und `DeviceTimeSource` über TCP,
Produktionsmodell, manuellen Zustand und `WeatherPublisher` über MQTTnet. Ein unabhängiger MQTT-Client empfängt
die sechs konkret abonnierten Themen. Der Prüfablauf verändert Pause/RunId und manuelle Vorgaben direkt an den
Produktionsklassen; er ersetzt keine interaktive Tastaturprüfung.

Zusätzlich wurden die tatsächlichen Programme `SimulationMaster` (15101, Autostart, Tempo 1) und
`WeatherSimulator --port 18080 --mqtt --master 127.0.0.1:15101 --broker 127.0.0.1:1884` gestartet.
Ein separater MQTT-3.1.1-Empfänger verglich die sechs numerischen Größen jeder Nachricht mit HTTP-Abfragen
derselben Modellstunde und Laufnummer. Danach ein zusätzlicher HTTP-Start ohne `--mqtt`.

## Ergebnisse

| Prüfung | Ergebnis |
|---|---|
| Standorte / Stundenkonstanz | Fünf Orte; reproduzierbar mit neuen Modellinstanzen, auch innerhalb der Stunde |
| Neumayer im Juni / Dezember | Keine Strahlung in Polarnacht; Sonnenhöhe am Dezember-Solstitium alle 24 Stunden positiv |
| Jahreszeiten | Antarktis im Dezember wärmer als Juni; Sahara im Juni wärmer als Dezember |
| Schnee in Polarnacht | `Snow`, nicht durch `Night` verdeckt |
| Sahara-Niederschlag | Ein vollständiges synthetisches Jahr, Lauf 7: 63,0 mm; endliche Werte in allen Stunden |
| Fehlender Master | `noTime`, keine Wetterwerte |
| Initial pausierter Master | Ein `paused` und fünf Anfangsstände; danach keine regelmäßigen Wetterwerte |
| Manuelle Änderung in Pause | Genau ein Standort, `manual`, unveränderte Modellzeit; andere Orte unbeeinflusst |
| Später Viewer | Alle fünf Wetterstände plus Status tatsächlich als retained empfangen |
| Fortsetzen | `continued`, Wetterstrom aktiv; keine periodische Wiederholung des Status |
| Neue RunId | Manuelle Vorgaben gelöscht, gespeicherte Werte des alten Laufs ersetzt |
| Broker gestoppt / gestartet | Wiederverbindung, aktueller manueller Stand auch in Pause nachgeholt; Vorgabe bleibt erhalten |
| Master-Ende | `masterLost`, auch bei manueller Änderung keine neue Wetternachricht |
| Ganz neuer Master mit gleicher RunId | Über `masterSession` erkannt; alte manuelle Vorgaben gelöscht |
| Geordnetes Publisher-Ende | `offline` empfangen |
| Tatsächliche Programme: HTTP/MQTT | Alle sechs Messgrößen je Standort stimmen exakt überein |
| HTTP-Fehler | Unbekannter Standort 404, ungültige Zeit 400 |
| Abruptes Ende des Wetterprozesses | Broker sendet `offline` mit `isLastWill=true` |
| HTTP ohne `--mqtt` | Fünf Standorte und Wetterabfrage funktionieren weiterhin |

Unverändertes Protokoll des erfolgreichen Klassen-/MQTT-Prüflaufs: [`pruefung.txt`](pruefung.txt).
Tatsächlich empfangene JSON-Messwertnachrichten der gestarteten Programme: [`mqtt-empfang.txt`](mqtt-empfang.txt).
Keine Zugangsdaten in diesen Dateien.

Ein früherer Prüfversuch wurde verworfen: Der Prüfer nahm fälschlich erste RunId 0 statt 1 an. Nach Korrektur der
Prüfannahme erfolgreich; anschließend separat der gleiche-RunId/neue-MasterSession-Fall geprüft.

## Grenzen und Wiederholung

Keine wissenschaftliche Validierung der Klimaverteilungen. Der Niederschlagsjahreswert ist eine deterministische
Stichprobe, kein Beweis einer Verteilung. Quellen und synthetische Parameter stehen im Wetter-README.
Interaktive Standortwahl/Tastatureingabe im echten Terminal und ein allgemeiner MQTT-Viewer sind noch live zu zeigen.
Die bestehenden Unit-Tests wurden gemäß Projektregel nicht neu ausgeführt; 7 neue Prüffälle sind gebaut.

Für eine manuelle Wiederholung: Broker laut `docker/README.md` starten, Master und
`dotnet run --project src/WeatherSimulator -- --mqtt` starten. Sechs konkrete Wettertopics abonnieren.
Im Master Pause/Fortsetzen/Neustart durchführen; im Wetterfenster mit `L` Standort wählen, `T` Temperatur ändern,
mit `X` auf Automatik zurück. Broker stoppen/starten, Master beenden/neustarten, anschließend Wetter mit `Q` beenden.
Gespeicherte Nachrichten nur zusammen mit Dienststatus und passender Prozess-/Master-/Laufkennung bewerten.

Die eigens gestarteten Prüfprogramme wurden beendet; der temporäre Broker wurde nach Prüfung seiner Container-ID entfernt.
Der Projektbroker bleibt aktiv. Kein Commit und kein Push.
