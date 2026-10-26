using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using Devices.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimCore.Contracts;
using WeatherSimulator.Simulation;

Console.OutputEncoding = Encoding.UTF8;

if (args.Any(a => a is "--help" or "-h" or "/?"))
{
    Console.WriteLine(Usage);
    return 0;
}

int port = 8080;
for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--port":
            if (i + 1 >= args.Length || !int.TryParse(args[++i], NumberStyles.Integer, CultureInfo.InvariantCulture, out port)
                || port is < 1 or > 65535)
            {
                Console.Error.WriteLine("Fehler: --port erwartet eine Portnummer (1–65535).");
                return 1;
            }
            break;
        default:
            Console.Error.WriteLine($"Fehler: Unbekanntes Argument '{args[i]}'.\n\n{Usage}");
            return 1;
    }
}

var manual = new ManualWeather();
var model = new WeatherModel(manual);
var last = new LastRequest();

var builder = WebApplication.CreateBuilder();
builder.Logging.SetMinimumLevel(LogLevel.Warning);
builder.WebHost.UseUrls($"http://127.0.0.1:{port}");
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
var app = builder.Build();

app.MapGet("/weather", (string? location, string? simTime, int? run) =>
{
    string key = string.IsNullOrWhiteSpace(location) ? LocationCatalog.DefaultKey : location;
    if (!LocationCatalog.TryGet(key, out var found))
        return Results.NotFound(new { error = $"Unbekannter Standort: '{location}'." });

    if (!TryParseSimTime(simTime, out var time))
        return Results.BadRequest(new { error = "simTime erwartet das Format yyyy-MM-ddTHH:mm:ss (UTC)." });

    var data = model.Get(found.Key, time, run ?? 0);
    last.Update(data);
    return Results.Ok(data);
});

app.MapGet("/locations", () => Results.Ok(LocationCatalog.All));

app.MapGet("/", () => Results.Ok(new
{
    service = "WeatherSimulator",
    endpoint = "/weather?location=<kennung>&simTime=<yyyy-MM-ddTHH:mm:ss>&run=<lauf>",
    locations = LocationCatalog.All.Select(l => l.Key)
}));

using var stop = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    stop.Cancel();
};

await app.StartAsync(stop.Token);
Console.WriteLine($"Wettersimulator läuft auf http://127.0.0.1:{port}/weather");
Console.WriteLine("Tasten: 1 sonnig · 2 bewölkt · 3 dicht bewölkt · 4 Gewitter · S Schnee · R Regen · T Temperatur · W Wind · X Automatik · Q Ende");

bool running = true;
bool interactive = !Console.IsOutputRedirected;
var nextRender = DateTime.MinValue;
var nextLog = DateTime.MinValue;
string notification = "";
DateTime notificationAt = DateTime.MinValue;

while (running && !stop.IsCancellationRequested)
{
    while (AsyncConsole.TryReadKey() is { } key)
    {
        switch (key.Key)
        {
            case ConsoleKey.D1 or ConsoleKey.NumPad1:
                manual.ApplyPreset(WeatherCondition.Sunny);
                SetNote("Manuell: SONNIG");
                break;
            case ConsoleKey.D2 or ConsoleKey.NumPad2:
                manual.ApplyPreset(WeatherCondition.PartlyCloudy);
                SetNote("Manuell: BEWÖLKT");
                break;
            case ConsoleKey.D3 or ConsoleKey.NumPad3:
                manual.ApplyPreset(WeatherCondition.Overcast);
                SetNote("Manuell: DICHT BEWÖLKT");
                break;
            case ConsoleKey.D4 or ConsoleKey.NumPad4:
                manual.ApplyPreset(WeatherCondition.Storm);
                SetNote("Manuell: GEWITTER");
                break;
            case ConsoleKey.S:
                manual.ApplyPreset(WeatherCondition.Snow);
                SetNote("Manuell: SCHNEE");
                break;
            case ConsoleKey.R:
                manual.SetRain();
                SetNote("Manuell: REGEN");
                break;
            case ConsoleKey.T:
                await SetNumberAsync("Temperatur in °C: ", manual.SetTemperature);
                break;
            case ConsoleKey.W:
                await SetNumberAsync("Wind in m/s: ", manual.SetWind);
                break;
            case ConsoleKey.X:
                manual.Clear();
                SetNote("Automatik (deterministisches Modell)");
                break;
            case ConsoleKey.Q:
                running = false;
                break;
        }
    }

    var now = DateTime.UtcNow;
    if (interactive)
    {
        if (now >= nextRender)
        {
            Render();
            nextRender = now.AddMilliseconds(400);
        }
    }
    else if (now >= nextLog)
    {
        await Console.Out.WriteLineAsync(StatusLine());
        nextLog = now.AddSeconds(5);
    }

    if (!running)
        break;

    try
    {
        await Task.Delay(80, stop.Token);
    }
    catch (OperationCanceledException)
    {
        break;
    }
}

