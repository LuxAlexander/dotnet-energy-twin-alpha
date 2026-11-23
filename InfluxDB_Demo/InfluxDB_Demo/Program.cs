using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InfluxDB_Demo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                // Konfiguration wird aus appsettings.json geladen
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                // Konfiguration der Dienste über Dependency Injection
                .ConfigureServices((hostContext, services) =>
                {
                    // Konfiguration für InfluxDB
                    services.Configure<InfluxDbOptions>(hostContext.Configuration.GetSection("InfluxDB"));
                    // Registrierung der InfluxDB-Client-Dienste
                    services.AddSingleton<IInfluxDbClientService, InfluxDbClientService>();
                    // Registrierung des Workers
                    // Worker wird automatisch gestartet, wenn die Anwendung gestartet wird
                    services.AddHostedService<SensorDataWorker>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}
