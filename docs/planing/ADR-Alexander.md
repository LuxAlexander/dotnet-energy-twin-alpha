# Entscheidungen – EnergyTwin

**Stand:** 2026-11-09 · **Zusammengeführt durch:** Alexander Lux mit Codex

Diese Sammlung verdichtet die bisherigen lokalen ADRs. Nummern, Entscheidungsgründe und relevante Folgen bleiben erhalten; Gesprächsrunden und Wiederholungen sind zusammengeführt. Die Projekt-READMEs bleiben maßgeblich für aktuelle Bedienung, Formeln und Schnittstellen. Ein damaliger Beschluss ist kein Ausführungsnachweis.

Auf ausdrücklichen Wunsch ist diese Sammlung versionierbar. Alle 15 vollständigen Originaltexte sind lokal in `docs/adr/2026-11-09-originale.md` gesichert; das Archiv bleibt ignoriert und ist nicht Bestandteil eines Checkouts. ADR-0003 wurde auf Nutzerwunsch aus der aktiven Sammlung entfernt. Die Nummer bleibt unbesetzt; damit ändern sich weder Code noch Toleranzangaben der Projekt-READMEs.

## Übersicht

| ADR | Entscheidung | Einordnung |
|---|---|---|
| [0001](#adr-0001) | Wettertransport per HTTP | Angenommen |
| [0002](#adr-0002) | Zuständigkeiten Wetter und PV | Historischer Zuordnungsstand |
| [0004](#adr-0004) | Arbeitsunterlagen und Veröffentlichung | Ablageentscheidung teilweise ersetzt |
| [0005](#adr-0005) | Wetter-HTTP-Vertrag | Angenommen |
| [0006](#adr-0006) | Standort und Niederschlag | Angenommen |
| [0007](#adr-0007) | Manuelle Wetterübersteuerung | Durch 0013 präzisiert und teilweise ersetzt |
| [0008](#adr-0008) | Wetterfehler im Gerät | Angenommen |
| [0009](#adr-0009) | WeatherSynth und Ergänzungsmodell | Durch 0013 regional erweitert |
| [0010](#adr-0010) | Standortkatalog | Durch 0013 erweitert |
| [0011](#adr-0011) | Hochlauf und PV-Anbindung | Historische Planung |
| [0012](#adr-0012) | UTC und lokale Anzeige | Angenommen |
| [0013](#adr-0013) | Wetter-MQTT und Klimaprofile | Bestätigt; datierter Umsetzungsnachweis |
| [0014](#adr-0014) | GetStatus-Skill | Verworfen zugunsten 0015 |
| [0015](#adr-0015) | Let's Document | Bestätigt; datierter Erstellungsnachweis |

<a id="adr-0001"></a>
## ADR-0001: Wettertransport per HTTP

**Beschluss:** angenommen am 2026-10-26.

Der eigenständige Wettersimulator stellt eine HTTP-Minimal-API über Kestrel bereit. Feldgeräte fragen Standort und virtuelle Zeit asynchron per `HttpClient` ab. Gegenüber dauerhaften TCP-Verbindungen mit JSON-Zeilen ist HTTP für seltene, einseitige Anfragen einfacher; eine reine In-Process-Bibliothek erfüllt die Prozessabgrenzung nicht. Geräte fragen je Modellstunde an.

Manuelle Übersteuerung ermöglicht gezielte Vorführungen und ist ein ausdrücklich gekennzeichneter Eingriff in das deterministische Modell. Fehlende oder ungültige Antworten müssen im Gerät sichtbar werden. Vertrag, Übersteuerung und Fehlerbehandlung konkretisieren ADR-0005, 0007 und 0008. MQTT ergänzt später nur die Sichtbarkeit, nicht die Wetterversorgung der Geräte (0013).

<a id="adr-0002"></a>
## ADR-0002: Zuständigkeiten für Wetter und PV

**Beschluss:** angenommen am 2026-10-26; historische Zuordnung.

Alexander übernimmt Wettersimulator, PV sowie Master/SimCore; Dominik die Batterie, Susanne den Charger. Die damalige Zuordnung für Diagnose-Client, viertes Gerät und zentrale Dokumentation war noch unvollständig. Ziel war eine fachlich begründete, möglichst gleichmäßige Verteilung entsprechend LB01.

Die aktuelle vollständige Zuständigkeitstabelle steht im [zentralen README](../README.md). Die damaligen offenen Stellen sind keine erneute Öffnung bereits zugewiesener Aufgaben.

<a id="adr-0004"></a>
## ADR-0004: Arbeitsunterlagen und Veröffentlichung

**Beschluss:** angenommen am 2026-10-26; Ablageentscheidung am 2026-11-09 teilweise ersetzt.

Ursprünglich sollten ADRs unter `docs/adr/` und das Glossar unter `docs/glossary.md` ausschließlich lokale, ignorierte Arbeitsunterlagen bleiben. Auf ausdrücklichen Nutzerentscheid ist nun **diese kompakte Sammlung versionierbar**. Das vollständige Originalarchiv bleibt lokal unter `docs/adr/`; die Glossarregel wird nicht verändert. Die entsprechenden lokalen Ablagehinweise aus ADR-0013 sind für die neue Sammlung ebenfalls ersetzt.

Der historische Abgabeplan sah vor, dass Alexander das annotierte Tag `lb02-2026` selbst setzt, sobald die Abgabe fertig ist. Dies belegt nicht, dass das Tag gesetzt wurde. Für heutige Commits und Pushes gelten die [AGENTS.md-Regeln](../AGENTS.md); die Zusammenführung erteilt keine Commit-, Push- oder Tag-Freigabe.

<a id="adr-0005"></a>
## ADR-0005: HTTP-Vertrag des Wettersimulators

**Beschluss:** angenommen am 2026-10-26.

- Anfrage: `GET /weather?location=<Kennung>&simTime=<yyyy-MM-ddTHH:mm:ss>&run=<RunId>`.
- Erfolg: HTTP 200 mit JSON `WeatherData`, einschließlich `PrecipitationMmPerHour`; unbekannter Standort: 404; nicht interpretierbare Parameter: 400.
- Standardport: `8080`, über `--port` konfigurierbar. Geräte verwenden `--weather <URL>`, standardmäßig `http://127.0.0.1:8080`.
- Gleicher Standort, gleiche virtuelle Zeit und gleicher Lauf sollen im automatischen Modell byte-identische Antworten ergeben, auch nach einem Dienstneustart. Manuelle Eingriffe sind die ausdrücklich beschriebene Ausnahme.
- Zeitverarbeitung in UTC; lokale Anzeigen gemäß ADR-0012.

Die Laufkennung ist Teil der Anfrage, weil Reproduzierbarkeit ausdrücklich für denselben Simulationslauf verlangt wird. Aktuelle Vertragsdetails: [Wettersimulator](../src/WeatherSimulator/README.md).

<a id="adr-0006"></a>
## ADR-0006: Standortkonfiguration und Niederschlag

**Beschluss:** angenommen am 2026-10-26.

Jedes Gerät erhält `--location <Kennung>`, Standard `salzburg`. Der Wettersimulator ordnet jeder Kennung Koordinaten und Höhe zu; unbekannte Kennungen ergeben 404 und einen sichtbaren Gerätefehler. Wetter ist damit standortabhängig statt global.

`WeatherData.PrecipitationMmPerHour` ist ein `double` für Niederschlag in mm/h; `0` bedeutet trocken. Für den Reproduzierbarkeitsnachweis wurden zwei PV-Instanzen mit gleicher Standortkennung vorgesehen. Diese Prüfplanung allein belegt keinen erfolgreichen Lauf.

<a id="adr-0007"></a>
## ADR-0007: Manuelle Wetterübersteuerung

**Beschluss:** angenommen am 2026-10-26; am 2026-11-08 durch ADR-0013 präzisiert.

Bedeckung, Temperatur, Niederschlag und Wind lassen sich per Tastatur übersteuern. Der Toggle bleibt innerhalb eines Laufs bis zur Aufhebung aktiv; die Anzeige unterscheidet **MANUELL** und **AUTOMATISCH**. Aufheben kehrt zum unveränderten deterministischen Modellkern zurück. Reproduzierbarkeitsnachweise erfolgen im Automatikmodus.

ADR-0013 ersetzt die globale Reichweite durch gezielte Standortwahl. Neue RunId beziehungsweise neue Master-Sitzung setzen Vorgaben zurück; Pause und reine Verbindungsunterbrechungen erhalten sie. Damit gilt die ursprüngliche Aussage „bis manuell aufgehoben“ nicht mehr über Lauf-/Sitzungswechsel hinweg.

<a id="adr-0008"></a>
## ADR-0008: Geräteverhalten bei Wetterfehlern

**Beschluss:** angenommen am 2026-10-26.

Wetter wird je Modellstunde mit **5 s Timeout** angefragt. Nach **drei aufeinanderfolgenden Fehlversuchen** zeigt das Gerät „WETTER: NICHT ERREICHBAR“ beziehungsweise „WETTER: UNGÜLTIG (Grund)“, etwa bei 404 oder nicht interpretierbarer Antwort. Es rechnet mit dem letzten gültigen Wetter weiter, aber nicht ohne sichtbaren Fehlerstatus.

Wiederholungsversuche erfolgen alle **10 s**. Die erste gültige Antwort beendet den Fehlerzustand mit „WETTER: OK“ und liefert die neue Wetterbasis. Die Entscheidung verbindet fortlaufende Simulation mit sichtbarer Fehlerbehandlung. Aktuelle gemeinsame Anbindung: [Devices.Common](../src/Devices.Common/README.md).

<a id="adr-0009"></a>
## ADR-0009: WeatherSynth als Basis und eigene Ergänzungsgrößen

**Beschluss:** angenommen am 2026-10-26 durch Alexander; regionale Erweiterung durch ADR-0013.

WeatherSynth **0.3.0** wurde für .NET 10 gewählt, um ohne Internet zur Laufzeit reproduzierbare, gekoppelte Solar-/Windverläufe zu erzeugen. `GenerateYear(jahr, seed)` nutzt die RunId als Seed pro Kalenderjahr; `CoupledWeatherYear.Hours` beziehungsweise `HoursBetween(...)` liefern stündliche GHI (Wh/m²) und Windgeschwindigkeit (m/s). Der damalige Paketabgleich beschreibt 8.760 Stunden. `SolarSite(lat, lon, altitude)` liefert Standortgeometrie, keine vollständige regionale Klimakalibrierung.

Eigene deterministische Ergänzungen liefern Sonnenhöhe aus Standort/Datum/Stunde, Bedeckung aus dem Verhältnis zur möglichen Clear-Sky-Einstrahlung, Temperatur als Tagesgang um eine Tagesbasis, Niederschlag aus Bedeckung mit temperaturabhängiger Schneegrenze sowie einen vereinfachten Wetterzustand. Formeln und Grenzen gehören in das [Wetter-README](../src/WeatherSimulator/README.md).

Der ursprüngliche Quellenabgleich nennt WeatherSynth unter AGPL-3.0, SolarCalculator unter LGPL-3.0 und eingebettete DWD-Daten unter CC BY 4.0. Die Bibliothek wird unverändert per NuGet referenziert; Quellen und eigene Modellanteile sind zu dokumentieren. Das in der Aufgabe genannte Anpassungs-/Fork-Verbot bezog sich auf EasyModbusTCP.NETCore. Diese Sammlung führt keinen neuen Lizenzabgleich durch.

Reproduzierbarkeit ist eine Eigenschaft des automatischen Modells; vorhandene direkte Funktionsprüffälle sind ohne Laufnachweis keine bestandenen Tests.

<a id="adr-0010"></a>
## ADR-0010: Standortkatalog

**Beschluss:** angenommen am 2026-10-26; erweitert durch ADR-0013.

Eine interne Tabelle ordnet Kennungen Breite, Länge und Höhe zu. Standard ist `salzburg`; ursprünglich vorgesehen waren `salzburg`, `wien`, `innsbruck`. Weitere Orte sind ergänzbar, unbekannte Kennungen ergeben 404. ADR-0013 ergänzt `tamanrasset` und `neumayer`. Numerische Standortdaten stehen im [Wetter-README](../src/WeatherSimulator/README.md).

<a id="adr-0011"></a>
## ADR-0011: Hochlauf und PV-Anbindung

**Beschluss:** angenommen am 2026-10-26; historische Hochlaufplanung.

Der damalige Plan sah einen sauber endenden WindSimulator-Platzhalter mit „Noch nicht implementiert“ vor, um die Solution baubar zu halten. Das ist keine aktuelle Anweisung, einen WindSimulator wieder einzuführen: Der heutige Projektbaum enthält das Kühlhaus als viertes Feldgerät.

Die PV sollte an Wetter-HTTP, Master-Zeit/StepScheduler und Modbus angeschlossen werden; lokales automatisches Wetter entfällt. `--seed` bleibt für die Zufallsanteile des PV-Störmodells. Nach Wetter- und PV-Umbau sollte das zentrale README entstehen.

Historisch zugeordnet waren Master/SimCore, PV und Wetter an Alexander, Batterie an Dominik, Charger und Diagnose an Susanne. Für den Industrieverbraucher sollte Dominik zunächst dokumentieren; die Entwicklung war damals noch offen. Aktuelle Zuständigkeiten gelten gemäß [Root-README](../README.md).

Konsolenprotokolle waren lokal unter `docs/screenshots/` vorgesehen; der vollständige Zehn-Punkte-Ablauf sollte in der Team-Demo folgen. Diese Planung ist kein Demo-Nachweis und ersetzt nicht die heutigen datierten Nachweise unter `docs/nachweise/`.

<a id="adr-0012"></a>
## ADR-0012: UTC im Code und lokale Anzeige

**Beschluss:** angenommen am 2026-10-26 durch Alexander.

Virtuelle Zeiten werden durchgehend in **UTC** verarbeitet, um fehlende oder doppelte Stunden durch Sommerzeitwechsel zu vermeiden. Konsolenanzeigen sollen UTC und lokale Zeit für **Europe/Vienna** ausgeben. Die Zeitzonenregel gehört in Root- und Wetter-README. Die Entscheidung dokumentiert die Vorgabe; sie behauptet keinen neuen Nachweis jeder Anzeige.

<a id="adr-0013"></a>
## ADR-0013: Wetter über MQTT sichtbar machen und Klimaprofile erweitern

**Beschluss:** am 2026-11-08 ausdrücklich bestätigt, laut datiertem Nachweis umgesetzt und funktional geprüft.

### Zweck und Transport

Wetter erhält einen optionalen MQTT-Ausgang über RabbitMQ für den Viewer, eingeschaltet mit `--mqtt`. Geräte beziehen Wetter weiterhin über HTTP; ein Wechsel ihrer Versorgung auf RabbitMQ wurde verworfen. HTTP bleibt ohne MQTT und bei Broker-Ausfall nutzbar. Alexander verantwortet den Broker, Susanne das Modbus–RabbitMQ-Gateway.

Der Wetterstrom verwendet die gemeinsame Master-Zeitverteilung. Historische HTTP-Abfragen beeinflussen ihn nicht. Alle **eine reale Sekunde** wird der neueste Stand für Salzburg, Wien, Innsbruck, Tamanrasset und Neumayer III veröffentlicht. Das Modell bleibt stündlich aufgelöst; übersprungene Stunden werden nicht nachgeliefert. Ziel ist Sichtbarkeit, keine lückenlose Zeitreihenarchivierung.

### Nachricht und Gültigkeit

- Themen: `energytwin/weather/{location}/measurements` und dienstweit `energytwin/weather/status`; eigener Vertrag mit Schemaversion.
- **QoS 1 und Retain** für Messwerte und Status; keine periodische Statuswiederholung. Sechs konkrete Abos ermöglichen den späteren Einstieg, da Wildcard-Abos im geprüften RabbitMQ-Aufbau Retain-Werte nicht zuverlässig lieferten.
- Messwerte: Temperatur, Einstrahlung, Windgeschwindigkeit, Niederschlag, Bedeckung, Sonnenhöhe und Wetterzustand mit Einheiten, Standort und automatischer/manueller Herkunft.
- Zeit-/Identitätsfelder: Modellstunde, aktuelle Master-Modellzeit, RunId, Veröffentlichungszeit, Kennung und Sequenz. Die Modellstunde darf älter als die aktuelle Zeit sein. Doppelte QoS-1-Zustellungen müssen anhand der Nachrichtenkennung erkennbar sein.
- Dienststatus entscheidet über die Gültigkeit gespeicherter Werte. `noTime`, `masterLost` und `offline` erlauben keine Darstellung alter Werte als aktuell bestätigt. Ein fachlich einmaliger Zustandswechsel garantiert keine exakt einmalige MQTT-Zustellung.
- Zugangsdaten kommen wie beim Gateway aus Umgebung oder lokaler `docker/.env`, niemals als Passwort-Option oder in Ausgaben.

### Pause, Ausfälle und manuelle Eingriffe

| Ereignis | Beschlossenes Verhalten |
|---|---|
| Pause | Einmal `paused`, regelmäßige Wetterveröffentlichung stoppt; Vorgaben bleiben erhalten. |
| Manuelle Änderung in Pause | Nur betroffenen Ort einmal mit unveränderter Modellzeit und `manual` veröffentlichen; kein Fortsetzen. |
| Fortsetzen | Einmal `continued`, anschließend wieder regelmäßige Veröffentlichung aller Orte. |
| Noch keine gültige Master-Zeit | `noTime`, keine neuen Wetterwerte. |
| Master-Verlust | Einmal `masterLost`, keine neuen Wetterwerte; nach Synchronisation `continued` oder `paused` gemäß Master. |
| Broker-Ausfall und Wiederverbindung | HTTP bleibt nutzbar; automatisch verbinden, dann aktuellen Status und aktuelle Ortswerte ohne Rückstau senden; ohne gültige Zeit nur Status. |
| Neuer Lauf/neue Master-Sitzung | Manuelle Vorgaben zurücksetzen; alte gespeicherte Werte entfernen beziehungsweise ersetzen. Neuer Lauf hat Vorrang vor Wiederverbindung. |
| Start bei gültigem, pausiertem Master | Einmal Anfangsstände aller Orte bereitstellen, danach keine regelmäßigen Wetterwerte. |
| Dienstende | Geordnetes Ende oder Last Will melden `offline`. |

Übersteuerung gilt gezielt für den ausgewählten Standort und wird in Konsole und Nachricht kenntlich gemacht. Innerhalb eines Laufs wirkt sie bis zur Aufhebung; reine Pause und Verbindungsunterbrechung löschen sie nicht.

### Modellgrenzen und Akzeptanz

Neue Koordinaten reichen nicht für andere Klimazonen: Die bisherige Temperatur-/Niederschlagsheuristik ist mitteleuropäisch; WeatherSynth.Default basiert laut damaligem Paketabgleich auf Solarwerten aus Bochum und Windwerten aus Essen-Bredeney. Für Tamanrasset und Neumayer sind Temperatur, Niederschlag als Wasseräquivalent auch bei Schnee, Wind und Bewölkung regional zu behandeln. Polartag/-nacht, südhemisphärische Jahreszeiten und Schnee/Sturm bei Dunkelheit müssen berücksichtigt werden.

Die österreichische numerische Modellbasis bleibt ohne fachlichen Änderungsgrund erhalten. Neue Profile sollen mit überschaubarem Aufwand plausibel sein; Quellen, Annahmen und Näherungen sind kenntlich zu machen. Es handelt sich um synthetisches Wetter, keine Vorhersage, historische Rekonstruktion oder nachgewiesen wissenschaftliche Kalibrierung.

Akzeptanz umfasst fünf Orte auch unter `/locations`, vollständige Größen/Einheiten, reproduzierbare Automatikwerte bei gleichem Ort/Modellstunde/Lauf, dasselbe Modell für HTTP und MQTT sowie die Zustandswechsel der Tabelle. Klimaprüfungen betreffen Saisonverhalten, trockenes Wüstenklima und Polartag/-nacht. Der ursprüngliche Umsetzungsauftrag verlangte einen Build mit null Fehlern/Warnungen und aktualisierte Anleitung, Vertrag, SRS und Alexanders Journal; das ist keine Build-Pflicht für jede spätere Dokumentationspflege.

**Nachweisgrenze:** Der [Probelauf vom 2026-11-08](nachweise/2026-11-08-wetter-mqtt/README.md) dokumentiert HTTP/MQTT, Zustandswechsel und Ausfälle. Interaktive Bedienung und ein neuer vollständiger Unit-Testlauf waren dort offen. Diese Zusammenführung führt keine neuen Prüfungen aus. Aktuelle Details und Klimadatenquellen: [Wettersimulator](../src/WeatherSimulator/README.md) und [Wetter-Nachrichtenvertrag](Wetter-Nachrichtenvertrag-LB03.md).

<a id="adr-0014"></a>
## ADR-0014: GetStatus-Skill

**Status:** am 2026-11-08 verworfen, ersetzt durch [ADR-0015](#adr-0015).

GetStatus sollte Aufgaben anhand vorhandener Belege als `erfüllt`, `teilweise erfüllt`, `nicht erfüllt` oder `nicht nachgewiesen` bewerten und Pflichtanforderungen von optionalen Erweiterungen trennen. Der Ansatz überschneidet sich zu stark mit Susannes `readTask`; Dominiks `probelauf` deckt aktive Funktionsprüfungen ab. Daher wurde kein eigener GetStatus-Skill umgesetzt.

Offene Entwurfsfragen zu Quellenhierarchie, Ausgabeformat und Prüfumfang sind mit der Verwerfung keine ausstehenden Implementierungsaufträge. Code allein belegt weiterhin keinen erfolgreichen Lauf.

<a id="adr-0015"></a>
## ADR-0015: Let's Document

**Beschluss:** am 2026-11-08 bestätigt; Skill erstellt, formal geprüft und lokal installiert.

Let's Document (`lets-document`) pflegt belegbare README-Inhalte, verdichtet Wiederholungen und archiviert überholte oder verworfene Aussagen nachvollziehbar. Es ersetzt weder Aufgabenbewertung durch `readTask` noch Funktionsprüfung durch `probelauf` und verändert diese Skills nicht. Die maßgebliche versionierte Ablage ist [skills/lets-document](../skills/lets-document/SKILL.md); die lokale Codex-Installation stammt daraus.

- Einzelner README-Pfad begrenzt die Bearbeitung auf diese README. Folgeanpassungen anderswo werden gemeldet; Archive und nach Projektregel erforderliche Journale bleiben möglich. Modul-/Sitzungsaufträge schließen nötige Root-README-Anpassungen ein; ein vollständiger Durchgang braucht einen ausdrücklichen Auftrag.
- Belegbare Lücken direkt ergänzen; unbekannte Angaben offenlassen. Testcode ist kein grüner Lauf. Nachweise mit Datum und Aussagegrenze benennen.
- Fachliche Konflikte nicht automatisch zugunsten des Codes entscheiden. User-Fragen mit Beleg, Auswirkung und Empfehlung bündeln; abhängige Fragen erst anschließend stellen. Nur davon abhängige Änderungen zurückstellen, keine Bestätigung jeder Formulierung verlangen.
- Vor Entfernen oder Verdichten Originale vollständig mit Herkunft, Datum, Grund und Ziel sichern; vorhandene Archive nicht überschreiben oder duplizieren. Unklare Aussagen nicht ungeprüft für veraltet erklären.
- Formeln, Registerpläne, gültige Befehle, Bedienung und Einschränkungen erhalten. Root-README gibt Überblick, Modul-READMEs enthalten fachliche Details. Relative Links statisch prüfen.
- Keine Tests, Benchmarks, Probeläufe oder Programmstarts ohne konkrete Zustimmung. Ein Dokumentationsauftrag allein erteilt keine Ausführungsfreigabe. Allgemeine Wiederverwendbarkeit durch Beachtung der jeweiligen Projektregeln statt fest eingebauter Team-/Technikannahmen.
- Prüf-/Vorschau-Aufträge liefern Befunde; Bearbeitungsaufträge setzen eindeutige autorisierte Änderungen um. Abschluss nennt Dateien, Ergänzungen, Archivorte, Kürzungen, offene Entscheidungen und tatsächlich erfolgte Prüfungen.

Beispielaufrufe:

```text
$lets-document src/Devices/PvSimulator/README.md
$lets-document Aktualisiere die READMEs der aktuellen Änderungen
$lets-document Prüfe alle READMEs in diesem Repository
```

**Nachweisgrenze:** Der [Nachweis vom 2026-11-08](nachweise/2026-11-08-lets-document/README.md) beschreibt formale Prüfung, drei identische installierte Dateien, einen begrenzten PV-README-Aufruf und einen isolierten Dokumentationsdurchgang. Tests, Simulatoren und Dienste wurden dabei nicht gestartet; ein damals vorgeschriebener Build wurde separat ausgeführt. Das ist kein neuer Prüfbericht dieser Zusammenführung.
