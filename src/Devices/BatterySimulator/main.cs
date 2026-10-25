using System.Text;
using Batterie_Sim;

Console.OutputEncoding = Encoding.UTF8;

if (ArgsParser.IsHelpRequested(args))
{
    Console.WriteLine(ArgsParser.Usage);
    return 0;
}

if (args.Any(a => a.Equals("--register-plan", StringComparison.OrdinalIgnoreCase)))
{
    Console.Write(Modbus.Contracts.BatteryRegisters.Plan.ToMarkdown());
    return 0;
}

SimulatorOptions options;
try
{
    options = ArgsParser.Parse(args);
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"Fehler: {ex.Message}\n\n{ArgsParser.Usage}");
    return 1;
}

var random = options.Seed is int seed ? new Random(seed) : new Random();
var battery = new Batterie(options.Config);
var profile = new DailyProfile(options.Config, random);
var simulator = new Simulator(battery, profile, options);

using var stop = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;                                              // geordnet beenden (Abmeldung beim Master, CSV schließen)
    stop.Cancel();
};

try
{
    string csvPath = await simulator.RunAsync(stop.Token);
    Console.WriteLine($"CSV gespeichert: {Path.GetFullPath(csvPath)}");
    return 0;
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
{
    Console.Error.WriteLine($"Fehler (Modbus-Port oder CSV-Datei): {ex.Message}");
    return 1;
}
