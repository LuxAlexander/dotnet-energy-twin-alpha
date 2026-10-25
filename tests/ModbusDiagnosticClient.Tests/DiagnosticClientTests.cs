using System.Net;
using System.Net.Sockets;
using Devices.Common;
using Devices.Common.Modbus;
using Modbus.Contracts;
using ModbusDiagnosticClient;

namespace ModbusDiagnosticClient.Tests;

/// <summary>Diagnose-Client gegen einen echten <see cref="ModbusDeviceServer"/> mit dem Registerplan der Ladesäule.</summary>
public sealed class DiagnosticClientTests : IDisposable
{
    private static readonly DateTime ModelTime = new(2026, 1, 15, 6, 5, 0);

    private readonly int _port = FreePort();
    private readonly DeviceTimeSource _time = DeviceTimeSource.Create(
        new TimeSyncOptions { Standalone = true, StandaloneStart = ModelTime, StandaloneSpeed = 60 }, "DC-01", "Charger");
    private ModbusDeviceServer _server;
    private readonly DeviceConnection _connection;
    private readonly StringWriter _output = new();
    private readonly DiagnosticSession _session;

    public DiagnosticClientTests()
    {
        _server = StartServer();
        _connection = new DeviceConnection("127.0.0.1", _port, 1, 1500);
        _session = new DiagnosticSession(_connection, _output);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _server.Dispose();
    }

    private ModbusDeviceServer StartServer()
    {
        var server = new ModbusDeviceServer(new ModbusOptions { Port = _port }, ChargerRegisters.Plan, "DC-01");
        server.Start();
        server.Publish(new HeaderValues(OperatingState.Running, 0, 42.5, 123.25, true, ModelTime, 1), _time,
            w => w.SetFloat(ChargerRegisters.PowerLimit, 150));
        return server;
    }

    private static int FreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    [Fact]
    public async Task Read_ShowsDecodedHeaderAndDeviceRegisters_WithRawValues()
    {
        await _session.PrintAllAsync();
        string text = _output.ToString();

        Assert.Contains("Charger Nr. 1", text);
        Assert.Contains("3 = Charger", text);
        Assert.Contains("2 = Betrieb", text);
        Assert.Contains("42,5 kW", text);                          // Float32 dekodiert
        Assert.Contains("0x422A 0x0000", text);                    // Rohwert von 42,5
        Assert.Contains("2026-01-15 06:05:00", text, StringComparison.Ordinal);   // Modellzeit
        Assert.Contains("2 = Standalone", text);
        Assert.Contains("Leistungsgrenze der Säule", text);
        Assert.Contains("150 kW", text);
        Assert.NotNull(_session.Plan);
    }

    [Fact]
    public async Task Write_ValidSetpoint_IsAcceptedAndVisibleInDevice()
    {
        var outcome = await _session.WriteAsync("Leistungsgrenze", "200", force: false, CancellationToken.None);

        Assert.True(outcome.Accepted);
        Assert.Contains("ANGENOMMEN", outcome.Message);
        Assert.True(_server.TryDequeueCommand(out var command));
        Assert.Equal(ChargerRegisters.PowerLimit, command!.Definition.Address);
        Assert.Equal(200.0, command.Value);
    }

    [Theory]
    [InlineData("49")]
    [InlineData("351")]
    [InlineData("abc")]
    public async Task Write_InvalidValue_IsRefusedByClientBeforeSending(string value)
    {
        var ex = await Assert.ThrowsAsync<DiagnosticException>(() =>
            _session.WriteAsync("Leistungsgrenze", value, force: false, CancellationToken.None));

        Assert.Equal(DiagnosticErrorKind.Value, ex.Kind);
        Assert.False(_server.TryDequeueCommand(out _));            // nichts gesendet
    }

    [Fact]
    public async Task Force_InvalidValue_IsRejectedByDevice_AndExplained()
    {
        var outcome = await _session.WriteAsync("20", "999", force: true, CancellationToken.None);

        Assert.False(outcome.Accepted);
        Assert.Equal(WriteError.InvalidValue, outcome.Error);
        Assert.Equal(ChargerRegisters.PowerLimit, outcome.ErrorAddress);
        Assert.Contains("ABGELEHNT", outcome.Message);
        Assert.Contains("150 kW", outcome.Message);                // alter Wert bleibt
    }

    [Fact]
    public async Task Force_ReadOnlyAndUnusedAddress_AreRejectedByDevice()
    {
        var readOnly = await _session.WriteAsync("Wirkleistung", "5", force: true, CancellationToken.None);
        var unused = await _session.WriteAsync("45", "1", force: true, CancellationToken.None);

        Assert.Equal(WriteError.NotWritable, readOnly.Error);
        Assert.Equal(WriteError.NotWritable, unused.Error);
    }

    [Fact]
    public async Task Write_ReadOnlyWithoutForce_IsRefusedByClient()
    {
        var ex = await Assert.ThrowsAsync<DiagnosticException>(() =>
            _session.WriteAsync("Wirkleistung", "5", force: false, CancellationToken.None));

        Assert.Equal(DiagnosticErrorKind.Value, ex.Kind);
        Assert.Contains("nur lesbar", ex.Message);
    }

    [Fact]
    public async Task Write_AmbiguousOrUnknownTarget_ThrowsValueError()
    {
        var ambiguous = await Assert.ThrowsAsync<DiagnosticException>(() =>
            _session.WriteAsync("Punkt", "1", force: false, CancellationToken.None));
        var unknown = await Assert.ThrowsAsync<DiagnosticException>(() =>
            _session.WriteAsync("gibtsnicht", "1", force: false, CancellationToken.None));

        Assert.Contains("nicht eindeutig", ambiguous.Message);
        Assert.Contains("Kein Register", unknown.Message);
    }

