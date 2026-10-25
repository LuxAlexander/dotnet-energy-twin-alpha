using Devices.Common;
using Devices.Common.Modbus;
using DualChargerSim.Models;
using Modbus.Contracts;
using static Modbus.Contracts.ChargerRegisters;

namespace DualChargerSim.Simulation;

/// <summary>
/// Modbus-Anbindung der Ladesäule: Registerplan, Zustand veröffentlichen, Schreibzugriffe auf die Säule anwenden.
/// Alle Methoden außer dem Modbus-Server selbst laufen im Simulationsthread, das Modell bleibt also einthreadig.
/// </summary>
public sealed class ChargerModbus : IDisposable
{
    private readonly DualCharger _charger;
    private readonly DeviceTimeSource _time;
    private readonly Action<string> _report;
    private readonly ModbusDeviceServer _server;

    public ChargerModbus(DualCharger charger, DeviceTimeSource time, ModbusOptions options, Action<string> report)
    {
        _charger = charger;
        _time = time;
        _report = report;
        _server = new ModbusDeviceServer(options, ChargerRegisters.Plan, charger.Id);
        _server.Log += (_, message) => _report(message);
    }

    public string StatusText => _server.StatusText;

    /// <exception cref="IOException">Port belegt.</exception>
    public void Start()
    {
        _server.Start();
        Publish(null, 0);
    }

    /// <summary>Setzt die angenommenen Schreibzugriffe um; die Werte hat der Server schon gegen den Registerplan geprüft.</summary>
    public void ProcessCommands()
    {
        while (_server.TryDequeueCommand(out var command))
        {
            switch (command!.Definition.Address)
            {
                case PowerLimit:
                    _charger.PowerLimitKW = command.Value;
                    _report($"Modbus: Leistungsgrenze auf {command.Value:0.#} kW gesetzt.");
                    break;

                case CommonRegisters.PowerSwitch:
                    if (command.Value >= 1)
                        _charger.SwitchOn();
                    else
                        _charger.SwitchOff();
                    _report($"Modbus: Säule {(_charger.IsOn ? "eingeschaltet" : "ausgeschaltet")}.");
                    break;

                case CommonRegisters.ResetCommand:
                    int reset = _charger.Points.Count(p => p.State == ChargingPointState.Faulted && p.Reset());
                    int stillFaulted = _charger.Points.Count(p => p.State == ChargingPointState.Faulted);
                    _report(reset > 0 ? $"Modbus: {reset} Störung(en) zurückgesetzt."
                          : stillFaulted > 0 ? $"Modbus: Reset nicht möglich – Temperatur noch über {ChargingPoint.ManualResetTemperatureC:0} °C."
                          : "Modbus: Reset ohne Wirkung – keine Störung.");
                    break;
            }
        }
    }

    /// <summary>Veröffentlicht den aktuellen Zustand; auch in der Pause aufrufen, damit Zeitstatus und Sollwerte stimmen.</summary>
    public void Publish(DateTime? modelTime, int runId)
    {
        var faulted = _charger.Points.Select(p => p.State == ChargingPointState.Faulted).ToArray();
        ushort errorCode = (ushort)((faulted[0] ? 1 : 0) | (faulted[1] ? 2 : 0));

        var state = !_charger.IsOn ? OperatingState.Off
            : errorCode != 0 ? OperatingState.Fault
            : _charger.TotalPowerKW > 0 ? OperatingState.Running
            : OperatingState.Standby;

        var header = new HeaderValues(state, errorCode, _charger.TotalPowerKW, _charger.TotalEnergyKWh, _charger.IsOn, modelTime, runId);

        _server.Publish(header, _time, writer =>
        {
            writer.SetFloat(PowerLimit, _charger.PowerLimitKW);
            foreach (var point in _charger.Points)
            {
                int baseAddress = PointBase + PointStride * (point.Number - 1);
                writer.SetUInt16(baseAddress + PointState, (ushort)point.State);
                writer.SetFloat(baseAddress + PointPower, point.PowerKW);
                writer.SetFloat(baseAddress + PointSoc, point.Vehicle?.SocPercent ?? 0);
                writer.SetFloat(baseAddress + PointTemperature, point.TemperatureC);
            }
            writer.SetUInt16(QueueLength, _charger.QueueLength);
            writer.SetUInt16(RejectedVehicles, Math.Min(_charger.RejectedVehicles, ushort.MaxValue));
        });
    }

    public void Dispose() => _server.Dispose();
}
