# Kühlhaus als viertes Feldgerät – Schritt-für-Schritt-Umsetzung

**Ziel:** `src/Devices/KuehlhausSimulator` vom LB01-Testprogramm zum LB02-Feldgerät umbauen:
virtuelle Zeit vom Master, async-Schleife, Modbus-Server mit Registerplan, CSV, README.
Der Dual-Port-Charger (`src/Devices/DualPortCharger`) ist die Vorlage – gleiche Klassen, gleiche Reihenfolge.

**Entschieden:**
- Außentemperatur kommt vom **Wettersimulator** (`GET /weather`, Feld `TemperatureC`); ohne Master bzw. mit
  `--weather off` rechnet ein lokales, geseedetes Tagesprofil.
- Modbus-Standardport **1505** (1502 ist bei PV/Charger belegt, 1503 bei der Batterie, 1504 nutzt der Charger in der Demo).
- Neuer Gerätetyp **5 = Industrieverbraucher** (4 bleibt Wind).
- Gerätename: `KH-01`. Projekt/Ordner heißen `KuehlhausSimulator` (wie `PvSimulator`, `BatterySimulator`,
  `DualPortCharger`) – entschieden und am 2026-10-26 umbenannt.

**Stand vor dem Umbau:** `Kuehlhaus.cs` und `KuehlhausConfig.cs` enthalten das Modell, `Program.cs` ist ein
LB01-Testmenü mit `ReadLine` und kompiliert nicht (`SetDoor(bool)` fehlt). Das Projekt ist nicht in `EnergyTwin.sln`
und hat keine Referenz auf `Devices.Common`.

**Stand 2026-10-26 (umgesetzt):** **alle neun Schritte sind fertig.** `KuehlhausSimulator` steht in `EnergyTwin.sln`
(unter `Devices`), das Modell ist repariert (exakte Exponentiallösung, `SetDoor(bool)`, Einheiten, `ResetFault()`),
`DeviceType.IndustrialConsumer = 5`, `KuehlhausRegisters.cs` und der Eintrag in `RegisterPlans.For` sind da.
Dazu kommen `SimulationConfig`, `SimulationEngine` (Master-Zeit, feste Schritte, CSV je Lauf), `AutoModeController`
(Tür nach Tageszeit), `KuehlhausModbus` (Register 20–31), `WeatherClient` (Außentemperatur je Modellstunde,
Fehler nach 3 Fehlversuchen sichtbar, Wiederholung alle 10 s), `ArgsParser`/`CsvLogger`, `ConsoleRenderer`/`InputHandler`
(Anzeige und Tasten `T/S/F/R/O/A/P/</>/Q`), 28 Tests in `tests/FieldDevices.Tests/ConsumerTests.cs` und das Geräte-README.
`dotnet build EnergyTwin.sln` läuft mit 0 Warnungen, `tests/FieldDevices.Tests` 62/62, `--register-plan` gibt die
Registertabelle aus. Probeläufe: 24 h standalone (289 CSV-Zeilen, 20,7 kWh, Tür 25 Schritte offen), mit Master
(Zeitstatus „synchron“, Tempo 600), nach Master-Abbruch „Master verloren“ im Register 12, Sollwert über den
Diagnose-Client gesetzt (`-20 → -15 °C`) und `force 5` vom Gerät abgelehnt (Register 18 = 2, Adresse 20).
**Entschieden (2026-10-26):** Projektname `KuehlhausSimulator`, `ZusatzwaermeKW` = 1,5 kW, Modbus-Standardport 1505.
**Offen:** echte Terminalfenster (Anzeige, Tasten, `S`-Eingabe).

---

## Überblick

| # | Schritt | Ergebnis | Prüfung |
|---|---|---|---|
| 1 | Projekt in die Solution, `.csproj` aufräumen | baut leer mit | `dotnet build EnergyTwin.sln` | ✅ |
| 2 | Modell reparieren (`SetDoor`, exakte Temperaturlösung, Einheiten) | Modell ohne Konsole | `dotnet build` | ✅ |
| 3 | Contracts: `DeviceType`, `KuehlhausRegisters`, `RegisterPlans` | Registerplan steht | `dotnet run -- --register-plan` | ✅ |
| 4 | `SimulationConfig` + `SimulationEngine` | folgt dem Master, schreibt CSV | Probelauf standalone | ✅ |
| 5 | `KuehlhausModbus` | Register lesen/schreiben | Diagnose-Client | ✅ |
| 6 | Wetteranbindung | Außentemperatur vom Wettersimulator | Probelauf mit Wetter | ✅ |
| 7 | `ConsoleRenderer`, `InputHandler` | bedienbar wie der Charger | Probelauf im Terminal (offen) | ✅ |
| 8 | Tests | 28 neue Tests grün | `dotnet test` | ✅ |
| 9 | Doku und Journal | README, zentrales README, `AGENTS.md` | Sichtprüfung | ✅ |

