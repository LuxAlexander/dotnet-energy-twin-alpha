using System.Globalization;

namespace Shared.Data;

/// <summary>
/// Einheitliches CSV-Zahlenformat für alle Feldgeräte: de-AT (Dezimalkomma), damit Excel die Werte
/// nicht als Datum liest (z. B. „1.66" → „Jan 66"). Datum und Uhrzeit bleiben ISO-förmig und werden
/// mit InvariantCulture erzeugt.
/// </summary>
public static class CsvFormat
{
    /// <summary>Kultur für Zahlen in der CSV: Dezimalkomma (de-AT), unabhängig vom System.</summary>
    public static readonly CultureInfo DeAt = CultureInfo.GetCultureInfo("de-AT");

    /// <summary>Invariante Kultur für Datum, Uhrzeit und Lauf-Nummern (ISO-förmig).</summary>
    public static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    /// <summary>Zahl im de-AT-CSV-Format (Dezimalkomma).</summary>
    public static string Num(this double value, string format) => value.ToString(format, DeAt);
}
