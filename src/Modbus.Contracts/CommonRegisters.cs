namespace Modbus.Contracts;

public enum DeviceType : ushort
{
    PvPlant = 1,
    Battery = 2,
    Charger = 3,
    Wind = 4,
    IndustrialConsumer = 5
}

/// <summary>Betriebszustand im gemeinsamen Registerkopf.</summary>
public enum OperatingState : ushort
{
    Off = 0,
    Standby = 1,
    Running = 2,
    Fault = 3
}

/// <summary>Zeitstatus des Geräts im gemeinsamen Registerkopf.</summary>
public enum TimeStatus : ushort
{
    NoTime = 0,
    Synced = 1,
    Standalone = 2,
    MasterLost = 3
}

/// <summary>Ergebnis des letzten Schreibzugriffs (Register <see cref="CommonRegisters.LastWriteError"/>).</summary>
public enum WriteError : ushort
{
    None = 0,

    /// <summary>Adresse nicht beschreibbar (nur lesbar oder nicht belegt).</summary>
    NotWritable = 1,

    /// <summary>Wert außerhalb des gültigen Bereichs oder falscher Typ.</summary>
    InvalidValue = 2,

    /// <summary>Mehrteiliger Wert nur zum Teil geschrieben (Float32/UInt32 immer mit einem Schreibzugriff auf beide Register).</summary>
    IncompleteValue = 3
}

/// <summary>
/// Gemeinsamer Registerkopf 0–19 aller Geräte (Holding Registers) und der Bereich 20–49 für gerätespezifische Werte.
/// Vorzeichen der Leistung: positiv = das Gerät nimmt Leistung auf (Verbraucher, Laden), negativ = es gibt Leistung ab.
/// </summary>
public static class CommonRegisters
{
    public const int DeviceType = 0;
    public const int DeviceInstance = 1;
    public const int OperatingState = 2;
    public const int ErrorCode = 3;
    public const int ActivePower = 4;
    public const int Energy = 6;
    public const int ModelTime = 8;
    public const int RunId = 10;
    public const int Sequence = 11;
    public const int TimeStatus = 12;
    public const int SimulationRunning = 13;
    public const int Speed = 14;
    public const int PowerSwitch = 16;
    public const int ResetCommand = 17;
    public const int LastWriteError = 18;
    public const int LastWriteAddress = 19;

    public const int SpecificStart = 20;
    public const int SpecificEnd = 50;

    /// <summary>Anzahl der Register, die ein Gerät bereitstellt (Kopf + gerätespezifisch).</summary>
    public const int TotalRegisters = SpecificEnd;

    /// <summary>Kopfregister, das bei <paramref name="address"/> beginnt.</summary>
    public static RegisterDefinition HeaderAt(int address) => Header.First(d => d.Address == address);

    public static IReadOnlyList<RegisterDefinition> Header { get; } =
    [
        new(DeviceType, "Gerätetyp", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "1 PV, 2 Batterie, 3 Charger, 4 Wind, 5 Industrieverbraucher",
            Labels: ["unbekannt", "PV-Anlage", "Batteriespeicher", "Charger", "Windkraftanlage", "Industrieverbraucher"]),
        new(DeviceInstance, "Geräte-Nummer", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "Zahl am Ende der Geräte-ID (DC-02 → 2), 0 wenn keine"),
        new(OperatingState, "Betriebszustand", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "0 Aus, 1 Bereit/Leerlauf, 2 Betrieb, 3 Störung",
            Labels: ["Aus", "Bereit", "Betrieb", "Störung"]),
        new(ErrorCode, "Fehlercode", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "0 = kein Fehler, sonst gerätespezifisch (siehe Registerplan des Geräts)"),
        new(ActivePower, "Wirkleistung", RegisterType.Float32, RegisterAccess.ReadOnly, "kW",
            "Aktuelle Wirkleistung; positiv = Aufnahme, negativ = Abgabe"),
        new(Energy, "Kumulierte Energie", RegisterType.Float32, RegisterAccess.ReadOnly, "kWh",
            "Energiezähler seit Programmstart (Bedeutung je Gerät im Registerplan)"),
        new(ModelTime, "Modellzeit", RegisterType.UInt32, RegisterAccess.ReadOnly, "s",
            "Virtuelle Zeit des dargestellten Zustands, Sekunden seit 1970-01-01 00:00 (ohne Zeitzone); 0 = noch keine Zeit"),
        new(RunId, "Lauf", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "Lauf-Nummer; ändert sich bei jedem Neustart im Master (Modellzeit springt dann)"),
        new(Sequence, "Sequenznummer", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "Zählt bei jeder neuen Modellzeit hoch (Überlauf bei 65535); gleiche Nummer = gleicher Zustand"),
        new(TimeStatus, "Zeitstatus", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "0 keine Zeit, 1 synchron mit Master, 2 Standalone, 3 Master verloren (Zeit steht)",
            Labels: ["keine Zeit", "synchron mit Master", "Standalone", "Master verloren (Zeit steht)"]),
        new(SimulationRunning, "Simulation läuft", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "1 = virtuelle Zeit läuft, 0 = pausiert oder keine Zeit; bei Pause zeigen die Register den letzten Zustand",
            Labels: ["pausiert / keine Zeit", "läuft"]),
        new(Speed, "Tempo", RegisterType.Float32, RegisterAccess.ReadOnly, "x",
            "Simulationsgeschwindigkeit gegenüber Echtzeit"),
        new(PowerSwitch, "Ein/Aus", RegisterType.UInt16, RegisterAccess.ReadWrite, "",
            "Lesen: 1 = Gerät ein. Schreiben: 0 aus, 1 ein", Min: 0, Max: 1,
            Labels: ["aus", "ein"]),
        new(ResetCommand, "Fehler zurücksetzen", RegisterType.UInt16, RegisterAccess.ReadWrite, "",
            "Schreiben von 1 setzt Störungen zurück, sofern das Gerät es zulässt; liest immer 0", Min: 1, Max: 1),
        new(LastWriteError, "Letzter Schreibfehler", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "0 = letzter Schreibzugriff angenommen, 1 nicht beschreibbar, 2 ungültiger Wert, 3 Wert unvollständig geschrieben",
            Labels: ["angenommen", "nicht beschreibbar", "ungültiger Wert", "Wert unvollständig geschrieben"]),
        new(LastWriteAddress, "Adresse des letzten Schreibzugriffs", RegisterType.UInt16, RegisterAccess.ReadOnly, "",
            "Erste Adresse des letzten Schreibzugriffs (bei Fehler: die fehlerhafte Adresse)"),
    ];
}
