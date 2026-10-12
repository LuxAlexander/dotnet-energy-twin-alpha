# Design-Notizen – Dual-Port-Charger (LB01)

Ergebnis einer Design-Fragerunde (KI-gestützt) vor der Implementierung.
Grundlage für README (Modellbeschreibung, Annahmen) und Entwicklungsjournal.

---

## 1. Gerät

- **DC-Schnelllader** mit **2 Ladepunkten** und einem **gemeinsamen Leistungsbudget** von standardmäßig **150 kW**.
- Lädt nur ein Punkt, darf er das ganze Budget nutzen. Laden beide, wird aufgeteilt (siehe 4.).
- Die Gesamtleistung ist der **zur Laufzeit änderbare Betriebsparameter** (z. B. 150 → 100 kW, Lastmanagement durch den Netzbetreiber). Erlaubt sind **50–350 kW**: beim Start über `--power` (sonst Fehlermeldung), zur Laufzeit über `+`/`-` (bleibt an der Grenze stehen).
- Die Säule kann ein- und ausgeschaltet werden.

## 2. Klassen

```
DualCharger
 ├─ ChargingPoint [1]  ── 0..1 Vehicle
 └─ ChargingPoint [2]  ── 0..1 Vehicle
```

| Klasse | Verantwortung |
|---|---|
| `Vehicle` | Modellname, `BatteryCapacityKWh`, `MaxChargePowerKW`, `SocPercent`, `TargetSocPercent`, Ladekurve `GetMaxPowerAtSoc()` |
| `VehicleCatalog` | Feste Liste realer Fahrzeugmodelle (`record VehicleModel`, ohne SOC), Erzeugen von Fahrzeugen manuell oder zufällig; `static`, `Random` wird übergeben |
| `ChargingPoint` | Zustand, verbundenes Fahrzeug, aktuelle Leistung, Temperatur, kumulierte Energie |
| `ChargingPointState` | `enum` der Zustände (siehe 5.) |
| `DualCharger` | Gesamtbudget, Ein/Aus, Budgetaufteilung, Warteschlange, Zähler abgewiesener Fahrzeuge, Gesamtenergie (Summe) |
| `SimulationConfig` | Alle Startparameter |
| `SimulationEngine` | Hauptschleife, Simulationszeit, Pause, Echtzeit-Warten |
| `AutoModeController` | Zufällige Ankünfte, SOC-Werte, Verweilzeiten |
| `ConsoleRenderer` | Statusausgabe ohne Flackern |
| `InputHandler` | Tastaturbefehle zur Laufzeit |
| `CsvLogger` | Schreibt eine Zeile pro Simulationsschritt |
| `ArgsParser` | Kommandozeile → `SimulationConfig` |
| `Program` | Nur Aufbau und Start (< 20 Zeilen) |

**SOC (State of Charge)** = Ladezustand der Batterie in %. Er liegt **im Fahrzeug**, weil die Batterie zum Auto gehört und beim Abstecken mitfährt.

## 3. Fahrzeuge und Ladekurve

### Katalog (gerundete Realwerte)

| Modell | Kapazität | Max. DC |
|---|---|---|
| Renault Zoe | 52 kWh | 50 kW |
| Fiat 500e | 42 kWh | 85 kW |
| VW ID.3 | 58 kWh | 120 kW |
| Tesla Model 3 | 60 kWh | 170 kW |
| Hyundai Ioniq 5 | 77 kWh | 220 kW |
| Mercedes eActros 600 (LKW) | 621 kWh | 400 kW |
| Kia EV6 (defekt) | 77 kWh | 240 kW |

Fahrzeuge mit über 150 kW zeigen, dass die Säule begrenzt. Der LKW kann mehr aufnehmen als die ganze Säule liefert und belegt einen Punkt mehrere Stunden. Der defekte Kia EV6 (`HasDamagedInlet`) überhitzt bei hoher Leistung (siehe 7.).

