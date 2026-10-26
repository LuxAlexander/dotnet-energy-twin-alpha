using Devices.Common.Modbus;
using Modbus.Contracts;
using Verbraucher;
using Verbraucher.IO;
using Verbraucher.Simulation;

namespace FieldDevices.Tests;

/// <summary>Modell des Kühlhauses: Exponentiallösung, Hysterese, Störungen und Sollwertgrenzen.</summary>
public class KuehlhausModelTests
{
    private static Kuehlhaus CreateRoom(Action<KuehlhausConfig>? change = null)
    {
        var config = new KuehlhausConfig();
        change?.Invoke(config);
        return new Kuehlhaus(config);
    }

    [Fact]
    public void Update_UsesExactExponentialSolution()
    {
        var room = CreateRoom();
        room.TurnOff();                                            // keine Kühlung, nur Wärmeeintrag über die Hülle
        double expected = 20 + (room.RoomTempC - 20) * Math.Exp(-24 / 20.0);   // τ = C/k = 1,0/0,05 = 20 h

        room.Update(TimeSpan.FromHours(24), 20);

        Assert.Equal(expected, room.RoomTempC, 6);
        Assert.True(room.RoomTempC < 20, "Die Exponentiallösung darf die Außentemperatur nicht überschwingen.");
    }

    [Fact]
    public void LargeStep_StaysStableAndDoesNotOvershoot()
    {
        var room = CreateRoom();
        room.TurnOff();

        room.Update(TimeSpan.FromDays(365), 20);

        Assert.InRange(room.RoomTempC, 19.9, 20.0);
    }

    [Fact]
    public void Compressor_SwitchesWithHysteresisAroundSetpoint()
    {
        var room = CreateRoom();
        Assert.False(room.CompressorOn);
        var time = new DateTime(2026, 6, 21, 12, 0, 0);

        // Aufwärmen mit offener Tür, bis der Kompressor bei Soll + Hysterese einschaltet.
        room.SetDoor(true);
        for (int i = 0; i < 200 && !room.CompressorOn; i++)
        {
            time = time.AddMinutes(5);
            room.Update(TimeSpan.FromMinutes(5), 20);
        }

        Assert.True(room.CompressorOn, "Der Kompressor muss bei Soll + Hysterese einschalten.");
        // Die Hysterese wird vor dem Temperaturschritt geprüft, der Schritt selbst kann etwas überschwingen.
        Assert.True(room.RoomTempC >= room.SetpointC + 1.9, $"eingeschaltet bei {room.RoomTempC:0.00} °C");

        // Kühlen, bis er bei Soll − Hysterese wieder abschaltet.
        room.SetDoor(false);
        for (int i = 0; i < 2000 && room.CompressorOn; i++)
        {
            time = time.AddMinutes(5);
            room.Update(TimeSpan.FromMinutes(5), 20);
        }

        Assert.False(room.CompressorOn, "Der Kompressor muss bei Soll − Hysterese abschalten.");
        // Abschalten wird vor dem Temperaturschritt geprüft, der Schritt selbst kann etwas zurückerwärmen.
        Assert.True(room.RoomTempC <= room.SetpointC - 1.7, $"abgeschaltet bei {room.RoomTempC:0.00} °C");
        Assert.Equal(DeviceState.Idle, room.State);
    }

    [Fact]
    public void ElectricPowerAndEnergy_FollowCoolingPowerAndCop()
    {
        var room = CreateRoom();
        room.Update(TimeSpan.FromHours(3), 20);                    // Raum auf −14 °C, noch unter dem Alarm
        room.Update(TimeSpan.Zero, 20);                           // Hysterese prüfen, ohne Zeit vergehen zu lassen
        Assert.True(room.CompressorOn);
        Assert.Equal(6.0, room.CoolingPowerKW);

        room.Update(TimeSpan.FromHours(1), 20);

        Assert.Equal(3.0, room.ElectricPowerKW, 6);                // 6 kW Kälte / COP 2,0 bei 20 °C
        Assert.Equal(3.0, room.EnergyKWh, 6);
        Assert.Equal(1.0, room.RuntimeHours, 6);
        Assert.Equal(DeviceState.Cooling, room.State);
    }

