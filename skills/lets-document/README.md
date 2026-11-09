# Skill „Let's Document“

Der Skill pflegt README-Dateien anhand belegter Projektinformationen. Maßgeblich sind die
[Skill-Anweisungen](SKILL.md); die [Archivregeln](references/archive.md) gelten nur nach ausdrücklicher
Bestätigung einer konkreten Archivierung.

| Feld | Inhalt |
|---|---|
| Verantwortlich | Alexander Lux |
| Zweck | README-Lücken klären, Inhalte knapp ergänzen und Wiederholungen verdichten |
| Aufruf | `$lets-document <README-Pfad>`; auch Modul-/Sitzungsauftrag oder ausdrücklich alle READMEs |
| Quellen | Ziel-README, Projektregeln, Code, Konfiguration, Entscheidungen und datierte Nachweise |
| Offene Fragen | Nach Recherche den User fragen; beantwortete Fragen entfernen und Antworten unauffällig einarbeiten |
| Archiv | Nur nach konkreter User-Bestätigung; kurze Zusammenfassung des geänderten alten Bereichs, kein Volltext und kein Archivverweis in der aktuellen README |
| Prüfungen | Keine Tests oder Programmstarts ohne konkrete Zustimmung |

Beispiele:

```text
$lets-document src/Devices/PvSimulator/README.md
$lets-document Aktualisiere die READMEs der aktuellen Änderungen
$lets-document Prüfe alle READMEs in diesem Repository
```

„Prüfe“ verlangt nur eine Bestandsaufnahme; „Aktualisiere“ erlaubt belegte Dokumentationsänderungen.
Eine einzelne README begrenzt den Bearbeitungsbereich. Die
[Anwendung vom 2026-11-08](../../docs/nachweise/2026-11-08-lets-document/README.md)
dokumentiert einen älteren Stand mit vollständigen Originalarchiven; sie ist keine Vorlage für neue Archivierungen.

Die Repository-Fassung ist maßgeblich. Nach Änderungen die lokale Codex-Installation synchronisieren.
