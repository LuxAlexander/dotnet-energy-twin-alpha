# RabbitMQ für LB03

**Verantwortlich:** Alexander Lux · **Stand:** 2026-11-08

Lokaler Nachrichtenbroker für das Modbus–RabbitMQ-Gateway und den optionalen Wetter-Publisher. Docker Desktop muss mit Linux-Containern laufen;
die eigenen .NET-Programme laufen auf dem Host. Beim ersten Start wird das Image aus dem Internet geladen,
der Simulationsbetrieb selbst benötigt kein Internet.

## Einrichtung und Start

Alle folgenden Befehle werden im **Repository-Root** in PowerShell ausgeführt.

1. Docker Desktop starten und auf die laufende Engine warten (`docker info`).
2. Einmalig `Copy-Item docker/.env.example docker/.env` ausführen, sofern noch keine `.env` existiert.
3. In `docker/.env` den Passwort-Platzhalter durch ein eigenes zufälliges Passwort ersetzen.
   Benutzer und Passwort werden von Broker, Gateway und Wetter-MQTT aus derselben Datei gelesen.
   Für identische Auswertung in Compose und Gateway einfache Werte ohne Leerzeichen, `$` oder Inline-Kommentare
   verwenden (z. B. ein zufälliges Hex-Passwort). `.env` ist durch `.gitignore` ausgeschlossen.
4. Konfiguration prüfen und Broker starten:

```powershell
docker compose --env-file docker/.env -f docker/docker-compose.yml config --quiet
docker compose --env-file docker/.env -f docker/docker-compose.yml up -d --wait --wait-timeout 180
docker compose --env-file docker/.env -f docker/docker-compose.yml ps
```

`config --quiet` prüft ohne Zugangsdaten auszugeben. Fehlende oder leere Zugangsdaten brechen die Konfiguration ab.
Die Beispielwerte sind Platzhalter und müssen vor dem Start ersetzt werden.

## Konfiguration und Kontrolle

| Einstellung | Wert |
|---|---|
| Image | `rabbitmq:4.4.0-management` |
| Compose-Projekt | `energytwin` |
| MQTT | `127.0.0.1:1883`, Plugin `rabbitmq_mqtt`, keine anonyme Anmeldung |
| AMQP | `127.0.0.1:5672` |
| Management | <http://localhost:15672>, Plugin `rabbitmq_management` |
| Zugangsdaten | `RABBITMQ_DEFAULT_USER` und `RABBITMQ_DEFAULT_PASS` in `docker/.env` |
| Broker-Konfiguration | `rabbitmq.conf`, nur lesend eingebunden |
| Plugins | `enabled_plugins`, nur lesend eingebunden |
| Daten | benanntes Volume `energytwin_rabbitmq-data`, stabiler Hostname |
| Bereitschaft | laufende RabbitMQ-Anwendung und erreichbare Listener; `up --wait` wartet darauf |

Die Ports sind nur auf diesem Rechner erreichbar. Anmeldung in der Management-Oberfläche mit den Werten aus `.env`.
Für einen MQTT-Viewer: Host `127.0.0.1`, Port `1883`, dieselben Zugangsdaten, kein TLS, Topic `energytwin/#`.
Die Management-Oberfläche ist eine Broker-Verwaltung, kein Ersatz für den Messwert-Viewer. Ein späteres Wildcard-Abo
liefert bei dem hier geprüften RabbitMQ 4.4.0 keine gespeicherten Retain-Werte; die exakten Topics stehen in den
[Geräte-](../docs/Nachrichtenvertrag-LB03.md) und [Wetterverträgen](../docs/Wetter-Nachrichtenvertrag-LB03.md).

```powershell
docker compose --env-file docker/.env -f docker/docker-compose.yml exec rabbitmq rabbitmq-diagnostics listeners
docker compose --env-file docker/.env -f docker/docker-compose.yml logs --tail 60 rabbitmq
```

## Simulatoren und Gateway

Viewer-Profil, konkrete Abos und JSON-Verläufe: [MQTT Explorer](../docs/viewer/README.md).