---

## Schritt 1 – Projekt in die Solution (10 min)

`src/Devices/KuehlhausSimulator/KuehlhausSimulator.csproj` (bzw. `KuehlhausSimulator.csproj`) auf das Minimum kürzen:
`TargetFramework`, `ImplicitUsings`, `Nullable` und `TreatWarningsAsErrors` kommen schon aus
`src/Directory.Build.props` – doppelt definiert ist es zwar kein Fehler, aber inkonsistent.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="../../Devices.Common/Devices.Common.csproj" />
  </ItemGroup>

</Project>
```

`Devices.Common` bringt EasyModbusTCP.NETCore, `SimCore.*`, `Modbus.Contracts` und `Shared.Config` mit – es braucht
keine weiteren Referenzen.

```bash
dotnet sln EnergyTwin.sln add src/Devices/KuehlhausSimulator/KuehlhausSimulator.csproj --solution-folder Devices
dotnet build EnergyTwin.sln
```

Wenn `--solution-folder` den Eintrag nicht unter `Devices` einsortiert: in Visual Studio verschieben oder den
`NestedProjects`-Block der `.sln` von Hand ergänzen (Muster: `WindSimulator`, GUID `{A604271D-…}`).
`Emums.cs` → `Enums.cs` umbenennen (`git mv`).

---

## Schritt 2 – Modell aufräumen (1 h)

`Kuehlhaus` bleibt frei von Konsole, Uhr und Netzwerk (Vorbild: `Batterie`, `DualCharger`). Änderungen:

1. **Build-Fehler:** `public void SetDoor(bool open) => DoorOpen = open;` (statt des parameterlosen `SetDoor()`).
2. **Namens- und Einheitenfehler** in `KuehlhausConfig`:

   | jetzt | neu | Einheit |
   |---|---|---|
   | `SollTempC` | `SollTempC` | °C |
   | `HystereseK` | `HystereseK` | K |
   | `WaermekapazitaetKwH` | `WaermekapazitaetKWhProK` | kWh/K |
   | `WaermeduchgangKw` | `WaermedurchgangKWProK` | kW/K |
   | `KuelleistungKw` | `KaelteleistungKW` | kW |
   | `ZusatzwaermekW` | `ZusatzwaermeKW` | kW |
   | `AlarmTempC`, `COP` | unverändert | °C, – |

   `ZusatzwaermekW` wird heute wie kW verwendet (fließt in `heatGainKW`), deshalb umbenennen **und** den Wert
   gedanklich prüfen (3 kW Türverlust ist viel für ein Kühlhaus; 0,5–1,5 kW sind plausibler).
   **Umgesetzt (2026-10-26):** `ZusatzwaermeKW` = **1,5 kW** (offene Tür: Gleichgewicht Außen + 30 K).
3. **Exakte Exponentiallösung** statt Euler – sonst schwingt der Raum bei `--step 5` und großer Kühlleistung.
   Das Charger-Muster (`ChargingPoint.UpdateTemperature`) 1:1:

   ```csharp
   // C·dT/dt = k·(T_aus − T) + Q_Tür − Q_Kühl
   public void Update(TimeSpan dt, double outsideC)
   {
       UpdateCompressor();                                  // Hysterese, wie bisher

       double k = _config.WaermedurchgangKWProK;            // kW/K
       double c = _config.WaermekapazitaetKWhProK;          // kWh/K
       double doorKW = DoorOpen ? _config.ZusatzwaermeKW : 0;
       double coolingKW = CompressorOn ? _config.KaelteleistungKW : 0;

       // Gleichgewichtstemperatur und Zeitkonstante des Systems
       double target = outsideC + (doorKW - coolingKW) / k;
       double tauHours = c / k;

       RoomTempC = target + (RoomTempC - target) * Math.Exp(-dt.TotalHours / tauHours);

       ElectricPowerKW = CompressorOn ? _config.KaelteleistungKW / GetCop(outsideC) : 0;
       EnergyKWh += ElectricPowerKW * dt.TotalHours;

       if (RoomTempC > _config.AlarmTempC)
           _overTempLatched = true;

       UpdateState();
   }
   ```

   Die alte Signatur `Update(double dtSeconds, double outsideTempC)` als Overload behalten, damit die LB01-Tests
   weiterlaufen: `=> Update(TimeSpan.FromSeconds(dtSeconds), outsideTempC)`.
4. **Entscheiden und dokumentieren:** Soll ein gespeicherter Alarm (`OverTemperature`) den Kompressor abschalten?
   Der Charger macht das bei `Faulted`. Für die Vorführung ist „Fault ⇒ 0 kW“ klarer, weil der Modbus-Fehlercode
   dann sichtbar zur Leistung passt. Wenn ja: in `UpdateCompressor()` zusätzlich `if (ActiveFault != FaultType.None) { CompressorOn = false; return; }`.
5. **Reset wie beim Charger**, aber ohne Selbstblockade:

   ```csharp
   public bool ResetFault()            // true = mindestens eine Störung zurückgesetzt
   {
       bool cleared = _compressorFailure;
       _compressorFailure = false;     // immer freigeben, sonst kühlt nichts mehr

       if (_overTempLatched && RoomTempC <= _config.AlarmTempC - _config.AlarmHystereseK)
       {
           _overTempLatched = false;
           cleared = true;
       }

       return cleared;
   }
   ```

   **Korrektur gegenüber der ersten Fassung:** Ein `return false`, solange der Raum zu warm ist, sperrt sich selbst aus –
   der Kompressor bleibt aus, der Raum wird wärmer, der Reset geht nie. Der Charger hat dieses Problem nicht, weil
   seine Temperatur von selbst auf Umgebung abfällt; das Kühlhaus kühlt nur mit laufendem Kompressor. Deshalb:
   Kompressorausfall immer quittieren, den Übertemperatur-Alarm erst unterhalb `AlarmTempC − AlarmHystereseK`
   (neue Konfigurationsgröße, Standard 2 K).
6. **`State`** in `UpdateState()` wie bisher mappen; `DeviceState.Off/Idle/Cooling/Fault` passt 1:1 auf
   `OperatingState.Off/Standby/Running/Fault` im Registerkopf.

**Prüfung:** `dotnet build src/Devices/KuehlhausSimulator` – 0 Fehler, 0 Warnungen.

---

## Schritt 3 – Contracts (30 min)

### 3a. `src/Modbus.Contracts/CommonRegisters.cs`

```csharp
public enum DeviceType : ushort
{
    PvPlant = 1,
    Battery = 2,
    Charger = 3,
    Wind = 4,
    IndustrialConsumer = 5
}
```

Im Kopf-Eintrag `DeviceType` die Beschreibung und Labels ergänzen:
`"1 PV, 2 Batterie, 3 Charger, 4 Wind, 5 Industrieverbraucher"` und
`Labels: ["unbekannt", "PV-Anlage", "Batteriespeicher", "Charger", "Windkraftanlage", "Industrieverbraucher"]`.
Danach die Kopftabelle in `src/Modbus.Contracts/README.md` (Zeile 24) und in den Geräte-READMEs, die sie zitieren,
mitziehen.

### 3b. Neu `src/Modbus.Contracts/KuehlhausRegisters.cs`

Muster: `ChargerRegisters.cs`. Bereich 20–49, Adressen wie vorgeschlagen:

```csharp
namespace Modbus.Contracts;

