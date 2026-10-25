using System.Net;
using System.Net.Sockets;
using Devices.Common.Modbus;
using EasyModbus;
using Modbus.Contracts;

namespace Devices.Common.Tests;

public sealed class ModbusDeviceServerTests : IDisposable
{
    private const int Limit = 20;     // Float32, schreibbar 50–350
    private const int Counter = 22;   // UInt16, nur lesbar

    private static readonly RegisterPlan Plan = new(DeviceType.Charger, "Test",
    [
        new RegisterDefinition(Limit, "Leistungsgrenze", RegisterType.Float32, RegisterAccess.ReadWrite, "kW", "Test", Min: 50, Max: 350),
        new RegisterDefinition(Counter, "Zähler", RegisterType.UInt16, RegisterAccess.ReadOnly, "", "Test"),
    ]);

    private readonly ModbusDeviceServer _server;
    private readonly ModbusClient _client;
    private readonly DeviceTimeSource _time = DeviceTimeSource.Create(
        new TimeSyncOptions { Standalone = true, StandaloneStart = new DateTime(2026, 1, 15, 6, 0, 0), StandaloneSpeed = 60 }, "DC-02", "Charger");
    private readonly List<string> _log = [];

    public ModbusDeviceServerTests()
    {
        int port = FreePort();
        _server = new ModbusDeviceServer(new ModbusOptions { Port = port }, Plan, "DC-02");
        _server.Log += (_, message) => { lock (_log) _log.Add(message); };
        _server.Start();
        _server.Publish(Header(), _time, w => w.SetFloat(Limit, 150));

        _client = new ModbusClient("127.0.0.1", port) { ConnectionTimeout = 2000 };
        _client.Connect();
    }

    public void Dispose()
    {
        _client.Disconnect();
        _server.Dispose();
    }

    private static HeaderValues Header(DateTime? modelTime = null, int run = 1, bool on = true) =>
        new(OperatingState.Running, 0, 42.5, 123.25, on, modelTime ?? new DateTime(2026, 1, 15, 6, 5, 0), run);

    private static int FreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    /// <summary>Die Bibliothek antwortet auf einen Schreibzugriff, bevor die Anwendung ihn sieht; deshalb kurz warten.</summary>
    private ModbusCommand? WaitForCommand()
    {
        ModbusCommand? command = null;
        SpinWait.SpinUntil(() => _server.TryDequeueCommand(out command), TimeSpan.FromSeconds(3));
        return command;
    }

    private ushort[] Read(int address, int count) => _client.ReadHoldingRegisters(address, count).Select(v => (ushort)v).ToArray();

    private double ReadFloat(int address)
    {
        var r = Read(address, 2);
        return RegisterCodec.DecodeFloat32(r[0], r[1]);
    }

    private void WriteFloat(int address, float value)
    {
        var r = RegisterCodec.EncodeFloat32(value);
        _client.WriteMultipleRegisters(address, [r[0], r[1]]);
    }

    [Fact]
    public void Header_IsReadableWithIdentityStateAndTime()
    {
        var header = Read(0, 20);

        Assert.Equal((ushort)DeviceType.Charger, header[CommonRegisters.DeviceType]);
        Assert.Equal(2, header[CommonRegisters.DeviceInstance]);
        Assert.Equal((ushort)OperatingState.Running, header[CommonRegisters.OperatingState]);
        Assert.Equal(42.5f, RegisterCodec.DecodeFloat32(header[4], header[5]));
        Assert.Equal(123.25f, RegisterCodec.DecodeFloat32(header[6], header[7]));
        Assert.Equal(new DateTime(2026, 1, 15, 6, 5, 0), RegisterCodec.FromSeconds(RegisterCodec.DecodeUInt32(header[8], header[9])));
        Assert.Equal(1, header[CommonRegisters.RunId]);
        Assert.Equal((ushort)TimeStatus.Standalone, header[CommonRegisters.TimeStatus]);
        Assert.Equal(1, header[CommonRegisters.PowerSwitch]);
        Assert.Equal(150f, ReadFloat(Limit));
    }

