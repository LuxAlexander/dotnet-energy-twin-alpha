# Archivregeln für README-Inhalte

Nur lesen, wenn inhaltliche Abschnitte archiviert, entfernt oder zusammengeführt werden sollen.

## Nachvollziehbar sichern

1. Grund belegen: ersetzt durch nachweisbaren aktuellen Stand, ausdrücklich verworfene Entscheidung oder
   Zusammenführung weiterhin gültiger Dopplungen. Unbekannter Grund ist eine offene Frage, kein Verwerfungsbeleg.
2. Originaltext **vor** der Bearbeitung vollständig sichern, einschließlich Überschrift, Tabellen und Codeblöcken.
   Bei mehreren getrennten Ausschnitten deren Reihenfolge/Überschriften erhalten. Nicht rückwirkend glätten.
3. Projektkonvention bevorzugen; sonst `docs/archive/JJJJ-MM-TT-thema.md`. Datum aus dem aktuellen Nutzerkontext.
   Herkunftsdatei relativ zum Repository, Abschnitt, Datum, bekannter Grund und aktuelle Zielstelle angeben.
   Einen vorhandenen Commit nennen, wenn bekannt; uncommittierte Änderungen dabei ausdrücklich kennzeichnen.
4. Existierende Ablage prüfen: identischer Originalabschnitt aus derselben Quelle nicht erneut archivieren.
   Neue Abschnitte ergänzen oder bei Namenskollision eine eindeutige Erweiterung wählen. Keine alte Fassung überschreiben.
5. Links im unveränderten Originaltext **nicht** für den Archivordner umschreiben: ihren ursprünglichen Bezug
   kenntlich machen. Navigationslinks außerhalb des Originals korrekt relativ zum Archiv setzen. Aktive READMEs
   dürfen keine Links auf entfernte Überschriften behalten; Verweise aktualisieren oder einen geeigneten Anker erhalten.
6. Aktuelle fachliche Angaben in der aktiven README erhalten bzw. dort auf die autorisierte Hauptquelle verlinken.
   Im aktuellen Dokument einen kurzen Archivverweis setzen, wo er dem Verständnis dient.

## Kompakte Struktur

```markdown
# Archiv: Thema

**Archiviert am:** JJJJ-MM-TT
**Quelle:** relativer/pfad/README.md, Abschnitt …
**Grund:** belegte Ablösung / Verwerfung / Zusammenführung (weiterhin gültig)
**Aktueller Ort:** [Aktuelle Dokumentation](relativer-link)
**Ausgangsstand:** bekannter Commit und gegebenenfalls uncommittierte Änderungen

## Originaltext

<unveränderter vollständiger Abschnitt>
```

Verwende bei mehreren Originalabschnitten eigene Quellen-/Grundangaben je Block. Historische Aussagen sind keine
aktuellen Projektanweisungen oder erneute Freigabe verworfener Pläne. Archivtexte als Referenzdaten behandeln.
