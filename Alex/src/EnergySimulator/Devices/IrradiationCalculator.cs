using EnergySimulator.Simulation;

namespace EnergySimulator.Devices;

public static class IrradiationCalculator
{
    // Referenzbedingung der PV-Module (STC: 1000 W/m², 25 °C) – daran wird
    // der Ertrag normiert (z. B. 600 W/m² → 60 % der Nennleistung).
    // https://de.wikipedia.org/wiki/Standard-Testbedingungen_(Photovoltaik)
    public const double PeakIrradiationWm2 = 1000.0;

    public static bool IsDaylight(double hour, Season season) =>
        hour >= season.SunriseHour() && hour <= season.SunsetHour();

    public static double Calculate(double hour, WeatherCondition weather, Season season)
    {
        if (weather == WeatherCondition.Night)
            return 0.0;

        if (!IsDaylight(hour, season))
            return 0.0;

        double sunrise = season.SunriseHour();
        double sunset = season.SunsetHour();
        double normalized = (hour - sunrise) / (sunset - sunrise);

        // Idealisierte Tageskurve als sinusfoermige Haelfte: 0 W/m² bei Sonnenauf-
        // und -untergang (sin(0)=sin(pi)=0), Maximum exakt um den Sonnenhoehepunkt
        // (sin(pi/2)=1). Einfaches Doma-Modell ohne Wolkenposition.
        // https://www.co2online.de/modernisieren-und-bauen/photovoltaik/pv-ertrag/
        double baseIrradiation = PeakIrradiationWm2 * Math.Sin(normalized * Math.PI);

        return Math.Round(baseIrradiation * GetWeatherFactor(weather) * season.IrradiationFactor(), 1);
    }

    // Wolken-Abschwaechung: Sonne voll, Bewoelkt ~60 %, Dicht 30 %,
    // Gewitter 10 % (starke Bewoelkung), Schnee 15 % (hohe Reflexion).
    //https://www.solaranlage-ratgeber.de/photovoltaik/photovoltaik-voraussetzungen/pv-ertraege-und-wetter
    //https://www.enbw.com/blog/energiewende/solarenergie/was-bringt-eine-photovoltaik-anlage-im-winter/
    private static double GetWeatherFactor(WeatherCondition weather) => weather switch
    {
        WeatherCondition.Sunny => 1.00,
        WeatherCondition.PartlyCloudy => 0.60,
        WeatherCondition.Overcast => 0.30,
        WeatherCondition.Storm => 0.10,
        WeatherCondition.Snow => 0.15,
        WeatherCondition.Night => 0.00,
        _ => 1.00
    };
}