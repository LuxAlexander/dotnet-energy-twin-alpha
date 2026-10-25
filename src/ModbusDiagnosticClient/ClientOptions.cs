using System.Globalization;

namespace ModbusDiagnosticClient;

/// <summary>Startoptionen des Diagnose-Clients (Host, Port, Unit-ID, Zeitlimit, Einmal-Modus).</summary>
public sealed record ClientOptions
{
    public const string Usage = """
        Aufruf: dotnet run --project src/ModbusDiagnosticClient -- [Optionen]

          --host <name|ip>      Host des Geräts                    (Standard: 127.0.0.1)
          --port <port>         Modbus-TCP-Port des Geräts         (Standard: 1502)
          --unit-id <1-247>     Modbus-Unit-ID                     (Standard: 1)
          --timeout <ms>        Zeitlimit für Verbinden und Antwort (Standard: 3000)
          --once                Register einmal lesen und beenden (ohne Eingabeaufforderung)
          --write <ziel>=<wert> Wert schreiben, dann beenden (mehrfach möglich); Ziel = Adresse oder Registername
          --help                Diese Hilfe anzeigen

        Ohne --once und --write startet eine Eingabeaufforderung (Befehl "help" zeigt alle Befehle).
        Exit-Code: 0 ok, 1 Verbindungs- oder Protokollfehler, 2 Schreibzugriff vom Gerät abgelehnt, 64 falscher Aufruf.

        Beispiele:
          dotnet run --project src/ModbusDiagnosticClient -- --port 1502
          dotnet run --project src/ModbusDiagnosticClient -- --port 1503 --once
          dotnet run --project src/ModbusDiagnosticClient -- --port 1502 --write "Leistungsgrenze=200"
        """;

    public string Host { get; init; } = "127.0.0.1";
    public int Port { get; init; } = 1502;
    public byte UnitId { get; init; } = 1;
    public int TimeoutMs { get; init; } = 3000;
    public bool Once { get; init; }
    public IReadOnlyList<(string Target, string Value)> Writes { get; init; } = [];

    /// <summary>Läuft ohne Eingabeaufforderung (<c>--once</c> oder <c>--write</c>)?</summary>
    public bool IsOneShot => Once || Writes.Count > 0;

    /// <exception cref="ArgumentException">Ungültige oder unbekannte Option.</exception>
    public static ClientOptions Parse(string[] args)
    {
        var options = new ClientOptions();
        var writes = new List<(string, string)>();

        for (int i = 0; i < args.Length; i++)
        {
            string name = args[i].ToLowerInvariant();
            switch (name)
            {
                case "--host":
                    string host = NextValue(args, ref i, name);
                    if (string.IsNullOrWhiteSpace(host))
                        throw new ArgumentException("--host darf nicht leer sein.");
                    options = options with { Host = host };
                    break;
                case "--port":
                    options = options with { Port = ParseInt(NextValue(args, ref i, name), name, 1, 65535) };
                    break;
                case "--unit-id":
                    options = options with { UnitId = (byte)ParseInt(NextValue(args, ref i, name), name, 1, 247) };
                    break;
                case "--timeout":
                    options = options with { TimeoutMs = ParseInt(NextValue(args, ref i, name), name, 100, 60000) };
                    break;
                case "--once":
                    options = options with { Once = true };
                    break;
                case "--write":
                    string pair = NextValue(args, ref i, name);
                    int equals = pair.LastIndexOf('=');
                    if (equals <= 0 || equals == pair.Length - 1)
                        throw new ArgumentException($"--write erwartet <ziel>=<wert>, nicht '{pair}'.");
                    writes.Add((pair[..equals].Trim(), pair[(equals + 1)..].Trim()));
                    break;
                default:
                    throw new ArgumentException($"Unbekanntes Argument: '{args[i]}'.");
            }
        }

        return options with { Writes = writes };
    }

    public static bool IsHelpRequested(string[] args) => args.Any(a => a is "--help" or "-h" or "/?");

    private static string NextValue(string[] args, ref int i, string name)
    {
        if (i + 1 >= args.Length)
            throw new ArgumentException($"Für {name} fehlt ein Wert.");
        return args[++i];
    }

    private static int ParseInt(string text, string name, int min, int max)
    {
        if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out int value) || value < min || value > max)
            throw new ArgumentException($"{name} muss eine Zahl von {min} bis {max} sein (angegeben: '{text}').");
        return value;
    }
}
