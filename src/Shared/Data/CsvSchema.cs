namespace Shared.Data;

/// <summary>
/// Gemeinsame Grundspalten der Geräte-CSVs. Jedes Gerät beginnt seine CSV mit diesen Spalten und
/// hängt seine gerätespezifischen Spalten rechts an. Das Feld <see cref="ColumnDevice"/> enthält den
/// Anlagennamen (z. B. „PV-Anlage“), die Geräte-ID (z. B. PV-01) steht im Dateinamen.
/// </summary>
public static class CsvSchema
{
    public const string ColumnDevice = "Gerät";
    public const string ColumnDate = "Datum";
    public const string ColumnTime = "Zeit";
    public const string ColumnRun = "Lauf";

    /// <summary>Kopfteil, den jedes Gerät vorne an seine eigene Kopfzeile setzt.</summary>
    public const string BaseHeader = $"{ColumnDevice};{ColumnDate};{ColumnTime};{ColumnRun}";
}