/// <summary>
/// Gerätespezifische Register des Kühlhauses (Adressen 20–31) und ihr <see cref="RegisterPlan"/>.
/// Leistung: positiv = Aufnahme (Verbraucher). Die Energie im Kopf ist die aufgenommene elektrische Energie.
/// </summary>
public static class KuehlhausRegisters
{
    public const int Setpoint = 20;          // Float32, schreibbar: Solltemperatur °C
    public const int Door = 22;              // UInt16,  schreibbar: 0 zu, 1 offen
    public const int RoomTemperature = 23;   // Float32
    public const int OutsideTemperature = 25;// Float32
    public const int Compressor = 27;        // UInt16
    public const int CoolingPower = 28;      // Float32, kW (Kälteleistung)
    public const int Runtime = 30;           // Float32, h

    public const double MinSetpointC = -30;
    public const double MaxSetpointC = 0;

    public static RegisterPlan Plan { get; } = new(DeviceType.IndustrialConsumer, "KuehlhausSimulator",
    [
        new(Setpoint, "Solltemperatur", RegisterType.Float32, RegisterAccess.ReadWrite, "°C",
            "Sollwert der Raumtemperatur. Außerhalb −30 … 0 °C wird der Schreibzugriff abgelehnt " +
            "(Letzter Schreibfehler = 2), der alte Wert bleibt",
            Min: MinSetpointC, Max: MaxSetpointC),
        new(Door, "Tür", RegisterType.UInt16, RegisterAccess.ReadWrite, "",
            "0 = geschlossen, 1 = offen (erhöht den Wärmeeintrag)", Min: 0, Max: 1, Labels: ["zu", "offen"]),
        new(RoomTemperature, "Raumtemperatur", RegisterType.Float32, RegisterAccess.ReadOnly, "°C",
            "Temperatur im Kühlraum"),
        new(OutsideTemperature, "Außentemperatur", RegisterType.Float32, RegisterAccess.ReadOnly, "°C",
            "Vom Wettersimulator (--weather) oder aus dem lokalen Tagesprofil"),
        new(Compressor, "Kompressor", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "1 = läuft", Min: 0, Max: 1, Labels: ["aus", "läuft"]),
        new(CoolingPower, "Kälteleistung", RegisterType.Float32, RegisterAccess.ReadOnly, "kW",
            "Thermische Leistung des Kompressors (elektrisch = Kälteleistung / COP)"),
        new(Runtime, "Laufzeit", RegisterType.Float32, RegisterAccess.ReadOnly, "h",
            "Kompressorlaufzeit seit Programmstart"),
    ]);
}
```

Der **Fehlercode bleibt im Kopf** (Register 3): 1 = Kompressorausfall, 2 = Übertemperatur. Damit verhält sich das
Gerät wie PV und Batterie, und der Diagnose-Client zeigt ihn ohne Sonderfall.

### 3c. `src/Modbus.Contracts/RegisterPlans.cs`

```csharp
DeviceType.IndustrialConsumer => KuehlhausRegisters.Plan,
```

**Prüfung:** `dotnet run --project src/Devices/KuehlhausSimulator -- --register-plan` gibt die Markdown-Tabelle aus
(dafür in Schritt 5 den Zweig in `Program.cs` einbauen).

---

## Schritt 4 – Simulation (2 h, überwiegend kopieren)

### 4a. `Simulation/SimulationConfig.cs`

Form von `DualPortCharger/Simulation/SimulationConfig.cs` übernehmen:

```csharp
public class SimulationConfig
{
    public const double DefaultStandaloneSpeed = 600;
    public const double MinimumRecommendedHours = 24;

