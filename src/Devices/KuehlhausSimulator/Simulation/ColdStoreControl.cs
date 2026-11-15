using Devices.Common.Control;

namespace KuehlhausSimulator.Simulation;

public sealed record ColdStoreControlState(bool IsOn, bool IsAuto, bool DoorOpen, double SetpointC, FaultType Fault);

public sealed partial class SimulationEngine
{
    public const string ControlKind = "ColdStore";

    private SimulationControl<EquipmentState<ColdStoreControlState>> CreateControl()
    {
        var startup = new EquipmentStartup(Config.DeviceId, Config.CoolingPowerKW, Config.SetpointC, Config.StepMinutes,
            Config.DurationHours, Config.Seed, Path.GetFullPath(Config.OutputDirectory), Config.TimeSync, Config.Modbus, Config.Weather);
        return new(() => new(startup, !Time.IsRunning, Time.Clock.Speed,
            new(_room.IsOn, IsAutoMode, _room.DoorOpen, _room.SetpointC, _room.ActiveFault)), ApplyControl);
    }

    private string? ApplyControl(DeviceCommand command)
    {
        switch (command.Action)
        {
            case "stop": Quit(); return null;
            case "pause" or "speed": return SimulationControl.ApplyTimeCommand(Time, command);
            case "mode" when bool.TryParse(command.Value, out var auto):
                if (IsAutoMode != auto) ToggleMode();
                return null;
            case "power" when bool.TryParse(command.Value, out var enabled):
                if (_room.IsOn != enabled)
                {
                    if (enabled) _room.TurnOn();
                    else _room.TurnOff();
                }
                return null;
            case "door" when bool.TryParse(command.Value, out var open):
                if (IsAutoMode) ToggleMode();
                _room.SetDoor(open);
                return null;
            case "setpoint" when SimulationControl.TryNumber(command.Value, out var setpoint):
                var accepted = TrySetSetpoint(setpoint, out var message);
                Message = message;
                return accepted ? null : message;
            case "fault":
                if (_room.ActiveFault != FaultType.CompressorFailure) _room.TriggerFault(FaultType.CompressorFailure);
                return null;
            case "reset":
                return _room.ActiveFault == FaultType.None || _room.ResetFault()
                    ? null : $"Reset erst unter {_room.ResetAlarmBelowC:0.0} °C möglich.";
            default:
                return "Die Aktion oder der angegebene Wert ist im aktuellen Betrieb nicht verfügbar.";
        }
    }
}
