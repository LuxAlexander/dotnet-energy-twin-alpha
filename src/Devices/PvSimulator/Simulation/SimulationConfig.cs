using Devices.Common;
using Devices.Common.Modbus;
using Shared.Config;

namespace EnergySimulator.Simulation;

/// <summary>Startparameter der PV-Anlage (Kennung, Nennleistung, Schrittweiten, CSV, Zeitquelle, Modbus).</summary>
public sealed class SimulationConfig
{
    public const double DefaultStandaloneSpeed = 60;

    public string DeviceId { get; set; } = "PV-01";
    public int StepMinutes { get; set; } = 15;               // CSV-Intervall
    public double ModelStepMinutes { get; set; } = 1;        // feste Schrittweite des Modells
    public int RefreshMs { get; set; } = 400;
    public double PeakPowerKw { get; set; } = 10.0;
    public string OutputDirectory { get; set; } = OutputPaths.CsvDirectory;
    public int? Seed { get; set; }

    /// <summary>Basis-URL des Wettersimulators; <c>null</c> = lokales Wetter (--weather off).</summary>
    public string? WeatherUrl { get; set; }

    /// <summary>Standard-Adresse des Wettersimulators, wenn kein Standalone-Lauf angefordert ist.</summary>
    public const string DefaultWeatherUrl = "http://127.0.0.1:8080";

    /// <summary>Standortkennung für die Wetterabfrage (salzburg, wien, innsbruck).</summary>
    public string Location { get; set; } = "salzburg";

    /// <summary>Woher die virtuelle Zeit kommt (Master oder lokale Uhr).</summary>
    public TimeSyncOptions TimeSync { get; set; } = new() { StandaloneSpeed = DefaultStandaloneSpeed };

    /// <summary>Host, Port und Unit-ID des Modbus-Servers.</summary>
    public ModbusOptions Modbus { get; set; } = new();

    public TimeSpan ModelStep => TimeSpan.FromMinutes(ModelStepMinutes);
}
