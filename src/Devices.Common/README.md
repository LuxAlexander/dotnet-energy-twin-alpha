# Devices.Common

**Zweck:** Gemeinsame Bausteine aller Feldgeräte (Charger, PV, Batterie nutzen sie seit 2026-10-25). Enthalten: Anbindung an die gemeinsame virtuelle Zeit,
feste Modellschritte, Konsoleneingabe ohne blockierendes Warten, Modbus-Server-Hosting (Registerkopf, Schreibprüfung) und seit 2026-10-26 der Wetterbezug
(HTTP-Client, Stunden-Cache, Fehlerbehandlung). Geplant: Thermik/Derating, Energiezähler, CSV, Konsolen-UI.
**Verantwortlich:** _offen_. Zeitanbindung am 2026-10-25 von Susanne Lastowicka umgesetzt.
**Basis aus LB01:** Susi Pausen-/Tempo-Tasten und Prompt (jetzt async), Domi Zeitkonto mit festen Schritten (jetzt `StepScheduler`).
**Abhängigkeiten:** `SimCore.Contracts`, `SimCore.Time`, `SimCore.TimeSync`, `Modbus.Contracts`, `Shared.Config`, EasyModbusTCP.NETCore.

## Bausteine

| Klasse | Aufgabe |
|---|---|
| `TimeSyncOptions` | Gemeinsame Startoptionen `--master host:port`, `--standalone`, `--start`, `--speed` (`TryApply` für den eigenen `ArgsParser`, Hilfetext `Usage`) |
| `DeviceTimeSource` | Zeitquelle des Geräts: mit Master `SyncedClock` + `TimeSyncClient`, standalone eine lokale `MasterClock`. Liefert `Clock`, `HasTime`, `IsRunning`, `StatusText`, `Log`; `TogglePause`/`ChangeSpeed`/`SetSpeed` nur standalone |
| `StepScheduler` | Rechnet aus der Uhrzeit die fälligen festen Schritte (`Update(now, runId)`), erkennt Neustarts, holt verpasste Schritte nach (höchstens 10 000 auf einmal, Rest wird gemeldet übersprungen) |
| `Modbus.ModbusOptions` | Gemeinsame Startoptionen `--modbus-port`, `--modbus-bind`, `--unit-id`, `--no-modbus`, `--register-plan` (`TryApply`, `Usage`) |
| `Modbus.ModbusDeviceServer` | Modbus-TCP-Server eines Geräts auf Basis eines `RegisterPlan`: `Start()` (prüft vorher, ob der Port frei ist, sonst `IOException`), `Publish(header, time, specific)` schreibt Kopf und gerätespezifische Register, `TryDequeueCommand` liefert angenommene Schreibzugriffe, `StatusText`, `Log`, `Dispose` |
| `Modbus.ModbusDeviceServer.ValueValidator` | Optionale gerätespezifische Prüfung für Werte, deren Grenzen nicht im Registerplan stehen können (z. B. Sollwert innerhalb der konfigurierten Leistung der Batterie) |
| `Modbus.HeaderValues`, `RegisterWriter`, `ModbusCommand` | Werte des Registerkopfs, Schreiber für gerätespezifische Register (prüft Adresse und Typ), angenommener Schreibzugriff |
| `AsyncConsole` | `TryReadKey()` ohne Warten, `ReadLineAsync(max, ct)` mit Echo, Rücktaste und Esc; bei umgeleiteter Eingabe `Console.In.ReadLineAsync` |
| `Weather.WeatherOptions` | Startoptionen `--weather <url\|off>`, `--location <ort>` (`TryApply`, `Usage`); `WithDefaultUrl(standalone)` setzt die Standard-URL nur mit Master |
| `Weather.WeatherClient` | HTTP-Client auf den Wettersimulator, implementiert `IWeatherService`; 5-s-Timeout, deutsche Fehler in `WeatherUnavailableException` |
| `Weather.WeatherFeed` | Wetter je Modellstunde: fragt höchstens einmal pro Stunde, Retry nach 10 s, behält das letzte Wetter, meldet nach 3 Fehlversuchen einmalig sichtbar |

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

## Modbus im Gerät (Muster: `src/Devices/DualPortCharger/Simulation/ChargerModbus.cs`; weitere: `PvModbus`, `BatteryModbus`)

