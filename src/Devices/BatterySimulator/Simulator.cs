using System.Collections.Concurrent;
using System.Globalization;
using Devices.Common;

namespace Batterie_Sim;

/// <summary>
/// Hauptschleife des Batteriespeichers: holt die virtuelle Zeit von der Zeitquelle (Master oder lokal), rechnet die fälligen
/// festen Schritte, schreibt CSV und Modbus-Register, zeichnet die Anzeige und verarbeitet Tasten – ohne blockierendes Warten.
/// Die Modbus-Register bleiben auch in der Pause und bei verlorenem Master erreichbar.
/// </summary>
public sealed class Simulator
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan RedirectedInterval = TimeSpan.FromSeconds(5);
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

    private readonly Batterie _battery;
    private readonly DailyProfile _profile;
    private readonly ConsoleDashboard _dashboard = new();
    private readonly SimulatorOptions _options;
    private readonly StepScheduler _scheduler;
    private readonly BatteryModbus? _modbus;
    private readonly ConcurrentQueue<Action> _pending = new();
    private DateTime? _endTime;
    private Task? _menuTask;
    private volatile string _message = "";

    public DeviceTimeSource Time { get; }
    public ControlMode Mode { get; set; } = ControlMode.Auto;
    public bool QuitRequested { get; private set; }

    /// <summary>Virtuelle Zeit des dargestellten Zustands (nach dem letzten Schritt); <c>null</c> bis zur ersten Zeit.</summary>
    public DateTime? SimulationTime { get; private set; }

    public int RunId => _scheduler.RunId;
    public double StepSeconds => _options.StepSeconds;
    public bool IsFinished => SimulationTime >= _endTime;

    /// <summary>Letzte Meldung; kann auch aus Zeitsynchronisation und Modbus (andere Threads) gesetzt werden.</summary>
    public string Message
    {
        get => _message;
        set => _message = value;
    }

    public string? ModbusStatus => _modbus?.StatusText;

    public Simulator(Batterie battery, DailyProfile profile, SimulatorOptions options)
    {
        _battery = battery;
        _profile = profile;
        _options = options;
        _scheduler = new StepScheduler(options.Step);

        Time = DeviceTimeSource.Create(options.TimeSync, battery.DeviceId, "Battery");
        Time.Log += (_, message) => Message = message;

        if (!options.Modbus.Disabled)
            _modbus = new BatteryModbus(this, battery, Time, options.Modbus, message => Message = message);
    }

    /// <returns>Pfad der geschriebenen CSV-Datei.</returns>
    public async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        _modbus?.Start();                                         // Port belegt → IOException, bevor etwas angelegt wird
        await using var logger = await CsvLogger.CreateAsync(_battery.Config, cancellationToken);
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var syncTask = Time.RunAsync(stop.Token);

        Message = Time.IsStandalone
            ? "Standalone: lokale Uhr, ohne Simulations-Master."
            : $"Verbinde mit Simulations-Master {_options.TimeSync.MasterEndpoint} …";

        bool interactive = ConsoleDashboard.IsInteractive;
        _dashboard.Initialize();
        var nextLog = DateTime.MinValue;
        try
        {
            while (!QuitRequested && !IsFinished && !stop.IsCancellationRequested)
            {
                bool menuOpen = _menuTask is { IsCompleted: false };
                if (!menuOpen)
                    HandleKeys(stop.Token);
                RunPending();
                await AdvanceAsync(logger, stop.Token);

                // Modbus läuft auch in der Pause weiter: Befehle umsetzen, Zustand veröffentlichen
                if (_modbus is not null)
                {
                    _modbus.ProcessCommands();
                    _modbus.Publish(SimulationTime, RunId);
                }

                if (interactive)
                {
                    if (!menuOpen)
                        _dashboard.Render(this, _battery);
                }
                else if (DateTime.UtcNow >= nextLog)
                {
                    await Console.Out.WriteLineAsync(ConsoleDashboard.StatusLine(this, _battery));
                    nextLog = DateTime.UtcNow + RedirectedInterval;
                }

                await Task.Delay(PollInterval, stop.Token);
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
            if (_menuTask is not null)
            {
                try { await _menuTask; } catch (OperationCanceledException) { }
            }
            await logger.FlushAsync(CancellationToken.None);
            _dashboard.Shutdown();
        }

        return logger.FilePath;
    }

    /// <summary>Rechnet alle fälligen Schritte bis zur aktuellen virtuellen Zeit.</summary>
    /// <returns><c>true</c>, wenn sich der Zustand geändert hat.</returns>
    private async Task<bool> AdvanceAsync(CsvLogger logger, CancellationToken cancellationToken)
    {
        if (!Time.HasTime)
            return false;

        // Standalone beginnt genau bei --start (die lokale Uhr läuft seit dem Programmstart schon)
        var clockNow = _scheduler.ModelTime is null && Time.IsStandalone ? _options.TimeSync.StandaloneStart : Time.Clock.Now;
        var update = _scheduler.Update(clockNow, Time.Clock.RunId);

        switch (update.Kind)
        {
            case StepUpdateKind.Started:
                SimulationTime = update.ModelTime;
                if (_options.DurationHours is { } hours)
                    _endTime = update.ModelTime + TimeSpan.FromHours(hours);
                await logger.LogAsync(update.ModelTime, RunId, _battery, Mode, cancellationToken);
                return true;

            case StepUpdateKind.Restarted:
                SimulationTime = update.ModelTime;
                Message = $"Neustart durch den Master: Lauf {RunId} ab {update.ModelTime.ToString("dd.MM.yyyy HH:mm", Culture)}.";
                await logger.LogAsync(update.ModelTime, RunId, _battery, Mode, cancellationToken);
                return true;
        }

        if (update.SkippedSteps > 0)
            Message = $"WARNUNG: {update.SkippedSteps} Schritte übersprungen (zu viele auf einmal fällig).";

        for (int i = 0; i < update.StepCount && !IsFinished; i++)
        {
            var stepStart = update.FirstStepAt + _options.Step * i;
            if (Mode == ControlMode.Auto)
                _battery.SetPower(_profile.GetSetpoint(new TimeSpan(stepStart.Hour, stepStart.Minute, stepStart.Second)));

            _battery.Update(_options.Step);
            SimulationTime = stepStart + _options.Step;
            await logger.LogAsync(SimulationTime.Value, RunId, _battery, Mode, cancellationToken);
        }

        if (update.StepCount > 0)
            await logger.FlushAsync(cancellationToken);
        return update.StepCount > 0;
    }

    private void RunPending()
    {
        while (_pending.TryDequeue(out var action))
            action();
    }

    /// <summary>Verarbeitet alle anstehenden Tasten ohne zu warten.</summary>
    private void HandleKeys(CancellationToken cancellationToken)
    {
        while (AsyncConsole.TryReadKey() is { } key)
            HandleKey(key, cancellationToken);
    }

    private void HandleKey(ConsoleKeyInfo info, CancellationToken cancellationToken)
    {
        switch (info.Key)
        {
            case ConsoleKey.Spacebar:
                Message = Time.TogglePause();
                break;
            case ConsoleKey.M:
                if (Mode == ControlMode.Auto)
                {
                    Mode = ControlMode.Manual;
                    _battery.SetPower(0);
                }
                else
                {
                    Mode = ControlMode.Auto;
                }
                Message = $"Modus {Mode}.";
                break;
            case ConsoleKey.Q:
                QuitRequested = true;
                break;
            case ConsoleKey.UpArrow:
                if (Mode == ControlMode.Manual)
                    SetSetpoint(_battery.SetpointKW + 0.5);
                break;
            case ConsoleKey.DownArrow:
                if (Mode == ControlMode.Manual)
                    SetSetpoint(_battery.SetpointKW - 0.5);
                break;
            case ConsoleKey.D0 or ConsoleKey.NumPad0:
                _battery.SetPower(0);
                break;
            case ConsoleKey.E:
                if (_battery.IsOn)
                    _battery.TurnOff();
                else
                    _battery.TurnOn();
                break;
            case ConsoleKey.H:
                _battery.TriggerFault(FaultType.Overheat);
                break;
            case ConsoleKey.N:
                _battery.TriggerFault(FaultType.EmergencyStop);
                break;
            case ConsoleKey.R:
                _battery.ResetFault();
                break;
            case ConsoleKey.S:
                if (ConsoleDashboard.IsInteractive)
                    _menuTask = SettingsMenuAsync(cancellationToken);
                else
                    Message = "Einstellungen brauchen eine interaktive Konsole.";
                break;
            default:
                if (info.KeyChar == '<')
                    Message = Time.ChangeSpeed(0.5);
                else if (info.KeyChar == '>')
                    Message = Time.ChangeSpeed(2);
                break;
        }
    }

    /// <summary>Sollwert innerhalb der konfigurierten Leistungsgrenzen.</summary>
    private void SetSetpoint(double kw) =>
        _battery.SetPower(Math.Clamp(kw, -_battery.Config.MaxDischargeKW, _battery.Config.MaxChargeKW));

    /// <summary>
    /// Einstellungen (Sollwert, Leistungsgrenzen, SOC-Grenzen) in einer Eingabeaufforderung unter der Anzeige. Die Zeit läuft währenddessen
    /// weiter; Änderungen werden im Simulationsthread übernommen. Ungültige Eingaben werden mit Begründung abgelehnt.
    /// </summary>
    private async Task SettingsMenuAsync(CancellationToken cancellationToken)
    {
        var config = _battery.Config;
        string message = "";
        while (!cancellationToken.IsCancellationRequested)
        {
            string[] info =
            [
                "=== Einstellungen ===" + (message.Length > 0 ? "   " + message : ""),
                FormattableString.Invariant($"1 Sollwert {_battery.SetpointKW:0.##} kW   2 max. Ladeleistung {config.MaxChargeKW:0.##} kW   3 max. Entladeleistung {config.MaxDischargeKW:0.##} kW"),
                FormattableString.Invariant($"4 SOC-Min {config.SocMinPercent:0.##} %   5 SOC-Max {config.SocMaxPercent:0.##} %   0 Zurück"),
            ];
            string choice = await PromptAsync("Auswahl:", info, cancellationToken);
            message = "";
            if (choice is "" or "0")
                break;

            string? question = choice switch
            {
                "1" => "Neuer Sollwert in kW (negativ = entladen):",
                "2" => "Neue max. Ladeleistung in kW:",
                "3" => "Neue max. Entladeleistung in kW:",
                "4" => "Neue SOC-Min in %:",
                "5" => "Neue SOC-Max in %:",
                _ => null
            };
            if (question is null)
            {
                message = "Ungültige Auswahl.";
                continue;
            }

            string text = await PromptAsync(question, null, cancellationToken);
            if (text.Length == 0)
                continue;
            if (!double.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double value) || !double.IsFinite(value))
            {
                message = "Ungültige Eingabe, Wert unverändert.";
                continue;
            }

            string? error = choice switch
            {
                "1" when value < -config.MaxDischargeKW || value > config.MaxChargeKW =>
                    FormattableString.Invariant($"Sollwert muss zwischen {-config.MaxDischargeKW:0.##} und {config.MaxChargeKW:0.##} kW liegen."),
                "2" or "3" when value <= 0 => "Leistung muss größer als 0 sein.",
                "4" when value < 0 || value >= config.SocMaxPercent => "SOC-Min muss ≥ 0 und kleiner als SOC-Max sein.",
                "5" when value > 100 || value <= config.SocMinPercent => "SOC-Max muss ≤ 100 und größer als SOC-Min sein.",
                _ => null
            };
            if (error is not null)
            {
                message = error + " Wert unverändert.";
                continue;
            }

            // Änderung im Simulationsthread übernehmen
            _pending.Enqueue(() =>
            {
                switch (choice)
                {
                    case "1": Mode = ControlMode.Manual; _battery.SetPower(value); break;
                    case "2": config.MaxChargeKW = value; break;
                    case "3": config.MaxDischargeKW = value; break;
                    case "4": config.SocMinPercent = value; break;
                    case "5": config.SocMaxPercent = value; break;
                }
            });
            message = "Übernommen.";
        }

        Message = "Einstellungen geschlossen.";
        await ClearPromptAsync();
    }

    private async Task<string> PromptAsync(string question, IReadOnlyList<string>? info, CancellationToken cancellationToken)
    {
        int top = _dashboard.BelowStatus;
        Console.SetCursorPosition(0, top);
        Console.CursorVisible = true;
        int width = Math.Max(Console.WindowWidth - 1, 40);

        if (info is not null)
            foreach (string line in info)
                await Console.Out.WriteLineAsync(line.PadRight(width));

        await Console.Out.WriteAsync(question + " ");
        string input = await AsyncConsole.ReadLineAsync(20, cancellationToken) ?? "";
        return input.Trim();
    }

    private async Task ClearPromptAsync()
    {
        int top = _dashboard.BelowStatus;
        int width = Math.Max(Console.WindowWidth - 1, 40);
        Console.SetCursorPosition(0, top);
        for (int i = 0; i < 5; i++)
            await Console.Out.WriteLineAsync(new string(' ', width));
        Console.CursorVisible = false;
    }
}
