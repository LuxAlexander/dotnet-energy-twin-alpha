# Entwicklungsjournal – Dominik Nachname

## 2026-10-05 – Anforderungen, Brainstorming u Spezifikation

- **Done:** Aufgabenstellung gelesen; Funktionsumfang des Batteriespeichers per Frage-Antwort-Brainstorming festgelegt (28 Entscheidungen); Spezifikation u Projekt-Memory geschrieben.
- **KI:** Claude (Anthropic, claude.ai) => Brainstorming zu Funktionen, Architektur, Auto-Modus, Störungen, CSV u Bedienung; Spezifikation u Entscheidungsprotokoll von der KI formuliert, von mir geprüft u bestätigt. Interface für Geräte (`ISimulatedDevice`) verworfen, erst bei Kopplung; Threads bewusst vermieden.
- **Artefact:** Spezifikation „Batteriespeicher-Simulator“, Projekt-Memory (Entscheidungsprotokoll).
- **Test:** Keine Codetests; 10 Abnahmetests in der Spezifikation definiert.
- **Comment:**
  - Batterie als eigenständiges Gerät, PV u Ladestation bauen die Kollegen
  - Auto-Modus = Tagesprofil mit ±10 % Rauschen statt eigenem PV-Modell
  - Soll/Ist getrennt, Gerät begrenzt selbst (wie BMS)
  - Störungen: Not-Aus u Überhitzung; Pflicht: Zähler, Taper; optional: Wirkungsgrad, Selbstentladung, SOH
  - Next: Projekt anlegen, Batterie-Klasse

## 2026-10-06 – Batterie-Modell

- **Done:** `Enum.cs`, `BatterieConfig`, `Batterie` (SOC, Begrenzung, Taper, Temperatur, Derating, Störungen, Energie- u Zykluszähler) u `DailyProfile` (Tagesprofil mit Rauschen) geschrieben.
- **KI:** Claude => Erklärung der Konzepte u Code-Review, Code selbst geschrieben. Gefundene Fehler: Ergebnis von `Math.Clamp` nicht gespeichert; kWh mit kW verglichen (Division durch Stunden fehlte); Zähler mit `=` statt `+=`; Temperaturformel als Gleichgewicht statt Änderung pro Schritt; Überhitzungs-Trip setzte `ActiveFault` in jedem Schritt neu (Reset wirkungslos); Klammerung der Exp-Funktion im Entladeprofil; Rauschen wechselte jede Minute (Fensternummer wurde nicht gemerkt).
- **Artefact:** `Enum.cs`, `BatterieConfig.cs`, `Batterie.cs`, `DailyProfile.cs`, Screenshots der Konsolentests.
- **Test:** Konsolenschleifen: Laden 50→100 % mit Herunterregeln im letzten Schritt; Entladen bis 10 %; Taper ab 80 %; Derating (HeatFactor kurz auf 0,6); Not-Aus u Reset; Überhitzung: Fault, Cooldown, Freigabe bei ca. 50 °C; Zähler (5 kWh geladen = 0,25 Zyklen bei 10 kWh); Profilkurve in 0,5-h-Schritten.
- **Comment:**
  - Natürliche Überhitzung erreicht wegen Derating nie 60 °C, Störung nur manuell
  - Not-Aus setzt Sollwert 0
  - Zusatzwärme über eigenes Flag (`faultHeatActive`) von der Anzeige-Störung getrennt
  - Standardkapazität von 10 auf 20 kWh, damit das Tagesprofil besser passt
  - Next: Simulator mit Realzeit-Takt

## 2026-10-07 – Simulator, Bedienung, CSV u Dashboard

- **Done:** `Simulator` (Realzeit-Takt, Pause, Auto/Manuell, Tasten, Pausenmenü), `CsvLogger`, `ConsoleDashboard`, Kommandozeilenargumente, README.
- **KI:** Claude => Anleitung u Code-Review; `Program.cs` (Argumente) u README von der KI erstellt, von mir geprüft. Gefundene Fehler: Zeitkonto beschleunigte sich selbst (`lastRealSeconds` falsch benutzt); Pause holte die Pausenzeit nach; Operator-Vorrang `&&`/`||` bei der Schleifenbedingung; `=` statt `==` bei der E-Taste; Beschriftungen in den CSV-Datenzeilen; `internal` statt `public`; `NullReference` weil Dashboard nicht übergeben; fester SOC-Balken.
- **Artefact:** `Simulator.cs`, `CsvLogger.cs`, `ConsoleDashboard.cs`, `Program.cs`, `README.md`, CSV-Beispieldatei, Screenshots des Dashboards.
- **Test:** Manuell: 24-h-Lauf (nachts 0 kW, mittags voll, abends Entladen); Pause 20 s ohne Zeitsprung; Tasten M, Pfeile, 0, E, N, H, R, Q; Pausenmenü: Max-Ladeleistung 2 => Ist < Soll, SOC-Max 80, ungültige Eingaben; CSV: 13 Spalten, Punkt als Dezimalzeichen. Argumente u Mehrfachstart noch zu testen.
- **Comment:**
  - Pausenmenü mit `ReadLine`, keine Threads; Menüzeit wird nicht nachgeholt
  - Dezimalkomma im Menü (Landeseinstellung), Punkt in Argumenten u CSV
  - Offen: Zielframework auf .NET 10 prüfen, Ereignisliste u Zusammenfassung optional
  - Next: Argumente u Mehrfachstart testen, Kopplung mit PV u Ladestation abstimmen, optional Wirkungsgrad
