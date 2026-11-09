# Dokumentenlandkarte – was steht wo, wann wird es nachgezogen

Nur lesen, wenn `doku-sync` den Wirkungskreis einer Änderung bestimmt. Quelle der Wahrheit bleiben Code,
Konfiguration und datierte Nachweise; diese Karte sagt **wo** eine belegte Aussage hingehört.

## Dokumente

| Dokument | Inhalt | Nachziehen, wenn … | Nicht hinein |
|---|---|---|---|
| `README.md` (Root) | Überblick, Zuständigkeitentabelle (inkl. „Aufgabe 03 – Modbus zu RabbitMQ“), LB03-Start, Aufbau, Startreihenfolge, Demo-Ablauf, Schnittstellen, Konfiguration, Tests, Journale, bekannte Einschränkungen | neue Module/Optionen, geänderte Startbefehle oder Ports, neue Nachweise, neue Testzahlen, geänderte Zuständigkeit | Formeln, Registerpläne, lange Bedienungsdetails (gehören ins Modul-README) |
| `AGENTS.md` §0 Stand-Kopf | datierter Stand, letzter dokumentierter Testlauf, offene Punkte | jede abgeschlossene größere Arbeit; Datum und Beleg | ungeprüfte Behauptungen, erfundene Testzahlen |
| `AGENTS.md` §0.1 | Arbeitsregeln (Journal, Commits, Tests, Sprache) | nur bei Team-Entscheidung | projektbezogene Details |
| `AGENTS.md` §0.2 Git-Stand | Branch, letzter Commit, Sonderregeln | wenn sich der Git-Stand ändert | Commit-Hashes als Wahrheit für Inhalte |
| `AGENTS.md` §0.3 Was fertig ist | Tabelle Baustein → Stand → wichtigste Dateien → Testzahlen | neuer/fertiger Baustein, geänderte Testzahl | geplante, aber nicht gebaute Funktionen |
| `AGENTS.md` §0.4 Noch offen | offene Abgabepunkte, Teamfragen | erledigt oder neu entdeckt | allgemeine Ideen |
| `AGENTS.md` §0.5 / §0.5a | Kurzreferenz Zeit-API / Modbus | API-, Protokoll- oder Registeränderung | vollständige Herleitungen (Modul-README) |
| `AGENTS.md` §0.6 / §0.6a | Stolperfallen, Sitzungsarchive | neue reproduzierbare Falle, neues Archiv | einmalige Tippfehler |
| `AGENTS.md` §3 | Architekturentscheidungen, Bibliotheksabhängigkeiten, offene Fragen | neue Abhängigkeit, neues Projekt, Protokoll-/Transportentscheidung | Details, die ein Modul-README schon führt |
| `AGENTS.md` §6 | Build-Status, Befehle, Testprojekte, Testfallzahlen | Build-/Testlauf mit Beleg, neues Testprojekt, neuer Befehl | vorweggenommene Läufe |
| `AGENTS.md` §7 / §8 | bekannte Probleme, Konventionen | Problem behoben/neu, neue Konvention | Spekulationen |
| `src/<Projekt>/README.md` (16 Stück) | Zweck, Verantwortlich, Basis, Konfiguration, Bedienung, Schnittstellen, Formeln, Registerplan, Tests, bekannte Probleme | Verhalten, Optionen, Defaults, Formeln, Register, Schnittstellen des Projekts | zentrale Startreihenfolge, Zuständigkeiten des Teams |
| `docs/SRS-LB03.md` | Anforderungen und Akzeptanzkriterien | neue/geänderte Anforderung | Umsetzungsdetails |
| `docs/Nachrichtenvertrag-LB03.md` | MQTT-Topics, QoS, Payload, Qualitätsstatus, Beispiele | Gateway-/Messwertformat ändert sich | Wetterformat (eigener Vertrag) |
| `docs/Wetter-Nachrichtenvertrag-LB03.md` | Wetter-Topics, Retain, Standorte | Wetterveröffentlichung ändert sich | Geräteversorgung per HTTP (WeatherSimulator-README) |
| `docker/README.md`, `docker/.env.example` | Start/Status/Stopp, Ports, Zugangsdatenvorlage | Compose-, Broker- oder Portänderung | echte Zugangsdaten |
| `docs/viewer/README.md` | Einrichtung und Deutung des MQTT-Viewers | Viewer-Abos, Werkzeugversion, Bedienung | Geräte-/Gateway-Implementierung |
| `docs/nachweise/JJJJ-MM-TT-<thema>/README.md` | Ziel, Aufbau, Ergebnistabelle, Beobachtungen, „nicht geprüft“ | nach einem echten Lauf (mit `.txt`-Protokollen) | Wunschergebnisse, `.log`-Dateien |
| `docs/development-journal/<person>.md` | Tageseintrag je Person (`Done`, `KI`, `Artefact`, `Test`, `Comment`) | **jede** Änderung (§0.1), nur die eigene Datei | fremde Journale |
| `docs/task-status/aufgabe-03.md` | Anforderungsabgleich mit Beleg/Lücke | nach `readTask`-Lauf | laufend gepflegte Statusliste ohne Datum |
| `skills/<id>/README.md` + Root-Tabelle | Zweck, Aufruf, Grenzen des Skills | neuer Skill oder geänderte Aufrufform | Code-Doku des Projekts |
| `Directory.Packages.props` | zentrale NuGet-Versionen | Paketänderung | Begründungen (→ `AGENTS.md` §3) |
| `docs/devices/<Gerät>/…` | Geräte-Anleitungen (z. B. Kühlhaus-Umsetzung) | gerätespezifische Umbauten | allgemeine Regeln |

