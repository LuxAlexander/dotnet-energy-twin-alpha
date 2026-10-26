namespace EnergySimulator.Simulation;

/// <summary>Laufzustand der PV-Anlage. AUTO/MANUELL wirken nur standalone; mit Master gilt die Zeit des Masters.</summary>
public enum SimulationMode
{
    Auto,
    Manual,
    Paused
}
