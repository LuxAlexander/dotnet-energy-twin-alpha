using Shared.Config;

namespace DualChargerSim.Simulation;

/// <summary>
/// Startparameter der Simulation (Geräte-ID, Leistung, Schrittweite, Tempo, Dauer, Seed).
/// </summary>
public class SimulationConfig
{
    public const double MinimumRecommendedHours = 24;
    public const double MinSpeed = 1;
    public const double MaxSpeed = 100_000;

    public string DeviceId { get; init; } = "DC-01";
    public double PowerLimitKW { get; init; } = 150;
    public double StepMinutes { get; init; } = 5;
    public double Speed { get; init; } = 120;
    public double DurationHours { get; init; } = 24;
    public int? Seed { get; init; }

    /// <summary>Zielordner der CSV; relativ zum Arbeitsverzeichnis oder absolut.</summary>
    public string OutputDirectory { get; init; } = OutputPaths.CsvDirectory;
    public DateTime StartTime { get; init; } = DateTime.Today;

    public TimeSpan Step => TimeSpan.FromMinutes(StepMinutes);
    public TimeSpan Duration => TimeSpan.FromHours(DurationHours);

    public bool IsShortRun => DurationHours < MinimumRecommendedHours;
}
