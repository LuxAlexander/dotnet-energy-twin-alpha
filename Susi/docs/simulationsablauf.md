# Simulationsablauf – ein Fahrzeug vom Start bis zur Abfahrt

Dieses Dokument verfolgt **einen kompletten Durchlauf** der Simulation für **ein einzelnes Fahrzeug**:
vom Programmstart über Ankunft, Laden und Abfahrt bis zum Programmende.
Bei jedem Schritt steht, **welche Funktion welche andere aufruft**, **welches Ereignis** das auslöst
und **in welcher Datei** (mit Zeilennummer) der Code liegt.

Alle Pfade sind relativ zu `src/DualChargerSim/`.

---

## 0. Beispiel-Szenario

Damit die Zahlen konkret sind, gehen wir von folgendem Lauf aus:

| Parameter | Wert | Herkunft |
|---|---|---|
| Startbefehl | `dotnet run` (ohne Argumente) | – |
| Gesamtleistung | 150 kW | `SimulationConfig.PowerLimitKW` |
| Schrittweite Δt | 5 min | `SimulationConfig.StepMinutes` |
| Tempo | 120× → 2,5 s Echtzeit pro Schritt | `SimulationConfig.Speed` |
| Startzeit | heute 00:00 | `SimulationConfig.StartTime` |
| Modus | **Auto** | `SimulationEngine.IsAutoMode = true` |
| Fahrzeug | **VW ID.3**, 58 kWh, max. 120 kW, Start-SOC 30 %, Ziel-SOC 90 % | `VehicleCatalog.CreateRandom` |

Das Fahrzeug kommt im Auto-Modus zufällig an; wir tun so, als ob genau dieses eine Fahrzeug
kommt und sonst keines.

---

## 1. Überblick: Wer ruft wen auf?

```
Program.Main                                   Program.cs:8
 ├─ ArgsParser.IsHelpRequested                 IO/ArgsParser.cs:27
 ├─ ArgsParser.Parse  → SimulationConfig       IO/ArgsParser.cs:31
 ├─ new SimulationEngine(config)               Simulation/SimulationEngine.cs:43
 │   ├─ new DualCharger(id, power)             Models/DualCharger.cs:34
 │   │   └─ new ChargingPoint(1), (2)          Models/ChargingPoint.cs:45
 │   ├─ new AutoModeController(charger, rnd)   Simulation/AutoModeController.cs:14
 │   ├─ new ConsoleRenderer()                  IO/ConsoleRenderer.cs
 │   └─ new InputHandler(engine, renderer)     IO/InputHandler.cs:18
 └─ SimulationEngine.Run                       Simulation/SimulationEngine.cs:75
     ├─ new CsvLogger(...)                     IO/CsvLogger.cs:24
     ├─ ConsoleRenderer.Initialize             IO/ConsoleRenderer.cs:24
     ├─ CsvLogger.Write   (Startzeile)         IO/CsvLogger.cs:39
     ├─ ConsoleRenderer.Draw                   IO/ConsoleRenderer.cs:43
     │
     ├─ while (IsRunning && !IsFinished)       ◄── HAUPTSCHLEIFE, 1 Durchlauf = 1 Simulationsschritt
     │   ├─ SimulationEngine.Step              Simulation/SimulationEngine.cs:109
     │   │   ├─ AutoModeController.Step        Simulation/AutoModeController.cs:37
     │   │   │   ├─ HandleDepartures           Simulation/AutoModeController.cs:46
     │   │   │   └─ (Zufall) DualCharger.Enqueue  Models/DualCharger.cs:52
     │   │   ├─ DualCharger.Tick               Models/DualCharger.cs:62
     │   │   │   ├─ ProcessQueue               Models/DualCharger.cs:89
     │   │   │   ├─ ChargingPoint.GetDemandKW  Models/ChargingPoint.cs:129   (×2)
     │   │   │   ├─ DualCharger.AllocatePower  Models/DualCharger.cs:78
     │   │   │   └─ ChargingPoint.Update       Models/ChargingPoint.cs:141   (×2)
     │   │   └─ SimulationTime += Δt
     │   ├─ CsvLogger.Write                    IO/CsvLogger.cs:39
     │   ├─ ConsoleRenderer.Draw               IO/ConsoleRenderer.cs:43
     │   └─ WaitAndHandleInput                 Simulation/SimulationEngine.cs:123
     │       └─ (Taste?) InputHandler.Handle   IO/InputHandler.cs:24
     │
     ├─ WaitForQuit                            Simulation/SimulationEngine.cs:151
     ├─ ConsoleRenderer.Shutdown               IO/ConsoleRenderer.cs:34
     └─ CsvLogger.Dispose  (durch using)       IO/CsvLogger.cs:66
```

