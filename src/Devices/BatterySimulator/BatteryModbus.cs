using Devices.Common;
using Devices.Common.Modbus;
using Modbus.Contracts;
using static Modbus.Contracts.BatteryRegisters;

namespace Batterie_Sim;

/// <summary>
/// Modbus-Anbindung des Batteriespeichers: Zustand veröffentlichen, Schreibzugriffe (Leistungssollwert, Betriebsmodus,
/// Ein/Aus, Reset) anwenden. Läuft im Simulationsthread; nur der Server selbst arbeitet in Bibliotheksthreads.
/// </summary>
public sealed class BatteryModbus : IDisposable
{
    private readonly Batterie _battery;
    private readonly Simulator _simulator;
    private readonly DeviceTimeSource _time;
    private readonly Action<string> _report;
    private readonly ModbusDeviceServer _server;

    public BatteryModbus(Simulator simulator, Batterie battery, DeviceTimeSource time, ModbusOptions options, Action<string> report)
    {
        _simulator = simulator;
        _battery = battery;
        _time = time;
        _report = report;
        _server = new ModbusDeviceServer(options, BatteryRegisters.Plan, battery.DeviceId)
        {
            // Der Sollwert ist durch die konfigurierte Maximalleistung begrenzt (im Registerplan steht nur die absolute Grenze)
            ValueValidator = ValidateValue
        };
        _server.Log += (_, message) => _report(message);
    }

    public string StatusText => _server.StatusText;

    /// <exception cref="IOException">Port belegt.</exception>
    public void Start()
    {
        _server.Start();
        Publish(null, 0);
    }

    private string? ValidateValue(RegisterDefinition definition, double value)
    {
        if (definition.Address != Setpoint)
            return null;

        var config = _battery.Config;
        return value < -config.MaxDischargeKW || value > config.MaxChargeKW
            ? $"außerhalb der konfigurierten Leistung −{config.MaxDischargeKW:0.##} … +{config.MaxChargeKW:0.##} kW"
            : null;
    }

    public void ProcessCommands()
    {
        while (_server.TryDequeueCommand(out var command))
        {
            switch (command!.Definition.Address)
            {
                case Setpoint:
                    _simulator.Mode = ControlMode.Manual;             // der Sollwert gilt nur im manuellen Betrieb
                    _battery.SetPower(command.Value);
                    _report($"Modbus: Sollwert {command.Value:0.##} kW gesetzt, Modus Manuell.");
                    break;

                case Mode:
                    if (command.Value >= 1)
                    {
                        if (_simulator.Mode != ControlMode.Manual)
                            _battery.SetPower(0);
                        _simulator.Mode = ControlMode.Manual;
                    }
                    else
                    {
                        _simulator.Mode = ControlMode.Auto;
                    }
                    _report($"Modbus: Modus {_simulator.Mode}.");
                    break;

                case CommonRegisters.PowerSwitch:
                    if (command.Value >= 1)
                        _battery.TurnOn();
                    else
                        _battery.TurnOff();
                    _report($"Modbus: Speicher {(_battery.IsOn ? "eingeschaltet" : "ausgeschaltet")}.");
                    break;

                case CommonRegisters.ResetCommand:
                    bool hadFault = _battery.ActiveFault != FaultType.None;
                    _battery.ResetFault();
                    _report(hadFault ? "Modbus: Störung zurückgesetzt." : "Modbus: Reset ohne Wirkung – keine Störung aktiv.");
                    break;
            }
        }
    }

    /// <summary>Veröffentlicht den zuletzt berechneten Zustand; auch in der Pause aufrufen.</summary>
    public void Publish(DateTime? modelTime, int runId)
    {
        var state = _battery.State switch
        {
            DeviceState.Off => OperatingState.Off,
            DeviceState.Standby => OperatingState.Standby,
            DeviceState.Charging or DeviceState.Discharging => OperatingState.Running,
            _ => OperatingState.Fault
        };

        // Fehlercode: 0 keiner, 1 Not-Aus, 2 Überhitzung, 3 Abkühlung nach Überhitzung (ohne aktive Störung)
        ushort error = _battery.State == DeviceState.Cooldown ? (ushort)3 : (ushort)_battery.ActiveFault;

        var header = new HeaderValues(state, error, _battery.ActualKW, _battery.ChargedKWh, _battery.IsOn, modelTime, runId);
        var config = _battery.Config;

        _server.Publish(header, _time, w =>
        {
            w.SetFloat(Setpoint, _battery.SetpointKW);
            w.SetUInt16(Mode, _simulator.Mode == ControlMode.Manual ? 1 : 0);
            w.SetFloat(Soc, _battery.SocPercent);
            w.SetFloat(Temperature, _battery.TemperaturC);
            w.SetFloat(ChargedEnergy, _battery.ChargedKWh);
            w.SetFloat(DischargedEnergy, _battery.DischargeKWh);
            w.SetFloat(Cycles, _battery.EquivalentCycles);
            w.SetFloat(Capacity, config.CapacityKWh);
            w.SetFloat(MaxChargePower, config.MaxChargeKW);
            w.SetFloat(MaxDischargePower, config.MaxDischargeKW);
            w.SetFloat(SocMin, config.SocMinPercent);
            w.SetFloat(SocMax, config.SocMaxPercent);
            w.SetUInt16(StateCode, (int)_battery.State);
        });
    }

    public void Dispose() => _server.Dispose();
}
