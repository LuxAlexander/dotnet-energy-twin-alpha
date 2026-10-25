using System.Text;
using EnergySimulator.Output;
using EnergySimulator.Simulation;

Console.OutputEncoding = Encoding.UTF8;                           // steigert die Kompatibilität

if (ArgsParser.IsHelpRequested(args))
{
    Console.WriteLine(ArgsParser.Usage);
    return 0;
}

if (args.Any(a => a.Equals("--register-plan", StringComparison.OrdinalIgnoreCase)))
{
    Console.Write(Modbus.Contracts.PvRegisters.Plan.ToMarkdown());
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
    e.Cancel = true;                                              // geordnet beenden (Abmeldung beim Master, CSV schließen)
    stop.Cancel();
};

try
{
    string csvPath = await engine.RunAsync(stop.Token);
    ConsoleDisplay.Release();

    Console.WriteLine();
    Console.Write("\x1b[1m\x1b[36m");
    Console.WriteLine("  Simulation beendet");
    Console.Write("\x1b[0m");
    Console.WriteLine($"  Zeit:          {engine.SimulationTime:dd.MM.yyyy HH:mm:ss}");
    Console.WriteLine($"  Gesamtenergie: {engine.Pv.TotalEnergyKwh:F2} kWh");
    Console.WriteLine($"  CSV:           {Path.GetFullPath(csvPath)}");
    return 0;
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
{
    ConsoleDisplay.Release();
    Console.Error.WriteLine($"Fehler (Modbus-Port oder CSV-Datei): {ex.Message}");
    return 1;
}
