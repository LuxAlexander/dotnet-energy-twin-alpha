using System.Globalization;
using System.Threading.Channels;
using Devices.Common.Control;
using Shared.Weather;

namespace EnergySimulator.Simulation;

public sealed record PvControlState(SimulationConfig Startup, bool Paused, SimulationMode Mode,
    double Speed, WeatherCondition Weather, int RefreshMs, bool InverterFault, bool Overheat, bool GridFault);

public sealed partial class SimulationEngine
{
    private sealed record ControlRequest(DeviceCommand? Command, CancellationToken CancellationToken,
        TaskCompletionSource<DeviceReply<PvControlState>> Completion);

    private readonly Channel<ControlRequest> _controlRequests = Channel.CreateBounded<ControlRequest>(32);

    private async Task<DeviceReply<PvControlState>> DispatchAsync(DeviceCommand? command, CancellationToken cancellationToken)
    {
        var completion = new TaskCompletionSource<DeviceReply<PvControlState>>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(3));
        await _controlRequests.Writer.WriteAsync(new(command, timeout.Token, completion), timeout.Token);
        return await completion.Task.WaitAsync(timeout.Token);
    }

    private void ProcessControlRequests()
    {
        while (_running && _controlRequests.Reader.TryRead(out var request))
        {
            if (request.CancellationToken.IsCancellationRequested) continue;
            var error = request.Command is { } command ? ApplyControl(command) : null;
            request.Completion.TrySetResult(new DeviceReply<PvControlState>(new PvControlState(
                _config, !Time.IsRunning, _mode, Time.Clock.Speed, _pv.Weather, _refreshMs,
                _faults.InverterFault, _faults.OverheatForced, _faults.GridDisturbanceForced), error));
        }
    }

    private string? ApplyControl(DeviceCommand command)
    {
        if (!Time.IsStandalone && command.Action is "pause" or "mode" or "speed" or "weather")
            return "Diese Aktion ist nur im Standalone-Betrieb verfügbar.";

        switch (command.Action)
        {
            case "stop":
                _running = false;
                SetNotification("Beendet");
                break;
            case "clean":
                CleanModules();
                break;
            case "refresh" when int.TryParse(command.Value, out var refresh) && refresh >= 50:
                SetRefresh(refresh);
                break;
            case "inverter" when bool.TryParse(command.Value, out var inverter):
                if (_faults.InverterFault != inverter) ToggleFault();
                break;
            case "overheat" when bool.TryParse(command.Value, out var heat):
                if (_faults.OverheatForced != heat) ToggleOverheat();
                break;
            case "grid" when bool.TryParse(command.Value, out var grid):
                if (_faults.GridDisturbanceForced != grid) ToggleGrid();
                break;
            case "pause" when bool.TryParse(command.Value, out var pause):
                if (Time.IsRunning == pause) SetNotification(Time.TogglePause());
                break;
            case "mode" when Enum.TryParse<SimulationMode>(command.Value, out var mode)
                && mode is SimulationMode.Auto or SimulationMode.Manual:
                SetMode(mode);
                break;
            case "speed" when double.TryParse(command.Value, CultureInfo.InvariantCulture, out var speed)
                && speed is >= 1 and <= 100000:
                SetNotification(Time.SetSpeed(speed));
                break;
            case "weather" when _weather is null && Enum.TryParse<WeatherCondition>(command.Value, out var weather)
                && weather is WeatherCondition.Night or WeatherCondition.Sunny or WeatherCondition.PartlyCloudy
                    or WeatherCondition.Overcast or WeatherCondition.Storm:
                SetWeather(weather);
                break;
            default:
                return "Diese Aktion oder der angegebene Wert ist im aktuellen Betrieb nicht verfügbar.";
        }
        return null;
    }

    private void SetRefresh(int refreshMs)
    {
        _refreshMs = refreshMs;
        SetNotification($"Bild-Aktualisierung → {_refreshMs} ms");
    }
}
