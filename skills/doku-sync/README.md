# Skill `doku-sync`

| Feld | Inhalt |
|---|---|
| **Verantwortliche Person** | Dominik Zach |
| **Zweck** | Nach einer Änderung bestimmen, **welche** Dokumente nachgezogen werden müssen, und sie belegbar nachziehen: zentrale `README.md`, `AGENTS.md` (Stand-Kopf, §0, §3, §6), Modul-READMEs, Nachrichtenverträge, Nachweise und Entwicklungsjournal. Ergänzt die LB03-Anforderung „eigener Skill pro Person“ um den Dokumentationsabgleich nach jeder Arbeit. |
| **Aufruf** | `@doku-sync <Anlass>` · z. B. `@doku-sync Modbus-Register geändert` · ohne Anlass: aus `git status` und letztem Commit ableiten und bestätigen lassen. Kein Slash-Command; der Text nach `@doku-sync` ist der Anlass. |
| **Benötigter Kontext** | `AGENTS.md` (§0.1, §0, §3, §6), zentrale `README.md`, betroffenes Modul-README und Code, `docs/`-Landkarte, `git status`/`git log`, vorhandene datierte Nachweise |
| **Arbeitsschritte** | 1. Anlass und Wirkungskreis aus `git status`/`git diff` · 2. Pflichtdokumente aus der Landkarte · 3. Ist-Stand belegen statt annehmen · 4. nachziehen in fester Reihenfolge (Vertrag → Modul-README → zentrale README → `AGENTS.md` → Journal) · 5. Prüfer laufen lassen · 6. knapp berichten |
| **Ergebnis** | kurze Lückenliste „Datei – was fehlt – Beleg“, die nachgezogenen Stellen und das Prüfergebnis; offene Punkte bleiben offen |
| **Abgrenzung** | `lets-document` regelt **wie** eine README formuliert und archiviert wird, `readTask` prüft die Aufgabenerfüllung, `probelauf` liefert den Laufzeitnachweis. `doku-sync` entscheidet **was/wo** nachgezogen wird. |
| **Grenzen** | Nur lesend prüfbar, kein Build/Test/Docker/Probelauf, kein Commit/Push/Tag, keine fremden Journale, keine Secrets, kein eigenmächtiges Archivieren |
| **Ablage** | versioniert in `skills/doku-sync/`; `.opencode/skills/doku-sync` bzw. `.claude/skills/doku-sync` ist eine lokale, ignorierte Kopie |

Die eigentlichen Anweisungen stehen in [`SKILL.md`](SKILL.md), die Dokumentenlandkarte in
[`references/dokumentenlandkarte.md`](references/dokumentenlandkarte.md), die Journalvorlage in
[`templates/journal-eintrag.md`](templates/journal-eintrag.md).

## Warum es den Skill gibt

Beim Umbau in LB02/LB03 wanderten dieselben Fakten durch viele Dateien: eine geänderte Testzahl steht in
`AGENTS.md` §0.3 **und** §6 **und** der zentralen `README.md`; ein neues Register im `Modbus.Contracts`-README,
im Geräte-README und in §0.5a; ein neuer Nachweis im Nachweisordner, im Stand-Kopf und in §6. Vergisst man eine
Stelle, widersprechen sich die Dokumente – und genau das ist bei einer Abgabe teuer.

`lets-document` beschreibt, wie man eine README gut schreibt. Es sagt aber nicht, **welche** Dateien eine Änderung
berührt. Diese Lücke schließt `doku-sync`: erst die Landkarte, dann die Änderung, dann ein Prüfer, der strukturelle
Brüche findet.

## Abgrenzung

| Frage | Skill | Ergebnis |
|---|---|---|
| Was muss nachgezogen werden, wo steht es? | **`doku-sync`** | Lückenliste + nachgezogene Dateien |
| Wie formuliere ich eine README, was wird archiviert? | `lets-document` | überarbeitete README + Archiv |
| Ist die Aufgabe laut Aufgabenblatt erfüllt? | `readTask` | Statusbericht mit Beleg/Lücke |
| Funktioniert es zur Laufzeit? | `probelauf` | Nachweis mit Protokollen |

Die vier schließen sich nicht aus: `probelauf` erzeugt den Beleg, `doku-sync` trägt ihn an die richtigen Stellen,
`lets-document` formuliert dort sauber, `readTask` prüft am Ende die Abgabe.

## Ablauf in Kurzform

1. **Anlass und Wirkungskreis:** `git status --short`, `git log -1 --stat`, `git diff --stat`. Was hat sich
   verhaltensmäßig geändert – nicht nur, welche Datei angefasst wurde?
2. **Pflichtdokumente:** Matrix „Änderungsart → Pflichtdokumente“ in der Landkarte.
3. **Belegen:** Code, Konfiguration, datierter Nachweis. Testzahlen nur aus einem dokumentierten Lauf.
4. **Nachziehen:** Vertrag/Registerplan → Modul-README → zentrale `README.md` → `AGENTS.md` → Journal.
5. **Prüfen:** `pruefe-doku.ps1` (siehe unten).
6. **Berichten:** geänderte Dateien, bewusst nicht geänderte Stellen, offene Punkte, Prüfergebnis, Commit-Status.

## Prüfer

```powershell
powershell -File skills/doku-sync/scripts/pruefe-doku.ps1 -Person dominik-Zach.md -Datum 2026-11-09
```

Read-only, läuft aus dem Repo-Root (erkennt `EnergyTwin.sln`), ändert und committet nichts:

