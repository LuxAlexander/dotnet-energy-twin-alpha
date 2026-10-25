namespace Modbus.Contracts;

/// <summary>
/// Gerätespezifische Register der Dual-Port-Ladesäule (Adressen 20–37) und ihr <see cref="RegisterPlan"/>.
/// Liegt im Vertrag, damit Säule (Server) und Diagnose-Client dieselbe Definition benutzen.
/// </summary>
public static class ChargerRegisters
{
    public const int PowerLimit = 20;            // Float32, schreibbar
    public const int PointBase = 22;             // je Ladepunkt 7 Register
    public const int PointStride = 7;
    public const int PointState = 0;             // UInt16  (Offset im Ladepunkt)
    public const int PointPower = 1;             // Float32
    public const int PointSoc = 3;               // Float32
    public const int PointTemperature = 5;       // Float32
    public const int QueueLength = 36;           // UInt16
    public const int RejectedVehicles = 37;      // UInt16

    /// <summary>Grenzen der Leistungsgrenze; müssen mit <c>DualCharger.MinPowerLimitKW/MaxPowerLimitKW</c> übereinstimmen.</summary>
    public const double MinPowerLimitKW = 50;
    public const double MaxPowerLimitKW = 350;

    public static RegisterPlan Plan { get; } = CreatePlan();

    private static RegisterPlan CreatePlan()
    {
        var specific = new List<RegisterDefinition>
        {
            new(PowerLimit, "Leistungsgrenze der Säule", RegisterType.Float32, RegisterAccess.ReadWrite, "kW",
                "Gemeinsames Leistungsbudget beider Ladepunkte (Sollwert). Außerhalb 50–350 kW wird der Schreibzugriff abgelehnt " +
                "(Letzter Schreibfehler = 2), der alte Wert bleibt",
                Min: MinPowerLimitKW, Max: MaxPowerLimitKW),
        };

        for (int number = 1; number <= 2; number++)
        {
            int b = PointBase + PointStride * (number - 1);
            specific.Add(new(b + PointState, $"Punkt {number}: Zustand", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
                "0 frei, 1 angesteckt, 2 lädt, 3 fertig, 4 Störung",
                Labels: ["frei", "angesteckt", "lädt", "fertig", "Störung"]));
            specific.Add(new(b + PointPower, $"Punkt {number}: Leistung", RegisterType.Float32, RegisterAccess.ReadOnly, "kW",
                "Mittlere Netzleistung des Punkts im letzten Schritt"));
            specific.Add(new(b + PointSoc, $"Punkt {number}: SOC", RegisterType.Float32, RegisterAccess.ReadOnly, "%",
                "Ladezustand des Fahrzeugs; 0 wenn kein Fahrzeug angesteckt"));
            specific.Add(new(b + PointTemperature, $"Punkt {number}: Temperatur", RegisterType.Float32, RegisterAccess.ReadOnly, "°C",
                "Temperatur von Kabel und Stecker; Derating ab 60 °C, Störung ab 80 °C"));
        }

        specific.Add(new(QueueLength, "Warteschlange", RegisterType.UInt16, RegisterAccess.ReadOnly, "Fahrzeuge",
            "Wartende Fahrzeuge (max. 5 min)"));
        specific.Add(new(RejectedVehicles, "Abgewiesene Fahrzeuge", RegisterType.UInt16, RegisterAccess.ReadOnly, "Fahrzeuge",
            "Seit Programmstart weggefahren, weil die Wartezeit abgelaufen ist (Sättigung bei 65535)"));

        return new RegisterPlan(DeviceType.Charger, "DualPortCharger", specific);
    }
}
