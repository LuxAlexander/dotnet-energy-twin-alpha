using System.Globalization;
using System.Text.RegularExpressions;
using Modbus.Contracts;

namespace ModbusRabbitGateway;

/// <summary>
/// Ein angebundenes Feldgerät: eindeutige Geräte-ID (wird Teil des Topics) und Modbus-Adresse.
/// Aus der ID ergeben sich die Werte, die das Gerät in Register 0 und 1 melden muss (<c>DC-01</c> ⇒ Charger, Nummer 1).
/// </summary>
public sealed partial record DeviceEndpoint(string DeviceId, string Host, int Port, byte UnitId = 1)
{
    /// <summary>Kürzel am Anfang der Geräte-ID und der zugehörige Gerätetyp (wie die Geräte-IDs in der zentralen README).</summary>
    private static readonly Dictionary<string, DeviceType> TypeByPrefix = new(StringComparer.OrdinalIgnoreCase)
    {
        ["PV"] = DeviceType.PvPlant,
        ["BAT"] = DeviceType.Battery,
        ["DC"] = DeviceType.Charger,
        ["KH"] = DeviceType.IndustrialConsumer,
    };

    public string Endpoint => $"{Host}:{Port.ToString(CultureInfo.InvariantCulture)}";

    /// <summary>Gerätetyp laut Kürzel der ID; <c>null</c>, wenn das Kürzel unbekannt ist (dann wird der Typ nicht geprüft).</summary>
    public DeviceType? ExpectedType
    {
        get
        {
            var match = PrefixPattern().Match(DeviceId);
            return match.Success && TypeByPrefix.TryGetValue(match.Value, out var type) ? type : null;
        }
    }

    /// <summary>Geräte-Nummer wie im Registerkopf: Zahl am Ende der ID (<c>DC-02</c> → 2), 0 wenn keine.</summary>
    public int ExpectedInstance
    {
        get
        {
            var match = TrailingNumber().Match(DeviceId);
            return match.Success && int.TryParse(match.Value, NumberStyles.None, CultureInfo.InvariantCulture, out int n)
                && n <= ushort.MaxValue ? n : 0;   // wie ModbusDeviceServer.ParseInstance
        }
    }

    /// <summary>Liest <c>ID=host:port</c> oder <c>ID=host:port/unit</c>, z. B. <c>DC-01=127.0.0.1:1504</c>.</summary>
    /// <exception cref="ArgumentException">Ungültige Angabe, mit deutscher Begründung.</exception>
    public static DeviceEndpoint Parse(string text)
    {
        int equals = text.IndexOf('=');
        if (equals <= 0)
            throw new ArgumentException($"'{text}': erwartet ID=host:port, z. B. DC-01=127.0.0.1:1504.");

        string id = text[..equals].Trim();
        if (!IdPattern().IsMatch(id))
            throw new ArgumentException($"Geräte-ID '{id}' ist ungültig: erlaubt sind Buchstaben, Ziffern und '-' (sie wird Teil des MQTT-Topics).");

        string address = text[(equals + 1)..].Trim();
        byte unit = 1;
        int slash = address.IndexOf('/');
        if (slash >= 0)
        {
            if (!byte.TryParse(address[(slash + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out unit) || unit is < 1 or > 247)
                throw new ArgumentException($"'{text}': Unit-ID muss 1–247 sein.");
            address = address[..slash];
        }

        int colon = address.LastIndexOf(':');
        if (colon <= 0 || !int.TryParse(address[(colon + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out int port)
            || port is < 1 or > 65535)
            throw new ArgumentException($"'{text}': Host und Port fehlen oder der Port ist ungültig (erwartet host:port).");

        return new DeviceEndpoint(id, address[..colon], port, unit);
    }

    public override string ToString() =>
        UnitId == 1 ? $"{DeviceId}={Endpoint}" : $"{DeviceId}={Endpoint}/{UnitId.ToString(CultureInfo.InvariantCulture)}";

    [GeneratedRegex("^[A-Za-z0-9-]+$")]
    private static partial Regex IdPattern();

    [GeneratedRegex("^[A-Za-z]+")]
    private static partial Regex PrefixPattern();

    [GeneratedRegex("[0-9]+$")]
    private static partial Regex TrailingNumber();
}