    [Fact]
    public void Setpoint_OutsideMinus30To0_IsRejected()
    {
        var room = CreateRoom();

        Assert.Throws<ArgumentOutOfRangeException>(() => room.SetpointC = 0.5);
        Assert.Throws<ArgumentOutOfRangeException>(() => room.SetpointC = -30.5);
        Assert.Throws<ArgumentOutOfRangeException>(() => room.SetpointC = double.NaN);

        room.SetpointC = -15;
        Assert.Equal(-15, room.SetpointC);
    }

    [Fact]
    public void TurnOff_StopsCompressorAndReportsOffState()
    {
        var room = CreateRoom();
        room.Update(TimeSpan.FromHours(3), 20);
        room.Update(TimeSpan.Zero, 20);
        Assert.True(room.CompressorOn);

        room.TurnOff();

        Assert.False(room.CompressorOn);
        Assert.Equal(0, room.CoolingPowerKW);
        Assert.Equal(0, room.ElectricPowerKW);
        Assert.Equal(DeviceState.Off, room.State);
    }

    [Fact]
    public void CompressorFailure_IsAlwaysClearedByReset()
    {
        var room = CreateRoom();
        room.TriggerFault(FaultType.CompressorFailure);
        Assert.Equal(FaultType.CompressorFailure, room.ActiveFault);
        Assert.Equal(DeviceState.Fault, room.State);

        room.Update(TimeSpan.FromMinutes(5), 20);
        Assert.False(room.CompressorOn);                           // ohne Kompressor keine Kühlung

        Assert.True(room.ResetFault());
        Assert.Equal(FaultType.None, room.ActiveFault);
    }

    [Fact]
    public void OverTemperature_LatchesAndNeedsCoolDownBeforeReset()
    {
        var room = CreateRoom();
        room.TurnOff();

        room.Update(TimeSpan.FromHours(4), 20);
        room.Update(TimeSpan.FromHours(4), 20);

        Assert.Equal(FaultType.OverTemperature, room.ActiveFault);
        Assert.False(room.ResetFault(), "Der Alarm darf im warmen Raum nicht quittierbar sein.");
        Assert.Equal(FaultType.OverTemperature, room.ActiveFault);

        room.TurnOn();
        room.Update(TimeSpan.FromHours(2), 20);                    // Kompressor kühlt unter Alarm − Hysterese

        Assert.True(room.RoomTempC <= room.ResetAlarmBelowC);
        Assert.True(room.ResetFault());
        Assert.Equal(FaultType.None, room.ActiveFault);
    }
}

/// <summary>Modbus-Server des Kühlhauses: Registerkopf, gerätespezifische Werte und Schreibprüfung.</summary>
public sealed class KuehlhausModbusTests : ModbusTestBase
{
    private readonly Kuehlhaus _room = new(new KuehlhausConfig());
    private readonly KuehlhausModbus _modbus;
    private readonly List<string> _messages = [];

    public KuehlhausModbusTests()
    {
        _modbus = new KuehlhausModbus(_room, Time, Options, message => _messages.Add(message));
        _modbus.Start();
        Connect();
    }

    public override void Dispose()
    {
        _modbus.Dispose();
        base.Dispose();
    }

