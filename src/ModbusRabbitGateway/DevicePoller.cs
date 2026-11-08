using Modbus.Client;
using Modbus.Contracts;

namespace ModbusRabbitGateway;

/// <summary>Ergebnis einer Lesung: Messwertnachricht (nur bei Erfolg) und/oder geänderter Gerätestatus.</summary>
public sealed record PollOutcome(MeasurementMessage? Message, DeviceStatusMessage? StatusChange);

/// <summary>
/// Liest ein Gerät über Modbus und baut daraus Nachrichten. Ohne gültige Lesung gibt es <b>keine</b> Messwertnachricht,
/// nur eine Statusänderung (<c>offline</c>/<c>decodeError</c>); so erscheinen alte Werte nie als aktuelle.
/// Die Modbus-Bibliothek ist synchron (akzeptierte Bibliotheksgrenze); nach einem Verbindungsfehler verbindet
/// <see cref="DeviceConnection"/> beim nächsten Lesen selbst neu.
/// </summary>
public sealed class DevicePoller : IDisposable
{
    private readonly DeviceConnection _connection;
    private readonly QualitySettings _quality;
    private string? _state;
    private string? _error;

    public DevicePoller(DeviceEndpoint device, int timeoutMs, QualitySettings quality)
    {
        Device = device;
        _quality = quality;
        _connection = new DeviceConnection(device.Host, device.Port, device.UnitId, timeoutMs);
    }

    public DeviceEndpoint Device { get; }

    public DeviceTracker Tracker { get; } = new();

    /// <summary>Aktueller Status (<c>online</c>, <c>offline</c>, <c>decodeError</c>); <c>null</c> vor der ersten Lesung.</summary>
    public string? State => _state;

    public PollOutcome Poll(DateTimeOffset now)
    {
        ushort[] registers;
        try
        {
            registers = _connection.ReadRegisters(0, CommonRegisters.TotalRegisters);
        }
        catch (DiagnosticException ex)
        {
            string state = ex.Kind == DiagnosticErrorKind.Connection ? Contract.DeviceState.Offline : Contract.DeviceState.DecodeError;
            return new PollOutcome(null, ChangeState(state, ex.Message, now));
        }

        var result = MessageBuilder.Build(Device, registers, now, Tracker, _quality);
        if (result.Message is null)
            return new PollOutcome(null, ChangeState(Contract.DeviceState.DecodeError, result.Error, now));

        return new PollOutcome(result.Message, ChangeState(Contract.DeviceState.Online, null, now));
    }

    /// <returns>Statusnachricht bei geändertem Zustand, sonst <c>null</c>.</returns>
    /// <remarks>
    /// Bei <c>offline</c> zählt nur der Zustand: Der erste Fehler kommt vom Lesen auf der abgebrochenen Verbindung, alle
    /// weiteren vom erneuten Verbinden – der Text wechselt, der Zustand nicht. Bei <c>decodeError</c> zählt auch der
    /// Grund (z. B. anderes falsches Gerät am Port).
    /// </remarks>
    private DeviceStatusMessage? ChangeState(string state, string? error, DateTimeOffset now)
    {
        if (state == _state && (state == Contract.DeviceState.Offline || error == _error))
            return null;

        _state = state;
        _error = error;
        return new DeviceStatusMessage(Contract.SchemaVersion, Device.DeviceId, state, error, Contract.FormatUtc(now),
            Tracker.LastSimTime, Tracker.LastRunId, Tracker.LastSeq);
    }

    public void Dispose() => _connection.Dispose();
}