    public string DeviceId { get; init; } = "KH-01";
    public double SetpointC { get; init; } = -20;
    public double StepMinutes { get; init; } = 5;
    public double? DurationHours { get; init; }         // nur standalone
    public int? Seed { get; init; }
    public string OutputDirectory { get; init; } = OutputPaths.CsvDirectory;
    public TimeSyncOptions TimeSync { get; init; } = new() { StandaloneSpeed = DefaultStandaloneSpeed };
    public ModbusOptions Modbus { get; init; } = new() { Port = 1505 };   // eigener Standardport!
    public string? WeatherUrl { get; init; }            // null = lokales Tagesprofil
    public string Location { get; init; } = "salzburg";

    public TimeSpan Step => TimeSpan.FromMinutes(StepMinutes);
    public TimeSpan? Duration => DurationHours is { } h ? TimeSpan.FromHours(h) : null;
    public KuehlhausConfig ToModelConfig() => new() { DeviceId = DeviceId, SollTempC = SetpointC };
}
```

### 4b. `Simulation/SimulationEngine.cs`

`DualPortCharger/Simulation/SimulationEngine.cs` als Vorlage **fast wörtlich** übernehmen. Die Stellen, die sich
ändern:

| Charger | Kühlhaus |
|---|---|
| `Charger = new DualCharger(...)` | `Room = new Kuehlhaus(config.ToModelConfig())` |
| `_autoMode = new AutoModeController(Charger, _random)` | `_autoMode = new AutoModeController(Room, _random)` |
| `DeviceTimeSource.Create(..., "Charger")` | `..., "Kühlhaus")` – erscheint so in der Master-Liste |
| `new ChargerModbus(Charger, Time, config.Modbus, …)` | `new KuehlhausModbus(Room, Time, config.Modbus, …)` |
| `_autoMode.Step(stepStart, Step); Charger.Tick(stepStart, Step);` | Wetter holen (Schritt 6), `_autoMode.Step(stepStart, Step)`, `Room.Update(Step, outsideC)` |
| `SelectedPointIndex`, `SelectedPoint`, `VehicleArrives` | `Türe offen/zu`, Sollwert ändern – oder ganz streichen (Bedienung in `InputHandler`) |

Alles andere bleibt: `PollInterval = 50 ms`, `RenderInterval = 200 ms` (umgeleitet 5 s),
`IsFinished`/`EndTime`, `Started`/`Restarted`-Zweig mit CSV-Zeile, `SkippedSteps`-Warnung, `try/finally` mit
`stop.CancelAsync()`, `_modbus?.Dispose()`, `await syncTask`, `_renderer.ShutdownAsync()`.

Wichtig, wie beim Charger: **`ProcessCommands()` und `Publish(SimulationTime, RunId)` in jedem Schleifendurchlauf**,
also auch in der Pause und bei `MasterLost` – sonst zeigen die Register während der Pause alte Zeitstatus-Werte.

### 4c. Neu `Simulation/AutoModeController.cs`

Der Charger regelt Ankünfte; hier sind es **Türöffnungen**:

```csharp
public sealed class AutoModeController
{
    private readonly Kuehlhaus _room;
    private readonly Random _random;
    private DateTime? _doorClosesAt;

