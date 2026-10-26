using System.Globalization;
using Devices.Common;
using Devices.Common.Weather;
using DualChargerSim.IO;
using DualChargerSim.Models;

namespace DualChargerSim.Simulation;

/// <summary>
/// Hauptschleife: holt die virtuelle Zeit von der Zeitquelle (Master oder lokal), rechnet die fälligen festen
/// Schritte, schreibt die CSV, zeichnet die Anzeige und verarbeitet Tasten – alles ohne blockierendes Warten.
/// </summary>
public class SimulationEngine
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);
    private static readonly TimeSpan RenderInterval = TimeSpan.FromMilliseconds(200);
    private static readonly TimeSpan RedirectedRenderInterval = TimeSpan.FromSeconds(5);
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

    private readonly AutoModeController _autoMode;
    private readonly ConsoleRenderer _renderer;
    private readonly InputHandler _input;
    private readonly StepScheduler _scheduler;
    private readonly Random _random;
    private readonly ChargerModbus? _modbus;
    private readonly WeatherFeed? _weather;
    private DateTime? _endTime;

    public SimulationConfig Config { get; }
    public DualCharger Charger { get; }
    public DeviceTimeSource Time { get; }

    /// <summary>Zustand des Modbus-Servers für die Anzeige; <c>null</c> mit <c>--no-modbus</c>.</summary>
    public string? ModbusStatus => _modbus?.StatusText;

    /// <summary>Status des Wetterbezugs; <c>null</c> ohne Wetter (feste 20 °C).</summary>
    public string? WeatherStatus => _weather?.Status;

    /// <summary>Virtuelle Zeit des dargestellten Zustands (nach dem letzten Schritt); <c>null</c> bis zur ersten Zeit.</summary>
    public DateTime? SimulationTime { get; private set; }

    /// <summary>Lauf (Neustart im Master), zu dem <see cref="SimulationTime"/> gehört.</summary>
    public int RunId => _scheduler.RunId;

    public bool IsAutoMode { get; set; } = true;
    public bool IsRunning { get; private set; } = true;
    public bool IsFinished => SimulationTime >= _endTime;
    public DateTime? EndTime => _endTime;
    public int SelectedPointIndex { get; set; }
    public ChargingPoint SelectedPoint => Charger.Points[SelectedPointIndex];

    /// <summary>Letzte Meldung; kann auch aus der Zeitsynchronisation (anderer Thread) gesetzt werden.</summary>
    public string Message { get; set; } = "";

    public SimulationEngine(SimulationConfig config)
    {
        Config = config;
        _random = config.Seed is int seed ? new Random(seed) : new Random();

        Charger = new DualCharger(config.DeviceId, config.PowerLimitKW);
        _autoMode = new AutoModeController(Charger, _random);
        _renderer = new ConsoleRenderer();
        _input = new InputHandler(this, _renderer);
        _scheduler = new StepScheduler(config.Step);

        Time = DeviceTimeSource.Create(config.TimeSync, config.DeviceId, "Charger");
        Time.Log += (_, message) => Message = message;

        if (!config.Modbus.Disabled)
            _modbus = new ChargerModbus(Charger, Time, config.Modbus, message => Message = message);

        if (config.Weather.Url is { } weatherUrl)
            _weather = new WeatherFeed(new WeatherClient(weatherUrl), config.Weather.Location, message => Message = message);
    }

    public void Quit() => IsRunning = false;

    /// <summary>
    /// Manueller Modus: zufälliges Fahrzeug kommt, steckt am freien Punkt an und lädt sofort.
    /// Abgesteckt wird von Hand (D).
    /// </summary>
    public void VehicleArrives()
    {
        if (SimulationTime is not { } now)
        {
            Message = "Noch keine virtuelle Zeit – warte auf den Simulations-Master.";
            return;
        }

        var vehicle = VehicleCatalog.CreateRandom(_random);
        Charger.Enqueue(vehicle, now);
        var point = Charger.Points.FirstOrDefault(p => p.Vehicle == vehicle);

        Message = point is null
            ? $"{vehicle.ModelName} wartet – beide Punkte belegt (fährt nach {DualCharger.MaxQueueWait.TotalMinutes:0} min weg)."
            : $"Punkt {point.Number}: {vehicle.ModelName} angesteckt, SOC {vehicle.SocPercent:0} % → {vehicle.TargetSocPercent:0} % – D zum Abstecken.";
    }

    /// <returns>Pfad der geschriebenen CSV-Datei.</returns>
    public async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        _modbus?.Start();                                         // Port belegt → IOException, bevor etwas angelegt wird
        await using var logger = await CsvLogger.CreateAsync(Config.DeviceId, Config.OutputDirectory, cancellationToken);
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var syncTask = Time.RunAsync(stop.Token);

        Message = Time.IsStandalone
            ? Config.IsShortRun
                ? $"WARNUNG: Dauer {Config.DurationHours} h < 24 h – für die Abgabe einen Lauf ≥ 24 h verwenden."
                : "Standalone: lokale Uhr, ohne Simulations-Master."
            : $"Verbinde mit Simulations-Master {Config.TimeSync.MasterEndpoint} …";

        await _renderer.InitializeAsync();
        try
        {
            var interval = ConsoleRenderer.IsInteractive ? RenderInterval : RedirectedRenderInterval;
            var nextRender = DateTime.MinValue;
            while (IsRunning && !stop.IsCancellationRequested && !IsFinished)
            {
                bool changed = await _input.HandlePendingKeysAsync(stop.Token);
                changed |= await AdvanceAsync(logger, stop.Token);

                // Modbus läuft auch in der Pause und bei verlorenem Master weiter: Befehle umsetzen, Zustand veröffentlichen
                if (_modbus is not null)
                {
                    _modbus.ProcessCommands();
                    _modbus.Publish(SimulationTime, RunId);
                }

                // Interaktiv sofort neu zeichnen; umgeleitet (Protokoll) nur im festen Takt.
                if ((changed && ConsoleRenderer.IsInteractive) || DateTime.UtcNow >= nextRender)
                {
                    await _renderer.DrawAsync(this);
                    nextRender = DateTime.UtcNow + interval;
                }
                await Task.Delay(PollInterval, stop.Token);
            }

            if (IsRunning && IsFinished)
            {
                Message = $"Simulation beendet. CSV: {logger.FilePath} – Q zum Beenden.";
                await _renderer.DrawAsync(this);
                await _input.WaitForQuitAsync(stop.Token);
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
            _weather?.Dispose();
            await syncTask;                                       // meldet sich beim Master ab
            await _renderer.ShutdownAsync();
        }
        return logger.FilePath;
    }

    /// <summary>Rechnet alle fälligen Schritte bis zur aktuellen virtuellen Zeit.</summary>
    /// <returns><c>true</c>, wenn sich der Zustand geändert hat.</returns>
    private async Task<bool> AdvanceAsync(CsvLogger logger, CancellationToken cancellationToken)
    {
        if (!Time.HasTime)
            return false;

        // Standalone beginnt genau bei --start (die lokale Uhr läuft seit dem Programmstart schon),
        // die fehlenden Schritte werden beim nächsten Durchlauf nachgeholt.
        var clockNow = _scheduler.ModelTime is null && Time.IsStandalone ? Config.TimeSync.StandaloneStart : Time.Clock.Now;
        var update = _scheduler.Update(clockNow, Time.Clock.RunId);
        switch (update.Kind)
        {
            case StepUpdateKind.Started:
                SimulationTime = update.ModelTime;
                if (Config.Duration is { } duration)
                    _endTime = update.ModelTime + duration;
                await logger.WriteAsync(update.ModelTime, RunId, IsAutoMode, Charger, cancellationToken);
                return true;

            case StepUpdateKind.Restarted:
                SimulationTime = update.ModelTime;
                Message = $"Neustart durch den Master: Lauf {RunId} ab {update.ModelTime.ToString("dd.MM.yyyy HH:mm", Culture)}.";
                await logger.WriteAsync(update.ModelTime, RunId, IsAutoMode, Charger, cancellationToken);
                return true;
        }

        if (update.SkippedSteps > 0)
            Message = $"WARNUNG: {update.SkippedSteps} Schritte übersprungen (zu viele auf einmal fällig).";

        for (int i = 0; i < update.StepCount && !IsFinished; i++)
        {
            var stepStart = update.FirstStepAt + Config.Step * i;
            if (_weather is not null && await _weather.UpdateAsync(stepStart, RunId, cancellationToken) is { } weather)
                Charger.AmbientTemperatureC = weather.TemperatureC;
            if (IsAutoMode)
                _autoMode.Step(stepStart, Config.Step);
            Charger.Tick(stepStart, Config.Step);
            SimulationTime = stepStart + Config.Step;
            await logger.WriteAsync(SimulationTime.Value, RunId, IsAutoMode, Charger, cancellationToken);
        }

        if (update.StepCount > 0)
            await logger.FlushAsync(cancellationToken);
        return update.StepCount > 0;
    }
}
