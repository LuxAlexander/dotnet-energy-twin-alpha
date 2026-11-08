---
name: readTask
description: Liest ein Aufgabenblatt (PDF/Markdown/Text) im Repo, extrahiert alle Anforderungen (Pflicht, optional, Abgabe, Bewertung, Nachweise), gleicht sie mit dem tatsächlichen Projektstand ab (erfüllt / teilweise erfüllt / nicht erfüllt / nicht nachgewiesen) und schlägt eine begründete Reihenfolge der offenen Arbeiten vor. Nur lesend. Aufruf z. B. "/readTask Aufgabe 03".
---

# readTask – Aufgabenblatt lesen, Stand abgleichen, Reihenfolge vorschlagen

**Verantwortlich:** Susanne Lastowicka · Details, Beispiel und Quelle: `README.md` in diesem Ordner.

## Aufruf
`/readTask <Datei oder Stichwort>` – z. B. `/readTask Aufgabe 03` oder `/readTask "dotNET Programmierung 2026 - Aufgabe 03.pdf"`.
Ohne Argument: die Aufgaben-PDF mit der höchsten Nummer im Repo-Root verwenden und das kurz nennen.

## Kontext, den du brauchst (zuerst lesen)
1. Die Aufgabendatei (PDF: Read-Werkzeug mit `pages`, höchstens 20 Seiten pro Aufruf; alle Seiten lesen).
2. `AGENTS.md` (Stand, Regeln, Architektur, offene Punkte) und die zentrale `README.md`.
3. Der tatsächliche Stand: `EnergyTwin.sln`, `src/`, `tests/`, `docker/`, `docs/`, `.env.example`, `.editorconfig`, `skills/`, Entwicklungsjournale in `docs/development-journal/`, `git log --oneline -n 15`, `git status --short`.

## Arbeitsschritte
1. **Extrahieren.** Jede Anforderung als eigene Zeile mit ID (`A1`, `A2` …) erfassen und einordnen:
   *Pflicht* · *optional* · *Abgabe/Repo-Inhalt* · *Bewertungskriterium* · *Nachweis (Screenshot/Protokoll)* · *Randbedingung* (z. B. async-Regeln, keine Secrets).
   Auch Verschachteltes erfassen (Mindestanforderungen, „Das Gateway muss …“-Listen, Abgabeliste). Wörtlich knapp zitieren, mit Seitenangabe. Nichts erfinden, Unklares als „Frage ans Team“ markieren.
2. **Abgleichen.** Je Anforderung im Projekt nach Belegen suchen (Code, Konfiguration, Doku, Tests) und mit Dateipfad belegen. Status vergeben:
   - **erfüllt** – Beleg in Code/Config/Doku **und** ein Nachweis (Test grün, Journal/Protokoll mit Lauf, Screenshot).
   - **teilweise erfüllt** – Teil umgesetzt; sagen, was fehlt.
   - **nicht erfüllt** – kein Beleg gefunden (Suchbegriffe nennen).
   - **nicht nachgewiesen** – Code ist da, aber kein belegter erfolgreicher Lauf. Vorhandener Code beweist keinen Lauf.
   Tests nicht selbst ausführen, außer die Userin verlangt es ausdrücklich (Projektregel). Sonst nur vorhandene Nachweise (Journal, README, Testdateien) bewerten und das offen sagen.
3. **Reihenfolge vorschlagen.** Nur offene Punkte (nicht erfüllt, teilweise, nicht nachgewiesen) sortieren nach:
   1. Abhängigkeiten (was blockiert anderes? z. B. SRS/Nachrichtenvertrag vor Gateway, Broker vor Gateway, Gateway vor Viewer),
   2. Pflicht vor optional, Bewertungs-/Abgaberelevanz vor Kür,
   3. Risiko und Aufwand (Unsicheres früh, Dokumentation und Nachweise zuletzt, aber vor der Abgabe),
   4. Zuständigkeit (Teamaufteilung, falls im README dokumentiert).
   Je Schritt: Ziel, grobe Größe (S/M/L), Abhängigkeit, Abnahmekriterium („fertig, wenn …“).
4. **Berichten** im Format unten. Kurz und ohne Füllsätze.

## Ausgabeformat
```
# Aufgabenabgleich: <Datei> (Stand <Datum>, Commit <kurzer Hash>)
## Zusammenfassung   – n Anforderungen: x erfüllt · y teilweise · z nicht erfüllt · w nicht nachgewiesen
## Anforderungen     – Tabelle: ID | Anforderung (Seite) | Art | Status | Beleg / Lücke
## Offene Fragen ans Team
## Vorgeschlagene Reihenfolge  – nummerierte Schritte mit Ziel, Größe, Abhängigkeit, Fertig-wenn
## Hinweise          – Widersprüche zwischen Doku und Code, veraltete Angaben in AGENTS.md/README
```

## Grenzen erlaubter Aktionen
- **Nur lesend:** Dateien lesen, suchen, `git log`/`git status`/`git diff` ansehen. Nichts ändern, nichts anlegen.
- Kein Commit, kein Push, kein Tag, kein Build/Test/Docker-Start, keine Netzwerkzugriffe, keine Subagenten.
- Keine Secrets ausgeben: `.env` nicht lesen (nur `.env.example`).
- Den Bericht nur auf ausdrücklichen Wunsch in eine Datei schreiben (Vorschlag: `docs/task-status/<aufgabe>.md`); das Entwicklungsjournal nie ohne Auftrag ändern.
- Anforderungen werden nie gestrichen, nur weil ihre Umsetzung fehlt: sie bleiben „nicht erfüllt“.
- Antworten auf Deutsch.
