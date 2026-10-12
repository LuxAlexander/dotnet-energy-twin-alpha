namespace DualChargerSim.Models;

/// <summary>
/// Betriebszustände eines Ladepunkts.
/// </summary>
public enum ChargingPointState
{
    Available,
    Connected,
    Charging,
    Finished,
    Faulted
}
