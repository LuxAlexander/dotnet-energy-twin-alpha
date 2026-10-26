using Devices.Common;
using Devices.Common.Modbus;
using Devices.Common.Weather;

namespace Batterie_Sim;

/// <summary>Startparameter des Batteriespeichers: Gerätedaten, Schrittweite, Zeitquelle und Modbus.</summary>
public sealed class SimulatorOptions
{
    public const double DefaultStandaloneSpeed = 600;

    public BatterieConfig Config { get; init; } = new();

    /// <summary>Feste Schrittweite des Modells in Simulationssekunden.</summary>
    public double StepSeconds { get; init; } = 60;

    /// <summary>Nur standalone: Simulationsdauer in Stunden, <c>null</c> = endlos.</summary>
    public double? DurationHours { get; init; }

    public int? Seed { get; init; }

    /// <summary>Woher die virtuelle Zeit kommt (Master oder lokale Uhr).</summary>
    public TimeSyncOptions TimeSync { get; init; } = new() { StandaloneSpeed = DefaultStandaloneSpeed };

    /// <summary>Host, Port und Unit-ID des Modbus-Servers.</summary>
    public ModbusOptions Modbus { get; init; } = new();

    /// <summary>Wetterbezug (optional; ohne Angabe mit Master der Wettersimulator, standalone aus).</summary>
    public WeatherOptions Weather { get; init; } = new();

    public TimeSpan Step => TimeSpan.FromSeconds(StepSeconds);
}
