using System.Globalization;
using Devices.Common;
using Devices.Common.Weather;
using KuehlhausSimulator.IO;

namespace KuehlhausSimulator.Simulation;

/// <summary>
/// Hauptschleife des Kühlhauses: holt die virtuelle Zeit von der Zeitquelle (Master oder lokal), rechnet die fälligen
/// festen Modellschritte mit der Außentemperatur aus dem Wettersimulator, schreibt CSV und Modbus-Register, zeichnet
/// die Anzeige und verarbeitet Tasten – alles ohne blockierendes Warten.
/// Der Modbus-Server bleibt auch in der Pause und bei verlorenem Master erreichbar.
/// </summary>
public sealed class SimulationEngine
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);
    private static readonly TimeSpan RenderInterval = TimeSpan.FromMilliseconds(200);
    private static readonly TimeSpan RedirectedRenderInterval = TimeSpan.FromSeconds(5);
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

    private readonly Kuehlhaus _room;
    private readonly AutoModeController _autoMode;
    private readonly ConsoleRenderer _renderer;
    private readonly InputHandler _input;
    private readonly StepScheduler _scheduler;
    private readonly KuehlhausModbus? _modbus;
    private readonly WeatherFeed? _weather;

    private DateTime? _endTime;

    public SimulationEngine(SimulationConfig config)
    {
        Config = config;
        _room = new Kuehlhaus(config.ToModelConfig());

        var random = config.Seed is int seed ? new Random(seed) : new Random();
        _autoMode = new AutoModeController(_room, random);
        _renderer = new ConsoleRenderer();
        _input = new InputHandler(this, _renderer);
        _scheduler = new StepScheduler(config.Step);

        Time = DeviceTimeSource.Create(config.TimeSync, config.DeviceId, "Kühlhaus");
        Time.Log += (_, message) => Message = message;

        if (!config.Modbus.Disabled)
            _modbus = new KuehlhausModbus(_room, Time, config.Modbus, message => Message = message);

        if (config.Weather.Url is { } weatherUrl)
            _weather = new WeatherFeed(new WeatherClient(weatherUrl), config.Weather.Location, message => Message = message);
    }

    public SimulationConfig Config { get; }

    /// <summary>Das Modell (Temperatur, Kompressor, Energie) für Anzeige und Tests.</summary>
    public Kuehlhaus Room => _room;

    public DeviceTimeSource Time { get; }

    /// <summary>Zustand des Modbus-Servers für die Anzeige; <c>null</c> mit <c>--no-modbus</c>.</summary>
    public string? ModbusStatus => _modbus?.StatusText;

    /// <summary>Zustand der Wetteranbindung für Anzeige und Tests.</summary>
    public string WeatherStatus => _weather?.Status ?? "aus (--weather off), lokales Tagesprofil";

    /// <summary>Virtuelle Zeit des dargestellten Zustands (nach dem letzten Schritt); <c>null</c> bis zur ersten Zeit.</summary>
    public DateTime? SimulationTime { get; private set; }

    /// <summary>Lauf (Neustart im Master), zu dem <see cref="SimulationTime"/> gehört.</summary>
    public int RunId => _scheduler.RunId;

    public bool IsAutoMode { get; private set; } = true;
    public bool IsRunning { get; private set; } = true;

    /// <summary>Nur standalone gesetzt: Zeitpunkt, an dem die Simulation endet.</summary>
    public DateTime? EndTime => _endTime;

    public bool IsFinished => _endTime is { } end && SimulationTime >= end;

    /// <summary>Virtuelle Zeit, zu der die Automatik die offene Tür schließt; <c>null</c> = keine Öffnung geplant.</summary>
    public DateTime? DoorClosesAt => _autoMode.DoorClosesAt;

    /// <summary>Letzte Meldung (Zeitsynchronisation, Modbus, Wetter); kann aus einem anderen Thread gesetzt werden.</summary>
    public string Message { get; set; } = "";

    /// <summary>Herkunft der Außentemperatur für die CSV-Spalte und die Anzeige.</summary>
    public string OutsideSource => _weather is null
        ? "lokal"
        : _weather.Last is null ? "lokal (kein Wetter)" : "Wettersimulator";

    public void Quit() => IsRunning = false;

    /// <summary>Umschalten zwischen Automatik (Tür nach Tageszeit) und Handbetrieb (Taste A).</summary>
    public void ToggleMode()
    {
        IsAutoMode = !IsAutoMode;
        if (!IsAutoMode)
            _room.SetDoor(false);

        Message = IsAutoMode
            ? "Automatik an: Tür öffnet und schließt nach Tageszeit."
            : "Handbetrieb: Tür bleibt zu, T öffnet und schließt sie.";
    }

    /// <summary>
    /// Schaltet die Tür von Hand. In der Automatik wird dabei auf Handbetrieb umgestellt, damit die Tür nicht
    /// beim nächsten Automatikschritt wieder zufällt.
    /// </summary>
    public void ToggleDoor()
    {
        string prefix = "";
        if (IsAutoMode)
        {
            IsAutoMode = false;
            prefix = "Handbetrieb (Automatik aus) – ";
        }

        _room.SetDoor(!_room.DoorOpen);
        Message = prefix + (_room.DoorOpen ? "Tür geöffnet." : "Tür geschlossen.");
    }

    /// <summary>Setzt die Solltemperatur; ungültige Werte werden vom Modell abgelehnt.</summary>
    /// <returns><c>true</c>, wenn der Wert übernommen wurde.</returns>
    public bool TrySetSetpoint(double valueC, out string message)
    {
        try
        {
            _room.SetpointC = valueC;
            message = string.Create(Culture,
                $"Solltemperatur auf {_room.SetpointC:0.#} °C gesetzt (Raum {_room.RoomTempC:0.0} °C).");
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            message = string.Create(Culture,
                $"Solltemperatur {valueC:0.#} °C abgelehnt – erlaubt sind {Kuehlhaus.MinSetpointC} … {Kuehlhaus.MaxSetpointC} °C.");
            return false;
        }
    }

    /// <returns>Pfad der geschriebenen CSV-Datei.</returns>
    public async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        _modbus?.Start();                                     // Port belegt → IOException, bevor etwas angelegt wird
        await using var logger = await CsvLogger.CreateAsync(Config.DeviceId, Config.OutputDirectory, cancellationToken);
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var syncTask = Time.RunAsync(stop.Token);

        Message = Time.IsStandalone
            ? Config.IsShortRun
                ? $"WARNUNG: Dauer {Config.DurationHours} h < {SimulationConfig.MinimumRecommendedHours} h – für die Abgabe einen Lauf ≥ 24 h verwenden."
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
            await syncTask;                                   // meldet sich beim Master ab
            await _renderer.ShutdownAsync();
            _weather?.Dispose();
        }

        return logger.FilePath;
    }

    /// <summary>Rechnet alle fälligen Modellschritte bis zur aktuellen virtuellen Zeit.</summary>
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
                await StartRunAsync(update.ModelTime, "Erste Zeit empfangen", logger, cancellationToken);
                return true;

            case StepUpdateKind.Restarted:
                await StartRunAsync(update.ModelTime, "Neustart durch den Master", logger, cancellationToken);
                return true;
        }

        if (update.SkippedSteps > 0)
            Message = $"WARNUNG: {update.SkippedSteps} Schritte übersprungen (zu viele auf einmal fällig).";

        for (int i = 0; i < update.StepCount && !IsFinished; i++)
        {
            var stepStart = update.FirstStepAt + Config.Step * i;

            double outsideC = await UpdateWeatherAsync(stepStart, cancellationToken);
            if (IsAutoMode)
                _autoMode.Step(stepStart, Config.Step);

            _room.Update(Config.Step, outsideC);
            SimulationTime = stepStart + Config.Step;

            await logger.WriteAsync(SimulationTime.Value, RunId, IsAutoMode, _room, OutsideSource, cancellationToken);
        }

        if (update.StepCount > 0)
            await logger.FlushAsync(cancellationToken);
        return update.StepCount > 0;
    }

    /// <summary>Erste Zeit oder neuer Lauf: Modellzeit setzen, Dauer neu rechnen, Startzeile schreiben.</summary>
    private async Task StartRunAsync(DateTime modelTime, string reason, CsvLogger logger, CancellationToken cancellationToken)
    {
        SimulationTime = modelTime;
        if (Config.Duration is { } duration)
            _endTime = modelTime + duration;

        Message = $"{reason}: Lauf {RunId} ab {modelTime.ToString("dd.MM.yyyy HH:mm", Culture)}.";
        _room.Update(TimeSpan.Zero, await UpdateWeatherAsync(modelTime, cancellationToken));   // nur Außentemperatur/Zustand setzen, keine Zeit vergeht
        await logger.WriteAsync(modelTime, RunId, IsAutoMode, _room, OutsideSource, cancellationToken);
        await logger.FlushAsync(cancellationToken);
    }

    /// <summary>
    /// Außentemperatur des Schritts: vom Wettersimulator (höchstens eine Abfrage je Modellstunde; <see cref="WeatherFeed"/>
    /// behält bei einem Fehler das letzte Wetter und meldet den Ausfall nach 3 Fehlversuchen), sonst aus dem lokalen Tagesprofil.
    /// </summary>
    private async Task<double> UpdateWeatherAsync(DateTime time, CancellationToken cancellationToken)
    {
        if (_weather is not null && await _weather.UpdateAsync(time, RunId, cancellationToken) is { } weather)
            return weather.TemperatureC;

        return LocalOutsideTemperature(time);
    }

    /// <summary>
    /// Lokales Tagesprofil der Außentemperatur in °C: Minimum 5 °C um 3 Uhr, Maximum 23 °C um 15 Uhr.
    /// Wird nur ohne Wettersimulator verwendet (<c>--weather off</c> oder Standalone-Standard).
    /// </summary>
    public static double LocalOutsideTemperature(DateTime time) =>
        14 + 9 * Math.Sin(2 * Math.PI * (time.Hour + time.Minute / 60.0 - 9) / 24);
}
