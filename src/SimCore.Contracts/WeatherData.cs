namespace SimCore.Contracts;

/// <summary>Vereinfachter Wetterzustand, den der Wettersimulator liefert und die Geräte anzeigen.</summary>
public enum WeatherCondition
{
    Sunny,
    PartlyCloudy,
    Overcast,
    Storm,
    Snow,
    Night
}

/// <summary>
/// Antwort des Wettersimulators auf eine Anfrage für (Standort, virtuelle Zeit, Lauf).
/// Gleicher Standort + gleiche virtuelle Zeit + gleicher Lauf ergeben stets denselben Wert.
/// Alle Zeiten sind UTC (siehe ADR-0012 im lokalen Arbeitsordner).
/// </summary>
/// <param name="Location">Angefragte Standortkennung (z. B. <c>salzburg</c>).</param>
/// <param name="SimTimeUtc">Virtuelle Zeit, für die das Wetter gilt (UTC, auf die Stunde abgerundet).</param>
/// <param name="RunId">Simulationslauf; Teil des Seeds.</param>
/// <param name="SunHeightDegrees">Sonnenhöhe über dem Horizont in Grad (0 = Horizont, negativ = Nacht).</param>
/// <param name="CloudCoverPercent">Bedeckungsgrad in Prozent (0 = wolkenlos, 100 = bedeckt).</param>
/// <param name="TemperatureC">Lufttemperatur in °C.</param>
/// <param name="PrecipitationMmPerHour">Niederschlag in mm je Stunde (0 = trocken).</param>
/// <param name="WindSpeedMs">Windgeschwindigkeit in m/s.</param>
/// <param name="IrradiationWm2">Globalstrahlung in W/m² (0 = Nacht).</param>
/// <param name="Condition">Vereinfachter Wetterzustand, abgeleitet aus den Größen oben.</param>
public sealed record WeatherData(
    string Location,
    DateTime SimTimeUtc,
    int RunId,
    double SunHeightDegrees,
    double CloudCoverPercent,
    double TemperatureC,
    double PrecipitationMmPerHour,
    double WindSpeedMs,
    double IrradiationWm2,
    WeatherCondition Condition);
