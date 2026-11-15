# LB03: Docker und Empfang aller vier Geräte – 2026-11-08

**Verantwortlich:** Alexander Lux · **Durchführung:** Codex (GPT-6), lokal auf Alexanders Rechner.

## Durchführung und Ergebnis

- Docker Desktop gestartet (Linux-Engine); eigenes Compose-Projekt `energytwin` angelegt.
- RabbitMQ `4.4.0-management` heruntergeladen und mit `up -d --wait --wait-timeout 180` gestartet: **healthy**.
- Listener geprüft: MQTT 1883, AMQP 5672, HTTP-Management 15672. Host-Ports ausschließlich an `127.0.0.1`.
- Management-API `/api/overview`: Anmeldung mit lokalen `.env`-Zugangsdaten erfolgreich, Version **4.4.0**.
- `git check-ignore docker/.env`: Datei wird ignoriert. Keine Zugangsdaten in diesem Nachweis.
- `dotnet build EnergyTwin.sln`: **0 Fehler, 0 Warnungen**. Kein neuer Unit-Testlauf.
- Echter `WeatherSimulator`, echter `SimulationMaster --autostart --speed 60` sowie alle vier Feldgeräte gestartet.
- Separater MQTT-3.1.1-Testempfänger vor dem Gateway auf `energytwin/#` angemeldet.
- `ModbusRabbitGateway --once`: liest alle Standardgeräte, veröffentlicht über RabbitMQ und beendet sich mit Exit-Code 0.
- Tatsächlich empfangene Messwertnachrichten stehen unverändert als JSON in `mqtt-empfang.txt`.

| Gerät | Modbus-Port | Empfangene Messwerte | Qualität |
|---|---|---|---|
| PV-01 | 1502 | 18 | good/running |
| BAT-01 | 1503 | 21 | good/running |
| DC-01 | 1504 | 19 | good/running |
| KH-01 | 1505 | 15 | good/running |

## Grenzen des Nachweises

Ein kurzer automatisierter Empfangsnachweis, keine interaktive Gesamtvorführung und keine neue Prüfung der
Zeitsynchronisationstoleranz. Gerätemodellzeiten können wegen verschiedener Modellschrittweiten abweichen.
Der MQTT-Testempfänger ersetzt für diesen Nachweis den Viewer; MQTT Explorer wurde noch nicht eingerichtet.
Gateway beendet sich über `--once` geordnet. Die übrigen eigens gestarteten Prüfprozesse wurden anschließend gezielt
beendet; das belegt nicht die Tastenbedienung oder das geordnete Herunterfahren aller Programme.
RabbitMQ bleibt aktiv. Die bestehenden anderen Docker-Container wurden nicht verändert.

Reproduzierbare Einrichtung und Startbefehle: [`docker/README.md`](../../../docker/README.md).
Pause, Ausfälle und Sollwertkette: [früherer Ladesäulen-Nachweis](../2026-11-08-gateway-mit-master/README.md).
