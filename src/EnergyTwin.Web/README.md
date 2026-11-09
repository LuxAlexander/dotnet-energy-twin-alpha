# EnergyTwin.Web – lokale Bedienoberfläche

**Stand:** 2026-11-09. Blazor Interactive Server mit lesender MQTT-Live-Anbindung.

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

MQTTnet empfängt `energytwin/#` mit QoS 1; die fünf Wettertopics und Wetterstatus zusätzlich exakt,
damit RabbitMQ gespeicherte Nachrichten liefert. Geräte-/Gateway-Status kann beim Einstieg bis zur
nächsten Wiederholung (standardmäßig 30 Sekunden) fehlen. MQTT-Empfang und letzte Nachrichten liegen
browserunabhängig im Singleton; Neuladen verliert sie nicht. Ein Webdienst-Neustart leert sie.
Maximal 512 Topics und 256 KiB Text pro Nachricht werden angenommen, keine dauerhafte Speicherung.
Ungültige JSON-/Schema-Nachrichten werden gezählt und verworfen, leere Nachrichten löschen das Topic.

Verbindungsfehler führen zu Neuversuchen mit 1–30 Sekunden Abstand. Nach Verbindungsverlust sind alle
letzten Werte unbestätigt; auch nach Reconnect bleiben sie das bis zum erneuten Empfang.
Gerätemesswerte werden nach 15 realen Sekunden, Status nach 45 Sekunden als veraltet gekennzeichnet.
Bei abweichenden Publisher-Intervallen können diese konservativen Grenzen zu Warnungen führen.
Qualität und Offline-/Dekodierstatus bleiben sichtbar. Wetterwerte werden mit Source-/Master-Sitzung
und Lauf des Dienststatus abgeglichen. Gespeicherte Pausenstände sind keine Prozess-Lebendbestätigung.
Keine Befehle werden gesendet oder für später gepuffert.

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
(allein für Geräteinstanzen). „Anlage starten" startet Broker (docker compose), Gateway, Wetterdienst,
Geräte und Master in dieser Reihenfolge; belegte Ports verhindern Doppelstarts, Fehler erscheinen pro
Modul; der Master startet pausiert. „Alles geordnet schließen" stoppt nach Bestätigung Master → Geräte
→ Wetter → Gateway; der Broker bleibt unberührt.

Grenzen: Nach einem Webdienst-Neustart laufen Prozesse weiter und werden an Port und Kommandozeile
wiedererkannt; sie können dann über „Einstellungen → Beenden" bzw. „Alles geordnet schließen"
geordnet beendet werden. Programme fremder Anbieter auf belegten Ports werden geschützt und nicht
berührt. „Anlage starten" startet keine Module doppelt: belegt ein Port oder läuft unser Gateway/Master
bereits extern, wird das Modul übersprungen (Meldung pro Modul). Sichere Wiedererkennung nach
Neustart bleibt auf Port+Kommandozeile beschränkt. Master-Fortsetzen und fachliche Befehle folgen mit
den jeweiligen Anbindungen; „Fortsetzen" bleibt daher deaktiviert. Prozessprotokolle liegen unter
`%TEMP%\energytwin-web\<Kennung>.log`.

## Aufbau und Prüfung

`Live/LiveStore.cs`: threadsicherer letzter Empfangsstand; `Live/MqttReceiver.cs`: Hintergrundempfang;
`Live/LiveTrends.cs`: begrenzter Verlaufsspeicher je Messwert-Schlüssel; `Live/LiveView.cs`: Aufbereitung je Modulart;
`Components/Pages/Live.razor`: Anzeige, Gültigkeit und Aktualisierung einmal je Sekunde;
`Components/Shared/LiveChart.razor`: Verlaufsanzeige mit virtueller Zeit und Wertetabelle;
`Management/InstanceStore.cs`: persistierte Instanzkonfiguration mit Konfliktprüfung;
`Management/ProcessManager.cs`: Start/Stop verwalteter Prozesse, Broker-Start über docker compose,
Wiedererkennung extern gestarteter Prozesse über Port und Kommandozeile sowie Doppelstart-Schutz;
`Components/Pages/AddInstance.razor`: Anlegen von Geräteinstanzen;
`Components/Pages/Module.razor`: Live-Beispiel-Wechsel je Kennung und Instanzaktionen im Einstellungen-Reiter.
`Presentation`, `PreviewCatalog` und `PreviewChart`: Beispielansichten.

Tatsächlich ausgeführte Prüfungen stehen im [Journal](../../docs/development-journal/alexander-lux.md).
Ein Build ersetzt weder Broker-Ausfalltests noch einen Windows-/Linux-Gesamtprobelauf.
