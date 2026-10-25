using System.Net;
using System.Net.Sockets;
using Batterie_Sim;
using Devices.Common;
using Devices.Common.Modbus;
using EasyModbus;
using EnergySimulator.Devices;
using EnergySimulator.Simulation;
using Modbus.Contracts;
using BatteryArgs = Batterie_Sim.ArgsParser;
using BatteryFault = Batterie_Sim.FaultType;
using PvArgs = EnergySimulator.Simulation.ArgsParser;
using PvFault = EnergySimulator.Devices.FaultType;

namespace FieldDevices.Tests;

/// <summary>Gemeinsame Hilfen: freier Port, Standalone-Zeitquelle, Registerzugriff über einen echten Modbus-Client.</summary>
public abstract class ModbusTestBase : IDisposable
{
    protected static readonly DateTime Start = new(2026, 6, 21, 12, 0, 0);

    protected readonly int Port = FreePort();
    protected readonly DeviceTimeSource Time = DeviceTimeSource.Create(
        new TimeSyncOptions { Standalone = true, StandaloneStart = Start, StandaloneSpeed = 60 }, "T-01", "Test");
    protected ModbusClient? Client;

    protected ModbusOptions Options => new() { Port = Port };

    protected static int FreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    protected void Connect()
    {
        Client = new ModbusClient("127.0.0.1", Port) { ConnectionTimeout = 2000 };
        Client.Connect();
    }

    protected ushort[] Read(int address, int count) => Client!.ReadHoldingRegisters(address, count).Select(v => (ushort)v).ToArray();

    protected double ReadFloat(int address)
    {
        var r = Read(address, 2);
        return RegisterCodec.DecodeFloat32(r[0], r[1]);
    }

    protected void WriteFloat(int address, float value)
    {
        var r = RegisterCodec.EncodeFloat32(value);
        Client!.WriteMultipleRegisters(address, [r[0], r[1]]);
    }

    /// <summary>
    /// Die Bibliothek schickt die Antwort auf einen Schreibzugriff, bevor die Anwendung ihn sieht (im Betrieb unkritisch,
    /// die Geräteschleife fragt ständig ab). Tests warten deshalb, bis die Bedingung erfüllt ist.
    /// </summary>
    protected static void WaitUntil(Func<bool> condition) =>
        Assert.True(SpinWait.SpinUntil(condition, TimeSpan.FromSeconds(3)), "Bedingung wurde nicht rechtzeitig erfüllt.");

    public virtual void Dispose() => Client?.Disconnect();
}

public sealed class PvModbusTests : ModbusTestBase
{
    private readonly PvSystem _pv;
    private readonly PvFaultModel _faults = new(new Random(1));
    private readonly PvModbus _modbus;
    private readonly List<string> _messages = [];

    public PvModbusTests()
    {
        _pv = new PvSystem(10.0, _faults);
        _modbus = new PvModbus(_pv, _faults, Time, Options, "PV-02", m => _messages.Add(m));
        _modbus.Start();
        _pv.StepContinuous(Start, 0.0);                           // Mittag im Sommer, Sonnig
        _modbus.Publish(Start, 1);
        Connect();
    }

    public override void Dispose()
    {
        base.Dispose();
        _modbus.Dispose();
    }

    [Fact]
    public void Header_ShowsGenerationAsNegativePower_AndPvSpecificValues()
    {
        var header = Read(0, 20);

        Assert.Equal((ushort)DeviceType.PvPlant, header[CommonRegisters.DeviceType]);
        Assert.Equal(2, header[CommonRegisters.DeviceInstance]);
        Assert.Equal((ushort)OperatingState.Running, header[CommonRegisters.OperatingState]);
        Assert.True(ReadFloat(CommonRegisters.ActivePower) < 0);                   // Abgabe = negativ
        Assert.Equal(-_pv.PowerKw, ReadFloat(CommonRegisters.ActivePower), 0.01);
        Assert.Equal(_pv.IrradiationWm2, ReadFloat(PvRegisters.Irradiation), 0.1);
        Assert.Equal(10.0, ReadFloat(PvRegisters.PeakPower));
        Assert.Equal(100.0, ReadFloat(PvRegisters.PowerLimit));
    }

    [Fact]
    public void PowerLimit_ValidValue_LimitsFeedInAtNextStep()
    {
        double unlimited = _pv.PowerKw;

        WriteFloat(PvRegisters.PowerLimit, 40f);
        WaitUntil(() => { _modbus.ProcessCommands(); return _pv.PowerLimitPercent == 40.0; });
        _pv.StepContinuous(Start.AddMinutes(1), 1 / 60.0);
        _modbus.Publish(Start.AddMinutes(1), 1);

        Assert.Equal(40.0, _pv.PowerLimitPercent);
        Assert.True(unlimited > 4.0);
        Assert.True(_pv.PowerKw <= 4.0 + 1e-9);
        Assert.Equal(40.0, ReadFloat(PvRegisters.PowerLimit));
        Assert.Equal(0, Read(CommonRegisters.LastWriteError, 1)[0]);
    }