Der Katalog enthält nur **Modelldaten**. Der SOC gehört zum einzelnen `Vehicle`, sonst würden sich zwei Fahrzeuge desselben Modells einen SOC teilen.

### Ladekurve (pro Fahrzeug)

```
SOC ≤ 80 %:  P_kurve = MaxChargePowerKW
SOC > 80 %:  P_kurve = MaxChargePowerKW · e^(−k · (SOC − 80)),   k = ln(10)/20 ≈ 0,115
```

→ Bei 100 % sind es noch 10 % der Maximalleistung. Die Exponentialfunktion erreicht nie 0, deshalb gilt als Abbruch: **SOC ≥ Ziel-SOC → `Finished`**.

### Leistung eines Ladepunkts

```
Bedarf = Min(P_kurve(SOC), Limit durch Temperatur-Derating)
```

## 4. Budgetaufteilung (in `DualCharger`)

Ein starres 50/50 ist **falsch**, weil Leistung verfällt, wenn ein Fahrzeug weniger aufnehmen kann.

```
wenn BedarfA + BedarfB ≤ Budget:  beide bekommen ihren Bedarf
sonst:
    a = Min(BedarfA, Budget / 2)
    b = Min(BedarfB, Budget − a)
    a = Min(BedarfA, Budget − b)      // Rest an A zurückgeben
```

Beispiel bei 150 kW: Bedarf 15 + 150 → Zuteilung **15 + 135** (nicht 75 + 75).

### Reihenfolge pro Tick

1. Bedarf beider Punkte ermitteln
2. Budget aufteilen
3. Energie, SOC und Temperatur aktualisieren

Niemals einen Punkt komplett abarbeiten, bevor der andere betrachtet wird.

## 5. Zustände eines Ladepunkts

`enum` + Guard-Methoden. Das State-Pattern wäre für diese Größe Overkill.

```
Available ──Connect()──► Connected ──Start()──► Charging
    ▲                        ▲                     │
    │                        └─────Stop()──────────┤
    │                                              ▼ SOC ≥ Ziel
    └────────────Disconnect()────────────────── Finished
Faulted: aus jedem Zustand (manuell F oder automatisch ab 80 °C)
         Reset manuell (R) unter 60 °C, automatisch unter 50 °C
         → Charging (wenn vorher geladen wurde), Connected, Finished (Fahrzeug voll) bzw. Available
```

- **`Finished`**: Das Fahrzeug ist voll, steckt aber noch. Der Punkt ist **blockiert**.
- **`Faulted`**: Störung. Die Leistung ist 0, das Fahrzeug **bleibt verbunden**: Das Kabel bleibt verriegelt, `Disconnect()` ist gesperrt. Der SOC bleibt stehen. Wurde vor der Störung geladen, lädt der Punkt nach dem Reset automatisch weiter.
- **Zwei Reset-Wege** (siehe 7.): Ein Techniker kann mit `R` quittieren, sobald die Temperatur unter 60 °C liegt. Unter 50 °C hebt sich die Störung am Ende des Schritts von selbst auf. Hätten beide dieselbe Schwelle, würde `R` nie etwas bewirken, weil der automatische Reset immer zuerst greift.
- Guards: Die Methoden geben `false` zurück, wenn der Übergang nicht erlaubt ist (z. B. `Connect()` nur in `Available`). Die Meldungszeile zeigt dann den Grund.

## 6. Energie, SOC und Wirkungsgrad

Δt immer in **Stunden** (15 min = 0,25 h). Wirkungsgrad **η = 0,95**.

```
E_netz  [kWh] = P [kW] · Δt [h]         → wird als Energie gezählt (Säule/Ladepunkt)
E_akku  [kWh] = E_netz · η              → landet in der Batterie
ΔSOC    [%]   = E_akku / Kapazität · 100
```

