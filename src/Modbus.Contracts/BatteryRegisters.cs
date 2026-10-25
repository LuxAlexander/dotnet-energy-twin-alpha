namespace Modbus.Contracts;

/// <summary>
/// Gerätespezifische Register des Batteriespeichers (Adressen 20–43) und ihr <see cref="RegisterPlan"/>.
/// Leistung: positiv = Laden (Aufnahme), negativ = Entladen (Abgabe). Die Energie im Kopf ist die geladene Energie.
/// </summary>
public static class BatteryRegisters
{
    public const int Setpoint = 20;              // Float32, schreibbar
    public const int Mode = 22;           // UInt16, schreibbar
    public const int Soc = 23;                   // Float32
    public const int Temperature = 25;           // Float32
    public const int ChargedEnergy = 27;         // Float32
    public const int DischargedEnergy = 29;      // Float32
    public const int Cycles = 31;                // Float32
    public const int Capacity = 33;              // Float32
    public const int MaxChargePower = 35;        // Float32
    public const int MaxDischargePower = 37;     // Float32
    public const int SocMin = 39;                // Float32
    public const int SocMax = 41;                // Float32
    public const int StateCode = 43;           // UInt16

    /// <summary>Absolute Grenze des Sollwerts im Registerplan; die konfigurierte Maximalleistung begrenzt zusätzlich im Gerät.</summary>
    public const double AbsoluteMaxPowerKW = 1000;

    public static RegisterPlan Plan { get; } = new(DeviceType.Battery, "BatterySimulator",
    [
        new(Setpoint, "Leistungssollwert", RegisterType.Float32, RegisterAccess.ReadWrite, "kW",
            "Sollwert der Leistung: positiv laden, negativ entladen. Gültig von −max. Entladeleistung bis +max. Ladeleistung " +
            "(Register 37 und 35, Standard ±5 kW), sonst abgelehnt (Letzter Schreibfehler = 2). Ein Schreibzugriff schaltet in den Betriebsmodus Manuell",
            Min: -AbsoluteMaxPowerKW, Max: AbsoluteMaxPowerKW),
        new(Mode, "Betriebsmodus", RegisterType.UInt16, RegisterAccess.ReadWrite, "",
            "0 Auto (Tagesprofil bestimmt den Sollwert), 1 Manuell (Sollwert aus Register 20)", Min: 0, Max: 1,
            Labels: ["Auto", "Manuell"]),
        new(Soc, "Ladezustand (SOC)", RegisterType.Float32, RegisterAccess.ReadOnly, "%", "Ladezustand, zwischen SOC-Min und SOC-Max begrenzt"),
        new(Temperature, "Temperatur", RegisterType.Float32, RegisterAccess.ReadOnly, "°C",
            "Derating ab 45 °C, Abschaltung bei 60 °C (Fehlercode 2), Freigabe ≤ 50 °C"),
        new(ChargedEnergy, "Geladene Energie", RegisterType.Float32, RegisterAccess.ReadOnly, "kWh", "Seit Programmstart (entspricht der Energie im Kopf)"),
        new(DischargedEnergy, "Entladene Energie", RegisterType.Float32, RegisterAccess.ReadOnly, "kWh", "Seit Programmstart"),
        new(Cycles, "Vollzyklen", RegisterType.Float32, RegisterAccess.ReadOnly, "", "Äquivalente Vollzyklen"),
        new(Capacity, "Kapazität", RegisterType.Float32, RegisterAccess.ReadOnly, "kWh", "Konfigurierte Kapazität"),
        new(MaxChargePower, "Max. Ladeleistung", RegisterType.Float32, RegisterAccess.ReadOnly, "kW", "Konfiguriert (--max-charge)"),
        new(MaxDischargePower, "Max. Entladeleistung", RegisterType.Float32, RegisterAccess.ReadOnly, "kW", "Konfiguriert (--max-discharge)"),
        new(SocMin, "SOC-Minimum", RegisterType.Float32, RegisterAccess.ReadOnly, "%", "Konfiguriert (--soc-min)"),
        new(SocMax, "SOC-Maximum", RegisterType.Float32, RegisterAccess.ReadOnly, "%", "Konfiguriert (--soc-max)"),
        new(StateCode, "Gerätezustand", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "0 aus, 1 Bereitschaft, 2 lädt, 3 entlädt, 4 Störung, 5 Abkühlung (Fehlercode im Kopf: 0 keiner, 1 Not-Aus, 2 Überhitzung, 3 Abkühlung nach Überhitzung)",
            Labels: ["aus", "Bereitschaft", "lädt", "entlädt", "Störung", "Abkühlung"]),
    ]);
}
