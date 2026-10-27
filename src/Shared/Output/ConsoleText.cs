namespace Shared.Output;

/// <summary>Kleine Konsolen-Hilfen, die in mehreren Geräte-Renderern gleich gebraucht werden.</summary>
public static class ConsoleText
{
    /// <summary>Kürzt zu lange Zeilen ab und füllt zu kurze mit Leerzeichen auf (feste Breite).</summary>
    public static string Fit(string line, int width) =>
        line.Length > width ? line[..width] : line.PadRight(width);
}
