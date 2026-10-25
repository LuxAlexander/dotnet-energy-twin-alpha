using System.Globalization;
using Devices.Common;
using EnergySimulator.Data;
using EnergySimulator.Devices;
using EnergySimulator.Output;

namespace EnergySimulator.Simulation;

/// <summary>
/// Hauptschleife der PV-Anlage: holt die virtuelle Zeit von der Zeitquelle (Master oder lokal), rechnet die fälligen
/// festen Modellschritte, schreibt CSV und Modbus-Register, zeichnet die Anzeige und verarbeitet Tasten –
/// ohne blockierendes Warten. Die Modbus-Register bleiben auch in der Pause und bei verlorenem Master aktuell erreichbar.
/// </summary>
public sealed class SimulationEngine
{
    private const int TickMs = 80;
    private static readonly TimeSpan RedirectedInterval = TimeSpan.FromSeconds(5);
    private static readonly int[] RefreshSteps = [100, 200, 400, 800, 1500, 3000];
    private static readonly double[] SpeedKeys = [1, 15, 60, 300, 1800];
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

    /// <summary>Mittlere Dauer bis zum nächsten Wetterwechsel im Auto-Modus (Modellzeit).</summary>
    private static readonly TimeSpan MeanWeatherDuration = TimeSpan.FromMinutes(20);

    private sealed record Note(string Text, DateTime At);

    private readonly SimulationConfig _config;
    private readonly PvSystem _pv;
    private readonly PvFaultModel _faults;
    private readonly Random _random;
    private readonly StepScheduler _scheduler;
    private readonly PvModbus? _modbus;

    private SimulationMode _mode = SimulationMode.Auto;
    private bool _running = true;
    private DateTime _currentDay;
    private DateTime _lastCsvAt;
    private Note _note = new("", DateTime.MinValue);

    public PvSystem Pv => _pv;
    public DeviceTimeSource Time { get; }

    /// <summary>Virtuelle Zeit des dargestellten Zustands (nach dem letzten Schritt); <c>null</c> bis zur ersten Zeit.</summary>
    public DateTime? SimulationTime { get; private set; }

    public int RunId => _scheduler.RunId;
    public SimulationMode Mode => _mode;

    public SimulationEngine(SimulationConfig config)
    {
        _config = config;
        _random = config.Seed is int seed ? new Random(seed) : new Random();
        _faults = new PvFaultModel(_random);
        _pv = new PvSystem(config.PeakPowerKw, _faults);
        _scheduler = new StepScheduler(config.ModelStep);

        Time = DeviceTimeSource.Create(config.TimeSync, config.DeviceId, "PV");
        Time.Log += (_, message) => SetNotification(message);

        if (!config.Modbus.Disabled)
            _modbus = new PvModbus(_pv, _faults, Time, config.Modbus, config.DeviceId, SetNotification);
    }

    public string? ModbusStatus => _modbus?.StatusText;

    /// <returns>Pfad der geschriebenen CSV-Datei.</returns>
    public async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        _modbus?.Start();                                         // Port belegt → IOException, bevor etwas angelegt wird
        await using var csv = await CsvExporter.CreateAsync(_config.DeviceId, _config.OutputDirectory, cancellationToken);
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var syncTask = Time.RunAsync(stop.Token);

        SetNotification(Time.IsStandalone
            ? "Standalone: lokale Uhr, ohne Simulations-Master."
            : $"Verbinde mit Simulations-Master {_config.TimeSync.MasterEndpoint} …");

        bool interactive = !Console.IsOutputRedirected;
        var nextLog = DateTime.MinValue;
        var nextRender = DateTime.MinValue;
        try
        {
            while (_running && !stop.IsCancellationRequested)
            {
                bool keyPressed = HandleKeys();
                await AdvanceAsync(csv, stop.Token);

                // Modbus läuft auch in der Pause weiter: Befehle umsetzen, Zustand veröffentlichen
                _modbus?.ProcessCommands();
                _modbus?.Publish(SimulationTime, RunId);

                var now = DateTime.UtcNow;
                if (interactive)
                {
                    if (keyPressed || now >= nextRender)
                    {
                        Draw();
                        nextRender = now.AddMilliseconds(_config.RefreshMs);
                    }
                }
                else if (now >= nextLog)
                {
                    await Console.Out.WriteLineAsync(StatusLine());
                    nextLog = now + RedirectedInterval;
                }

                await Task.Delay(TickMs, stop.Token);
            }
        }
        catch (OperationCanceledException) when (stop.IsCancellationRequested)
        {
            // Strg+C: geordnet beenden
        }
        finally
        {
            await stop.CancelAsync();
            _modbus?.Dispose();
            await syncTask;                                       // meldet sich beim Master ab
            if (SimulationTime is { } last)
                await csv.WriteRowAsync(last, _mode, RunId, _pv, CancellationToken.None);
            await csv.FlushAsync(CancellationToken.None);
        }

