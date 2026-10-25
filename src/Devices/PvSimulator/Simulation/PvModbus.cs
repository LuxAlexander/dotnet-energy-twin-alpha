using Devices.Common;
using Devices.Common.Modbus;
using EnergySimulator.Devices;
using Modbus.Contracts;
using static Modbus.Contracts.PvRegisters;

namespace EnergySimulator.Simulation;

/// <summary>
/// Modbus-Anbindung der PV-Anlage: Zustand veröffentlichen, Schreibzugriffe (Leistungsbegrenzung, Ein/Aus, Reset) anwenden.
/// Läuft im Simulationsthread; nur der Server selbst arbeitet in Bibliotheksthreads.
/// </summary>
public sealed class PvModbus : IDisposable
{
    private readonly PvSystem _pv;
    private readonly PvFaultModel _faults;
    private readonly DeviceTimeSource _time;
    private readonly Action<string> _report;
    private readonly ModbusDeviceServer _server;

    public PvModbus(PvSystem pv, PvFaultModel faults, DeviceTimeSource time, ModbusOptions options, string deviceId, Action<string> report)
    {
        _pv = pv;
        _faults = faults;
        _time = time;
        _report = report;
        _server = new ModbusDeviceServer(options, PvRegisters.Plan, deviceId);
        _server.Log += (_, message) => _report(message);
    }

    public string StatusText => _server.StatusText;

    /// <exception cref="IOException">Port belegt.</exception>
    public void Start()
    {
        _server.Start();
        Publish(null, 0);
    }

    public void ProcessCommands()
    {
        while (_server.TryDequeueCommand(out var command))
        {
            switch (command!.Definition.Address)
            {
                case PowerLimit:
                    _pv.PowerLimitPercent = command.Value;
                    _report($"Modbus: Leistungsbegrenzung auf {command.Value:0.#} % gesetzt.");
                    break;

                case CommonRegisters.PowerSwitch:
                    _pv.IsEnabled = command.Value >= 1;
                    _report($"Modbus: Anlage {(_pv.IsEnabled ? "eingeschaltet" : "ausgeschaltet")}.");
                    break;

                case CommonRegisters.ResetCommand:
                    bool hadFault = _faults.InverterFault || _faults.OverheatForced || _faults.GridDisturbanceForced;
                    _faults.ResetFaults();
                    _report(hadFault ? "Modbus: Störungen zurückgesetzt." : "Modbus: Reset ohne Wirkung – keine manuelle Störung aktiv.");
                    break;
            }
        }
    }

    /// <summary>Veröffentlicht den zuletzt berechneten Zustand; auch in der Pause aufrufen.</summary>
    public void Publish(DateTime? modelTime, int runId)
    {
        var state = !_pv.IsEnabled ? OperatingState.Off
            : _pv.HasFault ? OperatingState.Fault
            : _pv.PowerKw > 0 ? OperatingState.Running
            : OperatingState.Standby;

        // Erzeuger: Wirkleistung negativ (Abgabe), Energie = erzeugte Energie
        var header = new HeaderValues(state, (ushort)_pv.ActiveFault, 0.0 - _pv.PowerKw, _pv.TotalEnergyKwh, _pv.IsEnabled, modelTime, runId);

        _server.Publish(header, _time, w =>
        {
            w.SetFloat(PowerLimit, _pv.PowerLimitPercent);
            w.SetFloat(Irradiation, _pv.IrradiationWm2);
            w.SetFloat(ModuleTemperature, _pv.ModuleTemperatureC);
            w.SetFloat(AmbientTemperature, _pv.AmbientTemperatureC);
            w.SetFloat(GridVoltage, _pv.GridVoltageV);
            w.SetFloat(DailyEnergy, _pv.DailyEnergyKwh);
            w.SetFloat(Soiling, _pv.SoilingLossPercent);
            w.SetFloat(Aging, _pv.AgingLossPercent);
            w.SetUInt16(Weather, (int)_pv.Weather);
            w.SetFloat(PeakPower, _pv.PeakPowerKw);
        });
    }

    public void Dispose() => _server.Dispose();
}
