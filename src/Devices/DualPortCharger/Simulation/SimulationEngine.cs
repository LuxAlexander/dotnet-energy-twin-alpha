using System.Diagnostics;
using DualChargerSim.IO;
using DualChargerSim.Models;

namespace DualChargerSim.Simulation;

/// <summary>
/// Hauptschleife: Simulationszeit, Pause, Echtzeit-Warten, Aufruf von Tick, Ausgabe und CSV.
/// </summary>
public class SimulationEngine
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);
    private static readonly TimeSpan PausedRedrawInterval = TimeSpan.FromMilliseconds(200);

    private readonly AutoModeController _autoMode;
    private readonly ConsoleRenderer _renderer;
    private readonly InputHandler _input;
    private readonly DateTime _endTime;
    private readonly Random _random;
    private double _speed;

    public SimulationConfig Config { get; }
    public DualCharger Charger { get; }
    public DateTime SimulationTime { get; private set; }
    public bool IsPaused { get; set; }
    public bool IsAutoMode { get; set; } = true;
    public bool IsRunning { get; private set; } = true;
    public bool IsFinished => SimulationTime >= _endTime;
    public int SelectedPointIndex { get; set; }
    public ChargingPoint SelectedPoint => Charger.Points[SelectedPointIndex];
    public string Message { get; set; } = "";

    /// <summary>Tempo gegenüber Echtzeit; zur Laufzeit änderbar, begrenzt auf 1–100 000.</summary>
    public double Speed
    {
        get => _speed;
        set => _speed = Math.Clamp(value, SimulationConfig.MinSpeed, SimulationConfig.MaxSpeed);
    }

    /// <summary>Echtzeit-Wartezeit pro Schritt = Schrittweite / Tempo.</summary>
    public TimeSpan RealDelay => TimeSpan.FromSeconds(Config.Step.TotalSeconds / Speed);

    public SimulationEngine(SimulationConfig config)
    {
        Config = config;
        _random = config.Seed is int seed ? new Random(seed) : new Random();

        Charger = new DualCharger(config.DeviceId, config.PowerLimitKW);
        _autoMode = new AutoModeController(Charger, _random);
        _renderer = new ConsoleRenderer();
        _input = new InputHandler(this, _renderer);

        _speed = config.Speed;
        SimulationTime = config.StartTime;
        _endTime = config.StartTime + config.Duration;
    }

    public void Quit() => IsRunning = false;

    /// <summary>
    /// Manueller Modus: zufälliges Fahrzeug kommt, steckt am freien Punkt an und lädt sofort.
    /// Abgesteckt wird von Hand (D).
    /// </summary>
    public void VehicleArrives()
    {
        var vehicle = VehicleCatalog.CreateRandom(_random);
        Charger.Enqueue(vehicle, SimulationTime);
        var point = Charger.Points.FirstOrDefault(p => p.Vehicle == vehicle);

        Message = point is null
            ? $"{vehicle.ModelName} wartet – beide Punkte belegt (fährt nach {DualCharger.MaxQueueWait.TotalMinutes:0} min weg)."
            : $"Punkt {point.Number}: {vehicle.ModelName} angesteckt, SOC {vehicle.SocPercent:0} % → {vehicle.TargetSocPercent:0} % – D zum Abstecken.";
    }

    public void Run()
    {
        using var logger = new CsvLogger(Config.DeviceId, Config.OutputDirectory);

        if (Config.IsShortRun)
            Message = $"WARNUNG: Dauer {Config.DurationHours} h < 24 h – für die Abgabe einen Lauf ≥ 24 h verwenden.";

        _renderer.Initialize();
        logger.Write(SimulationTime, IsAutoMode, Charger);
        _renderer.Draw(this);

        while (IsRunning && !IsFinished)
        {
            if (!IsPaused)
            {
                Step();
                logger.Write(SimulationTime, IsAutoMode, Charger);
            }

            _renderer.Draw(this);
            WaitAndHandleInput();
        }

        if (IsRunning)
        {
            Message = $"Simulation beendet. CSV: {logger.FilePath} – Q zum Beenden.";
            _renderer.Draw(this);
            WaitForQuit();
        }

        _renderer.Shutdown();
        Console.WriteLine($"CSV gespeichert: {Path.GetFullPath(logger.FilePath)}");
    }

    private void Step()
    {
        if (IsAutoMode)
            _autoMode.Step(SimulationTime, Config.Step);

        Charger.Tick(SimulationTime, Config.Step);
        SimulationTime += Config.Step;
    }

    /// <summary>
    /// Wartet in kleinen Häppchen und fragt dabei die Tastatur ab,
    /// damit Eingaben auch bei langen Wartezeiten sofort wirken.
    /// Die Wartezeit wird laufend neu berechnet, damit eine Tempo-Änderung sofort greift.
    /// </summary>
    private void WaitAndHandleInput()
    {
        var stopwatch = Stopwatch.StartNew();
        bool wasPaused = IsPaused;

        do
        {
            if (!Console.IsInputRedirected)
            {
                while (Console.KeyAvailable)
                {
                    _input.Handle(Console.ReadKey(intercept: true));
                    _renderer.Draw(this);
                }
            }

            // Nach Beenden oder Fortsetzen nicht den Rest der Wartezeit absitzen
            if (!IsRunning || (wasPaused && !IsPaused))
                return;

            var remaining = CurrentDelay - stopwatch.Elapsed;
            if (remaining > TimeSpan.Zero)
                Thread.Sleep(remaining < PollInterval ? remaining : PollInterval);
        } while (stopwatch.Elapsed < CurrentDelay);
    }

    private TimeSpan CurrentDelay => IsPaused ? PausedRedrawInterval : RealDelay;

    private void WaitForQuit()
    {
        if (Console.IsInputRedirected)
            return;

        while (Console.ReadKey(intercept: true).Key != ConsoleKey.Q)
        {
        }
    }
}
