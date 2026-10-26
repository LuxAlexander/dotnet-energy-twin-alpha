using System.Globalization;
using System.Text;
using KuehlhausSimulator.IO;
using KuehlhausSimulator.Simulation;

namespace KuehlhausSimulator;

/// <summary>
/// Einstiegspunkt des Kühlhauses. Aufbau wie beim Dual-Port-Charger: Argumente lesen, Modbus-Server und Simulation
/// starten, bei Strg+C geordnet beenden (Abmeldung beim Master, CSV schließen, Modbus freigeben).
/// </summary>
public static class Program
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

    public static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;                      // °C und Umlaute in der Konsole

        if (ArgsParser.IsHelpRequested(args))
        {
            Console.WriteLine(ArgsParser.Usage);
            return 0;
        }

        if (args.Any(a => a.Equals("--register-plan", StringComparison.OrdinalIgnoreCase)))
        {
            Console.Write(Modbus.Contracts.KuehlhausRegisters.Plan.ToMarkdown(port: SimulationConfig.DefaultModbusPort));
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
            e.Cancel = true;                                         // geordnet beenden (Abmeldung beim Master, CSV schließen)
            stop.Cancel();
        };

        try
        {
            string csvPath = await engine.RunAsync(stop.Token);

            Console.WriteLine();
            Console.WriteLine($"Kühlhaus {engine.Config.DeviceId} beendet");
            Console.WriteLine($"  Zeit:     {engine.SimulationTime?.ToString("dd.MM.yyyy HH:mm", Culture) ?? "keine virtuelle Zeit"}");
            Console.WriteLine($"  Raum:     {engine.Room.RoomTempC.ToString("0.0", Culture)} °C");
            Console.WriteLine($"  Energie:  {engine.Room.EnergyKWh.ToString("0.00", Culture)} kWh");
            Console.WriteLine($"  CSV:      {Path.GetFullPath(csvPath)}");
            return 0;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Fehler (Modbus-Port oder CSV-Datei): {ex.Message}");
            return 1;
        }
    }
}
