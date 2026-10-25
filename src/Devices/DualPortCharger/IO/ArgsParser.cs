using System.Globalization;
using Devices.Common;
using Devices.Common.Modbus;
using DualChargerSim.Models;
using DualChargerSim.Simulation;

namespace DualChargerSim.IO;

/// <summary>
/// Liest die Kommandozeilenargumente in eine SimulationConfig ein.
/// </summary>
public static class ArgsParser
{
    public static readonly string Usage = $"""
        Aufruf: dotnet run --project src/Devices/DualPortCharger -- [Optionen]

          --id <text>        Geräte-ID                      (Standard: DC-01)
          --power <kW>       Gesamtleistung der Säule, 50–350 (Standard: 150)
          --step <min>       Schrittweite in Minuten        (Standard: 5)
          --seed <zahl>      Zufalls-Seed für wiederholbare Läufe (optional)
          --output <ordner>  Zielordner der CSV-Datei       (Standard: docs/data im Repo)
          --help             Diese Hilfe anzeigen

        Virtuelle Zeit (Standard: vom Simulations-Master):
        {TimeSyncOptions.Usage}
          --duration <h>        Nur standalone: Simulationsdauer in Stunden (Standard: endlos)

        Modbus-TCP-Server (jede Säule braucht einen eigenen Port):
        {ModbusOptions.Usage}

        Beispiele:
          dotnet run --project src/Devices/DualPortCharger -- --id DC-01 --power 150 --step 1 --seed 42
          dotnet run --project src/Devices/DualPortCharger -- --standalone --speed 300 --duration 24 --seed 42
          dotnet run --project src/Devices/DualPortCharger -- --id DC-02 --modbus-port 1503
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
        double? duration = null;
        int? seed = null;
        string output = defaults.OutputDirectory;
        var timeSync = defaults.TimeSync;
        var modbus = defaults.Modbus;

        for (int i = 0; i < args.Length; i++)
        {
            string name = args[i].ToLowerInvariant();
            int index = i;
            if (TimeSyncOptions.TryApply(name, () => NextValue(args, ref index, name), timeSync, out timeSync))
            {
                i = index;
                continue;
            }

            if (ModbusOptions.TryApply(name, () => NextValue(args, ref index, name), modbus, out modbus))
            {
                i = index;
                continue;
            }

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

        if (!timeSync.Standalone && (duration is not null || timeSync.HasStandaloneSettings))
            throw new ArgumentException(
                "--duration, --start und --speed gelten nur mit --standalone; mit Master bestimmt der Simulations-Master Zeit und Tempo.");

        return new SimulationConfig
        {
            DeviceId = id,
            PowerLimitKW = power,
            StepMinutes = step,
            DurationHours = duration,
            Seed = seed,
            OutputDirectory = output,
            TimeSync = timeSync,
            Modbus = modbus,
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