    [Fact]
    public void Publish_WritesHeaderAndSpecificRegisters()
    {
        _room.Update(TimeSpan.FromMinutes(5), 20);
        _modbus.Publish(Start, 1);

        var header = Read(0, 20);
        Assert.Equal((ushort)DeviceType.IndustrialConsumer, header[CommonRegisters.DeviceType]);
        Assert.Equal((ushort)1, header[CommonRegisters.DeviceInstance]);
        Assert.Equal((ushort)OperatingState.Standby, header[CommonRegisters.OperatingState]);
        Assert.Equal((ushort)0, header[CommonRegisters.ErrorCode]);
        Assert.Equal((ushort)TimeStatus.Standalone, header[CommonRegisters.TimeStatus]);
        Assert.Equal((ushort)1, header[CommonRegisters.RunId]);

        Assert.Equal(-20.0, ReadFloat(KuehlhausRegisters.Setpoint), 3);
        Assert.Equal(_room.RoomTempC, ReadFloat(KuehlhausRegisters.RoomTemperature), 3);
        Assert.Equal(20.0, ReadFloat(KuehlhausRegisters.OutsideTemperature), 3);
        Assert.Equal((ushort)0, Read(KuehlhausRegisters.Door, 1)[0]);
        Assert.Equal((ushort)0, Read(KuehlhausRegisters.Compressor, 1)[0]);
        Assert.Equal(0.0, ReadFloat(KuehlhausRegisters.CoolingPower), 3);
    }

    [Fact]
    public void Publish_RunningCompressor_SetsRunningStateAndPower()
    {
        _room.Update(TimeSpan.FromHours(3), 20);
        _room.Update(TimeSpan.Zero, 20);
        _room.Update(TimeSpan.FromHours(1), 20);
        Assert.True(_room.CompressorOn);

        _modbus.Publish(Start, 1);

        var header = Read(0, 20);
        Assert.Equal((ushort)OperatingState.Running, header[CommonRegisters.OperatingState]);
        Assert.Equal((ushort)1, Read(KuehlhausRegisters.Compressor, 1)[0]);
        Assert.True(ReadFloat(CommonRegisters.ActivePower) > 0);
        Assert.True(ReadFloat(CommonRegisters.Energy) > 0);
        Assert.Equal(6.0, ReadFloat(KuehlhausRegisters.CoolingPower), 3);
    }

    [Fact]
    public void SetpointWrite_IsApplied()
    {
        WriteFloat(KuehlhausRegisters.Setpoint, -15);
        WaitUntil(() => Read(CommonRegisters.LastWriteAddress, 1)[0] == KuehlhausRegisters.Setpoint);
        _modbus.ProcessCommands();

        Assert.Equal(-15.0, _room.SetpointC, 3);
        Assert.Equal(-15.0, ReadFloat(KuehlhausRegisters.Setpoint), 3);
        Assert.Equal((ushort)WriteError.None, Read(CommonRegisters.LastWriteError, 1)[0]);
    }

    [Fact]
    public void SetpointWrite_OutOfRange_IsRejectedAndOldValueKept()
    {
        WriteFloat(KuehlhausRegisters.Setpoint, 5);
        WaitUntil(() => Read(CommonRegisters.LastWriteError, 1)[0] != 0);
        _modbus.ProcessCommands();

        Assert.Equal(-20.0, _room.SetpointC, 3);
        Assert.Equal(-20.0, ReadFloat(KuehlhausRegisters.Setpoint), 3);
        Assert.Equal((ushort)WriteError.InvalidValue, Read(CommonRegisters.LastWriteError, 1)[0]);
        Assert.Equal((ushort)KuehlhausRegisters.Setpoint, Read(CommonRegisters.LastWriteAddress, 1)[0]);
    }

    [Fact]
    public void DoorWrite_OpensAndClosesTheDoor()
    {
        Client!.WriteSingleRegister(KuehlhausRegisters.Door, 1);
        WaitUntil(() => Read(KuehlhausRegisters.Door, 1)[0] == 1);
        _modbus.ProcessCommands();
        Assert.True(_room.DoorOpen);

        Client!.WriteSingleRegister(KuehlhausRegisters.Door, 0);
        WaitUntil(() => Read(KuehlhausRegisters.Door, 1)[0] == 0);
        _modbus.ProcessCommands();
        Assert.False(_room.DoorOpen);
    }