### Zustände eines Ladepunkts (`Models/ChargingPointState.cs`)

```
             Connect()            Start()             Update(): Vehicle.IsFull
 Available ───────────► Connected ────────► Charging ───────────────────────► Finished
     ▲                                                                            │
     └─────────────────────────── Disconnect() ◄──────────────────────────────────┘
                     (Auto-Modus: AutoModeController.HandleDepartures)
```

Für unser eine Fahrzeug werden genau diese vier Übergänge durchlaufen. `Faulted` kommt nur
bei Überhitzung oder Taste `F` vor (siehe Abschnitt 9).

---

## 2. Phase 1 – Programmstart und Aufbau

### 2.1 `Program.Main` — `Program.cs:8`

Einstiegspunkt. Macht nur drei Dinge:

1. `ArgsParser.IsHelpRequested(args)` (`IO/ArgsParser.cs:27`) – bei `--help` Hilfetext ausgeben und mit `0` beenden.
2. `ArgsParser.Parse(args)` (`IO/ArgsParser.cs:31`) – liest `--id`, `--power`, `--step`, `--speed`,
   `--duration`, `--seed`, `--output` in eine **`SimulationConfig`** (`Simulation/SimulationConfig.cs`).
   Ohne Argumente bleiben die Standardwerte. Ungültige Werte → `ArgumentException`, die in `Main`
   gefangen und als Fehlermeldung ausgegeben wird (Rückgabe `1`).
3. `new SimulationEngine(config).Run()` – startet die eigentliche Simulation.

### 2.2 `SimulationEngine`-Konstruktor — `Simulation/SimulationEngine.cs:43`

Baut alle Objekte auf:

| Zeile | Aufruf | Ergebnis |
|---|---|---|
| 46 | `new Random(seed)` / `new Random()` | gemeinsamer Zufallsgenerator (mit `--seed` wiederholbar) |
| 48 | `new DualCharger(id, power)` → `Models/DualCharger.cs:34` | prüft 50–350 kW, legt **zwei** `ChargingPoint`s an (`Models/ChargingPoint.cs:45`), beide `Available`, 20 °C |
| 49 | `new AutoModeController(Charger, _random)` → `Simulation/AutoModeController.cs:14` | merkt sich Säule und Zufall |
| 50 | `new ConsoleRenderer()` | Konsolenausgabe |
| 51 | `new InputHandler(this, _renderer)` → `IO/InputHandler.cs:18` | Tastaturverarbeitung, kennt die Engine |
| 53–55 | `_speed`, `SimulationTime`, `_endTime` setzen | Uhr steht auf 00:00, Ende = 00:00 + 24 h |

### 2.3 `SimulationEngine.Run` – Vorbereitung — `Simulation/SimulationEngine.cs:75`

| Zeile | Aufruf | Was passiert |
|---|---|---|
| 77 | `new CsvLogger(deviceId, outputDir)` → `IO/CsvLogger.cs:24` | Ordner anlegen, Datei `DC-01_<Datum>_<Uhrzeit>.csv` öffnen, Kopfzeile schreiben |
| 79–80 | `Config.IsShortRun` | bei Dauer < 24 h Warnmeldung setzen |
| 82 | `_renderer.Initialize()` → `IO/ConsoleRenderer.cs:24` | UTF-8, Cursor aus, Bildschirm leeren |
| 83 | `logger.Write(...)` → `IO/CsvLogger.cs:39` | **erste CSV-Zeile** mit dem Ausgangszustand (00:00, beide Punkte `Available`) |
| 84 | `_renderer.Draw(this)` → `IO/ConsoleRenderer.cs:43` | erste Statusanzeige |

Danach beginnt die **Hauptschleife** (Zeile 86).

---

## 3. Die Hauptschleife – was in jedem Schritt passiert

