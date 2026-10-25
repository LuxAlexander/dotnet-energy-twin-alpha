using System.Collections.Concurrent;
using System.Globalization;
using System.Net.Sockets;
using EasyModbus;
using Modbus.Contracts;
using SimCore.Contracts;
using SimCore.Time;

namespace Devices.Common.Modbus;

/// <summary>Ein angenommener Schreibzugriff eines Clients, den das Gerät in seinem Simulationsthread umsetzt.</summary>
/// <param name="Definition">Beschriebenes Register (Registerplan).</param>
/// <param name="Value">Technischer Wert (nach Skalierung), bereits gegen den Registerplan geprüft.</param>
public sealed record ModbusCommand(RegisterDefinition Definition, double Value);

/// <summary>Werte des gemeinsamen Registerkopfs, die das Gerät liefert; Zeitfelder füllt der Server aus der Zeitquelle.</summary>
/// <param name="ModelTime">Virtuelle Zeit des dargestellten Zustands; <c>null</c> bis zur ersten Zeit.</param>
public sealed record HeaderValues(
    OperatingState State,
    ushort ErrorCode,
    double ActivePowerKW,
    double EnergyKWh,
    bool IsOn,
    DateTime? ModelTime,
    int RunId);

/// <summary>Schreibt gerätespezifische Werte in den Registerplan; Adresse und Datentyp werden geprüft.</summary>
public sealed class RegisterWriter
{
    private readonly RegisterPlan _plan;
    private readonly ushort[] _registers;

    internal RegisterWriter(RegisterPlan plan, ushort[] registers)
    {
        _plan = plan;
        _registers = registers;
    }

    public void SetUInt16(int address, int value) => Set(address, RegisterType.UInt16, value);

    public void SetUInt32(int address, double value) => Set(address, RegisterType.UInt32, value);

    public void SetFloat(int address, double value) => Set(address, RegisterType.Float32, value);

    private void Set(int address, RegisterType type, double value)
    {
        var definition = _plan.Get(address);
        if (definition.Type != type)
            throw new ArgumentException($"Register {address} ({definition.Name}) ist {definition.Type}, nicht {type}.");

        RegisterCodec.Encode(definition, value).CopyTo(_registers, address);
    }
}

/// <summary>
/// Modbus-TCP-Server eines Geräts (EasyModbusTCP.NETCore): bildet den <see cref="RegisterPlan"/> auf die Holding Registers ab.
/// Der Server läuft in eigenen Threads der Bibliothek und bleibt auch bei Pause oder verlorenem Master erreichbar;
/// das Gerät veröffentlicht seinen Zustand mit <see cref="Publish"/> und liest Schreibzugriffe mit <see cref="TryDequeueCommand"/>.
/// </summary>
/// <remarks>
/// Die Bibliothek übernimmt einen Schreibzugriff zuerst und meldet ihn erst danach. Ungültige Zugriffe werden deshalb
/// sofort rückgängig gemacht (Register zeigt wieder den alten Wert), im Register <c>Letzter Schreibfehler</c> vermerkt und
/// als Meldung gemeldet; der Client bekommt von der Bibliothek trotzdem eine positive Antwort und muss den Fehler lesen.
/// </remarks>
public sealed class ModbusDeviceServer : IDisposable
{
    private const int MaxQueuedCommands = 256;

    private readonly ModbusOptions _options;
    private readonly RegisterPlan _plan;
    private readonly int _instance;
    private readonly ModbusServer _server = new();
    private readonly ConcurrentQueue<ModbusCommand> _commands = new();
    private readonly Lock _gate = new();
    private readonly ushort[] _shadow = new ushort[CommonRegisters.TotalRegisters];

    private DateTime? _lastModelTime;
    private int _lastRunId = -1;
    private ushort _sequence;
    private bool _listening;

    public ModbusDeviceServer(ModbusOptions options, RegisterPlan plan, string deviceId)
    {
        _options = options;
        _plan = plan;
        _instance = ParseInstance(deviceId);
    }

    /// <summary>Meldungen über Clients und Schreibzugriffe (kann aus Serverthreads kommen).</summary>
    public event EventHandler<string>? Log;

    public ModbusOptions Options => _options;

    /// <summary>
    /// Zusätzliche gerätespezifische Prüfung für Werte, deren Grenzen nicht im Registerplan stehen können
    /// (z. B. Sollwert innerhalb der konfigurierten Maximalleistung). Liefert <c>null</c> für gültig, sonst die Begründung.
    /// Wird im Thread des Modbus-Servers aufgerufen und darf nur Werte lesen.
    /// </summary>
    public Func<RegisterDefinition, double, string?>? ValueValidator { get; set; }

    public RegisterPlan Plan => _plan;