    /// <summary>Öffnungen pro Stunde nach Tageszeit (Lieferverkehr, Schichtwechsel).</summary>
    public static double GetDoorRatePerHour(int hour) => hour switch
    {
        < 6 => 0.1,
        < 9 => 1.0,
        < 12 => 1.5,
        < 18 => 0.8,
        _ => 0.4
    };

    public static double GetDoorProbability(int hour, TimeSpan dt) =>
        1 - Math.Exp(-GetDoorRatePerHour(hour) * dt.TotalHours);

    public void Step(DateTime now, TimeSpan dt)
    {
        if (_doorClosesAt is { } closes)
        {
            if (now >= closes) { _room.SetDoor(false); _doorClosesAt = null; }
            return;                                    // Tür bleibt bis zum Schließen offen
        }

        if (_random.NextDouble() < GetDoorProbability(now.Hour, dt))
        {
            _room.SetDoor(true);
            _doorClosesAt = now + TimeSpan.FromMinutes(1 + _random.NextDouble() * 9);
        }
    }
}
```

### 4d. Lokales Tagesprofil (Rückfall ohne Wettersimulator)

Deterministisch, nicht aus der Uhr geseedet:

```csharp
/// <summary>Außentemperatur ohne Wettersimulator: Tagesgang 14 ± 9 K, Minimum 05:00, Maximum 15:00.</summary>
public static double LocalOutsideTemperature(DateTime time) =>
    14 + 9 * Math.Sin(2 * Math.PI * (time.Hour + time.Minute / 60.0 - 9) / 24);
```

---

## Schritt 5 – Modbus-Anbindung

Neu `Simulation/KuehlhausModbus.cs`, Muster `ChargerModbus.cs` (drei Methoden):

```csharp
public sealed class KuehlhausModbus : IDisposable
{
    private readonly Kuehlhaus _room;
    private readonly DeviceTimeSource _time;
    private readonly Action<string> _report;
    private readonly ModbusDeviceServer _server;

    public KuehlhausModbus(Kuehlhaus room, DeviceTimeSource time, ModbusOptions options, Action<string> report)
    {
        _room = room;
        _time = time;
        _report = report;
        _server = new ModbusDeviceServer(options, KuehlhausRegisters.Plan, room.Id);
        _server.Log += (_, message) => _report(message);
    }

    public string StatusText => _server.StatusText;

    public void Start() { _server.Start(); Publish(null, 0); }

    public void ProcessCommands()
    {
        while (_server.TryDequeueCommand(out var command))
        {
            switch (command!.Definition.Address)
            {
                case KuehlhausRegisters.Setpoint:
                    _room.SetpointC = command.Value;                 // geprüft: −30 … 0 °C
                    _report($"Modbus: Solltemperatur auf {command.Value:0.#} °C gesetzt.");
                    break;
                case KuehlhausRegisters.Door:
                    _room.SetDoor(command.Value >= 1);
                    _report($"Modbus: Tür {(command.Value >= 1 ? "geöffnet" : "geschlossen")}.");
                    break;
                case CommonRegisters.PowerSwitch:
                    if (command.Value >= 1) _room.TurnOn(); else _room.TurnOff();
                    _report($"Modbus: Anlage {(_room.IsOn ? "eingeschaltet" : "ausgeschaltet")}.");
                    break;
                case CommonRegisters.ResetCommand:
                    _report(_room.ResetFault() ? "Modbus: Störung zurückgesetzt."
                            : $"Modbus: Reset nicht möglich (Raum {_room.RoomTempC:0.0} °C).");
                    break;
            }
        }
    }