`Simulation/SimulationEngine.cs:86–96`

```csharp
while (IsRunning && !IsFinished)
{
    if (!IsPaused)
    {
        Step();                                   // Modell um Δt weiterrechnen
        logger.Write(SimulationTime, ...);        // eine CSV-Zeile
    }
    _renderer.Draw(this);                         // Anzeige aktualisieren
    WaitAndHandleInput();                         // Echtzeit warten + Tasten abfragen
}
```

Ein Schleifendurchlauf = **ein Simulationsschritt** = 5 Simulationsminuten = 2,5 s Echtzeit.

### 3.1 `SimulationEngine.Step` — `Simulation/SimulationEngine.cs:109`

Die **Reihenfolge** ist wichtig, weil sie bestimmt, wann das Fahrzeug zu laden beginnt:

1. `_autoMode.Step(SimulationTime, Config.Step)` – **nur im Auto-Modus**: Abfahrten und Ankünfte.
2. `Charger.Tick(SimulationTime, Config.Step)` – Leistung aufteilen, Energie/SOC/Temperatur rechnen.
3. `SimulationTime += Config.Step` – Uhr weiterdrehen.

### 3.2 `WaitAndHandleInput` — `Simulation/SimulationEngine.cs:123`

Wartet `RealDelay` = Schrittweite / Tempo (Zeile 41; hier 300 s / 120 = 2,5 s), aber in
50-ms-Häppchen (`PollInterval`). In jedem Häppchen:

- `Console.KeyAvailable`? → `InputHandler.Handle(key)` (`IO/InputHandler.cs:24`) und sofort `Draw`.
- Nach `Q` (`IsRunning = false`) oder nach dem Fortsetzen aus der Pause wird die Restwartezeit abgebrochen.
- `CurrentDelay` wird jedes Mal neu berechnet, damit eine Tempoänderung (`<` / `>`) sofort wirkt.

Im Auto-Modus ohne Tastendruck passiert hier nichts außer Warten.

---

## 4. Phase 2 – Leerlauf bis zur Ankunft

Solange noch kein Fahrzeug da ist, läuft jeder Schritt so ab:

1. `AutoModeController.Step` (`Simulation/AutoModeController.cs:37`)
   - `HandleDepartures(now)` (Zeile 46): beide Punkte sind `Available`, also nicht `Finished` →
     nichts zu tun.
   - Zufallsentscheidung (Zeile 41):
     `_random.NextDouble() < GetArrivalProbability(now.Hour, dt)`
     - `GetArrivalRatePerHour(hour)` (Zeile 21): um 00:00 → **0,2 Ankünfte/h**
     - `GetArrivalProbability` (Zeile 34): p = 1 − e^(−0,2 · 5/60) ≈ **1,7 %** pro Schritt
     - Meistens **keine** Ankunft.
2. `DualCharger.Tick` (`Models/DualCharger.cs:62`)
   - `ProcessQueue` → Warteschlange leer.
   - `GetDemandKW()` beider Punkte → 0 (nicht `Charging`).
   - `AllocatePower(150, 0, 0)` → (0, 0).
   - `ChargingPoint.Update(0, dt)` → keine Energie, `UpdateTemperature` hält 20 °C.
3. Uhr + 5 min, CSV-Zeile, Anzeige, 2,5 s warten.

---

## 5. Phase 3 – Ereignis „Fahrzeug kommt an" (Schritt *n*)

Irgendwann ist die Zufallszahl kleiner als p. Jetzt passiert **alles innerhalb desselben `Step`**:

### 5.1 Fahrzeug erzeugen — `Models/VehicleCatalog.cs:34`

`AutoModeController.Step` (Zeile 42) ruft
`_charger.Enqueue(VehicleCatalog.CreateRandom(_random), now)`.

`VehicleCatalog.CreateRandom`:
- wählt zufällig ein Modell aus `Models` (Zeile 17) → hier **VW ID.3** (58 kWh, 120 kW),
- Start-SOC 10–50 % → **30 %**, Ziel-SOC 80–100 % → **90 %**,
- ruft `VehicleCatalog.Create` (Zeile 28) → `new Vehicle(...)` (`Models/Vehicle.cs:28`), das die Werte prüft.

