# EnergyTwin.Web – lokale Bedienoberfläche

**Stand:** 2026-11-10. Blazor Interactive Server mit MQTT-Live-Anbindung und bestätigter Master-Bedienung.

## Start

.NET 10 SDK, aus dem Repository-Root:

```text
dotnet run --project src/EnergyTwin.Web
```

[Lokale Oberfläche](http://localhost:5180). Loopback-Zugriff unter Windows und Linux vorgesehen;
kein automatischer Browser-, Broker- oder Simulatorstart. Port über `--Web:Port=5181`
oder `Web__Port` (1024–65535). Alle Browserdateien werden lokal ausgeliefert.

## Live-Empfang

Die Startseite `/` zeigt Geräte-, Gateway- und Wetternachrichten aus RabbitMQ.
Gateway und Wetterdienst müssen separat gestartet sein; Wetter benötigt seine MQTT-Option.
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
`energytwin/gateway/<ID>/status` und je Gerät `energytwin/devices/<ID>/status` zusätzlich exakt,
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

## Beispielansichten und offene Arbeit

`/vorschau` enthält die filterbare Beispielübersicht. `/module/{Id}` wechselt zwischen Live-Daten (wenn vorhanden)
und gekennzeichneten Beispielansichten für PV, Batterie und Kühlhaus; Ladesäulen-Anschlüsse, Wetter und Infrastruktur
haben eigene Darstellungen. Die Navigation kennzeichnet die Trennung zur Live-Startseite.
Der PreviewCatalog ist kein Laufzeitspeicher und dient als Beispielfallback.

## Instanzverwaltung und Prozessverwaltung

Instanzen sind in `instances.local.json` im Projektordner lokal gespeichert (nicht versioniert; Pfad über
`Instances:File`). Über `/instanz/neu` Geräteinstanzen (PV, Batterie, Ladesäule, Kühlhaus) anlegen;
Kennung und Modbus-Port werden vorgeschlagen, Kennungs- und Portkonflikte werden abgelehnt, speichern
allein startet keinen Prozess. Im Reiter „Einstellungen" einer Instanz: Starten, Beenden und Löschen
(allein für Geräteinstanzen). „Alles starten" startet Broker (docker compose), den Time Master
(pausiert), dann Gateway, Wetterdienst und Geräte in dieser Reihenfolge; belegt ein Port oder läuft
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
`%TEMP%\energytwin-web\<Kennung>.log`. Der Broker hat keine Start-/Stopp-Knöpfe in der Weboberfläche;
er wird über Docker verwaltet und dort als „Verwaltung: Docker" statt mit einem Prozessstatus angezeigt. Beim Starten, Beenden oder Löschen einer Geräteinstanz wird ein
laufendes Gateway mit der aktuellen Geräteliste neu gestartet, damit neue Anlagen ohne manuellen Neustart
Live-Daten liefern.

## Aufbau und Prüfung

`Live/LiveStore.cs`: threadsicherer letzter Empfangsstand; `Live/MqttReceiver.cs`: Hintergrundempfang;
`Live/LiveTrends.cs`: begrenzter Verlaufsspeicher je Messwert-Schlüssel; `Live/LiveView.cs`: Aufbereitung je Modulart;
`Components/Pages/Live.razor`: Anzeige, Gültigkeit und Aktualisierung einmal je Sekunde;
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
Die Master-Detailansicht zeigt zusätzlich Sitzung und Sequenz. Nach vier Sekunden ohne aktuellen Status
oder bei Broker-Verlust ist die Bedienung gesperrt; letzte Werte bleiben als unbestätigt sichtbar.
Ein Reconnect allein bestätigt alte Werte nicht. Ein fehlender Bescheid nach sechs Sekunden bedeutet
„Ergebnis unklar“, niemals automatisch Erfolg oder erneutes Senden.
Die Fortsetzen- bzw. Pause-Schaltfläche ist nur aktiv, wenn der Master im Gegenteilzustand ist (Pause nur beim Laufen, Fortsetzen nur im Pausenmodus); bei nicht verfügbarem Master-Status sind beide gesperrt.

Die Masterseite bietet Tempo (1–100.000, „Übernehmen“) auch im laufenden Betrieb; eine pausierte Simulation
bleibt pausiert. „Neuer Lauf“ übernimmt Datum und Uhrzeit erst nach ausdrücklicher Bestätigung und startet sofort.
Gerätezustände bleiben erhalten; die Bestätigung weist darauf hin. Vorlagen setzen den 15.01./15.04./15.07./15.10.
um 06:00 Uhr im aktuellen Simulationsjahr und befüllen nur die Eingabe. Jeder Auftrag besitzt eine UUID und fünf
Sekunden Gültigkeit; der Master prüft Sitzung und Sequenz atomar gegen die Konsole und liefert für doppelte
Aufträge das gespeicherte Ergebnis zurück, sodass ein doppelt zugestellter Neustart keinen zweiten Lauf auslöst.

Extern gestartete Master benötigen `--mqtt`; ältere laufende Prozesse werden nicht automatisch ersetzt.
Für einen abweichenden Broker den Master mit `--mqtt-host`, `--mqtt-port` und gegebenenfalls `--mqtt-env`
passend zur Web-Konfiguration starten. Der Anlagenstarter verwendet weiterhin die lokalen Broker-Standardwerte.
`Live/MasterControlClient.cs` teilt Zustand und einen ausstehenden Auftrag zwischen Browsern; der vorhandene
`MqttReceiver` transportiert Status, Befehle und Antworten. Master-Topics werden separat verarbeitet und
zählen nicht als Geräte-/Wettertopics des LiveStore. Keine zusätzliche direkte Verbindung zum Master.

Vertrag und Grenzen: [Master-MQTT-Vertrag](../../docs/Master-MQTT-Vertrag.md).
