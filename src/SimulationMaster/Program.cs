using System.Net.Sockets;
using SimCore.Time;
using SimCore.TimeSync;

namespace SimulationMaster;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Contains("--help") || args.Contains("-h"))
        {
            Console.WriteLine(MasterOptions.Usage);
            return 0;
        }

        MasterOptions options;
        try
        {
            options = MasterOptions.Parse(args);
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine($"Fehler: {ex.Message}");
            Console.Error.WriteLine();
            Console.Error.WriteLine(MasterOptions.Usage);
            return 1;
        }

        using var stop = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;                                      // geordnet beenden statt Prozess abbrechen
            stop.Cancel();
        };

        var clock = new MasterClock(TimeProvider.System, options.StartTime, options.Speed, options.LeadTime);
        var server = new TimeSyncServer(clock, options.Port, options.BindAddress, options.HeartbeatInterval);
        var console = new MasterConsole(clock, server, options);
        server.Log += (_, message) => console.AddMessage(message);

        try
        {
            server.Start();
        }
        catch (SocketException ex)
        {
            Console.Error.WriteLine($"Fehler: Zeitserver kann {options.BindAddress}:{options.Port} nicht öffnen ({ex.Message}).");
            Console.Error.WriteLine("Läuft schon ein Master? Anderen Port mit --port wählen.");
            return 2;
        }

        console.AddMessage($"Zeitserver bereit auf {server.LocalEndPoint}");
        if (options.AutoStart)
            clock.Resume();
        else
            console.AddMessage("Simulation pausiert – Start mit Leertaste");

        var serverTask = server.RunAsync(stop.Token);
        await console.RunAsync(stop.Cancel, stop.Token);
        await stop.CancelAsync();
        await serverTask;

        await console.ShutdownAsync();
        Console.WriteLine();
        Console.WriteLine("Simulations-Master beendet, alle Simulatoren wurden benachrichtigt.");
        return 0;
    }
}
