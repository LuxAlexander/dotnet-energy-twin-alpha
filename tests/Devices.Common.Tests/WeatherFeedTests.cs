using Devices.Common.Weather;
using SimCore.Contracts;

namespace Devices.Common.Tests;

public class WeatherFeedTests
{
    private sealed class FakeWeatherService : IWeatherService
    {
        public int Calls { get; private set; }
        public bool Fail { get; set; }

        public Task<WeatherData> GetAsync(string location, DateTime simTimeUtc, int runId, CancellationToken cancellationToken)
        {
            Calls++;
            if (Fail)
                throw new WeatherUnavailableException("NICHT ERREICHBAR");
            return Task.FromResult(new WeatherData(
                location, simTimeUtc, runId, 10, 20, 7.5, 0, 2, 100, WeatherCondition.Sunny));
        }
    }

    private static DateTime Hour(int hour) => new(2026, 6, 1, hour, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task SameHour_FetchesOnlyOnce()
    {
        var service = new FakeWeatherService();
        using var feed = new WeatherFeed(service, "salzburg");

        await feed.UpdateAsync(Hour(12).AddMinutes(5), 1, CancellationToken.None);
        await feed.UpdateAsync(Hour(12).AddMinutes(55), 1, CancellationToken.None);

        Assert.Equal(1, service.Calls);
        Assert.Equal(7.5, feed.Last!.TemperatureC);
    }

    [Fact]
    public async Task NextHourAndOtherRun_FetchAgain()
    {
        var service = new FakeWeatherService();
        using var feed = new WeatherFeed(service, "salzburg");

        await feed.UpdateAsync(Hour(12), 1, CancellationToken.None);
        await feed.UpdateAsync(Hour(13), 1, CancellationToken.None);
        await feed.UpdateAsync(Hour(13), 2, CancellationToken.None);

        Assert.Equal(3, service.Calls);
        Assert.Equal(7.5, feed.Last!.TemperatureC);
    }

    [Fact]
    public async Task Failure_KeepsLastWeatherAndUpdatesStatus()
    {
        var service = new FakeWeatherService();
        using var feed = new WeatherFeed(service, "salzburg");
        await feed.UpdateAsync(Hour(12), 1, CancellationToken.None);

        service.Fail = true;
        var last = await feed.UpdateAsync(Hour(13), 1, CancellationToken.None);

        Assert.Equal(7.5, last!.TemperatureC);
        Assert.Contains("Fehlversuch", feed.Status);

        // Innerhalb der 10-s-Wartezeit wird nicht erneut gefragt.
        await feed.UpdateAsync(Hour(14), 1, CancellationToken.None);
        Assert.Equal(2, service.Calls);
    }

    [Fact]
    public void Options_MasterDefaultsToWeatherSimulator()
    {
        var options = new WeatherOptions().WithDefaultUrl(standalone: false);

        Assert.Equal(WeatherOptions.DefaultUrl, options.Url);
        Assert.Equal("salzburg", options.Location);
    }

    [Fact]
    public void Options_StandaloneHasNoWeather()
    {
        var options = new WeatherOptions().WithDefaultUrl(standalone: true);

        Assert.Null(options.Url);
    }

    [Fact]
    public void Options_OffStaysOffWithMaster()
    {
        var options = new WeatherOptions();
        Assert.True(WeatherOptions.TryApply("--weather", () => "off", options, out options));

        Assert.Null(options.WithDefaultUrl(standalone: false).Url);
        Assert.True(options.Specified);
    }

    [Fact]
    public void Options_LocationIsLowercased()
    {
        var options = new WeatherOptions();
        Assert.True(WeatherOptions.TryApply("--location", () => "Wien", options, out options));

        Assert.Equal("wien", options.Location);
    }
}
