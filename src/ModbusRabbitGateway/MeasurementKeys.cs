using System.Globalization;
using System.Text;
using Modbus.Contracts;

namespace ModbusRabbitGateway;

/// <summary>Übersicht der Messwerte eines Plans: eine Zeile <c>adresse schlüssel | name | einheit</c> je Messwert.</summary>
public static class KeyCatalog
{
    public static IEnumerable<string> For(RegisterPlan plan) =>
        MessageBuilder.MeasuredRegisters(plan).Select(d =>
            $"{d.Address.ToString(CultureInfo.InvariantCulture),2} {MeasurementKeys.From(d.Name)} | {d.Name} | {d.Unit}".TrimEnd(' ', '|'));
}

/// <summary>
/// Stabiler, maschinenlesbarer Schlüssel je Messwert, abgeleitet aus dem deutschen Namen im Registerplan
/// (<c>"Punkt 1: SOC"</c> → <c>punkt_1_soc</c>). Nur <c>a–z</c>, <c>0–9</c> und <c>_</c>: so kann der Schlüssel später
/// ohne Escaping als InfluxDB-Feldname dienen. Die Schlüssel aller Pläne sind im Test
/// <c>MeasurementKeysTests</c> festgehalten; benennt jemand ein Register um, schlägt der Test an, statt dass eine
/// Zeitreihe still den Namen wechselt.
/// </summary>
public static class MeasurementKeys
{
    public static string From(string name)
    {
        var sb = new StringBuilder(name.Length + 4);
        bool pendingSeparator = false;

        foreach (char c in name.ToLowerInvariant())
        {
            string? mapped = c switch
            {
                'ä' => "ae",
                'ö' => "oe",
                'ü' => "ue",
                'ß' => "ss",
                _ when char.IsAsciiLetterLower(c) || char.IsAsciiDigit(c) => c.ToString(),
                _ => null
            };

            if (mapped is null)
            {
                pendingSeparator = sb.Length > 0;
                continue;
            }

            if (pendingSeparator)
                sb.Append('_');
            pendingSeparator = false;
            sb.Append(mapped);
        }

        return sb.ToString();
    }
}
