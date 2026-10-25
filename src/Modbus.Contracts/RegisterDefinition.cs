using System.Globalization;

namespace Modbus.Contracts;

/// <summary>Datentyp eines Registers oder einer Registergruppe (alle Werte stehen in Holding Registers, Big-Endian).</summary>
public enum RegisterType
{
    /// <summary>Ein 16-Bit-Register ohne Vorzeichen.</summary>
    UInt16,

    /// <summary>Zwei Register, 32 Bit ohne Vorzeichen, höherwertiges Wort zuerst.</summary>
    UInt32,

    /// <summary>Zwei Register, IEEE-754 float, höherwertiges Wort zuerst (Big-Endian, "ABCD").</summary>
    Float32
}

public enum RegisterAccess
{
    ReadOnly,

    /// <summary>Vom Diagnose-Client beschreibbar; Werte außerhalb des gültigen Bereichs werden abgelehnt.</summary>
    ReadWrite
}

/// <summary>
/// Ein Eintrag im Registerplan. <paramref name="Address"/> ist die Protokolladresse (0-basiert, wie im Modbus-Telegramm).
/// Der technische Wert ergibt sich aus dem Rohwert mal <paramref name="Scale"/>.
/// </summary>
/// <param name="Min">Kleinster gültiger Wert (technisch, nach Skalierung); <c>null</c> = nur der Datentyp begrenzt.</param>
/// <param name="Max">Größter gültiger Wert (technisch, nach Skalierung).</param>
/// <param name="Labels">Klartext zu ganzzahligen Werten (Index = Wert), z. B. für Zustände; für die Anzeige im Diagnose-Client.</param>
public sealed record RegisterDefinition(
    int Address,
    string Name,
    RegisterType Type,
    RegisterAccess Access,
    string Unit,
    string Description,
    double Scale = 1.0,
    double? Min = null,
    double? Max = null,
    IReadOnlyList<string>? Labels = null)
{
    /// <summary>Anzahl der belegten 16-Bit-Register.</summary>
    public int Count => Type == RegisterType.UInt16 ? 1 : 2;

    public int EndAddress => Address + Count;

    public bool IsWritable => Access == RegisterAccess.ReadWrite;

    /// <summary>Klartext zum Wert, falls <see cref="Labels"/> ihn kennt.</summary>
    public string? LabelFor(double value) =>
        Labels is not null && value >= 0 && value < Labels.Count && value == Math.Floor(value) ? Labels[(int)value] : null;

    public bool Contains(int address) => address >= Address && address < EndAddress;

    /// <summary>Prüft einen technischen Wert gegen Datentyp und Bereich.</summary>
    /// <returns><c>null</c> wenn gültig, sonst die Begründung.</returns>
    public string? Validate(double value)
    {
        if (!double.IsFinite(value))
            return "Wert ist keine endliche Zahl";

        double raw = value / Scale;
        if (Type != RegisterType.Float32)
        {
            if (Math.Abs(raw - Math.Round(raw)) > 1e-9)
                return "Wert muss ganzzahlig (im Raster der Skalierung) sein";
            double max = Type == RegisterType.UInt16 ? ushort.MaxValue : uint.MaxValue;
            if (raw < 0 || raw > max)
                return "Wert passt nicht in den Datentyp";
        }

        if ((Min is { } min && value < min) || (Max is { } upper && value > upper))
            return $"außerhalb des gültigen Bereichs {FormatLimit(Min)} … {FormatLimit(Max)} {Unit}".TrimEnd();

        return null;
    }

    private static string FormatLimit(double? limit) =>
        limit?.ToString("0.###", CultureInfo.InvariantCulture) ?? "–";
}
