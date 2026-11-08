using System.Text.Json;
using Modbus.Contracts;

namespace ModbusRabbitGateway.Tests;

/// <summary>Dekodierung und Qualitätsstatus nach Nachrichtenvertrag, mit von Hand gefüllten Registern (ohne Netzwerk).</summary>
public sealed class MessageBuilderTests
{
    private static readonly DateTime SimTime = new(2026, 1, 15, 6, 0, 0);
    private static readonly DateTimeOffset Now = new(2026, 11, 8, 14, 32, 5, 123, TimeSpan.Zero);
    private static readonly DeviceEndpoint Charger = new("DC-01", "127.0.0.1", 1504);

    /// <summary>Register eines laufenden, synchronen Chargers wie in der Beispielnachricht des Vertrags.</summary>
    private static ushort[] ChargerRegisters(
        TimeStatus status = TimeStatus.Synced, bool running = true, int seq = 412, int runId = 1, float speed = 600,
        DateTime? simTime = null, ushort instance = 1, DeviceType type = DeviceType.Charger)
    {
        var r = new ushort[CommonRegisters.TotalRegisters];
        r[CommonRegisters.DeviceType] = (ushort)type;
        r[CommonRegisters.DeviceInstance] = instance;
        r[CommonRegisters.OperatingState] = (ushort)OperatingState.Running;
        SetFloat(r, CommonRegisters.ActivePower, 187.5f);
        SetFloat(r, CommonRegisters.Energy, 1243.82f);
        var time = RegisterCodec.EncodeUInt32(simTime is { } t ? RegisterCodec.ToSeconds(t) : RegisterCodec.ToSeconds(SimTime));
        r[CommonRegisters.ModelTime] = time[0];
        r[CommonRegisters.ModelTime + 1] = time[1];
        r[CommonRegisters.RunId] = (ushort)runId;
        r[CommonRegisters.Sequence] = (ushort)seq;
        r[CommonRegisters.TimeStatus] = (ushort)status;
        r[CommonRegisters.SimulationRunning] = (ushort)(running ? 1 : 0);
        SetFloat(r, CommonRegisters.Speed, speed);
        r[CommonRegisters.PowerSwitch] = 1;
        r[CommonRegisters.LastWriteError] = 2;
        r[CommonRegisters.LastWriteAddress] = 20;
        SetFloat(r, Modbus.Contracts.ChargerRegisters.PowerLimit, 200);
        r[Modbus.Contracts.ChargerRegisters.PointBase] = 2;                  // Punkt 1 lädt
        SetFloat(r, Modbus.Contracts.ChargerRegisters.PointBase + Modbus.Contracts.ChargerRegisters.PointSoc, 63.4f);
        return r;
    }

    private static void SetFloat(ushort[] registers, int address, float value)
    {
        var words = RegisterCodec.EncodeFloat32(value);
        registers[address] = words[0];
        registers[address + 1] = words[1];
    }

    private static MeasurementMessage Build(ushort[] registers, DeviceTracker? tracker = null, DateTimeOffset? at = null) =>
        MessageBuilder.Build(Charger, registers, at ?? Now, tracker ?? new DeviceTracker(), QualitySettings.Default).Message
        ?? throw new Xunit.Sdk.XunitException("keine Nachricht");

    [Fact]
    public void Build_FillsEnvelopeFromHeader_AndKeepsVirtualAndRealTimeApart()
    {
        var message = Build(ChargerRegisters());

        Assert.Equal(1, message.SchemaVersion);
        Assert.Equal("DC-01", message.DeviceId);
        Assert.Equal("Charger", message.DeviceType);
        Assert.Equal("2026-01-15T06:00:00", message.SimTime);                // virtuelle Zeit aus Register 8–9
        Assert.Equal(1768456800, message.SimTimeEpoch);
        Assert.Equal("2026-11-08T14:32:05.123Z", message.PolledAtUtc);      // reale Abfragezeit, getrennt
        Assert.Equal(1, message.RunId);
        Assert.Equal(412, message.Seq);
        Assert.Equal(1, message.PublishSeq);
        Assert.Equal((Contract.Quality.Good, Contract.Reason.Running), (message.Quality, message.QualityReason));
    }

    [Fact]
    public void Build_MeasurementsComeFromRegisterPlan_WithKeyUnitAndText()
    {
        var message = Build(ChargerRegisters());
        var byKey = message.Measurements.ToDictionary(m => m.Key);

        Assert.Equal(187.5, byKey["wirkleistung"].Value);
        Assert.Equal("kW", byKey["wirkleistung"].Unit);
        Assert.Equal(4, byKey["wirkleistung"].Address);
        Assert.Equal(1243.82, byKey["kumulierte_energie"].Value);           // auf 3 Stellen gerundet, kein Float-Rauschen
        Assert.Equal("Betrieb", byKey["betriebszustand"].Text);
        Assert.Equal(200, byKey["leistungsgrenze_der_saeule"].Value);
        Assert.Equal("lädt", byKey["punkt_1_zustand"].Text);
        Assert.Equal(63.4, byKey["punkt_1_soc"].Value);
        Assert.Equal("Punkt 1: SOC", byKey["punkt_1_soc"].Name);

        // Entscheidung 2026-11-08: Schreibfehler-Register werden mitgeschickt
        Assert.Equal("ungültiger Wert", byKey["letzter_schreibfehler"].Text);
        Assert.Equal(20, byKey["adresse_des_letzten_schreibzugriffs"].Value);

        // Umschlag-Register und Reset-Befehl sind keine Messwerte
        int[] envelope = [0, 1, 8, 10, 11, 12, 13, 17];
        Assert.DoesNotContain(message.Measurements, m => envelope.Contains(m.Address));
    }

