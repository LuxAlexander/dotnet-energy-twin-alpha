namespace ModbusRabbitGateway.Tests;

public sealed class GatewayOptionsTests
{
    [Fact]
    public void Defaults_AreTheFourDemoDevices()
    {
        var options = GatewayOptions.Parse([]);

        Assert.Equal(["PV-01", "BAT-01", "DC-01", "KH-01"], options.Devices.Select(d => d.DeviceId));
        Assert.Equal([1502, 1503, 1504, 1505], options.Devices.Select(d => d.Port));
        Assert.Equal(TimeSpan.FromSeconds(1), options.Interval);
        Assert.Equal(TimeSpan.FromSeconds(3), options.Quality.StaleMinimumReal);
    }

    [Fact]
    public void Device_ReplacesDefaults_WithHostPortAndUnit()
    {
        var options = GatewayOptions.Parse(["--device", "DC-02=192.168.1.5:1510/3", "--interval", "0,5", "--stale-after", "30"]);

        var device = Assert.Single(options.Devices);
        Assert.Equal(("DC-02", "192.168.1.5", 1510, (byte)3), (device.DeviceId, device.Host, device.Port, device.UnitId));
        Assert.Equal(Modbus.Contracts.DeviceType.Charger, device.ExpectedType);
        Assert.Equal(2, device.ExpectedInstance);
        Assert.Equal(TimeSpan.FromMilliseconds(500), options.Interval);
        Assert.Equal(TimeSpan.FromMinutes(30), options.StaleModelTime);
    }

    [Fact]
    public void UnknownPrefix_SkipsTypeCheck()
    {
        var device = DeviceEndpoint.Parse("X9=127.0.0.1:1600");

        Assert.Null(device.ExpectedType);
        Assert.Equal(9, device.ExpectedInstance);
    }

    [Theory]
    [InlineData("--device", "DC 01=127.0.0.1:1504")]   // Leerzeichen im Topic
    [InlineData("--device", "DC-01/x=127.0.0.1:1504")]
    [InlineData("--device", "DC-01=127.0.0.1")]
    [InlineData("--device", "DC-01=127.0.0.1:99999")]
    [InlineData("--device", "DC-01=127.0.0.1:1504/0")]
    [InlineData("--interval", "0,01")]
    [InlineData("--timeout", "abc")]
    [InlineData("--gateway-id", "GW/1")]
    [InlineData("--unbekannt", "1")]
    public void InvalidValues_AreRejectedWithGermanMessage(string option, string value)
    {
        var ex = Assert.Throws<ArgumentException>(() => GatewayOptions.Parse([option, value]));
        Assert.False(string.IsNullOrWhiteSpace(ex.Message));
    }

    [Fact]
    public void DuplicateDeviceId_IsRejected() =>
        Assert.Throws<ArgumentException>(() =>
            GatewayOptions.Parse(["--device", "DC-01=127.0.0.1:1504", "--device", "dc-01=127.0.0.1:1505"]));
}
