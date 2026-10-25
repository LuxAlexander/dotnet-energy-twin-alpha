using System.Globalization;
using System.Net;

namespace Devices.Common.Modbus;

/// <summary>
/// Gemeinsame Startoptionen des Modbus-TCP-Servers jedes Geräts (Host, Port, Unit-ID).
/// Mehrere Geräte auf einem Rechner brauchen verschiedene Ports.
/// </summary>
public sealed record ModbusOptions
{
    public const int DefaultPort = 1502;
    public const string DefaultBind = "127.0.0.1";

    public const string Usage = """
          --modbus-port <port>  Port des Modbus-TCP-Servers, 1024–65535 (Standard: 1502; jedes Gerät braucht einen eigenen)
          --modbus-bind <ip>    Adresse, auf der der Server lauscht (Standard: 127.0.0.1; 0.0.0.0 = alle Netzwerkkarten)
          --unit-id <1-247>     Modbus-Unit-ID (Standard: 1)
          --no-modbus           Modbus-Server nicht starten
          --register-plan       Registerplan als Markdown ausgeben und beenden
        """;

    public bool Disabled { get; init; }
    public string Bind { get; init; } = DefaultBind;
    public int Port { get; init; } = DefaultPort;
    public byte UnitId { get; init; } = 1;

    /// <summary>Wurde <c>--register-plan</c> angegeben? Das Gerät gibt dann den Plan aus und beendet sich.</summary>
    public bool PrintRegisterPlan { get; init; }

    public IPAddress BindAddress => IPAddress.Parse(Bind);

    /// <summary>Verarbeitet eine Modbus-Option, falls <paramref name="name"/> eine ist.</summary>
    /// <param name="nextValue">Liefert den Wert hinter der Option (wirft, wenn er fehlt).</param>
    /// <returns><c>false</c>, wenn die Option nicht zu Modbus gehört.</returns>
    /// <exception cref="ArgumentException">Ungültiger Wert.</exception>
    public static bool TryApply(string name, Func<string> nextValue, ModbusOptions current, out ModbusOptions updated)
    {
        updated = name.ToLowerInvariant() switch
        {
            "--modbus-port" => current with { Port = ParsePort(nextValue()) },
            "--modbus-bind" => current with { Bind = ParseBind(nextValue()) },
            "--unit-id" => current with { UnitId = ParseUnitId(nextValue()) },
            "--no-modbus" => current with { Disabled = true },
            "--register-plan" => current with { PrintRegisterPlan = true },
            _ => current
        };
        return !ReferenceEquals(updated, current);
    }

    private static int ParsePort(string text)
    {
        if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out int port) || port is < 1024 or > 65535)
            throw new ArgumentException($"--modbus-port muss eine Zahl von 1024 bis 65535 sein (angegeben: '{text}').");
        return port;
    }

    private static string ParseBind(string text)
    {
        if (!IPAddress.TryParse(text, out var address))
            throw new ArgumentException($"--modbus-bind erwartet eine IP-Adresse, nicht '{text}'.");
        return address.ToString();
    }

    private static byte ParseUnitId(string text)
    {
        if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out int id) || id is < 1 or > 247)
            throw new ArgumentException($"--unit-id muss eine Zahl von 1 bis 247 sein (angegeben: '{text}').");
        return (byte)id;
    }
}
