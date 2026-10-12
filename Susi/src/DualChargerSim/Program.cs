using DualChargerSim.IO;
using DualChargerSim.Simulation;

namespace DualChargerSim;

public static class Program
{
    public static int Main(string[] args)
    {
        if (ArgsParser.IsHelpRequested(args))
        {
            Console.WriteLine(ArgsParser.Usage);
            return 0;
        }

        try
        {
            new SimulationEngine(ArgsParser.Parse(args)).Run();
            return 0;
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine($"Fehler: {ex.Message}\n\n{ArgsParser.Usage}");
            return 1;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Fehler beim Schreiben der CSV-Datei: {ex.Message}");
            return 1;
        }
    }
}
