namespace Verbraucher;

/// <summary>
/// Parameter des Kühlhaus-Modells. Die Einheit steht im Namen: <c>…C</c> Grad Celsius, <c>…K</c> Kelvin,
/// <c>…KW</c> Kilowatt, <c>…KWhProK</c> Kilowattstunden je Kelvin Wärmekapazität.
/// </summary>
public class KuehlhausConfig
{
    /// <summary>Geräte-ID, erscheint im Registerkopf als Geräte-Nummer und in der Master-Liste.</summary>
    public string DeviceId { get; set; } = "KH-01";

    /// <summary>Solltemperatur des Kühlraums in °C; gültig sind −30 … 0 °C.</summary>
    public double SollTempC { get; set; } = -20.0;

    /// <summary>Schalthysterese des Kompressors um die Solltemperatur in K (ein bei Soll + H, aus bei Soll − H).</summary>
    public double HystereseK { get; set; } = 2;

    /// <summary>Wärmekapazität von Raum, Ware und Bauwerk in kWh/K (bestimmt zusammen mit k die Zeitkonstante).</summary>
    public double WaermekapazitaetKWhProK { get; set; } = 1.0;

    /// <summary>Wärmedurchgang der Hülle in kW/K; jeder Kelvin Differenz zur Außenluft bringt so viel Wärme ein.</summary>
    public double WaermedurchgangKWProK { get; set; } = 0.05;

    /// <summary>Thermische Kälteleistung des Kompressors in kW (elektrisch = Kälteleistung / COP).</summary>
    public double KaelteleistungKW { get; set; } = 6;

    /// <summary>Zusätzlicher Wärmeeintrag bei offener Tür in kW.</summary>
    public double ZusatzwaermeKW { get; set; } = 3.0;

    /// <summary>Raumtemperatur in °C, ab der der Übertemperatur-Alarm gespeichert wird.</summary>
    public double AlarmTempC { get; set; } = -10.0;

    /// <summary>Hysterese in K unterhalb des Alarms, ab der er zurückgesetzt werden darf.</summary>
    public double AlarmHystereseK { get; set; } = 2;

    /// <summary>Leistungszahl des Kompressors bei 20 °C Außentemperatur.</summary>
    public double COP { get; set; } = 2.0;
}
