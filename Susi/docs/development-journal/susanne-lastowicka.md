# Entwicklungsjournal – Susanne Lastowicka
## 03.10.2026 – Anforderungsliste und Eingrenzung der Aufgabenstellung
- **Done:** Mit `claude` über die Anforderungen der Angabe diskutiert und mit `grill me` unbedachte Aspekte aufgedeckt. Das wurde in `design-notes.md` festgehalten. Leere Klassen erstellt.
- **KI:** Claude Opus 5.5 => `grill me` und Erstellung der leeren Klassen, die durch das diskutieren ermittelt wurden.
- **Artefact:** `docs/design-notes.md` und alles unter `src/`
- **Comment:** Einige Vorschläge der KI nach Recherche eingegrenzt oder verworfen. Z.B. Andere Störungen, AC statt DC.

## 04.10.2026 – Erstellung aller Klassen und Konsolenausgabe
- **Done:** Alle Klassen unter `src/Models` grob selbst auf Basis der `design-notes.md` erstellt, KI hat dann korrigiert. Alle anderen Klassen mit KI erstellt. Erste Tests.
- **KI:** Claude Opus 5.5 => Erstellung und Korrektur der Klassen vom Vortag. Es wurden nach dem ersten Test der Konsolenanwendung Verbesserungen und Logikfehler korrigiert.
- **Artefact:** alles unter `src/` und .csv unter `data`
- **Test:** Manueller 24-Stunden-Lauf; zu "Stoßzeiten" kommen mehr Fahrzeuge an, Fault löst aus.
- **Comment:** Der ausgelöste Fault wurde nicht angezeigt (Temperatur blieb gleich), die kW des Chargers konnten in unrealistische Bereiche angehoben werden. Wurde gefixt.

## 05.10.2026 – Dokumentation mit Screenshots
- **Done:** Screenshots der Konsolenanwendung unter `docs/screenshots` erstellt
- **KI:** Keine
- **Artefact:** alles unter `docs/screenshots`

## 06.10.2026 – Fahrzeug auf Tastendruck, Überhitzung, Tempo zur Laufzeit
- **Done:** Im manuellen Modus kommt mit `N` ein zufälliges Fahrzeug, steckt sofort an und lädt; abgesteckt wird manuell mit `D`. Der Auto-Modus bleibt wie bisher (zufällige Ankünfte, automatische Abfahrt). Neues Fahrzeug „Kia EV6 (defekt)“ mit beschädigter Ladebuchse, das bei hoher Leistung überhitzt. Tempo während der Simulation mit `<` / `>` halbieren / verdoppeln. Absturz beim Beenden mit `Q` behoben. README und `design-notes.md` angepasst.
- **KI:** Claude Opus 5.5 => Umsetzung der Änderungen, Analyse warum die automatische Überhitzung nie auftrat, Fehlersuche beim Absturz mit `Q`, Tests mit kleinen Hilfsprogrammen.
- **Artefact:** `src/DualChargerSim` (`ChargingPoint`, `DualCharger`, `Vehicle`, `VehicleCatalog`, `SimulationConfig`, `SimulationEngine`, `ConsoleRenderer`, `InputHandler`), `README.md`, `docs/design-notes.md`
- **Test:** Defekter Kia EV6 bei 150 kW allein am Punkt: nach 20 min `FAULTED` (80,5 °C), unter 50 °C automatischer Reset, lädt weiter; bei 100 kW bleibt er unter 51 °C. Auto-Modus 24 h: 20 Fahrzeuge angesteckt, 19 automatisch abgesteckt. Manueller Modus: Fahrzeug mit `N` bleibt nach 24 h am Punkt (`Finished`).
- **Comment:** Die Überhitzung trat nie auf, weil das Derating die Leistung bei genau 80 °C auf 0 senkt und die Temperatur sich 80 °C nur annähert. Lösung: Beim defekten Fahrzeug fällt der Temperatursensor aus (kein Derating) und der Übergangswiderstand verdoppelt die Erwärmung, nur die Abschaltung bei 80 °C schützt. Die KI hatte meine Anforderung zuerst falsch verstanden und `N` in den Auto-Modus eingebaut (Auto-Modus ohne zufällige Ankünfte), wurde danach korrigiert. Der Absturz bei `Q` kam von einem zu kleinen Konsolenfenster: Der Cursor wurde unter den Konsolenpuffer gesetzt.
