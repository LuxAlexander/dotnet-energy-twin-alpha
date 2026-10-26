using Devices.Common;
using Devices.Common.Modbus;
using Modbus.Contracts;
using static Modbus.Contracts.KuehlhausRegisters;

namespace KuehlhausSimulator.Simulation;

/// <summary>
/// Modbus-Anbindung des Kühlhauses: Registerplan, Zustand veröffentlichen, Schreibzugriffe auf das Modell anwenden.
/// Alle Methoden außer dem Modbus-Server selbst laufen im Simulationsthread, das Modell bleibt also einthreadig.
/// </summary>
public sealed class KuehlhausModbus : IDisposable
{
    private readonly Kuehlhaus _room;
    private readonly DeviceTimeSource _time;
    private readonly Action<string> _report;
    private readonly ModbusDeviceServer _server;

    public KuehlhausModbus(Kuehlhaus room, DeviceTimeSource time, ModbusOptions options, Action<string> report)
    {
        _room = room;
        _time = time;
        _report = report;
        _server = new ModbusDeviceServer(options, KuehlhausRegisters.Plan, room.Id);
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
                case Setpoint:
                    try
                    {
                        _room.SetpointC = command.Value;
                        _report($"Modbus: Solltemperatur auf {command.Value:0.#} °C gesetzt.");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Sollte nicht vorkommen: Registerplan und Modell prüfen dieselben Grenzen.
                        _report($"Modbus: Solltemperatur {command.Value:0.#} °C vom Modell abgelehnt.");
                    }

                    break;

                case Door:
                    _room.SetDoor(command.Value >= 1);
                    _report($"Modbus: Tür {(command.Value >= 1 ? "geöffnet" : "geschlossen")}.");
                    break;

                case CommonRegisters.PowerSwitch:
                    if (command.Value >= 1)
                        _room.TurnOn();
                    else
                        _room.TurnOff();
                    _report($"Modbus: Anlage {(_room.IsOn ? "eingeschaltet" : "ausgeschaltet")}.");
                    break;

                case CommonRegisters.ResetCommand:
                    _report(_room.ResetFault()
                        ? "Modbus: Störung zurückgesetzt."
                        : $"Modbus: Reset ohne Wirkung – Raum {_room.RoomTempC:0.0} °C " +
                          $"(Alarm erst unter {_room.ResetAlarmBelowC:0.0} °C quittierbar).");
                    break;
            }
        }
    }

    /// <summary>Veröffentlicht den aktuellen Zustand; auch in der Pause aufrufen, damit Zeitstatus und Sollwerte stimmen.</summary>
    public void Publish(DateTime? modelTime, int runId)
    {
        ushort errorCode = _room.ActiveFault switch
        {
            FaultType.CompressorFailure => 1,
            FaultType.OverTemperature => 2,
            _ => 0
        };

        var state = !_room.IsOn ? OperatingState.Off
            : errorCode != 0 ? OperatingState.Fault
            : _room.CompressorOn ? OperatingState.Running
            : OperatingState.Standby;

        var header = new HeaderValues(state, errorCode, _room.ElectricPowerKW, _room.EnergyKWh, _room.IsOn, modelTime, runId);

        _server.Publish(header, _time, writer =>
        {
            writer.SetFloat(Setpoint, _room.SetpointC);
            writer.SetUInt16(Door, _room.DoorOpen ? (ushort)1 : (ushort)0);
            writer.SetFloat(RoomTemperature, _room.RoomTempC);
            writer.SetFloat(OutsideTemperature, _room.OutsideTempC);
            writer.SetUInt16(Compressor, _room.CompressorOn ? (ushort)1 : (ushort)0);
            writer.SetFloat(CoolingPower, _room.CoolingPowerKW);
            writer.SetFloat(Runtime, _room.RuntimeHours);
        });
    }

    public void Dispose() => _server.Dispose();
}
