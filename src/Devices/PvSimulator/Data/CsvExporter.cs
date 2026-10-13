using System.Text;
using EnergySimulator.Devices;
using EnergySimulator.Simulation;

namespace EnergySimulator.Data;

public sealed class CsvExporter
{
    private const int FlushEveryRows = 5;

    // alle 5 Zeilen Flush: kurze Testlaeufe (z. B. 20 s)
    // laengere Laeufe verursachen nicht pro Zeile
    // einen Dateizugriff.

    private readonly StringBuilder _builder = new();
    private readonly string _filePath;
    private bool _headerWritten;
    private int _rowsSinceFlush;

    public CsvExporter(string directory, string fileName)
    {
        Directory.CreateDirectory(directory);
        _filePath = Path.Combine(directory, fileName);
    }

    public void WriteHeader()
    {
        _builder.Clear();
        _builder.AppendLine(string.Join(';',
            "Datum", "Zeit", "Jahreszeit", "Modus", "Wetter",
            "Einstrahlung_W_m2", "Leistung_kW", "Tagesenergie_kWh", "Gesamtenergie_kWh",
            "Modultemperatur_C", "Aussentemperatur_C", "Netzspannung_V",
            "Verschmutzung_pct", "Alterung_pct", "Fehler", "Anlagenstatus"));
        _headerWritten = true;
    }

    public void WriteRow(DateTime now, SimulationMode mode, PvSystem pv)
    {
        if (!_headerWritten)
            WriteHeader();

        string modeStr = mode switch
        {
            SimulationMode.Auto => "AUTO",
            SimulationMode.Manual => "MANUELL",
            SimulationMode.Paused => "PAUSE",
            _ => "?"
        };

        string status = !pv.IsEnabled ? "AUS" : pv.ActiveFault != FaultType.None ? "STOERUNG" : "AKTIV";

        // InvariantCulture: Dezimaltrenner ist immer ".", unabhaengig von der
        // Sprach- und Regionseinstellung des Rechners. Sonst wuerde bei einem
        // System mit "Komma als Dezimaltrenner" (z. B. de-DE) eine unlesbare
        // CSV entstehen (12,5 statt 12.5) oder der Import in Tools scheitern.
        // built-in static property for consistent formatting and parsing
        _builder.AppendLine(string.Join(';',
            now.ToString("yyyy-MM-dd"),
            now.ToString("HH:mm"),
            SeasonInfo.FromMonth(now.Month).ToGerman(),
            modeStr,
            pv.Weather.ToGerman(),
            pv.IrradiationWm2.ToString("F1", System.Globalization.CultureInfo.InvariantCulture),
            pv.PowerKw.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            pv.DailyEnergyKwh.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
            pv.TotalEnergyKwh.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            pv.ModuleTemperatureC.ToString("F1", System.Globalization.CultureInfo.InvariantCulture),
            pv.AmbientTemperatureC.ToString("F1", System.Globalization.CultureInfo.InvariantCulture),
            pv.GridVoltageV.ToString("F1", System.Globalization.CultureInfo.InvariantCulture),
            pv.SoilingLossPercent.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            pv.AgingLossPercent.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            pv.ActiveFault.ToGString(),
            status));

        if (++_rowsSinceFlush >= FlushEveryRows)
            Flush();
    }

    public void Flush() => File.WriteAllText(_filePath, _builder.ToString());
}