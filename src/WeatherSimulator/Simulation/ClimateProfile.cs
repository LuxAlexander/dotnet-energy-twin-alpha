namespace WeatherSimulator.Simulation;

/// <summary>Regionale Klimakenngrößen. Quellen und ausdrücklich synthetische Parameter: Wetter-README.</summary>
public sealed record ClimateProfile(
    double[] MonthlyTemperatureC, double[] MonthlyPrecipitationMm,
    double DiurnalAmplitudeC, double MeanWindMs, double MeanCloudPercent, double DailyVariationC)
{
    // JMA ClimatView: Normalwerte der Station 60680, siehe README.
    public static ClimateProfile Sahara { get; } = new(
        [12.8, 15.3, 19.2, 23.8, 27.7, 30.0, 30.1, 29.1, 27.9, 23.8, 18.2, 14.4],
        [1.1, 0.9, 5.2, 0.1, 1.5, 5.7, 7.5, 18.5, 6.5, 8.2, 0.4, 0.6],
        7.0, 4.0, 20.0, 3.0);

    // Gerundete saisonale Näherung an BAS READER; Niederschlag ist Wasseräquivalent, kein Schneehöhenzuwachs.
    public static ClimateProfile Antarctic { get; } = new(
        [-4.2, -8.0, -13.0, -17.5, -20.5, -23.5, -25.0, -25.0, -23.0, -17.5, -10.0, -4.8],
        [25, 25, 30, 35, 35, 35, 35, 35, 35, 30, 25, 25],
        1.5, 8.7, 70.0, 6.0);

    /// <summary>Linear zwischen Monatsmitten interpolieren, auch über Jahresgrenzen.</summary>
    public static double Interpolate(double[] values, DateTime time)
    {
        var middle = new DateTime(time.Year, time.Month, 15, 0, 0, 0, DateTimeKind.Utc);
        int current = time.Month - 1;
        if (time < middle)
        {
            int previous = (current + 11) % 12;
            int year = time.Month == 1 ? Math.Max(1, time.Year - 1) : time.Year;
            double fraction = 1 + (time - middle).TotalDays / DateTime.DaysInMonth(year, previous + 1);
            return values[previous] * (1 - fraction) + values[current] * fraction;
        }
        double nextFraction = (time - middle).TotalDays / DateTime.DaysInMonth(time.Year, time.Month);
        return values[current] * (1 - nextFraction) + values[(current + 1) % 12] * nextFraction;
    }
}
