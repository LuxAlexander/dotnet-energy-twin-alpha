using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SimCore.Contracts;

namespace EnergySimulator.Simulation;

/// <summary>Fehler beim Abruf des Wetters (nicht erreichbar, ungültige Antwort, unbekannter Standort).</summary>
public sealed class WeatherUnavailableException : Exception
{
    public WeatherUnavailableException(string message, Exception? inner = null) : base(message, inner)
    {
    }
}

/// <summary>
/// Asynchroner HTTP-Client für den Wettersimulator. Fragt je Modellstunde (
/// <c>GET /weather?location=…&amp;simTime=…&amp;run=…</c>) und übersetzt alle Fehler in
/// <see cref="WeatherUnavailableException"/> mit einer deutschen Meldung.
/// </summary>
public sealed class WeatherClient : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _http;

    public WeatherClient(string baseUrl)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(5)
        };
    }

    public string BaseUrl => _http.BaseAddress!.ToString();

    /// <summary>Holt das Wetter für (Standort, virtuelle Zeit, Lauf). Zeiten werden als UTC übertragen.</summary>
    /// <exception cref="WeatherUnavailableException">Kein Netz, Timeout, 404 oder unparsebare Antwort.</exception>
    public async Task<WeatherData> GetAsync(string location, DateTime simTimeUtc, int runId, CancellationToken cancellationToken)
    {
        string time = DateTime.SpecifyKind(simTimeUtc, DateTimeKind.Utc)
            .ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        string url = string.Create(CultureInfo.InvariantCulture,
            $"weather?location={Uri.EscapeDataString(location)}&simTime={time}&run={runId}");

        try
        {
            using var response = await _http.GetAsync(url, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new WeatherUnavailableException("UNGÜLTIG (unbekannter Standort)");

            if (!response.IsSuccessStatusCode)
                throw new WeatherUnavailableException($"UNGÜLTIG (HTTP {(int)response.StatusCode})");

            var data = await response.Content.ReadFromJsonAsync<WeatherData>(JsonOptions, cancellationToken);
            if (data is null)
                throw new WeatherUnavailableException("UNGÜLTIG (leere Antwort)");

            // JSON-Zeitstempel kommen mit "Z" an; System.Text.Json liefert sie als Lokalzeit – hier auf UTC normalisieren.
            return data with { SimTimeUtc = data.SimTimeUtc.ToUniversalTime() };
        }
        catch (WeatherUnavailableException)
        {
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new WeatherUnavailableException("NICHT ERREICHBAR (Zeitüberschreitung)", ex);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new WeatherUnavailableException("NICHT ERREICHBAR", ex);
        }
        catch (JsonException ex)
        {
            throw new WeatherUnavailableException("UNGÜLTIG (Antwort nicht lesbar)", ex);
        }
    }

    public void Dispose() => _http.Dispose();
}
