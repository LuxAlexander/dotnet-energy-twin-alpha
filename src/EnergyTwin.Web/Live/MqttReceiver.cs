using MQTTnet;
using MQTTnet.Protocol;
using Shared.Config;

namespace EnergyTwin.Web.Live;

public sealed class MqttReceiver(LiveStore store, IConfiguration configuration, IHostEnvironment environment) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delay = 1;
        while (!stoppingToken.IsCancellationRequested)
        {
            using var client = new MqttClientFactory().CreateMqttClient();
            client.ApplicationMessageReceivedAsync += args =>
            {
                store.Receive(args.ApplicationMessage.Topic, args.ApplicationMessage.ConvertPayloadToString());
                return Task.CompletedTask;
            };
            client.DisconnectedAsync += args =>
            {
                store.SetConnection(false, "Broker getrennt – letzte Werte nicht aktuell bestätigt");
                return Task.CompletedTask;
            };
            try
            {
                var envFile = configuration["Mqtt:EnvFile"];
                if (string.IsNullOrWhiteSpace(envFile))
                {
                    var directory = new DirectoryInfo(environment.ContentRootPath);
                    while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "EnergyTwin.sln"))) directory = directory.Parent;
                    envFile = directory is null ? null : Path.Combine(directory.FullName, "docker", ".env");
                }
                var credentials = MqttCredentials.Resolve(null, envFile, Environment.GetEnvironmentVariable);
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer(configuration["Mqtt:Host"] ?? "127.0.0.1", configuration.GetValue("Mqtt:Port", 1883))
                    .WithClientId($"energytwin-web-{Guid.NewGuid():N}").WithCleanSession()
                    .WithCredentials(credentials.User, credentials.Password).Build();
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                timeout.CancelAfter(TimeSpan.FromSeconds(10));
                var result = await client.ConnectAsync(options, timeout.Token);
                if (result.ResultCode != MqttClientConnectResultCode.Success) throw new InvalidOperationException();
                // Vor dem Abo markieren, damit retained Nachrichten zur neuen Verbindung gehören.
                store.SetConnection(true, "Mit RabbitMQ verbunden");
                var subscriptions = new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f.WithTopic("energytwin/#").WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce));
                foreach (var location in new[] { "salzburg", "wien", "innsbruck", "tamanrasset", "neumayer" })
                    subscriptions.WithTopicFilter(f => f.WithTopic($"energytwin/weather/{location}/measurements").WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce));
                subscriptions.WithTopicFilter(f => f.WithTopic("energytwin/weather/status").WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce));
                var subscribed = await client.SubscribeAsync(subscriptions.Build(), timeout.Token);
                if (subscribed.Items.Any(item => (int)item.ResultCode >= 128)) throw new InvalidOperationException();
                delay = 1;
                while (client.IsConnected) await Task.Delay(500, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception)
            {
                // Keine Exceptiontexte aus Transport oder Konfiguration in die Oberfläche übernehmen.
                store.SetConnection(false, "Verbindung fehlgeschlagen – Broker, Port und Zugangsdaten prüfen. Automatischer Neuversuch.");
            }
            try { await Task.Delay(TimeSpan.FromSeconds(delay), stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            delay = Math.Min(delay * 2, 30);
        }
        store.SetConnection(false, "Empfang beendet");
    }
}