    [Fact]
    public void Publish_SequenceGrowsOnlyWithNewModelTime()
    {
        ushort first = Read(CommonRegisters.Sequence, 1)[0];

        _server.Publish(Header(), _time);                                    // gleicher Zustand (z. B. Pause)
        Assert.Equal(first, Read(CommonRegisters.Sequence, 1)[0]);

        _server.Publish(Header(new DateTime(2026, 1, 15, 6, 10, 0)), _time);
        Assert.Equal((ushort)(first + 1), Read(CommonRegisters.Sequence, 1)[0]);

        _server.Publish(Header(new DateTime(2026, 1, 15, 6, 10, 0), run: 2), _time);   // Neustart im Master
        Assert.Equal((ushort)(first + 2), Read(CommonRegisters.Sequence, 1)[0]);
        Assert.Equal(2, Read(CommonRegisters.RunId, 1)[0]);
    }

    [Fact]
    public void ValidWrite_IsAccepted_AndQueuedForDevice()
    {
        WriteFloat(Limit, 200f);

        Assert.Equal(200f, ReadFloat(Limit));
        Assert.Equal(0, Read(CommonRegisters.LastWriteError, 1)[0]);
        var command = WaitForCommand();
        Assert.NotNull(command);
        Assert.Equal(Limit, command.Definition.Address);
        Assert.Equal(200.0, command.Value);
    }

    [Theory]
    [InlineData(49f)]
    [InlineData(351f)]
    [InlineData(-10f)]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    public void InvalidValue_IsRejected_AndOldValueStays(float value)
    {
        WriteFloat(Limit, value);

        Assert.Equal(150f, ReadFloat(Limit));
        Assert.Equal((ushort)WriteError.InvalidValue, Read(CommonRegisters.LastWriteError, 1)[0]);
        Assert.Equal(Limit, Read(CommonRegisters.LastWriteAddress, 1)[0]);
        Assert.False(_server.TryDequeueCommand(out _));
        Assert.Contains(_log, m => m.Contains("abgelehnt"));
    }

    [Fact]
    public void WriteToReadOnlyRegister_IsRejected()
    {
        ushort before = Read(CommonRegisters.OperatingState, 1)[0];

        _client.WriteSingleRegister(CommonRegisters.OperatingState, 0);

        Assert.Equal(before, Read(CommonRegisters.OperatingState, 1)[0]);
        Assert.Equal((ushort)WriteError.NotWritable, Read(CommonRegisters.LastWriteError, 1)[0]);
        Assert.False(_server.TryDequeueCommand(out _));
    }

    [Fact]
    public void WriteToUnusedAddress_IsRejected()
    {
        _client.WriteSingleRegister(40, 7);

        Assert.Equal(0, Read(40, 1)[0]);
        Assert.Equal((ushort)WriteError.NotWritable, Read(CommonRegisters.LastWriteError, 1)[0]);
    }

    [Fact]
    public void PartialWriteOfFloat_IsRejected()
    {
        _client.WriteSingleRegister(Limit, 0x4348);                          // nur das höhere Wort von 200

        Assert.Equal(150f, ReadFloat(Limit));
        Assert.Equal((ushort)WriteError.IncompleteValue, Read(CommonRegisters.LastWriteError, 1)[0]);
        Assert.False(_server.TryDequeueCommand(out _));
    }

    [Fact]
    public void WriteSpanningReadOnlyAndWritable_IsRejectedCompletely()
    {
        var limit = RegisterCodec.EncodeFloat32(300f);
        _client.WriteMultipleRegisters(Limit, [limit[0], limit[1], 99]);     // Zähler (Adresse 22) ist nur lesbar

        Assert.Equal(150f, ReadFloat(Limit));
        Assert.Equal((ushort)WriteError.NotWritable, Read(CommonRegisters.LastWriteError, 1)[0]);
        Assert.False(_server.TryDequeueCommand(out _));
    }

