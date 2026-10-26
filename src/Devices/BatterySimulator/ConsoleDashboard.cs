using System.Globalization;

namespace Batterie_Sim;

/// <summary>Anzeige des Batteriespeichers; überschreibt die Zeilen ab Position (0,0), ohne die Konsole zu löschen.</summary>
public sealed class ConsoleDashboard
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

    private int _lineCount;

    public static bool IsInteractive => !Console.IsOutputRedirected;

    /// <summary>Zeile, unter der Eingaben (Einstellungsmenü) erscheinen dürfen.</summary>
    public int BelowStatus => Math.Min(_lineCount + 1, Console.BufferHeight - 1);

    public void Initialize()
    {
        if (!IsInteractive)
            return;
        Console.CursorVisible = false;
        Console.Clear();
    }

    public void Shutdown()
    {
        if (!IsInteractive)
            return;
        Console.SetCursorPosition(0, BelowStatus);
        Console.CursorVisible = true;
    }

    public void Render(Simulator sim, Batterie battery)
    {
        string bar = BuildBar(battery.SocPercent);
        string simTime = sim.SimulationTime?.ToString("ddd yyyy-MM-dd HH:mm:ss", Culture) ?? "– (noch keine Zeit vom Master)";
        string running = sim.Time.IsRunning ? "" : "   ZEIT STEHT";
        string speed = sim.Time.HasTime ? $"x{sim.Time.Clock.Speed.ToString("0.##", Culture)}" : "–";

        string[] lines =
        [
            $" === Batteriespeicher {battery.DeviceId} ===============",
            $" Simulationszeit : {simTime}   Lauf {sim.RunId}   Tempo: {speed}   Schritt: {sim.StepSeconds.ToString("0.##", Culture)} s{running}",
            $" Zeitquelle      : {sim.Time.StatusText}",
            $" Modbus          : {sim.ModbusStatus ?? "aus (--no-modbus)"}",
            $" Wetter          : {sim.WeatherStatus ?? "aus (feste 20 °C)"}",
            $" Modus           : {sim.Mode}   Zustand: {battery.State}   {(battery.IsOn ? "EIN" : "AUS")}",
            $" Sollwert        : {battery.SetpointKW.ToString("F2", Culture)} kW",
            $" Ist-Leistung    : {battery.ActualKW.ToString("F2", Culture)} kW",
            $" SOC             : {bar} {battery.SocPercent.ToString("F2", Culture)} %",
            $" Temperatur      : {battery.TemperaturC.ToString("F2", Culture)} °C   Störung: {battery.ActiveFault}",
            $" Energie         : geladen {battery.ChargedKWh.ToString("F2", Culture)} entladen {battery.DischargeKWh.ToString("F2", Culture)} kWh   Zyklen {battery.EquivalentCycles.ToString("F2", Culture)}",
            $" Meldung         : {sim.Message}",
            " --------------------------------------------",
            " [Leertaste] Pause (standalone)  [M] Modus  [↑/↓] Sollwert ±0,5  [0] Sollwert 0  [E] Ein/Aus  [S] Einstellungen",
            sim.Time.IsStandalone
                ? " [N] Not-Aus  [H] Überhitzung  [R] Störung zurücksetzen  [</>] Tempo ÷2/×2  [Q] Ende"
                : " [N] Not-Aus  [H] Überhitzung  [R] Störung zurücksetzen  [Q] Ende   (Pause und Tempo steuert der Master)"
        ];

        int width = Math.Max(Console.WindowWidth - 1, 40);
        try
        {
            Console.SetCursorPosition(0, 0);
        }
        catch (Exception ex) when (ex is IOException or ArgumentOutOfRangeException)
        {
            return;                                               // Fenster wird gerade verändert
        }

        var sb = new System.Text.StringBuilder();
        foreach (string line in lines)
            sb.AppendLine(Fit(line, width));
        for (int i = lines.Length; i < _lineCount; i++)
            sb.AppendLine(new string(' ', width));

        Console.Write(sb.ToString());
        _lineCount = lines.Length;
    }

    /// <summary>Kurzfassung für umgeleitete Ausgabe (Protokoll statt Bildschirm).</summary>
    public static string StatusLine(Simulator sim, Batterie battery)
    {
        string time = sim.SimulationTime?.ToString("yyyy-MM-dd HH:mm", Culture) ?? "--";
        return FormattableString.Invariant($"{time}  Lauf {sim.RunId}  {battery.ActualKW:F2} kW  SOC {battery.SocPercent:F1} %  {battery.State}  | {sim.Time.StatusText}  | Modbus {sim.ModbusStatus ?? "aus"}  | Wetter {sim.WeatherStatus ?? "aus"}  | {sim.Message}");
    }

    private static string BuildBar(double percent, int barWidth = 20)
    {
        int filled = Math.Clamp((int)(percent / 100.0 * barWidth), 0, barWidth);
        return new string('#', filled) + new string('-', barWidth - filled);
    }

    private static string Fit(string line, int width) => line.Length > width ? line[..width] : line.PadRight(width);
}
