using System.Globalization;
using System.Text;

namespace Verbraucher.IO;

/// <summary>
/// Schreibt pro Simulationsschritt eine Zeile in die CSV-Datei (asynchron).
/// Semikolon als Trenner, Zahlen fix im de-AT-Format (Dezimalkomma) – unabhängig vom System.
/// </summary>
public sealed class CsvLogger : IAsyncDisposable
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

    private const string Header =
        "Zeit;Lauf;Modus;Anlage_Ein;Soll_C;Raum_C;Aussen_C;Aussenquelle;Kompressor;Kaelte_kW;Leistung_kW;" +
        "Energie_kWh;Laufzeit_h;Tuer;Zustand;Fehler";

    private readonly StreamWriter _writer;

    public string FilePath { get; }

    private CsvLogger(string filePath, StreamWriter writer)
    {
        FilePath = filePath;
        _writer = writer;
    }

    /// <summary>Legt Ordner und Datei an und schreibt die Kopfzeile.</summary>
    public static async Task<CsvLogger> CreateAsync(string deviceId, string directory, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(directory);

        string safeId = string.Concat(deviceId.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        string filePath = Path.Combine(directory, $"{safeId}_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv");

        // UTF-8 mit BOM, damit Excel die Datei korrekt erkennt; FileOptions.Asynchronous für echtes async-I/O
        var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.Asynchronous);
        var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        var logger = new CsvLogger(filePath, writer);
        await writer.WriteLineAsync(Header.AsMemory(), cancellationToken);
        await writer.FlushAsync(cancellationToken);
        return logger;
    }

    /// <summary>Eine Zeile; geschrieben wird gepuffert, <see cref="FlushAsync"/> nach jedem Schleifendurchlauf.</summary>
    public async Task WriteAsync(
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

        await _writer.WriteLineAsync(string.Join(';', fields).AsMemory(), cancellationToken);
    }

    public Task FlushAsync(CancellationToken cancellationToken) => _writer.FlushAsync(cancellationToken);

    public ValueTask DisposeAsync() => _writer.DisposeAsync();

    private static string Number(double value, int decimals) => value.ToString("F" + decimals, Culture);
}
