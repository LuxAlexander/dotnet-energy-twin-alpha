using System.Globalization;
using System.Text;
using Devices.Common;
using DualChargerSim.Models;
using DualChargerSim.Simulation;

namespace DualChargerSim.IO;

/// <summary>
/// Flackerfreie Statusausgabe beider Ladepunkte in der Konsole.
/// Überschreibt die Zeilen an Position (0,0), statt mit Console.Clear() neu zu zeichnen.
/// </summary>
public class ConsoleRenderer
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");
    private const int BarWidth = 10;
    private const int RedirectedWidth = 140;

    private int _lineCount;

    public static bool IsInteractive => !Console.IsOutputRedirected;

    private static int Width => IsInteractive ? Math.Max(Console.WindowWidth - 1, 40) : RedirectedWidth;

    public async Task InitializeAsync()
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (IsInteractive)
        {
            Console.CursorVisible = false;
            Console.Clear();
        }
        await Console.Out.FlushAsync();
    }

    public async Task ShutdownAsync()
    {
        if (IsInteractive)
        {
            Console.SetCursorPosition(0, BelowStatus());
            Console.CursorVisible = true;
        }
        await Console.Out.FlushAsync();
    }

    public async Task DrawAsync(SimulationEngine engine)
    {
        var lines = IsInteractive ? BuildLines(engine) : [BuildStatusLine(engine)];
        int width = Width;

        var sb = new StringBuilder();
        foreach (string line in lines)
            sb.AppendLine(Fit(line, width));

        // Zeilen, die beim letzten Mal länger waren (z. B. alte Prompt-Reste), leeren
        for (int i = lines.Count; i < _lineCount; i++)
            sb.AppendLine(new string(' ', width));

        if (IsInteractive)
        {
            try
            {
                Console.SetCursorPosition(0, 0);
            }
            catch (Exception ex) when (ex is IOException or ArgumentOutOfRangeException)
            {
                return;                                           // Fenster wird gerade verändert
            }
        }
        await Console.Out.WriteAsync(sb.ToString());
        await Console.Out.FlushAsync();
        _lineCount = IsInteractive ? lines.Count : 0;
    }

    /// <summary>
    /// Eingabe unterhalb der Statusanzeige, ohne blockierendes Warten (Esc = Abbruch, Ergebnis "").
    /// Die Zeitsynchronisation läuft währenddessen weiter, die fälligen Schritte werden danach nachgeholt.
    /// </summary>
    public async Task<string> PromptAsync(string question, CancellationToken cancellationToken, IReadOnlyList<string>? info = null)
    {
        int width = Width;
        int top = IsInteractive ? BelowStatus() : 0;
        int infoCount = info?.Count ?? 0;

        if (IsInteractive)
        {
            Console.SetCursorPosition(0, top);
            Console.CursorVisible = true;
        }

        if (info is not null)
            foreach (string line in info)
                await Console.Out.WriteLineAsync(Fit(line, width));

        await Console.Out.WriteAsync(question + " ");
        string input = await AsyncConsole.ReadLineAsync(40, cancellationToken) ?? "";

        if (IsInteractive)
        {
            Console.SetCursorPosition(0, Math.Min(top, Console.BufferHeight - 1));
            for (int i = 0; i <= infoCount; i++)
                await Console.Out.WriteLineAsync(new string(' ', width));
            Console.CursorVisible = false;
        }

        return input.Trim();
    }

    /// <summary>Eine Zeile unter der Anzeige, aber nie außerhalb des Konsolenpuffers (kleines Fenster).</summary>
    private int BelowStatus() => Math.Min(_lineCount + 1, Console.BufferHeight - 1);

    /// <summary>Kurzfassung für umgeleitete Ausgabe (Protokoll statt Bildschirm).</summary>
    private static string BuildStatusLine(SimulationEngine engine)
    {
        var charger = engine.Charger;
        string simTime = engine.SimulationTime?.ToString("yyyy-MM-dd HH:mm", Culture) ?? "--";
        return F($"{simTime}  Lauf {engine.RunId}  {charger.TotalPowerKW:N1} kW  {charger.TotalEnergyKWh:N1} kWh  | {engine.Time.StatusText}  | {engine.Message}");
    }

    private static List<string> BuildLines(SimulationEngine engine)
    {
        var charger = engine.Charger;
        var config = engine.Config;
        var time = engine.Time;
        string mode = engine.IsAutoMode ? "AUTO" : "MANUELL";
        string pause = time.IsRunning ? "" : " | ZEIT STEHT";
        double loadRatio = charger.PowerLimitKW > 0 ? charger.TotalPowerKW / charger.PowerLimitKW : 0;
        string simTime = engine.SimulationTime?.ToString("ddd yyyy-MM-dd HH:mm", Culture) ?? "– (noch keine Zeit vom Master)";
        string end = engine.EndTime is { } endTime ? F($"   Ende: {endTime:yyyy-MM-dd HH:mm}") : "";

        var lines = new List<string>
        {
            $"=== Dual-Charger {charger.Id} | {mode}{pause} ===",
            F($"Simulationszeit: {simTime}   Lauf: {engine.RunId}   Schritt: {config.StepMinutes:0.##} min{end}"),
            "Zeitquelle:      " + time.StatusText,
            "",
            F($"Säule: {(charger.IsOn ? "EIN" : "AUS")}   Limit: {charger.PowerLimitKW:N0} kW   Abgewiesen: {charger.RejectedVehicles}   Warteschlange: {charger.QueueLength}"),
            F($"Leistung: {charger.TotalPowerKW,7:N1} kW [{Bar(loadRatio)}] {loadRatio * 100,3:N0} %   Energie: {charger.TotalEnergyKWh:N1} kWh"),
            "",
        };

        foreach (var point in charger.Points)
            lines.Add(FormatPoint(point, point == engine.SelectedPoint));

        lines.Add("");
        lines.Add("Meldung: " + engine.Message);
        lines.Add("");
        lines.Add("Tasten: A Auto | N Fahrzeug kommt (Manuell) | 1/2 Punkt wählen | C Anstecken | D Abstecken | S Start/Stop");
        lines.Add("        F Störung | R Reset | +/- Limit ±10 kW | O Säule ein/aus | Q Beenden");
        lines.Add(time.IsStandalone
            ? "        P Pause | </> Tempo ÷2/×2 (lokale Uhr)"
            : "        Pause, Tempo und Neustart steuert der Simulations-Master");
        return lines;
    }

    private static string FormatPoint(ChargingPoint point, bool selected)
    {
        string marker = selected ? ">" : " ";
        string state = point.State.ToString().ToUpperInvariant();
        string derating = point.DeratingFactor < 1 && point.State != ChargingPointState.Faulted ? "  DERATING" : "";

        if (point.Vehicle is not Vehicle v)
            return F($"{marker}[Punkt {point.Number}] {state,-10} {"(frei)",-21} {"",37} {point.PowerKW,6:N1} kW  {point.TemperatureC,3:N0} °C  {point.EnergyKWh,8:N1} kWh{derating}");

        return F($"{marker}[Punkt {point.Number}] {state,-10} {v.ModelName,-21} SOC {v.SocPercent,5:N1} % [{Bar(v.SocPercent / 100)}] Ziel {v.TargetSocPercent,3:N0} %  {point.PowerKW,6:N1} kW  {point.TemperatureC,3:N0} °C  {point.EnergyKWh,8:N1} kWh{derating}");
    }

    private static string Bar(double fraction)
    {
        int filled = (int)Math.Round(Math.Clamp(fraction, 0, 1) * BarWidth);
        return new string('█', filled) + new string('░', BarWidth - filled);
    }

    private static string Fit(string line, int width) =>
        line.Length > width ? line[..width] : line.PadRight(width);

    private static string F(FormattableString text) => text.ToString(Culture);
}
