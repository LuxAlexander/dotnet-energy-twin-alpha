using System.Globalization;
using Devices.Common;
using Devices.Common.Modbus;

namespace EnergySimulator.Simulation;

/// <summary>Liest die Kommandozeile in eine <see cref="SimulationConfig"/>.</summary>
public static class ArgsParser
{
    public static readonly string Usage = $"""
        EnergySimulator – PV-Anlage

        Aufruf: dotnet run --project src/Devices/PvSimulator -- [Optionen]

          --id <text>            Geräte-ID                          (Standard: PV-01)
          -p, --peak <kW>        Nennleistung in kW                 (Standard: 10.0)
          -t, --step <min>       CSV-Intervall in Minuten           (Standard: 15)
          --model-step <min>     feste Schrittweite des Modells     (Standard: 1)
          -r, --refresh <ms>     Bildschirm-Aktualisierung, mind. 50 (Standard: 400)
          -o, --output <ordner>  CSV-Ordner                         (Standard: docs/data im Repo)
          --seed <zahl>          Zufalls-Seed für Auto-Wetter und Netzrauschen (optional)
          -h, --help             Diese Hilfe

        Virtuelle Zeit (Standard: vom Simulations-Master):
        {TimeSyncOptions.Usage}
          (-s ist ein Kürzel für --speed, -d für --start)

        Modbus-TCP-Server (jede Anlage braucht einen eigenen Port):
        {ModbusOptions.Usage}

        Tasten: P Pause (nur standalone) · M Modus AUTO/MANUELL · Q Beenden · C Module reinigen
                F Inverterfehler · T Überhitzung · G Netzstörung · 0–4 Wetter (manuell)
                5–9 Tempo x1/x15/x60/x300/x1800 (nur standalone) · R Bild ruhiger/schneller

        Beispiele:
          dotnet run --project src/Devices/PvSimulator -- --id PV-01 -p 10
          dotnet run --project src/Devices/PvSimulator -- --standalone -s 300 -d "2026-06-21 05:00"
        """;

    public static bool IsHelpRequested(string[] args) => args.Any(a => a is "--help" or "-h" or "/?");

    /// <exception cref="ArgumentException">Ungültige oder unbekannte Option.</exception>
    public static SimulationConfig Parse(string[] args)
    {
        var config = new SimulationConfig();
        var timeSync = config.TimeSync;
        var modbus = config.Modbus;

        for (int i = 0; i < args.Length; i++)
        {
            string name = Canonical(args[i]);
            int index = i;

            if (TimeSyncOptions.TryApply(name, () => NextValue(args, ref index, name), timeSync, out timeSync)
                || ModbusOptions.TryApply(name, () => NextValue(args, ref index, name), modbus, out modbus))
            {
                i = index;
                continue;
            }

            switch (name)
            {
                case "--id":
                    config.DeviceId = NextValue(args, ref i, name);
                    if (string.IsNullOrWhiteSpace(config.DeviceId))
                        throw new ArgumentException("--id darf nicht leer sein.");
                    break;
                case "--peak":
                    config.PeakPowerKw = ParsePositive(NextValue(args, ref i, name), name);
                    break;
                case "--step":
                    config.StepMinutes = (int)ParsePositive(NextValue(args, ref i, name), name);
                    break;
                case "--model-step":
                    config.ModelStepMinutes = ParsePositive(NextValue(args, ref i, name), name);
                    break;
                case "--refresh":
                    // Untergrenze 50 ms, damit der Diff-Modus die Konsole nicht flackern lässt
                    config.RefreshMs = Math.Max(50, (int)ParsePositive(NextValue(args, ref i, name), name));
                    break;
                case "--output":
                    config.OutputDirectory = NextValue(args, ref i, name);
                    if (string.IsNullOrWhiteSpace(config.OutputDirectory) || config.OutputDirectory.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                        throw new ArgumentException($"--output ist kein gültiger Ordner: '{config.OutputDirectory}'.");
                    break;
                case "--seed":
                    string value = NextValue(args, ref i, name);
                    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int seed))
                        throw new ArgumentException($"--seed erwartet eine ganze Zahl, nicht '{value}'.");
                    config.Seed = seed;
                    break;
                default:
                    throw new ArgumentException($"Unbekanntes Argument: '{args[i]}'.");
            }
        }

        if (!timeSync.Standalone && timeSync.HasStandaloneSettings)
            throw new ArgumentException("--start und --speed gelten nur mit --standalone; mit Master bestimmt der Simulations-Master Zeit und Tempo.");

        config.TimeSync = timeSync;
        config.Modbus = modbus;
        return config;
    }

    /// <summary>Kürzel aus LB01 auf die langen Namen abbilden.</summary>
    private static string Canonical(string arg) => arg.ToLowerInvariant() switch
    {
        "-s" => "--speed",
        "-d" => "--start",
        "-p" => "--peak",
        "-t" => "--step",
        "-r" => "--refresh",
        "-o" => "--output",
        var other => other
    };

    private static string NextValue(string[] args, ref int i, string name)
    {
        if (i + 1 >= args.Length)
            throw new ArgumentException($"Für {name} fehlt ein Wert.");
        return args[++i];
    }

    /// <summary>Akzeptiert Punkt und Komma als Dezimaltrenner.</summary>
    private static double ParsePositive(string value, string name)
    {
        if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
            throw new ArgumentException($"{name} erwartet eine Zahl, nicht '{value}'.");
        if (result <= 0 || double.IsInfinity(result))
            throw new ArgumentException($"{name} muss größer als 0 sein (angegeben: {value}).");
        return result;
    }
}