- **Überlauf:** Wenn ΔSOC über den Ziel-SOC hinausgeht, wird E_akku auf `(Ziel − SOC) · Kapazität / 100` begrenzt und E_netz entsprechend auf `E_akku / η`.
- **Genauigkeit:** Die Leistung wird am Schrittanfang berechnet und gilt für den ganzen Schritt. Große Schrittweiten sind über 80 % SOC deshalb ungenau. Das ist der fachliche Grund für die einstellbare Schrittweite (Detailgrad).
- Energie wird **pro Ladepunkt** kumuliert, die Gerätesumme ist eine Property von `DualCharger`.

## 7. Temperaturmodell (pro Ladepunkt = Kabel/Stecker)

Plausible Annahmen, keine Normwerte.

| Parameter | Wert |
|---|---|
| Umgebungstemperatur T_umg | 20 °C |
| Beharrungstemperatur bei voller Leistung (150 kW) | 55 °C |
| Zeitkonstante τ | 10 min |
| Zeitkonstante τ bei Kühlungsdefekt | 30 min |
| Derating ab | 60 °C (linear 100 % → 0 % bis 80 °C) |
| Fault ab | 80 °C |
| Manueller Reset (`R`) unter | 60 °C (kein Derating mehr nötig) |
| Automatischer Reset unter | 50 °C (Hysterese) |

Die Erwärmung ist ∝ P², weil die Kabelverluste ∝ I² sind:

```
T_ziel = T_umg + 35 °C · (P / 150 kW)²
T_neu  = T_ziel + (T_alt − T_ziel) · e^(−Δt / τ)
```

**Warum die e-Funktion statt `T += …·Δt` (Euler)?** Bei Δt = 15 min und τ = 10 min schießt Euler über und schwingt. Die exakte Lösung ist für **jede** Schrittweite stabil.

- Im Normalbetrieb (Limit 150 kW) bleibt die Temperatur ≤ 55 °C, Derating und Fault treten also nicht auf.
- Auch bei höherem Limit erreicht ein normales Fahrzeug praktisch nie 80 °C: Das Derating senkt die Leistung linear auf 0 % bei 80 °C, die Temperatur nähert sich 80 °C also nur an.
- **Automatische Überhitzung – Kia EV6 (defekt):** Der Temperatursensor der Ladebuchse ist ausgefallen, deshalb gibt es **kein Derating** (`DeratingFactor` = 1). Zusätzlich verdoppelt der erhöhte Übergangswiderstand die Erwärmung (`DamagedInletHeatFactor` = 2, `T_ziel = 20 °C + 70 °C · (P / 150 kW)²`). Ab ca. 140 kW liegt T_ziel über 80 °C, nur die Abschaltung der Säule bei 80 °C schützt. Verlauf bei 150 kW allein am Punkt (5-min-Schritte):

```
 5 min: 47,5 °C  Charging
10 min: 64,2 °C  Charging   ← kein Derating
15 min: 74,4 °C  Charging
20 min: 80,5 °C  Faulted
30 min: 42,3 °C  Charging   ← automatisch zurückgesetzt, heizt wieder auf
```
- **Manuelle Störung** (`F`): Kühlungsdefekt. Der Punkt wird sofort `Faulted` und T auf 85 °C gesetzt. Weil die Kühlung defekt ist, kühlt er langsamer ab (τ = 30 min statt 10 min). So ist die Störung in Anzeige und CSV über mehrere Schritte sichtbar. Nach dem Reset gilt wieder τ = 10 min.

Verlauf nach `F` bei 5-min-Schritten:

```
 0 min: 85,0 °C  Faulted
 5 min: 75,0 °C  Faulted
10 min: 66,6 °C  Faulted
15 min: 59,4 °C  Faulted   ← manueller Reset möglich
20 min: 53,4 °C  Faulted
25 min: 48,2 °C  Charging  ← automatisch zurückgesetzt
```

