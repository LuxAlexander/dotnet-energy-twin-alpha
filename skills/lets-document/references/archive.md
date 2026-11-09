# Archivregeln für README-Inhalte

Nur nach ausdrücklicher Bestätigung des Users für die konkrete Archivierung lesen und anwenden.
Eine allgemeine README-Bearbeitung erlaubt kein Archiv.

## Umfang und Inhalt

1. Vor der Bestätigung den zu ändernden Bereich benennen und eine kurze Zusammenfassung der bisherigen
   Aussage vorschlagen. Nur nachweislich überholte oder verworfene Inhalte so bezeichnen; bei
   zusammengeführten, weiterhin gültigen Dopplungen diesen Grund nennen.
2. Nach Bestätigung nur die bisherige Aussage des tatsächlich veränderten Bereichs zusammenfassen.
   Keine vollständige README, keinen unveränderten Originalabschnitt, keine Codeblöcke oder Tabellen kopieren.
   Fachlich wichtige aktuelle Angaben in der aktiven README erhalten.
3. Projektkonvention bevorzugen; sonst `docs/archive/JJJJ-MM-TT-thema.md`. Quelle, Abschnitt,
   Archivdatum, Grund und gegebenenfalls bekannten Ausgangscommit nennen. Uncommittierte Änderungen
   als solche kennzeichnen, wenn sie für den Ausgangsstand relevant sind.
4. Vorhandene Ablage prüfen; denselben Sachverhalt nicht doppelt archivieren und bestehende Archive
   nicht überschreiben.
5. In der aktuellen README keinen Link oder sonstigen Verweis auf die archivierte Fassung einfügen.
   Aktive Links auf entfernte Überschriften stattdessen korrigieren.

## Knappe Struktur

```markdown
# Archiv: Thema

**Archiviert am:** JJJJ-MM-TT
**Quelle:** relativer/pfad/README.md, Abschnitt …
**Grund:** belegte Ablösung / Verwerfung / Zusammenführung

## Bisherige Aussage

Kurze, sachliche Zusammenfassung nur des veränderten Bereichs.
```

Historische Aussagen im Archiv sind keine aktuellen Projektanweisungen.