await app.StopAsync(CancellationToken.None);
Console.WriteLine("\nWettersimulator beendet.");
return 0;

void SetNote(string text)
{
    notification = text;
    notificationAt = DateTime.UtcNow;
}

async Task SetNumberAsync(string prompt, Action<double> apply)
{
    await Console.Out.WriteAsync(prompt);
    string? input = await AsyncConsole.ReadLineAsync(12, stop.Token);
    if (input is null || string.IsNullOrWhiteSpace(input))
        return;

    if (double.TryParse(input.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
    {
        apply(value);
        SetNote($"Manuell: {prompt.TrimEnd(':', ' ')} = {value.ToString("0.##", CultureInfo.InvariantCulture)}");
    }
    else
    {
        SetNote($"Ungültige Eingabe: '{input}'");
    }
}

void Render()
{
    var local = ToLocal(last.SimTimeUtc);
    string manualState = manual.IsActive
        ? FormattableString.Invariant($"MANUELL (Wolken {manual.CloudCoverPercent?.ToString("0", CultureInfo.InvariantCulture) ?? "–"} %, Temp {manual.TemperatureC?.ToString("0.0", CultureInfo.InvariantCulture) ?? "–"} °C, Regen {manual.PrecipitationMmPerHour?.ToString("0.0", CultureInfo.InvariantCulture) ?? "–"} mm/h, Wind {manual.WindSpeedMs?.ToString("0.0", CultureInfo.InvariantCulture) ?? "–"} m/s)")
        : "AUTOMATISCH (deterministisch)";

    var lines = new List<string>
    {
        "",
        $"  WETTERSIMULATOR   http://127.0.0.1:{port}/weather",
        $"  Modus:            {manualState}",
        "",
        "  Letzte Abfrage:",
        last.SimTimeUtc is null
            ? "    (noch keine)"
            : FormattableString.Invariant($"    {last.Location,-10}  {last.SimTimeUtc:yyyy-MM-dd HH:mm} UTC  /  {local:HH:mm} Ortszeit"),
        last.SimTimeUtc is null
            ? ""
            : FormattableString.Invariant($"    Strahlung {last.IrradiationWm2,6:0.0} W/m²   Sonnenhöhe {last.SunHeightDegrees,5:0.0}°   Bewölkung {last.CloudCoverPercent,3:0} %"),
        last.SimTimeUtc is null
            ? ""
            : FormattableString.Invariant($"    Temperatur {last.TemperatureC,5:0.0} °C   Niederschlag {last.PrecipitationMmPerHour,4:0.0} mm/h   Wind {last.WindSpeedMs,4:0.0} m/s   {last.Condition}"),
        $"  Abfragen:          {last.Count}",
        "",
        "  1 sonnig · 2 bewölkt · 3 dicht · 4 Gewitter · S Schnee · R Regen · T Temp · W Wind · X Automatik · Q Ende",
        (DateTime.UtcNow - notificationAt).TotalSeconds < 5 ? $"  » {notification}" : ""
    };

    Console.Write("\x1b[H\x1b[?25l");
    foreach (string line in lines)
        Console.WriteLine(line.PadRight(Console.WindowWidth - 1)[..Math.Max(1, Console.WindowWidth - 1)]);
    Console.Write("\x1b[?25h");
}

string StatusLine()
{
    string state = manual.IsActive ? "MANUELL" : "AUTOMATISCH";
    string time = last.SimTimeUtc is { } t ? FormattableString.Invariant($"{t:yyyy-MM-dd HH:mm} UTC") : "--";
    return FormattableString.Invariant($"{time}  {state}  {last.Location,-10}  {last.Condition,-13}  Strahlung {last.IrradiationWm2:F1} W/m²  {last.TemperatureC:F1} °C  {last.PrecipitationMmPerHour:F1} mm/h  Wind {last.WindSpeedMs:F1} m/s  | Abfragen {last.Count}  {notification}");
}

DateTime ToLocal(DateTime? utc)
{
    if (utc is not { } value)
        return default;

    try
    {
        return TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.FindSystemTimeZoneById("Europe/Vienna"));
    }
    catch (TimeZoneNotFoundException)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"));
    }
}

static bool TryParseSimTime(string? text, out DateTime utc)
{
    utc = default;
    if (string.IsNullOrWhiteSpace(text))
        return false;

    if (!DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsed))
        return false;

    utc = DateTime.SpecifyKind(parsed, DateTimeKind.Utc);
    return true;
}