    public void Publish(DateTime? modelTime, int runId)
    {
        var state = !_room.IsOn ? OperatingState.Off
            : _room.ActiveFault != FaultType.None ? OperatingState.Fault
            : _room.CompressorOn ? OperatingState.Running
            : OperatingState.Standby;

        ushort errorCode = _room.ActiveFault switch
        {
            FaultType.CompressorFailure => 1,
            FaultType.OverTemperature => 2,
            _ => 0
        };

        var header = new HeaderValues(state, errorCode, _room.ElectricPowerKW, _room.EnergyKWh,
                                      _room.IsOn, modelTime, runId);

        _server.Publish(header, _time, writer =>
        {
            writer.SetFloat(KuehlhausRegisters.Setpoint, _room.SetpointC);
            writer.SetUInt16(KuehlhausRegisters.Door, _room.DoorOpen ? 1 : 0);
            writer.SetFloat(KuehlhausRegisters.RoomTemperature, _room.RoomTempC);
            writer.SetFloat(KuehlhausRegisters.OutsideTemperature, OutsideTemperatureC);
            writer.SetUInt16(KuehlhausRegisters.Compressor, _room.CompressorOn ? 1 : 0);
            writer.SetFloat(KuehlhausRegisters.CoolingPower, _room.CompressorOn ? _config.KaelteleistungKW : 0);
            writer.SetFloat(KuehlhausRegisters.Runtime, _room.RuntimeHours);
        });
    }

    public void Dispose() => _server.Dispose();
}
```

Zwei Punkte zum Nachdenken:
- `SetpointC` liegt im Modell als `private set` – einen Setter mit `Math.Clamp` oder `ArgumentException` ergänzen;
  der Registerplan prüft schon, aber das Modell sollte sich nicht auf den Server verlassen.
- `_server.ValueValidator` ist nur nötig, wenn die Grenze von der Konfiguration abhängt (wie bei der Batterie,
  ±max. Leistung). Hier reicht `Min`/`Max` im Plan.

Für die Anzeige der Außentemperatur in `Publish` den zuletzt verwendeten Wert in einem Feld der Engine halten und
an `KuehlhausModbus` übergeben (Konstruktorparameter oder Property `OutsideTemperatureC`).

---

## Schritt 6 – Wetteranbindung (1 h)

1. `src/Devices/PvSimulator/Simulation/WeatherClient.cs` nach `src/Devices/KuehlhausSimulator/Simulation/WeatherClient.cs`
   kopieren, Namespace auf `KuehlhausSimulator.Simulation` ändern. Beide Klassen (`WeatherUnavailableException`,
   `WeatherClient`) sind geräteunabhängig und benutzen nur `SimCore.Contracts.WeatherData`.
   **Nicht** in `Devices.Common` verschieben, solange die PV ihre eigene Kopie hat – das wäre ein eigener Umbau.
2. In `SimulationEngine` genau das PV-Muster übernehmen (`src/Devices/PvSimulator/Simulation/SimulationEngine.cs`,
   `UpdateWeatherAsync`, Zeilen 222–259):
   - Abfrage **je Modellstunde**: `hour = new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0, DateTimeKind.Utc)`
   - nur holen, wenn `_lastWeather is null || _lastWeather.SimTimeUtc != hour || _lastWeather.RunId != RunId`
   - Erfolg: `_lastWeather = data`, `_weatherFailures = 0`, Statuszeile mit `data.TemperatureC`,
     `data.WindSpeedMs`, `data.Condition`
   - Fehler (`WeatherUnavailableException`): Zähler++, `_nextWeatherAttempt = DateTime.UtcNow.AddSeconds(10)`,
     Meldung „… – n. Fehlversuch, letztes Wetter bleibt“; ab 3 Fehlversuchen einmal eine sichtbare Meldung
   - immer `outsideC = _lastWeather?.TemperatureC ?? LocalOutsideTemperature(stepEnd)`
3. Default-Regel wie bei der PV: **mit Master** `http://127.0.0.1:8080`, **standalone** lokal.
   Im `ArgsParser` umsetzen (`--weather <url|off>`, `--location`, Standard `salzburg`).
4. Anzeige: eigene Zeile „Wetter“ (Standort, Temperatur, Zustand, letzte Abfrage) im `ConsoleRenderer`,
   Status auch in der einzeiligen Ausgabe für umgeleitete Protokolle.

---

## Schritt 7 – Ein-/Ausgabe

