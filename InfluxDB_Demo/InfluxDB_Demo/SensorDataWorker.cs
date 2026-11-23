using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SensorDataWorker : BackgroundService
{
    private readonly ILogger<SensorDataWorker> _logger;
    private readonly IInfluxDbClientService _influxDbClient;
    private readonly Random _random = new();

    public SensorDataWorker(ILogger<SensorDataWorker> logger, IInfluxDbClientService influxDbClient)
    {
        _logger = logger;
        _influxDbClient = influxDbClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SensorDataWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var temperature = MathF.Round(_random.Next(15, 30) + _random.NextSingle(), 1);
            var humidity = MathF.Round(_random.Next(30, 70) + _random.NextSingle(), 1);

            const string measurement = "sensor_data";
            var timestamp = await _influxDbClient.WriteSensorDataAsync(
                measurement, temperature, humidity, stoppingToken);
            _logger.LogInformation("Written data at {Timestamp:O}: Temperature={Temperature:F1}, Humidity={Humidity:F1}",
                timestamp, temperature, humidity);

            var stored = await _influxDbClient.ReadSensorDataAsync(measurement, timestamp, stoppingToken);
            _logger.LogInformation("Read back at {Timestamp:O}: Temperature={Temperature:F1}, Humidity={Humidity:F1}",
                timestamp, stored.Temperature, stored.Humidity);

            var storedWithInfluxQl = await _influxDbClient.ReadSensorDataInfluxQlAsync(measurement, timestamp, stoppingToken);
            _logger.LogInformation("Read back with InfluxQL at {Timestamp:O}: Temperature={Temperature:F1}, Humidity={Humidity:F1}",
                timestamp, storedWithInfluxQl.Temperature, storedWithInfluxQl.Humidity);

            await Task.Delay(1000, stoppingToken);
        }
    }
}