    [Theory]
    [InlineData(-1f)]
    [InlineData(100.5f)]
    public void PowerLimit_OutOfRange_IsRejected_AndDeviceKeepsValue(float value)
    {
        WriteFloat(PvRegisters.PowerLimit, value);
        WaitUntil(() => Read(CommonRegisters.LastWriteError, 1)[0] != 0);
        _modbus.ProcessCommands();

        Assert.Equal(100.0, _pv.PowerLimitPercent);
        Assert.Equal(100.0, ReadFloat(PvRegisters.PowerLimit));
        Assert.Equal((ushort)WriteError.InvalidValue, Read(CommonRegisters.LastWriteError, 1)[0]);
    }

    [Fact]
    public void OnOffAndReset_AreApplied()
    {
        _faults.ToggleInverterFault();
        _pv.StepContinuous(Start.AddMinutes(1), 1 / 60.0);
        _modbus.Publish(Start.AddMinutes(1), 1);
        var header = Read(0, 20);
        Assert.Equal((ushort)OperatingState.Fault, header[CommonRegisters.OperatingState]);
        Assert.Equal((ushort)PvFault.Inverter, header[CommonRegisters.ErrorCode]);

        Client!.WriteSingleRegister(CommonRegisters.ResetCommand, 1);
        Client.WriteSingleRegister(CommonRegisters.PowerSwitch, 0);
        WaitUntil(() => { _modbus.ProcessCommands(); return !_pv.IsEnabled && !_faults.InverterFault; });
        _pv.StepContinuous(Start.AddMinutes(2), 1 / 60.0);
        _modbus.Publish(Start.AddMinutes(2), 1);

        Assert.False(_faults.InverterFault);
        Assert.False(_pv.IsEnabled);
        header = Read(0, 20);
        Assert.Equal((ushort)OperatingState.Off, header[CommonRegisters.OperatingState]);
        Assert.Equal(0, header[CommonRegisters.ErrorCode]);
        Assert.Equal(0.0, ReadFloat(CommonRegisters.ActivePower));
    }

    [Fact]
    public void Registers_StayReadable_WhileTimeIsPaused()
    {
        Time.TogglePause();
        _modbus.Publish(Start, 1);

        var header = Read(0, 20);
        Assert.Equal(0, header[CommonRegisters.SimulationRunning]);
        Assert.Equal(Start, RegisterCodec.FromSeconds(RegisterCodec.DecodeUInt32(header[8], header[9])));
    }
}

public sealed class BatteryModbusTests : ModbusTestBase
{
    private readonly BatterieConfig _config = new() { DeviceId = "BAT-02" };
    private readonly Batterie _battery;
    private readonly Simulator _simulator;
    private readonly BatteryModbus _modbus;

    public BatteryModbusTests()
    {
        _battery = new Batterie(_config);
        var options = new SimulatorOptions
        {
            Config = _config,
            TimeSync = new TimeSyncOptions { Standalone = true, StandaloneStart = Start, StandaloneSpeed = 60 },
            Modbus = Options
        };
        _simulator = new Simulator(_battery, new DailyProfile(_config, new Random(1)), options);
        _modbus = new BatteryModbus(_simulator, _battery, Time, Options, _ => { });
        _modbus.Start();
        Connect();
    }

    public override void Dispose()
    {
        base.Dispose();
        _modbus.Dispose();
    }

    [Fact]
    public void Header_ShowsChargingAsPositivePower_AndBatteryValues()
    {
        _battery.SetPower(3.0);
        _battery.Update(TimeSpan.FromMinutes(1));
        _modbus.Publish(Start, 1);

        var header = Read(0, 20);
        Assert.Equal((ushort)DeviceType.Battery, header[CommonRegisters.DeviceType]);
        Assert.Equal((ushort)OperatingState.Running, header[CommonRegisters.OperatingState]);
        Assert.Equal(3.0, ReadFloat(CommonRegisters.ActivePower), 0.01);          // Laden = positiv
        Assert.Equal(_battery.SocPercent, ReadFloat(BatteryRegisters.Soc), 0.01);
        Assert.Equal(20.0, ReadFloat(BatteryRegisters.Capacity));
        Assert.Equal(5.0, ReadFloat(BatteryRegisters.MaxChargePower));
        Assert.Equal(_battery.ChargedKWh, ReadFloat(CommonRegisters.Energy), 0.001);
    }