| Datei | Vorlage | Änderungen |
|---|---|---|
| `IO/ArgsParser.cs` | `DualPortCharger/IO/ArgsParser.cs` | eigene Optionen: `--id`, `--setpoint`, `--cooling`, `--alarm`, `--step`, `--seed`, `--output`; `TimeSyncOptions.TryApply` und `ModbusOptions.TryApply` übernehmen; `--duration`/`--start`/`--speed` nur mit `--standalone`; `--register-plan` in `Program.cs` behandeln |
| `IO/CsvLogger.cs` | Charger | Spalten: `Zeit;Lauf;Modus;Anlage_Ein;Soll_C;Raum_C;Aussen_C;Kompressor;Kaelte_kW;Leistung_kW;Energie_kWh;Tuer;Fehler`; de-AT, UTF-8 mit BOM, `Lauf`-Spalte |
| `IO/ConsoleRenderer.cs` | Charger | Zeilen: Kopf, Simulationszeit/Lauf/Schritt, Zeitquelle, Modbus, Wetter, Anlage, Raumtemperatur mit Balken gegenüber Soll, Kompressor, Leistung/Energie, Tür, Meldung, Tastenhilfe; Fallback eine Zeile bei umgeleiteter Ausgabe |
| `IO/InputHandler.cs` | Charger | Tasten unten |

Tasten (bewusst andere als beim Charger, damit sie nicht verwechselt werden):

| Taste | Wirkung |
|---|---|
| `T` | Tür öffnen/schließen |
| `S` | Solltemperatur eingeben (Prompt, `Esc` = Abbruch) |
| `F` | Kompressorausfall auslösen |
| `R` | Störung zurücksetzen |
| `O` | Anlage ein/aus |
| `P`, `<`, `>` | nur standalone: Pause, Tempo |
| `Q` | Beenden |

`Program.cs` übernimmt die Struktur des Chargers wörtlich: Hilfe, `--register-plan`
(`Console.Write(KuehlhausRegisters.Plan.ToMarkdown())`), `ArgsParser.Parse`, `Console.CancelKeyPress`,
`engine.RunAsync`, `IOException`/`UnauthorizedAccessException` → Meldung, Exit-Code 1.
Das alte Testmenü entfällt – die fünf Szenarien werden Tests (Schritt 8) oder ein separates
`docs/devices/KuehlhausSimulator/`-Dokument.

---

## Schritt 8 – Tests

`tests/FieldDevices.Tests` um `KuehlhausSimulator` erweitern (Projekt-Referenz ergänzen) oder ein eigenes
`tests/Consumer.Tests` anlegen. Wiederverwendbar: `ModbusTestBase` aus `FieldDeviceTests.cs`
(freier Port, `Connect`, `ReadFloat`, `WriteFloat`, `WaitUntil`).

Mindestens:

1. `Header_ShowsConsumptionAsPositivePower_AndRoomValues` – `DeviceType.IndustrialConsumer`, `ActivePower > 0`,
   Raumtemperatur, Sollwert.
2. `Setpoint_ValidValue_IsApplied_AtNextStep` – `WriteFloat(20, -18)`, `WaitUntil(… ProcessCommands() …)`.
3. `Setpoint_OutOfRange_IsRejected` – `[InlineData(5f)]`, `[InlineData(-40f)]`, `[InlineData(float.NaN)]`,
   Register 18 = 2, alter Wert bleibt.
4. `Door_OpenRaisesTemperature_AndCompressorEnergy` – zwei Modelle, eines mit offener Tür, nach 2 h vergleichen.
5. `CompressorFailure_SetsFaultAndErrorCode` / `ResetOnlyWhenCooledDown` – `TriggerFault`, `ResetFault` bei
   warmer und kalter Kammer.
6. `Registers_StayReadable_WhilePaused` – `Time.TogglePause()`, `SimulationRunning = 0`, Modellzeit stimmt.
7. `RegisterPlanRequirementTests` erweitern: `KuehlhausRegisters.Plan` in das Array von
   `EveryDevicePlan_HasSetpointAndAtLeastThreeSpecificValues` aufnehmen und
   `Assert.Equal(plan, RegisterPlans.For(plan.DeviceType))` prüft dann auch die Registrierung.
8. `ArgsParserTests` – Defaults, `--standalone`-Regeln, ungültige Werte.
9. `Engine_RunsStandalone_WritesCsvAndStopsOnCancel` – wie `Pv_RunsStandalone_…`, mit `--no-modbus`,
   `--weather off`, `--speed 20000`, 2 s Laufzeit, prüfen: Kopfzeile, Startzeile bei `--start`, `Lauf`-Spalte.
10. Wetter: `LocalOutsideTemperature` ist bei gleicher Zeit gleich (Determinismus) und hat Minimum am Morgen.

