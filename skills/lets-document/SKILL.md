---
name: lets-document
description: >-
  README-Dateien anhand belegter Projektinformationen ergänzen, Wiederholungen verdichten und alte oder
  verworfene Inhalte nachvollziehbar archivieren. Verwenden bei README-Lücken, veralteten Anleitungen oder
  dem Wunsch nach einer konsistenten, kürzeren Projektdokumentation. Bindet den User bei fachlichen
  Widersprüchen gezielt ein; ersetzt weder Aufgabenbewertung noch Funktionsprüfung.
---

# Let's Document

Pflege aktuelle, nutzbare READMEs. Trenne belegte Tatsachen, historische Aussagen und offene Entscheidungen.
Beachte zuerst die Projektregeln, Dokumentationssprache, fachlichen Hauptquellen und Zuständigkeiten.

## Zielbereich

- Einzelner README-Pfad: nur diese README bearbeiten. Nötige Folgeanpassungen anderswo melden; keine anderen
  READMEs, AGENTS, SRS oder Verträge ungefragt ändern. Archivdateien und vorgeschriebene Journale bleiben möglich.
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
Änderungen berücksichtigen. Unbekanntes bleibt offen, nicht als vermutete Tatsache ergänzen.

Bei fachlichen Konflikten nicht automatisch den Code zur Wahrheit erklären: README-Formeln, Aufgabenanforderungen
oder Entscheidungen können verbindlich sein; der Code kann fehlerhaft sein. Unklare Zuständigkeiten nicht erfinden.

## Ergänzen, verdichten, archivieren

- Ergänze nutzbare fehlende Informationen mit Quellenbezug, keine erfundenen Befehle, Defaults oder Testergebnisse.
- Bündele Dopplungen, kürze lange Erklärungen. Erhalte Formeln, Register-/Schnittstellenpläne, gültige Startbefehle,
  Bedienung und Einschränkungen vollständig. Fachlich aktuelle Details gehören in die aktuelle Dokumentation,
  nicht ausschließlich ins Archiv. Root-README: Überblick mit Links; Modul-README: maßgebliche Details.
- Nur nachweislich überholte/verworfene Inhalte als solche archivieren. Alter oder fehlender Code allein reichen nicht.
  Bei Zusammenführung weiterhin gültiger Dopplungen ausdrücklich diesen Grund verwenden, nicht „veraltet“ behaupten.
- Vor Entfernen/Zusammenfassen betroffener Abschnitte [Archivregeln](references/archive.md) lesen und Original sichern.
  Reine Schreibfehlerkorrekturen brauchen kein Archiv. Bestehende Archive nicht überschreiben oder duplizieren.
- Vorhandene fremde Änderungen, Zeilenenden und fachfremde Abschnitte erhalten. Keine Codefehler nebenbei reparieren.

## Gezielt mit dem User entscheiden

Bei einem echten Konflikt oder mehreren fachlich unterschiedlichen Varianten kurze Fragerunden wie bei Grill Me:
Beleg/Widerspruch nennen, eine konkrete Entscheidung fragen, Auswirkungen erklären und eine Empfehlung geben.
Unabhängige Fragen bündeln; abhängige erst nach Antwort stellen. Falls verfügbar `grilling` verwenden, sonst selbst
nach diesem Muster fragen. Den benannten Skill dafür lesen; keine speziellen Tools oder Installationen voraussetzen.

Nur die von einer offenen Entscheidung abhängigen Änderungen zurückstellen. Eindeutige, autorisierte Korrekturen
weiterführen. Keine Bestätigung jeder Formulierung; fehlende Zustimmung niemals durch Wartezeit ersetzen.

## Ausführung und Abschluss

Keine Tests, Benchmarks, Probeläufe, Simulatoren oder Dienste ohne ausdrückliche Zustimmung für die konkrete Prüfung
starten. Ein Dokumentationsauftrag ist keine Ausführungsfreigabe. Vorhandene Nachweise lesen und fehlende Prüfungen
benennen. Verlangt eine Projektregel zusätzliche Prüfungen, die eine noch fehlende Zustimmung benötigen, die
betroffene Prüfung offenlassen und gezielt um Zustimmung bitten; keinen Erfolg behaupten.

Inhalte und relative Links statisch prüfen, fachlich geschützte Angaben mit dem Ausgangstext vergleichen und
Projektregeln für Journale beachten. Keine Secrets in Dokumentation/Archive übernehmen; Beispielkonfigurationen nutzen.

Zum Abschluss knapp berichten: bearbeitete Dateien, ergänzte Lücken, Archivorte, wesentliche Kürzungen,
offene Entscheidungen und tatsächlich ausgeführte Prüfungen. Nicht durchgeführte Läufe bleiben nicht nachgewiesen.