- Ein zufälliger Kühlungsdefekt im Auto-Modus (optional) ist nicht umgesetzt.

## 8. Auto-Modus

### Ankünfte: Rate pro Stunde, abhängig von der Tageszeit

| Uhrzeit | Ankünfte/h |
|---|---|
| 00–06 | 0,2 |
| 06–09 | 1,5 |
| 09–16 | 1,0 |
| 16–20 | 2,5 |
| 20–24 | 0,8 |

```
p = 1 − e^(−Rate · Δt[h])      // unabhängig von der Schrittweite!
if (random.NextDouble() < p) → Fahrzeug kommt
```

Eine feste Wahrscheinlichkeit *pro Schritt* wäre falsch: Bei 1-Minuten-Schritten kämen 15× mehr Autos als bei 15-Minuten-Schritten.

### Fahrzeug bei Ankunft

- Modell zufällig aus `VehicleCatalog`
- Ankunfts-SOC zufällig **10–50 %**, Ziel-SOC zufällig **80–100 %**

### Kein Punkt frei → Warteschlange

- Das Fahrzeug wartet **max. 5 Minuten** (FIFO). Wird ein Punkt frei, steckt es an und startet sofort.
- Pro Schritt wird zuerst ein freier Punkt gesucht, erst danach werden Fahrzeuge mit Wartezeit ≥ 5 min abgewiesen.
- Danach fährt es weg, und der Zähler **`RejectedVehicles`** wird erhöht (Statusausgabe + CSV).
- Hinweis fürs README: Bei Schrittweite ≥ 5 min wird die Wartezeit nur grob, also schrittweise, ausgewertet.

### Abfahrt nach `Finished`

- 80 %: nach 0–5 min
- 20 %: nach 10–40 min (blockiert den Punkt)

### Manueller Modus: Fahrzeug auf Tastendruck

- Mit `N` kommt ein zufälliges Fahrzeug, steckt am ersten freien Punkt an und startet sofort (`DualCharger.Enqueue` verbindet direkt, nicht erst im nächsten Schritt).
- Es fährt nicht von selbst ab, abgesteckt wird mit `D`. Im Auto-Modus ist `N` gesperrt, dort kommen die Fahrzeuge von selbst.

### Zufall

- **Eine** `Random`-Instanz für das ganze Programm, an die Klassen weitergegeben.
- Standard ohne Seed (jeder Lauf anders). Optional `--seed N` für reproduzierbare Läufe (Debugging, Abgabe-CSV, Screenshots). Ein Seed macht die Simulation nicht weniger zufällig, nur wiederholbar.

## 9. Zeit-Engine

- **Schrittweite** = Rechenintervall Δt = eine CSV-Zeile.
- **Tempo** = Beschleunigung gegenüber der Echtzeit.

```
Echtzeit-Wartezeit [s] = Schrittweite [s] / Tempo
15 min, 60x  → 15 s pro Tick → 24 h in 24 min
15 min, 600x → 1,5 s pro Tick → 24 h in 2,4 min
```

- Das Tempo ist zur Laufzeit mit `<` / `>` halbierbar / verdoppelbar (1–100 000x). Die Wartezeit wird während des Wartens laufend neu berechnet, die Änderung greift also sofort.
- **Simulationsdauer** ist einstellbar, Standard 24 h. Bei < 24 h gibt es eine **Warnung**, der Wert ist aber erlaubt. Für die Abgabe-CSV einen Lauf ≥ 24 h verwenden (Mindestanforderung).
- Warten in kleinen Häppchen (z. B. 50 ms) mit `Console.KeyAvailable`-Abfrage, **nicht** ein einziges `Thread.Sleep(15 s)`, sonst reagiert die Eingabe bis zu 15 s verzögert.

