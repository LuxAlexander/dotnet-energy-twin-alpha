using System.Globalization;
using System.Threading.Channels;
using Devices.Common.Control;

namespace Batterie_Sim;

public sealed record BatteryControlState(SimulatorOptions Startup, bool Paused, double Speed, bool IsOn,
    ControlMode Mode, FaultType Fault, double SetpointKw, double MaxChargeKw, double MaxDischargeKw,
    double SocMin, double SocMax)
{
    public const string Kind = "Battery";
}

public sealed partial class Simulator
{
    private sealed record ControlRequest(DeviceCommand? Command, CancellationToken CancellationToken,
        TaskCompletionSource<DeviceReply<BatteryControlState>> Completion);

    private readonly SimulatorOptions _startup;
    private readonly Channel<ControlRequest> _controlRequests = Channel.CreateBounded<ControlRequest>(32);

    private static BatterieConfig CopyConfig(BatterieConfig config) => new()
    {
        DeviceId = config.DeviceId,
        CapacityKWh = config.CapacityKWh,
        MaxChargeKW = config.MaxChargeKW,
        MaxDischargeKW = config.MaxDischargeKW,
        SocMinPercent = config.SocMinPercent,
        SocMaxPercent = config.SocMaxPercent,
        StartSocPercent = config.StartSocPercent,
        Efficiency = config.Efficiency
    };

    private async Task<DeviceReply<BatteryControlState>> DispatchAsync(DeviceCommand? command, CancellationToken cancellationToken)
    {
        var completion = new TaskCompletionSource<DeviceReply<BatteryControlState>>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(3));
        await _controlRequests.Writer.WriteAsync(new(command, timeout.Token, completion), timeout.Token);
        return await completion.Task.WaitAsync(timeout.Token);
    }

    private void ProcessControlRequests()
    {
        while (!QuitRequested && _controlRequests.Reader.TryRead(out var request))
        {
            if (request.CancellationToken.IsCancellationRequested) continue;
            var error = request.Command is { } command ? ApplyControl(command) : null;
            var config = _battery.Config;
            request.Completion.TrySetResult(new(new BatteryControlState(_startup, !Time.IsRunning, Time.Clock.Speed,
                _battery.IsOn, Mode, _battery.ActiveFault, _battery.SetpointKW,
                config.MaxChargeKW, config.MaxDischargeKW, config.SocMinPercent, config.SocMaxPercent), error));
        }
    }

    private string? ApplyControl(DeviceCommand command)
    {
        var config = _battery.Config;
        switch (command.Action)
        {
            case "stop": QuitRequested = true; break;
            case "pause" when Time.IsStandalone && bool.TryParse(command.Value, out var paused):
                if (Time.IsRunning == paused) Message = Time.TogglePause();
                break;
            case "speed" when Time.IsStandalone && TryNumber(command.Value, out var speed) && speed is >= 1 and <= 100000:
                Message = Time.SetSpeed(speed);
                break;
            case "power" when bool.TryParse(command.Value, out var enabled):
                SetPowerEnabled(enabled);
                break;
            case "mode" when command.Value is "Auto" or "Manual":
                SetMode(command.Value == "Auto" ? ControlMode.Auto : ControlMode.Manual);
                break;
            case "fault" when command.Value is "Overheat" or "EmergencyStop":
                if (_battery.ActiveFault.ToString() != command.Value)
                    _battery.TriggerFault(command.Value == "Overheat" ? FaultType.Overheat : FaultType.EmergencyStop);
                break;
            case "reset": _battery.ResetFault(); break;
            case "setpoint" when TryNumber(command.Value, out var setpoint)
                && setpoint >= -config.MaxDischargeKW && setpoint <= config.MaxChargeKW:
                SetMode(ControlMode.Manual);
                _battery.SetPower(setpoint);
                break;
            case "max-charge" when TryNumber(command.Value, out var charge) && charge > 0:
                config.MaxChargeKW = charge;
                break;
            case "max-discharge" when TryNumber(command.Value, out var discharge) && discharge > 0:
                config.MaxDischargeKW = discharge;
                break;
            case "soc-min" when TryNumber(command.Value, out var min) && min >= 0 && min < config.SocMaxPercent:
                config.SocMinPercent = min;
                break;
            case "soc-max" when TryNumber(command.Value, out var max) && max <= 100 && max > config.SocMinPercent:
                config.SocMaxPercent = max;
                break;
            default:
                return "Diese Aktion oder der angegebene Wert ist im aktuellen Betrieb nicht verfügbar.";
        }
        return null;
    }

    private static bool TryNumber(string? text, out double value) =>
        double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && double.IsFinite(value);

    private void SetMode(ControlMode mode)
    {
        if (mode == Mode) return;
        Mode = mode;
        if (mode == ControlMode.Manual) _battery.SetPower(0);
        Message = $"Modus {Mode}.";
    }

    private void SetPowerEnabled(bool enabled)
    {
        if (enabled == _battery.IsOn) return;
        if (enabled) _battery.TurnOn();
        else _battery.TurnOff();
    }
}
