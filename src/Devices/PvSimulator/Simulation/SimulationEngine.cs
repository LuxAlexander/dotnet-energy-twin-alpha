using EnergySimulator.Data;
using EnergySimulator.Devices;
using EnergySimulator.Output;
using EnergySimulator.Simulation;

namespace EnergySimulator.Simulation;

public sealed class SimulationEngine
{
    // Alle 80 ms wird die Schleife erneut durchlaufen (Grundtakt). Das
    // begrenzt die Tastaturabfrage, bremst die Simulation selbst aber nicht –
    // die simulierte Zeit richtet sich nach der real verstrichenen Zeit.
    private const int TickMs = 80;

    private readonly SimulationConfig _config;
    private readonly PvSystem _pv;
    private readonly PvFaultModel _faults;
    private readonly CsvExporter _csv;
    // Zeit-seeded statt datums-seeded: Wetter und Netzrauschen sollen pro Lauf der
    // Realitaet nachempfunden zufaellig bleiben (fuer reproduzierbare
    // Temperaturkurven sorgt PvFaultModel.DailyOffsetC separat).
    private readonly Random _random = new();
    private readonly SimulationClock _clock;

    private SimulationMode _mode = SimulationMode.Auto;
    private SimulationMode _resumeMode = SimulationMode.Auto;
    private bool _running;
    private DateTime _lastTick;
    private DateTime _lastRender;
    private double _lastCsvHours;
    private string _notification = string.Empty;
    private DateTime _notificationSetAt;

    public SimulationMode Mode => _mode;
    public bool IsRunning => _running;
    public PvSystem Pv => _pv;
    public SimulationClock Clock => _clock;
    public int Timescale => _config.Timescale;

    public SimulationEngine(SimulationConfig config)
    {
        _config = config;
        _random = new Random();
        _faults = new PvFaultModel(_random);
        _pv = new PvSystem(config.PeakPowerKw, _faults);
        _csv = new CsvExporter(config.OutputDirectory, config.CsvFileName);
        _clock = new SimulationClock(config.Timescale, config.StartDate);
        _lastTick = DateTime.Now;
        _lastRender = DateTime.MinValue;
        _lastCsvHours = 0.0;
        _csv.WriteHeader();
    }

    public void Start()
    {
        _running = true;
        SetNotification($"Start {_clock.Now:dd.MM.yyyy HH:mm:ss} · {_clock.Now:MMM} · x{_config.Timescale}");

        while (_running)
        {
            if (_mode == SimulationMode.Paused)
            {
                HandlePaused();
                continue;
            }

            if (_mode == SimulationMode.Auto)
                UpdateAutoWeather();

            DateTime now = DateTime.Now;
            double elapsedRealMs = (now - _lastTick).TotalMilliseconds;
            _lastTick = now;

            bool newDay = _clock.Advance(elapsedRealMs);
            double elapsedSimHours = elapsedRealMs * _config.Timescale / 3_600_000.0;

            _pv.StepContinuous(_clock.Now, elapsedSimHours);

            if (newDay)
            {
                _csv.WriteRow(_clock.Now, _mode, _pv);
                _pv.ResetDailyEnergy();
                _lastCsvHours = 0.0;
                //_clock.Now.TimeOfDay.TotalHours either change the varaible or the meaning to Time:Hours
                SetNotification($"Neuer Tag · {_clock.Now:dd.MM.yyyy} · {_clock.Now.TimeOfDay.TotalHours:F1} kWh Vortag");
            }
            else if (_clock.Now.TimeOfDay.TotalHours >= _lastCsvHours + _config.StepMinutes / 60.0)
            {
                _csv.WriteRow(_clock.Now, _mode, _pv);
                _lastCsvHours = _clock.Now.TimeOfDay.TotalHours;
            }

            RenderIfDue();

            if (KeyAvailable())
            {
                HandleKey(Console.ReadKey(intercept: true).Key);
                Render(true);
            }
            else
            {
                Thread.Sleep(TickMs);
            }
        }

        _csv.WriteRow(_clock.Now, _mode, _pv);
        _csv.Flush();
        _running = false;
    }

    public void Stop() => _running = false;

    private void HandlePaused()
    {
        RenderIfDue();

        // Console.ReadKey(intercept: true) aendert das Verhalten: die Taste wird
        // gelesen, aber nicht in der Konsole angezeigt (bei non-modalen Eingaben
        // wuerde sie sonst den Bildschirm inhaltlich zerbrechen).
        if (KeyAvailable())
        {
            HandleKey(Console.ReadKey(intercept: true).Key);
            Render(true);
        }
        else
        {
            Thread.Sleep(TickMs);
        }
    }