    public bool IsListening => _listening;

    public int ClientCount => _server.NumberOfConnections;

    /// <summary>Beschreibung des letzten Schreibzugriffs für die Anzeige, leer solange keiner stattfand.</summary>
    public string LastWriteText { get; private set; } = "";

    public string StatusText =>
        $"{_options.Bind}:{_options.Port} · Unit {_options.UnitId} · {ClientCount} Client(s)" +
        (LastWriteText.Length > 0 ? $" · {LastWriteText}" : "");

    /// <exception cref="IOException">Port belegt oder Adresse nicht verfügbar.</exception>
    public void Start()
    {
        if (_listening)
            return;

        // Die Bibliothek bindet den Port erst in ihrem eigenen Thread; ein Fehler dort würde das Programm
        // unkontrolliert beenden. Deshalb vorher selbst prüfen, ob der Port frei ist.
        try
        {
            var probe = new TcpListener(_options.BindAddress, _options.Port);
            probe.Start();
            probe.Stop();
        }
        catch (SocketException ex)
        {
            throw new IOException(
                $"Modbus-Server kann nicht auf {_options.Bind}:{_options.Port} starten (Port schon belegt? Mit --modbus-port einen anderen wählen): {ex.Message}", ex);
        }

        _server.LocalIPAddress = _options.BindAddress;
        _server.Port = _options.Port;
        _server.UnitIdentifier = _options.UnitId;
        _server.HoldingRegistersChanged += OnHoldingRegistersChanged;
        _server.NumberOfConnectedClientsChanged += OnClientsChanged;
        _server.Listen();
        _listening = true;
    }

    /// <summary>
    /// Schreibt den aktuellen Gerätezustand in die Register. Mehrfach pro Sekunde aufrufbar, auch während der Pause:
    /// die Sequenznummer wächst nur, wenn sich Modellzeit oder Lauf geändert haben.
    /// </summary>
    /// <param name="specific">Gerätespezifische Register (ab Adresse 20) und Sollwerte, die zurückgelesen werden.</param>
    public void Publish(HeaderValues header, DeviceTimeSource time, Action<RegisterWriter>? specific = null)
    {
        lock (_gate)
        {
            if (header.ModelTime != _lastModelTime || header.RunId != _lastRunId)
            {
                _sequence++;
                _lastModelTime = header.ModelTime;
                _lastRunId = header.RunId;
            }

            var staging = (ushort[])_shadow.Clone();
            var writer = new RegisterWriter(_plan, staging);

            writer.SetUInt16(CommonRegisters.DeviceType, (ushort)_plan.DeviceType);
            writer.SetUInt16(CommonRegisters.DeviceInstance, _instance);
            writer.SetUInt16(CommonRegisters.OperatingState, (ushort)header.State);
            writer.SetUInt16(CommonRegisters.ErrorCode, header.ErrorCode);
            writer.SetFloat(CommonRegisters.ActivePower, header.ActivePowerKW);
            writer.SetFloat(CommonRegisters.Energy, header.EnergyKWh);
            writer.SetUInt32(CommonRegisters.ModelTime, header.ModelTime is { } modelTime ? RegisterCodec.ToSeconds(modelTime) : 0);
            writer.SetUInt16(CommonRegisters.RunId, (ushort)header.RunId);
            writer.SetUInt16(CommonRegisters.Sequence, _sequence);
            writer.SetUInt16(CommonRegisters.TimeStatus, (ushort)MapStatus(time));
            writer.SetUInt16(CommonRegisters.SimulationRunning, time.IsRunning ? 1 : 0);
            writer.SetFloat(CommonRegisters.Speed, time.HasTime ? time.Clock.Speed : 0);
            writer.SetUInt16(CommonRegisters.PowerSwitch, header.IsOn ? 1 : 0);
            writer.SetUInt16(CommonRegisters.ResetCommand, 0);
            specific?.Invoke(writer);

            Commit(staging, 0, staging.Length);
        }
    }

    /// <returns><c>false</c>, wenn kein angenommener Schreibzugriff wartet.</returns>
    public bool TryDequeueCommand(out ModbusCommand? command) => _commands.TryDequeue(out command);

    public void Dispose()
    {
        if (!_listening)
            return;

        _listening = false;
        _server.HoldingRegistersChanged -= OnHoldingRegistersChanged;
        _server.NumberOfConnectedClientsChanged -= OnClientsChanged;
        _server.StopListening();
    }

    private static TimeStatus MapStatus(DeviceTimeSource time) =>
        time.IsStandalone ? TimeStatus.Standalone
        : time.Status switch
        {
            SyncStatus.Synced => TimeStatus.Synced,
            SyncStatus.MasterLost => TimeStatus.MasterLost,
            _ => TimeStatus.NoTime
        };

