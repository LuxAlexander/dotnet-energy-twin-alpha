namespace WeatherSimulator.Simulation;

/// <summary>
/// Manuelle Wetterübersteuerung (Toggle, ADR-0007): gilt für alle Standorte, bis sie wieder
/// aufgehoben wird. Alle nicht gesetzten Größen bleiben aus dem deterministischen Modell.
/// </summary>
public sealed class ManualWeather
{
    public bool IsActive { get; private set; }
    public double? CloudCoverPercent { get; private set; }
    public double? TemperatureC { get; private set; }
    public double? PrecipitationMmPerHour { get; private set; }
    public double? WindSpeedMs { get; private set; }

    /// <summary>Setzt einen der Voreinstellungs-Zustände (Sonne, bewölkt, Regen, Schnee, Gewitter).</summary>
    public void ApplyPreset(SimCore.Contracts.WeatherCondition condition)
    {
        IsActive = true;
        switch (condition)
        {
            case SimCore.Contracts.WeatherCondition.Sunny:
                CloudCoverPercent = 5;
                PrecipitationMmPerHour = 0;
                break;
            case SimCore.Contracts.WeatherCondition.PartlyCloudy:
                CloudCoverPercent = 45;
                PrecipitationMmPerHour = 0;
                break;
            case SimCore.Contracts.WeatherCondition.Overcast:
                CloudCoverPercent = 95;
                PrecipitationMmPerHour = 0;
                break;
            case SimCore.Contracts.WeatherCondition.Storm:
                CloudCoverPercent = 100;
                PrecipitationMmPerHour = 6.0;
                break;
            case SimCore.Contracts.WeatherCondition.Snow:
                CloudCoverPercent = 100;
                TemperatureC = -2.0;
                PrecipitationMmPerHour = 1.5;
                break;
            case SimCore.Contracts.WeatherCondition.Night:
                CloudCoverPercent = 30;
                PrecipitationMmPerHour = 0;
                break;
        }
    }

    /// <summary>Regen: dicht bewölkt, mäßiger Niederschlag, wenig Wind.</summary>
    public void SetRain()
    {
        IsActive = true;
        CloudCoverPercent = 95;
        TemperatureC = null;
        PrecipitationMmPerHour = 3.5;
        WindSpeedMs = 4.0;
    }

    public void SetTemperature(double value)
    {
        IsActive = true;
        TemperatureC = value;
    }

    public void SetWind(double value)
    {
        IsActive = true;
        WindSpeedMs = Math.Max(0, value);
    }

    public void Clear()
    {
        IsActive = false;
        CloudCoverPercent = null;
        TemperatureC = null;
        PrecipitationMmPerHour = null;
        WindSpeedMs = null;
    }
}