```csharp
while (simTime < endTime)
{
    if (!paused) { charger.Tick(dt); logger.Write(...); renderer.Draw(...); }

    var waitUntil = DateTime.Now + realDelay;
    while (DateTime.Now < waitUntil)
    {
        if (Console.KeyAvailable) input.Handle(Console.ReadKey(true));
        Thread.Sleep(50);
    }
}
```

## 10. Bedienung

### Startparameter (Kommandozeile)

```
dotnet run -- --id DC-01 --power 150 --step 5 --speed 120 --duration 24 [--seed 42] [--output ordner]
```

| Argument | Bedeutung | Standard |
|---|---|---|
| `--id` | Geräte-ID | `DC-01` |
| `--power` | Gesamtleistung kW (50–350, sonst Fehlermeldung) | 150 |
| `--step` | Schrittweite min | 5 |
| `--speed` | Tempo-Faktor | 120 |
| `--duration` | Simulationsdauer h | 24 (Warnung bei < 24) |
| `--seed` | Zufalls-Seed | keiner |
| `--output` | Zielordner der CSV (relativ zum Arbeitsverzeichnis oder absolut, wird angelegt) | `output` |

Bei ungültigen Werten (≤ 0, unbekanntes Argument) gibt es eine klare Fehlermeldung.

### Laufzeit-Befehle (Tastatur)

| Taste | Befehl |
|---|---|
| `P` | Pause an/aus (Simulationszeit steht, keine CSV-Zeilen; alle anderen Tasten funktionieren weiter) |
| `A` | Auto-Modus an/aus (unabhängig von Pause) |
| `N` | Manueller Modus: zufälliges Fahrzeug kommt, steckt an und lädt sofort; abstecken mit `D` |
| `1` / `2` | Ladepunkt wählen (markiert mit `>`) |
| `C` | Fahrzeug anstecken: Modell, Start-SOC [20 %] und Ziel-SOC [100 %] per `ReadLine` wählen; die Simulation steht während der Eingabe. Danach mit `S` starten |
| `D` | Fahrzeug abstecken (nicht bei `Faulted`) |
| `S` | Laden starten/stoppen |
| `F` / `R` | Störung auslösen / zurücksetzen (`R` unter 60 °C, unter 50 °C automatisch) |
| `+` / `-` | Gesamtleistung ±10 kW (Grenzen 50–350 kW) |
| `<` / `>` | Tempo halbieren / verdoppeln (1–100 000x) |
| `O` | Säule ein/aus |
| `Q` | Beenden |

Die maximale Ladeleistung eines Fahrzeugs ist **nicht** änderbar, sie gehört zum Modell. Der SOC wird **beim Anstecken** festgelegt, nicht während des Ladens.

## 11. Konsolenausgabe

Ohne Flackern: `Console.CursorVisible = false`, `Console.SetCursorPosition(0, 0)` und jede Zeile mit `PadRight(...)` überschreiben, statt `Console.Clear()` zu verwenden.

Mindestinhalt laut Angabe, beide Ladepunkte getrennt:

```
=== Dual-Charger DC-01 | AUTO ===
Simulationszeit: 2026-07-15 17:30   Schritt: 5 min   Tempo: 120x
Säule: EIN   Limit: 150 kW   Leistung: 142,0 kW   Energie: 1.284,3 kWh   Abgewiesen: 3

[Punkt 1] CHARGING   VW ID.3          SOC  62 % [██████░░░░]   120,0 kW   41 °C   612,4 kWh
[Punkt 2] CHARGING   Renault Zoe      SOC  84 % [████████░░]    22,0 kW   27 °C   671,9 kWh
Warteschlange: 0
```

## 12. CSV

