using Modbus.Client;
using Modbus.Contracts;

namespace ModbusRabbitGateway;

/// <summary>Ergebnis einer Lesung: Messwertnachricht (nur bei Erfolg) und/oder zu sendender Gerätestatus.</summary>
/// <param name="StatusRepeated"><c>true</c>, wenn <paramref name="StatusChange"/> nur eine Wiederholung ohne Änderung ist.</param>
public sealed record PollOutcome(MeasurementMessage? Message, DeviceStatusMessage? StatusChange, bool StatusRepeated = false);

/// <summary>
/// Liest ein Gerät über Modbus und baut daraus Nachrichten. Ohne gültige Lesung gibt es <b>keine</b> Messwertnachricht,
/// nur einen Status (<c>offline</c>/<c>decodeError</c>); so erscheinen alte Werte nie als aktuelle.
/// Der Status wird bei Änderung gesendet und zusätzlich alle <c>statusRepeat</c> wiederholt: RabbitMQ liefert retained
/// Nachrichten nur an Abos mit exakt passendem Topic, nicht an <c>energytwin/#</c>; ohne Wiederholung sähe ein später
/// gestarteter Viewer ein ausgefallenes Gerät nicht.
/// Die Modbus-Bibliothek ist synchron (akzeptierte Bibliotheksgrenze); nach einem Verbindungsfehler verbindet
/// <see cref="DeviceConnection"/> beim nächsten Lesen selbst neu.
/// </summary>
public sealed class DevicePoller : IDisposable
{
    private readonly DeviceConnection _connection;
    private readonly QualitySettings _quality;
    private readonly TimeSpan _statusRepeat;
    private string? _state;
    private string? _error;
    private DateTimeOffset _since;
    private DateTimeOffset _lastSent;

    /// <param name="statusRepeat">Abstand für die Wiederholung des Status; <see cref="TimeSpan.Zero"/> = nur bei Änderung.</param>
    public DevicePoller(DeviceEndpoint device, int timeoutMs, QualitySettings quality, TimeSpan statusRepeat = default)
    {
        Device = device;
        _quality = quality;
        _statusRepeat = statusRepeat;
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
            return Status(null, state, ex.Message, now);
        }

        var result = MessageBuilder.Build(Device, registers, now, Tracker, _quality);
        if (result.Message is null)
            return Status(null, Contract.DeviceState.DecodeError, result.Error, now);

        return Status(result.Message, Contract.DeviceState.Online, null, now);
    }

    /// <remarks>
    /// Bei <c>offline</c> zählt nur der Zustand: Der erste Fehler kommt vom Lesen auf der abgebrochenen Verbindung, alle
    /// weiteren vom erneuten Verbinden – der Text wechselt, der Zustand nicht. Bei <c>decodeError</c> zählt auch der
    /// Grund (z. B. anderes falsches Gerät am Port). <c>sinceUtc</c> bleibt bei Wiederholungen der Beginn des Zustands.
    /// </remarks>
    private PollOutcome Status(MeasurementMessage? message, string state, string? error, DateTimeOffset now)
    {
        bool changed = state != _state || (state != Contract.DeviceState.Offline && error != _error);
        bool repeatDue = _statusRepeat > TimeSpan.Zero && now - _lastSent >= _statusRepeat;
        if (!changed && !repeatDue)
            return new PollOutcome(message, null);

        if (changed)
        {
            _state = state;
            _error = error;
            _since = now;
        }
        _lastSent = now;

        var status = new DeviceStatusMessage(Contract.SchemaVersion, Device.DeviceId, _state!, _error, Contract.FormatUtc(_since),
            Tracker.LastSimTime, Tracker.LastRunId, Tracker.LastSeq);
        return new PollOutcome(message, status, StatusRepeated: !changed);
    }

    public void Dispose() => _connection.Dispose();
}
