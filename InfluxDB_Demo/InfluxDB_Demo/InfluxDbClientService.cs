using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Net.Http.Headers;

// InfluxDB-Client-Dienst
public class InfluxDbClientService : IInfluxDbClientService
{
    private static readonly HttpClient HttpClient = new();
    // Konfigurationsoptionen
    private readonly InfluxDbOptions _options;
    // InfluxDB-Client
    private readonly InfluxDBClient _client;

    // Konstruktor initialisiert den Dienst mit den Konfigurationsoptionen
    public InfluxDbClientService(IOptions<InfluxDbOptions> options)
    {
        _options = options.Value;
        _client = new InfluxDBClient(_options.Url, _options.Token);
    }

    // Schreibt Sensordaten in die InfluxDB
    public async Task<DateTime> WriteSensorDataAsync(string measurement, float temperature, float humidity, CancellationToken cancellationToken)
    {
        // Hole das Schreib-API zum Schreiben der Daten
        var writeApi = _client.GetWriteApiAsync();

        // Erstelle ein Point-Objekt mit den Messwerten
        var timestamp = DateTime.UtcNow;
        var point = PointData.Measurement(measurement)
            .Field("temperature", temperature)
            .Field("humidity", humidity)
            .Timestamp(timestamp, WritePrecision.Ns);
        // Schreibe den Punkt in die InfluxDB
        await writeApi.WritePointAsync(point, _options.Bucket, _options.Organization, cancellationToken);
        return timestamp;
    }

    public async Task<(double Temperature, double Humidity)> ReadSensorDataInfluxQlAsync(string measurement, DateTime timestamp, CancellationToken cancellationToken)
    {
        // The bucket is mapped to a DBRP named after the bucket with the default retention policy.
        var query = $"SELECT \"temperature\", \"humidity\" FROM \"{measurement}\" WHERE time >= '{timestamp:O}' AND time < '{timestamp.AddTicks(1):O}'";
        var url = $"{_options.Url.TrimEnd('/')}/query?db={Uri.EscapeDataString(_options.Bucket)}&rp=autogen&q={Uri.EscapeDataString(query)}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Token", _options.Token);
        using var response = await HttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        var series = document.RootElement.GetProperty("results")[0].GetProperty("series")[0];
        var columns = series.GetProperty("columns").EnumerateArray().Select(column => column.GetString()).ToArray();
        var values = series.GetProperty("values")[0].EnumerateArray().ToArray();
        var temperatureIndex = Array.IndexOf(columns, "temperature");
        var humidityIndex = Array.IndexOf(columns, "humidity");
        if (temperatureIndex >= 0 && humidityIndex >= 0 &&
            values[temperatureIndex].TryGetDouble(out var temperature) &&
            values[humidityIndex].TryGetDouble(out var humidity))
        {
            return (Math.Round(temperature, 1), Math.Round(humidity, 1));
        }

        throw new InvalidOperationException("InfluxQL returned no complete sensor data point.");
    }

    public async Task<(double Temperature, double Humidity)> ReadSensorDataAsync(string measurement, DateTime timestamp, CancellationToken cancellationToken)
    {
        // Das exklusive Ende einen Tick später begrenzt die Abfrage auf den geschriebenen Punkt.
        var query = $$"""
            from(bucket: {{JsonSerializer.Serialize(_options.Bucket)}})
              |> range(start: {{timestamp:O}}, stop: {{timestamp.AddTicks(1):O}})
              |> filter(fn: (r) => r._measurement == {{JsonSerializer.Serialize(measurement)}})
              |> filter(fn: (r) => r._field == "temperature" or r._field == "humidity")
              |> pivot(rowKey: ["_time"], columnKey: ["_field"], valueColumn: "_value")
            """;
        var tables = await _client.GetQueryApi().QueryAsync(query, _options.Organization, cancellationToken);
        var record = tables.SelectMany(table => table.Records).SingleOrDefault();

        if (record?.GetValueByKey("temperature") is double temperature &&
            record.GetValueByKey("humidity") is double humidity)
        {
            return (Math.Round(temperature, 1), Math.Round(humidity, 1));
        }

        throw new InvalidOperationException("Der geschriebene Messpunkt wurde nicht vollständig gelesen.");
    }
}