- **Dateiname:** `<output>/<ID>_<yyyy-MM-dd_HHmmss>.csv` → eindeutig, nichts wird überschrieben. Ohne `--output` landet die Datei in `output/` **im Arbeitsverzeichnis**: bei `dotnet run` im Projektordner, beim Start aus der IDE dort, wo `launchSettings.json` (`workingDirectory`) hinzeigt. Mit `--output` ist der Ort unabhängig vom Start, z. B. für mehrere Instanzen mit verschiedenen IDs.
- **Trenner:** Semikolon `;`, Zahlen mit **explizit** `CultureInfo("de-AT")` (Dezimalkomma). Das deutsche Excel öffnet die Datei damit direkt richtig, und das Ergebnis hängt nicht von den Systemeinstellungen des Rechners ab. Eine Datei mit `,` als Trenner und Dezimalkomma wäre kaputt.
- **Spalten:**

```
Zeit;Modus;Saeule_Ein;Gesamtlimit_kW;Gesamtleistung_kW;Energie_kWh;Abgewiesen;Warteschlange;
P1_Zustand;P1_Fahrzeug;P1_SOC;P1_Leistung_kW;P1_Temp_C;P1_Energie_kWh;
P2_Zustand;P2_Fahrzeug;P2_SOC;P2_Leistung_kW;P2_Temp_C;P2_Energie_kWh
```

- Eine Zeile pro Schritt (Zeit = Ende des Schritts) plus eine Startzeile. Bei `--step 5` also alle 5 Minuten Simulationszeit.
- Keine Spalte `Pausiert`: Während der Pause entstehen keine Zeilen, die Spalte wäre immer 0.
- `Px_Leistung_kW` ist die mittlere Leistung im Schritt (E_netz / Δt). Leistung · Δt passt dadurch immer zur gezählten Energie, auch im Schritt, in dem das Ziel erreicht wird.

Parameteränderung (`Gesamtlimit_kW`), Störung (`Px_Zustand = Faulted`, Leistung 0, Temperatur) und Modus sind damit im Verlauf erkennbar. Das ist eine Pflichtanforderung.

## 13. Projektstruktur

```
Dual_Charger_Simulation.sln
src/DualChargerSim/
  Models/       Vehicle, VehicleCatalog, ChargingPoint, ChargingPointState, DualCharger
  Simulation/   SimulationConfig, SimulationEngine, AutoModeController
  IO/           ConsoleRenderer, InputHandler, CsvLogger, ArgsParser
  Program.cs
tests/DualChargerSim.Tests/     (xUnit, optional)
data/                           ausgewählte CSV für die Abgabe (committet)
docs/screenshots/               Konsolen-Screenshots
docs/development-journal/<vorname-nachname>.md
README.md
```

### Empfohlene Unit-Tests

1. Budgetaufteilung: Bedarf 15 + 150 bei 150 kW → 15 + 135
2. SOC-Formel: 50 kWh Akku, 100 kW, 15 min, η = 1 → +50 %
3. Guard: `Connect()` auf `Faulted` schlägt fehl
4. Temperatur: Bei 150 kW stellen sich nach langer Zeit ≈ 55 °C ein

## 14. Git und Abgabe

- `.gitignore` per `dotnet new gitignore` erzeugen und zusätzlich `.idea/` und `output/` eintragen. Die `.sln` **wird committet**.
- Eine aussagekräftige CSV (Lauf ≥ 24 h, mit Fault und Leistungsänderung) bewusst nach `data/` kopieren.
- Abgabe-Tag muss **annotiert** sein:

```
git tag -a lb01-2026 -m "Abgabe LB01"
git push origin lb01-2026          # Tags werden nicht automatisch gepusht
git rev-list -n 1 lb01-2026        # vollständiger COMMIT-Hash für Moodle
```

  Achtung: `git rev-parse lb01-2026` liefert bei annotierten Tags den Hash des Tag-Objekts, nicht den des Commits.
- Repo-Name `dotnet-energy-sim-lb01`, Leserechte für die Lehrperson.
- Pflicht im Repo: Quellcode, README (Voraussetzungen, Start, Bedienung, Modell, Beispielparameter), Journal, CSV/Kurve, mehrere Screenshots.
