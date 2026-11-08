# Skill `readTask`

| Feld | Inhalt |
|---|---|
| **Verantwortliche Person** | Susanne Lastowicka |
| **Zweck** | Aufgabenblatt lesen, alle Anforderungen extrahieren, mit dem Projektstand abgleichen und eine Reihenfolge der offenen Arbeiten vorschlagen. Entspricht dem Anwendungsfall „Aufgabenblatt mit Umsetzung abgleichen“ aus Aufgabe 03 (§6), erweitert um die Reihenfolge. |
| **Aufruf** | `/readTask Aufgabe 03` · `/readTask "dotNET Programmierung 2026 - Aufgabe 03.pdf"` · ohne Argument: neueste Aufgaben-PDF im Repo-Root |
| **Benötigter Kontext** | Aufgabendatei (PDF/MD), `AGENTS.md`, zentrale `README.md`, `EnergyTwin.sln`, `src/`, `tests/`, `docker/`, `docs/`, Journale, `.env.example`, `git log`/`git status` |
| **Arbeitsschritte** | 1. Anforderungen extrahieren und klassifizieren (Pflicht/optional/Abgabe/Bewertung/Nachweis) · 2. je Anforderung Belege suchen, Status vergeben (erfüllt / teilweise erfüllt / nicht erfüllt / nicht nachgewiesen) · 3. offene Punkte nach Abhängigkeit, Pflicht, Risiko, Zuständigkeit ordnen · 4. Bericht in festem Format |
| **Grenzen** | Nur lesend. Kein Commit/Push/Tag, kein Build/Test/Docker, kein Netzwerk, keine Änderung am Journal, keine Secrets (`.env` nie lesen). Bericht nur auf Wunsch als Datei. Tests nur auf ausdrücklichen Wunsch (Projektregel in `AGENTS.md` §0.1). |

## Nutzen und Wirkung
Das Aufgabenblatt ist lang und verschachtelt (Pflichtumfang, Mindestanforderungen, Abgabeliste, Bewertung). Der Skill
macht daraus eine prüfbare Checkliste mit Belegen und verhindert zwei typische Fehler: Anforderungen übersehen und
vorhandenen Code mit erfolgreichem Nachweis verwechseln. Die vorgeschlagene Reihenfolge spart Umplanung, weil
Abhängigkeiten (Vertrag → Broker → Gateway → Viewer) vorher sichtbar sind.

## Beispielaufruf mit Ergebnis (2026-11-08)
Aufruf: `/readTask Aufgabe 03`. Vollständiges Ergebnis: [`docs/task-status/aufgabe-03.md`](../../docs/task-status/aufgabe-03.md)
(28 Anforderungen: 3 erfüllt, 6 teilweise, 16 nicht erfüllt, 3 nicht nachgewiesen). **Historisches Beispiel vor
Broker-/Gateway-/Wetter-Umsetzung und SRS-Entwurf**, kein aktueller Projektstatus. Ein neuer Abgleich muss die
späteren Quellen und datierten Nachweise berücksichtigen. Unveränderter Auszug:

| ID | Anforderung (Seite) | Art | Status | Beleg / Lücke |
|---|---|---|---|---|
| A7 | Compose im Repo, dokumentiert (S. 2) | Pflicht | teilweise | `docker/docker-compose.yml` vorhanden; Image ohne genaue Version, `rabbitmq.conf` ist ein leerer Ordner, keine Start/Stopp-Doku |
| A9 | Keine Zugangsdaten versioniert, `.env.example` (S. 2, 5) | Pflicht | nicht erfüllt | Passwort fest in der Compose-Datei, `.env.example` fehlt |
| A10 | Modbus–RabbitMQ-Gateway (S. 2–3) | Pflicht | nicht erfüllt | kein Gateway-Projekt in `EnergyTwin.sln` |
| A5 | SRS `docs/SRS-LB03.md` (S. 2) | Pflicht | nicht erfüllt | Datei fehlt |
| A16 | Eigener Skill pro Person (S. 4–5) | Pflicht | teilweise | `skills/readTask` vorhanden; Alexander und Dominik offen |

Vorgeschlagene Reihenfolge (Auszug): 1. SRS mit Nachrichtenformat und Akzeptanzkriterien (blockiert alles) →
2. Compose und Broker-Doku abschließen → 3. Gateway → 4. Viewer → 5. Nachweise (Screenshots, Protokolle) →
6. README, `AGENTS.md`, Journale, Abgabe-Tag `lb03-2026`.

## Aktivierung
Claude Code lädt Skills aus `.claude/skills/`. Dieser Ordner `skills/` ist die versionierte Ablage im Repo. Zum Verwenden:
`skills/readTask` nach `.claude/skills/readTask` kopieren (oder verlinken), sofern dort noch keine eigene Fassung liegt:
```
New-Item -ItemType Directory -Force .claude/skills
Copy-Item -Recurse skills/readTask .claude/skills/readTask
```

`.claude/` ist lokal und ignoriert. Nach Änderungen an `skills/readTask` die aktive Kopie gezielt abgleichen.

## Quelle und Lizenz
Eigenentwicklung für dieses Projekt, nicht aus einer fremden Vorlage übernommen. Inspiration: die Beschreibung des
Anwendungsfalls in Aufgabe 03 (Roland Graf, 2026). Keine fremde Lizenz zu beachten.

## Bekannte Grenzen
- Statusurteile beruhen auf lesbaren Belegen; ohne ausgeführten Lauf bleibt „nicht nachgewiesen“.
- PDFs werden als Seitenbilder gelesen; bei Scans mit schlechter Qualität Anforderungen im Zweifel nachfragen.