        return csv.FilePath;
    }

    /// <summary>Rechnet alle fälligen Schritte bis zur aktuellen virtuellen Zeit.</summary>
    private async Task AdvanceAsync(CsvExporter csv, CancellationToken cancellationToken)
    {
        if (!Time.HasTime)
            return;

        // Standalone beginnt genau bei --start (die lokale Uhr läuft seit dem Programmstart schon)
        var clockNow = _scheduler.ModelTime is null && Time.IsStandalone ? _config.TimeSync.StandaloneStart : Time.Clock.Now;
        var update = _scheduler.Update(clockNow, Time.Clock.RunId);

        switch (update.Kind)
        {
            case StepUpdateKind.Started:
            case StepUpdateKind.Restarted:
                if (update.Kind == StepUpdateKind.Restarted)
                {
                    SetNotification($"Neustart durch den Master: Lauf {RunId} ab {update.ModelTime.ToString("dd.MM.yyyy HH:mm", Culture)}.");
                    if (update.ModelTime.Date != _currentDay)
                        _pv.ResetDailyEnergy();
                }

                SimulationTime = update.ModelTime;
                _currentDay = update.ModelTime.Date;
                _lastCsvAt = update.ModelTime;
                if (_mode == SimulationMode.Auto)
                    UpdateAutoWeather(update.ModelTime, TimeSpan.Zero);
                _pv.StepContinuous(update.ModelTime, 0.0);        // Anzeigewerte zur Startzeit, ohne Zeitfortschritt
                await csv.WriteRowAsync(update.ModelTime, _mode, RunId, _pv, cancellationToken);
                return;
        }

        if (update.SkippedSteps > 0)
            SetNotification($"WARNUNG: {update.SkippedSteps} Schritte übersprungen (zu viele auf einmal fällig).");

        var step = _config.ModelStep;
        for (int i = 0; i < update.StepCount; i++)
        {
            var stepEnd = update.FirstStepAt + step * (i + 1);
            if (_mode == SimulationMode.Auto)
                UpdateAutoWeather(stepEnd, step);

            _pv.StepContinuous(stepEnd, step.TotalHours);
            SimulationTime = stepEnd;

            if (stepEnd.Date != _currentDay)
            {
                SetNotification($"Neuer Tag · {stepEnd:dd.MM.yyyy} · {_pv.DailyEnergyKwh.ToString("F1", Culture)} kWh Vortag");
                await csv.WriteRowAsync(stepEnd, _mode, RunId, _pv, cancellationToken);
                _pv.ResetDailyEnergy();
                _currentDay = stepEnd.Date;
                _lastCsvAt = stepEnd;
            }
            else if (stepEnd - _lastCsvAt >= TimeSpan.FromMinutes(_config.StepMinutes))
            {
                await csv.WriteRowAsync(stepEnd, _mode, RunId, _pv, cancellationToken);
                _lastCsvAt = stepEnd;
            }
        }

        if (update.StepCount > 0)
            await csv.FlushAsync(cancellationToken);
    }

    /// <summary>Verarbeitet alle anstehenden Tasten ohne zu warten.</summary>
    /// <returns><c>true</c>, wenn eine Taste gedrückt wurde (Anzeige sofort neu zeichnen).</returns>
    private bool HandleKeys()
    {
        bool any = false;
        while (AsyncConsole.TryReadKey() is { } key)
        {
            any = true;
            HandleKey(key.Key);
        }
        return any;
    }

    private void HandleKey(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.P: SetNotification(Time.TogglePause()); break;
            case ConsoleKey.M: ToggleMode(); break;
            case ConsoleKey.Q: _running = false; SetNotification("Beendet"); break;
            case ConsoleKey.F: ToggleFault(); break;
            case ConsoleKey.T: ToggleOverheat(); break;
            case ConsoleKey.G: ToggleGrid(); break;
            case ConsoleKey.C: CleanModules(); break;
            case ConsoleKey.R: CycleRefresh(); break;

            case ConsoleKey.D0 or ConsoleKey.NumPad0: SetWeather(WeatherCondition.Night); break;
            case ConsoleKey.D1 or ConsoleKey.NumPad1: SetWeather(WeatherCondition.Sunny); break;
            case ConsoleKey.D2 or ConsoleKey.NumPad2: SetWeather(WeatherCondition.PartlyCloudy); break;
            case ConsoleKey.D3 or ConsoleKey.NumPad3: SetWeather(WeatherCondition.Overcast); break;
            case ConsoleKey.D4 or ConsoleKey.NumPad4: SetWeather(WeatherCondition.Storm); break;

            case >= ConsoleKey.D5 and <= ConsoleKey.D9:
                SetNotification(Time.SetSpeed(SpeedKeys[key - ConsoleKey.D5]));
                break;
            case >= ConsoleKey.NumPad5 and <= ConsoleKey.NumPad9:
                SetNotification(Time.SetSpeed(SpeedKeys[key - ConsoleKey.NumPad5]));
                break;
        }
    }

    private void CycleRefresh()
    {
        int index = Array.IndexOf(RefreshSteps, _config.RefreshMs);
        if (index < 0)
            index = 2;

        _config.RefreshMs = RefreshSteps[(index + 1) % RefreshSteps.Length];
        SetNotification($"Bild-Aktualisierung → {_config.RefreshMs} ms");
    }

    private void ToggleMode()
    {
        _mode = _mode == SimulationMode.Auto ? SimulationMode.Manual : SimulationMode.Auto;
        SetNotification($"Modus → {(_mode == SimulationMode.Auto ? "AUTO" : "MANUELL")}");
    }

    private void SetWeather(WeatherCondition weather)
    {
        _pv.SetWeather(weather);
        SetNotification($"Wetter → {weather.ToGerman()}");
    }

    private void ToggleFault()
    {
        _faults.ToggleInverterFault();
        SetNotification(_faults.InverterFault ? "Störung → Inverterfehler ausgelöst" : "Störung → Inverterfehler zurückgesetzt");
    }

    private void ToggleOverheat()
    {
        _faults.ToggleForceOverheat();
        SetNotification(_faults.OverheatForced ? "Störung → Überhitzung erzwungen" : "Störung → Überhitzung aus");
    }

    private void ToggleGrid()
    {
        _faults.ToggleGridDisturbance();
        SetNotification(_faults.GridDisturbanceForced ? "Störung → Netzspannungsstörung aktiv" : "Störung → Netzspannung normal");
    }

    private void CleanModules()
    {
        _faults.CleanModules();
        SetNotification("Wartung → Module gereinigt, Verschmutzung zurückgesetzt");
    }

    /// <summary>
    /// Auto-Wetter: nachts Nacht, tagsüber zufällige Wechsel mit mittlerer Dauer <see cref="MeanWeatherDuration"/> (Modellzeit),
    /// unabhängig von Tempo und Schrittweite. Mit <c>--seed</c> wiederholbar.
    /// </summary>
    private void UpdateAutoWeather(DateTime time, TimeSpan step)
    {
        Season season = SeasonInfo.FromMonth(time.Month);
        double hour = time.Hour + time.Minute / 60.0 + time.Second / 3600.0;

        if (!IrradiationCalculator.IsDaylight(hour, season))
        {
            if (!_pv.IsNight)
                _pv.SetWeather(WeatherCondition.Night);
            return;
        }

        if (_pv.IsNight)
            _pv.SetWeather(RandomWeather(season));

        double changeProbability = 1.0 - Math.Exp(-step / MeanWeatherDuration);
        if (_random.NextDouble() < changeProbability)
        {
            var next = RandomWeather(season);
            _pv.SetWeather(next);
            if (next == WeatherCondition.Snow)
                SetNotification("Wetter → Schneefall");
        }
    }

    private WeatherCondition RandomWeather(Season season)
    {
        if (_random.NextDouble() < season.SnowProbability())
            return WeatherCondition.Snow;

        WeatherCondition[] options = [WeatherCondition.Sunny, WeatherCondition.PartlyCloudy, WeatherCondition.Overcast, WeatherCondition.Storm];
        return options[_random.Next(options.Length)];
    }

    private void Draw()
    {
        var note = _note;
        string notification = (DateTime.Now - note.At).TotalSeconds > 4 ? "" : note.Text;
        var time = SimulationTime;

        ConsoleDisplay.Render(
            time,
            SeasonInfo.FromMonth((time ?? _config.TimeSync.StandaloneStart).Month),
            Time.IsRunning ? _mode : SimulationMode.Paused,
            _pv,
            _config.PeakPowerKw,
            Time.HasTime ? Time.Clock.Speed : 0,
            notification,
            time?.TimeOfDay.TotalHours / 24.0 ?? 0.0,
            Time.StatusText,
            _modbus?.StatusText);
    }

    /// <summary>Kurzfassung für umgeleitete Ausgabe (Protokoll statt Bildschirm).</summary>
    private string StatusLine()
    {
        string time = SimulationTime?.ToString("yyyy-MM-dd HH:mm", Culture) ?? "--";
        return FormattableString.Invariant($"{time}  Lauf {RunId}  {_pv.PowerKw:F2} kW  {_pv.TotalEnergyKwh:F2} kWh  {_pv.Weather.ToGerman()}  | {Time.StatusText}  | Modbus {_modbus?.StatusText ?? "aus"}  | {_note.Text}");
    }

    private void SetNotification(string message) => _note = new Note(message, DateTime.Now);
}
