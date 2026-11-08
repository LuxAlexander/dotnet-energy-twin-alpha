using System.Text;
using Modbus.Contracts;
using ModbusRabbitGateway;

Console.OutputEncoding = Encoding.UTF8;

if (GatewayOptions.IsHelpRequested(args))
{
    Console.WriteLine(GatewayOptions.Usage);
    return 0;
}

GatewayOptions options;
try
{
    options = GatewayOptions.Parse(args);
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"Fehler: {ex.Message}\n\n{GatewayOptions.Usage}");
    return 64;
}

if (options.ListKeys)
{
    PrintKeys();
    return 0;
}

using var app = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;                                              // geordnet beenden statt hart abbrechen
    app.Cancel();
};

var pollers = options.Devices.Select(d => new DevicePoller(d, options.TimeoutMs, options.Quality, options.StatusRepeat)).ToList();
MqttPublisher? mqtt = null;
try
{
    IMessagePublisher publisher;
    if (options.ConsoleOnly)
    {
        publisher = new ConsolePublisher(Console.Out, options.ShowJson);
    }
    else
    {
        var credentials = MqttCredentials.Resolve(options.MqttUser, options.EnvFile, Environment.GetEnvironmentVariable);
        mqtt = new MqttPublisher(new BrokerSettings(options.BrokerHost, options.BrokerPort, options.GatewayId, credentials),
            options.GatewayId, TimeProvider.System);
        publisher = options.ShowJson ? new TeePublisher(mqtt, new ConsolePublisher(Console.Out, true)) : mqtt;
    }

    Console.WriteLine($"Modbus–RabbitMQ-Gateway {options.GatewayId} – Ziel: {publisher.Description}");
    Console.WriteLine($"Geräte: {string.Join("  ", options.Devices)} · Intervall {options.Interval.TotalSeconds:0.###} s");
    if (!options.Once)
        Console.WriteLine("Beenden mit Q oder Strg+C.");

    var keys = options.Once || Console.IsInputRedirected ? Task.CompletedTask : WatchQuitKeyAsync(app);
    var service = new GatewayService(options, pollers, publisher, TimeProvider.System, Console.Out);
    await service.RunAsync(app.Token);
    app.Cancel();
    await keys;
}
catch (OperationCanceledException) when (app.IsCancellationRequested)
{
    // Q oder Strg+C
}
finally
{
    foreach (var poller in pollers)
        poller.Dispose();
    if (mqtt is not null)
        await mqtt.DisposeAsync();                                // geordnet trennen (kein Last Will)
}

Console.WriteLine("Gateway beendet.");
return 0;

static async Task WatchQuitKeyAsync(CancellationTokenSource app)
{
    try
    {
        while (!app.IsCancellationRequested)
        {
            while (Console.KeyAvailable)
            {
                if (Console.ReadKey(intercept: true).Key == ConsoleKey.Q)
                {
                    app.Cancel();
                    return;
                }
            }
            await Task.Delay(100, app.Token);
        }
    }
    catch (OperationCanceledException)
    {
        // Programmende
    }
}

static void PrintKeys()
{
    foreach (var type in Enum.GetValues<DeviceType>())
    {
        if (RegisterPlans.For(type) is not { } plan)
            continue;
        Console.WriteLine($"# {plan.DeviceName} ({type})");
        foreach (var key in KeyCatalog.For(plan))
            Console.WriteLine(key);
        Console.WriteLine();
    }
}
