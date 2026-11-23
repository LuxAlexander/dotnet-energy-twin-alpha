# RabbitMQ für LB03

**Verantwortlich:** Alexander Lux · **Stand:** 2026-11-08

Lokaler Nachrichtenbroker für das Modbus–RabbitMQ-Gateway und den optionalen Wetter-Publisher. Docker Desktop muss mit Linux-Containern laufen.
Die .NET-Programme laufen beim bisherigen Sammelstart auf dem Host. Für den PV-Simulator und das Kühlhaus gibt es
zusätzlich einen [PV-](#pv-simulator-im-container-lb04) bzw.
[Kühlhaus-Containerstart](#kuehlhaus-simulator-im-container-lb04). Für die erstmalige Bereitstellung der Images ist
Internetzugang erforderlich, der Simulationsbetrieb selbst benötigt kein Internet.

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

## PV-Simulator im Container

Alexander Lux verantwortet den PV-Simulator und seine [Container-Konfiguration](PvSimulator.Dockerfile).
Aus dem Repository-Root mit laufendem Docker Desktop (Linux-Container) das Image bauen:

```powershell
docker build --file docker/PvSimulator.Dockerfile --tag energytwin/pv:lb04 .
```

Für den gemeinsamen Lauf Master und Wetterdienst auf Windows starten. m Beispiel nutzt PV die Standardports
5100 und 8080 über `host.docker.internal`. Der Diagnose-Client oder das Gateway erreicht die PV auf Windows
unter `127.0.0.1:1502` mit Unit-ID 1. Die PV-Instanz dieses Beispiels muss die einzige mit der ID `PV-01` sein,
und der Windows-Port 1502 muss frei sein. Die CSV-Dateien landen im Windows-Ordner `docs/data`.
I
```powershell
$csv = (New-Item -ItemType Directory -Force docs/data).FullName
docker run --detach --pull never --name energytwin-pv-01 --publish 127.0.0.1:1502:1502 --mount "type=bind,source=$csv,target=/data" energytwin/pv:lb04 --id PV-01 --master host.docker.internal:5100 --weather http://host.docker.internal:8080 --location salzburg --modbus-bind 0.0.0.0 --output /data
docker ps --filter name=energytwin-pv-01
docker logs --tail 30 energytwin-pv-01
dotnet run --project src/ModbusDiagnosticClient -- --port 1502 --once
```

Zum manuellen Eingriff kann der [Diagnose-Client](../src/ModbusDiagnosticClient/README.md) etwa die
Leistungsbegrenzung setzen. `docker stop` löst den geordneten Stopp aus, inklusive Abmeldung vom Master
und Abschluss der CSV-Datei. Für einen erneuten Start mit geänderten Argumenten den gestoppten Container
entfernen und mit den neuen Argumenten erstellen.

```powershell
docker stop --timeout 12 energytwin-pv-01
docker rm energytwin-pv-01
```

Beim Start aus dem Container gelten die [PV-Startoptionen](../src/Devices/PvSimulator/README.md).
Der eigenständige Konsolenstart steht weiter zur Verfügung. Die bestehende Compose-Datei verwaltet RabbitMQ.

### Verwaltung durch Blazor

Unter „PV-Anlagen“ in der Web-App eine Instanz auswählen oder anlegen und starten.
Fehlt das Image, baut die Web-App es automatisch aus dem vollständigen Repository im Docker-Kontext
`desktop-linux` (Windows). Docker Desktop muss laufen; der erste Build benötigt Internet und kann
mehrere Minuten dauern. Nach spätestens zehn Minuten wird der Buildversuch abgebrochen.
Vorhandene Images werden weiterverwendet; für Quellcodeänderungen das Image manuell neu bauen.
Beim Neustart wird das Image vor dem Stoppen der laufenden PV bereitgestellt.
Buildfehler erscheinen in der Rückmeldung und ausführlich im Webdienst-Protokoll.
Die Web-App legt je PV-ID einen Container an. Sie bindet den eingestellten CSV-Ordner ein, veröffentlicht
den konfigurierten Modbus-Port ausschließlich auf Windows-Loopback und übersetzt lokale Master- und
Wetteradressen für Docker Desktop. Externe Adressen und `Wetter aus` bleiben erhalten.

Start, Live-Bedienung, Neustart und Wiederherstellung laufen über die Web-App. Start gilt erst nach
Bestätigung durch den PV-Steuerzugang. Für die Diagnose in Docker die Container-ID über das PV-Label
ermitteln und die Logs anzeigen:

```powershell
docker --context desktop-linux ps -a --filter label=energytwin.web.pv-id=PV-01
docker --context desktop-linux logs --tail 30 <Container-ID>
```

Die Container laufen bei einem Neustart der Web-App weiter. Nach einem Stopp durch Docker oder einem
Windows-Neustart kann die Anlage erneut über Blazor gestartet werden. Der manuelle Start oben dient
auch der Diagnose außerhalb der Web-App. Für dieselbe Geräte-ID nur einen Startweg gleichzeitig verwenden.

## Kühlhaus-Simulator im Container (LB04)

Das Kühlhaus ist wie die PV-Anlage als Linux-Container verfügbar; verantwortlich ist Dominik Zach.
Die [Container-Konfiguration](KuehlhausSimulator.Dockerfile) baut das Image aus dem vollständigen Repository.
Aus dem Repository-Root mit laufendem Docker Desktop (Linux-Container):

```powershell
docker build --file docker/KuehlhausSimulator.Dockerfile --tag energytwin/kh:lb04 .
```

Der manuelle Start erwartet Master und Wetterdienst auf dem Windows-Host. Das Image bringt die passenden
Standardargumente mit (`--master host.docker.internal:5100`, `--weather http://host.docker.internal:8080`,
`--modbus-bind 0.0.0.0`, `--output /data`). Das Beispiel veröffentlicht den Modbus-Standardport 1505 nur auf
Windows-Loopback und schreibt die CSV in den Windows-Ordner `docs/data`:

```powershell
$csv = (New-Item -ItemType Directory -Force docs/data).FullName
docker run --detach --pull never --name energytwin-kh-01 --publish 127.0.0.1:1505:1505 --mount "type=bind,source=$csv,target=/data" energytwin/kh:lb04 --id KH-01
docker ps --filter name=energytwin-kh-01
docker logs --tail 30 energytwin-kh-01
dotnet run --project src/ModbusDiagnosticClient -- --port 1505 --once
```

`docker stop --timeout 12` löst auch hier den geordneten Stopp aus: Unter Linux empfängt der Prozess SIGTERM,
meldet sich beim Master ab, schließt die CSV und gibt den Modbus-Port frei. Für geänderte Argumente den
gestoppten Container entfernen und neu erstellen. Beim Start aus dem Container gelten die
[Kühlhaus-Startoptionen](../src/Devices/KuehlhausSimulator/README.md).

### Verwaltung durch Blazor

Unter „Kühlhäuser“ in der Web-App eine Instanz auswählen oder anlegen und starten. Fehlt das Image
`energytwin/kh:lb04`, baut die Web-App es automatisch aus dem vollständigen Repository im Docker-Kontext
`desktop-linux` (Windows); der erste Build benötigt Internet und kann mehrere Minuten dauern (Abbruch nach
höchstens zehn Minuten). Vorhandene Images werden weiterverwendet; für Quellcodeänderungen das Image manuell
neu bauen. Beim Neustart wird das Image vor dem Stoppen des laufenden Kühlhauses bereitgestellt.

Die Web-App legt je Kühlhaus-ID einen Container `energytwin-kh-<Hash>` an. Sie bindet den eingestellten
CSV-Ordner ein, veröffentlicht den konfigurierten Modbus-Port ausschließlich auf Windows-Loopback und
übersetzt lokale Master- und Wetteradressen für Docker Desktop. Externe Adressen und `Wetter aus` bleiben
erhalten. Über die Umgebungsvariable `ENERGYTWIN_KUEHLHAUS_CONTAINER_CONTROL=1` aktiviert sie den lokalen
Steuerzugang auf Port **5192** (PV nutzt 5191). Auch dieser Port wird nur auf `127.0.0.1` veröffentlicht und
dient der Live-Bedienung, dem geordneten Stopp und der Wiedererkennung; der Start gilt erst nach Bestätigung
durch diesen Zugang. Der Container wird nach einem Web-App-Neustart über die Labels `energytwin.web.kh-run`
und `energytwin.web.kh-id` wiedererkannt. Für die Diagnose:

```powershell
docker --context desktop-linux ps -a --filter label=energytwin.web.kh-id=KH-01
docker --context desktop-linux logs --tail 30 <Container-ID>
```

Nach einem Stopp durch Docker oder einem Windows-Neustart kann das Kühlhaus erneut über Blazor gestartet
werden. Für dieselbe Geräte-ID nur einen Startweg gleichzeitig verwenden. Ein containergeführter Lauf des
Kühlhauses ist bisher nicht als Nachweis dokumentiert.

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
