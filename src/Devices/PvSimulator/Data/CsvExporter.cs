using System.Globalization;
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
    // InvariantCulture: Dezimaltrenner ist immer ".", unabhängig von der Regionseinstellung des Rechners.
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    private const string Header =
        "Datum;Zeit;Jahreszeit;Modus;Wetter;Einstrahlung_W_m2;Leistung_kW;Tagesenergie_kWh;Gesamtenergie_kWh;" +
        "Modultemperatur_C;Aussentemperatur_C;Netzspannung_V;Verschmutzung_pct;Alterung_pct;Fehler;Anlagenstatus;" +
        "Lauf;Leistungsbegrenzung_pct";

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
            pv.PowerLimitPercent.ToString("F1", Invariant)
        ];

        await _writer.WriteRowAsync(values, cancellationToken);
    }

    public Task FlushAsync(CancellationToken cancellationToken) => _writer.FlushAsync(cancellationToken);

    public ValueTask DisposeAsync() => _writer.DisposeAsync();
}
