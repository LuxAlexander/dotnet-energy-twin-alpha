---
name: doku-sync
description: >-
  Prüft nach einer Änderung, welche Dokumente im EnergyTwin-Repo nachgezogen werden müssen, und zieht sie
  konsistent nach: zentrale README, AGENTS.md (Stand-Kopf, §0, §3, §6), Modul-READMEs, Nachrichtenverträge,
  Nachweise und Entwicklungsjournal. Nennt jede Änderung mit Beleg und lässt Nicht-Belegbares offen. Enthält
  einen lesenden Prüfer für relative Links, Pfadangaben, Journal-Eintrag, *.log und versionierte .env.
  Aufruf z. B. "@doku-sync Modbus-Register geändert". Nicht für neue README-Inhalte von Grund auf
  (lets-document) und nicht für Laufzeitnachweise (probelauf).
---

# doku-sync – Dokumentation nach einer Änderung nachziehen

**Verantwortlich:** Dominik Zach (Team Alpha). Details, Abgrenzung und Beispiel: [`README.md`](README.md).

## Aufruf

`@doku-sync <Anlass>` – z. B. `@doku-sync Modbus-Register geändert`, `@doku-sync neuer CLI-Schalter`,
`@doku-sync Gateway-Nachweis`. Der Text dahinter ist der Anlass; das Skill-Werkzeug selbst nimmt keine Parameter.
Ohne Anlass: **aus `git status` und dem letzten Commit ableiten und den Umfang bestätigen lassen** – nicht raten.

Der Skill wird über seine ID `doku-sync` geladen (Ordner `.opencode/skills/doku-sync`, die ID kommt aus dem
Ordnernamen). Ein Slash-Command gibt es bewusst nicht.

## Abgrenzung (wichtig, sonst wird doppelt gearbeitet)

| Frage | Zuständig |
|---|---|
| **Was** muss nachgezogen werden, **wo** steht es? | **dieser Skill** |
| **Wie** formuliert man eine README, was wird archiviert? | `lets-document` (Alexander Lux) |
| Ist die Aufgabe laut Aufgabenblatt erfüllt? | `readTask` (Susanne Lastowicka) |
| Funktioniert es zur Laufzeit? | `probelauf` (Dominik Zach) |

`doku-sync` ändert keine Formulierungen aus Geschmack, archiviert nichts eigenmächtig und führt nichts aus.
Es liefert die Lückenliste und zieht belegte Stellen nach.

## Zuerst lesen (Pflicht, in dieser Reihenfolge)

1. `AGENTS.md` §0.1 (Arbeitsregeln, Journalpflicht), §0 (Stand-Kopf), §3 (Architektur), §6 (Build-Status).
2. `README.md` im Repo-Root (Zuständigkeiten, Startreihenfolge, Tests).
3. In diesem Ordner: `references/dokumentenlandkarte.md` (Dokument → Inhalt → Auslöser).
4. Die README des betroffenen Projekts und den betroffenen Code – nicht aus dem Gedächtnis.

## Ablauf

### 1. Anlass und Wirkungskreis bestimmen (nur lesend)

```powershell
git status --short
git log -1 --stat --oneline
git diff --stat
```

Notieren: **was** hat sich geändert (Verhalten, Schnittstelle, Konfiguration, Zahl, Struktur) und **welche
Dokumente** das berührt. Eine Umbenennung berührt mehr als eine Formulierung; eine Testzahl berührt mehr als
eine Zeile.

### 2. Pflichtdokumente aus der Landkarte ableiten

`references/dokumentenlandkarte.md`: Abschnitt „Änderungsart → Pflichtdokumente“. Ergebnis ist eine kurze Liste
„Datei – was fehlt – Beleg“. Diese Liste ist der Auftrag; alles andere bleibt unangetastet.

### 3. Ist-Stand belegen, nicht annehmen

- Aussagen über Verhalten **aus Code, Konfiguration oder einem datierten Nachweis** belegen.
- Testzahlen nur nennen, wenn der Lauf tatsächlich dokumentiert ist (Journaleintrag oder Nachweisordner) –
  **nie** aus `AGENTS.md` abschreiben, wenn dort ein älterer Stand steht.
