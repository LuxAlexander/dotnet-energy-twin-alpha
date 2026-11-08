namespace ModbusRabbitGateway.Tests;

/// <summary>Zugangsdaten: Reihenfolge der Quellen, .env-Format, Passwort nie in Anzeige oder Kommandozeile.</summary>
public sealed class MqttCredentialsTests : IDisposable
{
    private readonly string _envFile = Path.Combine(Path.GetTempPath(), $"gateway-test-{Guid.NewGuid():N}.env");

    public MqttCredentialsTests() =>
        File.WriteAllLines(_envFile,
        [
            "# Beispiel wie docker/.env",
            "RABBITMQ_DEFAULT_USER=dateiuser",
            "RABBITMQ_DEFAULT_PASS=\"geheim aus datei\"",
            "",
            "UNWICHTIG = 1",
        ]);

    public void Dispose() => File.Delete(_envFile);

    private static Func<string, string?> Env(params (string Name, string Value)[] values) =>
        name => values.FirstOrDefault(v => v.Name == name).Value;

    [Fact]
    public void EnvFile_IsUsedWhenNoEnvironmentVariables()
    {
        var c = MqttCredentials.Resolve(null, _envFile, Env());

        Assert.Equal(("dateiuser", "geheim aus datei"), (c.User, c.Password));
        Assert.Equal(_envFile, c.Source);
    }

    [Fact]
    public void EnvironmentVariables_WinOverFile_AndOptionWinsForUser()
    {
        var env = Env(("ENERGYTWIN_MQTT_USER", "envuser"), ("ENERGYTWIN_MQTT_PASSWORD", "envpass"));

        Assert.Equal(("envuser", "envpass"), (MqttCredentials.Resolve(null, _envFile, env).User, MqttCredentials.Resolve(null, _envFile, env).Password));
        Assert.Equal("optuser", MqttCredentials.Resolve("optuser", _envFile, env).User);
    }

    [Fact]
    public void Nothing_GivesAnonymous()
    {
        var c = MqttCredentials.Resolve(null, Path.Combine(Path.GetTempPath(), "gibt-es-nicht.env"), Env());

        Assert.Null(c.User);
        Assert.Null(c.Password);
        Assert.Equal("anonym", c.ToString());
    }

    [Fact]
    public void ToString_NeverShowsPassword()
    {
        var c = MqttCredentials.Resolve(null, _envFile, Env());

        Assert.DoesNotContain("geheim", c.ToString());
        Assert.Contains("dateiuser", c.ToString());
    }

    [Fact]
    public void PasswordOption_IsRejected()
    {
        var ex = Assert.Throws<ArgumentException>(() => GatewayOptions.Parse(["--mqtt-password", "x"]));
        Assert.Contains("ENERGYTWIN_MQTT_PASSWORD", ex.Message);
    }

    [Fact]
    public void BrokerAndConsoleOptions_AreParsed()
    {
        var options = GatewayOptions.Parse(["--broker", "10.0.0.5:1884", "--console", "--mqtt-user", "u", "--env-file", _envFile]);

        Assert.Equal(("10.0.0.5", 1884, true, "u"), (options.BrokerHost, options.BrokerPort, options.ConsoleOnly, options.MqttUser));
        Assert.Equal(_envFile, options.EnvFile);
        Assert.Throws<ArgumentException>(() => GatewayOptions.Parse(["--broker", "nurhost"]));
    }
}
