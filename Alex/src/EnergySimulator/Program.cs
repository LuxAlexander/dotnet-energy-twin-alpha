using EnergySimulator.Simulation;

// Top-Level-Statements: C# erzeugt daraus eine Main-Methode. Lokale
// Funktionen am Dateiende (ParseArgs, PrintHelp) koennen erst nach dem
// letzten "normalen" Programmcode stehen.
Console.OutputEncoding = System.Text.Encoding.UTF8;//steigert die Kompatibilität

var config = ParseArgs(args);
var engine = new SimulationEngine(config);

TraceStart("engine-erstellt");

string csvPath = Path.GetFullPath(Path.Combine(config.OutputDirectory, config.CsvFileName));

Console.WriteLine();
Console.WriteLine("\x1b[1m\x1b[36m  PV-Anlage – Konfiguration\x1b[0m");
Console.WriteLine($"  Nennleistung    {config.PeakPowerKw,8:F2} kW");
Console.WriteLine($"  Zeitraffer      x{config.Timescale}");
Console.WriteLine($"  CSV-Intervall   {config.StepMinutes,8} min");
Console.WriteLine($"  Bild-Aktualis.  {config.RefreshMs,8} ms");
Console.WriteLine($"  Startzeit       {engine.Clock.Start:dd.MM.yyyy HH:mm:ss}");
Console.WriteLine($"  CSV-Datei       {csvPath}");
Console.WriteLine();

TraceStart("banner-gezeichnet");

try
{
    engine.Start();
}
finally
{
    TraceStart("start-beendet");
    EnergySimulator.Output.ConsoleDisplay.Release();
}

static void TraceStart(string step)
{
    string? path = Environment.GetEnvironmentVariable("ENERGYSIM_RENDER_LOG");
    if (path is null)
        return;

    try { File.AppendAllText(path, $"{DateTime.Now:HH:mm:ss.fff};START;{step}\n"); }
    catch { }
}

Console.WriteLine();
Console.Write("\x1b[1m\x1b[36m");
Console.WriteLine("  Simulation beendet");
Console.Write("\x1b[0m");
Console.WriteLine($"  Laufzeit:      {engine.Clock.Start:dd.MM.yyyy HH:mm:ss} → {engine.Clock.Now:dd.MM.yyyy HH:mm:ss}");
Console.WriteLine($"  Gesamtenergie: {engine.Pv.TotalEnergyKwh:F2} kWh");//F2 is floating-point formatting with exactly 2 decimal places
Console.WriteLine($"  CSV:           {csvPath}");

static SimulationConfig ParseArgs(string[] args)
{
    var config = new SimulationConfig();

    for (int i = 0; i < args.Length; i++)
    {
        switch (args[i].ToLowerInvariant())
        {
            case "--timescale" or "-s" when i + 1 < args.Length:
                config.Timescale = int.Parse(args[++i]);
                break;
            case "--step" or "-t" when i + 1 < args.Length:
                config.StepMinutes = int.Parse(args[++i]);
                break;
            case "--refresh" or "-r" when i + 1 < args.Length:
                // Untergrenze 50 ms, damit der Diff-Modus die Konsole nicht
                // mit zu schnellen Neuzeichnungen flackern laesst.
                config.RefreshMs = Math.Max(50, int.Parse(args[++i]));
                break;
            case "--peak" or "-p" when i + 1 < args.Length:
                // InvariantCulture beim Einlesen: mit de-DE-Region wuerde sonst
                // "10.5" als Dezimaltrennpunkt abgelehnt.
                config.PeakPowerKw = double.Parse(args[++i], System.Globalization.CultureInfo.InvariantCulture);
                break;
            case "--output" or "-o" when i + 1 < args.Length:
                config.OutputDirectory = args[++i];
                break;
            case "--start" or "-d" when i + 1 < args.Length:
                if (DateTime.TryParse(args[++i], out DateTime parsed))
                    config.StartDate = parsed;
                else
                    Console.Error.WriteLine($"Ungueltiges Startdatum: {args[i]} (erwartet z.B. 2026-01-15)");
                break;
            case "--help" or "-h":
                PrintHelp();
                Environment.Exit(0);
                break;
        }
    }

    return config;
}

static void PrintHelp()
{
    Console.WriteLine(@"
EnergySimulator – PV-Anlage (LB01)
===================================

Die Simulation läuft ohne Zeitlimit und startet mit aktuellem Datum und Uhrzeit.

Optionen:
  -s, --timescale <n>   Zeitraffer x1, x15, x60, x300, x1800 … (Default: 60)
  -t, --step <min>      CSV-Intervall in Minuten (Default: 15)
  -r, --refresh <ms>    Bildschirm-Aktualisierung in Millisekunden (Default: 400)
                        Hoeher = ruhiger und besser lesbar, z.B. 1000
  -p, --peak <kw>       Nennleistung in kW (Default: 10.0)
  -o, --output <dir>    CSV-Verzeichnis (Default: data)
  -d, --start <datum>   Startdatum, z.B. 2026-01-15 (Default: jetzt)
  -h, --help            Diese Hilfe

Tasten – Steuerung:
  P        Pause / Fortsetzen
  M        Modus AUTO / MANUELL
  Q        Beenden

Tasten – Störungen:
  F        Wechselrichterfehler ein-/auslösen
  T        Überhitzung erzwingen / aufheben
  G        Netzspannungsstörung ein-/ausschalten
  C        Module reinigen (Verschmutzung zurücksetzen)

Tasten – Wetter:
  0 Nacht   1 Sonnig   2 Bewölkt   3 Dicht bewölkt   4 Gewitter

Tasten – Zeitraffer:
  5 x1    6 x15    7 x60    8 x300    9 x1800

Beispiele:
  dotnet run -- -s 300
  dotnet run -- -s 1800 -p 15.5 -t 10
");
}