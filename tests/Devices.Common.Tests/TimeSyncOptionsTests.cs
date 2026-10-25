namespace Devices.Common.Tests;

public class TimeSyncOptionsTests
{
    private static TimeSyncOptions Apply(params string[] args)
    {
        var options = new TimeSyncOptions();
        for (int i = 0; i < args.Length; i++)
        {
            int index = i;
            Assert.True(TimeSyncOptions.TryApply(args[i], () => args[++index], options, out options), args[i]);
            i = index;
        }
        return options;
    }

    [Fact]
    public void Defaults_UseLocalMaster()
    {
        var options = new TimeSyncOptions();

        Assert.False(options.Standalone);
        Assert.Equal("127.0.0.1:5100", options.MasterEndpoint);
    }

    [Theory]
    [InlineData("192.168.1.20:5200", "192.168.1.20", 5200)]
    [InlineData("master-pc", "master-pc", 5100)]
    public void Master_ParsesHostAndPort(string text, string host, int port)
    {
        var options = Apply("--master", text);

        Assert.Equal(host, options.MasterHost);
        Assert.Equal(port, options.MasterPort);
    }

    [Theory]
    [InlineData("host:0")]
    [InlineData("host:70000")]
    [InlineData("host:abc")]
    [InlineData(":5100")]
    public void Master_InvalidEndpoint_IsRejected(string text)
    {
        Assert.Throws<ArgumentException>(() => Apply("--master", text));
    }

    [Fact]
    public void Standalone_TakesStartAndSpeed_WithCommaOrDot()
    {
        var options = Apply("--standalone", "--start", "2026-01-15 06:00", "--speed", "2,5");

        Assert.True(options.Standalone);
        Assert.True(options.HasStandaloneSettings);
        Assert.Equal(new DateTime(2026, 1, 15, 6, 0, 0), options.StandaloneStart);
        Assert.Equal(2.5, options.StandaloneSpeed);
    }

    [Theory]
    [InlineData("--speed", "0")]
    [InlineData("--speed", "100001")]
    [InlineData("--speed", "schnell")]
    [InlineData("--start", "15.01.2026")]
    public void InvalidStandaloneValues_AreRejected(string name, string value)
    {
        Assert.Throws<ArgumentException>(() => Apply(name, value));
    }

    [Fact]
    public void OtherOptions_AreNotConsumed()
    {
        var options = new TimeSyncOptions();

        Assert.False(TimeSyncOptions.TryApply("--power", () => "150", options, out var updated));
        Assert.Same(options, updated);
    }
}
