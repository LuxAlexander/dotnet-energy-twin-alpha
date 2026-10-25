namespace Modbus.Contracts;

/// <summary>Alle bekannten Registerpläne nach Gerätetyp; der Diagnose-Client wählt damit den Plan anhand von Register 0.</summary>
public static class RegisterPlans
{
    /// <summary>Plan des Gerätetyps; <c>null</c>, solange für den Typ (Wind) noch kein Plan eingetragen ist.</summary>
    public static RegisterPlan? For(DeviceType type) => type switch
    {
        DeviceType.Charger => ChargerRegisters.Plan,
        DeviceType.PvPlant => PvRegisters.Plan,
        DeviceType.Battery => BatteryRegisters.Plan,
        _ => null
    };
}
