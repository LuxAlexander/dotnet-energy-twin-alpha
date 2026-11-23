using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IInfluxDbClientService
{
    Task<DateTime> WriteSensorDataAsync(string measurement, float temperature, float humidity, CancellationToken cancellationToken);
    Task<(double Temperature, double Humidity)> ReadSensorDataAsync(string measurement, DateTime timestamp, CancellationToken cancellationToken);
    Task<(double Temperature, double Humidity)> ReadSensorDataInfluxQlAsync(string measurement, DateTime timestamp, CancellationToken cancellationToken);
}

