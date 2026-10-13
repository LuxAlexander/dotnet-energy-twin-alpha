using System.Text;
using EnergySimulator.Devices;
using EnergySimulator.Simulation;

namespace EnergySimulator.Output;

public static class ConsoleDisplay
{
    private const int InnerWidth = 70;

    // Row() rahmt mit einem fuehrenden und einem abschliessenden Leerzeichen ein,
    // die Balken-Spanne ist daher InnerWidth + 2 Zeichen breit.
    private const int BorderWidth = InnerWidth + 2;

    // ANSI-Escape-Sequenzen (ESC = \x1b): moderne Konsolen setzen Farben und
    // Cursor-Steuerung direkt ueber solche Codes um. R setzt alle Attribute
    // zurueck, EraseLine ("\x1b[K") loescht den Rest der aktuellen Zeile –
    // beides zusammen macht das diff-basierte Neuzeichnen erst moeglich.
    private const string R = "\x1b[0m";
    private const string B = "\x1b[1m";
    private const string DIM = "\x1b[90m";
    private const string CYAN = "\x1b[36m";
    private const string GREEN = "\x1b[32m";
    private const string YELLOW = "\x1b[33m";
    private const string RED = "\x1b[31m";
    private const string MAGENTA = "\x1b[35m";
    private const string BLUE = "\x1b[34m";
    private const string WHITE = "\x1b[37m";

    private const string EraseLine = "\x1b[K";

    private static readonly List<string> _frame = new();

    private static string[]? _previous;
    private static bool _cursorProbed;
    private static bool _cursorSupported;
    private static bool _cursorHidden;

    // Opt-in-Diagnose: ENERGYSIM_RENDER_LOG=<datei> protokolliert, welche Zeilen
    // pro Bild tatsaechlich geschrieben wurden.
    private static readonly string? _renderLog =
        Environment.GetEnvironmentVariable("ENERGYSIM_RENDER_LOG");

    public static void Render(
        DateTime now,
        Season season,
        SimulationMode mode,
        PvSystem pv,
        double peakPowerKw,
        int timescale,
        string notification,
        double dayProgress)
    {
        bool night = pv.IsNight;
        bool off = !pv.IsEnabled;
        bool generating = pv.PowerKw > 0;
        FaultType fault = pv.ActiveFault;

        string time = now.ToString("HH:mm:ss");

        string modeText = mode switch
        {
            SimulationMode.Auto => "AUTO",
            SimulationMode.Manual => "MANUELL",
            SimulationMode.Paused => "PAUSE",
            _ => "?"
        };
        string modeColor = mode switch
        {
            SimulationMode.Auto => GREEN,
            SimulationMode.Manual => YELLOW,
            SimulationMode.Paused => MAGENTA,
            _ => WHITE
        };

        string seasonColor = season switch
        {
            Season.Spring => GREEN,
            Season.Summer => YELLOW,
            Season.Autumn => MAGENTA,
            Season.Winter => CYAN,
            _ => WHITE
        };

        string stateText = off ? "AUS" : fault != FaultType.None ? "STÖRUNG" : "EIN";
        string stateColor = off || fault != FaultType.None ? RED : GREEN;

        string powerColor = off || fault != FaultType.None ? RED : generating ? GREEN : WHITE;
        string weatherColor = night ? BLUE : pv.Weather switch
        {
            WeatherCondition.Sunny => YELLOW,
            WeatherCondition.PartlyCloudy => WHITE,
            WeatherCondition.Overcast => DIM,
            WeatherCondition.Storm => MAGENTA,
            WeatherCondition.Snow => CYAN,
            _ => WHITE
        };

        string faultColor = fault == FaultType.None ? DIM : RED;

        var progress = BarChart.Split(dayProgress, 1.0, InnerWidth - 2);

        _frame.Clear();

        Top();

        Row(
            ("  ", ""),
            (now.ToString("dd.MM.yyyy"), B + WHITE),
            ("  ", ""),
            (time, B + WHITE),
            ("   ", ""),
            (season.ToGerman().PadRight(9), seasonColor),
            (modeText.PadRight(9), modeColor),
            (stateText.PadRight(8), stateColor),
            ($"x{timescale}", DIM)
        );

        Row(("  ", ""), (progress.Filled, CYAN), (progress.Empty, DIM));

        Sep();

        TwoCol("Leistung", $"{pv.PowerKw,7:F2} kW", powerColor,
               "Tagesenergie", $"{pv.DailyEnergyKwh,7:F2} kWh", GREEN);

        TwoCol("Gesamtenergie", $"{pv.TotalEnergyKwh,8:F1} kWh", GREEN,
               "Fehler", fault.ToGString(), faultColor);

        TwoCol("Wetter", pv.Weather.ToGerman(), weatherColor,
               "Einstrahlung", $"{pv.IrradiationWm2,4:F0} W/m²", night ? DIM : WHITE);

        TwoCol("Modultemperatur", $"{pv.ModuleTemperatureC,5:F1} °C", pv.ModuleTemperatureC >= 70.0 ? RED : WHITE,
               "Außentemperatur", $"{pv.AmbientTemperatureC,5:F1} °C", DIM);

        TwoCol("Netzspannung", $"{pv.GridVoltageV,6:F1} V", VoltageColor(pv.GridVoltageV),
               "Verschmutzung", $"{pv.SoilingLossPercent,4:F1} %", pv.SoilingLossPercent > 8.0 ? YELLOW : WHITE);

        TwoCol("Alterung", $"{pv.AgingLossPercent,4:F1} %", pv.AgingLossPercent > 6.0 ? YELLOW : WHITE,
               "Wirkungsgrad", $"{pv.EfficiencyPercent,4:F1} %", DIM);

        int capacityWidth = 48;
        var capacity = BarChart.Split(pv.CapacityPercent, 100.0, capacityWidth);
        Row(
            ("  ", ""),
            ("Auslastung ", B),
            (capacity.Filled, off || fault != FaultType.None ? RED : generating ? GREEN : DIM),
            (capacity.Empty, DIM),
            ($" {pv.CapacityPercent,5:F1}%", DIM)
        );

        if (!string.IsNullOrEmpty(notification))
        {
            Sep();
            Row(("  " + notification, B + YELLOW));
        }

        Bottom();
        Footer();

        Apply();
    }