**Wichtig:** Modbus-Schreibzugriffe werden von der Bibliothek bestätigt, **bevor** die Anwendung sie sieht –
Tests deshalb immer mit `WaitUntil(() => { _modbus.ProcessCommands(); return …; })` und nicht mit `Thread.Sleep`.

---

## Schritt 9 – Doku und Journal

1. `src/Devices/KuehlhausSimulator/README.md`: Zweck, Verantwortliche/r, Basis aus LB01, Start und Optionen, Bedienung,
   Tasten, Modellformeln (inkl. Exponentiallösung und COP), CSV-Format, **Registerplan** (aus `--register-plan`
   übernehmen), Schnittstellen (Master, Wetter, Modbus), Tests, bekannte Probleme.
2. `README.md` (zentral): Gerät in die Architekturtabelle, Startreihenfolge, Zuständigkeiten, Testliste.
3. `AGENTS.md`: Stand-Kopf, `§0.3` (Zeile für das Gerät), `§0.4` (Punkt 5: Wind ist nicht mehr das einzige
   offene vierte Gerät), `§3` (Struktur), `§6` (Build-Status).
4. `docs/development-journal/<eigene Person>.md`: ein Tageseintrag mit `**Done:**`, `**KI:**`, `**Artefact:**`,
   `**Test:**`, `**Comment:**` – **nur die eigene Datei** bearbeiten.
5. `docs/devices/KuehlhausSimulator/`: optional `design-notes.md` mit Herleitung von τ und COP.

---

## Abnahme (Probeläufe)

```bash
dotnet build EnergyTwin.sln                                                     # 0 Fehler, 0 Warnungen
dotnet test tests/FieldDevices.Tests                                            # alle grün

# 1. Master + Wetter + Kühlhaus + Client
dotnet run --project src/SimulationMaster -- --speed 600
dotnet run --project src/WeatherSimulator
dotnet run --project src/Devices/KuehlhausSimulator -- --id KH-01                      # Modbus 1505
dotnet run --project src/ModbusDiagnosticClient -- --port 1505                  # read, write Solltemperatur -18

# 2. Standalone ohne Wetter
dotnet run --project src/Devices/KuehlhausSimulator -- --standalone --speed 6000 --duration 24 --weather off --seed 42
```

Abzunehmen ist:

- Zeit im Kühlhaus und im Master gleich (Abweichung ≤ 10 ms · Tempo, Anzeige „Abweichung ≤ …“).
- Pause im Master: Register bleiben lesbar, `Simulation läuft` = 0, Modellzeit steht, Kompressorleistung letzter Stand.
- Neustart im Master: neue `Lauf`-Nummer, Modellzeit springt, Zustand (Raumtemperatur, Zähler) bleibt.
- `write Solltemperatur -18` → Anzeige im Kühlhaus, Register 20–21 und 18/19 stimmen; `write Solltemperatur 5`
  wird abgelehnt (Register 18 = 2, alter Wert bleibt).
- Wettersimulator beendet → sichtbare Meldung nach 3 Fehlversuchen, letzte Außentemperatur bleibt.
- `Q`/Strg+C: geordnetes Ende, CSV-Pfad wird ausgegeben.

---

## Fallstricke aus diesem Repo (schon einmal aufgetreten)

- **Portprüfung:** `ModbusDeviceServer.Start()` prüft den Port selbst; belegt ⇒ `IOException`. In `Program.cs`
  abfangen, sonst wirkt es wie ein Absturz. Zwei Instanzen brauchen zwei Ports (`--modbus-port`).
- **Schreibantwort:** Die Bibliothek antwortet vor der Anwendung; Register 18/19 sind die einzige Fehlermeldung.
- **`switch`-Ausdrücke mit gemischten Typen** rechnen in `float` – in `RegisterCodec.Decode` ist jeder Zweig
  `double`. Beim Dekodieren im Client nichts selbst umbauen.
- **PowerShell:** `Get-Content`/`Set-Content` zerstört UTF-8 ohne BOM. Dateien mit dem Editor-Werkzeug oder
  Python `newline=''` bearbeiten; die LB01-Dateien haben teils BOM/CRLF.
- **Umgeleitete Ausgabe** ≠ echtes Terminal: `Console.SetCursorPosition` braucht den Fallback, sonst endet das
  Programm beim ersten Frame. Interaktive Anzeige und Tasten muss die Userin in einem echten Fenster prüfen.
- **Kein `Thread.Sleep`, kein `ReadLine`** in der Schleife; `Task.Delay(PollInterval, ct)` und `AsyncConsole`.
- **`dotnet build` mit 0 Warnungen** (`TreatWarningsAsErrors`) – unbenutzte Felder fallen sofort auf.
