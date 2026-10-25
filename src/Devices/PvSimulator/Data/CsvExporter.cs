using System.Globalization;
using System.Text;
using EnergySimulator.Devices;
using EnergySimulator.Simulation;

namespace EnergySimulator.Data;

/// <summary>
/// Schreibt die Messwerte der PV-Anlage als CSV (asynchron, angehängt statt die ganze Datei neu zu schreiben).
/// Der Dateiname enthält Geräte-ID und Startzeit, damit mehrere Anlagen und mehrere Läufe sich nicht überschreiben.
/// </summary>
public sealed class CsvExporter : IAsyncDisposable
{
    // InvariantCulture: Dezimaltrenner ist immer ".", unabhängig von der Regionseinstellung des Rechners.
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    private const string Header =
        "Datum;Zeit;Jahreszeit;Modus;Wetter;Einstrahlung_W_m2;Leistung_kW;Tagesenergie_kWh;Gesamtenergie_kWh;" +
        "Modultemperatur_C;Aussentemperatur_C;Netzspannung_V;Verschmutzung_pct;Alterung_pct;Fehler;Anlagenstatus;" +
        "Lauf;Leistungsbegrenzung_pct";

    private readonly StreamWriter _writer;

    public string FilePath { get; }

    private CsvExporter(string filePath, StreamWriter writer)
    {
        FilePath = filePath;
        _writer = writer;
    }

    /// <summary>Legt Ordner und Datei an und schreibt die Kopfzeile.</summary>
    public static async Task<CsvExporter> CreateAsync(string deviceId, string directory, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(directory);

        string safeId = string.Concat(deviceId.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        string filePath = Path.Combine(directory, $"{safeId}_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv");

        var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.Asynchronous);
        var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        await writer.WriteLineAsync(Header.AsMemory(), cancellationToken);
        await writer.FlushAsync(cancellationToken);
        return new CsvExporter(filePath, writer);
    }

    public async Task WriteRowAsync(DateTime now, SimulationMode mode, int runId, PvSystem pv, CancellationToken cancellationToken)
    {
        string modeStr = mode switch
        {
            SimulationMode.Auto => "AUTO",
            SimulationMode.Manual => "MANUELL",
            SimulationMode.Paused => "PAUSE",
            _ => "?"
        };

        string status = !pv.IsEnabled ? "AUS" : pv.ActiveFault != FaultType.None ? "STOERUNG" : "AKTIV";

        string line = string.Join(';',
            now.ToString("yyyy-MM-dd", Invariant),
            now.ToString("HH:mm", Invariant),
            SeasonInfo.FromMonth(now.Month).ToGerman(),
            modeStr,
            pv.Weather.ToGerman(),
            pv.IrradiationWm2.ToString("F1", Invariant),
            pv.PowerKw.ToString("F2", Invariant),
            pv.DailyEnergyKwh.ToString("F3", Invariant),
            pv.TotalEnergyKwh.ToString("F2", Invariant),
            pv.ModuleTemperatureC.ToString("F1", Invariant),
            pv.AmbientTemperatureC.ToString("F1", Invariant),
            pv.GridVoltageV.ToString("F1", Invariant),
            pv.SoilingLossPercent.ToString("F2", Invariant),
            pv.AgingLossPercent.ToString("F2", Invariant),
            pv.ActiveFault.ToGString(),
            status,
            runId.ToString(Invariant),
            pv.PowerLimitPercent.ToString("F1", Invariant));

        await _writer.WriteLineAsync(line.AsMemory(), cancellationToken);
    }

    public Task FlushAsync(CancellationToken cancellationToken) => _writer.FlushAsync(cancellationToken);

    public ValueTask DisposeAsync() => _writer.DisposeAsync();
}
