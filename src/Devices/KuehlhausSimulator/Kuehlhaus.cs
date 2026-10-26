namespace KuehlhausSimulator;

/// <summary>
/// Physikalisches Modell eines Kühlhauses (Kühlzelle mit einem Kompressor). Kennt weder Konsole noch Uhr
/// noch Netzwerk: die Außentemperatur kommt als Parameter, die Zeit als Schrittweite.
/// <para>
/// Wärmebilanz: <c>C · dT/dt = k · (T_aus − T) + Q_Tür − Q_Kühl</c> mit Wärmekapazität <c>C</c> (kWh/K),
/// Wärmedurchgang <c>k</c> (kW/K), Türverlust <c>Q_Tür</c> und Kälteleistung <c>Q_Kühl</c> (kW).
/// Der Schritt wird exakt gelöst, nicht mit Euler:
/// <c>T_neu = T_∞ + (T_alt − T_∞) · e^(−Δt/τ)</c> mit <c>T_∞ = T_aus + (Q_Tür − Q_Kühl) / k</c> und <c>τ = C / k</c>.
/// Damit bleibt das Modell auch bei großen Schrittweiten stabil.
/// </para>
/// </summary>
public class Kuehlhaus
{
    /// <summary>Kleinste zulässige Solltemperatur in °C; muss mit <c>KuehlhausRegisters.MinSetpointC</c> übereinstimmen.</summary>
    public const double MinSetpointC = -30;

    /// <summary>Größte zulässige Solltemperatur in °C; muss mit <c>KuehlhausRegisters.MaxSetpointC</c> übereinstimmen.</summary>
    public const double MaxSetpointC = 0;

    private readonly KuehlhausConfig _config;
    private bool _overTempLatched;
    private bool _compressorFailure;
    private bool _isOn = true;
    private double _setpointC;
    private double _runtimeHours;

    public Kuehlhaus(KuehlhausConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        if (config.WaermekapazitaetKWhProK <= 0)
            throw new ArgumentException("Die Wärmekapazität muss größer als 0 sein.", nameof(config));
        if (config.WaermedurchgangKWProK <= 0)
            throw new ArgumentException("Der Wärmedurchgang muss größer als 0 sein.", nameof(config));
        if (config.KaelteleistungKW < 0)
            throw new ArgumentException("Die Kälteleistung darf nicht negativ sein.", nameof(config));
        if (config.COP <= 0)
            throw new ArgumentException("Der COP muss größer als 0 sein.", nameof(config));

        _config = config;
        SetpointC = config.SollTempC;
        RoomTempC = config.SollTempC;
        State = DeviceState.Idle;
    }

    /// <summary>Geräte-ID aus der Konfiguration (z. B. <c>KH-01</c>).</summary>
    public string Id => _config.DeviceId;

    public bool IsOn => _isOn;

    /// <summary>Raumtemperatur in °C.</summary>
    public double RoomTempC { get; private set; }

    /// <summary>Außentemperatur des letzten Schritts in °C (vom Wettersimulator oder aus dem lokalen Tagesprofil).</summary>
    public double OutsideTempC { get; private set; }

