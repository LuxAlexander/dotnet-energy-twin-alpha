namespace EnergySimulator.Devices;

public enum FaultType
{
    None,
    Inverter,
    Overheat,
    GridVoltage
}

public static class FaultTypeExtensions
{
    public static string ToGString(this FaultType fault) => fault switch
    {
        FaultType.None => "keiner",
        FaultType.Inverter => "INVERTER",
        FaultType.Overheat => "ÜBERHITZUNG",
        FaultType.GridVoltage => "NETZSPANNUNG",
        _ => "UNBEKANNT"
    };
}