    [Fact]
    public void PowerSwitch_TurnsPlantOffAndOn()
    {
        Client!.WriteSingleRegister(CommonRegisters.PowerSwitch, 0);
        WaitUntil(() => Read(CommonRegisters.PowerSwitch, 1)[0] == 0);
        _modbus.ProcessCommands();
        Assert.False(_room.IsOn);

        _modbus.Publish(Start, 1);
        Assert.Equal((ushort)OperatingState.Off, Read(CommonRegisters.OperatingState, 1)[0]);

        Client!.WriteSingleRegister(CommonRegisters.PowerSwitch, 1);
        WaitUntil(() => Read(CommonRegisters.PowerSwitch, 1)[0] == 1);
        _modbus.ProcessCommands();
        Assert.True(_room.IsOn);
    }

    [Fact]
    public void ResetCommand_ClearsCompressorFailure()
    {
        _room.TriggerFault(FaultType.CompressorFailure);
        _modbus.Publish(Start, 1);
        var header = Read(0, 20);
        Assert.Equal((ushort)OperatingState.Fault, header[CommonRegisters.OperatingState]);
        Assert.Equal((ushort)1, header[CommonRegisters.ErrorCode]);

        Client!.WriteSingleRegister(CommonRegisters.ResetCommand, 1);
        WaitUntil(() => _messages.Count > 0);
        _modbus.ProcessCommands();

        Assert.Equal(FaultType.None, _room.ActiveFault);
        _modbus.Publish(Start, 1);
        Assert.Equal((ushort)0, Read(CommonRegisters.ErrorCode, 1)[0]);
    }
}

/// <summary>Kommandozeilenoptionen des Kühlhauses.</summary>
public class KuehlhausArgsTests
{
    [Fact]
    public void StandaloneDefaults_MatchTheDeviceDocumentation()
    {
        var config = ArgsParser.Parse(["--standalone"]);

        Assert.Equal("KH-01", config.DeviceId);
        Assert.Equal(-20.0, config.SetpointC);
        Assert.Equal(6.0, config.CoolingPowerKW);
        Assert.Equal(5.0, config.StepMinutes);
        Assert.Null(config.DurationHours);
        Assert.Null(config.Seed);
        Assert.Null(config.WeatherUrl);                            // standalone: lokales Tagesprofil
        Assert.Equal(SimulationConfig.DefaultModbusPort, config.Modbus.Port);
        Assert.Equal(1504, config.Modbus.Port);
        Assert.True(config.TimeSync.Standalone);
    }

    [Fact]
    public void WithMaster_WeatherSimulatorIsDefault()
    {
        var config = ArgsParser.Parse([]);

        Assert.Equal(SimulationConfig.DefaultWeatherUrl, config.WeatherUrl);
        Assert.Equal("salzburg", config.Location);
    }

    [Fact]
    public void WeatherOff_DisablesTheWeatherSimulator()
    {
        Assert.Null(ArgsParser.Parse(["--weather", "off"]).WeatherUrl);
        Assert.Null(ArgsParser.Parse(["--weather", "off", "--standalone"]).WeatherUrl);
    }

    [Fact]
    public void Values_AreParsedWithDotAndComma()
    {
        var config = ArgsParser.Parse(["--standalone", "--setpoint", "-15,5", "--cooling", "8", "--step", "10", "--duration", "24"]);

        Assert.Equal(-15.5, config.SetpointC);
        Assert.Equal(8.0, config.CoolingPowerKW);
        Assert.Equal(10.0, config.StepMinutes);
        Assert.Equal(24.0, config.DurationHours);
        Assert.Equal(TimeSpan.FromMinutes(10), config.Step);
    }

    [Theory]
    [InlineData("5")]
    [InlineData("-30.5")]
    [InlineData("abc")]
    public void InvalidSetpoint_IsRejected(string value)
    {
        Assert.Throws<ArgumentException>(() => ArgsParser.Parse(["--standalone", "--setpoint", value]));
    }

    [Fact]
    public void DurationWithoutStandalone_IsRejected()
    {
        var ex = Assert.Throws<ArgumentException>(() => ArgsParser.Parse(["--duration", "24"]));
        Assert.Contains("--standalone", ex.Message);
    }

