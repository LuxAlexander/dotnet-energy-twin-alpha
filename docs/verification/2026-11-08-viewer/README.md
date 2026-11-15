# MQTT Explorer – textueller Viewer-Nachweis 2026-11-08

**Verantwortlich:** Alexander Lux · **Durchführung:** Codex, lokal auf Alexanders Rechner.  
**Freigabe:** Nach dem Planungsinterview ausdrücklich „Starte mit der Umsetzung“, einschließlich begrenztem Probelauf.

## Aufbau

- MQTT Explorer 0.3.5, offizielle portable Windows-Datei, lokal unter `tools/local/mqtt-explorer/` (ignoriert).
- Download: [Projekt-Release](https://github.com/thomasnordquist/MQTT-Explorer/releases/tag/v0.3.5).
- Lokal ermittelter SHA256: `F155B7E96D0D5C666B1A446E56AE22859276149D61CD981049099BF0A1114886`.
  Das ist eine Kennung der heruntergeladenen Datei, kein Abgleich mit einem unabhängig veröffentlichten Herstellerhash.
  Authenticode-Status: `NotSigned`; keine Sicherheitsabfrage umgangen.
- Bestehender RabbitMQ 4.4.0-Projektbroker, `energytwin-rabbitmq-1`, gesund; kein Broker-Neustart im Probelauf.
- Profil `EnergyTwin lokal`, MQTT `127.0.0.1:1883`, eigene Client-ID, zwölf Abos gemäß [Anleitung](../../viewer/README.md).
- Echte Programme aus dem aktuellen Solution-Build: `SimulationMaster --autostart --speed 60 --start "2026-06-21 12:00"`,
  `WeatherSimulator --mqtt`, PV-01 1502, BAT-01 1503, DC-01 1504, KH-01 1505, `ModbusRabbitGateway` mit Standardgeräten.
- Prüfprogramme ohne eigene Konsolenfenster mit umgeleiteten Ausgaben gestartet. MQTT Explorer lief als sichtbares
  Windows-Programm; Topic-Baum, JSON und Diagramme wurden über dessen Oberfläche beobachtet und bedient.

## Beobachtungen

| Prüfung | Tatsächlich beobachtet |
|---|---|
| Verbindung | Lokales Profil verbindet sich; Viewer zeigt aktive Verbindung und neue Nachrichten |
| Alle Geräte | `devices` enthält PV-01, BAT-01, DC-01, KH-01 mit jeweils Messwert- und Status-Topic |
| Gateway | `gateway/GW-01/status` vorhanden |
| Wetter | Salzburg, Wien, Innsbruck, Tamanrasset, Neumayer sowie Wetterstatus sichtbar |
| Umfang | 15 Topics insgesamt: 8 Geräte-, 1 Gateway- und 6 Wetter-Topics |
| Geräte-JSON | PV-Nachricht zeigt Kennung, virtuelle Zeit, Sequenz, Qualität und Messwerte mit Einheiten |
| PV-Verlauf | `measurements.2.value` im PV-Topic ausgewählt; Punkte und wechselnde negative Wirkleistung sichtbar |
| Wetter-Verlauf | `measurements.0.value` im Salzburg-Topic ausgewählt; Temperaturpunkte und Änderung zwischen Modellstunden sichtbar |
| Lokale Diagrammauswahl | Beide Topic-/Feldpfade in `connection_view_state` gespeichert; keine historische Messwertpersistenz behauptet |
| Master-Verlust | Eigenen Master beendet; Wetterstatus `masterLost` im Viewer und Gerätequalität `bad/masterLost` im Gateway-Protokoll |
| Wetterprozess beendet | Viewer empfängt `offline` mit `isLastWill=true` |
| Gateway beendet | Viewer zeigt `gatewayId=GW-01`, `state=offline` |

Direkt aus dem im Viewer beobachteten Nachrichtentext gesichert:
[Wetter Master verloren](wetter-masterlost.json), [Wetter Last Will](wetter-offline-lastwill.json),
[Gateway offline](gateway-offline-lastwill.json). Keine Zugangsdaten in diesen Dateien.
Last-Will-Zeitfelder stammen aus der vorbereiteten Nachricht und sind keine Messung der tatsächlichen Ausfallzeit.

## Grenzen und Abschluss

Kein vollständiger neuer Unit-Testlauf. Keine neue Prüfung von Broker-Neustart, globaler Pause/Fortsetzung,
Sollwertschreiben oder geordnetem Beenden per Taste. Die vorherigen Funktionsnachweise bleiben getrennt.
Der Viewer zeigte Empfangs-QoS 0; dieser Lauf ist kein Nachweis eines QoS-1-Empfangs im Viewer.
Ein separater Wiedereinstieg bei ausschließlich gespeicherten Retain-Werten wurde hier nicht isoliert geprüft;
die konkreten Abos sind eingerichtet, der frühere Wetter-Nachweis prüfte Retain separat.

Die X-Achse verwendet reale Empfangszeit; dies ist zusätzlich im offiziellen `TopicPlot.tsx` der verwendeten Version
nachvollziehbar. Messwert-Historie nach Programmneustart, andere Windows-Rechner und weitere einzelne Diagrammfelder
wurden nicht geprüft. Keine eigene automatische Statusampel und keine dauerhafte Speicherung ergänzt.

Zum Ende wurden ausschließlich die sieben protokollierten eigenen Prüfprozesse beendet. Ihre Prozesskennungen
und Programmpfade wurden vor dem Beenden zugeordnet. Master-/Wetter-/Gateway-Abbruch war absichtlich Teil der
Ausfallprüfung; damit wird kein geordnetes Herunterfahren behauptet. RabbitMQ blieb gesund, MQTT Explorer geöffnet.
Keine fremden Prozesse beendet; kein Commit/Push. Der abschließende Build steht im Alexander-Journal.
