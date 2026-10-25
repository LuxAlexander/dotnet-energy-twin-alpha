using DualChargerSim.IO;
using DualChargerSim.Simulation;

namespace DualChargerSim;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (ArgsParser.IsHelpRequested(args))
        {
            Console.WriteLine(ArgsParser.Usage);
            return 0;
        }

        SimulationEngine engine;
        try
        {
            engine = new SimulationEngine(ArgsParser.Parse(args));
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine($"Fehler: {ex.Message}\n\n{ArgsParser.Usage}");
            return 1;
        }

        using var stop = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;                                      // geordnet beenden (Abmeldung beim Master, CSV schließen)
            stop.Cancel();
        };

        try
        {
            string csvPath = await engine.RunAsync(stop.Token);
            Console.WriteLine($"CSV gespeichert: {Path.GetFullPath(csvPath)}");
            return 0;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Fehler beim Schreiben der CSV-Datei: {ex.Message}");
            return 1;
        }
    }
}