### 5.2 In die Warteschlange und sofort anstecken — `Models/DualCharger.cs:52`

```
DualCharger.Enqueue(vehicle, now)                   DualCharger.cs:52
 ├─ _queue.Enqueue((vehicle, now))
 └─ ConnectWaitingVehicles()                        DualCharger.cs:101
     ├─ freePoint = erster Punkt mit IsFree → Punkt 1
     ├─ _queue.Dequeue()
     ├─ ChargingPoint.Connect(vehicle)              ChargingPoint.cs:54
     │     Available → Connected, Vehicle gesetzt
     └─ ChargingPoint.Start()                       ChargingPoint.cs:64
           Vehicle.IsFull? (30 % ≥ 90 %? nein) → Connected → Charging
```

**Ereignis:** Punkt 1 wechselt `Available → Connected → Charging`, beides in einem Aufruf.
Wäre kein Punkt frei, bliebe das Fahrzeug in `_queue` stehen (siehe Abschnitt 9).

### 5.3 Noch im selben Schritt: erster Ladeschritt

Zurück in `SimulationEngine.Step` folgt direkt `Charger.Tick` – das Fahrzeug lädt also
**schon im Ankunftsschritt**. Was dabei genau passiert, beschreibt Abschnitt 6.

---

## 6. Phase 4 – Laden (Schritte *n* … *n+3*)

Jeder Ladeschritt ist dieselbe Aufrufkette:

```
DualCharger.Tick(now, dt)                              DualCharger.cs:62
 ├─ ProcessQueue(now)                                  DualCharger.cs:89
 │    └─ ConnectWaitingVehicles()  → Warteschlange leer
 ├─ demandA = Points[0].GetDemandKW()                  ChargingPoint.cs:129
 │    └─ Vehicle.GetMaxPowerAtSoc() × DeratingFactor   Vehicle.cs:53 / ChargingPoint.cs:40
 ├─ demandB = Points[1].GetDemandKW() → 0
 ├─ (a, b) = AllocatePower(PowerLimitKW, demandA, 0)   DualCharger.cs:78
 ├─ Points[0].Update(a, dt)                            ChargingPoint.cs:141
 │    ├─ Vehicle.AddEnergy(power · h · η)              Vehicle.cs:66
 │    ├─ EnergyKWh += E_netz
 │    ├─ Vehicle.IsFull? → State = Finished            (Ereignis „voll")
 │    ├─ PowerKW = E_netz / h
 │    ├─ UpdateTemperature(PowerKW, dt)                ChargingPoint.cs:173
 │    └─ TemperatureC ≥ 80 °C? → EnterFault()          ChargingPoint.cs:182
 └─ Points[1].Update(0, dt)  → Punkt 2 bleibt frei, kühlt/bleibt bei 20 °C
```

### 6.1 Leistungsbedarf — `ChargingPoint.GetDemandKW` (`Models/ChargingPoint.cs:129`)

- Nur im Zustand `Charging`, sonst 0.
- `Vehicle.GetMaxPowerAtSoc()` (`Models/Vehicle.cs:53`) – die **Ladekurve**:
  bis 80 % SOC volle Leistung (120 kW), darüber `Pmax · e^(−k·(SOC−80))`.
- `DeratingFactor` (`Models/ChargingPoint.cs:40`) – bis 60 °C = 1,0, dann linear auf 0 bei 80 °C.
- Der Bedarf wird mit dem SOC **am Anfang** des Schritts berechnet.

### 6.2 Budget aufteilen — `DualCharger.AllocatePower` (`Models/DualCharger.cs:78`)

Mit nur einem Fahrzeug ist das trivial: 120 + 0 ≤ 150 → Punkt 1 bekommt seine **120 kW**.
(Interessant wird es erst bei zwei Fahrzeugen oder einem Fahrzeug über 150 kW, z. B. dem eActros
mit 400 kW: dann bekommt es das ganze Budget von 150 kW.)

Ist die Säule ausgeschaltet (`IsOn == false`, Taste `O`), werden beide Bedarfe in `Tick` auf 0 gesetzt.

### 6.3 Energie, SOC, Temperatur — `ChargingPoint.Update` (`Models/ChargingPoint.cs:141`)

