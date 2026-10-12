using System.Globalization;
using Batterie_Sim;

// ---------- Standardwerte ----------
var config = new BatterieConfig();   // ID, Kapazität, Leistungen und SOC-Werte stehen als Standard in der Config
double speed = 600;                  // Simulationssekunden pro Realsekunde
double step = 60;                    // Schrittweite in Simulationssekunden
double duration = 0;                // Stunden, 0 = endlos
TimeSpan start = TimeSpan.Zero;      // Startuhrzeit

// ---------- Argumente einlesen ----------
for (int i = 0; i < args.Length; i++)
{
    string name = args[i];

    // Jedes Argument braucht einen Wert, z. B. "--capacity 30"
    if (i + 1 >= args.Length)
    {
        Console.WriteLine($"Fehler: Zu '{name}' fehlt der Wert.");
        return;
    }
    string text = args[++i];

    // Textwerte
    if (name == "--id")
    {
        config.DeviceId = text;
        continue;
    }
    if (name == "--start")
    {
        if (!TimeSpan.TryParse(text, CultureInfo.InvariantCulture, out start))
        {
            Console.WriteLine($"Fehler: '{text}' ist keine gültige Uhrzeit (Beispiel: 06:00).");
            return;
        }
        continue;
    }

    // Zahlenwerte (mit Punkt als Dezimalzeichen, z. B. 7.5)
    if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
    {
        Console.WriteLine($"Fehler: '{text}' ist keine gültige Zahl für '{name}'.");
        return;
    }

    switch (name)
    {
        case "--capacity": config.CapacityKWh = number; break;
        case "--max-charge": config.MaxChargeKW = number; break;
        case "--max-discharge": config.MaxDischargeKW = number; break;
        case "--soc-min": config.SocMinPercent = number; break;
        case "--soc-max": config.SocMaxPercent = number; break;
        case "--soc-start": config.StartSocPercent = number; break;
        case "--speed": speed = number; break;
        case "--step": step = number; break;
        case "--duration": duration = number; break;
        default:
            Console.WriteLine($"Fehler: Unbekanntes Argument '{name}'.");
            return;
    }
}

// ---------- Prüfungen ----------
if (speed <= 0 || step <= 0)
{
    Console.WriteLine("Fehler: --speed und --step müssen größer als 0 sein.");
    return;
}
if (config.CapacityKWh <= 0)
{
    Console.WriteLine("Fehler: --capacity muss größer als 0 sein.");
    return;
}
if (config.SocMinPercent >= config.SocMaxPercent)
{
    Console.WriteLine("Fehler: --soc-min muss kleiner als --soc-max sein.");
    return;
}

// ---------- Objekte erzeugen (erst NACH dem Einlesen der Argumente) ----------
var battery = new Batterie(config);
var profile = new DailyProfile(config);
var logger = new CsvLogger(config);
var dashboard = new ConsoleDashboard();
var sim = new Simulator(battery, profile, start, logger, dashboard);

sim.Speed = speed;
sim.StepSeconds = step;

// ---------- Start ----------
sim.Run(duration);
