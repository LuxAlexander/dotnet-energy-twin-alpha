# Aufgabenabgleich: Aufgabe 03 – Von Modbus zu RabbitMQ (Stand 2026-11-08, Commit `f30bcd1`)

> **Historischer Abgleich:** Die folgende Tabelle beschreibt den damaligen Commit, nicht den aktuellen Arbeitsstand.
> Gateway, Nachrichtenvertrag und SRS sind inzwischen vorhanden. Alexander hat am 2026-11-08 die Broker-Konfiguration
> auf feste Version, lokale `.env`, Healthcheck und dokumentierten Start umgestellt und Nachrichten aller vier Geräte
> empfangen. Aktuell: [`SRS-LB03.md`](../SRS-LB03.md), [`Docker-Anleitung`](../../docker/README.md) und
> [Funktionsnachweis](../nachweise/2026-11-08-docker-alexander/README.md). Ein vollständiger neuer Anforderungsabgleich steht aus.

Erstellt mit dem Skill [`readTask`](../../skills/readTask/README.md), Aufruf `/readTask Aufgabe 03` (Susanne Lastowicka).
Nur lesend ermittelt: kein Build, kein Test und kein Docker-Start (Grenzen des Skills).

## Zusammenfassung
**28 Anforderungen:** 3 erfüllt · 6 teilweise erfüllt · 16 nicht erfüllt (davon 1 optional) · 3 nicht nachgewiesen.

Die LB02-Grundlage steht. Vom neuen Datenweg ist nur die Compose-Datei angefangen, und darin gibt es zwei Fehler.
Gateway, Viewer, SRS und Nachrichtenvertrag fehlen noch ganz.

## Anforderungen