| # | Prüfung | Warum |
|---|---|---|
| 1 | relative Markdown-Links | Umbenennungen und verschobene Nachweise erzeugen sonst tote Verweise. `graphify-out/` (generiert) und `docs/archive/` (bewahrt Originalverweise) sind ausgenommen, Codeblöcke werden ignoriert. |
| 2 | Pfadangaben in Backticks in `AGENTS.md`/`README.md` | Genau dort stehen die Pfade, die niemand beim Build prüft. |
| 3 | `*.log` unter `docs/` | `.gitignore` schließt `*.log` aus; Protokolle müssen `.txt` sein. |
| 4 | versionierte `.env`-Dateien | Keine Secrets im Repo, nur `.env.example`. |
| 5 | Journal-Eintrag der eigenen Person | `AGENTS.md` §0.1 verlangt einen Eintrag pro Änderung. |

Beispielausgabe des ersten vollständigen Laufs (2026-11-09):

```
1) Markdown-Links: 141 in 54 Dateien (ohne graphify-out/, docs/archive/)
2) Pfadangaben in AGENTS.md/README.md: 50
3) *.log unter docs/: 0
4) versionierte .env-Dateien: 0
5) Journal dominik-Zach.md fuer 2026-11-09: geprueft
```

Der Prüfer kennt **keine Absicht**. Ein Befund ist ein Prüfauftrag: Ein toter Link kann gewollt historisch sein,
ein fehlender Pfad kann auf ein noch nicht geschriebenes Dokument zeigen. Deshalb wird jeder Befund kommentiert
und nicht automatisch „repariert“.

## Erste Anwendung (2026-11-09)

Der Prüfer lief zuerst gegen den echten Repo-Stand und förderte zwei Befunde zutage:

| Befund | Behandlung |
|---|---|
| `AGENTS.md` Zeile 22 verweist auf `docs/handoff/2026-11-08-alexander-lb03.md`, das es nicht gibt (auch nicht in Git) | **gemeldet**, nicht geändert – fremder Arbeitsstand; das Handoff-Dokument muss von Alexander nachgeliefert oder der Verweis entfernt werden |
| Kein Journal-Eintrag für 2026-11-09 in `dominik-Zach.md` | durch diesen Arbeitsschritt behoben |

Der erste Skriptlauf meldete 61 Befunde. 59 davon waren Fehlalarme: `docs/archive/` bewahrt absichtlich die alten
relativen Verweise, und das Platzhalterbeispiel in `references/archive.md` steht in einem Codeblock. Daraufhin
überspringt der Prüfer Archive und eingezäunte Codeblöcke. Ein Selbsttest in einem Wegwerf-Repo bestätigte, dass
tote Links, fehlende Pfadangaben und ein fehlendes Journal weiterhin erkannt werden.

## Dateien im Ordner

| Datei | Inhalt |
|---|---|
| `SKILL.md` | Ablauf, Regeln und Grenzen – die eigentlichen Anweisungen |
| `README.md` | dieser Überblick mit Zweck, Abgrenzung, Prüfer und Beispiel |
| `references/dokumentenlandkarte.md` | Dokument → Inhalt → Auslöser; Matrix „Änderungsart → Pflichtdokumente“; „nie von Hand ändern“ |
| `templates/journal-eintrag.md` | Felder `Done`, `KI`, `Artefact`, `Test`, `Comment` und die Journalregeln |
| `scripts/pruefe-doku.ps1` | read-only Prüfer für Links, Pfade, `*.log`, `.env` und Journal |

## Ablage und Aktivierung

Die versionierte Fassung liegt in **`skills/doku-sync/`**; `.opencode/` und `.claude/` sind lokale, ignorierte
Ablagen (zentrale `README.md`, Abschnitt „Konfiguration“). Für OpenCode aus dem Repository-Root kopieren, sofern
dort noch keine eigene Fassung liegt:

```powershell
New-Item -ItemType Directory -Force .opencode/skills
Copy-Item -Recurse skills/doku-sync .opencode/skills/doku-sync
```

Bei späteren Änderungen die lokale Kopie gezielt abgleichen. Der Skill wird über seine ID `doku-sync` geladen
(aus dem Ordnernamen); ein Eintrag in `opencode.json` ist für diesen Ordner nicht nötig, wenn er dort liegt.

Aufruf im Prompt:

- `@doku-sync <Anlass>` – lädt diesen Skill und übergibt den Anlass als Text (das Skill-Werkzeug selbst hat keine
  Parameter), z. B. `@doku-sync Gateway-Nachweis`.
- Ohne Mention: den Auftrag einfach beschreiben; das Modell sieht die `description` und lädt den Skill selbst.

## Quelle

Eigenentwicklung für dieses Projekt, keine fremde Vorlage. Idee aus der LB03-Anforderung „eigener Skill pro Person“
und aus den wiederkehrenden Doku-Nachträgen in `AGENTS.md` §0.1. Keine fremde Lizenz zu beachten.

## Bekannte Grenzen

- Der Prüfer findet strukturelle Brüche, keine fachlich falschen Aussagen. Ob eine Zahl stimmt, entscheidet der Beleg.
- Ohne ausgeführten Lauf bleibt jede Verhaltensaussage „nicht nachgewiesen“; der Skill startet nichts.
- `docs/archive/` wird bewusst nicht geprüft – dort sind tote Verweise erwartbar.
- Die Landkarte muss bei neuen Dokumenten mitgepflegt werden; sonst zeigt sie ins Leere.