/// <summary>Zuletzt beantwortete Abfrage für die Konsolenanzeige.</summary>
internal sealed class LastRequest
{
    private readonly object _gate = new();
    public string Location { get; private set; } = "–";
    public DateTime? SimTimeUtc { get; private set; }
    public string Condition { get; private set; } = "–";
    public double IrradiationWm2 { get; private set; }
    public double SunHeightDegrees { get; private set; }
    public double CloudCoverPercent { get; private set; }
    public double TemperatureC { get; private set; }
    public double PrecipitationMmPerHour { get; private set; }
    public double WindSpeedMs { get; private set; }
    public int Count { get; private set; }

    public void Update(WeatherData data)
    {
        lock (_gate)
        {
            Location = data.Location;
            SimTimeUtc = data.SimTimeUtc;
            Condition = data.Condition.ToString();
            IrradiationWm2 = data.IrradiationWm2;
            SunHeightDegrees = data.SunHeightDegrees;
            CloudCoverPercent = data.CloudCoverPercent;
            TemperatureC = data.TemperatureC;
            PrecipitationMmPerHour = data.PrecipitationMmPerHour;
            WindSpeedMs = data.WindSpeedMs;
            Count++;
        }
    }
}

static partial class Program
{
    public const string Usage = """
        WeatherSimulator — deterministisches Wetter für das verteilte Simulationssystem

        Aufruf: dotnet run --project src/WeatherSimulator -- [Optionen]

          --port <nummer>   HTTP-Port (Standard: 8080)
          -h, --help        Diese Hilfe

        Schnittstelle:
          GET /weather?location=<kennung>&simTime=<yyyy-MM-ddTHH:mm:ss>&run=<lauf>
              → WeatherData (JSON), 404 bei unbekannter Kennung, 400 bei ungültiger Zeit
          GET /locations
              → unterstützte Standorte

        Standorte: salzburg (Standard), wien, innsbruck
        Alle Zeiten sind UTC; die Anzeige zeigt zusätzlich die Ortszeit.

        Tasten: 1 sonnig · 2 bewölkt · 3 dicht bewölkt · 4 Gewitter · S Schnee · R Regen
                T Temperatur eingeben · W Wind eingeben · X zurück auf Automatik · Q Ende
        """;
}