    [Fact]
    public void UnknownOption_IsRejected()
    {
        Assert.Throws<ArgumentException>(() => ArgsParser.Parse(["--standalone", "--temperatur", "5"]));
    }
}

/// <summary>Hauptschleife des Kühlhauses: Standalone-Lauf, CSV und Anzeigezeilen.</summary>
public class KuehlhausEngineTests
{
    [Fact]
    public async Task Kuehlhaus_RunsStandalone_WritesCsvAndStopsOnCancel()
    {
        string directory = Path.Combine(Path.GetTempPath(), "kuehlhaus-test-" + Guid.NewGuid().ToString("N"));
        try
        {
            var config = ArgsParser.Parse(["--standalone", "--speed", "20000", "--start", "2026-06-21 08:00",
                "--weather", "off", "--no-modbus", "--seed", "42", "--step", "15", "--output", directory]);
            var engine = new SimulationEngine(config);
            using var stop = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            string csv = await engine.RunAsync(stop.Token);

            var lines = await File.ReadAllLinesAsync(csv);
            Assert.True(lines.Length > 5, $"nur {lines.Length} Zeilen");
            Assert.EndsWith("Tuer;Zustand;Fehler", lines[0]);
            Assert.StartsWith("2026-06-21 08:00;", lines[1]);                     // beginnt genau bei --start
            Assert.Contains(";lokal;", lines[1]);                                 // ohne Wettersimulator lokal
            Assert.True(engine.Room.RuntimeHours > 0, "Der Kompressor muss im Lauf kühlen.");
            Assert.Equal(1, engine.RunId);
            Assert.Contains("off", engine.WeatherStatus);
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task Renderer_BuildsInteractiveLayout()
    {
        var config = ArgsParser.Parse(["--standalone", "--weather", "off", "--no-modbus", "--seed", "1"]);
        var engine = new SimulationEngine(config);

        var lines = ConsoleRenderer.BuildLines(engine);

        Assert.True(lines.Count >= 10, $"nur {lines.Count} Zeilen");
        Assert.Contains(lines, line => line.StartsWith("=== Kühlhaus KH-01", StringComparison.Ordinal));
        Assert.Contains(lines, line => line.Contains("Außentemperatur"));
        Assert.Contains(lines, line => line.Contains("Tasten:"));
        Assert.Contains(lines, line => line.Contains("Tür"));
        await Task.CompletedTask;
    }
}

/// <summary>Automatikbetrieb: Tagesprofil der Türöffnungen, Reproduzierbarkeit mit Seed.</summary>
public class KuehlhausAutoModeTests
{
    private static int CountOpenSteps(int seed)
    {
        var room = new Kuehlhaus(new KuehlhausConfig());
        var auto = new AutoModeController(room, new Random(seed));
        var time = new DateTime(2026, 6, 21, 0, 0, 0);
        int open = 0;

        for (int i = 0; i < 24 * 12; i++)
        {
            auto.Step(time, TimeSpan.FromMinutes(5));
            if (room.DoorOpen)
                open++;

            room.Update(TimeSpan.FromMinutes(5), 12);
            time = time.AddMinutes(5);
        }

        return open;
    }

    [Fact]
    public void DoorRate_IsHigherDuringTheDayThanAtNight()
    {
        Assert.True(AutoModeController.DoorRatePerHour(10) > AutoModeController.DoorRatePerHour(3));
        Assert.True(AutoModeController.DoorRatePerHour(6) > AutoModeController.DoorRatePerHour(22));

        double probability = AutoModeController.DoorProbability(10, TimeSpan.FromMinutes(5));
        Assert.InRange(probability, 0, 1);
    }

    [Fact]
    public void SameSeed_OpensTheDoorTheSameWay()
    {
        int first = CountOpenSteps(42);
        int second = CountOpenSteps(42);

        Assert.Equal(first, second);
        Assert.True(first > 0, "Über 24 h muss die Tür mindestens einmal aufgehen.");
    }
}