    private void HandleKey(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.P: TogglePause(); break;
            case ConsoleKey.M: ToggleMode(); break;
            case ConsoleKey.Q: _running = false; SetNotification("Beendet"); break;
            case ConsoleKey.F: ToggleFault(); break;
            case ConsoleKey.T: ToggleOverheat(); break;
            case ConsoleKey.G: ToggleGrid(); break;
            case ConsoleKey.C: CleanModules(); break;

            case ConsoleKey.D0: case ConsoleKey.NumPad0: SetWeather(WeatherCondition.Night); break;
            case ConsoleKey.D1: case ConsoleKey.NumPad1: SetWeather(WeatherCondition.Sunny); break;
            case ConsoleKey.D2: case ConsoleKey.NumPad2: SetWeather(WeatherCondition.PartlyCloudy); break;
            case ConsoleKey.D3: case ConsoleKey.NumPad3: SetWeather(WeatherCondition.Overcast); break;
            case ConsoleKey.D4: case ConsoleKey.NumPad4: SetWeather(WeatherCondition.Storm); break;
            case ConsoleKey.D5: case ConsoleKey.NumPad5: SetTimescale(1); break;
            case ConsoleKey.D6: case ConsoleKey.NumPad6: SetTimescale(15); break;
            case ConsoleKey.D7: case ConsoleKey.NumPad7: SetTimescale(60); break;
            case ConsoleKey.D8: case ConsoleKey.NumPad8: SetTimescale(300); break;
            case ConsoleKey.D9: case ConsoleKey.NumPad9: SetTimescale(1800); break;
            case ConsoleKey.R: CycleRefresh(); break;
        }
    }

    private static readonly int[] RefreshSteps = { 100, 200, 400, 800, 1500, 3000 };

    private void CycleRefresh()
    {
        int index = Array.IndexOf(RefreshSteps, _config.RefreshMs);
        if (index < 0)
            index = 2;

        index = (index + 1) % RefreshSteps.Length;
        _config.RefreshMs = RefreshSteps[index];

        SetNotification($"Bild-Aktualisierung → {_config.RefreshMs} ms");
    }

    private void TogglePause()
    {
        if (_mode == SimulationMode.Paused)
        {
            _mode = _resumeMode;
            _lastTick = DateTime.Now;
            SetNotification($"Fortgesetzt · x{_config.Timescale}");
        }
        else
        {
            _resumeMode = _mode;
            _mode = SimulationMode.Paused;
            SetNotification("PAUSE");
        }
    }

    private void ToggleMode()
    {
        if (_mode == SimulationMode.Paused)
            return;

        _mode = _mode == SimulationMode.Auto ? SimulationMode.Manual : SimulationMode.Auto;
        SetNotification($"Modus → {(_mode == SimulationMode.Auto ? "AUTO" : "MANUELL")}");
    }

    private void SetTimescale(int timescale)
    {
        _config.Timescale = timescale;
        _clock.Timescale = timescale;
        _lastTick = DateTime.Now;
        SetNotification($"Tempo → x{timescale}");
    }

    private void SetWeather(WeatherCondition weather)
    {
        _pv.SetWeather(weather);
        SetNotification($"Wetter → {weather.ToGerman()}");
    }

    private void ToggleFault()
    {
        _faults.ToggleInverterFault();
        SetNotification(_faults.InverterFault
            ? "Störung → Inverterfehler ausgelöst"
            : "Störung → Inverterfehler zurückgesetzt");
    }

    private void ToggleOverheat()
    {
        _faults.ToggleForceOverheat();
        SetNotification(_faults.OverheatForced
            ? "Störung → Überhitzung erzwungen"
            : "Störung → Überhitzung aus");
    }

    private void ToggleGrid()
    {
        _faults.ToggleGridDisturbance();
        SetNotification(_faults.GridDisturbanceForced
            ? "Störung → Netzspannungsstörung aktiv"
            : "Störung → Netzspannung normal");
    }

    private void CleanModules()
    {
        _faults.CleanModules();
        SetNotification("Wartung → Module gereinigt, Verschmutzung zurückgesetzt");
    }

    private void UpdateAutoWeather()
    {
        Season season = SeasonInfo.FromMonth(_clock.Now.Month);
        double hour = _clock.HourOfDay;

        if (!IrradiationCalculator.IsDaylight(hour, season))
        {
            if (!_pv.IsNight)
                _pv.SetWeather(WeatherCondition.Night);
            return;
        }

        if (_pv.IsNight)
            _pv.SetWeather(RandomWeather(season));

        if (_random.NextDouble() < 0.004)
        {
            WeatherCondition next = RandomWeather(season);
            _pv.SetWeather(next);

            if (next == WeatherCondition.Snow)
                SetNotification("Wetter → Schneefall");
        }
    }

    private WeatherCondition RandomWeather(Season season)
    {
        if (_random.NextDouble() < season.SnowProbability())
            return WeatherCondition.Snow;

        var options = new[]
        {
            WeatherCondition.Sunny,
            WeatherCondition.PartlyCloudy,
            WeatherCondition.Overcast,
            WeatherCondition.Storm
        };

        return options[_random.Next(options.Length)];
    }

    private void RenderIfDue()
    {
        DateTime now = DateTime.Now;
        if ((now - _lastRender).TotalMilliseconds < _config.RefreshMs)
            return;

        _lastRender = now;
        Draw();
    }

    private void Render(bool force = false)
    {
        if (force)
            _lastRender = DateTime.Now;

        if ((DateTime.Now - _notificationSetAt).TotalSeconds > 4)
            _notification = string.Empty;

        Draw();
    }

    private void Draw()
    {
        ConsoleDisplay.Render(
            _clock.Now,
            SeasonInfo.FromMonth(_clock.Now.Month),
            _mode,
            _pv,
            _config.PeakPowerKw,
            _config.Timescale,
            _notification,
            _clock.DayProgress);
    }

    private void SetNotification(string message)
    {
        _notification = message;
        _notificationSetAt = DateTime.Now;
    }

    private static bool KeyAvailable()
    {
        try { return Console.KeyAvailable; }
        catch { return false; }
    }
}