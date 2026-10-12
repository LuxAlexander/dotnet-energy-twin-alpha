using System.Globalization;
using System.Text;
using DualChargerSim.Models;

namespace DualChargerSim.IO;

/// <summary>
/// Schreibt pro Simulationsschritt eine Zeile in die CSV-Datei.
/// Semikolon als Trenner, Zahlen fix im de-AT-Format (Dezimalkomma) – unabhängig vom System.
/// </summary>
public sealed class CsvLogger : IDisposable
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

    private const string Header =
        "Zeit;Modus;Saeule_Ein;Gesamtlimit_kW;Gesamtleistung_kW;Energie_kWh;Abgewiesen;Warteschlange;" +
        "P1_Zustand;P1_Fahrzeug;P1_SOC;P1_Leistung_kW;P1_Temp_C;P1_Energie_kWh;" +
        "P2_Zustand;P2_Fahrzeug;P2_SOC;P2_Leistung_kW;P2_Temp_C;P2_Energie_kWh";

    private readonly StreamWriter _writer;

    public string FilePath { get; }

    public CsvLogger(string deviceId, string directory = "output")
    {
        Directory.CreateDirectory(directory);

        string safeId = string.Concat(deviceId.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        FilePath = Path.Combine(directory, $"{safeId}_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv");

        // UTF-8 mit BOM, damit Excel die Datei korrekt erkennt; AutoFlush, damit bei Absturz nichts verloren geht
        _writer = new StreamWriter(FilePath, append: false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true))
        {
            AutoFlush = true,
        };
        _writer.WriteLine(Header);
    }

    public void Write(DateTime time, bool autoMode, DualCharger charger)
    {
        var fields = new List<string>
        {
            time.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
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

        _writer.WriteLine(string.Join(';', fields));
    }

    public void Dispose() => _writer.Dispose();

    private static string Number(double value, int decimals) => value.ToString("F" + decimals, Culture);
}
