using System.Globalization;
using System.Text;
using DualChargerSim.Models;

namespace DualChargerSim.IO;

/// <summary>
/// Schreibt pro Simulationsschritt eine Zeile in die CSV-Datei (asynchron).
/// Semikolon als Trenner, Zahlen fix im de-AT-Format (Dezimalkomma) – unabhängig vom System.
/// </summary>
public sealed class CsvLogger : IAsyncDisposable
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

    private const string Header =
        "Zeit;Lauf;Modus;Saeule_Ein;Gesamtlimit_kW;Gesamtleistung_kW;Energie_kWh;Abgewiesen;Warteschlange;" +
        "P1_Zustand;P1_Fahrzeug;P1_SOC;P1_Leistung_kW;P1_Temp_C;P1_Energie_kWh;" +
        "P2_Zustand;P2_Fahrzeug;P2_SOC;P2_Leistung_kW;P2_Temp_C;P2_Energie_kWh";

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
    public async Task WriteAsync(DateTime time, int runId, bool autoMode, DualCharger charger, CancellationToken cancellationToken)
    {
        var fields = new List<string>
        {
            time.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            runId.ToString(Culture),
            autoMode ? "AUTO" : "MANUELL",
            charger.IsOn ? "1" : "0",
            Number(charger.PowerLimitKW, 1),
            Number(charger.TotalPowerKW, 2),
            Number(charger.TotalEnergyKWh, 3),
            charger.RejectedVehicles.ToString(Culture),
            charger.QueueLength.ToString(Culture),
        };

        foreach (var point in charger.Points)
        {
            fields.Add(point.State.ToString());
            fields.Add(point.Vehicle?.ModelName ?? "");
            fields.Add(point.Vehicle is null ? "" : Number(point.Vehicle.SocPercent, 2));
            fields.Add(Number(point.PowerKW, 2));
            fields.Add(Number(point.TemperatureC, 1));
            fields.Add(Number(point.EnergyKWh, 3));
        }

        await _writer.WriteLineAsync(string.Join(';', fields).AsMemory(), cancellationToken);
    }

    public Task FlushAsync(CancellationToken cancellationToken) => _writer.FlushAsync(cancellationToken);

    public ValueTask DisposeAsync() => _writer.DisposeAsync();

    private static string Number(double value, int decimals) => value.ToString("F" + decimals, Culture);
}
