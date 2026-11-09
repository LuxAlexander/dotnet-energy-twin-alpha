# Vorlage – Journaleintrag (`docs/development-journal/<person>.md`)

Nur die eigene Datei bearbeiten. Pro Tag **ein** Eintrag; weitere Arbeit am selben Tag als `### Nachtrag` darunter.
Reihenfolge der Felder einhalten, keine Felder weglassen. Deutsch, knapp, belegbar.

```markdown
## JJJJ-MM-TT – Titel der Arbeit

- **Done:** Was wurde geändert und warum? Zwei bis fünf Sätze, betroffene Dateien namentlich, keine
  Wiederholung des Codes. Entscheidungen und verworfene Wege mit Grund nennen.
- **KI:** <Modell/Produkt> => was die KI gemacht hat (Analyse, Text, Code, Prüfung) und was der Mensch
  beigetragen oder entschieden hat.
- **Artefact:** neue/geänderte/gelöschte Dateien mit Pfad, Nachweisordner, ggf. Commit-Hash.
- **Test:** was tatsächlich ausgeführt wurde, mit Ergebnis (`dotnet build EnergyTwin.sln` 0 Fehler/0 Warnungen;
  `dotnet test tests/<Projekt>` x/y; Prüfskript-Ausgabe). Nicht Ausgeführtes ausdrücklich als „nicht ausgeführt“
  benennen – keine erfundenen Läufe.
- **Comment:**
  - Beobachtungen, Einschränkungen, offene Punkte, Folgeschritte.
  - Nicht committet. (oder: Commit `<hash>` mit wörtlicher Message)

### Nachtrag JJJJ-MM-TT – Titel

- **Done:** …
- **KI:** …
- **Artefact:** …
- **Test:** …
- **Comment:**
  - …
```

## Regeln

- **Ein Tag, ein `##`-Eintrag.** Alles Weitere am selben Tag wird `### Nachtrag`.
- **Keine fremden Journale.** Maßgeblich ist die Person, in deren Arbeitsstand gearbeitet wird (`AGENTS.md` §0.1).
- **Testzahlen nur aus einem belegten Lauf** (eigener Lauf oder datierter Nachweis). Sonst: „nicht ausgeführt“.
- **Datum aus dem Nutzerkontext**, nicht aus der Systemuhr raten.
- **Keine Secrets**, keine `.env`-Inhalte, keine personenbezogenen Zugangsdaten.
- **Committet oder nicht?** Immer im `Comment` sagen. Ohne Anweisung wird nicht committet.
- **Ältere Einträge nicht umschreiben**; Korrekturen kommen als neuer Eintrag/Nachtrag mit Bezug.
