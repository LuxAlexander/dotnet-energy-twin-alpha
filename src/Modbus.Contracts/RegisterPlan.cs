using System.Globalization;
using System.Text;

namespace Modbus.Contracts;

/// <summary>
/// Registerplan eines Geräts: gemeinsamer Kopf (<see cref="CommonRegisters.Header"/>) plus gerätespezifische Register
/// ab <see cref="CommonRegisters.SpecificStart"/>. Prüft beim Anlegen auf Überschneidungen und Bereichsverletzungen.
/// </summary>
public sealed class RegisterPlan
{
    private readonly RegisterDefinition?[] _byAddress = new RegisterDefinition?[CommonRegisters.TotalRegisters];

    public RegisterPlan(DeviceType deviceType, string deviceName, IEnumerable<RegisterDefinition> specific)
    {
        DeviceType = deviceType;
        DeviceName = deviceName;

        var all = CommonRegisters.Header.ToList();
        foreach (var definition in specific)
        {
            if (definition.Address < CommonRegisters.SpecificStart || definition.EndAddress > CommonRegisters.SpecificEnd)
                throw new ArgumentException(
                    $"{definition.Name}: Adresse {definition.Address} liegt nicht im gerätespezifischen Bereich {CommonRegisters.SpecificStart}–{CommonRegisters.SpecificEnd - 1}.");
            all.Add(definition);
        }

        foreach (var definition in all)
            for (int address = definition.Address; address < definition.EndAddress; address++)
            {
                if (_byAddress[address] is { } existing)
                    throw new ArgumentException($"Register {address} doppelt belegt: {existing.Name} und {definition.Name}.");
                _byAddress[address] = definition;
            }

        Definitions = all.OrderBy(d => d.Address).ToList();
    }

    public DeviceType DeviceType { get; }
    public string DeviceName { get; }
    public IReadOnlyList<RegisterDefinition> Definitions { get; }

    /// <summary>Eintrag, der die Adresse belegt; <c>null</c> bei einer Lücke oder außerhalb des Plans.</summary>
    public RegisterDefinition? Find(int address) =>
        address >= 0 && address < _byAddress.Length ? _byAddress[address] : null;

    /// <summary>Eintrag, der genau bei <paramref name="startAddress"/> beginnt.</summary>
    public RegisterDefinition Get(int startAddress) =>
        Definitions.FirstOrDefault(d => d.Address == startAddress)
        ?? throw new ArgumentException($"Kein Register beginnt bei Adresse {startAddress}.", nameof(startAddress));

    /// <summary>Registerplan als Markdown-Tabelle für die README des Geräts.</summary>
    public string ToMarkdown(string host = "127.0.0.1", int port = 1502, int unitId = 1)
    {
        var c = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();
        sb.AppendLine($"Holding Registers (Funktionscode 03 lesen, 06/16 schreiben). Standard: Host `{host}`, Port `{port}`, Unit-ID `{unitId}` – alle drei sind konfigurierbar.");
        sb.AppendLine("Adressen sind Protokolladressen (0-basiert, Referenzadresse = 40001 + Adresse). Pro Register Big-Endian;");
        sb.AppendLine("mehrteilige Werte (`UInt32`, `Float32`) belegen zwei Register, das höherwertige Wort steht zuerst (Float32 = IEEE 754, Byte-Folge ABCD).");
        sb.AppendLine("Mehrteilige Werte müssen mit **einem** Schreibzugriff (FC16) auf beide Register geschrieben werden.");
        sb.AppendLine();
        sb.AppendLine("| Adresse | Ref. | Name | Typ | Regs | Einheit | Zugriff | Schreibbereich | Bedeutung / Fehlerverhalten |");
        sb.AppendLine("|---|---|---|---|---|---|---|---|---|");
        foreach (var d in Definitions)
        {
            string address = d.Count == 1 ? d.Address.ToString(c) : $"{d.Address}–{d.EndAddress - 1}";
            string reference = (40001 + d.Address).ToString(c);
            string range = d.IsWritable
                ? $"{d.Min?.ToString("0.###", c) ?? "–"} … {d.Max?.ToString("0.###", c) ?? "–"}"
                : "–";
            string access = d.IsWritable ? "lesen/schreiben" : "lesen";
            string unit = d.Scale == 1.0 ? d.Unit : $"{d.Unit} (Rohwert × {d.Scale.ToString(c)})";
            sb.AppendLine($"| {address} | {reference} | {d.Name} | {d.Type} | {d.Count} | {unit} | {access} | {range} | {d.Description} |");
        }
        return sb.ToString();
    }
}
