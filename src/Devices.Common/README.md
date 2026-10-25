# Devices.Common

**Zweck:** Gemeinsame Bausteine aller Feldgeräte. Bisher (2026-10-25): Anbindung an die gemeinsame virtuelle Zeit,
feste Modellschritte, Konsoleneingabe ohne blockierendes Warten. Geplant: Modbus-Server-Hosting, Thermik/Derating,
Batteriemodell, Energiezähler, CSV, Konsolen-UI.
**Verantwortlich:** _offen_. Zeitanbindung am 2026-10-25 von Susanne Lastowicka umgesetzt.
**Basis aus LB01:** Susi Pausen-/Tempo-Tasten und Prompt (jetzt async), Domi Zeitkonto mit festen Schritten (jetzt `StepScheduler`).
**Abhängigkeiten:** `SimCore.Contracts`, `SimCore.Time`, `SimCore.TimeSync`, `Modbus.Contracts`, `Shared.Config`, EasyModbusTCP.NETCore.

## Bausteine

| Klasse | Aufgabe |
|---|---|
| `TimeSyncOptions` | Gemeinsame Startoptionen `--master host:port`, `--standalone`, `--start`, `--speed` (`TryApply` für den eigenen `ArgsParser`, Hilfetext `Usage`) |
| `DeviceTimeSource` | Zeitquelle des Geräts: mit Master `SyncedClock` + `TimeSyncClient`, standalone eine lokale `MasterClock`. Liefert `Clock`, `HasTime`, `IsRunning`, `StatusText`, `Log`; `TogglePause`/`ChangeSpeed` nur standalone |
| `StepScheduler` | Rechnet aus der Uhrzeit die fälligen festen Schritte (`Update(now, runId)`), erkennt Neustarts, holt verpasste Schritte nach (höchstens 10 000 auf einmal, Rest wird gemeldet übersprungen) |
| `AsyncConsole` | `TryReadKey()` ohne Warten, `ReadLineAsync(max, ct)` mit Echo, Rücktaste und Esc; bei umgeleiteter Eingabe `Console.In.ReadLineAsync` |

## Verwendung in einem Gerät (Muster: `src/Devices/DualPortCharger/Simulation/SimulationEngine.cs`)

```csharp
var time = DeviceTimeSource.Create(config.TimeSync, deviceId: "BAT-01", deviceKind: "Battery");
time.Log += (_, m) => message = m;
var scheduler = new StepScheduler(TimeSpan.FromSeconds(60));
var syncTask = time.RunAsync(ct);                     // Verbindung zum Master, verbindet sich selbst neu

while (!ct.IsCancellationRequested)
{
    // Tasten: AsyncConsole.TryReadKey(); P und </> an time.TogglePause()/time.ChangeSpeed() weitergeben
    if (time.HasTime)
    {
        var u = scheduler.Update(time.Clock.Now, time.Clock.RunId);
        if (u.Kind == StepUpdateKind.Restarted) { /* Neustart melden */ }
        for (int i = 0; i < u.StepCount; i++)
            device.Step(u.FirstStepAt + scheduler.Step * i, scheduler.Step);
        // u.ModelTime = Zeit des dargestellten Zustands (für Anzeige, CSV, später Modbus-Kopf)
    }
    // Anzeige mit time.StatusText
    await Task.Delay(50, ct);
}
await syncTask;                                        // meldet sich beim Master ab
```

Regeln:
- Das Gerät zählt die Zeit **nie selbst** hoch und wartet nie `Schritt / Tempo` ab. Es fragt die Uhr und rechnet die fälligen Schritte.
  Dadurch bleibt es bei jedem Tempo synchron, auch wenn die Schleife einmal langsamer ist.
- Pause und Tempo bestimmt der Master. Die Geräte-Tasten dafür wirken nur standalone.
- Bei `MasterLost` steht die Uhr, `StepCount` ist dann 0. Das Gerät rechnet nicht weiter, die Anzeige (und später Modbus) bleibt erreichbar.
- Standalone: Die erste Modellzeit ist genau `--start` (siehe Charger).

## Konfiguration
Über die Kommandozeile des jeweiligen Geräts (`TimeSyncOptions.Usage`):

| Option | Standard | Bedeutung |
|---|---|---|
| `--master <host:port>` | `127.0.0.1:5100` | Simulations-Master |
| `--standalone` | aus | ohne Master, lokale Uhr |
| `--start <zeit>` | heute 00:00 | nur standalone |
| `--speed <x>` | je Gerät (Charger 120) | nur standalone, 1–100 000 |

## Tests
`tests/Devices.Common.Tests` (24 Tests, alle grün am 2026-10-25).

## Bekannte Probleme
- Modbus-Bausteine fehlen noch.
- PV- und Batteriesimulator verwenden die Zeitanbindung noch nicht (enthalten noch `Thread.Sleep`/`ReadLine`).