    [Fact]
    public void Setpoint_WithinConfiguredPower_IsAccepted_AndSwitchesToManual()
    {
        _modbus.Publish(Start, 1);

        WriteFloat(BatteryRegisters.Setpoint, -2.5f);
        WaitUntil(() => { _modbus.ProcessCommands(); return _battery.SetpointKW == -2.5; });

        Assert.Equal(ControlMode.Manual, _simulator.Mode);
        Assert.Equal(-2.5, _battery.SetpointKW);
        _modbus.Publish(Start, 1);
        Assert.Equal(-2.5, ReadFloat(BatteryRegisters.Setpoint));
        Assert.Equal(1, Read(BatteryRegisters.Mode, 1)[0]);
        Assert.Equal(0, Read(CommonRegisters.LastWriteError, 1)[0]);
    }

    [Theory]
    [InlineData(5.5f)]
    [InlineData(-5.5f)]
    [InlineData(float.NaN)]
    public void Setpoint_BeyondConfiguredPower_IsRejected(float value)
    {
        _modbus.Publish(Start, 1);

        WriteFloat(BatteryRegisters.Setpoint, value);
        WaitUntil(() => Read(CommonRegisters.LastWriteError, 1)[0] != 0);
        _modbus.ProcessCommands();

        Assert.Equal(ControlMode.Auto, _simulator.Mode);
        Assert.Equal(0.0, _battery.SetpointKW);
        Assert.Equal(0.0, ReadFloat(BatteryRegisters.Setpoint));
        Assert.Equal((ushort)WriteError.InvalidValue, Read(CommonRegisters.LastWriteError, 1)[0]);
    }

    [Fact]
    public void ModeRegister_SwitchesBackToAuto()
    {
        _simulator.Mode = ControlMode.Manual;
        _modbus.Publish(Start, 1);

        Client!.WriteSingleRegister(BatteryRegisters.Mode, 0);
        WaitUntil(() => { _modbus.ProcessCommands(); return _simulator.Mode == ControlMode.Auto; });

        Client.WriteSingleRegister(BatteryRegisters.Mode, 2);                      // ungültig
        WaitUntil(() => Read(CommonRegisters.LastWriteError, 1)[0] != 0);
        _modbus.ProcessCommands();
        Assert.Equal(ControlMode.Auto, _simulator.Mode);
        Assert.Equal((ushort)WriteError.InvalidValue, Read(CommonRegisters.LastWriteError, 1)[0]);
    }

    [Fact]
    public void FaultsAndSwitch_AreReportedAndResettable()
    {
        _battery.TriggerFault(BatteryFault.EmergencyStop);
        _battery.Update(TimeSpan.FromMinutes(1));
        _modbus.Publish(Start, 1);

        var header = Read(0, 20);
        Assert.Equal((ushort)OperatingState.Fault, header[CommonRegisters.OperatingState]);
        Assert.Equal((ushort)BatteryFault.EmergencyStop, header[CommonRegisters.ErrorCode]);

        Client!.WriteSingleRegister(CommonRegisters.ResetCommand, 1);
        Client.WriteSingleRegister(CommonRegisters.PowerSwitch, 0);
        WaitUntil(() => { _modbus.ProcessCommands(); return !_battery.IsOn && _battery.ActiveFault == BatteryFault.None; });
        _battery.Update(TimeSpan.FromMinutes(1));
        _modbus.Publish(Start, 1);

        header = Read(0, 20);
        Assert.Equal((ushort)OperatingState.Off, header[CommonRegisters.OperatingState]);
        Assert.Equal(0, header[CommonRegisters.ErrorCode]);
        Assert.False(_battery.IsOn);
    }
}

public class RegisterPlanRequirementTests
{
    [Fact]
    public void EveryDevicePlan_HasSetpointAndAtLeastThreeSpecificValues()
    {
        foreach (var plan in new[] { ChargerRegisters.Plan, PvRegisters.Plan, BatteryRegisters.Plan })
        {
            var specific = plan.Definitions.Where(d => d.Address >= CommonRegisters.SpecificStart).ToList();
            Assert.True(specific.Count(d => !d.IsWritable) >= 3, plan.DeviceName);
            Assert.Contains(specific, d => d.IsWritable && d.Min is not null && d.Max is not null);
            Assert.Equal(plan, RegisterPlans.For(plan.DeviceType));
        }
    }

    [Fact]
    public void Markdown_ListsAllRegisters_WithByteAndWordOrder()
    {
        string markdown = PvRegisters.Plan.ToMarkdown();

        Assert.Contains("Leistungsbegrenzung", markdown);
        Assert.Contains("Big-Endian", markdown);
        Assert.Contains("höherwertige Wort", markdown);
        Assert.Contains("Port `1502`", markdown);
    }
}

