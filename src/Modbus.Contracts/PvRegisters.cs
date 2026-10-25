namespace Modbus.Contracts;

/// <summary>
/// Gerätespezifische Register der PV-Anlage (Adressen 20–38) und ihr <see cref="RegisterPlan"/>.
/// Die Wirkleistung im Kopf ist negativ, solange die Anlage einspeist (Abgabe).
/// </summary>
public static class PvRegisters
{
    public const int PowerLimit = 20;            // Float32, schreibbar (Wirkleistungsbegrenzung in % der Nennleistung)
    public const int Irradiation = 22;           // Float32
    public const int ModuleTemperature = 24;     // Float32
    public const int AmbientTemperature = 26;    // Float32
    public const int GridVoltage = 28;           // Float32
    public const int DailyEnergy = 30;           // Float32
    public const int Soiling = 32;               // Float32
    public const int Aging = 34;                 // Float32
    public const int Weather = 36;               // UInt16
    public const int PeakPower = 37;             // Float32 (37–38)

    public const double MinPowerLimitPercent = 0;
    public const double MaxPowerLimitPercent = 100;

    public static RegisterPlan Plan { get; } = new(DeviceType.PvPlant, "PvSimulator",
    [
        new(PowerLimit, "Leistungsbegrenzung", RegisterType.Float32, RegisterAccess.ReadWrite, "%",
            "Sollwert: begrenzt die Einspeisung auf diesen Anteil der Nennleistung (Einspeisemanagement). Außerhalb 0–100 % wird " +
            "der Schreibzugriff abgelehnt (Letzter Schreibfehler = 2), der alte Wert bleibt",
            Min: MinPowerLimitPercent, Max: MaxPowerLimitPercent),
        new(Irradiation, "Einstrahlung", RegisterType.Float32, RegisterAccess.ReadOnly, "W/m²", "Aktuelle Einstrahlung auf die Module"),
        new(ModuleTemperature, "Modultemperatur", RegisterType.Float32, RegisterAccess.ReadOnly, "°C",
            "Überhitzung (Fehlercode 2) ab 70 °C, Freigabe unter 55 °C"),
        new(AmbientTemperature, "Außentemperatur", RegisterType.Float32, RegisterAccess.ReadOnly, "°C", "Umgebungstemperatur der Anlage"),
        new(GridVoltage, "Netzspannung", RegisterType.Float32, RegisterAccess.ReadOnly, "V",
            "Netzspannung am Anschlusspunkt; außerhalb 200–250 V Störung (Fehlercode 3)"),
        new(DailyEnergy, "Tagesenergie", RegisterType.Float32, RegisterAccess.ReadOnly, "kWh", "Erzeugte Energie seit 00:00 Modellzeit"),
        new(Soiling, "Verschmutzung", RegisterType.Float32, RegisterAccess.ReadOnly, "%", "Ertragsverlust durch Verschmutzung (max. 25 %)"),
        new(Aging, "Alterung", RegisterType.Float32, RegisterAccess.ReadOnly, "%", "Ertragsverlust durch Alterung (max. 15 %)"),
        new(Weather, "Wetter", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "0 sonnig, 1 bewölkt, 2 dicht bewölkt, 3 Gewitter, 4 Schnee, 5 Nacht",
            Labels: ["sonnig", "bewölkt", "dicht bewölkt", "Gewitter", "Schnee", "Nacht"]),
        new(PeakPower, "Nennleistung", RegisterType.Float32, RegisterAccess.ReadOnly, "kW", "Konfigurierte Nennleistung (--peak)"),
    ]);
}
