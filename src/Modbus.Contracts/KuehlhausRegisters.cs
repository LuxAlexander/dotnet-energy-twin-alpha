namespace Modbus.Contracts;

/// <summary>
/// Gerätespezifische Register des Kühlhauses (Adressen 20–31) und ihr <see cref="RegisterPlan"/>.
/// Liegt im Vertrag, damit Gerät (Server) und Diagnose-Client dieselbe Definition benutzen.
/// Leistung: positiv = Aufnahme (Verbraucher). Die Energie im Kopf ist die aufgenommene elektrische Energie.
/// Fehlercode im Kopf: 1 = Kompressorausfall, 2 = Übertemperatur.
/// </summary>
public static class KuehlhausRegisters
{
    public const int Setpoint = 20;              // Float32, schreibbar: Solltemperatur in °C
    public const int Door = 22;                  // UInt16,  schreibbar: 0 zu, 1 offen
    public const int RoomTemperature = 23;       // Float32
    public const int OutsideTemperature = 25;    // Float32
    public const int Compressor = 27;            // UInt16
    public const int CoolingPower = 28;          // Float32
    public const int Runtime = 30;               // Float32

    /// <summary>Grenzen der Solltemperatur; müssen mit <c>Kuehlhaus.MinSetpointC/MaxSetpointC</c> übereinstimmen.</summary>
    public const double MinSetpointC = -30;
    public const double MaxSetpointC = 0;

    public static RegisterPlan Plan { get; } = CreatePlan();

    private static RegisterPlan CreatePlan()
    {
        var specific = new List<RegisterDefinition>
        {
            new(Setpoint, "Solltemperatur", RegisterType.Float32, RegisterAccess.ReadWrite, "°C",
                "Solltemperatur des Kühlraums (Sollwert). Außerhalb −30 … 0 °C wird der Schreibzugriff abgelehnt " +
                "(Letzter Schreibfehler = 2), der alte Wert bleibt",
                Min: MinSetpointC, Max: MaxSetpointC),
            new(Door, "Tür", RegisterType.UInt16, RegisterAccess.ReadWrite, "",
                "0 = geschlossen, 1 = offen; eine offene Tür erhöht den Wärmeeintrag",
                Min: 0, Max: 1, Labels: ["zu", "offen"]),
            new(RoomTemperature, "Raumtemperatur", RegisterType.Float32, RegisterAccess.ReadOnly, "°C",
                "Temperatur im Kühlraum"),
            new(OutsideTemperature, "Außentemperatur", RegisterType.Float32, RegisterAccess.ReadOnly, "°C",
                "Außentemperatur des letzten Schritts (vom Wettersimulator oder aus dem lokalen Tagesprofil)"),
            new(Compressor, "Kompressor", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
                "1 = läuft, 0 = aus (Hysterese um die Solltemperatur)",
                Min: 0, Max: 1, Labels: ["aus", "läuft"]),
            new(CoolingPower, "Kälteleistung", RegisterType.Float32, RegisterAccess.ReadOnly, "kW",
                "Thermische Leistung des Kompressors; die elektrische Leistung steht im Kopf (Kälteleistung / COP)"),
            new(Runtime, "Laufzeit", RegisterType.Float32, RegisterAccess.ReadOnly, "h",
                "Kompressorlaufzeit seit Programmstart"),
        };

        return new RegisterPlan(DeviceType.IndustrialConsumer, "Verbraucher", specific);
    }
}