1. `power = Clamp(allocatedKW, 0, GetDemandKW())`
2. `Vehicle.AddEnergy(power · Δt · η)` (`Models/Vehicle.cs:66`):
   - η = 0,95 → 120 kW · 1/12 h · 0,95 = **9,5 kWh** in die Batterie = **+16,4 % SOC**
   - begrenzt auf den Ziel-SOC; gibt die **tatsächlich** aufgenommene Energie zurück.
3. `E_netz = E_akku / η` wird zu `EnergyKWh` addiert (das zählt die Säule).
4. `PowerKW = E_netz / Δt` – **mittlere** Leistung im Schritt (wichtig für den letzten, kürzeren Schritt).
5. `UpdateTemperature` (Zeile 173): exakte Lösung 1. Ordnung,
   Zieltemperatur = 20 °C + 35 °C · (P/150)², Zeitkonstante τ = 10 min.
6. Störungsprüfung: ab 80 °C `EnterFault()` (bei unserem ID.3 erreicht die Temperatur nur ~37 °C).

### 6.4 Verlauf in Zahlen

Die Uhrzeit in der CSV ist die Zeit **nach** dem Schritt (weil `logger.Write` nach
`SimulationTime += Δt` aufgerufen wird). Ankunft z. B. im Schritt, der um 07:20 beginnt:

| Schritt | Start-SOC | Bedarf | Zugeteilt | SOC danach | PowerKW | Temp. danach | Zustand danach | CSV-Zeit |
|---|---|---|---|---|---|---|---|---|
| *n* (Ankunft) | 30,0 % | 120 kW | 120 kW | 46,4 % | 120,0 kW | ~28,8 °C | `Charging` | 07:25 |
| *n+1* | 46,4 % | 120 kW | 120 kW | 62,8 % | 120,0 kW | ~34,2 °C | `Charging` | 07:30 |
| *n+2* | 62,8 % | 120 kW | 120 kW | 79,1 % | 120,0 kW | ~37,4 °C | `Charging` | 07:35 |
| *n+3* | 79,1 % | 120 kW | 120 kW | **90,0 %** (gedeckelt) | ~79,6 kW | ~34,4 °C | **`Finished`** | 07:40 |

Im Schritt *n+3* würde die volle Leistung den SOC auf ~95 % treiben. `AddEnergy` nimmt aber
nur die 6,3 kWh bis 90 % an; daraus ergibt sich die mittlere Leistung von ~79,6 kW.
Weil der Start-SOC dieses Schritts noch unter 80 % liegt, greift die abfallende Ladekurve hier nicht –
bei einem höheren Ziel-SOC würde sie die Leistung in den folgenden Schritten deutlich senken.

---

## 7. Phase 5 – Ereignis „Ziel-SOC erreicht"

**Auslöser:** `ChargingPoint.Update` (`Models/ChargingPoint.cs:153–155`)

```csharp
if (Vehicle.IsFull)            // Vehicle.cs:26 → SocPercent >= TargetSocPercent
    State = ChargingPointState.Finished;
```

`AddEnergy` setzt den SOC beim Erreichen des Ziels exakt auf `TargetSocPercent`
(`Models/Vehicle.cs:75–76`), damit `IsFull` trotz Rundungsfehlern sicher greift.

Ab jetzt in jedem `Tick`:
- `GetDemandKW()` → 0 (Zustand ist nicht mehr `Charging`),
- `Update(0, dt)` → keine Energie, `PowerKW = 0`, die Temperatur sinkt Richtung 20 °C.

Das Fahrzeug **steht aber noch am Punkt** – es blockiert ihn, bis es abfährt.

---

## 8. Phase 6 – Ereignis „Fahrzeug fährt ab"

Zuständig ist `AutoModeController.HandleDepartures` (`Simulation/AutoModeController.cs:46`), das
am Anfang **jedes** `AutoModeController.Step` läuft – also im Schritt **nach** dem Erreichen von `Finished`.

### 8.1 Erster Schritt im Zustand `Finished` (Schritt *n+4*)

```
HandleDepartures(now)
 └─ Punkt 1: State == Finished
     ├─ noch kein Eintrag in _departures
     │    → departure = now + RandomDwellTime()       AutoModeController.cs:71
     │         80 %: 0–5 min   |   20 %: 10–40 min (blockiert den Punkt länger)
     └─ now >= departure?  → meistens noch nicht
```

