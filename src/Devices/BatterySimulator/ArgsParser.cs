using System.Globalization;
using Devices.Common;
using Devices.Common.Modbus;
using Devices.Common.Weather;

namespace Batterie_Sim;

/// <summary>Liest die Kommandozeile in <see cref="SimulatorOptions"/>.</summary>
public static class ArgsParser
{
    public static readonly string Usage = $"""
        Batteriespeicher-Simulator

        Aufruf: dotnet run --project src/Devices/BatterySimulator -- [Optionen]

          --id <text>              Geräte-ID                       (Standard: BAT-01)
          --capacity <kWh>         Kapazität                       (Standard: 20)
          --max-charge <kW>        maximale Ladeleistung           (Standard: 5)
          --max-discharge <kW>     maximale Entladeleistung        (Standard: 5)
          --soc-min <%>            untere SOC-Grenze               (Standard: 10)
          --soc-max <%>            obere SOC-Grenze                (Standard: 100)
          --soc-start <%>          Start-SOC                       (Standard: 50)
          --step <s>               Schrittweite in Sekunden        (Standard: 60)
          --seed <zahl>            Zufalls-Seed für das Tagesprofil (optional)
          --duration <h>           Nur standalone: Dauer in Stunden (Standard: endlos)
          --help                   Diese Hilfe

        Virtuelle Zeit (Standard: vom Simulations-Master):
        {TimeSyncOptions.Usage}

        Modbus-TCP-Server (jeder Speicher braucht einen eigenen Port):
        {ModbusOptions.Usage}

        Wetter (optional; Aussentemperatur für das Temperaturmodell):
        {WeatherOptions.Usage}

        Tasten: Leertaste Pause (nur standalone) · M Auto/Manuell · ↑/↓ Sollwert ±0,5 kW · 0 Sollwert 0 · E Ein/Aus
                N Not-Aus · H Überhitzung · R Störung zurücksetzen · S Einstellungen · < > Tempo (nur standalone) · Q Ende

        Beispiele:
          dotnet run --project src/Devices/BatterySimulator -- --id BAT-01
          dotnet run --project src/Devices/BatterySimulator -- --standalone --speed 600 --start "2026-01-15 06:00" --duration 24
        """;

    public static bool IsHelpRequested(string[] args) => args.Any(a => a is "--help" or "-h" or "/?");

    /// <exception cref="ArgumentException">Ungültige oder unbekannte Option.</exception>
    public static SimulatorOptions Parse(string[] args)
    {
        var defaults = new SimulatorOptions();
        var config = new BatterieConfig();
        double step = defaults.StepSeconds;
        double? duration = null;
        int? seed = null;
        var timeSync = defaults.TimeSync;
        var modbus = defaults.Modbus;
        var weather = defaults.Weather;

        for (int i = 0; i < args.Length; i++)
        {
            string name = args[i].ToLowerInvariant();
            int index = i;

            if (TimeSyncOptions.TryApply(name, () => NextValue(args, ref index, name), timeSync, out timeSync)
                || ModbusOptions.TryApply(name, () => NextValue(args, ref index, name), modbus, out modbus)
                || WeatherOptions.TryApply(name, () => NextValue(args, ref index, name), weather, out weather))
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
                case "--capacity": config.CapacityKWh = ParseNumber(NextValue(args, ref i, name), name); break;
                case "--max-charge": config.MaxChargeKW = ParseNumber(NextValue(args, ref i, name), name); break;
                case "--max-discharge": config.MaxDischargeKW = ParseNumber(NextValue(args, ref i, name), name); break;
                case "--soc-min": config.SocMinPercent = ParseNumber(NextValue(args, ref i, name), name); break;
                case "--soc-max": config.SocMaxPercent = ParseNumber(NextValue(args, ref i, name), name); break;
                case "--soc-start": config.StartSocPercent = ParseNumber(NextValue(args, ref i, name), name); break;
                case "--step": step = ParseNumber(NextValue(args, ref i, name), name); break;
                case "--duration": duration = ParseNumber(NextValue(args, ref i, name), name); break;
                case "--seed":
                    string value = NextValue(args, ref i, name);
                    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
                        throw new ArgumentException($"--seed erwartet eine ganze Zahl, nicht '{value}'.");
                    seed = parsed;
                    break;
                default:
                    throw new ArgumentException($"Unbekanntes Argument: '{args[i]}'.");
            }
        }

        Validate(config, step);

        if (!timeSync.Standalone && (duration is not null || timeSync.HasStandaloneSettings))
            throw new ArgumentException(
                "--duration, --start und --speed gelten nur mit --standalone; mit Master bestimmt der Simulations-Master Zeit und Tempo.");
        if (duration is <= 0)
            throw new ArgumentException("--duration muss größer als 0 sein.");

        return new SimulatorOptions
        {
            Config = config,
            StepSeconds = step,
            DurationHours = duration,
            Seed = seed,
            TimeSync = timeSync,
            Modbus = modbus,
            Weather = weather.WithDefaultUrl(timeSync.Standalone),
        };
    }

    private static void Validate(BatterieConfig config, double step)
    {
        if (step <= 0)
            throw new ArgumentException("--step muss größer als 0 sein.");
        if (config.CapacityKWh <= 0)
            throw new ArgumentException("--capacity muss größer als 0 sein.");
        if (config.MaxChargeKW <= 0 || config.MaxDischargeKW <= 0)
            throw new ArgumentException("--max-charge und --max-discharge müssen größer als 0 sein.");
        if (config.SocMinPercent < 0 || config.SocMaxPercent > 100 || config.SocMinPercent >= config.SocMaxPercent)
            throw new ArgumentException("Es muss gelten: 0 ≤ --soc-min < --soc-max ≤ 100.");
        if (config.StartSocPercent < config.SocMinPercent || config.StartSocPercent > config.SocMaxPercent)
            throw new ArgumentException("--soc-start muss zwischen --soc-min und --soc-max liegen.");
    }

    private static string NextValue(string[] args, ref int i, string name)
    {
        if (i + 1 >= args.Length)
            throw new ArgumentException($"Für {name} fehlt ein Wert.");
        return args[++i];
    }

    /// <summary>Akzeptiert Punkt und Komma als Dezimaltrenner.</summary>
    private static double ParseNumber(string value, string name)
    {
        if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double result) || !double.IsFinite(result))
            throw new ArgumentException($"{name} erwartet eine Zahl, nicht '{value}'.");
        return result;
    }
}
