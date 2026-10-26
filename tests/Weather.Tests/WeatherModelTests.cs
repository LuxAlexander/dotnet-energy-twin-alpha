using SimCore.Contracts;
using WeatherSimulator.Simulation;

namespace Weather.Tests;

public class WeatherModelTests
{
    private static WeatherModel NewModel() => new(new ManualWeather());

    [Fact]
    public void SameLocationTimeAndRun_ReturnsIdenticalWeather()
    {
        var model = NewModel();
        var time = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);

        var first = model.Get("salzburg", time, 1);
        var second = model.Get("salzburg", time, 1);

        Assert.Equal(first, second);
    }

    [Fact]
    public void MinuteWithinHour_DoesNotChangeWeather()
    {
        var model = NewModel();
        var hour = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);

        var atStart = model.Get("salzburg", hour, 1);
        var atEnd = model.Get("salzburg", hour.AddMinutes(59), 1);

        Assert.Equal(atStart, atEnd);
    }

    [Fact]
    public void DifferentLocations_GiveDifferentWeather()
    {
        var model = NewModel();
        var time = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);

        var salzburg = model.Get("salzburg", time, 1);
        var innsbruck = model.Get("innsbruck", time, 1);

        Assert.NotEqual(salzburg, innsbruck);
        Assert.Equal("salzburg", salzburg.Location);
        Assert.Equal("innsbruck", innsbruck.Location);
    }

    [Fact]
    public void UnknownLocation_Throws()
    {
        var model = NewModel();

        Assert.Throws<KeyNotFoundException>(() =>
            model.Get("graz", new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc), 1));
    }

    [Fact]
    public void Night_HasNoIrradiationAndNightCondition()
    {
        var model = NewModel();

        var weather = model.Get("salzburg", new DateTime(2026, 12, 21, 1, 0, 0, DateTimeKind.Utc), 1);

        Assert.Equal(0, weather.IrradiationWm2);
        Assert.Equal(WeatherCondition.Night, weather.Condition);
        Assert.True(weather.SunHeightDegrees <= 0);
    }

    [Fact]
    public void SummerNoon_IsHigherThanWinterNoon()
    {
        var model = NewModel();

        var summer = model.Get("salzburg", new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc), 1);
        var winter = model.Get("salzburg", new DateTime(2026, 12, 21, 12, 0, 0, DateTimeKind.Utc), 1);

        Assert.True(summer.SunHeightDegrees > winter.SunHeightDegrees);
        Assert.True(summer.IrradiationWm2 > winter.IrradiationWm2);
    }

    [Fact]
    public void ManualOverride_AppliesToAllLocations()
    {
        var manual = new ManualWeather();
        var model = new WeatherModel(manual);
        var time = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);

        manual.SetTemperature(-12);
        manual.SetWind(0);

        var salzburg = model.Get("salzburg", time, 1);
        var wien = model.Get("wien", time, 1);

        Assert.Equal(-12, salzburg.TemperatureC);
        Assert.Equal(-12, wien.TemperatureC);
        Assert.Equal(0, salzburg.WindSpeedMs);
    }

    [Fact]
    public void RainPreset_ProducesPrecipitation()
    {
        var manual = new ManualWeather();
        var model = new WeatherModel(manual);

        manual.SetRain();
        var weather = model.Get("salzburg", new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc), 1);

        Assert.Equal(3.5, weather.PrecipitationMmPerHour);
        Assert.True(weather.CloudCoverPercent >= 90);
    }

    [Fact]
    public void Clear_RemovesManualOverride()
    {
        var manual = new ManualWeather();
        var model = new WeatherModel(manual);
        var time = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
        var before = model.Get("salzburg", time, 1);

        manual.SetTemperature(40);
        manual.Clear();

        Assert.Equal(before, model.Get("salzburg", time, 1));
    }

    [Fact]
    public void DifferentRuns_GiveDifferentTemperature()
    {
        var model = NewModel();
        var time = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);

        var run1 = model.Get("salzburg", time, 1);
        var run2 = model.Get("salzburg", time, 2);

        Assert.NotEqual(run1.TemperatureC, run2.TemperatureC);
    }

    [Fact]
    public void Values_AreInPlausibleRanges()
    {
        var model = NewModel();

        for (int day = 0; day < 365; day += 17)
        {
            var time = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(day);
            var weather = model.Get("salzburg", time, 7);

            Assert.InRange(weather.CloudCoverPercent, 0, 100);
            Assert.InRange(weather.IrradiationWm2, 0, 1200);
            Assert.InRange(weather.TemperatureC, -35, 45);
            Assert.InRange(weather.WindSpeedMs, 0, 60);
            Assert.InRange(weather.PrecipitationMmPerHour, 0, 30);
            Assert.InRange(weather.SunHeightDegrees, -90, 90);
        }
    }
}
