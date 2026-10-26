namespace KuehlhausSimulator;

/// <summary>Zustand des Kühlhauses, wie er in der Anzeige und im Registerkopf erscheint.</summary>
public enum DeviceState
{
    Off,
    Idle,
    Cooling,
    Fault
}

/// <summary>Störung des Kühlhauses; der Zahlenwert geht als Fehlercode in den Registerkopf.</summary>
public enum FaultType
{
    None,
    CompressorFailure,
    OverTemperature
}
