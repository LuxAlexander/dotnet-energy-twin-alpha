# Shared.Config

**Verantwortlich:** Alexander Lux

Gemeinsame Pfad- und MQTT-Zugangsdatenauflösung für die Programme.

## Pfade

`OutputPaths.RepositoryRoot` sucht von der Programmdatei aufwärts nach `EnergyTwin.sln`.
`OutputPaths.CsvDirectory` liefert `<Repo>/docs/data`, außerhalb des Repositories `./docs/data`.
Die Programme können eigene Optionen zur Überschreibung des Ausgabeordners anbieten.

## MQTT-Zugangsdaten

`MqttCredentials.Resolve(userOption, envFile, environment)` wird von Gateway und Wetter verwendet.
Reihenfolge: Benutzeroption (falls angeboten) → `ENERGYTWIN_MQTT_USER`/`ENERGYTWIN_MQTT_PASSWORD` →
Datei mit `RABBITMQ_DEFAULT_USER`/`RABBITMQ_DEFAULT_PASS`. Der Aufrufer legt den Dateipfad fest,
standardmäßig `docker/.env`. `.env` bleibt ignoriert, `.env.example` ist die Vorlage.

Einfaches `SCHLÜSSEL=Wert`-Format, Kommentarzeilen mit `#`, äußere Anführungszeichen werden entfernt.
Keine vollständige Shell-/Compose-Expansion; für gleiche Auswertung einfache Werte ohne `$`, Leerzeichen oder
Inline-Kommentare verwenden. `ToString` enthält höchstens Benutzer und Datenquelle, niemals das Passwort.
Die öffentliche Gateway-Klasse bleibt als delegierende Fassade kompatibel.

Das Projekt ist eine Bibliothek ohne eigenes Startprogramm. Gemeinsamer Build: `dotnet build EnergyTwin.sln`.
Die bestehende Gateway-Testsuite prüft die Zugangsdatenauflösung; neuer Unit-Testlauf erfolgt nur gemäß Projektregel.