    /// <summary>Zahl am Ende der Geräte-ID ("DC-02" → 2), sonst 0.</summary>
    private static int ParseInstance(string deviceId)
    {
        int end = deviceId.Length;
        int start = end;
        while (start > 0 && char.IsAsciiDigit(deviceId[start - 1]))
            start--;
        return start < end && int.TryParse(deviceId[start..end], NumberStyles.None, CultureInfo.InvariantCulture, out int value) && value <= ushort.MaxValue
            ? value
            : 0;
    }

    /// <summary>Übernimmt Registerinhalte in den Server und in die Kopie, mit der Ungültiges zurückgesetzt wird.</summary>
    private void Commit(ushort[] source, int start, int count)
    {
        for (int address = start; address < start + count; address++)
        {
            _shadow[address] = source[address];
            _server.holdingRegisters[address + 1] = unchecked((short)source[address]);
        }
    }

    private void SetErrorRegisters(WriteError error, int address)
    {
        _shadow[CommonRegisters.LastWriteError] = (ushort)error;
        _shadow[CommonRegisters.LastWriteAddress] = (ushort)address;
        Commit(_shadow, CommonRegisters.LastWriteError, 2);
    }

    private void OnClientsChanged() =>
        Log?.Invoke(this, $"Modbus: {ClientCount} Client(s) verbunden.");

    /// <summary>
    /// Prüft einen Schreibzugriff der Bibliothek (<paramref name="register"/> ist die 1-basierte Startadresse).
    /// Alles oder nichts: Ist ein Teil ungültig, wird der ganze Zugriff zurückgesetzt.
    /// </summary>
    private void OnHoldingRegistersChanged(int register, int numberOfRegisters)
    {
        string message;
        lock (_gate)
        {
            int start = register - 1;
            int end = Math.Min(start + numberOfRegisters, CommonRegisters.TotalRegisters);
            var accepted = new List<ModbusCommand>();
            var failure = Validate(start, end, accepted);

            if (failure is { } f)
            {
                // Rückgängig machen: Register zeigen wieder den letzten bekannten Stand
                for (int address = Math.Max(start, 0); address < end; address++)
                    _server.holdingRegisters[address + 1] = unchecked((short)_shadow[address]);
                SetErrorRegisters(f.Error, f.Address);
                message = $"Modbus-Schreibzugriff ab Adresse {start} ({numberOfRegisters} Register) abgelehnt: {f.Reason}.";
            }
            else
            {
                for (int address = start; address < end; address++)
                    _shadow[address] = unchecked((ushort)_server.holdingRegisters[address + 1]);
                SetErrorRegisters(WriteError.None, start);
                foreach (var command in accepted)
                {
                    _commands.Enqueue(command);
                    while (_commands.Count > MaxQueuedCommands)
                        _commands.TryDequeue(out _);
                }
                message = "Modbus-Schreibzugriff angenommen: " +
                          string.Join(", ", accepted.Select(c => FormattableString.Invariant($"{c.Definition.Name} = {c.Value:0.###} {c.Definition.Unit}").TrimEnd()));
            }
            LastWriteText = message;
        }

        Log?.Invoke(this, message);
    }

    private (WriteError Error, int Address, string Reason)? Validate(int start, int end, List<ModbusCommand> accepted)
    {
        if (start < 0 || start >= end)
            return (WriteError.NotWritable, Math.Max(start, 0), "Adresse außerhalb des Registerplans");

        int address = start;
        while (address < end)
        {
            var definition = _plan.Find(address);
            if (definition is null)
                return (WriteError.NotWritable, address, $"Adresse {address} ist nicht belegt");
            if (!definition.IsWritable)
                return (WriteError.NotWritable, address, $"{definition.Name} (Adresse {definition.Address}) ist nur lesbar");
            if (definition.Address < start || definition.EndAddress > end)
                return (WriteError.IncompleteValue, definition.Address,
                    $"{definition.Name} belegt {definition.Count} Register und muss mit einem Zugriff vollständig geschrieben werden");

            var raw = new ushort[definition.Count];
            for (int i = 0; i < raw.Length; i++)
                raw[i] = unchecked((ushort)_server.holdingRegisters[definition.Address + i + 1]);

            double value = RegisterCodec.Decode(definition, raw);
            if ((definition.Validate(value) ?? ValueValidator?.Invoke(definition, value)) is { } reason)
                return (WriteError.InvalidValue, definition.Address, $"{definition.Name} = {value.ToString("0.###", CultureInfo.InvariantCulture)}: {reason}");

            accepted.Add(new ModbusCommand(definition, value));
            address = definition.EndAddress;
        }
        return null;
    }
}