## Änderungsart → Pflichtdokumente

| Änderung | Mindestens nachziehen |
|---|---|
| Geräteverhalten, Formel, Taste | Geräte-README → Root-README (Aufbau) → `AGENTS.md` §0.3 → Journal |
| Modbus-Register/Adresse/Typ | `src/Modbus.Contracts/README.md` (Kopftabelle) → Geräte-README → `AGENTS.md` §0.5a/§3 → ggf. Nachrichtenvertrag → Journal; `--register-plan` gegenlesen |
| MQTT-Topic/Payload/QoS | `docs/Nachrichtenvertrag-LB03.md` → `src/ModbusRabbitGateway/README.md` → SRS → Journal |
| Neue CLI-Option oder Default | Modul-README → Root-README (Startbefehle) → ggf. `start-all.ps1`-Hilfe → Journal |
| Neues Projekt/neue Abhängigkeit | `AGENTS.md` §3 (Projektliste) → `Directory.Packages.props`/`.csproj`-Doku → Root-README (Aufbau) → Journal |
| Neue/geänderte Tests | `AGENTS.md` §0.3 **und** §6 → Root-README (Tests) → Journal; Zahlen nur aus belegtem Lauf |
| Neuer Funktionsnachweis | `docs/nachweise/<datum>-<thema>/README.md` → `AGENTS.md` Stand-Kopf/§6 → Root-README (Nachweise) → Journal |
| Docker/Broker/Ports | `docker/README.md` → `.env.example` → Root-README (Konfiguration) → `AGENTS.md` §3 → Journal |
| Zeitprotokoll (`SimCore.*`) | `src/SimCore.TimeSync/README.md` → `AGENTS.md` §0.5/§3 → Journal |
| Umbenennung/Verschiebung | alle relativen Links (Prüfer) → `AGENTS.md` §3 → Root-README → betroffene Modul-READMEs → Journal |
| Neuer Skill | `skills/<id>/SKILL.md` + `README.md` → Root-README (Skill-Tabelle) → Journal |
| Neues Wettermodell/-profil | `src/WeatherSimulator/README.md` → Wettervertrag → Nachweis → Journal |
| Reine Formulierung | Journal; sonst nichts |

## Nie von Hand ändern

- `graphify-out/` – generiert; Status in `graphify-out/UPDATE_STATUS.json`, nach Änderungen neu indizieren.
- `docs/data/*.csv` – Programme schreiben sie; `*.csv` ist ignoriert. Beispiele bleiben versioniert.
- `.opencode/`, `.claude/`, `.agents/` – lokale, ignorierte Skill-Kopien; versioniert wird `skills/`.
- `bin/`, `obj/`, `.vs/`, `.idea/` – Build- und IDE-Ausgaben.

## Wenn die Landkarte nicht passt

Ein fehlendes Dokument ist selbst ein Befund: entweder fehlt die Datei (dann als offen melden) oder die
Landkarte ist veraltet (dann hier korrigieren und im Journal nennen).