    /// <summary>
    /// Solltemperatur in °C. Werte außerhalb <see cref="MinSetpointC"/> … <see cref="MaxSetpointC"/> werden abgelehnt,
    /// damit das Modell auch ohne Modbus-Prüfung konsistent bleibt.
    /// </summary>
    public double SetpointC
    {
        get => _setpointC;
        set
        {
            if (!double.IsFinite(value) || value < MinSetpointC || value > MaxSetpointC)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    $"Die Solltemperatur muss zwischen {MinSetpointC} und {MaxSetpointC} °C liegen.");
            _setpointC = value;
        }
    }

    /// <summary>Aufgenommene elektrische Leistung in kW (positiv = Verbraucher).</summary>
    public double ElectricPowerKW { get; private set; }

    /// <summary>Aufgenommene elektrische Energie seit Programmstart in kWh.</summary>
    public double EnergyKWh { get; private set; }

    /// <summary>Kompressorlaufzeit seit Programmstart in Stunden.</summary>
    public double RuntimeHours => _runtimeHours;

    public bool CompressorOn { get; private set; }

    /// <summary>Thermische Kälteleistung in kW, die der Kompressor gerade erbringt (0, wenn er aus ist).</summary>
    public double CoolingPowerKW => CompressorOn ? _config.KaelteleistungKW : 0;

    public bool DoorOpen { get; private set; }

    /// <summary>Gespeicherte Störung; Übertemperatur hat Vorrang vor dem Kompressorausfall.</summary>
    public FaultType ActiveFault
    {
        get
        {
            if (_overTempLatched) return FaultType.OverTemperature;
            if (_compressorFailure) return FaultType.CompressorFailure;
            return FaultType.None;
        }
    }

    public DeviceState State { get; private set; }

    /// <summary>Raumtemperatur, ab der der Übertemperatur-Alarm gespeichert wird (in °C).</summary>
    public double AlarmTempC => _config.AlarmTempC;

    /// <summary>Raumtemperatur, unter der der Übertemperatur-Alarm quittiert werden darf (in °C).</summary>
    public double ResetAlarmBelowC => _config.AlarmTempC - _config.AlarmHystereseK;

    /// <summary>LB01-Signatur (Sekunden); ruft <see cref="Update(TimeSpan, double)"/> auf.</summary>
    public void Update(double dtSeconds, double outsideTempC) =>
        Update(TimeSpan.FromSeconds(dtSeconds), outsideTempC);

    /// <summary>Ein Modellschritt der Länge <paramref name="dt"/> mit der Außentemperatur <paramref name="outsideTempC"/>.</summary>
    public void Update(TimeSpan dt, double outsideTempC)
    {
        OutsideTempC = outsideTempC;
        UpdateCompressor();

        double k = _config.WaermedurchgangKWProK;               // kW/K
        double c = _config.WaermekapazitaetKWhProK;             // kWh/K
        double doorKW = DoorOpen ? _config.ZusatzwaermeKW : 0;
        double coolingKW = CompressorOn ? _config.KaelteleistungKW : 0;

        // Gleichgewichtstemperatur und Zeitkonstante; die Exponentialfunktion ersetzt den Euler-Schritt.
        double targetC = outsideTempC + (doorKW - coolingKW) / k;
        double tauHours = c / k;
        RoomTempC = targetC + (RoomTempC - targetC) * Math.Exp(-dt.TotalHours / tauHours);

        ElectricPowerKW = CompressorOn ? _config.KaelteleistungKW / GetCop(outsideTempC) : 0;
        EnergyKWh += ElectricPowerKW * dt.TotalHours;
        if (CompressorOn)
            _runtimeHours += dt.TotalHours;

        if (RoomTempC > _config.AlarmTempC)
            _overTempLatched = true;

        UpdateState();
    }

    /// <summary>Öffnet oder schließt die Tür (Ursache des zusätzlichen Wärmeeintrags).</summary>
    public void SetDoor(bool open) => DoorOpen = open;

    /// <summary>
    /// Quittiert die Störung. Der Kompressorausfall wird immer aufgehoben, damit die Kühlung wieder anlaufen kann;
    /// der Übertemperatur-Alarm nur, wenn der Raum unter <c>AlarmTempC − AlarmHystereseK</c> abgekühlt ist
    /// (sonst sperrt er sich selbst aus, weil ohne Kompressor nichts mehr kühlt).
    /// </summary>
    /// <returns><c>true</c>, wenn mindestens eine Störung zurückgesetzt wurde.</returns>
    public bool ResetFault()
    {
        bool cleared = _compressorFailure;
        _compressorFailure = false;

        if (_overTempLatched && RoomTempC <= _config.AlarmTempC - _config.AlarmHystereseK)
        {
            _overTempLatched = false;
            cleared = true;
        }

        return cleared;
    }

    /// <summary>Löst eine Störung aus; <see cref="FaultType.OverTemperature"/> ist nur über die Temperatur erreichbar.</summary>
    public void TriggerFault(FaultType fault)
    {
        if (fault == FaultType.CompressorFailure)
            _compressorFailure = true;

        UpdateState();
    }

    public void TurnOff()
    {
        _isOn = false;
        UpdateCompressor();
        ElectricPowerKW = 0;                        // ausgeschaltete Anlage nimmt sofort keine Leistung mehr auf
        UpdateState();
    }

    public void TurnOn()
    {
        _isOn = true;
        UpdateState();
    }

    /// <summary>Hysterese-Schaltung des Kompressors; bei ausgeschalteter Anlage oder Ausfall bleibt er aus.</summary>
    private void UpdateCompressor()
    {
        if (!_isOn || _compressorFailure)
        {
            CompressorOn = false;
            return;
        }

        if (!CompressorOn && RoomTempC >= SetpointC + _config.HystereseK)
            CompressorOn = true;
        else if (CompressorOn && RoomTempC <= SetpointC - _config.HystereseK)
            CompressorOn = false;
    }

    private void UpdateState()
    {
        if (!_isOn)
            State = DeviceState.Off;
        else if (ActiveFault != FaultType.None)
            State = DeviceState.Fault;
        else if (CompressorOn)
            State = DeviceState.Cooling;
        else
            State = DeviceState.Idle;
    }

    /// <summary>Leistungszahl: 1,0 bei 20 °C, je Grad wärmer 0,05 weniger, nie unter 1,0.</summary>
    private double GetCop(double outsideTempC) =>
        Math.Max(1.0, _config.COP - 0.05 * (outsideTempC - 20));
}
