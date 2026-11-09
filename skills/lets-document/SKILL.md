---
name: lets-document
description: >-
  README-Dateien anhand belegter Projektinformationen knapp ergänzen und verdichten.
  Verwenden bei README-Lücken, veralteten Anleitungen oder Dokumentations-Refactoring.
  Offene Fragen mit dem User klären; Archivierung nur nach dessen ausdrücklicher Bestätigung.
---

# Let's Document

Pflege aktuelle, knappe und aussagekräftige READMEs. Beachte Projektregeln, Dokumentationssprache,
fachliche Hauptquellen und Zuständigkeiten.

## Zielbereich

- Einzelner README-Pfad: nur diese README bearbeiten. Nötige Folgeanpassungen anderswo melden;
  keine anderen READMEs, AGENTS, SRS oder Verträge ungefragt ändern. Vorgeschriebene Journale bleiben möglich.
- Modul oder aktuelle Arbeit: betroffene READMEs ermitteln, notwendige Anpassungen im zentralen README einbeziehen.
- Alle READMEs nur auf ausdrücklichen Auftrag. Generierte/vendor-Dateien entsprechend den Projektregeln behandeln.
- Ohne eindeutiges Ziel aus dem Gespräch einen kleinen sinnvollen Bereich ableiten; bei mehreren plausiblen Zielen nachfragen.
- Bei Prüf-/Vorschau-Auftrag nur Befunde liefern. Bei Bearbeitungsauftrag eindeutige Korrekturen direkt durchführen.

## Belege und Lücken

Lies die Ziel-README und suche gezielt passende Quellen: Code, Konfiguration, Schnittstellen, vorhandene Entscheidungen
und datierte Nachweise. Prüfe je nach Projekt Zweck, Einstieg, Voraussetzungen, Optionen/Defaults, Bedienung,
Schnittstellen, Beispiele, Prüfstand und Einschränkungen; keine leeren Standardschablonen erzeugen.

Ergänze nur Belegbares. Code zeigt Implementierung, kein erfolgreiches Ausführen. Testdateien zeigen Prüffälle,
keinen grünen Testlauf. Vorhandene Lauf-/Testnachweise mit Datum und Aussagegrenze nennen; relevante spätere
Änderungen berücksichtigen. Unbekanntes nicht als Tatsache ergänzen.

Bei fachlichen Konflikten nicht automatisch den Code zur Wahrheit erklären: README-Formeln, Aufgabenanforderungen
oder Entscheidungen können verbindlich sein; der Code kann fehlerhaft sein. Unklare Zuständigkeiten nicht erfinden.

## Schreiben und Archivieren

- Formuliere kurz und konkret. Streiche lange Prosa und Wiederholungen; nutze Listen oder Tabellen, wenn sie
  Information leichter erfassbar machen. Erhalte Formeln, Register-/Schnittstellenpläne, gültige Startbefehle,
  Bedienung und Einschränkungen vollständig. Root-README: Überblick mit Links; Modul-README: maßgebliche Details.
- Fachlich aktuelle Details gehören in die aktuelle Dokumentation, nicht ausschließlich ins Archiv.
- Archiviere entfernte oder zusammengefasste Inhalte **nur nach ausdrücklicher Bestätigung des Users für diese
  Archivierung**. Eine allgemeine Bitte um README-Pflege ist keine Archivfreigabe. Vorher den betroffenen Bereich
  und eine knappe Zusammenfassung des alten Inhalts zur Entscheidung vorlegen; die übrige autorisierte Arbeit fortsetzen.
- Nach Bestätigung [Archivregeln](references/archive.md) lesen. Nur den veränderten Bereich zusammenfassen,
  niemals das gesamte Dokument oder den vollständigen Originaltext archivieren. In der aktuellen README keinen
  Verweis auf das Archiv ergänzen. Bestehende Archive nicht überschreiben oder duplizieren.
- Vorhandene fremde Änderungen, Zeilenenden und fachfremde Abschnitte erhalten. Keine Codefehler nebenbei reparieren.

## Gezielt mit dem User entscheiden

Recherchiere offene Fragen und Dokumentationslücken zunächst in verfügbaren Quellen. Was danach fachlich offen
bleibt, frage den User gezielt: Befund nennen, Entscheidung oder fehlende Information erfragen, Auswirkung und
gegebenenfalls Empfehlung erklären. Unabhängige Fragen bündeln; abhängige erst nach Antwort stellen. Falls
verfügbar `grilling` verwenden, sonst selbst nach diesem Muster fragen.

Beantwortete Fragen aus der aktuellen Dokumentation entfernen und die Antwort ohne besondere Hervorhebung in
den passenden Fließtext einarbeiten. Unbeantwortete Fragen nicht als Vermutung auflösen. Nur die davon abhängigen
Änderungen zurückstellen; eindeutige, autorisierte Korrekturen weiterführen. Fehlende Zustimmung zur Archivierung
niemals durch Wartezeit ersetzen.

## Ausführung und Abschluss

Keine Tests, Benchmarks, Probeläufe, Simulatoren oder Dienste ohne ausdrückliche Zustimmung für die konkrete Prüfung
starten. Ein Dokumentationsauftrag ist keine Ausführungsfreigabe. Vorhandene Nachweise lesen und fehlende Prüfungen
benennen. Verlangt eine Projektregel zusätzliche Prüfungen, die eine noch fehlende Zustimmung benötigen, die
betroffene Prüfung offenlassen und gezielt um Zustimmung bitten; keinen Erfolg behaupten.

Inhalte und relative Links statisch prüfen, fachlich geschützte Angaben mit dem Ausgangstext vergleichen und
Projektregeln für Journale beachten. Keine Secrets in Dokumentation/Archive übernehmen; Beispielkonfigurationen nutzen.

Zum Abschluss knapp berichten: bearbeitete Dateien, ergänzte Lücken, Archivorte nur bei freigegebener Archivierung,
wesentliche Kürzungen, offene Entscheidungen und tatsächlich ausgeführte Prüfungen.
