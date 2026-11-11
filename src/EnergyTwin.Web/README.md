# EnergyTwin.Web – lokale Bedienoberfläche

**Stand:** 2026-11-11. Blazor Interactive Server mit Instanzstart, MQTT-Live-Daten und bestätigter Master-Bedienung.

## Start

.NET 10 SDK, aus dem Repository-Root:

```text
dotnet run --project src/EnergyTwin.Web
```

[Lokale Oberfläche](http://localhost:5180). Loopback-Zugriff unter Windows und Linux vorgesehen;
kein automatischer Browser-, Broker- oder Simulatorstart. Port über `--Web:Port=5181`
oder `Web__Port` (1024–65535). Alle Browserdateien werden lokal ausgeliefert.

Unter „Darstellung“ lassen sich System, Hell und Dunkel wählen. Die Auswahl wird im jeweiligen
Browser gespeichert. Die Modulnavigation kann bei Bedarf aufgeklappt werden.

## Live-Empfang

Unter den Instanzen zeigt „MQTT-Diagnose“ den Verbindungszustand. „Nachrichten anzeigen“ öffnet
die Geräte-, Gateway- und Wetternachrichten aus RabbitMQ samt Suche.
Für Gerätedaten das Gateway starten, für Wetterdaten den Wetterdienst. Der Start über die Web-App aktiviert Wetter-MQTT.
Die Anzeige enthält Messwerte mit Einheit, virtuelle Zeit, Lauf, Sequenz, Qualität,
reale Empfangszeit sowie die empfangene JSON-Nachricht zur Diagnose.
Neue Gerätekennungen erscheinen automatisch. Topics sind durchsuchbar.
Prozessstatus und globale Master-Zeit werden nicht aus einzelnen Geräten abgeleitet.

Verbindung: `Mqtt:Host` (Standard `127.0.0.1`), `Mqtt:Port` (1883), optional `Mqtt:EnvFile`.
Beispiel: `dotnet run --project src/EnergyTwin.Web -- --Mqtt:Port=1884`.
Zugangsdaten kommen aus `ENERGYTWIN_MQTT_USER` und `ENERGYTWIN_MQTT_PASSWORD`, ersatzweise
`docker/.env` im gefundenen Repository-Root. Bei veröffentlichten Dateien außerhalb des Repositories
den Dateipfad ausdrücklich setzen oder Umgebungsvariablen verwenden. Keine Passwörter in appsettings.

MQTTnet empfängt `energytwin/#` mit QoS 1; die fünf Wettertopics, der Wetterstatus sowie
`energytwin/gateway/<ID>/status`, Master-Status und -Antworten sowie je Gerät
`energytwin/devices/<ID>/status` zusätzlich exakt,
damit RabbitMQ gespeicherte Nachrichten sofort liefert. Ohne diese exakten Abos erscheint ein
retained Gateway-Status bei einem Wildcard-Abo erst mit der nächsten Wiederholung (standardmäßig
30 Sekunden). MQTT-Empfang und letzte Nachrichten liegen
browserunabhängig im Singleton; Neuladen verliert sie nicht. Ein Webdienst-Neustart leert sie.
Maximal 512 Topics und 256 KiB Text pro Nachricht werden angenommen, keine dauerhafte Speicherung.
Ungültige JSON-/Schema-Nachrichten werden gezählt und verworfen, leere Nachrichten löschen das Topic.

Verbindungsfehler führen zu Neuversuchen mit 1–30 Sekunden Abstand. Nach Verbindungsverlust sind alle
letzten Werte unbestätigt; auch nach Reconnect bleiben sie das bis zum erneuten Empfang.
Gerätemesswerte werden nach 15 realen Sekunden, Status nach 45 Sekunden als veraltet gekennzeichnet.
Bei abweichenden Publisher-Intervallen können diese konservativen Grenzen zu Warnungen führen.
Qualität und Offline-/Dekodierstatus bleiben sichtbar. Wetterwerte werden mit Source-/Master-Sitzung
und Lauf des Dienststatus abgeglichen. Gespeicherte Pausenstände sind keine Prozess-Lebendbestätigung.
Gerätebefehle bleiben deaktiviert. Master-Befehle werden wie unten beschrieben gesendet; keine Anwendung puffert sie für spätere Neuversuche.

## Live-Verläufe und gerätespezifische Live-Ansichten

Die `/module/{Id}`-Ansichten zeigen Live-Daten, sobald für die Kennung Gerätenachrichten empfangen werden:
Kennzahlen, Verbrauch dekodierender Registerwerte unter „Register" und gerätespezifische Verläufe
(PV/Wirkleistung, Batterie/Lade- und Entladeleistung, Kühlhaus/Ist- und Solltemperatur) mit virtueller Zeit.
Ladesäule zeigt beide Anschlüsse getrennt, Wetter zeigt alle fünf Standorte, Gateway zeigt seine Statusnachricht.
Ohne passende Nachrichten erscheinen die bestehenden, ausdrücklich als Beispiel bezeichneten Ansichten.

Der begrenzte Verlaufsspeicher `Live/LiveTrends.cs` hält je Messwert-Schlüssel bis zu 720 Punkte im Webdienst
(browserunabhängig, konfigurierbar über `Live:History:MaxPoints`), ohne dauerhafte Speicherung; ein WebSocket-/Dienst-Neustart leert ihn.
X-Achse ist die virtuelle Simulationszeit; ein Wechsel der Laufkennung beginnt eine neue Kurve. Punkte ohne
numerischen Wert werden nicht übernommen; Metadaten (Sequenz, Qualität) bleiben der Nachricht entnommen.
Fehlende Seitenzahlen ersetzt der Ringpuffer durch den letzten Stand; Charts zeigen neue Läufe als Trennung der Kurven.

## Short Info und Moduldetails

`/short-info` zeigt die konfigurierten Instanzen als filterbare Karten mit jeweils einem aktuellen Kurzinfo-Wert.
Bei fehlenden, veralteten oder ungültigen Messdaten zeigt die Karte „Keine aktuellen Live-Daten“. Der bisherige
Pfad `/vorschau` öffnet dieselbe Seite. `/module/{Id}` wechselt zwischen Live-Daten (wenn vorhanden)
und gekennzeichneten Beispielansichten für PV, Batterie und Kühlhaus. Ladesäulen-Anschlüsse, Wetter und Infrastruktur
haben eigene Darstellungen.
Der PreviewCatalog ist kein Laufzeitspeicher und dient als Beispielfallback.

## Instanzverwaltung und Prozessverwaltung

Instanzen sind in `instances.local.json` im Projektordner lokal gespeichert (nicht versioniert; Pfad über
`Instances:File`). Über `/instanz/neu` Geräteinstanzen (PV, Batterie, Ladesäule, Kühlhaus) anlegen;
Kennung und Modbus-Port werden vorgeschlagen, Kennungs- und Portkonflikte werden abgelehnt, speichern
allein startet keinen Prozess. Die Startseite gruppiert Instanzen nach Modulgruppe. Jede Instanz hat eine rote
oder grüne Ampel mit Textstatus und einen eigenen Startknopf. Bei aktuellen Live-Daten steht ein farblich
hervorgehobener Kurzinfo-Wert in der Zeile. Grün bei der Ampel steht für einen laufenden verwalteten Prozess, eine per
MQTT gemeldete Gateway-Verbindung, verbundenes RabbitMQ oder einen steuerbereiten Master. Bei
einem belegten Geräteport bleibt der Start gesperrt. Beim Master wechselt die Ampel erst nach
einer aktuellen MQTT-Statusmeldung auf Grün. Bei getrenntem oder verspätet gestartetem Broker
verbinden sich Web-App und Master automatisch erneut.

Im Reiter „Einstellungen" einer Instanz: Starten, Beenden und Löschen
(allein für Geräteinstanzen). „Alles starten" startet Broker (docker compose), den Time Master
(zunächst pausiert), dann Gateway, Wetterdienst und Geräte in dieser Reihenfolge. Sobald die Steuerung
bereit ist, sendet „Alles starten" innerhalb der Wartezeit den Fortsetzen-Befehl. Läuft die Wartezeit
ab, kann der Master nach dem Verbindungsaufbau manuell fortgesetzt werden. Ist ein Port belegt oder läuft
unser Master bereits extern, wird das Modul übersprungen (Meldung pro Modul). „Alles starten" ist deaktiviert, sobald alle Module laufen; „Alles geordnet schließen" stoppt nach Bestätigung Master → Geräte
→ Wetter → Gateway; der Broker bleibt unberührt.

Grenzen: Nach einem Webdienst-Neustart laufen Prozesse weiter und werden an Port und Kommandozeile
wiedererkannt; sie können dann über „Einstellungen → Beenden" bzw. „Alles geordnet schließen"
geordnet beendet werden. Programme fremder Anbieter auf belegten Ports werden geschützt und nicht
berührt. „Alles starten" startet keine Module doppelt: belegt ein Port oder läuft unser Master
bereits extern, wird das Modul übersprungen (Meldung pro Modul). Ein laufendes Gateway wird dagegen
mit der aktuellen Geräteliste neu gestartet, damit neue Anlagen Live-Daten liefern. Sichere Wiedererkennung nach
Neustart bleibt auf Port+Kommandozeile beschränkt. Der verwaltete Master startet mit `--mqtt`; Pause/Fortsetzen ist bei frischem Master-Status verfügbar.
Gerätebefehle bleiben offen. Tempoänderung und neuer Lauf stehen auf der Masterseite bereit. Prozessprotokolle liegen unter
`%TEMP%\energytwin-web\<Kennung>.log`. Der Startknopf für RabbitMQ auf der Startseite verwendet Docker Compose.
Beim Starten, Beenden oder Löschen einer Geräteinstanz wird ein
laufendes Gateway mit der aktuellen Geräteliste neu gestartet, damit neue Anlagen ohne manuellen Neustart
Live-Daten liefern.

## Aufbau und Prüfung

`Live/LiveStore.cs`: threadsicherer letzter Empfangsstand; `Live/MqttReceiver.cs`: Hintergrundempfang;
`Live/LiveTrends.cs`: begrenzter Verlaufsspeicher je Messwert-Schlüssel; `Live/LiveView.cs`: Aufbereitung je Modulart;
`Components/Pages/Live.razor`: Instanzstart, Statusampeln und MQTT-Nachrichten mit Aktualisierung einmal je Sekunde;
`Components/Pages/Home.razor`: filterbare Kurzinfos mit Live-Kennzahlen aus `Live/OverviewMetrics.cs`;
`Components/Shared/LiveChart.razor`: Verlaufsanzeige mit virtueller Zeit und Wertetabelle;
`Management/InstanceStore.cs`: persistierte Instanzkonfiguration mit Konfliktprüfung;
`Management/ProcessManager.cs`: Start/Stop verwalteter Prozesse, Broker-Start über docker compose,
Wiedererkennung extern gestarteter Prozesse über Port und Kommandozeile sowie Doppelstart-Schutz;
`Components/Pages/AddInstance.razor`: Anlegen von Geräteinstanzen;
`Components/Pages/Module.razor`: Live-Beispiel-Wechsel je Kennung und Instanzaktionen im Einstellungen-Reiter;
`Components/Shared/MasterControls.razor`: Tempo, neuer Lauf und Jahreszeiten-Vorlagen der Masterseite;
`Live/MasterControlClient.cs`: gemeinsamer Master-Zustand und bestätigte Aufträge zwischen Browsern;
`Presentation`, `PreviewCatalog` und `PreviewChart`: Beispielansichten.

Befehlsweg und Duplikatschutz prüfen `tests/SimCore.Tests` (`MasterControlTests`) und
`tests/SimulationMaster.Tests` (`MasterMqttServiceTests`) ohne Broker. Tatsächlich ausgeführte Prüfungen stehen im
[Journal](../../docs/development-journal/alexander-lux.md). Ein Build ersetzt weder Broker-Ausfalltests noch einen
Windows-/Linux-Gesamtprobelauf.

## Master-Steuerung über MQTT

Die Anlagenleiste zeigt echte Master-Zeit (einmal je Sekunde gemeldeter Stand), Modus, Tempo und Lauf.
Pause/Fortsetzen wartet auf eine zuordenbare Master-Bestätigung; diese bestätigt die Annahme inklusive
Wirksamkeitszeitpunkt, nicht vorzeitig den bereits wirksamen Zustand. Die Statusanzeige bleibt empfangsbasiert.
Die Master-Detailansicht zeigt zusätzlich Sitzung und Sequenz. Nach zehn Sekunden ohne gültige Statusmeldung
oder bei Broker-Verlust ist die Bedienung gesperrt. Die Uhr zeigt den letzten Stand mit einem Wartehinweis.
Bleiben Master-Meldungen bei offener Broker-Verbindung aus, baut die Web-App den Empfang neu auf.
Bis zur ersten gültigen Meldung erneuert sie die Master-Abos regelmäßig.
Ein Reconnect allein bestätigt alte Werte nicht. Ein fehlender Bescheid nach sechs Sekunden bedeutet
„Ergebnis unklar“, niemals automatisch Erfolg oder erneutes Senden.
Die Schaltfläche in der Anlagenleiste wechselt zwischen „Pausieren“ und „Fortsetzen“ je nach aktuellem
Master-Modus. Ohne bestätigten Status oder während eines angekündigten Wechsels ist sie gesperrt.

Die Masterseite bietet Tempo (1–100.000, „Übernehmen“) auch im laufenden Betrieb; eine pausierte Simulation
bleibt pausiert. „Neuer Lauf“ übernimmt Datum und Uhrzeit erst nach ausdrücklicher Bestätigung und startet sofort.
Gerätezustände bleiben erhalten; die Bestätigung weist darauf hin. Vorlagen setzen den 15.01./15.04./15.07./15.10.
um 06:00 Uhr im aktuellen Simulationsjahr und befüllen nur die Eingabe. Jeder Auftrag besitzt eine UUID und fünf
Sekunden Gültigkeit; der Master prüft Sitzung und Sequenz atomar gegen die Konsole und liefert für doppelte
Aufträge das gespeicherte Ergebnis zurück, sodass ein doppelt zugestellter Neustart keinen zweiten Lauf auslöst.

Der Start über die Web-App oder `start-all.ps1` aktiviert `--mqtt`; beim manuellen Start die Option ergänzen.
Für einen abweichenden Broker den Master mit `--mqtt-host`, `--mqtt-port` und gegebenenfalls `--mqtt-env`
passend zur Web-Konfiguration starten. Der von der Web-App gestartete Master übernimmt `Mqtt:Host`,
`Mqtt:Port` und `Mqtt:EnvFile` aus ihrer Konfiguration.
`Live/MasterControlClient.cs` teilt Zustand und einen ausstehenden Auftrag zwischen Browsern; der vorhandene
`MqttReceiver` transportiert Status, Befehle und Antworten. Master-Topics werden separat verarbeitet und
zählen nicht als Geräte-/Wettertopics des LiveStore. Keine zusätzliche direkte Verbindung zum Master.

Vertrag und Grenzen: [Master-MQTT-Vertrag](../../docs/Master-MQTT-Vertrag.md).
