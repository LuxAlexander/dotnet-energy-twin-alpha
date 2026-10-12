using System.Globalization;
using DualChargerSim.Models;
using DualChargerSim.Simulation;

namespace DualChargerSim.IO;

/// <summary>
/// Liest die Kommandozeilenargumente in eine SimulationConfig ein.
/// </summary>
public static class ArgsParser
{
    public const string Usage = """
        Aufruf: dotnet run -- [Optionen]

          --id <text>        Geräte-ID                      (Standard: DC-01)
          --power <kW>       Gesamtleistung der Säule, 50–350 (Standard: 150)
          --step <min>       Schrittweite in Minuten        (Standard: 5)
          --speed <faktor>   Tempo gegenüber Echtzeit       (Standard: 120)
          --duration <h>     Simulationsdauer in Stunden    (Standard: 24)
          --seed <zahl>      Zufalls-Seed für wiederholbare Läufe (optional)
          --output <ordner>  Zielordner der CSV-Datei       (Standard: output im Arbeitsverzeichnis)
          --help             Diese Hilfe anzeigen

        Beispiel: dotnet run -- --id DC-01 --power 150 --step 5 --speed 120 --duration 24 --seed 42 --output ../../data
        """;

    public static bool IsHelpRequested(string[] args) =>
        args.Any(a => a is "--help" or "-h" or "/?");

    /// <summary>Wirft <see cref="ArgumentException"/> mit verständlicher Meldung bei ungültigen Werten.</summary>
    public static SimulationConfig Parse(string[] args)
    {
        var defaults = new SimulationConfig();
        string id = defaults.DeviceId;
        double power = defaults.PowerLimitKW;
        double step = defaults.StepMinutes;
        double speed = defaults.Speed;
        double duration = defaults.DurationHours;
        int? seed = null;
        string output = defaults.OutputDirectory;

        for (int i = 0; i < args.Length; i++)
        {
            string name = args[i].ToLowerInvariant();
            switch (name)
            {
                case "--id":
                    id = NextValue(args, ref i, name);
                    if (string.IsNullOrWhiteSpace(id))
                        throw new ArgumentException("--id darf nicht leer sein.");
                    break;
                case "--power":
                    power = ParsePositive(NextValue(args, ref i, name), name);
                    if (power is < DualCharger.MinPowerLimitKW or > DualCharger.MaxPowerLimitKW)
                        throw new ArgumentException(
                            $"--power muss zwischen {DualCharger.MinPowerLimitKW} und {DualCharger.MaxPowerLimitKW} kW liegen (angegeben: {args[i]}).");
                    break;
                case "--step":
                    step = ParsePositive(NextValue(args, ref i, name), name);
                    break;
                case "--speed":
                    speed = ParsePositive(NextValue(args, ref i, name), name);
                    break;
                case "--duration":
                    duration = ParsePositive(NextValue(args, ref i, name), name);
                    break;
                case "--seed":
                    string value = NextValue(args, ref i, name);
                    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedSeed))
                        throw new ArgumentException($"--seed erwartet eine ganze Zahl, nicht '{value}'.");
                    seed = parsedSeed;
                    break;
                case "--output":
                    output = NextValue(args, ref i, name);
                    if (string.IsNullOrWhiteSpace(output) || output.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                        throw new ArgumentException($"--output ist kein gültiger Ordner: '{output}'.");
                    break;
                default:
                    throw new ArgumentException($"Unbekanntes Argument: '{args[i]}'.");
            }
        }

        return new SimulationConfig
        {
            DeviceId = id,
            PowerLimitKW = power,
            StepMinutes = step,
            Speed = speed,
            DurationHours = duration,
            Seed = seed,
            OutputDirectory = output,
        };
    }

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
