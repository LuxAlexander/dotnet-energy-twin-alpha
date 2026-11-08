using SimCore.Contracts;

namespace WeatherSimulator.Simulation;

/// <summary>Unveränderliche Momentaufnahme der manuellen Vorgaben eines Standorts.</summary>
public sealed record ManualWeatherState(long Revision = 0, bool IsActive = false,
    double? CloudCoverPercent = null, double? TemperatureC = null,
    double? PrecipitationMmPerHour = null, double? WindSpeedMs = null);

/// <summary>Threadsichere, standortbezogene Übersteuerungen; nicht gesetzte Größen bleiben automatisch.</summary>
public sealed class ManualWeather
{
    private readonly object _gate = new();
    private readonly Dictionary<string, ManualWeatherState> _states = new(StringComparer.OrdinalIgnoreCase);
    private string _selectedLocation = LocationCatalog.DefaultKey;
    private long _revision;

    public string SelectedLocation { get { lock (_gate) return _selectedLocation; } }
    public bool IsActive => Get(SelectedLocation).IsActive;
    public double? CloudCoverPercent => Get(SelectedLocation).CloudCoverPercent;
    public double? TemperatureC => Get(SelectedLocation).TemperatureC;
    public double? PrecipitationMmPerHour => Get(SelectedLocation).PrecipitationMmPerHour;
    public double? WindSpeedMs => Get(SelectedLocation).WindSpeedMs;

    public ManualWeatherState Get(string location)
    {
        lock (_gate) return _states.GetValueOrDefault(location) ?? new ManualWeatherState();
    }

    public void Select(string location)
    {
        if (!LocationCatalog.TryGet(location, out var found))
            throw new ArgumentException($"Unbekannter Standort: '{location}'.");
        lock (_gate) _selectedLocation = found.Key;
    }

    private void Change(Func<ManualWeatherState, ManualWeatherState> update)
    {
        lock (_gate)
        {
            var before = _states.GetValueOrDefault(_selectedLocation) ?? new ManualWeatherState();
            _states[_selectedLocation] = update(before) with { Revision = ++_revision };
        }
    }

    public void ApplyPreset(WeatherCondition condition) => Change(before => condition switch
    {
        WeatherCondition.Sunny => before with { IsActive = true, CloudCoverPercent = 5, PrecipitationMmPerHour = 0 },
        WeatherCondition.PartlyCloudy => before with { IsActive = true, CloudCoverPercent = 45, PrecipitationMmPerHour = 0 },
        WeatherCondition.Overcast => before with { IsActive = true, CloudCoverPercent = 95, PrecipitationMmPerHour = 0 },
        WeatherCondition.Storm => before with { IsActive = true, CloudCoverPercent = 100, PrecipitationMmPerHour = 6, WindSpeedMs = 16 },
        WeatherCondition.Snow => before with { IsActive = true, CloudCoverPercent = 100, TemperatureC = -2, PrecipitationMmPerHour = 1.5 },
        _ => before with { IsActive = true, CloudCoverPercent = 30, PrecipitationMmPerHour = 0 }
    });

    public void SetRain() => Change(before => before with
    { IsActive = true, CloudCoverPercent = 95, TemperatureC = null, PrecipitationMmPerHour = 3.5, WindSpeedMs = 4 });

    public void SetTemperature(double value)
    {
        if (!double.IsFinite(value) || value is < -100 or > 70)
            throw new ArgumentOutOfRangeException(nameof(value), "Temperatur muss zwischen -100 und 70 °C liegen.");
        Change(before => before with { IsActive = true, TemperatureC = value });
    }

    public void SetWind(double value)
    {
        if (!double.IsFinite(value) || value is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(value), "Wind muss zwischen 0 und 100 m/s liegen.");
        Change(before => before with { IsActive = true, WindSpeedMs = value });
    }

    public void Clear() => Change(_ => new ManualWeatherState());

    public void ClearAll()
    {
        lock (_gate)
            foreach (var location in LocationCatalog.All)
                _states[location.Key] = new ManualWeatherState(Revision: ++_revision);
    }
}