public class ArgsParserTests
{
    [Fact]
    public void Pv_Defaults_AndLb01Shortcuts()
    {
        var config = PvArgs.Parse(["--standalone", "-s", "300", "-d", "2026-06-21 05:00", "-p", "12,5", "-t", "30"]);

        Assert.Equal("PV-01", config.DeviceId);
        Assert.Equal(300, config.TimeSync.StandaloneSpeed);
        Assert.Equal(new DateTime(2026, 6, 21, 5, 0, 0), config.TimeSync.StandaloneStart);
        Assert.Equal(12.5, config.PeakPowerKw);
        Assert.Equal(30, config.StepMinutes);
        Assert.Equal(1502, config.Modbus.Port);
    }

    [Theory]
    [InlineData("--speed", "100")]
    [InlineData("-s", "100")]
    [InlineData("--start", "2026-01-01 00:00")]
    public void Pv_StandaloneOptions_RequireStandalone(string name, string value) =>
        Assert.Throws<ArgumentException>(() => PvArgs.Parse([name, value]));

    [Theory]
    [InlineData("--peak", "0")]
    [InlineData("--peak", "abc")]
    [InlineData("--modbus-port", "80")]
    [InlineData("--bogus", "1")]
    public void Pv_InvalidValues_Throw(string name, string value) =>
        Assert.Throws<ArgumentException>(() => PvArgs.Parse([name, value]));

    [Fact]
    public void Battery_ParsesConfig_TimeAndModbus()
    {
        var options = BatteryArgs.Parse(["--id", "BAT-07", "--capacity", "30", "--max-charge", "7,5", "--soc-start", "40",
            "--step", "30", "--standalone", "--speed", "1200", "--duration", "24", "--modbus-port", "1510", "--seed", "5"]);

        Assert.Equal("BAT-07", options.Config.DeviceId);
        Assert.Equal(30, options.Config.CapacityKWh);
        Assert.Equal(7.5, options.Config.MaxChargeKW);
        Assert.Equal(30, options.StepSeconds);
        Assert.Equal(24, options.DurationHours);
        Assert.Equal(1200, options.TimeSync.StandaloneSpeed);
        Assert.Equal(1510, options.Modbus.Port);
        Assert.Equal(5, options.Seed);
    }

    [Theory]
    [InlineData("--soc-min", "60")]                               // ≥ soc-max? nein, aber Start 50 liegt darunter
    [InlineData("--soc-max", "5")]
    [InlineData("--capacity", "0")]
    [InlineData("--step", "0")]
    [InlineData("--max-charge", "-1")]
    public void Battery_InvalidConfig_Throws(string name, string value) =>
        Assert.Throws<ArgumentException>(() => BatteryArgs.Parse([name, value]));

    [Theory]
    [InlineData("--duration", "24")]
    [InlineData("--speed", "100")]
    public void Battery_StandaloneOnlyOptions_RequireStandalone(string name, string value) =>
        Assert.Throws<ArgumentException>(() => BatteryArgs.Parse([name, value]));
}

public class PvEngineTests
{
    [Fact]
    public async Task Pv_RunsStandalone_WritesCsvAndStopsOnCancel()
    {
        string directory = Path.Combine(Path.GetTempPath(), "pv-test-" + Guid.NewGuid().ToString("N"));
        try
        {
            var config = PvArgs.Parse(["--standalone", "--speed", "20000", "--start", "2026-06-21 08:00", "--no-modbus",
                "--seed", "1", "--step", "15", "-o", directory]);
            var engine = new SimulationEngine(config);
            using var stop = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            string csv = await engine.RunAsync(stop.Token);

            var lines = await File.ReadAllLinesAsync(csv);
            Assert.True(lines.Length > 5, $"nur {lines.Length} Zeilen");
            Assert.EndsWith("Lauf;Leistungsbegrenzung_pct", lines[0]);
            Assert.StartsWith("2026-06-21;08:00;", lines[1]);                    // beginnt genau bei --start
            Assert.True(engine.Pv.TotalEnergyKwh > 0);
            Assert.Equal(1, engine.RunId);
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
    }
}

public class BatteryUnitTests
{
    [Fact]
    public void Update_TimeSpanAndSeconds_GiveSameResult()
    {
        var a = new Batterie(new BatterieConfig());
        var b = new Batterie(new BatterieConfig());
        a.SetPower(3);
        b.SetPower(3);

        a.Update(TimeSpan.FromMinutes(5));
        b.Update(300.0);

        Assert.Equal(b.SocPercent, a.SocPercent);
        Assert.Equal(b.ChargedKWh, a.ChargedKWh);
    }

    [Fact]
    public void DailyProfile_WithSeed_IsRepeatable()
    {
        var config = new BatterieConfig();
        var first = new DailyProfile(config, new Random(7));
        var second = new DailyProfile(config, new Random(7));

        foreach (var hour in new[] { 8, 10, 12, 19, 21 })
            Assert.Equal(first.GetSetpoint(TimeSpan.FromHours(hour)), second.GetSetpoint(TimeSpan.FromHours(hour)));
    }
}
