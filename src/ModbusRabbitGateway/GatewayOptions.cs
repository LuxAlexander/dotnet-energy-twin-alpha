using System.Globalization;

namespace ModbusRabbitGateway;

/// <summary>Kommandozeilenoptionen des Gateways (Nachrichtenvertrag §5).</summary>
public sealed class GatewayOptions
{
    /// <summary>Standardgeräte wie in der Demo der zentralen README (Charger auf 1504, damit er nicht mit der PV kollidiert).</summary>
    public static IReadOnlyList<DeviceEndpoint> DefaultDevices { get; } =
    [
        new("PV-01", "127.0.0.1", 1502),
        new("BAT-01", "127.0.0.1", 1503),
        new("DC-01", "127.0.0.1", 1504),
        new("KH-01", "127.0.0.1", 1505),
    ];

    public IReadOnlyList<DeviceEndpoint> Devices { get; private set; } = DefaultDevices;
    public TimeSpan Interval { get; private set; } = TimeSpan.FromSeconds(1);
    public TimeSpan StaleModelTime { get; private set; } = QualitySettings.Default.StaleModelTime;
    public int TimeoutMs { get; private set; } = 1000;
    public string GatewayId { get; private set; } = "GW-01";
    public bool Once { get; private set; }
    public bool ShowJson { get; private set; }
    public bool ListKeys { get; private set; }

    /// <summary>Reale Mindestwartezeit für <c>stale</c>: drei Abfrageintervalle.</summary>
    public QualitySettings Quality => new(StaleModelTime, Interval * 3);

    public const string Usage = """
        Modbus–RabbitMQ-Gateway – liest die Feldgeräte über Modbus TCP und veröffentlicht sie nach docs/Nachrichtenvertrag-LB03.md.
        Derzeit Probebetrieb ohne Broker: Übersicht auf der Konsole, mit --json die vollständigen Nachrichten.

        Aufruf: dotnet run --project src/ModbusRabbitGateway -- [Optionen]

          --device ID=host:port[/unit]  Gerät anbinden, mehrfach möglich; ersetzt die Standardgeräte
                                        (Standard: PV-01=127.0.0.1:1502 BAT-01=…:1503 DC-01=…:1504 KH-01=…:1505)
          --interval <s>                Abfrage- und Veröffentlichungsintervall in Sekunden (Standard 1, mind. 0,1)
          --stale-after <min>           Modellzeit in Minuten ohne neue Gerätesequenz bis „stale“ (Standard 60)
          --timeout <ms>                Modbus-Zeitlimit je Gerät (Standard 1000)
          --gateway-id <ID>             Kennung für das Gateway-Status-Topic (Standard GW-01)
          --once                        jedes Gerät einmal lesen und beenden
          --json                        vollständige Nachrichten (Topic + JSON) ausgeben
          --keys                        Messwert-Schlüssel aller Registerpläne ausgeben und beenden
          --help                        diese Hilfe

        Beenden: Q oder Strg+C.
        """;

    public static bool IsHelpRequested(IEnumerable<string> args) =>
        args.Any(a => a is "--help" or "-h" or "-?" or "/?");

    /// <exception cref="ArgumentException">Unbekannte Option oder ungültiger Wert (deutsche Meldung).</exception>
    public static GatewayOptions Parse(IReadOnlyList<string> args)
    {
        var options = new GatewayOptions();
        var devices = new List<DeviceEndpoint>();

        for (int i = 0; i < args.Count; i++)
        {
            string name = args[i];
            switch (name)
            {
                case "--device":
                    devices.Add(DeviceEndpoint.Parse(Next(args, ref i, name)));
                    break;
                case "--interval":
                    double seconds = ParseNumber(Next(args, ref i, name), name);
                    if (seconds < 0.1)
                        throw new ArgumentException("--interval muss mindestens 0,1 s sein.");
                    options.Interval = TimeSpan.FromSeconds(seconds);
                    break;
                case "--stale-after":
                    double minutes = ParseNumber(Next(args, ref i, name), name);
                    if (minutes <= 0)
                        throw new ArgumentException("--stale-after muss größer als 0 sein.");
                    options.StaleModelTime = TimeSpan.FromMinutes(minutes);
                    break;
                case "--timeout":
                    double timeout = ParseNumber(Next(args, ref i, name), name);
                    if (timeout is < 100 or > 60_000 || timeout != Math.Floor(timeout))
                        throw new ArgumentException("--timeout muss eine ganze Zahl von 100 bis 60000 ms sein.");
                    options.TimeoutMs = (int)timeout;
                    break;
                case "--gateway-id":
                    string id = Next(args, ref i, name);
                    if (!id.All(c => char.IsAsciiLetterOrDigit(c) || c == '-'))
                        throw new ArgumentException($"Gateway-ID '{id}' ist ungültig: erlaubt sind Buchstaben, Ziffern und '-'.");
                    options.GatewayId = id;
                    break;
                case "--once":
                    options.Once = true;
                    break;
                case "--json":
                    options.ShowJson = true;
                    break;
                case "--keys":
                    options.ListKeys = true;
                    break;
                default:
                    throw new ArgumentException($"Unbekannte Option '{name}'.");
            }
        }

        if (devices.Count > 0)
        {
            var duplicate = devices.GroupBy(d => d.DeviceId, StringComparer.OrdinalIgnoreCase).FirstOrDefault(g => g.Count() > 1);
            if (duplicate is not null)
                throw new ArgumentException($"Geräte-ID '{duplicate.Key}' ist mehrfach angegeben.");
            options.Devices = devices;
        }

        return options;
    }

    private static string Next(IReadOnlyList<string> args, ref int i, string name) =>
        ++i < args.Count ? args[i] : throw new ArgumentException($"{name} braucht einen Wert.");

    /// <summary>Zahl mit Punkt oder Komma.</summary>
    private static double ParseNumber(string text, string name) =>
        double.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double value) && double.IsFinite(value)
            ? value
            : throw new ArgumentException($"{name}: '{text}' ist keine Zahl.");
}
