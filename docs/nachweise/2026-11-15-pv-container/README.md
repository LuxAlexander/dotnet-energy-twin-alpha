# PV-Container mit Windows-Diensten

Manueller Probelauf am 15. November 2026. Geprüft wurde der lokale Arbeitsstand auf Basis von
`a5726b0` mit den damaligen uncommitteten Änderungen und dem [PV-Dockerfile](../../../docker/PvSimulator.Dockerfile).
Die [Startanleitung](../../../docker/README.md#pv-simulator-im-container-lb04) enthält die Befehle
für Build, Start, Logabfrage und Stopp.

## Aufbau

- Docker Desktop mit Linux-Containern, lokales Image `energytwin/pv:lb04`.
- Master und Wetterdienst unter Windows mit ihren unveränderten Loopback-Bindungen. Für den
  isolierten Lauf wurden TCP 52404 und HTTP 52405 verwendet.
- PV `PV-PROBE-f22cc5cf`, Nennleistung 12 kW, Standort Salzburg, Modbus-Unit-ID 1.
  Der Container nutzte `host.docker.internal` für Master und Wetter, der Windows-Modbus-Port 52406
  war auf `127.0.0.1` veröffentlicht. CSV-Ausgabe in einen eingebundenen Windows-Testordner.
- Master-Zeit am 15.04.2026 ab 12:00 Uhr, Tempo ×60, Master mit Autostart.

## Beobachtungen

Der Simulator meldete in Lauf 1 eine synchrone Master-Verbindung und Wetterdaten für Salzburg
mit 13,7 °C und rund 397 W/m². Das Protokoll meldete beim Start einen verspäteten Zeitzustand.
Der vorhandene Modbus-Diagnose-Client las anschließend:

| Register | Wert |
|---|---|
| 4–5 Wirkleistung | −4,76 kW |
| 12 Zeitstatus | 1, synchron mit Master |
| 22–23 Einstrahlung | 396,9 W/m² |
| 37–38 Nennleistung | 12 kW |

Nach `docker stop --timeout 12` meldete der Simulator die Beendigung. Der Container hatte Exit-Code 0.
Auf Windows lag die abgeschlossene CSV mit Kopfzeile und zwei Datenzeilen.

## Nachtrag zur Blazor-Verwaltung

Im isolierten Browserlauf liefen zwei von Blazor gestartete PV-Container parallel im Standalone-Betrieb.
Die erste PV bestätigte „Module reinigen“. Nach dem Neustart waren 11 kW aktiv. Ein Fehlstart mit einer
Datei anstelle des CSV-Ordners ließ sich mit den vorherigen Startwerten beheben. Die neuen Einstellungen
blieben gespeichert. Nach einem Neustart der Web-App wurden beide Container wiedererkannt.
Den zweiten, außerhalb von Blazor gestoppten Container konnte die Web-App erneut starten und beenden.

Der gemeinsame Durchlauf über Gateway und RabbitMQ bis zum MQTT Explorer steht noch aus.