1. `RegisterPlan` des Geräts anlegen (Kopf aus `CommonRegisters`, eigene Register ab Adresse 20; `RegisterPlan.ToMarkdown()` erzeugt die Tabelle für die README).
2. `new ModbusDeviceServer(options, plan, deviceId)`, `Start()` **vor** Konsole und CSV (Port belegt ⇒ `IOException` ⇒ Fehlermeldung, Exit-Code 1).
3. In **jedem** Durchlauf der Gerätschleife, auch in der Pause und bei `MasterLost`:
   `TryDequeueCommand` bis leer (Befehle im Simulationsthread auf das Modell anwenden), dann `Publish(...)` mit dem aktuellen Zustand
   (`ModelTime` = Zeit des zuletzt berechneten Schritts). Die Sequenznummer wächst nur mit neuer Modellzeit.
4. Sollwerte, die zurückgelesen werden sollen, im `specific`-Callback mit veröffentlichen.
5. Beim Beenden `Dispose()`.

**Timing:** Die Bibliothek antwortet auf einen Schreibzugriff, bevor die Anwendung ihn sieht; angenommene Befehle kommen deshalb erst im nächsten Schleifendurchlauf
(Tests warten darauf, der Diagnose-Client wartet 200 ms).

Die Prüfung der Schreibzugriffe (Adresse, Bereich, Typ, unvollständige Floats) übernimmt der Server anhand des Registerplans; das Gerät
bekommt nur gültige Werte. Ungültiges wird zurückgesetzt und in den Registern 18/19 vermerkt, weil die Bibliothek erst nach dem Schreiben
meldet (Details im Charger-README). Der Server läuft in Threads der Bibliothek, die Modelllogik bleibt einthreadig.

## Wetter im Gerät (Muster: `src/Devices/BatterySimulator/Simulator.cs`; ebenso Charger, PV)

```csharp
var feed = new WeatherFeed(new WeatherClient(url), location, message => Message = message);
// je Modellschritt vor der Physik:
if (await feed.UpdateAsync(stepStart, runId, ct) is { } weather)
    device.AmbientTemperatureC = weather.TemperatureC;
```

- Die Abfrage erfolgt je Modellstunde, auch wenn die Schrittweite kleiner ist (`WeatherFeed` cached).
- `--weather off` (oder standalone ohne Angabe) ⇒ kein HTTP-Verkehr, das Gerät rechnet mit festen 20 °C.
- Ausfall: letztes Wetter bleibt aktiv; nach 3 Fehlversuchen erscheint „WETTER: NICHT ERREICHBAR“ in der Meldungszeile, neue Versuche alle 10 s.
- Der Client gehört nach `Devices.Common`, nicht in `SimCore.Contracts` (Contracts bleibt abhängigkeitsfrei).

## Konfiguration
Über die Kommandozeile des jeweiligen Geräts (`TimeSyncOptions.Usage`):

| Option | Standard | Bedeutung |
|---|---|---|
| `--master <host:port>` | `127.0.0.1:5100` | Simulations-Master |
| `--standalone` | aus | ohne Master, lokale Uhr |
| `--start <zeit>` | heute 00:00 | nur standalone |
| `--speed <x>` | je Gerät (Charger 120) | nur standalone, 1–100 000 |
| `--modbus-port <port>` | 1502 | Modbus-TCP-Port, 1024–65535, pro Gerät eigener |
| `--modbus-bind <ip>` | 127.0.0.1 | Adresse des Servers (`0.0.0.0` = alle) |
| `--unit-id <n>` | 1 | Modbus-Unit-ID, 1–247 |
| `--no-modbus` | aus | Server nicht starten |
| `--register-plan` | | Registerplan ausgeben und beenden (im Gerät auszuwerten) |
| `--weather <url\|off>` | mit Master `http://127.0.0.1:8080`, standalone aus | Wettersimulator; `off` = feste 20 °C |
| `--location <ort>` | salzburg | Standort für die Wetterabfrage |

## Tests
`tests/Devices.Common.Tests` (51 Tests, alle grün am 2026-10-26; neu: `WeatherFeed` und `WeatherOptions`).

## Bekannte Probleme
- Modbus: Bibliotheksgrenze, Schreibfehler werden nicht als Modbus-Ausnahme gemeldet; die Bibliothek bindet den Port in einem eigenen Thread, deshalb prüft `Start()` vorher selbst (kleines Zeitfenster zwischen Prüfung und Bindung).
