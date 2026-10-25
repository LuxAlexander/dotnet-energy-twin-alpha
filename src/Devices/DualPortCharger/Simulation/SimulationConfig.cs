using Devices.Common;
using Devices.Common.Modbus;
using Shared.Config;

namespace DualChargerSim.Simulation;

/// <summary>
/// Startparameter der Simulation (Geräte-ID, Leistung, Schrittweite, Zeitquelle, Dauer, Seed).
/// </summary>
public class SimulationConfig
{
    public const double MinimumRecommendedHours = 24;
    public const double DefaultStandaloneSpeed = 120;

    public string DeviceId { get; init; } = "DC-01";
    public double PowerLimitKW { get; init; } = 150;
    public double StepMinutes { get; init; } = 5;

    /// <summary>Nur standalone: Simulationsdauer in Stunden, <c>null</c> = endlos.</summary>
    public double? DurationHours { get; init; }
    public int? Seed { get; init; }

    /// <summary>Zielordner der CSV; relativ zum Arbeitsverzeichnis oder absolut.</summary>
    public string OutputDirectory { get; init; } = OutputPaths.CsvDirectory;

    /// <summary>Woher die virtuelle Zeit kommt (Master oder lokale Uhr).</summary>
    public TimeSyncOptions TimeSync { get; init; } = new() { StandaloneSpeed = DefaultStandaloneSpeed };

    /// <summary>Host, Port und Unit-ID des Modbus-Servers.</summary>
    public ModbusOptions Modbus { get; init; } = new();

    public TimeSpan Step => TimeSpan.FromMinutes(StepMinutes);
    public TimeSpan? Duration => DurationHours is { } hours ? TimeSpan.FromHours(hours) : null;

    public bool IsShortRun => DurationHours < MinimumRecommendedHours;
}
