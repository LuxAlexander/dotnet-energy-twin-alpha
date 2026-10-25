using System.Globalization;
using System.Text;
using Modbus.Contracts;

namespace ModbusDiagnosticClient;

/// <summary>Stellt Register als Rohwert und interpretierter Wert dar (zum Nachvollziehen von Rohdaten und Deutung).</summary>
public static class RegisterFormatter
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

    /// <summary>Rohwert der Register: <c>0x4216 0x0000</c>.</summary>
    public static string Raw(ReadOnlySpan<ushort> registers)
    {
        var sb = new StringBuilder();
        foreach (ushort register in registers)
        {
            if (sb.Length > 0)
                sb.Append(' ');
            sb.Append("0x").Append(register.ToString("X4", CultureInfo.InvariantCulture));
        }
        return sb.ToString();
    }

    /// <summary>Interpretierter Wert mit Einheit und gegebenenfalls Klartext, z. B. <c>2 = Betrieb</c> oder <c>37,5 kW</c>.</summary>
    public static string Value(RegisterDefinition definition, ReadOnlySpan<ushort> registers)
    {
        double value = RegisterCodec.Decode(definition, registers);

        if (definition.Address == CommonRegisters.ModelTime)
            return registers[0] == 0 && registers[1] == 0
                ? "– (noch keine Zeit)"
                : RegisterCodec.FromSeconds((uint)value).ToString("ddd yyyy-MM-dd HH:mm:ss", Culture);

        if (definition.Address == CommonRegisters.ErrorCode)
            return value == 0 ? "0 = kein Fehler" : $"{value:0} (Bedeutung im Registerplan des Geräts)";

        string label = definition.LabelFor(value) is { } text ? $" = {text}" : "";
        string unit = definition.Unit.Length > 0 ? $" {definition.Unit}" : "";
        return definition.Type == RegisterType.Float32
            ? $"{Number(value)}{unit}"
            : $"{value.ToString("0.###", Culture)}{label}{unit}";
    }

    /// <summary>Kurzform für einzeilige Anzeigen: nur der Klartext, falls vorhanden, sonst der Wert.</summary>
    public static string Short(RegisterDefinition definition, ReadOnlySpan<ushort> registers) =>
        definition.LabelFor(RegisterCodec.Decode(definition, registers)) ?? Value(definition, registers);

    /// <summary>Tabelle der Register von <paramref name="definitions"/>; <paramref name="registers"/> ist mit der Protokolladresse indiziert.</summary>
    public static string Table(IEnumerable<RegisterDefinition> definitions, IReadOnlyList<ushort> registers)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"  {"Adr",-7}{"Name",-38}{"Rohwert",-15}  Wert");
        foreach (var d in definitions)
        {
            if (d.EndAddress > registers.Count)
                continue;
            var raw = registers.Skip(d.Address).Take(d.Count).ToArray();
            string address = d.Count == 1 ? d.Address.ToString(Culture) : $"{d.Address}–{d.EndAddress - 1}";
            string access = d.IsWritable ? " [schreibbar]" : "";
            sb.AppendLine($"  {address,-7}{d.Name,-38}{Raw(raw),-15}  {Value(d, raw)}{access}");
        }
        return sb.ToString();
    }

    private static string Number(double value) => value.ToString(Math.Abs(value) >= 100 ? "0.#" : "0.##", Culture);
}