    [Theory]
    [InlineData(TimeStatus.Synced, true, "good", "running")]
    [InlineData(TimeStatus.Synced, false, "good", "paused")]
    [InlineData(TimeStatus.Standalone, true, "uncertain", "standalone")]
    [InlineData(TimeStatus.MasterLost, false, "bad", "masterLost")]
    [InlineData(TimeStatus.NoTime, false, "bad", "noTime")]
    public void Quality_FollowsTimeStatusAndRunningFlag(TimeStatus status, bool running, string quality, string reason)
    {
        var message = Build(ChargerRegisters(status, running));

        Assert.Equal((quality, reason), (message.Quality, message.QualityReason));
    }

    [Fact]
    public void Quality_PausedKeepsPublishing_WithSameSimTime()
    {
        var tracker = new DeviceTracker();
        var first = Build(ChargerRegisters(running: false), tracker, Now);
        var later = Build(ChargerRegisters(running: false), tracker, Now.AddMinutes(10));

        Assert.Equal("paused", later.QualityReason);                        // Pause ist nie „stale“
        Assert.Equal(first.SimTime, later.SimTime);
        Assert.Equal(2, later.PublishSeq);
    }

    [Fact]
    public void Quality_Stale_WhenRunningButSequenceStandsLongerThanModelLimit()
    {
        var tracker = new DeviceTracker();
        Build(ChargerRegisters(speed: 600), tracker, Now);

        // 5 s real × 600 = 50 min Modellzeit < 60 min: noch gut
        Assert.Equal("running", Build(ChargerRegisters(speed: 600), tracker, Now.AddSeconds(5)).QualityReason);
        // 7 s real × 600 = 70 min Modellzeit ohne neue Sequenz: Gerät hängt
        Assert.Equal("stale", Build(ChargerRegisters(speed: 600), tracker, Now.AddSeconds(7)).QualityReason);
        // neue Sequenz: wieder gut
        Assert.Equal("running", Build(ChargerRegisters(speed: 600, seq: 413), tracker, Now.AddSeconds(8)).QualityReason);
    }

    [Fact]
    public void Quality_NotStale_AtSpeedOne_WhenDeviceStepIsMinutes()
    {
        var tracker = new DeviceTracker();
        Build(ChargerRegisters(speed: 1), tracker, Now);

        // Tempo 1, Schrittweite 1 min: 50 s ohne neue Sequenz sind normal
        Assert.Equal("running", Build(ChargerRegisters(speed: 1), tracker, Now.AddSeconds(50)).QualityReason);
    }

    [Fact]
    public void Restart_NewRunWithSameSequence_CountsAsChange()
    {
        var tracker = new DeviceTracker();
        Build(ChargerRegisters(speed: 600), tracker, Now);

        var restarted = Build(ChargerRegisters(speed: 600, runId: 2, simTime: new DateTime(2026, 7, 1)), tracker, Now.AddSeconds(30));

        Assert.Equal("running", restarted.QualityReason);
        Assert.Equal(2, restarted.RunId);
        Assert.Equal("2026-07-01T00:00:00", restarted.SimTime);
    }

    [Fact]
    public void WrongDevice_IsDecodeError_AndNoMessage()
    {
        var wrongType = MessageBuilder.Build(Charger, ChargerRegisters(type: DeviceType.PvPlant), Now, new DeviceTracker(), QualitySettings.Default);
        var wrongNumber = MessageBuilder.Build(Charger, ChargerRegisters(instance: 2), Now, new DeviceTracker(), QualitySettings.Default);
        var unknown = MessageBuilder.Build(Charger, ChargerRegisters(type: (DeviceType)99), Now, new DeviceTracker(), QualitySettings.Default);

        Assert.Null(wrongType.Message);
        Assert.Contains("erwartet Charger", wrongType.Error);
        Assert.Null(wrongNumber.Message);
        Assert.Contains("Geräte-Nummer 1", wrongNumber.Error);
        Assert.Null(unknown.Message);
        Assert.Contains("Unbekannter Gerätetyp 99", unknown.Error);
    }

    [Fact]
    public void NonFiniteRegister_BecomesNull_AndQualityUncertain()
    {
        var registers = ChargerRegisters();
        SetFloat(registers, CommonRegisters.ActivePower, float.NaN);

        var message = Build(registers);

        Assert.Null(message.Measurements.Single(m => m.Key == "wirkleistung").Value);
        Assert.Equal(("uncertain", "invalidValue"), (message.Quality, message.QualityReason));
        Assert.Contains("\"value\":null", MessageJson.Serialize(message));   // gültiges JSON trotz NaN
    }

    [Fact]
    public void Json_IsCamelCase_KeepsUmlautsAndUnits_AndOmitsMissingText()
    {
        string json = MessageJson.Serialize(Build(ChargerRegisters()));
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("DC-01", root.GetProperty("deviceId").GetString());
        Assert.Equal("2026-01-15T06:00:00", root.GetProperty("simTime").GetString());
        Assert.Equal(412, root.GetProperty("seq").GetInt32());
        Assert.Contains("\"unit\":\"°C\"", json);
        Assert.Contains("Leistungsgrenze der Säule", json);
        var power = root.GetProperty("measurements").EnumerateArray().First(m => m.GetProperty("key").GetString() == "wirkleistung");
        Assert.False(power.TryGetProperty("text", out _));
    }
}
