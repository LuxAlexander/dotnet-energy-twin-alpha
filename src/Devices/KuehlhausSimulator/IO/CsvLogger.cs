using System.Globalization;
using Shared.Data;

namespace KuehlhausSimulator.IO;

/// <summary>
/// Schreibt pro Simulationsschritt eine Zeile in die CSV-Datei (asynchron). Die Datei – Name mit Geräte-ID und
/// Zeitstempel, Semikolon, UTF-8 mit BOM – legt <see cref="CsvFileWriter"/> an; die Zahlen stehen fix im
/// de-AT-Format (Dezimalkomma) – unabhängig vom System.
/// </summary>
public sealed class CsvLogger : IAsyncDisposable
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

    private const string Header =
        "Zeit;Lauf;Modus;Anlage_Ein;Soll_C;Raum_C;Aussen_C;Aussenquelle;Kompressor;Kaelte_kW;Leistung_kW;" +
        "Energie_kWh;Laufzeit_h;Tuer;Zustand;Fehler";

    private readonly CsvFileWriter _writer;

    public string FilePath => _writer.FilePath;

    private CsvLogger(CsvFileWriter writer) => _writer = writer;

    /// <summary>Legt Ordner und Datei an und schreibt die Kopfzeile.</summary>
    public static async Task<CsvLogger> CreateAsync(string deviceId, string directory, CancellationToken cancellationToken)
    {
        var writer = await CsvFileWriter.CreateAsync(deviceId, directory, Header, cancellationToken);
        return new CsvLogger(writer);
    }

    /// <summary>Eine Zeile; geschrieben wird gepuffert, <see cref="FlushAsync"/> nach jedem Schleifendurchlauf.</summary>
    public Task WriteAsync(
        DateTime time, int runId, bool autoMode, Kuehlhaus room, string outsideSource, CancellationToken cancellationToken)
    {
        var fields = new List<string>
        {
            time.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            runId.ToString(Culture),
            autoMode ? "AUTO" : "MANUELL",
            room.IsOn ? "1" : "0",
            Number(room.SetpointC, 1),
            Number(room.RoomTempC, 2),
            Number(room.OutsideTempC, 2),
            outsideSource,
            room.CompressorOn ? "1" : "0",
            Number(room.CoolingPowerKW, 2),
            Number(room.ElectricPowerKW, 2),
            Number(room.EnergyKWh, 3),
            Number(room.RuntimeHours, 2),
            room.DoorOpen ? "1" : "0",
            room.State.ToString(),
            room.ActiveFault.ToString(),
        };

        return _writer.WriteRowAsync(fields, cancellationToken);
    }

    public Task FlushAsync(CancellationToken cancellationToken) => _writer.FlushAsync(cancellationToken);

    public ValueTask DisposeAsync() => _writer.DisposeAsync();

    private static string Number(double value, int decimals) => value.ToString("F" + decimals, Culture);
}