| ID | Anforderung (Seite) | Art | Status | Beleg / Lücke |
|---|---|---|---|---|
| A1 | Datenweg Gerät → Modbus → Gateway → RabbitMQ → Viewer (S. 1) | Pflicht | nicht erfüllt | In `src/` und `EnergyTwin.sln` kommen RabbitMQ, MQTT und AMQP nicht vor |
| A2 | Zwei Simulatoren pro Person im zentralen README (S. 1) | Pflicht | erfüllt | `README.md:11-15`: Susanne → Master + Charger, Alexander → Wetter + PV, Dominik → Batterie + Kühlhaus |
| A3 | Neue Module und Integration zugeordnet (S. 2) | Pflicht | nicht erfüllt | Für Gateway, Viewer und Broker gibt es noch keine Zuständigkeit |
| A4 | Alle Pflicht-Geräte (PV, Batterie, Charger, Kühlhaus) an RabbitMQ (S. 1, 5) | Pflicht | nicht erfüllt | – |
| A5 | SRS `docs/SRS-LB03.md`: Ziel, Datenweg, Anforderungen an Gateway und Viewer, Nachrichtenformat, Fehlerverhalten, Akzeptanzkriterien (S. 2) | Pflicht | nicht erfüllt | Datei fehlt |
| A6 | Docker-Umgebung, RabbitMQ läuft lokal (S. 2) | Pflicht | nicht nachgewiesen | Es gibt keinen Lauf-Nachweis und keinen Journaleintrag |
| A7 | Compose im Repo, dokumentiert mit Image-Version, Ports/Plugins, Zugangsdaten, Start/Status/Stopp (S. 2) | Pflicht | teilweise | `docker/docker-compose.yml` ist da. Image `rabbitmq:4-management` ohne genaue Version, keine Doku zu Start, Status und Stopp. `docker/rabbitmq.conf` ist ein leerer Ordner, eingebunden ist er als Datei |
| A8 | AMQP oder MQTT gewählt und begründet, Konfiguration passend (S. 2) | Pflicht | teilweise | MQTT-Plugin aktiv, Ports 5672 und 1883 offen. Es gibt weder eine Entscheidung noch eine Begründung |
| A9 | Keine Zugangsdaten versioniert, `.env.example` mit Platzhaltern (S. 2, 5) | Pflicht | nicht erfüllt | Benutzer und Passwort stehen fest in der versionierten Compose-Datei. Eine `.env.example` fehlt |
| A10 | Gateway: konfigurierbare Geräte/Hosts/Ports, Kopf und Geräteregister dekodieren, Intervalle konfigurierbar, gelesene Simulationszeit verwenden, Fehler sichtbar, kontrolliertes Beenden (S. 2–3) | Pflicht | nicht erfüllt | Wiederverwendbar sind `ModbusDiagnosticClient` (`DeviceConnection`, `RegisterFormatter`), `RegisterPlans.For`, `RegisterCodec` und im Kopf Modellzeit, Sequenz und Zeitstatus |
| A11 | Daten nur über Modbus, Diagnose-Client bleibt nutzbar (S. 3) | Pflicht | erfüllt | Stand LB02 unverändert, 28 Tests (`AGENTS.md` §6). Nach dem Gateway-Umbau erneut prüfen |
| A12 | Nachrichtenvertrag mit DeviceId, virtueller Zeit, Sequenz, Qualität, Name/Wert/Einheit; Topics bzw. Exchanges/Queues/Bindings; Beispielnachricht (S. 3) | Pflicht | nicht erfüllt | – |
| A13 | Virtuelle Zeit und Abfragezeit getrennt, Einschränkungen (keine atomare Lesung) dokumentiert (S. 3) | Pflicht | nicht erfüllt | – |
| A14 | Viewer zeigt alle Geräte mit Zeit, Werten, Einheiten, Sequenz und Qualität (S. 3) | Pflicht | nicht erfüllt | Bei MQTT reicht auch MQTT Explorer |
| A15 | `AGENTS.md` aktuell und kurz, mit Struktur, Zuständigkeiten, Befehlen und Regeln für Konfiguration, Geheimnisse und generierte Dateien (S. 4) | Pflicht | teilweise | Vorhanden, aber Stand LB02 und sehr lang. Eine Regel für generierte Dateien fehlt (`graphify-out/` mit 2008 versionierten Dateien) |
| A16 | Ein eigener Skill pro Person (3) mit Doku und konkretem Aufruf samt Ergebnis (S. 4–5) | Pflicht | teilweise | `skills/readTask` (Susanne), dieser Bericht ist der erste Aufruf. Die Skills von Alexander und Dominik fehlen |
| M1 | .NET 10, gemeinsam über die Solution baubar (S. 5) | Pflicht | nicht nachgewiesen | `net10.0` ist gesetzt. Der letzte belegte Build ist vom 2026-10-27, danach kamen 3 Commits |
| M2 | Zeitsteuerung, Wetter und Bedienung aus LB02 bleiben erhalten (S. 5) | Pflicht | erfüllt | 206 Tests laut `AGENTS.md`. Nach dem Umbau erneut laufen lassen |
| M4 | Fehler sichtbar, ungültige oder veraltete Werte nicht als gültig (S. 5) | Pflicht | nicht erfüllt | Hängt am Gateway |
| M5 | Broker-Ausfall ohne Endlosschleife oder Absturz, Wiederanlauf dokumentiert (S. 5) | Pflicht | nicht erfüllt | – |
| M6 | Zentrales README und Projekt-READMEs aktuell, mit Konfiguration, Startreihenfolge, Einschränkungen (S. 5) | Pflicht | teilweise | Stand LB02 (Titel „LB02“), Broker, Gateway und Viewer fehlen |
| N1 | Nachweise 1–7: Broker-Start, Programme, Nachrichten, Sollwert-Kette, Pause, Verbindungsabbruch, Beenden (S. 5–6) | Nachweis | nicht erfüllt | – |
| N8 | Bisherige Tests weiter ausgeführt und dokumentiert (S. 6) | Nachweis | nicht nachgewiesen | Letzter Lauf 2026-10-26/27 |
| N9 | Skill-Einsatz und Abgleich mit den SRS-Akzeptanzkriterien dokumentiert (S. 6) | Nachweis | nicht erfüllt | Skill-Einsatz mit diesem Bericht belegt; SRS-Abgleich fehlt |
| O1 | Wetter oder TimeSim über RabbitMQ (S. 6) | optional | nicht erfüllt | – |
| G1 | Annotierter Tag `lb03-2026` mit vollständigem Hash (S. 6) | Abgabe | nicht erfüllt | – |
| G2 | Entwicklungsjournale fortgeführt, Planung und Umsetzung getrennt (S. 7) | Abgabe | teilweise | Nur Susanne hat einen LB03-Eintrag |
| G3 | Reproduzierbare Anleitung für externe Werkzeuge (Docker Desktop, ggf. MQTT Explorer) (S. 6) | Abgabe | nicht erfüllt | – |

## Offene Fragen ans Team
1. **AMQP oder MQTT?** Empfehlung: MQTT. Das Plugin ist schon aktiv, und MQTT Explorer ersetzt einen eigenen Viewer.
   Vorher mit der Demo aus dem Unterricht abgleichen.
2. **Gateway als eigenes Projekt oder als Erweiterung des Diagnose-Clients?** Empfehlung: eigenes Projekt. Die
   Modbus-Verbindung käme dann aus einer gemeinsamen Bibliothek, und der Diagnose-Client bleibt unverändert nutzbar.
