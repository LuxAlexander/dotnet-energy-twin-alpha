# MQTT Explorer – Viewer für EnergyTwin

**Verantwortlich:** Alexander Lux · **Stand:** 2026-11-08  
**Verwendete Version:** MQTT Explorer **0.3.5**, portable Windows-Ausgabe.

Der Viewer empfängt die Daten vom lokalen RabbitMQ-Broker und zeigt Topics, JSON-Nachrichten und Live-Verläufe.
Er greift nicht auf Modbus zu und steuert die Geräte nicht. Wetter bleibt für die Geräte über HTTP angebunden.
Eine fertige Anlagenübersicht mit Statusampeln ist eine **optionale spätere Erweiterung**; ebenso dauerhafte
Zeitreihenspeicherung und eine eigene Diagrammachse für virtuelle Simulationszeit.

## 1. Portable Ausgabe bereitstellen

Die [offizielle stabile Veröffentlichung v0.3.5](https://github.com/thomasnordquist/MQTT-Explorer/releases/tag/v0.3.5)
enthält `MQTT-Explorer-0.3.5.exe` und einen separaten Setup-Installer. Hier wird die Datei **ohne „Setup“** verwendet.
Die Version stammt aus 2019; die inzwischen angebotene 0.4.0-Beta wurde in diesem Aufbau nicht verwendet.

Aus dem Repository-Root in PowerShell:

```powershell
New-Item -ItemType Directory -Force tools/local/mqtt-explorer
Invoke-WebRequest -Uri https://github.com/thomasnordquist/MQTT-Explorer/releases/download/v0.3.5/MQTT-Explorer-0.3.5.exe -OutFile tools/local/mqtt-explorer/MQTT-Explorer-0.3.5.exe
```

Danach die Datei im Explorer öffnen oder starten mit:

```powershell
& .\tools\local\mqtt-explorer\MQTT-Explorer-0.3.5.exe
```

`tools/local/mqtt-explorer/` ist ignoriert; die Programmdatei gehört nicht ins Repository. Das heruntergeladene
Original ist nicht Authenticode-signiert; es wurde aus dem oben verlinkten Projekt-Release bezogen. Der lokal
ermittelte SHA256 steht im [textuellen Nachweis](../nachweise/2026-11-08-viewer/README.md).
Eine Windows-Sicherheitsabfrage gegebenenfalls selbst beurteilen; die Anleitung setzt keine Umgehung voraus.

„Portable“ betrifft die Programmdatei. **Profile und Diagrammauswahl** speichert diese Version trotzdem lokal in
`%APPDATA%\MQTT-Explorer\settings.json`. Das Profil enthält das gespeicherte Passwort; diese Datei nicht versionieren
oder als Teamkonfiguration weitergeben. MQTT Explorer liest Änderungen an `docker/.env` nicht automatisch ein.

## 2. Broker und Simulatoren starten

Voraussetzungen: .NET 10 SDK, Docker Desktop mit Linux-Containern und die eingerichtete lokale `docker/.env`.
Die vollständige Broker-Anleitung steht in [docker/README.md](../../docker/README.md).

```powershell
docker compose --env-file docker/.env -f docker/docker-compose.yml up -d --wait --wait-timeout 180
.\start-all.ps1 -Autostart -NoClient -WeatherMqtt
# Weiteres Terminal:
dotnet run --no-build --project src/ModbusRabbitGateway
```

Das Startskript baut die Solution und startet Wetter, Master und vier Geräte; das Gateway wird separat gestartet.
Belegte Ports werden übersprungen: Ein bereits laufender Wetterdienst muss für den Wetterstrom bereits mit
`--mqtt` gestartet worden sein. Gemeinsamer Aufbau: PV 1502, Batterie 1503, Charger 1504, Kühlhaus 1505.

## 3. Verbindung „EnergyTwin lokal“ anlegen

In MQTT Explorer **Connections → Add** wählen und eintragen:

| Feld | Wert |
|---|---|
| Name | `EnergyTwin lokal` |
| Protocol | `mqtt://` |
| Host | `127.0.0.1` |
| Port | `1883` |
| Username | Wert von `RABBITMQ_DEFAULT_USER` aus lokaler `docker/.env` |
| Password | Wert von `RABBITMQ_DEFAULT_PASS` aus lokaler `docker/.env` |
| Encryption (tls) | aus, entsprechend der lokalen Broker-Konfiguration |
| ADVANCED → MQTT Client ID | eigene eindeutige Kennung, hier `energytwin-viewer-alexander` |

Für weitere gleichzeitig verbundene Viewer andere Client-IDs wählen. Die öffentlichen Beispielprofile sind für
EnergyTwin nicht erforderlich; es wird ausschließlich der lokale Broker verwendet.

Unter **ADVANCED** jedes folgende Topic in **Subscription** eintragen und mit **+** hinzufügen. Vorhandene
Standard-Abos `#` und `$SYS/#` können durch diese gezielte Liste ersetzt werden:

```text
energytwin/#
energytwin/devices/PV-01/status
energytwin/devices/BAT-01/status
energytwin/devices/DC-01/status
energytwin/devices/KH-01/status
energytwin/gateway/GW-01/status
energytwin/weather/status
energytwin/weather/salzburg/measurements
energytwin/weather/wien/measurements
energytwin/weather/innsbruck/measurements
energytwin/weather/tamanrasset/measurements
energytwin/weather/neumayer/measurements
```

Anschließend zurück zur Verbindung, **SAVE**, dann **CONNECT**. Das vorbereitete lokale Profil enthält bereits diese
zwölf Abos. `energytwin/#` zeigt laufend alle neuen Nachrichten. Die konkreten Abos liefern zusätzlich gespeicherte
Retain-Zustände beim späteren Einstieg: Im geprüften RabbitMQ 4.4.0 reicht dafür das Wildcard-Abo allein nicht.
Überlappende Abos sind kein Grund, empfangene Nachrichten als eindeutige Ereigniszählung zu betrachten.

## 4. Werte und Status lesen

Links `127.0.0.1 → energytwin` aufklappen. Erwartet werden **15 Topics**:

- `devices`: PV-01, BAT-01, DC-01 und KH-01 mit jeweils `measurements` und `status` (8 Topics).
- `gateway → GW-01 → status` (1 Topic).
- `weather`: fünf Standorte mit `measurements` sowie ein gemeinsamer Dienststatus (6 Topics).

Ein Topic auswählen. Unter **Value** auf **Raw / formatted JSON** wechseln, wenn die voreingestellte
Differenzansicht alte und neue Felder gleichzeitig zeigt. Änderungen lassen sich alternativ in dieser
Differenzansicht oder über **History** vergleichen.

Gerätenachrichten enthalten `deviceId`, `simTime`, `seq`, `publishSeq`, `quality`, `qualityReason` und
`measurements` mit `key`, `value`, `unit`. Bei Wetter stehen unter anderem `location`, `modelTimeUtc`,
`simTimeUtc`, `runId`, `condition`, `isDaylight` und ebenfalls `measurements` im JSON.

| Beobachtung | Einordnung |
|---|---|
| Gerät `good/running` | Messwerte bei laufender Master-Zeit |
| Gerät `good/paused` | Zeit steht; Gateway sendet weiterhin Momentaufnahmen |
| Wetter `paused` / `continued` | Status bleibt bis zur nächsten Änderung gültig; keine periodische Statuswiederholung nötig |
| Wetter `paused` | Keine regelmäßigen Wetterwerte; manuelle Änderung kann den betroffenen Ort einmal veröffentlichen |
| `noTime` / `masterLost` | Fehlende oder verlorene Master-Zeit; gespeicherte Wetterwerte nicht als neue gültige Daten deuten |
| Gerät `offline` / `decodeError` | Keine neuen Messwerte dieses Geräts; Details im Status |
| Gateway `offline` | Gerätewerte und eventuell noch `online` lautende gespeicherte Gerätestatus sind nicht aktuell bestätigt |
| Wetter `offline` | Letzte Wetterstände bleiben möglicherweise sichtbar; Dienst ist nicht aktiv |

Bei Verbindungsverlust zusätzlich den **Verbindungsstatus von MQTT Explorer** beachten. Alte Werte verschwinden
nicht automatisch aus dem Topic-Baum. Ein Last Will wird vorab vorbereitet; seine Zeitfelder sind kein sicherer
Zeitpunkt der tatsächlichen Ausfallerkennung. Bei Wetter kennzeichnet `isLastWill=true` diesen Fall.
Zeitstempel mit `Utc` oder `Z` sind UTC; MQTT Explorer zeigte seine Empfangszeiten im geprüften Profil lokal an.

## 5. Werteverläufe hinzufügen

1. Das gewünschte `measurements`-Topic auswählen und **Raw / formatted JSON** aktivieren.
2. Im JSON zu `measurements` scrollen und den Eintrag anhand von **`key` und `unit`** identifizieren.
3. Links neben der numerischen **`value`-Zeile** auf das kleine Diagrammsymbol klicken
   (**Add to chart panel**). Nicht versehentlich `address`, `seq` oder eine andere Zahl auswählen.
4. Das Diagramm erscheint links unten. Das Zahnrad öffnet die Diagrammeinstellungen; mit **×** lässt sich die
   Diagrammauswahl entfernen. Der Topic-Baum und das JSON bleiben daneben nutzbar.

Tatsächlich eingerichtet und mit Datenpunkten geprüft:

| Verlauf | Topic | Feldpfad in dieser Payload |
|---|---|---|
| PV-Wirkleistung, kW | `energytwin/devices/PV-01/measurements` | `measurements.2.value`, Eintrag `wirkleistung` |
| Temperatur Salzburg, °C | `energytwin/weather/salzburg/measurements` | `measurements.0.value`, Eintrag `temperature_c` |

Die Feldpfade greifen über den Arrayindex zu. Bei einer Änderung der Reihenfolge muss die Auswahl erneut anhand
des `key` geprüft werden. Derselbe Bedienweg gilt für weitere numerische Geräte-/Wetterwerte; zusätzliche
Kurven wurden in diesem Probelauf nicht einzeln geprüft. PV-Leistung ist gemäß Vertrag bei Einspeisung negativ.

**Die X-Achse ist reale Empfangszeit**, keine virtuelle Simulationszeit. Tempo 60 bedeutet beispielsweise eine
Simulationsminute pro realer Sekunde. Wetter wird stündlich im Modell berechnet und kann deshalb zwischen zwei
Modellstunden trotz vieler Nachrichten konstant bleiben. Pausen, Zustandswechsel und neue Läufe anhand von JSON
und Status bewerten, nicht allein anhand einer flachen Kurve.

Die Diagrammauswahl wurde lokal in `settings.json` gespeichert. Eine dauerhafte Speicherung der Messwert-Historie
über Programmneustarts wurde nicht nachgewiesen; MQTT Explorer ersetzt hier keine Zeitreihendatenbank.

## 6. Prüfstand und Ende

[Textueller Viewer-Nachweis vom 2026-11-08](../nachweise/2026-11-08-viewer/README.md): alle 15 Topics, die beiden
Diagramme, sichtbarer Master-Verlust und Offline-Status nach Prozessende. Keine Screenshots und kein neuer
vollständiger Unit-Testlauf. Broker-Neustart und Pause/Fortsetzen wurden in diesem Viewer-Lauf nicht erneut geprüft;
die [früheren Wetterprüfungen](../nachweise/2026-11-08-wetter-mqtt/README.md) bleiben separate Nachweise.

Für den normalen Abschluss Gateway und Simulatoren mit `Q` beziehungsweise Strg+C beenden und MQTT Explorer mit
**DISCONNECT** trennen. Broker-Stopp ist separat in der Docker-Anleitung beschrieben. Beim begrenzten automatisierten
Probelauf wurden nur die eigenen Prüfprozesse gezielt beendet; das war zugleich die Last-Will-Prüfung und kein
Nachweis ihrer geordneten Tastenbedienung. Broker und MQTT Explorer blieben geöffnet.

Bei fehlenden Daten: Host/Port und Zugangsdaten prüfen, aktive Publisher kontrollieren, Wetter mit `--mqtt`
starten und bei spätem Einstieg die konkreten Abos prüfen. Die RabbitMQ-Webverwaltung auf Port **15672** bleibt
die Broker-Verwaltung; MQTT Explorer verbindet sich auf Port **1883**.

Verbindlich für Nachrichten und Status: [Gerätevertrag](../Nachrichtenvertrag-LB03.md) und
[Wettervertrag](../Wetter-Nachrichtenvertrag-LB03.md). Bestätigter Umfang und spätere Optionen:
[Planung](planung.md). Implementierungsbeleg für die Empfangszeit-Achse:
[TopicPlot.tsx in v0.3.5](https://github.com/thomasnordquist/MQTT-Explorer/blob/v0.3.5/app/src/components/TopicPlot.tsx).
