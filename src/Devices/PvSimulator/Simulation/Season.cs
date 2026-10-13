namespace EnergySimulator.Simulation;

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter
}

public static class SeasonInfo
{
    // Meteorologische Einteilung (ganze Monate ab Monatsanfang),
    // nicht astronomisch an den Tagundnachtgleichen.
    public static Season FromMonth(int month) => month switch
    {
        3 or 4 or 5 => Season.Spring,
        6 or 7 or 8 => Season.Summer,
        9 or 10 or 11 => Season.Autumn,
        _ => Season.Winter
    };

    // Erweiterungsmethoden: ueber `this Season` wird der erste Parameter zum
    // Empfaenger – man schreibt `season.ToGerman()` statt `SeasonInfo.ToGerman(season)`.
    // In anderen Sprachen gibt es meist keine direkt aufrufbare Instanzfunktion
    // fuer fremde Typen.
    public static string ToGerman(this Season season) => season switch
    {
        Season.Spring => "FRÜHLING",
        Season.Summer => "SOMMER",
        Season.Autumn => "HERBST",
        Season.Winter => "WINTER",
        _ => "?"
    };

    // Ertragsschwankung relativ zur Sommer-Mitte (1.00 = voller Einstrahlungs-
    // Faktor), grob den Verhaeltnissen im Flaechenmodul angepasst. Der Sommer
    // traegt trotz langer Tage die groesste Energiebilanz.
    public static double IrradiationFactor(this Season season) => season switch
    {
        Season.Spring => 0.75,
        Season.Summer => 1.00,
        Season.Autumn => 0.60,
        Season.Winter => 0.40,
        _ => 1.00
    };

    // Vereinfachte Tageslaengen als feste Zeiten pro Jahreszeit. Sommer: Sonnenauf-
    // gang ~4:30, Sonnenuntergang ~20:00 (langer Tag); Winter ~8:00 bis ~16:00.
    public static double SunriseHour(this Season season) => season switch
    {
        Season.Spring => 6.0,
        Season.Summer => 4.5,
        Season.Autumn => 6.5,
        Season.Winter => 8.0,
        _ => 6.0
    };

    public static double SunsetHour(this Season season) => season switch
    {
        Season.Spring => 18.0,
        Season.Summer => 20.0,
        Season.Autumn => 17.5,
        Season.Winter => 16.0,
        _ => 18.0
    };

    //Basic environmental Light Levels
    public static double BaseAmbientC(this Season season) => season switch
    {
        Season.Spring => 11.0,
        Season.Summer => 21.0,
        Season.Autumn => 10.0,
        Season.Winter => 2.0,
        _ => 12.0
    };

    public static double SnowProbability(this Season season) => season switch
    {
        Season.Winter => 0.40,
        Season.Autumn => 0.08,
        Season.Spring => 0.03,
        Season.Summer => 0.00,
        _ => 0.00
    };
}