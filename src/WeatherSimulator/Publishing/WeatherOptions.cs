using Devices.Common;
using Shared.Config;

namespace WeatherSimulator.Publishing;

/// <summary>HTTP bleibt eigenständig; nur --mqtt aktiviert Master-Anbindung und Veröffentlichung.</summary>
public sealed class WeatherOptions
{
    public int Port { get; private set; } = 8080;
    public bool Mqtt { get; private set; }
    public string BrokerHost { get; private set; } = "127.0.0.1";
    public int BrokerPort { get; private set; } = 1883;
    public string? EnvFile { get; private set; } = OutputPaths.RepositoryRoot is { } root ? Path.Combine(root, "docker", ".env") : null;
    public TimeSyncOptions TimeSync { get; private set; } = new();

    public static WeatherOptions Parse(string[] args)
    {
        var result = new WeatherOptions();
        for (int i = 0; i < args.Length; i++)
        {
            string option = args[i];
            string Next() => ++i < args.Length ? args[i] : throw new ArgumentException($"Wert für {option} fehlt.");
            switch (option)
            {
                case "--mqtt": result.Mqtt = true; break;
                case "--port": result.Port = ParsePort(Next()); break;
                case "--env-file": result.EnvFile = Next(); break;
                case "--master":
                    TimeSyncOptions.TryApply(option, Next, result.TimeSync, out var time);
                    result.TimeSync = time;
                    break;
                case "--broker":
                    string endpoint = Next();
                    int colon = endpoint.LastIndexOf(':');
                    if (colon <= 0) throw new ArgumentException("--broker erwartet host:port.");
                    result.BrokerHost = endpoint[..colon];
                    result.BrokerPort = ParsePort(endpoint[(colon + 1)..]);
                    break;
                default: throw new ArgumentException($"Unbekanntes Argument: {option}");
            }
        }
        return result;
    }

    private static int ParsePort(string value) => int.TryParse(value, out int port) && port is >= 1 and <= 65535
        ? port : throw new ArgumentException("Port muss zwischen 1 und 65535 liegen.");
}
