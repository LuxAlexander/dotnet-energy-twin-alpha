namespace SimCore.Contracts;

/// <summary>
/// Vertrag für den Wetterbezug: liefert das Wetter für (Standort, virtuelle Zeit, Lauf).
/// Implementiert vom HTTP-Client in <c>Devices.Common</c>; Geräte hängen nur von diesem Interface ab.
/// </summary>
public interface IWeatherService
{
    /// <summary>Holt das Wetter für eine Modellstunde. Zeiten werden als UTC übertragen.</summary>
    Task<WeatherData> GetAsync(string location, DateTime simTimeUtc, int runId, CancellationToken cancellationToken);
}