- Code zeigt die Implementierung, keinen erfolgreichen Lauf. Testdateien zeigen Prüffälle, kein grünes Ergebnis.
- Unbekanntes bleibt offen und wird als offen gemeldet, nicht als Vermutung formuliert.

### 4. In fester Reihenfolge nachziehen

1. **Vertrag / Registerplan / SRS** – zuerst die fachliche Quelle, weil Module sich darauf berufen.
2. **Modul-README** (`src/<Projekt>/README.md`) – Optionen, Defaults, Bedienung, Schnittstellen, Prüfstand.
3. **Zentrale `README.md`** – nur Überblick, Zuständigkeiten, Startbefehle, Testzahlen, Links.
4. **`AGENTS.md`** – Stand-Kopf, §0.2–§0.4, §3, §6, ggf. §0.5/§0.5a; veraltete Verweise ersetzen.
5. **`docker/README.md`**, `docs/viewer/README.md`, `docs/task-status/…` – nur wenn betroffen.
6. **Nachweis** unter `docs/nachweise/JJJJ-MM-TT-<thema>/` – nur wenn ein Lauf tatsächlich stattfand.
7. **Journal der eigenen Person** (`docs/development-journal/…`) – Pflicht laut `AGENTS.md` §0.1.

Reihenfolge einhalten: Wird zuerst die zentrale README geändert, wandern falsche Details nach oben.

### 5. Prüfen

```powershell
powershell -File .opencode/skills/doku-sync/scripts/pruefe-doku.ps1 -Person dominik-Zach.md -Datum 2026-11-09
```

Der Prüfer ist **read-only** und meldet:
1. relative Markdown-Links (ohne `graphify-out/` und `docs/archive/`),
2. Pfadangaben in Backticks in `AGENTS.md`/`README.md`,
3. `*.log` unter `docs/` (muss `.txt` sein),
4. versionierte `.env`-Dateien,
5. Journal-Eintrag der eigenen Person für das Datum.

Ein Befund ist ein **Prüfauftrag, keine automatische Korrektur**: Der Prüfer kennt keine Absicht. `docs/archive/`
ist absichtlich ausgenommen, weil Archive den Originaltext samt damals gültiger Verweise bewahren.

### 6. Berichten

Kurz und prüfbar:
- geänderte Dateien mit einer Zeile Begründung,
- geprüfte, aber bewusst **nicht** geänderte Stellen (mit Grund),
- offene Punkte und Widersprüche (Datei, Zeile, Aussage, Gegenbeleg),
- Ergebnis des Prüfers (Prüfungen, Befunde),
- ob committet wurde (**in der Regel nicht**).

## Regeln

- **Nichts committen, pushen oder taggen** ohne ausdrückliche Anweisung (`AGENTS.md` §0.1); Commit-Message wörtlich, ohne `Co-Authored-By`.
- **Nur die eigene Journaldatei** bearbeiten, nie die anderer Personen.
- **Fremde und uncommittete Änderungen nicht überschreiben.** Vor dem Ersetzen eines Absatzes den aktuellen Text lesen; bei fremden Arbeitsständen fragen.
- **Keine Secrets** in Doku oder Nachweise übernehmen; `.env` nie lesen oder ausgeben, nur `.env.example` nennen.
- **Testläufe** nur auf ausdrücklichen Wunsch oder vor einem Commit. Ein Doku-Auftrag ist keine Freigabe für Build, Tests, Docker oder einen Probelauf.
- **Datum aus dem Nutzerkontext** (heute), nicht aus dem Systemzufall; „zuletzt geprüft“ nur mit Belegdatum.
- **Widersprüche nicht glätten:** Beleg, Auswirkung und Empfehlung nennen; die Entscheidung trifft das Team.

## Grenzen

- Ersetzt weder `lets-document` (Formulieren/Archivieren) noch `probelauf` (Laufzeitnachweis) noch `readTask` (Anforderungsabgleich).
- Der Prüfer findet keine fachlich falschen Aussagen, nur strukturelle Brüche.
- Ohne ausgeführten Lauf bleibt jede Verhaltensaussage „nicht nachgewiesen“.