    [Fact]
    public void PowerSwitchAndReset_AreWritableCommands()
    {
        _client.WriteSingleRegister(CommonRegisters.PowerSwitch, 0);
        _client.WriteSingleRegister(CommonRegisters.ResetCommand, 1);

        var off = WaitForCommand();
        Assert.NotNull(off);
        Assert.Equal(CommonRegisters.PowerSwitch, off.Definition.Address);
        Assert.Equal(0, off.Value);
        var reset = WaitForCommand();
        Assert.NotNull(reset);
        Assert.Equal(CommonRegisters.ResetCommand, reset.Definition.Address);

        _client.WriteSingleRegister(CommonRegisters.PowerSwitch, 2);          // nur 0/1 erlaubt
        _client.WriteSingleRegister(CommonRegisters.ResetCommand, 0);         // nur 1 erlaubt
        Assert.True(SpinWait.SpinUntil(() => Read(CommonRegisters.LastWriteError, 1)[0] != 0, TimeSpan.FromSeconds(3)));
        Assert.False(_server.TryDequeueCommand(out _));
    }

    [Fact]
    public void Registers_StayReadable_WhilePaused_AndKeepLastState()
    {
        _time.TogglePause();                                                  // lokale Uhr pausiert
        _server.Publish(Header(), _time);

        var header = Read(0, 20);
        Assert.Equal(0, header[CommonRegisters.SimulationRunning]);
        Assert.Equal(new DateTime(2026, 1, 15, 6, 5, 0), RegisterCodec.FromSeconds(RegisterCodec.DecodeUInt32(header[8], header[9])));
        Assert.Equal(42.5f, RegisterCodec.DecodeFloat32(header[4], header[5]));
    }

    [Fact]
    public void Start_OnBusyPort_ThrowsIOException()
    {
        using var second = new ModbusDeviceServer(_server.Options, Plan, "X-1");

        Assert.Throws<IOException>(second.Start);
    }
}

public class RegisterCodecTests
{
    [Fact]
    public void Float32_IsBigEndianWithHighWordFirst()
    {
        // 1.0f = 0x3F800000
        Assert.Equal(new ushort[] { 0x3F80, 0x0000 }, RegisterCodec.EncodeFloat32(1.0f));
        Assert.Equal(-2.5f, RegisterCodec.DecodeFloat32(RegisterCodec.EncodeFloat32(-2.5f)[0], RegisterCodec.EncodeFloat32(-2.5f)[1]));
    }

    [Fact]
    public void UInt32_RoundTrips()
    {
        var registers = RegisterCodec.EncodeUInt32(0x12345678);
        Assert.Equal(new ushort[] { 0x1234, 0x5678 }, registers);
        Assert.Equal(0x12345678u, RegisterCodec.DecodeUInt32(registers[0], registers[1]));
    }

    [Fact]
    public void ModelTime_RoundTripsThroughSeconds()
    {
        var time = new DateTime(2026, 12, 31, 23, 59, 58);
        Assert.Equal(time, RegisterCodec.FromSeconds(RegisterCodec.ToSeconds(time)));
    }

    [Fact]
    public void Plan_RejectsOverlapsAndRangeViolations()
    {
        Assert.Throws<ArgumentException>(() => new RegisterPlan(DeviceType.Wind, "x",
            [new RegisterDefinition(5, "falsch", RegisterType.UInt16, RegisterAccess.ReadOnly, "", "im Kopf")]));
        Assert.Throws<ArgumentException>(() => new RegisterPlan(DeviceType.Wind, "x",
        [
            new RegisterDefinition(20, "a", RegisterType.Float32, RegisterAccess.ReadOnly, "", ""),
            new RegisterDefinition(21, "b", RegisterType.UInt16, RegisterAccess.ReadOnly, "", ""),
        ]));
        Assert.Throws<ArgumentException>(() => new RegisterPlan(DeviceType.Wind, "x",
            [new RegisterDefinition(49, "zu lang", RegisterType.Float32, RegisterAccess.ReadOnly, "", "")]));
    }

    [Fact]
    public void Validate_ChecksRangeTypeAndFiniteness()
    {
        var limit = new RegisterDefinition(20, "L", RegisterType.Float32, RegisterAccess.ReadWrite, "kW", "", Min: 50, Max: 350);
        Assert.Null(limit.Validate(50));
        Assert.Null(limit.Validate(350));
        Assert.NotNull(limit.Validate(49.9));
        Assert.NotNull(limit.Validate(double.NaN));

        var count = new RegisterDefinition(20, "N", RegisterType.UInt16, RegisterAccess.ReadWrite, "", "", Min: 0, Max: 10);
        Assert.NotNull(count.Validate(1.5));
        Assert.NotNull(count.Validate(11));
    }
}