```powershell
dotnet build EnergyTwin.sln
.\start-all.ps1 -Autostart -NoClient -WeatherMqtt
```

Das vorhandene Startskript öffnet Wetter, Master und alle vier Feldgeräte in eigenen Fenstern.
`-WeatherMqtt` schaltet den zusätzlichen Wetterstrom ein; ohne den Schalter läuft Wetter nur über HTTP.
Für gespeicherte Wetterwerte und Pausenstatus muss der Viewer die sechs konkreten Wettertopics aus dem
[Wettervertrag](../docs/Wetter-Nachrichtenvertrag-LB03.md) abonnieren. Ein bereits laufender Wetterdienst wird
übersprungen und muss für diese Funktion bereits mit `--mqtt` gestartet sein.
Danach in einem weiteren Terminal:

```powershell
dotnet run --no-build --project src/ModbusRabbitGateway
```

Das Gateway verwendet standardmäßig PV 1502, Batterie 1503, Charger 1504 und Kühlhaus 1505 sowie `docker/.env`.
Bereits gesetzte `ENERGYTWIN_MQTT_USER`/`ENERGYTWIN_MQTT_PASSWORD` haben Vorrang vor der Datei.
Zum kurzen Prüfen: `dotnet run --no-build --project src/ModbusRabbitGateway -- --once --json`.
Ohne laufende Geräte sind `offline`-Statusmeldungen zu erwarten.

## Stoppen und erneuter Start

Die Simulatoren und das Gateway mit `Q` bzw. Strg+C beenden, dann:

```powershell
docker compose --env-file docker/.env -f docker/docker-compose.yml stop
# Wieder starten, einschließlich Bereitschaftsprüfung:
docker compose --env-file docker/.env -f docker/docker-compose.yml up -d --wait --wait-timeout 180
# Alternativ Container und Netzwerk entfernen; das Datenvolume bleibt erhalten:
docker compose --env-file docker/.env -f docker/docker-compose.yml down
```

**Persistenz:** Die Default-Zugangsdaten werden nur bei einer leeren Broker-Datenbank angelegt. Eine spätere Änderung
an `.env` ändert bestehende RabbitMQ-Benutzer nicht automatisch. Passwort in RabbitMQ (Management-Oberfläche) und
`.env` gemeinsam ändern. Kein `down -v` für einen normalen Neustart: Das würde die Broker-Daten löschen.

## Fehlerbehebung

- Engine nicht erreichbar: Docker Desktop starten, Linux-Container auswählen, `docker info` prüfen.
- Port belegt: vorhandenen Dienst identifizieren; keine fremden Container automatisch beenden.
- Datei-Mount fehlerhaft: `rabbitmq.conf` und `enabled_plugins` müssen Dateien sein. Compose legt fehlende Pfade
  bewusst nicht als Verzeichnisse an.
- Anmeldung abgelehnt: `.env`, eventuell gesetzte Gateway-Umgebungsvariablen und bestehende Broker-Benutzer prüfen.
- Broker noch nicht bereit: `ps` und `logs` prüfen; Image-Download benötigt beim ersten Start eine Verbindung.

Grundlagen: [RabbitMQ MQTT-Konfiguration](https://www.rabbitmq.com/docs/mqtt),
[Compose-Service-Konfiguration](https://docs.docker.com/reference/compose-file/services/).

## Vorhandene Nachweise

- [2026-11-08: Empfang aller vier Geräte](../docs/nachweise/2026-11-08-docker-alexander/README.md) über Master, Modbus, Gateway und RabbitMQ.
- [2026-11-08: Wetter-MQTT](../docs/nachweise/2026-11-08-wetter-mqtt/README.md), unter anderem Retain, Pause, Master-/Broker-Ausfall und Wiederanlauf.

Die Nachweise beschreiben ihren jeweiligen Aufbau; sie ersetzen keine interaktive Gesamtvorführung oder erneuten
Unit-Testlauf nach späteren Codeänderungen.
