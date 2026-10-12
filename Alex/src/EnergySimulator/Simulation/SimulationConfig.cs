namespace EnergySimulator.Simulation;

public sealed class SimulationConfig
{
    public int Timescale { get; set; } = 60;
    public int StepMinutes { get; set; } = 15;
    public int RefreshMs { get; set; } = 400;
    public double PeakPowerKw { get; set; } = 10.0;
    public string OutputDirectory { get; set; } = "data";
    public string CsvFileName { get; set; } = "pv-simulation.csv";
    public DateTime? StartDate { get; set; }
}