    /// <summary>
    /// Schreibt nur die Zeilen, die sich gegenueber dem vorigen Bild geaendert haben.
    /// Ohne Cursor-Steuerung (umgeleitete Konsole) wird das ganze Bild geschrieben.
    /// </summary>
    private static void Apply()
    {
        if (_frame.Count == 0)
            return;

        Trace("apply-begin");

        if (!ProbeCursor())
        {
            Trace("apply-writeall");
            WriteAll();
            _previous = _frame.ToArray();
            LogFrame("writeall", _frame.Count, _frame.Count);
            return;
        }

        // Beim ersten Bild Reste des Shells loeschen; alle Zeilen ohnehin neu.
        if (_previous is null)
        {
            try { Console.Clear(); Trace("clear-ok"); }
            catch
            {
                DisableCursor();
                WriteAll();
                _previous = _frame.ToArray();
                LogFrame("writeall-first", _frame.Count, _frame.Count);
                return;
            }
        }

        int count = Math.Max(_frame.Count, _previous?.Length ?? 0);
        int written = 0;

        for (int i = 0; i < count; i++)
        {
            string? next = i < _frame.Count ? _frame[i] : null;
            string? prev = i < (_previous?.Length ?? 0) ? _previous![i] : null;

            if (string.Equals(next, prev, StringComparison.Ordinal))
                continue;

            try
            {
                Console.SetCursorPosition(0, i);
            }
            catch
            {
                Trace($"setpos-failed-{i}");
                DisableCursor();
                WriteAll();
                _previous = _frame.ToArray();
                LogFrame("fallback-writeall", count, count);
                return;
            }

            if (!_cursorHidden)
            {
                Trace("cursor-hide");
                try { Console.CursorVisible = false; _cursorHidden = true; } catch { }
            }

            Console.Write(next ?? string.Empty);
            Console.Write(EraseLine);
            written++;
        }

        _previous = _frame.ToArray();
        LogFrame("diff", count, written);
    }

    private static void LogFrame(string mode, int rows, int written)
    {
        if (_renderLog is null)
            return;

        try { File.AppendAllText(_renderLog, $"{DateTime.Now:HH:mm:ss.fff};{mode};rows={rows};written={written}\n"); }
        catch { }
    }

    private static void Trace(string step)
    {
        if (_renderLog is null)
            return;

        try { File.AppendAllText(_renderLog, $"{DateTime.Now:HH:mm:ss.fff};TRACE;{step}\n"); }
        catch { }
    }