Die geplante Abfahrtszeit wird im Dictionary `_departures` (Zeile 12) pro Ladepunkt gemerkt.

### 8.2 Abfahrt (z. B. Schritt *n+5*)

Sobald `now >= departure`:

```
HandleDepartures(now)
 ├─ ChargingPoint.Disconnect()                        ChargingPoint.cs:84
 │     Finished → Available, Vehicle = null, PowerKW = 0
 └─ _departures.Remove(point)
```

**Ereignis:** Punkt 1 ist wieder `Available`. Das `Vehicle`-Objekt wird von niemandem mehr
referenziert – das Fahrzeug ist aus der Simulation verschwunden. Die bisher geladene Energie bleibt
in `ChargingPoint.EnergyKWh` (bzw. `DualCharger.TotalEnergyKWh`) erhalten.

Im selben `AutoModeController.Step` wird danach wieder gewürfelt, ob ein neues Fahrzeug kommt –
der Kreislauf beginnt von vorn (Abschnitt 4).

---

## 9. Abweichungen vom Normalfall (kurz)

| Ereignis | Wo ausgelöst | Was passiert |
|---|---|---|
| **Beide Punkte belegt** bei Ankunft | `DualCharger.ConnectWaitingVehicles` (`Models/DualCharger.cs:101`) findet keinen freien Punkt | Fahrzeug bleibt in `_queue`. In jedem `Tick` versucht `ProcessQueue` (Zeile 89) erneut anzustecken; nach **5 min** (`MaxQueueWait`) fährt es weg → `RejectedVehicles++`. |
| **Überhitzung** (≥ 80 °C) | `ChargingPoint.Update` → `EnterFault()` (`Models/ChargingPoint.cs:164–165, 182`) | `Charging → Faulted`, `PowerKW = 0`, `_wasChargingBeforeFault = true`. Typisch beim „Kia EV6 (defekt)": doppelte Erwärmung, kein Derating. |
| **Störung beendet** | `ChargingPoint.Update` → `TryReset(50 °C)` (Zeile 162–163) automatisch, oder Taste `R` → `Reset()` (Zeile 109) unter 60 °C | Zurück nach `Charging` (wenn vorher geladen wurde), `Finished` (wenn voll) oder `Available` (ohne Fahrzeug). |
| **Taste F** | `InputHandler.Handle` → `ChargingPoint.TriggerFault()` (`Models/ChargingPoint.cs:98`) | Kühlungsdefekt: 85 °C, τ = 30 min, `Faulted`. |
| **Im Zustand `Faulted`** | `HandleDepartures` | Kein Abstecken möglich (Kabel verriegelt) – `Disconnect()` liefert `false`, `_departures`-Eintrag wird entfernt. |
| **Säule aus** (Taste `O`) | `DualCharger.Tick` (`Models/DualCharger.cs:66–67`) | Bedarf beider Punkte = 0; Zustand bleibt `Charging`, aber es fließt keine Leistung. |
| **Limit ändern** (`+`/`-`) | `InputHandler.ChangePowerLimit` (`IO/InputHandler.cs:97`) | `DualCharger.PowerLimitKW` (50–350 kW) gilt ab dem nächsten `AllocatePower`. |

### Variante: dasselbe Fahrzeug im manuellen Modus

Nach Taste `A` (`IsAutoMode = false`) wird `AutoModeController.Step` **nicht mehr** aufgerufen –
es gibt weder zufällige Ankünfte noch automatische Abfahrten.

| Schritt | Taste | Aufrufkette |
|---|---|---|
| Ankunft | `N` | `InputHandler.Handle` → `SimulationEngine.VehicleArrives` (`Simulation/SimulationEngine.cs:64`) → `VehicleCatalog.CreateRandom` → `DualCharger.Enqueue` → `Connect` + `Start` (wie Abschnitt 5.2) |
| oder: gezielt anstecken | `C` | `InputHandler.ConnectVehicle` (`IO/InputHandler.cs:133`) → `ConsoleRenderer.Prompt` (Modell, SOC) → `VehicleCatalog.Create` → `ChargingPoint.Connect` (nur `Connected`!) |
| Laden starten/stoppen | `S` | `InputHandler.ToggleCharging` (`IO/InputHandler.cs:117`) → `ChargingPoint.Start()` / `Stop()` |
| Laden | – | unverändert über `DualCharger.Tick` in jedem `Step` (Abschnitt 6) |
| Abstecken | `D` | `InputHandler.Handle` → `ChargingPoint.Disconnect()` |

