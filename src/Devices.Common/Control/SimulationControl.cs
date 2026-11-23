using System.Globalization;
using System.Threading.Channels;
using Devices.Common.Modbus;
using Devices.Common.Weather;

namespace Devices.Common.Control;

/// <summary>Startwerte der Ladesäule und des Kühlhauses für die lokale Geräteverwaltung.</summary>
public sealed record EquipmentStartup(string DeviceId, double PowerKw, double? SetpointC,
    double StepMinutes, double? DurationHours, int? Seed, string OutputDirectory,
    TimeSyncOptions TimeSync, ModbusOptions Modbus, WeatherOptions Weather);

public sealed record EquipmentState<T>(EquipmentStartup Startup, bool Paused, double Speed, T Device);

/// <summary>Übergibt HTTP-Anfragen an die Simulationsschleife und antwortet mit deren Zustand.</summary>
public sealed class SimulationControl<T>(Func<T> readState, Func<DeviceCommand, string?> applyCommand)
{
    private sealed record Request(DeviceCommand? Command, CancellationToken Token,
        TaskCompletionSource<DeviceReply<T>> Completion);

    private readonly Channel<Request> _requests = Channel.CreateBounded<Request>(32);

    public Task<IAsyncDisposable> StartAsync(string id, string kind, CancellationToken cancellationToken,
        int? containerPort = null) =>
        DeviceControl.StartAsync<T>(id, kind, DispatchAsync, cancellationToken, containerPort);

    private async Task<DeviceReply<T>> DispatchAsync(DeviceCommand? command, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(3));
        var completion = new TaskCompletionSource<DeviceReply<T>>(TaskCreationOptions.RunContinuationsAsynchronously);
        await _requests.Writer.WriteAsync(new(command, timeout.Token, completion), timeout.Token);
        return await completion.Task.WaitAsync(timeout.Token);
    }

    public bool ProcessRequests(Func<bool> isRunning)
    {
        var changed = false;
        while (isRunning() && _requests.Reader.TryRead(out var request))
        {
            if (request.Token.IsCancellationRequested) continue;
            var command = request.Command;
            var error = command is null ? null : applyCommand(command);
            changed |= command is not null && error is null;
            request.Completion.TrySetResult(new(readState(), error));
        }
        return changed;
    }
}

public static class SimulationControl
{
    public static bool TryNumber(string? text, out double value) =>
        double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && double.IsFinite(value);

    public static string? ApplyTimeCommand(DeviceTimeSource time, DeviceCommand command)
    {
        if (!time.IsStandalone) return "Pause und Tempo werden vom Master gesteuert.";
        switch (command.Action)
        {
            case "pause" when bool.TryParse(command.Value, out var paused):
                if (time.IsRunning == paused) time.TogglePause();
                return null;
            case "speed" when TryNumber(command.Value, out var speed) && speed is >= 1 and <= 100000:
                time.SetSpeed(speed);
                return null;
            default:
                return "Ungültiger Wert für Pause oder Tempo.";
        }
    }
}
