using SimCore.Contracts;

namespace Devices.Common.Weather;

/// <summary>
/// Hält das Wetter je Modellstunde vor: fragt höchstens einmal pro Stunde, wiederholt nach einem Fehler erst nach
/// 10 s, behält das letzte gültige Wetter und meldet einen anhaltenden Ausfall nach 3 Fehlversuchen sichtbar.
/// </summary>
public sealed class WeatherFeed : IDisposable
{
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(10);
    private const int ReportAfterFailures = 3;

    private readonly IWeatherService _service;
    private readonly Action<string>? _report;
    private WeatherData? _last;
    private int _failures;
    private DateTime _nextAttemptUtc = DateTime.MinValue;
    private bool _errorReported;

    /// <param name="service">Wetterquelle (HTTP-Client oder Test-Double).</param>
    /// <param name="location">Standortkennung für die Abfrage.</param>
    /// <param name="report">Meldet einen anhaltenden Ausfall (ab 3 Fehlversuchen einmalig).</param>
    public WeatherFeed(IWeatherService service, string location, Action<string>? report = null)
    {
        _service = service;
        Location = location;
        _report = report;
        Status = $"warte auf {location} …";
    }

    public string Location { get; }

    /// <summary>Letztes gültiges Wetter; <c>null</c>, solange noch keines angekommen ist.</summary>
    public WeatherData? Last => _last;

    /// <summary>Kurzstatus für Anzeige und Meldungen.</summary>
    public string Status { get; private set; }

    /// <summary>
    /// Liefert das Wetter für die Modellstunde von <paramref name="simTime"/>. Bei einem Fehler kommt das letzte
    /// gültige Wetter zurück; <c>null</c>, wenn noch keines vorliegt.
    /// </summary>
    public async Task<WeatherData?> UpdateAsync(DateTime simTime, int runId, CancellationToken cancellationToken)
    {
        var hour = new DateTime(simTime.Year, simTime.Month, simTime.Day, simTime.Hour, 0, 0, DateTimeKind.Utc);
        bool needsFetch = _last is null || _last.SimTimeUtc != hour || _last.RunId != runId;

        if (needsFetch && DateTime.UtcNow >= _nextAttemptUtc)
        {
            try
            {
                var data = await _service.GetAsync(Location, hour, runId, cancellationToken);
                _last = data;
                _failures = 0;
                _errorReported = false;
                Status = FormattableString.Invariant(
                    $"{data.Location}: {Describe(data.Condition)} · {data.TemperatureC:0.0} °C · {data.WindSpeedMs:0.0} m/s · {data.IrradiationWm2:0} W/m²");
            }
            catch (WeatherUnavailableException ex) when (!cancellationToken.IsCancellationRequested)
            {
                _failures++;
                _nextAttemptUtc = DateTime.UtcNow + RetryDelay;
                Status = FormattableString.Invariant(
                    $"{ex.Message} – {_failures}. Fehlversuch, letztes Wetter bleibt");
                if (_failures >= ReportAfterFailures && !_errorReported)
                {
                    _errorReported = true;
                    _report?.Invoke($"WETTER: {ex.Message} – {Location} (Versuche alle 10 s)");
                }
            }
        }

        return _last;
    }

    /// <summary>Deutscher Kurzname des Wetterzustands.</summary>
    public static string Describe(WeatherCondition condition) => condition switch
    {
        WeatherCondition.Sunny => "SONNIG",
        WeatherCondition.PartlyCloudy => "BEWÖLKT",
        WeatherCondition.Overcast => "DICHT BEWÖLKT",
        WeatherCondition.Storm => "GEWITTER",
        WeatherCondition.Snow => "SCHNEE",
        WeatherCondition.Night => "NACHT",
        _ => condition.ToString().ToUpperInvariant()
    };

    public void Dispose() => (_service as IDisposable)?.Dispose();
}
