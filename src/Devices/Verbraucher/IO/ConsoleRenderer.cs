using System.Globalization;
using System.Text;
using Devices.Common;
using Verbraucher.Simulation;

namespace Verbraucher.IO;

/// <summary>
/// Flackerfreie Statusausgabe des Kühlhauses. Die Zeilen werden ab (0,0) überschrieben, statt mit
/// <see cref="Console.Clear"/> neu zu zeichnen. Bei umgeleiteter Ausgabe (Protokoll, Tests) bleibt eine
/// einzelne Statuszeile je Takt, damit die Datei lesbar bleibt.
/// </summary>
public class ConsoleRenderer
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");
    private const int BarWidth = 10;
    private const int RedirectedWidth = 190;

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
        var lines = IsInteractive ? BuildLines(engine) : BuildStatusLines(engine);
        int width = Width;

        var sb = new StringBuilder();
        foreach (string line in lines)
            sb.AppendLine(IsInteractive ? Fit(line, width) : Cut(line, width));

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

    /// <summary>Kurzfassung für umgeleitete Ausgabe (Protokoll statt Bildschirm): Datenzeile und Statuszeile.</summary>
    private static List<string> BuildStatusLines(SimulationEngine engine)
    {
        var room = engine.Room;
        string simTime = engine.SimulationTime?.ToString("yyyy-MM-dd HH:mm", Culture) ?? "--";
        string compressor = room.CompressorOn ? "Kälte" : "aus";
        string door = room.DoorOpen ? "offen" : "zu";
        string modbus = engine.ModbusStatus ?? "aus";

        return
        [
            F($"{simTime}  Lauf {engine.RunId}  Raum {room.RoomTempC:N2} °C  außen {room.OutsideTempC:N2} °C  Soll {room.SetpointC:N1} °C  {compressor,6} {room.ElectricPowerKW,5:N2} kW  {room.EnergyKWh:N2} kWh  Tür {door}"),
            $"  Zeit: {engine.Time.StatusText}  | Modbus: {modbus}  | Meldung: {engine.Message}",
        ];
    }

    /// <summary>
    /// Die Zeilen der Bildschirmanzeige. Öffentlich, damit Tests die Ausgabe ohne echtes Terminal prüfen können
    /// (Formatfehler würden sonst erst im interaktiven Betrieb auffallen).
    /// </summary>
    public static IReadOnlyList<string> BuildLines(SimulationEngine engine)
    {
        var room = engine.Room;
        var config = engine.Config;
        var time = engine.Time;
        string mode = engine.IsAutoMode ? "AUTOMATIK" : "HANDBETRIEB";
        string pause = time.IsRunning ? "" : " | ZEIT STEHT";
        string simTime = engine.SimulationTime?.ToString("ddd yyyy-MM-dd HH:mm", Culture) ?? "– (noch keine Zeit vom Master)";
        string end = engine.EndTime is { } endTime ? F($"   Ende: {endTime:yyyy-MM-dd HH:mm}") : "";
        double untilAlarm = room.AlarmTempC - room.SetpointC;
        double ratio = untilAlarm > 0 ? (room.RoomTempC - room.SetpointC) / untilAlarm : 0;
        string plant = room.IsOn ? "EIN" : "AUS";
        string compressor = room.CompressorOn ? "EIN" : "aus";
        string ratioPercent = ((Math.Clamp(ratio, 0, 1) * 100).ToString("N0", Culture));
        string bar = Bar(ratio);

        return
        [
            $"=== Kühlhaus {room.Id} | {mode}{pause} ===",
            F($"Simulationszeit: {simTime}   Lauf: {engine.RunId}   Schritt: {config.StepMinutes:0.##} min{end}"),
            "Zeitquelle:      " + time.StatusText,
            "Modbus:          " + (engine.ModbusStatus ?? "aus (--no-modbus)"),
            "Wetter:          " + engine.WeatherStatus,
            F($"Außentemperatur: {room.OutsideTempC:N1} °C ({engine.OutsideSource})"),
            "",
            F($"Anlage:     {plant,-4} Soll: {room.SetpointC,6:N1} °C   Raum: {room.RoomTempC,6:N1} °C [{bar}] {ratioPercent,3} % bis Alarm ({room.AlarmTempC:N0} °C)"),
            F($"Kompressor: {compressor,-4} Kälte: {room.CoolingPowerKW,5:N2} kW  Leistung: {room.ElectricPowerKW,5:N2} kW   Energie: {room.EnergyKWh:N2} kWh   Laufzeit: {room.RuntimeHours:N2} h"),
            F($"Tür:        {DescribeDoor(engine)}"),
            "",
            "Meldung: " + engine.Message,
            "",
            "Tasten: T Tür | S Solltemperatur | F Kompressorausfall | R Reset | O Anlage ein/aus | A Automatik/Handbetrieb",
            "        Q Beenden"
                + (time.IsStandalone ? " | P Pause | </> Tempo ÷2/×2 (lokale Uhr)" : " | Pause, Tempo und Neustart steuert der Master"),
        ];
    }

    private static string DescribeDoor(SimulationEngine engine)
    {
        if (engine.Room.DoorOpen)
            return engine.IsAutoMode && engine.DoorClosesAt is { } closes
                ? F($"offen (Automatik schließt {closes:HH:mm})")
                : "offen (T schließt)";

        return engine.IsAutoMode
            ? "zu (Automatik öffnet nach Tageszeit)"
            : "zu (Handbetrieb)";
    }

    private static string Bar(double fraction)
    {
        int filled = (int)Math.Round(Math.Clamp(fraction, 0, 1) * BarWidth);
        return new string('█', filled) + new string('░', BarWidth - filled);
    }

    private static string Fit(string line, int width) =>
        line.Length > width ? line[..width] : line.PadRight(width);

    /// <summary>Nur kürzen, nicht auffüllen – für Protokolldateien (umgeleitete Ausgabe).</summary>
    private static string Cut(string line, int width) =>
        line.Length > width ? line[..width] : line;

    private static string F(FormattableString text) => text.ToString(Culture);
}
