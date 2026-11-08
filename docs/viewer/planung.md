# Viewer – bestätigte Planung

**Stand:** 2026-11-08 · **Verantwortlich:** Alexander Lux
**Status:** Am 2026-11-08 ausdrücklich zur Umsetzung freigegeben und umgesetzt.
Aktuelle Einrichtung und Grenzen: [README](README.md); [textueller Nachweis](../nachweise/2026-11-08-viewer/README.md).

## Bestätigter Umfang

- MQTT Explorer als Viewer: Topic-Baum, lesbare JSON-Nachrichten und Werteverläufe.
- Alle vier Feldgeräte, Gateway-Status und Wetterdaten aller fünf Standorte einschließlich Dienststatus.
- Empfang ausschließlich über RabbitMQ/MQTT; bestehende Geräte- und Wetterverträge bleiben maßgeblich.
- Alexander übernimmt Einrichtung und Dokumentation des Viewers.
- Portable Windows-Ausgabe, ohne reguläre Installation.
- Textuelle README mit Erklärung und Schritt-für-Schritt-Anleitung; Screenshots sind nicht erforderlich.
- Werteverläufe zunächst als Live-Diagnose. Virtuelle Zeit und Qualität werden im JSON abgelesen;
  eine Simulationszeit-Achse und dauerhafte Verlaufsspeicherung sind nicht zugesagt.

## Optionale spätere Erweiterung

Eine fertige Anlagenübersicht mit Statusampeln kann später ergänzt werden. Sie gehört nicht zur ersten
Viewer-Einrichtung. Dauerhafte Zeitreihenspeicherung und eine eigene Simulationszeit-Achse sind ebenfalls
mögliche spätere Erweiterungen. Konkretes Werkzeug und Umsetzung sind dafür noch nicht entschieden.

## Bestätigter und ausgeführter Plan

1. Offizielle portable Windows-Ausgabe von MQTT Explorer v0.3.5 verwenden (GitHub als letzte stabile Version
   gekennzeichnet; Veröffentlichung von 2019). Werkzeug lokal unter `tools/local/mqtt-explorer/` ablegen und
   diesen Ordner ignorieren; keine Programmdatei oder Zugangsdaten versionieren.
2. Lokales Profil für `127.0.0.1:1883` einrichten. Zugangsdaten aus der vorhandenen lokalen `docker/.env`,
   keine Passwörter in README, Protokollen oder exportierten Profilen.
3. `energytwin/#` für Live-Daten sowie konkrete Wetter-, Geräte- und Gateway-Status-Topics für gespeicherte
   Zustände abonnieren. Genaues Verhalten der ausgewählten Version am vorhandenen Broker prüfen.
4. MQTT Explorer sowie die benötigten eigenen Simulatoren, Master und Gateway für einen begrenzten
   Viewer-Probelauf starten; vorhandene Programme/Ports vorher prüfen, keine fremden Prozesse beenden.
   Falls erforderlich den Projektbroker über die bestehende Compose-Konfiguration starten.
5. Empfang der vier Geräte, aller fünf Wetterorte und Statusmeldungen sowie die tatsächliche Auswahl
   numerischer JSON-Felder für Diagramme prüfen. Grenzen sichtbar dokumentieren; bei fehlender Unterstützung
   keine verdeckte Änderung des Nachrichtenvertrags oder eigene Viewer-Entwicklung.
6. `docs/viewer/README.md` mit Erklärung, Einrichtung, Abos, Werteverläufen, Statusdeutung und textuellem
   Prüfergebnis erstellen. Zentrale README und vorhandene Zuständigkeitsangaben entsprechend der bestätigten
   Übernahme durch Alexander nachziehen. Keine Screenshots und kein vollständiger neuer Unit-Testlauf.

**Offizielle Veröffentlichung:** [v0.3.5](https://github.com/thomasnordquist/MQTT-Explorer/releases/tag/v0.3.5).
Die konkrete JSON-Struktur wurde im Probelauf für PV-Wirkleistung und Salzburger Temperatur erfolgreich geplottet.

## Vorhandene Grundlagen

- [Broker-Einrichtung](../../docker/README.md)
- [Geräte-Nachrichtenvertrag](../Nachrichtenvertrag-LB03.md)
- [Wetter-Nachrichtenvertrag](../Wetter-Nachrichtenvertrag-LB03.md), insbesondere konkrete Retain-Abos

Diese Datei hält die Entscheidungen fest; die tatsächlichen Beobachtungen und Grenzen stehen im verlinkten Nachweis.
Eine dauerhafte Zeitreihenspeicherung ist weiterhin nicht beschlossen.
