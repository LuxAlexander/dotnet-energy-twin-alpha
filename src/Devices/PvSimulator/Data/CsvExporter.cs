using EnergySimulator.Devices;
using EnergySimulator.Simulation;
using Shared.Data;
using Shared.Weather;

namespace EnergySimulator.Data;

/// <summary>
/// Schreibt die Messwerte der PV-Anlage als CSV (asynchron, angehängt statt die ganze Datei neu zu schreiben).
/// Der Dateiname enthält Geräte-ID und Startzeit, damit mehrere Anlagen und mehrere Läufe sich nicht überschreiben.
/// </summary>
public sealed class CsvExporter : IAsyncDisposable
{
    // Zahlen über Shared.Data.CsvFormat (de-AT, Dezimalkomma); Datum und Uhrzeit bleiben InvariantCulture/ISO.
    private const string Header =
        CsvSchema.BaseHeader + ";Jahreszeit;Modus;Wetter;Einstrahlung_W_m2;Leistung_kW;Tagesenergie_kWh;" +
        "Gesamtenergie_kWh;Modultemperatur_C;Aussentemperatur_C;Netzspannung_V;Verschmutzung_pct;Alterung_pct;" +
        "Fehler;Anlagenstatus;Leistungsbegrenzung_pct";

    private readonly CsvFileWriter _writer;

    public string FilePath => _writer.FilePath;

    private CsvExporter(CsvFileWriter writer) => _writer = writer;

    /// <summary>Legt Ordner und Datei an und schreibt die Kopfzeile.</summary>
    public static async Task<CsvExporter> CreateAsync(string deviceId, string directory, CancellationToken cancellationToken)
    {
        var writer = await CsvFileWriter.CreateAsync(deviceId, directory, Header, cancellationToken);
        return new CsvExporter(writer);
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

        string[] values =
        [
            pv.Name,
            now.ToString("yyyy-MM-dd", CsvFormat.Invariant),
            now.ToString("HH:mm", CsvFormat.Invariant),
            runId.ToString(CsvFormat.Invariant),
            SeasonInfo.FromMonth(now.Month).ToGerman(),
            modeStr,
            pv.Weather.ToGerman(),
            pv.IrradiationWm2.Num("F1"),
            pv.PowerKw.Num("F2"),
            pv.DailyEnergyKwh.Num("F3"),
            pv.TotalEnergyKwh.Num("F3"),
            pv.ModuleTemperatureC.Num("F1"),
            pv.AmbientTemperatureC.Num("F1"),
            pv.GridVoltageV.Num("F1"),
            pv.SoilingLossPercent.Num("F2"),
            pv.AgingLossPercent.Num("F2"),
            pv.ActiveFault.ToGString(),
            status,
            pv.PowerLimitPercent.Num("F1")
        ];

        await _writer.WriteRowAsync(values, cancellationToken);
    }

    public Task FlushAsync(CancellationToken cancellationToken) => _writer.FlushAsync(cancellationToken);

    public ValueTask DisposeAsync() => _writer.DisposeAsync();
}
