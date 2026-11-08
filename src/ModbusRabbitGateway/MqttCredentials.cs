namespace ModbusRabbitGateway;

/// <summary>
/// Zugangsdaten für den Broker. Das Passwort steht nie im Repo und nie auf der Kommandozeile (wäre in der Prozessliste
/// sichtbar). Reihenfolge: Option <c>--mqtt-user</c> (nur Benutzer) → Umgebungsvariablen
/// <c>ENERGYTWIN_MQTT_USER</c>/<c>ENERGYTWIN_MQTT_PASSWORD</c> → <c>.env</c>-Datei (Standard <c>docker/.env</c>, dieselbe
/// Datei, aus der die Compose-Datei <c>RABBITMQ_DEFAULT_USER</c>/<c>RABBITMQ_DEFAULT_PASS</c> liest).
/// </summary>
/// <param name="Source">Woher die Daten stammen, für die Anzeige (ohne Passwort).</param>
public sealed record MqttCredentials(string? User, string? Password, string Source)
{
    public const string UserVariable = "ENERGYTWIN_MQTT_USER";
    public const string PasswordVariable = "ENERGYTWIN_MQTT_PASSWORD";
    public const string FileUserKey = "RABBITMQ_DEFAULT_USER";
    public const string FilePasswordKey = "RABBITMQ_DEFAULT_PASS";

    public static MqttCredentials Resolve(string? userOption, string? envFile, Func<string, string?> environment)
    {
        var file = envFile is not null && File.Exists(envFile) ? ReadEnvFile(envFile) : new Dictionary<string, string>();
        string? Env(string name) => environment(name) is { Length: > 0 } value ? value : null;
        string? FromFile(string key) => file.TryGetValue(key, out var value) && value.Length > 0 ? value : null;

        string? user = userOption ?? Env(UserVariable) ?? FromFile(FileUserKey);
        string? password = Env(PasswordVariable) ?? FromFile(FilePasswordKey);

        string source = (Env(PasswordVariable), FromFile(FilePasswordKey)) switch
        {
            ({ }, _) => $"Umgebungsvariable {PasswordVariable}",
            (null, { }) => envFile!,
            _ => "keine (anonym)",
        };
        return new MqttCredentials(user, password, source);
    }

    /// <summary>Einfaches <c>.env</c>-Format: <c>SCHLÜSSEL=Wert</c> je Zeile, <c>#</c> = Kommentar, Anführungszeichen werden entfernt.</summary>
    public static Dictionary<string, string> ReadEnvFile(string path)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (string raw in File.ReadLines(path))
        {
            string line = raw.Trim();
            int equals = line.IndexOf('=');
            if (line.Length == 0 || line.StartsWith('#') || equals <= 0)
                continue;

            string value = line[(equals + 1)..].Trim();
            if (value.Length >= 2 && (value[0] == '"' || value[0] == '\'') && value[^1] == value[0])
                value = value[1..^1];
            values[line[..equals].Trim()] = value;
        }
        return values;
    }

    public override string ToString() => User is null ? "anonym" : $"Benutzer {User} (Passwort aus {Source})";
}
