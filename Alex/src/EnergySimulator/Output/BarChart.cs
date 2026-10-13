namespace EnergySimulator.Output;

public static class BarChart
{
    // Rueckgabetyp ist ein C#-Werttupel (ValueTuple): zwei benannte Werte in
    // Klammern, ohne eigene Klasse/Record. Anders als z. B. in Java sind
    // Mehrfach-Rueckgaben damit ohne Hilfstyp moeglich.
    public static (string Filled, string Empty) Split(double value, double max, int width)
    {
        if (width <= 0)
            return (string.Empty, string.Empty);

        if (max <= 0)
            return (string.Empty, new string('░', width));

        // Anteil am Maximum klammern und kaufmaennisch runden, damit der Balken
        // nie laenger/kuerzer als die Rasterbreite wird.
        double ratio = Math.Clamp(value / max, 0.0, 1.0);
        int filled = Math.Clamp((int)Math.Round(ratio * width), 0, width);

        return (new string('█', filled), new string('░', width - filled));
    }
}