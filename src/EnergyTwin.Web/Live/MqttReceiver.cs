using System.Diagnostics;
using EnergyTwin.Web.Management;
using MQTTnet;
using MQTTnet.Protocol;
using Shared.Config;
using SimCore.Contracts;

namespace EnergyTwin.Web.Live;

public sealed class MqttReceiver(LiveStore store, MasterControlClient master, InstanceStore instances,
    IConfiguration configuration, IHostEnvironment environment, ILogger<MqttReceiver> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delay = 1;
        while (!stoppingToken.IsCancellationRequested)
        {
            using var client = new MqttClientFactory().CreateMqttClient();
            client.ApplicationMessageReceivedAsync += args =>
            {
                if (args.ApplicationMessage.Topic.StartsWith("energytwin/master/", StringComparison.Ordinal))
                    master.Receive(client, args.ApplicationMessage);
                else store.Receive(args.ApplicationMessage.Topic, args.ApplicationMessage.ConvertPayloadToString());
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
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(10))
                    .WithTimeout(TimeSpan.FromSeconds(5))
                    .WithCredentials(credentials.User, credentials.Password).Build();
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                timeout.CancelAfter(TimeSpan.FromSeconds(10));
                var result = await client.ConnectAsync(options, timeout.Token);
                if (result.ResultCode != MqttClientConnectResultCode.Success) throw new InvalidOperationException();
                master.SetConnection(client);
                // Vor dem Abo markieren, damit retained Nachrichten zur neuen Verbindung gehören.
                store.SetConnection(true, "Mit RabbitMQ verbunden");
                var subscriptions = MasterSubscriptions()
                    .WithTopicFilter(f => f.WithTopic("energytwin/#").WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce));
                foreach (var location in new[] { "salzburg", "wien", "innsbruck", "tamanrasset", "neumayer" })
                    subscriptions.WithTopicFilter(f => f.WithTopic($"energytwin/weather/{location}/measurements").WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce));
                subscriptions.WithTopicFilter(f => f.WithTopic("energytwin/weather/status").WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce));
                // RabbitMQ liefert gespeicherte (retained) Nachrichten nicht an energytwin/#; exakte
                // Status-Topics holen den aktuellen Stand sofort nach einem Webdienst-Neustart.
                subscriptions.WithTopicFilter(f => f.WithTopic($"energytwin/gateway/{configuration["Mqtt:GatewayId"] ?? "GW-01"}/status")
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce));
                foreach (var instance in instances.Instances.Where(instance => instance.ModbusPort is { }))
                    subscriptions.WithTopicFilter(f => f.WithTopic($"energytwin/devices/{instance.Id}/status")
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce));
                await SubscribeAsync(client, subscriptions, timeout.Token);
                master.ConfirmSubscriptions();
                delay = 1;
                var lastMasterSubscription = Stopwatch.GetTimestamp();
                var masterWasAvailable = false;
                var stalled = false;
                while (client.IsConnected)
                {
                    if (master.Available)
                        masterWasAvailable = true;
                    else if (masterWasAvailable)
                    {
                        // Eine vollständige Neuverbindung je Ausfall. Solange der Master aus
                        // bleibt, werden nur seine Abos aufgefrischt.
                        stalled = true;
                        logger.LogWarning("Master-Status ausgeblieben. MQTT-Empfang wird neu verbunden.");
                        break;
                    }
                    else if (Stopwatch.GetElapsedTime(lastMasterSubscription) >= MasterControlClient.StatusTimeout)
                    {
                        using var subscribeTimeout = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                        subscribeTimeout.CancelAfter(TimeSpan.FromSeconds(5));
                        await SubscribeAsync(client, MasterSubscriptions(), subscribeTimeout.Token);
                        lastMasterSubscription = Stopwatch.GetTimestamp();
                    }
                    await Task.Delay(500, stoppingToken);
                }
                store.SetConnection(false, stalled ? "Master-Status ausgeblieben – MQTT verbindet sich neu"
                    : "Broker getrennt – letzte Werte nicht aktuell bestätigt");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception)
            {
                // Keine Exceptiontexte aus Transport oder Konfiguration in die Oberfläche übernehmen.
                store.SetConnection(false, "Verbindung fehlgeschlagen – Broker, Port und Zugangsdaten prüfen. Automatischer Neuversuch.");
            }
            finally { master.SetConnection(null); }
            try { await Task.Delay(TimeSpan.FromSeconds(delay), stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            delay = Math.Min(delay * 2, 30);
        }
        store.SetConnection(false, "Empfang beendet");
    }

    private static MqttClientSubscribeOptionsBuilder MasterSubscriptions() => new MqttClientSubscribeOptionsBuilder()
        .WithTopicFilter(f => f.WithTopic(MasterControl.StatusTopic).WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce))
        .WithTopicFilter(f => f.WithTopic(MasterControl.ResultTopic).WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce));

    private static async Task SubscribeAsync(IMqttClient client, MqttClientSubscribeOptionsBuilder subscriptions,
        CancellationToken cancellationToken)
    {
        var result = await client.SubscribeAsync(subscriptions.Build(), cancellationToken);
        if (result.Items.Any(item => (int)item.ResultCode >= 128))
            throw new InvalidOperationException("Broker hat ein MQTT-Abo abgelehnt.");
    }
}