3. **Wer übernimmt Gateway, Viewer und Broker?**
4. **Was wird während der Pause veröffentlicht?** Weiter senden mit einem Qualitätsstatus wie „pausiert“, oder nichts
   senden? Und was gilt bei `MasterLost` und bei unveränderter Sequenznummer (Wert veraltet)?
5. **Welche Themen haben die Skills von Alexander und Dominik?** Sie sollen sich nicht mit `readTask` überschneiden.

## Vorgeschlagene Reihenfolge
1. **SRS und Nachrichtenvertrag** (M): Fragen 1–4 entscheiden, dann `docs/SRS-LB03.md` mit Topics, Beispielnachricht,
   Qualitätsstatus, Zeitzuordnung und Akzeptanzkriterien schreiben. Das blockiert alles Weitere.
   *Fertig, wenn* das Team die SRS geprüft hat.
2. **Broker sauber aufsetzen** (S), parallel zu 1 möglich: Image-Version fest angeben, `rabbitmq.conf` als echte Datei
   anlegen, Zugangsdaten über `.env` laden, `.env.example` dazulegen, `docker/README.md` mit Start, Status und Stopp.
   *Fertig, wenn* `docker compose up -d` aus einem frischen Checkout läuft und kein Passwort mehr im Repo steht.
3. **Gateway-Grundgerüst** (M, braucht 1): neues Projekt in der Solution, Geräteliste und Intervalle konfigurierbar,
   Modbus lesen, Kopf und Geräteregister in das Nachrichtenmodell dekodieren, zuerst nur auf der Konsole ausgeben.
   Dazu Unit-Tests für Dekodierung und Serialisierung.
   *Fertig, wenn* alle 4 Geräte korrekt gelesen und als Nachricht ausgegeben werden.
4. **Veröffentlichen und Robustheit** (M/L, braucht 2 und 3): Publisher, Qualitätsstatus und Erkennung veralteter Werte,
   Wiederanlauf bei Abbruch von Modbus und Broker, geordnetes Beenden, async und `CancellationToken` durchgängig.
   *Fertig, wenn* ein gestoppter Broker oder ein gestopptes Gerät sichtbar gemeldet wird und der Betrieb danach von
   selbst weiterläuft.
5. **Viewer** (S/M, braucht 4): MQTT Explorer mit Anleitung oder eine eigene Konsolen-App.
   *Fertig, wenn* alle 4 Geräte mit Zeit, Einheit, Sequenz und Qualität sichtbar sind.
6. **Skills von Alexander und Dominik** (je S): jederzeit parallel möglich, jeweils mit einem dokumentierten Aufruf.
7. **Doku nachziehen** (S/M): zentrales README (LB03, Zuständigkeiten neuer Module, Startreihenfolge), Projekt-READMEs,
   `AGENTS.md` kürzen und um Regeln für Secrets und generierte Dateien ergänzen, `.gitignore`, optional `start-all.ps1`
   um Broker und Gateway erweitern.
8. **Nachweise** (M): Ablauf 1–7 mit Screenshots oder Protokollen, Testprojekte einzeln laufen lassen,
   SRS-Akzeptanzkriterien abgleichen, `readTask` erneut aufrufen.
9. **Abgabe** (S): Journale abschließen, annotierten Tag `lb03-2026` setzen, Remote prüfen.

## Hinweise
- **`docker/rabbitmq.conf` ist ein leerer Ordner, keine Datei.** Vermutlich hat Docker ihn beim ersten `compose up`
  angelegt, weil die Datei fehlte. Die Konfiguration fehlt also, und der Start kann scheitern oder sie ignorieren.
  Git versioniert leere Ordner nicht.
- **Passwort im Repo:** Es steht in `docker/docker-compose.yml`, und `graphify-out/` spiegelt die Compose-Datei.
  Nach der Umstellung auch dort nachziehen.
- **Die PDF zu Aufgabe 03 ist committet.** `AGENTS.md` §0.1 sagt, Aufgaben-PDFs seien nur lokal, aber `.gitignore`
  nennt nur Aufgabe 02.
- **`graphify-out/` (2008 Dateien) ist generiert und versioniert.** Das Team sollte entscheiden, ob das so bleibt;
  Aufgabe 03 verlangt Regeln für generierte Dateien.
- **Veraltete Angaben in `AGENTS.md`:** §2 sagt noch „geplant: Wind“, der Stand-Kopf ist 2026-10-27.
- **`.claude/skills/readTask` ist nicht versioniert, `skills/readTask` ist die versionierte Ablage.** Die beiden Kopien
  können auseinanderlaufen.
