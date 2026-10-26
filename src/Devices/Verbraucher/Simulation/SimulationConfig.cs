using Devices.Common;
using Devices.Common.Modbus;
using Shared.Config;

namespace Verbraucher.Simulation;

/// <summary>
/// Startparameter der Simulation (Geräte-ID, Solltemperatur, Kälteleistung, Schrittweite, Dauer, Seed,
/// Zeitquelle, Modbus und Wetter).
/// </summary>
public class SimulationConfig
{
    public const double MinimumRecommendedHours = 24;

    /// <summary>Tempo im Standalone-Betrieb; ein Kühlhaus reagiert langsam, deshalb kein allzu hoher Wert.</summary>
    public const double DefaultStandaloneSpeed = 600;

    /// <summary>Wettersimulator; mit Master vorbelegt, standalone bleibt das Wetter lokal.</summary>
    public const string DefaultWeatherUrl = "http://127.0.0.1:8080";

    /// <summary>Eigener Modbus-Standardport; 1502 nutzen PV und Charger, 1503 die Batterie.</summary>
    public const int DefaultModbusPort = 1504;

    public string DeviceId { get; init; } = "KH-01";

    /// <summary>Solltemperatur des Kühlraums in °C (gültig −30 … 0).</summary>
    public double SetpointC { get; init; } = -20;

    /// <summary>Thermische Kälteleistung des Kompressors in kW.</summary>
    public double CoolingPowerKW { get; init; } = 6;

    public double StepMinutes { get; init; } = 5;

    /// <summary>Nur standalone: Simulationsdauer in Stunden, <c>null</c> = endlos.</summary>
    public double? DurationHours { get; init; }

    public int? Seed { get; init; }

    /// <summary>Zielordner der CSV; relativ zum Arbeitsverzeichnis oder absolut.</summary>
    public string OutputDirectory { get; init; } = OutputPaths.CsvDirectory;

    /// <summary>Woher die virtuelle Zeit kommt (Master oder lokale Uhr).</summary>
    public TimeSyncOptions TimeSync { get; init; } = new() { StandaloneSpeed = DefaultStandaloneSpeed };

    /// <summary>Host, Port und Unit-ID des Modbus-Servers.</summary>
    public ModbusOptions Modbus { get; init; } = new() { Port = DefaultModbusPort };

    /// <summary>Basis-URL des Wettersimulators; <c>null</c> = lokales Tagesprofil (<c>--weather off</c>).</summary>
    public string? WeatherUrl { get; init; }

    /// <summary>Standortkennung für die Wetterabfrage (siehe Wettersimulator).</summary>
    public string Location { get; init; } = "salzburg";

    public TimeSpan Step => TimeSpan.FromMinutes(StepMinutes);
    public TimeSpan? Duration => DurationHours is { } hours ? TimeSpan.FromHours(hours) : null;

    public bool IsShortRun => DurationHours < MinimumRecommendedHours;

    /// <summary>Modellparameter aus den Startoptionen.</summary>
    public KuehlhausConfig ToModelConfig() => new()
    {
        DeviceId = DeviceId,
        SollTempC = SetpointC,
        KaelteleistungKW = CoolingPowerKW,
    };
}