    /// <summary>Macht die Anzeige nach dem Lauf sauber abschliessbar.</summary>
    public static void Release()
    {
        if (_cursorHidden)
        {
            try { Console.CursorVisible = true; } catch { }
            _cursorHidden = false;
        }

        if (_cursorSupported && _previous is not null)
        {
            try { Console.SetCursorPosition(0, _previous.Length); } catch { }
        }

        _previous = null;
    }

    private static void WriteAll()
    {
        foreach (string line in _frame)
            Console.WriteLine(line);
    }

    private static bool ProbeCursor()
    {
        if (_cursorProbed)
            return _cursorSupported;

        _cursorProbed = true;
        Trace("probe-begin");

        try
        {
            _ = Console.CursorLeft;
            _ = Console.CursorTop;
            _cursorSupported = true;
        }
        catch (Exception ex)
        {
            _cursorSupported = false;
            Trace($"probe-failed-{ex.GetType().Name}");
        }

        // Nur Windows: Puffergroesse an die Rahmenhoehe anpassen.
        // Schlaegt das fehl, darf der Cursor-Modus deshalb nicht abgeschaltet werden.
        if (_cursorSupported && OperatingSystem.IsWindows())
        {
            try
            {
                int wanted = _frame.Count + 4;
                if (Console.BufferHeight < wanted)
                {
                    Console.BufferHeight = wanted;
                    Trace($"buffer-set-{wanted}");
                }
            }
            catch (Exception ex)
            {
                Trace($"buffer-failed-{ex.GetType().Name}");
            }
        }

        Trace($"probe-end-{_cursorSupported}");
        return _cursorSupported;
    }

    private static void DisableCursor()
    {
        _cursorSupported = false;
        _cursorProbed = true;
    }

    private static string VoltageColor(double voltage) =>
        voltage < PvFaultModel.GridMinVolt || voltage > PvFaultModel.GridMaxVolt ? RED : WHITE;

    // params: beliebig viele Argumente sind erlaubt (der Aufrufer uebergibt
    // 3..12 Segmente). Jedes Segment ist ein Tupel aus Text und ANSI-Farbe;
    // die Spanne wird auf InnerWidth aufgefuellt, damit alle Box-Zeilen gleich
    // breit (76) bleiben.
    private static void Row(params (string Text, string Color)[] segments)
    {
        int visible = 0;
        foreach (var segment in segments)
            visible += segment.Text.Length;

        var sb = new StringBuilder();
        sb.Append("  ").Append(DIM).Append('│').Append(R).Append(' ');

        foreach (var (text, color) in segments)
        {
            if (color.Length == 0)
                sb.Append(text);
            else
                sb.Append(color).Append(text).Append(R);
        }

        if (visible < InnerWidth)
            sb.Append(' ', InnerWidth - visible);

        sb.Append(' ').Append(DIM).Append('│').Append(R);

        _frame.Add(sb.ToString());
    }

    private static void TwoCol(
        string leftLabel, string leftValue, string leftColor,
        string rightLabel, string rightValue, string rightColor)
    {
        string left = leftLabel + ": " + leftValue;
        string right = rightLabel + ": " + rightValue;
        int spacer = Math.Max(1, InnerWidth - (2 + left.Length + right.Length));

        Row(
            ("  ", ""),
            (leftLabel, B), (": ", ""), (leftValue, leftColor),
            (new string(' ', spacer), ""),
            (rightLabel, B), (": ", ""), (rightValue, rightColor)
        );
    }

    private static void Top() =>
        _frame.Add("  " + DIM + "╭" + new string('─', BorderWidth) + "╮" + R);

    private static void Sep() =>
        _frame.Add("  " + DIM + "├" + new string('─', BorderWidth) + "┤" + R);

    private static void Bottom() =>
        _frame.Add("  " + DIM + "╰" + new string('─', BorderWidth) + "╯" + R);

    private static void Footer()
    {
        _frame.Add(string.Empty);
        _frame.Add("  " + DIM + "P Pause   M Modus   Q Beenden   C Module reinigen" + R);
        _frame.Add("  " + DIM + "Störung:  F Inverter   T Überhitzung   G Netzspannung" + R);
        _frame.Add("  " + DIM + "Wetter:   0 Nacht  1 Sonnig  2 Bewölkt  3 Dicht  4 Gewitter" + R);
        _frame.Add("  " + DIM + "Tempo:    5 x1  6 x15  7 x60  8 x300  9 x1800" + R);
        _frame.Add("  " + DIM + "Anzeige:  R ruhiger / schneller" + R);
    }
}