Tasten wirken immer **zwischen** zwei Schritten (in `WaitAndHandleInput`); die Änderung wird im
nächsten `Step` berücksichtigt und sofort per `Draw` angezeigt.

---

## 10. Phase 7 – Ende der Simulation

Die Hauptschleife endet, wenn

- `IsFinished` (`Simulation/SimulationEngine.cs:28`): `SimulationTime >= _endTime` (hier nach 24 h = 288 Schritten), **oder**
- `IsRunning == false`: Taste `Q` → `InputHandler.Handle` → `SimulationEngine.Quit()` (Zeile 58).

Danach in `Run` (Zeile 98–106):

| Zeile | Aufruf | Was passiert |
|---|---|---|
| 98–103 | nur wenn regulär zu Ende: Meldung setzen, `Draw`, `WaitForQuit()` (Zeile 151) | Anzeige bleibt stehen, bis `Q` gedrückt wird |
| 105 | `_renderer.Shutdown()` → `IO/ConsoleRenderer.cs:34` | Cursor unter die Anzeige setzen und wieder einblenden |
| 106 | `Console.WriteLine(...)` | vollständigen Pfad der CSV-Datei ausgeben |
| Blockende | `CsvLogger.Dispose()` → `IO/CsvLogger.cs:66` (durch `using var`) | Datei schließen |

Zurück in `Program.Main` → Rückgabewert `0`.

---

## 11. Zusammenfassung: Lebenslauf des einen Fahrzeugs

| # | Ereignis | Auslösende Funktion | Datei | Zustand Punkt 1 |
|---|---|---|---|---|
| 1 | Programmstart, Aufbau | `Program.Main` → `SimulationEngine`-Konstruktor | `Program.cs`, `Simulation/SimulationEngine.cs` | `Available` |
| 2 | Leere Schritte | `SimulationEngine.Step` → `AutoModeController.Step` (kein Treffer) → `DualCharger.Tick` | `Simulation/…`, `Models/DualCharger.cs` | `Available` |
| 3 | Zufällige Ankunft | `AutoModeController.Step` → `VehicleCatalog.CreateRandom` | `Simulation/AutoModeController.cs`, `Models/VehicleCatalog.cs` | `Available` |
| 4 | Anstecken | `DualCharger.Enqueue` → `ConnectWaitingVehicles` → `ChargingPoint.Connect` | `Models/DualCharger.cs`, `Models/ChargingPoint.cs` | `Connected` |
| 5 | Ladestart | `ChargingPoint.Start` | `Models/ChargingPoint.cs` | `Charging` |
| 6 | Laden (je Schritt) | `DualCharger.Tick` → `GetDemandKW` → `AllocatePower` → `ChargingPoint.Update` → `Vehicle.AddEnergy` | `Models/DualCharger.cs`, `Models/ChargingPoint.cs`, `Models/Vehicle.cs` | `Charging` |
| 7 | Ziel-SOC erreicht | `ChargingPoint.Update` (`Vehicle.IsFull`) | `Models/ChargingPoint.cs` | `Finished` |
| 8 | Verweilzeit festlegen | `AutoModeController.HandleDepartures` → `RandomDwellTime` | `Simulation/AutoModeController.cs` | `Finished` |
| 9 | Abfahrt | `AutoModeController.HandleDepartures` → `ChargingPoint.Disconnect` | `Simulation/AutoModeController.cs`, `Models/ChargingPoint.cs` | `Available` |
| 10 | Nach jedem Schritt | `CsvLogger.Write`, `ConsoleRenderer.Draw`, `WaitAndHandleInput` | `IO/CsvLogger.cs`, `IO/ConsoleRenderer.cs`, `Simulation/SimulationEngine.cs` | – |
| 11 | Simulationsende | `SimulationEngine.Run` (Schleifenende) → `Shutdown` → `Dispose` | `Simulation/SimulationEngine.cs`, `IO/…` | – |
