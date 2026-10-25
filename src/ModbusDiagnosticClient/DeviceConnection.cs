using System.Net.Sockets;
using EasyModbus;
using EasyModbus.Exceptions;
using Modbus.Contracts;

namespace ModbusDiagnosticClient;

public enum DiagnosticErrorKind
{
    /// <summary>Gerät nicht erreichbar oder Verbindung abgebrochen.</summary>
    Connection,

    /// <summary>Gerät hat die Anfrage mit einer Modbus-Ausnahme beantwortet oder die Antwort war ungültig.</summary>
    Protocol,

    /// <summary>Eingabe oder Wert ungültig (noch vor dem Senden erkannt).</summary>
    Value
}

/// <summary>Fehler mit verständlicher deutscher Meldung, getrennt nach Verbindungs-, Protokoll- und Wertefehler.</summary>
public sealed class DiagnosticException(DiagnosticErrorKind kind, string message, Exception? inner = null)
    : Exception(message, inner)
{
    public DiagnosticErrorKind Kind { get; } = kind;
}

/// <summary>
/// Verbindung zu einem Modbus-Gerät. Die Bibliothek EasyModbusTCP.NETCore ist synchron (Bibliotheksgrenze der Aufgabe,
/// deshalb nicht in <c>Task.Run</c> verpackt); alle Bibliotheksfehler werden hier in <see cref="DiagnosticException"/> übersetzt.
/// Nach einem Verbindungsfehler baut der nächste Zugriff die Verbindung selbst wieder auf.
/// </summary>
public sealed class DeviceConnection : IDisposable
{
    private readonly ModbusClient _client;

    public DeviceConnection(string host, int port, byte unitId, int timeoutMs)
    {
        Host = host;
        Port = port;
        UnitId = unitId;
        TimeoutMs = timeoutMs;
        _client = new ModbusClient(host, port) { UnitIdentifier = unitId, ConnectionTimeout = timeoutMs, NumberOfRetries = 1 };
    }

    public string Host { get; }
    public int Port { get; }
    public byte UnitId { get; }
    public int TimeoutMs { get; }

    public string Endpoint => $"{Host}:{Port} (Unit {UnitId})";

    public bool IsConnected => _client.Connected;

    /// <exception cref="DiagnosticException">Gerät nicht erreichbar.</exception>
    public void Connect()
    {
        if (_client.Connected)
            return;

        try
        {
            _client.Connect();
        }
        catch (Exception ex) when (IsConnectionProblem(ex))
        {
            Close();
            throw new DiagnosticException(DiagnosticErrorKind.Connection,
                $"Keine Verbindung zu {Endpoint}: {Describe(ex)}. Läuft das Gerät, stimmen Port und --modbus-bind?", ex);
        }
    }

    /// <summary>Liest Holding Registers (Protokolladressen, 0-basiert).</summary>
    public ushort[] ReadRegisters(int start, int count)
    {
        Connect();
        try
        {
            return _client.ReadHoldingRegisters(start, count).Select(v => unchecked((ushort)v)).ToArray();
        }
        catch (Exception ex) when (IsModbusProblem(ex) || IsConnectionProblem(ex))
        {
            throw Translate(ex, $"Lesen ab Adresse {start} ({count} Register)");
        }
    }

    /// <summary>Schreibt einen Wert: ein Register mit FC06, mehrteilige Werte mit FC16 auf beide Register.</summary>
    public void Write(RegisterDefinition definition, double value)
    {
        Connect();
        var registers = RegisterCodec.Encode(definition, value);
        try
        {
            if (registers.Length == 1)
                _client.WriteSingleRegister(definition.Address, registers[0]);
            else
                _client.WriteMultipleRegisters(definition.Address, registers.Select(r => (int)r).ToArray());
        }
        catch (Exception ex) when (IsModbusProblem(ex) || IsConnectionProblem(ex))
        {
            throw Translate(ex, $"Schreiben von {definition.Name} (Adresse {definition.Address})");
        }
    }

    public void Close()
    {
        try
        {
            _client.Disconnect();
        }
        catch (Exception ex) when (IsConnectionProblem(ex))
        {
            // Verbindung war ohnehin weg
        }
    }

    public void Dispose() => Close();

    private DiagnosticException Translate(Exception ex, string action)
    {
        if (ex is FunctionCodeNotSupportedException)
            return new DiagnosticException(DiagnosticErrorKind.Protocol, $"{action}: Funktionscode vom Gerät nicht unterstützt.", ex);
        if (ex is StartingAddressInvalidException)
            return new DiagnosticException(DiagnosticErrorKind.Protocol, $"{action}: Adresse existiert im Gerät nicht.", ex);
        if (ex is QuantityInvalidException)
            return new DiagnosticException(DiagnosticErrorKind.Protocol, $"{action}: ungültige Anzahl Register.", ex);
        if (ex is ModbusException and not ConnectionException)
            return new DiagnosticException(DiagnosticErrorKind.Protocol, $"{action}: Modbus-Fehler: {ex.Message}", ex);

        Close();                                                  // der nächste Zugriff verbindet neu
        return new DiagnosticException(DiagnosticErrorKind.Connection,
            $"{action}: Verbindung zu {Endpoint} abgebrochen oder keine Antwort innerhalb {TimeoutMs} ms ({Describe(ex)}).", ex);
    }

    private static bool IsModbusProblem(Exception ex) => ex is ModbusException or CRCCheckFailedException;

    private static bool IsConnectionProblem(Exception ex) =>
        ex is ConnectionException or SocketException or IOException or TimeoutException or ObjectDisposedException
            or InvalidOperationException or NullReferenceException;

    private static string Describe(Exception ex) =>
        ex is SocketException socket ? $"{socket.Message} (Socket-Fehler {socket.SocketErrorCode})" : ex.Message;
}
