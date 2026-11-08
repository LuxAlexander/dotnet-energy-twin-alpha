using SimCore.Contracts;
using WeatherSimulator.Publishing;
using WeatherSimulator.Simulation;

namespace Weather.Tests;

public class RegionalWeatherTests
{
    [Theory]
    [InlineData("tamanrasset")]
    [InlineData("neumayer")]
    public void RegionalWeather_IsReproducibleWithinHour(string location)
    {
        var time = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
        var first = new WeatherModel(new ManualWeather()).Get(location, time, 7);
        var second = new WeatherModel(new ManualWeather()).Get(location, time.AddMinutes(59), 7);
        Assert.Equal(first, second);
        Assert.InRange(first.CloudCoverPercent, 0, 100);
        Assert.True(double.IsFinite(first.TemperatureC));
    }

    [Fact]
    public void Neumayer_HasPolarNightInJuneAndPolarDayInDecember()
    {
        var model = new WeatherModel(new ManualWeather());
        for (int hour = 0; hour < 24; hour++)
        {
            Assert.Equal(0, model.Get("neumayer", new DateTime(2026, 6, 21, hour, 0, 0, DateTimeKind.Utc), 1).IrradiationWm2);
            Assert.True(WeatherModel.SunHeightDegrees(new DateTime(2026, 12, 21, hour, 0, 0, DateTimeKind.Utc), -70.667, -8.267) > 0);
        }
    }

    [Fact]
    public void PolarNight_DoesNotHideSnow()
    {
        var manual = new ManualWeather();
        manual.Select("neumayer");
        manual.ApplyPreset(WeatherCondition.Snow);
        var model = new WeatherModel(manual);
        Assert.Equal(WeatherCondition.Snow, model.Get("neumayer", new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc), 1).Condition);
        Assert.False(manual.Get("salzburg").IsActive);
    }

    [Fact]
    public void ClearAll_ResetsEveryLocationAndUpdatesRevision()
    {
        var manual = new ManualWeather();
        manual.SetTemperature(35);
        long before = manual.Get("salzburg").Revision;
        manual.Select("neumayer");
        manual.SetWind(20);
        manual.ClearAll();
        Assert.All(LocationCatalog.All, location => Assert.False(manual.Get(location.Key).IsActive));
        Assert.True(manual.Get("salzburg").Revision > before);
    }

    [Fact]
    public void Options_RequireExplicitMqttActivation()
    {
        Assert.False(WeatherOptions.Parse([]).Mqtt);
        Assert.True(WeatherOptions.Parse(["--mqtt"]).Mqtt);
        Assert.Throws<ArgumentException>(() => WeatherOptions.Parse(["--broker", "localhost"]));
    }

    [Fact]
    public void ManualValues_RejectNonFiniteInput()
    {
        var manual = new ManualWeather();
        Assert.Throws<ArgumentOutOfRangeException>(() => manual.SetTemperature(double.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() => manual.SetWind(double.PositiveInfinity));
    }
}
