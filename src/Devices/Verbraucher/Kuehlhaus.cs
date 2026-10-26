using Verbraucher;

public class Kuehlhaus
{
    private readonly KuehlhausConfig _config;
    private bool _overTempLatched = false;
    private bool _compressorFailure = false;
    private bool _ison = true;

    public bool IsOn => _ison;
    public double RoomTempC { get; private set; }
    public double SetpointC { get; private set; }
    public double ElectricPowerKW { get; private set; }
    public double EnergyKWh { get; private set; }
    public bool CompressorOn { get; private set; }
    public bool DoorOpen { get; private set; }

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
    public Kuehlhaus(KuehlhausConfig config)
    {
        _config = config;
        SetpointC = config.SollTempC;
        RoomTempC = config.SollTempC;
        State = DeviceState.Idle;
    }

    public void Update(double dtSeconds, double outsideTempC)
    {
        if (!IsOn|| _compressorFailure)
        {
            CompressorOn = false;
        }
        else if(!CompressorOn && RoomTempC >= SetpointC + _config.HystereseK)
        {
            CompressorOn = true;
        }
        else if (CompressorOn && RoomTempC <= SetpointC - _config.HystereseK)
        {
            CompressorOn = false;
        }


        double dtHours = dtSeconds / 3600;

        double heatGainKW = _config.WaermeduchgangKw * (outsideTempC - RoomTempC)+(DoorOpen ? _config.ZusatzwaermekW : 0);
        double coolingPowerKW = CompressorOn ? _config.KuelleistungKw : 0;
        RoomTempC += (heatGainKW - coolingPowerKW)   / _config.WaermekapazitaetKwH*dtHours;
        double cop = GetCop(outsideTempC);
        ElectricPowerKW = CompressorOn ? _config.KuelleistungKw / cop : 0;
        EnergyKWh += ElectricPowerKW * dtHours;
        if (RoomTempC > _config.AlarmTempC)
        {
            _overTempLatched = true;
        }
        UpdateState();
    }

    public void SetDoor()
    {
        DoorOpen=true;
    }

    private void UpdateState()
    {
        if(!IsOn)
        {
            State = DeviceState.Off;
        }
        else if (ActiveFault != FaultType.None)
        {
            State = DeviceState.Fault;
        }
        else if (CompressorOn)
        {
            State = DeviceState.Cooling;
        }
        else
        {
            State = DeviceState.Idle;
        }
    }

    private double GetCop(double outsideTempC)
    {
        double cop = Math.Max(1.0, _config.COP - 0.05 * (outsideTempC - 20));
        return cop;
    }

    public void TriggerFault(FaultType fault)
    {

        if (fault == FaultType.CompressorFailure)
        {
            _compressorFailure = true;
        }
    }
    
    public void ResetFault()
    {
        _compressorFailure = false;
        if (RoomTempC < _config.AlarmTempC)
        {
            _overTempLatched = false;
        }
    }

    public void TurnOff()
    {
        _ison = false;
       
    }
    public void TurnOn()
    {
        _ison = true;
    }
}