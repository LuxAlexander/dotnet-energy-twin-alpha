namespace ModbusRabbitGateway;

/// <summary>Kompatible Gateway-Fassade für die gemeinsame Zugangsdatenauflösung in Shared.Config.</summary>
public sealed record MqttCredentials(string? User, string? Password, string Source)
{
    public const string UserVariable = Shared.Config.MqttCredentials.UserVariable;
    public const string PasswordVariable = Shared.Config.MqttCredentials.PasswordVariable;
    public const string FileUserKey = Shared.Config.MqttCredentials.FileUserKey;
    public const string FilePasswordKey = Shared.Config.MqttCredentials.FilePasswordKey;

    public static MqttCredentials Resolve(string? userOption, string? envFile, Func<string, string?> environment)
    {
        var result = Shared.Config.MqttCredentials.Resolve(userOption, envFile, environment);
        return new(result.User, result.Password, result.Source);
    }

    public static Dictionary<string, string> ReadEnvFile(string path) => Shared.Config.MqttCredentials.ReadEnvFile(path);
    public override string ToString() => User is null ? "anonym" : $"Benutzer {User} (Passwort aus {Source})";
}
