using EnergySimulator.Simulation;
using ContractWeather = SimCore.Contracts.WeatherCondition;
using WeatherData = SimCore.Contracts.WeatherData;

namespace EnergySimulator.Devices;

public sealed class PvSystem : IDevice
{
    private readonly double _peakPowerKw;
    private readonly PvFaultModel _faults;

    public string Name => "PV-Anlage";
    public double PeakPowerKw => _peakPowerKw;

    /// <summary>Wirkleistungsbegrenzung in % der Nennleistung (Einspeisemanagement, per Modbus einstellbar).</summary>
    public double PowerLimitPercent { get; set; } = 100.0;
    public bool IsEnabled { get; set; } = true;
    public WeatherCondition Weather { get; private set; } = WeatherCondition.Sunny;

    public double IrradiationWm2 { get; private set; }
    public double PowerKw { get; private set; }
    public double DailyEnergyKwh { get; private set; }
    public double TotalEnergyKwh { get; private set; }

    public FaultType ActiveFault => _faults.ActiveFault;
    public bool HasFault => _faults.HasFault;
    public double ModuleTemperatureC => _faults.ModuleTemperatureC;
    public double GridVoltageV => _faults.GridVoltageV;
    public double SoilingLossPercent => _faults.SoilingLossPercent;
    public double AgingLossPercent => _faults.AgingLossPercent;
    public double AmbientTemperatureC => _faults.AmbientTemperatureC;
    public double CapacityPercent => _peakPowerKw > 0 ? Math.Round(PowerKw / _peakPowerKw * 100.0, 1) : 0.0;
    public double EfficiencyPercent => Math.Round(Math.Max(0.10, 1.0 - _faults.TotalLossPercent / 100.0) * 100.0, 1);
    public bool IsNight => Weather == WeatherCondition.Night;

    public PvSystem(double peakPowerKw, PvFaultModel faults)
    {
        _peakPowerKw = peakPowerKw;
        _faults = faults;
    }

    public void SetWeather(WeatherCondition weather) => Weather = weather;

    /// <summary>Aktuelles Wetter vom Wettersimulator; <c>null</c> = lokales Modell (Standalone/--weather off).</summary>
    public WeatherData? ExternalWeather { get; private set; }

    /// <summary>Übernimmt die Wetterwerte des Wettersimulators (Einstrahlung, Zustand, Außentemperatur).</summary>
    public void ApplyExternalWeather(WeatherData weather)
    {
        ExternalWeather = weather;
        Weather = weather.Condition switch
        {
            ContractWeather.Sunny => WeatherCondition.Sunny,
            ContractWeather.PartlyCloudy => WeatherCondition.PartlyCloudy,
            ContractWeather.Overcast => WeatherCondition.Overcast,
            ContractWeather.Storm => WeatherCondition.Storm,
            ContractWeather.Snow => WeatherCondition.Snow,
            ContractWeather.Night => WeatherCondition.Night,
            _ => WeatherCondition.Sunny
        };
    }

    public void StepContinuous(DateTime now, double elapsedSimHours)
    {
        double hour = now.Hour + now.Minute / 60.0 + now.Second / 3600.0;
        Season season = SeasonInfo.FromMonth(now.Month);

        if (ExternalWeather is { } weather)
        {
            // Wetter vom Wettersimulator: Strahlung, Zustand und Außentemperatur kommen von dort
            IrradiationWm2 = weather.IrradiationWm2;
            _faults.Step(IrradiationWm2, now, season, hour, elapsedSimHours, weather.TemperatureC);
        }
        else
        {
            IrradiationWm2 = IrradiationCalculator.Calculate(hour, Weather, season);
            _faults.Step(IrradiationWm2, now, season, hour, elapsedSimHours, null);
        }

        if (!IsEnabled || _faults.HasFault || IrradiationWm2 <= 0)
        {
            PowerKw = 0.0;
            return;
        }

        // Ertrag steigt linear mit der Einstrahlung bis zur STC-Referenz (1000 W/m²),
        // darüber wird sie gedeckelt (1.0) statt weiter zuzunehmen.
        double yieldFactor = Math.Min(1.0, IrradiationWm2 / IrradiationCalculator.PeakIrradiationWm2);
        // Untergrenze 10 %: Verschmutzung und Alterung koennen die Anlage nie
        // ganz abschalten, nur stark begrenzen – realitaetsnah fuer eine PV-Anlage.
        double degradation = Math.Max(0.10, 1.0 - _faults.TotalLossPercent / 100.0);

        PowerKw = Math.Round(Math.Min(_peakPowerKw * yieldFactor * degradation, _peakPowerKw * PowerLimitPercent / 100.0), 2);

        double produced = PowerKw * elapsedSimHours;
        DailyEnergyKwh = Math.Round(DailyEnergyKwh + produced, 4);
        TotalEnergyKwh = Math.Round(TotalEnergyKwh + produced, 4);
    }

    public void ResetDailyEnergy() => DailyEnergyKwh = 0.0;
}