using System.Globalization;
using Devices.Common;
using Devices.Common.Modbus;
using Devices.Common.Weather;
using KuehlhausSimulator.Simulation;

namespace KuehlhausSimulator.IO;

/// <summary>Liest die Kommandozeilenargumente in eine <see cref="SimulationConfig"/>.</summary>
public static class ArgsParser
{
    public static readonly string Usage = $"""
        KuehlhausSimulator – Kühlhaus (Industrieverbraucher)

        Aufruf: dotnet run --project src/Devices/KuehlhausSimulator -- [Optionen]

          --id <text>            Geräte-ID                             (Standard: KH-01)
          --setpoint <°C>        Solltemperatur -30…0 °C               (Standard: -20)
          --cooling <kW>         Kälteleistung des Kompressors         (Standard: 6)
          --step <min>           Schrittweite in Minuten              (Standard: 5)
          --seed <zahl>          Zufalls-Seed für wiederholbare Läufe (optional)
          --output <ordner>      Zielordner der CSV-Datei             (Standard: docs/data im Repo)
          --help                 Diese Hilfe anzeigen

        Virtuelle Zeit (Standard: vom Simulations-Master):
        {TimeSyncOptions.Usage}
          --duration <h>        Nur standalone: Simulationsdauer in Stunden (Standard: endlos)

        Modbus-TCP-Server (jedes Gerät braucht einen eigenen Port, Standard hier {SimulationConfig.DefaultModbusPort}):
          --modbus-port <port>  Port des Modbus-TCP-Servers, 1024–65535 (Standard: {SimulationConfig.DefaultModbusPort})
          --modbus-bind <ip>    Adresse, auf der der Server lauscht (Standard: 127.0.0.1; 0.0.0.0 = alle Netzwerkkarten)
          --unit-id <1-247>     Modbus-Unit-ID (Standard: 1)
          --no-modbus           Modbus-Server nicht starten
          --register-plan       Registerplan als Markdown ausgeben und beenden

        Wetter (optional; Außentemperatur des Kühlraums):
        {WeatherOptions.Usage}

        Beispiele:
          dotnet run --project src/Devices/KuehlhausSimulator -- --id KH-01 --setpoint -18 --weather off
          dotnet run --project src/Devices/KuehlhausSimulator -- --standalone --speed 6000 --duration 24 --seed 42
          dotnet run --project src/Devices/KuehlhausSimulator -- --register-plan
        """;

    public static bool IsHelpRequested(string[] args) => args.Any(a => a is "--help" or "-h" or "/?");

    /// <exception cref="ArgumentException">Ungültige oder unbekannte Option.</exception>
    public static SimulationConfig Parse(string[] args)
    {
        var defaults = new SimulationConfig();
        string id = defaults.DeviceId;
        double setpoint = defaults.SetpointC;
        double cooling = defaults.CoolingPowerKW;
        double step = defaults.StepMinutes;
        double? duration = null;
        int? seed = null;
        string output = defaults.OutputDirectory;
        var timeSync = defaults.TimeSync;
        var modbus = defaults.Modbus;
        var weather = defaults.Weather;

        for (int i = 0; i < args.Length; i++)
        {
            string name = args[i].ToLowerInvariant();
            int index = i;
            if (TimeSyncOptions.TryApply(name, () => NextValue(args, ref index, name), timeSync, out timeSync))
            {
                i = index;
                continue;
            }

            if (ModbusOptions.TryApply(name, () => NextValue(args, ref index, name), modbus, out modbus)
                || WeatherOptions.TryApply(name, () => NextValue(args, ref index, name), weather, out weather))
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

                case "--setpoint":
                    setpoint = ParseNumber(NextValue(args, ref i, name), name);
                    if (setpoint < Kuehlhaus.MinSetpointC || setpoint > Kuehlhaus.MaxSetpointC)
                        throw new ArgumentException(
                            $"--setpoint muss zwischen {Kuehlhaus.MinSetpointC} und {Kuehlhaus.MaxSetpointC} °C liegen (angegeben: {args[i]}).");
                    break;

                case "--cooling":
                    cooling = ParsePositive(NextValue(args, ref i, name), name);
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
            SetpointC = setpoint,
            CoolingPowerKW = cooling,
            StepMinutes = step,
            DurationHours = duration,
            Seed = seed,
            OutputDirectory = output,
            TimeSync = timeSync,
            Modbus = modbus,
            Weather = weather.WithDefaultUrl(timeSync.Standalone),
        };
    }

    private static string NextValue(string[] args, ref int i, string name)
    {
        if (i + 1 >= args.Length)
            throw new ArgumentException($"Für {name} fehlt ein Wert.");
        return args[++i];
    }

    /// <summary>Akzeptiert Punkt und Komma als Dezimaltrenner; Vorzeichen erlaubt.</summary>
    private static double ParseNumber(string value, string name)
    {
        if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double result)
            || !double.IsFinite(result))
            throw new ArgumentException($"{name} erwartet eine Zahl, nicht '{value}'.");
        return result;
    }

    private static double ParsePositive(string value, string name)
    {
        double result = ParseNumber(value, name);
        if (result <= 0)
            throw new ArgumentException($"{name} muss größer als 0 sein (angegeben: {value}).");
        return result;
    }
}
