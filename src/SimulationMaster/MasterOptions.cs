using System.Globalization;
using System.Net;
using SimCore.Contracts;
using SimCore.Time;
using SimCore.TimeSync;

namespace SimulationMaster;

/// <summary>Startparameter des Simulations-Masters (aus der Kommandozeile).</summary>
public sealed record MasterOptions
{
    public static readonly string[] DateFormats = ["yyyy-MM-dd HH:mm", "yyyy-MM-ddTHH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd"];

    public IPAddress BindAddress { get; init; } = IPAddress.Loopback;
    public int Port { get; init; } = SyncProtocol.DefaultPort;
    public DateTime StartTime { get; init; } = new(DateTime.Today.Year, 4, 15, 6, 0, 0);
    public double Speed { get; init; } = 60;
    public TimeSpan LeadTime { get; init; } = MasterClock.DefaultLeadTime;
    public TimeSpan HeartbeatInterval { get; init; } = TimeSyncServer.DefaultHeartbeatInterval;

    /// <summary>Zulässige Abweichung in realer Zeit; in Sim-Zeit = Toleranz · Tempo.</summary>
    public TimeSpan Tolerance { get; init; } = TimeSpan.FromMilliseconds(10);

    public bool AutoStart { get; init; }

    public const string Usage = """
        Simulations-Master – besitzt die gemeinsame virtuelle Zeit

        Aufruf: dotnet run --project src/SimulationMaster -- [Optionen]

          --bind <ip>          Adresse des Zeitservers (Standard 127.0.0.1, für andere Rechner 0.0.0.0)
          --port <n>           TCP-Port des Zeitservers (Standard 5100)
          --start <zeit>       Virtuelle Startzeit "yyyy-MM-dd HH:mm" (Standard: 15.04. dieses Jahres, 06:00)
          --speed <x>          Tempo, virtuelle Sekunden je realer Sekunde, 1–100000 (Standard 60)
          --lead <ms>          Vorlaufzeit für Änderungen in ms, 10–5000 (Standard 200)
          --heartbeat <ms>     Heartbeat-Intervall in ms, 100–10000 (Standard 1000)
          --tolerance <ms>     Zulässige Abweichung in realen ms, Sim-Toleranz = Wert · Tempo (Standard 10)
          --autostart          Simulation sofort starten (sonst pausiert bis Leertaste)
          --help               Diese Hilfe
        """;

    /// <exception cref="ArgumentException">Unbekannte Option oder ungültiger Wert.</exception>
    public static MasterOptions Parse(string[] args)
    {
        var options = new MasterOptions();
        for (int i = 0; i < args.Length; i++)
        {
            string name = args[i].ToLowerInvariant();
            options = name switch
            {
                "--bind" => options with { BindAddress = ParseAddress(Value(args, ref i)) },
                "--port" => options with { Port = (int)ParseNumber(name, Value(args, ref i), 1024, 65535) },
                "--start" => options with { StartTime = ParseTime(Value(args, ref i)) },
                "--speed" => options with { Speed = ParseNumber(name, Value(args, ref i), MasterClock.MinSpeed, MasterClock.MaxSpeed) },
                "--lead" => options with { LeadTime = TimeSpan.FromMilliseconds(ParseNumber(name, Value(args, ref i), 10, 5000)) },
                "--heartbeat" => options with { HeartbeatInterval = TimeSpan.FromMilliseconds(ParseNumber(name, Value(args, ref i), 100, 10_000)) },
                "--tolerance" => options with { Tolerance = TimeSpan.FromMilliseconds(ParseNumber(name, Value(args, ref i), 0.1, 10_000)) },
                "--autostart" => options with { AutoStart = true },
                _ => throw new ArgumentException($"Unbekannte Option: {args[i]}")
            };
        }
        return options;
    }

    /// <summary>Liest eine Zeit in einem der <see cref="DateFormats"/>.</summary>
    public static bool TryParseTime(string text, out DateTime time) =>
        DateTime.TryParseExact(text.Trim(), DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out time);

    /// <summary>Liest eine Zahl mit Punkt oder Komma als Dezimaltrennzeichen.</summary>
    public static bool TryParseNumber(string text, out double value) =>
        double.TryParse(text.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out value)
        && double.IsFinite(value);

    private static string Value(string[] args, ref int i)
    {
        if (i + 1 >= args.Length)
            throw new ArgumentException($"Wert fehlt für {args[i]}");
        return args[++i];
    }

    private static double ParseNumber(string name, string text, double min, double max)
    {
        if (!TryParseNumber(text, out double value))
            throw new ArgumentException($"{name}: '{text}' ist keine Zahl");
        if (value < min || value > max)
            throw new ArgumentException($"{name}: {text} liegt nicht zwischen {min} und {max}");
        return value;
    }

    private static DateTime ParseTime(string text) =>
        TryParseTime(text, out var time)
            ? time
            : throw new ArgumentException($"--start: '{text}' hat nicht das Format yyyy-MM-dd HH:mm");

    private static IPAddress ParseAddress(string text) =>
        IPAddress.TryParse(text, out var address)
            ? address
            : throw new ArgumentException($"--bind: '{text}' ist keine IP-Adresse");
}
