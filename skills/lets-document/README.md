# Skill „Let's Document“

Diese Projektbeschreibung belegt Alexanders eigenen Skill für LB03. Die eigentlichen wiederverwendbaren
Anweisungen stehen in [`SKILL.md`](SKILL.md); Archivdetails werden nur bei Bedarf aus [`references/archive.md`](references/archive.md) geladen.

| Feld | Inhalt |
|---|---|
| Verantwortlich | Alexander Lux |
| Zweck | README-Lücken belegbar ergänzen, Dopplungen verdichten, alte/verworfene Inhalte nachvollziehbar archivieren |
| Aufruf | `$lets-document <README-Pfad>`; außerdem Modul-/Sitzungsauftrag oder ausdrücklich alle READMEs |
| Kontext | Ziel-README, Projektregeln, zugehöriger Code/Konfiguration und vorhandene datierte Nachweise |
| User-Einbindung | Fachliche Konflikte als kurze, gezielte Fragerunde mit Beleg, Auswirkung und Empfehlung |
| Grenzen | Keine Tests oder Programmstarts ohne konkrete Zustimmung; Einzeldateigrenze einhalten; keine erfundenen Ergebnisse |
| Ablage | Diese Repository-Fassung ist maßgeblich; lokal sind Skill-Einstieg, Referenz und Codex-Metadaten installiert |

## Beispiele

```text
$lets-document src/Devices/PvSimulator/README.md
$lets-document Aktualisiere die READMEs der aktuellen Änderungen
$lets-document Prüfe alle READMEs in diesem Repository
```

„Prüfe“ fordert eine Bestandsaufnahme an, „Aktualisiere“ autorisiert belegte Dokumentationsänderungen.
Bei einem einzelnen README-Pfad ist dessen Bearbeitung gemeint; weitere README-Anpassungen werden nur gemeldet.

## Dokumentierter Beispielaufruf vom 2026-11-08

```text
$lets-document src/Devices/PvSimulator/README.md
Aktualisiere nur die Einleitung und bündele den Anzeigehinweis im Abschnitt Konsolenausgabe.
Ergänze den vorhandenen datierten PV-MQTT-Empfangsnachweis. Erhalte Formeln, Registerplan,
Startbefehle und übrige Inhalte. Archiviere die ersetzten Originalabschnitte nachvollziehbar.
```

**Ergebnis:** Einleitung auf den aktuellen Master-/Standalone-, Modbus- und Wetterbetrieb ausgerichtet;
Anzeigehinweis ohne Informationsverlust zusammengeführt; vorhandenen Empfangsnachweis vom 2026-11-08 mit Grenzen ergänzt.
Originaltext: [`docs/archive/2026-11-08-pv-readme.md`](../../docs/archive/2026-11-08-pv-readme.md).
Alle vorhandenen Code-/Formelblöcke unverändert, keine andere README im Beispielauftrag bearbeitet.
Keine neuen Lauf-/Testergebnisse behauptet. Die Registrierung dieses Skills im zentralen README gehört zum
separaten Erstellungsauftrag, nicht zum Einzeldatei-Beispiel.

## Überprüfung

Formale Skill-Prüfung erfolgreich. Zusätzlicher unabhängiger Dokumentationsdurchgang auf isolierten Beispieldateien:
belegte Startoption berichtigt, Testbestand von letztem erfolgreichen Lauf getrennt, unklaren Formelkonflikt
dem User vorgelegt und nicht eigenmächtig aufgelöst. Keine Tests, Programme oder Dienste dafür gestartet.
Ausführlicher [Nachweis](../../docs/nachweise/2026-11-08-lets-document/README.md).

Projektregel: Der vorgeschriebene Solution-Build wurde im Erstellungsauftrag ausgeführt; das ist kein Nachweis eines
erfolgreichen Simulator- oder Unit-Testlaufs. Nach Änderungen an der Repository-Fassung die lokal installierten
Skill-Dateien synchronisieren, damit keine zwei unterschiedlichen Anweisungsstände entstehen.