    [Fact]
    public async Task OnOffReset_ByLabelAndShortcut_ReachTheDevice()
    {
        await _session.ExecuteAsync("write Ein/Aus aus", CancellationToken.None);
        await _session.ExecuteAsync("reset", CancellationToken.None);

        Assert.True(_server.TryDequeueCommand(out var off));
        Assert.Equal(CommonRegisters.PowerSwitch, off!.Definition.Address);
        Assert.Equal(0, off.Value);
        Assert.True(_server.TryDequeueCommand(out var reset));
        Assert.Equal(CommonRegisters.ResetCommand, reset!.Definition.Address);
    }

    [Fact]
    public async Task ConnectionLoss_IsReportedAsConnectionError_AndReconnectsWhenDeviceIsBack()
    {
        _session.ReadAll();                                        // Verbindung steht

        _server.Dispose();                                         // Gerät verschwindet
        var lost = Assert.Throws<DiagnosticException>(() => _session.ReadAll());
        Assert.Equal(DiagnosticErrorKind.Connection, lost.Kind);
        Assert.False(_connection.IsConnected);

        _server = StartServer();                                   // Gerät kommt zurück: der nächste Zugriff verbindet neu
        var registers = _session.ReadAll();
        Assert.Equal((ushort)DeviceType.Charger, registers[CommonRegisters.DeviceType]);

        await _session.ExecuteAsync("read", CancellationToken.None);
        Assert.Contains("Charger Nr. 1", _output.ToString());
    }

    [Fact]
    public async Task Execute_ShowsErrorsAsTextInsteadOfThrowing()
    {
        _server.Dispose();

        bool keepGoing = await _session.ExecuteAsync("read", CancellationToken.None);

        Assert.True(keepGoing);
        Assert.Contains("VERBINDUNGSFEHLER", _output.ToString());
    }

    [Fact]
    public async Task Watch_KeepsGoingDuringOutage()
    {
        _server.Dispose();

        await _session.ExecuteAsync("watch 2 0.1", CancellationToken.None);

        string text = _output.ToString();
        Assert.Equal(2, text.Split("neuer Versuch").Length - 1);
    }

    [Fact]
    public async Task Quit_EndsTheSession()
    {
        Assert.False(await _session.ExecuteAsync("quit", CancellationToken.None));
        Assert.True(await _session.ExecuteAsync("unsinn", CancellationToken.None));
        Assert.Contains("Unbekannter Befehl", _output.ToString());
    }
}

public class ClientOptionsTests
{
    [Fact]
    public void Defaults()
    {
        var options = ClientOptions.Parse([]);

        Assert.Equal("127.0.0.1", options.Host);
        Assert.Equal(1502, options.Port);
        Assert.Equal(1, options.UnitId);
        Assert.False(options.IsOneShot);
    }

    [Fact]
    public void Parses_AllOptions_AndWrites()
    {
        var options = ClientOptions.Parse(["--host", "pc", "--port", "1503", "--unit-id", "5", "--timeout", "500",
            "--write", "Leistungsgrenze=200", "--write", "Ein/Aus=aus"]);

        Assert.Equal("pc", options.Host);
        Assert.Equal(1503, options.Port);
        Assert.Equal(5, options.UnitId);
        Assert.Equal(500, options.TimeoutMs);
        Assert.Equal([("Leistungsgrenze", "200"), ("Ein/Aus", "aus")], options.Writes);
        Assert.True(options.IsOneShot);
    }

    [Theory]
    [InlineData("--port", "0")]
    [InlineData("--port", "abc")]
    [InlineData("--unit-id", "248")]
    [InlineData("--timeout", "5")]
    [InlineData("--write", "ohneGleich")]
    [InlineData("--write", "=5")]
    [InlineData("--bogus", "1")]
    public void Invalid_Throws(string name, string value) =>
        Assert.Throws<ArgumentException>(() => ClientOptions.Parse([name, value]));

    [Fact]
    public void MissingValue_Throws() =>
        Assert.Throws<ArgumentException>(() => ClientOptions.Parse(["--host"]));
}

public class RegisterFormatterTests
{
    [Fact]
    public void Raw_ShowsHexWords() =>
        Assert.Equal("0x4316 0x0000", RegisterFormatter.Raw([0x4316, 0x0000]));

    [Fact]
    public void Value_UsesLabelUnitAndScale()
    {
        var state = CommonRegisters.HeaderAt(CommonRegisters.OperatingState);
        var power = CommonRegisters.HeaderAt(CommonRegisters.ActivePower);

        Assert.Equal("3 = Störung", RegisterFormatter.Value(state, [3]));
        Assert.Equal("150 kW", RegisterFormatter.Value(power, RegisterCodec.EncodeFloat32(150f)));
        Assert.Equal("Störung", RegisterFormatter.Short(state, [3]));
    }

    [Fact]
    public void Value_ModelTime_IsExactToTheSecond()
    {
        var time = new DateTime(2026, 1, 15, 6, 5, 7);
        var definition = CommonRegisters.HeaderAt(CommonRegisters.ModelTime);
        var registers = RegisterCodec.EncodeUInt32(RegisterCodec.ToSeconds(time));

        Assert.Equal(RegisterCodec.ToSeconds(time), RegisterCodec.Decode(definition, registers));   // kein float-Rundungsfehler
        Assert.Contains("2026-01-15 06:05:07", RegisterFormatter.Value(definition, registers));
    }

    [Fact]
    public void Value_ModelTimeZero_MeansNoTime() =>
        Assert.Contains("keine Zeit", RegisterFormatter.Value(CommonRegisters.HeaderAt(CommonRegisters.ModelTime), [0, 0]));
}
