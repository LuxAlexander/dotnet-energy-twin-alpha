using System.Globalization;
using SimCore.Contracts;
using SimCore.Time;

namespace Devices.Common;

/// <summary>
/// Gemeinsame Startoptionen aller Geräte für die virtuelle Zeit: Verbindung zum Simulations-Master
/// oder Standalone-Betrieb mit lokaler Uhr (zum Testen ohne Master, z. B. für Modbus).
/// </summary>
public sealed record TimeSyncOptions
{
    public const string DefaultHost = "127.0.0.1";

    public static readonly string[] DateFormats = ["yyyy-MM-dd HH:mm", "yyyy-MM-ddTHH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd"];

    public const string Usage = """
          --master <host:port>  Simulations-Master (Standard: 127.0.0.1:5100)
          --standalone          Ohne Master mit lokaler Uhr laufen (Test, z. B. Modbus)
          --start <zeit>        Nur standalone: virtuelle Startzeit "yyyy-MM-dd HH:mm" (Standard: heute 00:00)
          --speed <faktor>      Nur standalone: Tempo gegenüber Echtzeit, 1–100000
        """;

    public bool Standalone { get; init; }
    public string MasterHost { get; init; } = DefaultHost;
    public int MasterPort { get; init; } = SyncProtocol.DefaultPort;
    public DateTime StandaloneStart { get; init; } = DateTime.Today;
    public double StandaloneSpeed { get; init; } = 60;

    /// <summary>Wurde <c>--start</c> oder <c>--speed</c> angegeben? (wirkt nur standalone, sonst Hinweis)</summary>
    public bool HasStandaloneSettings { get; init; }

    public string MasterEndpoint => $"{MasterHost}:{MasterPort}";

    /// <summary>
    /// Verarbeitet eine Zeitoption, falls <paramref name="name"/> eine ist.
    /// </summary>
    /// <param name="nextValue">Liefert den Wert hinter der Option (wirft, wenn er fehlt).</param>
    /// <returns><c>false</c>, wenn die Option nicht zur Zeit gehört.</returns>
    /// <exception cref="ArgumentException">Ungültiger Wert.</exception>
    public static bool TryApply(string name, Func<string> nextValue, TimeSyncOptions current, out TimeSyncOptions updated)
    {
        updated = name.ToLowerInvariant() switch
        {
            "--master" => ParseEndpoint(nextValue(), current),
            "--standalone" => current with { Standalone = true },
            "--start" => current with { StandaloneStart = ParseTime(nextValue()), HasStandaloneSettings = true },
            "--speed" => current with { StandaloneSpeed = ParseSpeed(nextValue()), HasStandaloneSettings = true },
            _ => current
        };
        return !ReferenceEquals(updated, current);
    }

    private static TimeSyncOptions ParseEndpoint(string text, TimeSyncOptions current)
    {
        string host = text;
        int port = SyncProtocol.DefaultPort;
        int colon = text.LastIndexOf(':');
        if (colon >= 0)
        {
            host = text[..colon];
            if (!int.TryParse(text[(colon + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out port)
                || port is < 1 or > 65535)
                throw new ArgumentException($"--master: Port in '{text}' ist ungültig (1–65535).");
        }
        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentException($"--master: Host fehlt in '{text}' (Format host:port).");
        return current with { MasterHost = host, MasterPort = port };
    }

    private static DateTime ParseTime(string text) =>
        DateTime.TryParseExact(text.Trim(), DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time)
            ? time
            : throw new ArgumentException($"--start: '{text}' hat nicht das Format yyyy-MM-dd HH:mm.");

    private static double ParseSpeed(string text)
    {
        if (!double.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double speed)
            || !double.IsFinite(speed))
            throw new ArgumentException($"--speed erwartet eine Zahl, nicht '{text}'.");
        if (speed < MasterClock.MinSpeed || speed > MasterClock.MaxSpeed)
            throw new ArgumentException($"--speed muss zwischen {MasterClock.MinSpeed} und {MasterClock.MaxSpeed} liegen (angegeben: {text}).");
        return speed;
    }
}
