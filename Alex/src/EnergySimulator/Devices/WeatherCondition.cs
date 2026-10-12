namespace EnergySimulator.Devices;

public enum WeatherCondition
{
    Sunny,
    PartlyCloudy,
    Overcast,
    Storm,
    Snow,
    Night
}

public static class WeatherConditionExtensions
{
    // Switchausdruck (switch expression): liefert direkt einen Wert zurueck,
    // im Gegensatz zum Switch-Statement von Java muss nach jedem Arm der
    // Pfeil `=>` stehen und das Ergebnis ist der Rueckgabewert der ganzen
    // Zeile. `_` ist der Default-Fall.
    public static string ToGerman(this WeatherCondition weather) => weather switch
    {
        WeatherCondition.Sunny => "SONNIG",
        WeatherCondition.PartlyCloudy => "BEWÖLKT",
        WeatherCondition.Overcast => "DICHT BEWÖLKT",
        WeatherCondition.Storm => "GEWITTER",
        WeatherCondition.Snow => "SCHNEE",
        WeatherCondition.Night => "NACHT",
        _ => "UNBEKANNT"
    };

    public static bool IsDayWeather(this WeatherCondition weather) =>
        weather != WeatherCondition.Night;
}