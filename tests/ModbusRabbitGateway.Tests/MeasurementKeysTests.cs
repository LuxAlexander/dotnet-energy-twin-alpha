using Modbus.Contracts;

namespace ModbusRabbitGateway.Tests;

/// <summary>Stabilität der Messwert-Schlüssel (später InfluxDB-Feldnamen).</summary>
public sealed class MeasurementKeysTests
{
    [Theory]
    [InlineData("Wirkleistung", "wirkleistung")]
    [InlineData("Punkt 1: SOC", "punkt_1_soc")]
    [InlineData("Ein/Aus", "ein_aus")]
    [InlineData("Ladezustand (SOC)", "ladezustand_soc")]
    [InlineData("Max. Ladeleistung", "max_ladeleistung")]
    [InlineData("Außentemperatur", "aussentemperatur")]
    [InlineData("Kälteleistung", "kaelteleistung")]
    [InlineData("Tür", "tuer")]
    [InlineData("  SOC-Minimum ", "soc_minimum")]
    public void From_MakesAsciiSnakeCase(string name, string expected) =>
        Assert.Equal(expected, MeasurementKeys.From(name));

    [Fact]
    public void Keys_AreUniqueAndAsciiSafe_InEveryPlan()
    {
        foreach (var type in Enum.GetValues<DeviceType>())
        {
            if (RegisterPlans.For(type) is not { } plan)
                continue;
            var keys = MessageBuilder.MeasuredRegisters(plan).Select(d => MeasurementKeys.From(d.Name)).ToList();

            Assert.Equal(keys.Count, keys.Distinct().Count());
            Assert.All(keys, k => Assert.Matches("^[a-z0-9]+(_[a-z0-9]+)*$", k));
        }
    }

    /// <summary>
    /// Benennt jemand ein Register um oder fügt eines hinzu, ändern sich Schlüssel und damit spätere Zeitreihen.
    /// Dann diesen Test bewusst anpassen: <c>dotnet run --project src/ModbusRabbitGateway -- --keys</c> und die Ausgabe
    /// nach <c>tests/ModbusRabbitGateway.Tests/measurement-keys.txt</c> übernehmen.
    /// </summary>
    [Fact]
    public void Keys_MatchTheRecordedCatalog()
    {
        var expected = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "measurement-keys.txt"))
            .Where(l => l.Length > 0 && !l.StartsWith('#'));
        var actual = Enum.GetValues<DeviceType>()
            .Select(RegisterPlans.For)
            .OfType<RegisterPlan>()
            .SelectMany(KeyCatalog.For);

        Assert.Equal(expected, actual);
    }
}
