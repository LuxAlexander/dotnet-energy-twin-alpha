using System.Globalization;
using Modbus.Contracts;

namespace ModbusDiagnosticClient;

/// <summary>Ergebnis eines Schreibzugriffs, nachdem das Gerät befragt wurde.</summary>
public sealed record WriteOutcome(bool Accepted, WriteError Error, int ErrorAddress, string Message);

/// <summary>
/// Befehle des Diagnose-Clients: Register lesen und dekodieren, Sollwerte schreiben und das Ergebnis beim Gerät nachprüfen,
/// Verlauf beobachten. Ausgabe über einen <see cref="TextWriter"/> (testbar).
/// </summary>
public sealed class DiagnosticSession
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");
    private static readonly TimeSpan WriteSettleTime = TimeSpan.FromMilliseconds(200);

    public const string Help = """
        Befehle:
          read                 Registerkopf und gerätespezifische Register lesen (mit Rohwerten)
          header               nur den gemeinsamen Registerkopf lesen
          watch [anzahl] [s]   Verlauf beobachten (Standard: bis Strg+C, alle 1 s); überlebt Verbindungsabbrüche
          write <ziel> <wert>  Wert schreiben; Ziel = Adresse oder (Teil des) Registernamens, Wert Zahl oder Klartext (ein/aus)
          force <ziel> <wert>  wie write, aber ohne Prüfung im Client – zum Testen, ob das Gerät Ungültiges ablehnt
          on | off | reset     Kurzbefehle für Ein/Aus (Adresse 16) und Fehler zurücksetzen (Adresse 17)
          raw <adresse> <n>    n Register ab Adresse roh lesen (dezimal und hex)
          info                 Verbindung und Registerplan anzeigen
          connect <host[:port]> [unit]   mit einem anderen Gerät verbinden
          help | quit
        """;

    private readonly TextWriter _out;
    private RegisterPlan? _plan;
    private DeviceType? _deviceType;

    public DiagnosticSession(DeviceConnection connection, TextWriter output)
    {
        Connection = connection;
        _out = output;
    }

    public DeviceConnection Connection { get; private set; }

    /// <summary>Registerplan des Geräts, sobald der Gerätetyp gelesen wurde; <c>null</c> für Geräte ohne eigenen Plan.</summary>
    public RegisterPlan? Plan => _plan;

    /// <summary>Liest den Kopf, bestimmt den Gerätetyp und wählt den Registerplan.</summary>
    /// <exception cref="DiagnosticException">Verbindungs- oder Protokollfehler.</exception>
    public ushort[] ReadHeader()
    {
        var header = Connection.ReadRegisters(0, CommonRegisters.SpecificStart);
        var type = (DeviceType)header[CommonRegisters.DeviceType];
        if (_deviceType != type)
        {
            _deviceType = type;
            _plan = Enum.IsDefined(type) ? RegisterPlans.For(type) : null;
        }
        return header;
    }

    /// <summary>Liest Kopf und gerätespezifische Register in einem Registerabbild (Index = Protokolladresse).</summary>
    public ushort[] ReadAll()
    {
        var header = ReadHeader();
        int end = _plan?.Definitions.Max(d => d.EndAddress) ?? CommonRegisters.SpecificStart;
        if (end <= CommonRegisters.SpecificStart)
            return header;

        var specific = Connection.ReadRegisters(CommonRegisters.SpecificStart, end - CommonRegisters.SpecificStart);
        return [.. header, .. specific];
    }

    public string DescribeDevice(IReadOnlyList<ushort> header)
    {
        var type = (DeviceType)header[CommonRegisters.DeviceType];
        string name = Enum.IsDefined(type) ? type.ToString() : $"unbekannter Typ {header[CommonRegisters.DeviceType]}";
        return $"{name} Nr. {header[CommonRegisters.DeviceInstance]} an {Connection.Endpoint}";
    }

    /// <summary>Führt eine Eingabezeile aus.</summary>
    /// <returns><c>false</c> bei <c>quit</c>.</returns>
    public async Task<bool> ExecuteAsync(string line, CancellationToken cancellationToken)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
            return true;

        try
        {
            switch (parts[0].ToLowerInvariant())
            {
                case "quit" or "q" or "exit":
                    return false;
                case "help" or "h" or "?":
                    await _out.WriteLineAsync(Help);
                    break;
                case "read" or "r":
                    await PrintAllAsync();
                    break;
                case "header":
                    await PrintHeaderAsync();
                    break;
                case "watch" or "w":
                    await WatchAsync(parts, cancellationToken);
                    break;
                case "write" or "force":
                    if (parts.Length < 3)
                        throw new DiagnosticException(DiagnosticErrorKind.Value, $"Aufruf: {parts[0]} <ziel> <wert>");
                    var outcome = await WriteAsync(string.Join(' ', parts[1..^1]), parts[^1], parts[0].Equals("force", StringComparison.OrdinalIgnoreCase), cancellationToken);
                    await _out.WriteLineAsync(outcome.Message);
                    break;
                case "on":
                    await _out.WriteLineAsync((await WriteAsync(CommonRegisters.PowerSwitch.ToString(Culture), "1", false, cancellationToken)).Message);
                    break;
                case "off":
                    await _out.WriteLineAsync((await WriteAsync(CommonRegisters.PowerSwitch.ToString(Culture), "0", false, cancellationToken)).Message);
                    break;
                case "reset":
                    await _out.WriteLineAsync((await WriteAsync(CommonRegisters.ResetCommand.ToString(Culture), "1", false, cancellationToken)).Message);
                    break;
                case "raw":
                    await PrintRawAsync(parts);
                    break;
                case "info":
                    await PrintInfoAsync();
                    break;
                case "connect":
                    await ConnectAsync(parts);
                    break;
                default:
                    await _out.WriteLineAsync($"Unbekannter Befehl '{parts[0]}'. \"help\" zeigt alle Befehle.");
                    break;
            }
        }
        catch (DiagnosticException ex)
        {
            await _out.WriteLineAsync(Prefix(ex.Kind) + ex.Message);
        }
        return true;
    }

    public static string Prefix(DiagnosticErrorKind kind) => kind switch
    {
        DiagnosticErrorKind.Connection => "VERBINDUNGSFEHLER: ",
        DiagnosticErrorKind.Protocol => "PROTOKOLLFEHLER: ",
        _ => "WERTEFEHLER: "
    };

    public async Task PrintAllAsync()
    {
        var registers = ReadAll();
        await _out.WriteLineAsync($"=== {DescribeDevice(registers)} ===");
        await _out.WriteLineAsync("Gemeinsamer Registerkopf (0–19):");
        await _out.WriteAsync(RegisterFormatter.Table(CommonRegisters.Header, registers));

        if (_plan is { } plan)
        {
            await _out.WriteLineAsync($"Gerätespezifisch ({plan.DeviceName}, {CommonRegisters.SpecificStart}–{plan.Definitions.Max(d => d.EndAddress) - 1}):");
            await _out.WriteAsync(RegisterFormatter.Table(plan.Definitions.Where(d => d.Address >= CommonRegisters.SpecificStart), registers));
        }
        else
        {
            await _out.WriteLineAsync("Für diesen Gerätetyp ist noch kein Registerplan hinterlegt; gerätespezifische Register mit \"raw 20 30\" lesen.");
        }
    }

    private async Task PrintHeaderAsync()
    {
        var header = ReadHeader();
        await _out.WriteLineAsync($"=== {DescribeDevice(header)} ===");
        await _out.WriteAsync(RegisterFormatter.Table(CommonRegisters.Header, header));
    }

    /// <summary>
    /// Schreibt einen Wert und fragt danach beim Gerät nach, ob er angenommen wurde (Register 18/19) und was das Register jetzt zeigt.
    /// </summary>
    /// <param name="force">Ohne Prüfung im Client senden (auch nicht beschreibbare oder ungültige Werte).</param>
    public async Task<WriteOutcome> WriteAsync(string target, string valueText, bool force, CancellationToken cancellationToken)
    {
        ReadHeader();                                             // Gerätetyp und damit Registerplan bestimmen
        var definition = ResolveTarget(target, force);
        double value = ParseValue(definition, valueText);

        if (!force)
        {
            if (!definition.IsWritable)
                throw new DiagnosticException(DiagnosticErrorKind.Value,
                    $"{definition.Name} (Adresse {definition.Address}) ist nur lesbar. Mit \"force\" lässt sich testen, wie das Gerät reagiert.");
            if (definition.Validate(value) is { } reason)
                throw new DiagnosticException(DiagnosticErrorKind.Value,
                    $"{definition.Name} = {valueText}: {reason}. Mit \"force\" trotzdem senden.");
        }

        var before = Connection.ReadRegisters(definition.Address, definition.Count);
        Connection.Write(definition, value);
        await Task.Delay(WriteSettleTime, cancellationToken);     // das Gerät setzt den Wert im nächsten Schleifendurchlauf um

        var status = Connection.ReadRegisters(CommonRegisters.LastWriteError, 2);
        var after = Connection.ReadRegisters(definition.Address, definition.Count);
        var error = (WriteError)status[0];
        string was = RegisterFormatter.Value(definition, before);
        string now = RegisterFormatter.Value(definition, after);

        if (error == WriteError.None)
        {
            string note;
            if (definition.Address == CommonRegisters.ResetCommand)
            {
                // Befehlsregister: liest immer 0, die Wirkung zeigt sich im Zustand
                var state = Connection.ReadRegisters(CommonRegisters.OperatingState, 2);
                note = $"Befehl angenommen (das Register liest immer 0). Gerät meldet jetzt {RegisterFormatter.Short(CommonRegisters.HeaderAt(CommonRegisters.OperatingState), state.AsSpan(0, 1))}, Fehlercode {state[1]}.";
            }
            else
            {
                note = RegisterCodecEquals(definition, after, value)
                    ? $"Gerät zeigt jetzt {now}."
                    : $"Gerät zeigt noch {now} (Befehl wird im nächsten Schritt umgesetzt oder vom Gerät verworfen).";
            }
            return new WriteOutcome(true, error, status[1], $"ANGENOMMEN: {definition.Name} {was} → {Format(definition, value)}. {note}");
        }

        string meaning = CommonRegisters.HeaderAt(CommonRegisters.LastWriteError).LabelFor(status[0]) ?? status[0].ToString(Culture);
        return new WriteOutcome(false, error, status[1],
            $"ABGELEHNT vom Gerät: {definition.Name} = {Format(definition, value)} → Fehlerregister {CommonRegisters.LastWriteError} = {status[0]} ({meaning}), " +
            $"Adresse {status[1]}. Register zeigt weiter {now}.");
    }

    private static bool RegisterCodecEquals(RegisterDefinition definition, ushort[] registers, double expected) =>
        Math.Abs(RegisterCodec.Decode(definition, registers) - expected) <= Math.Max(1e-3, Math.Abs(expected) * 1e-6);

    private static string Format(RegisterDefinition definition, double value) =>
        $"{value.ToString("0.###", Culture)}{(definition.Unit.Length > 0 ? " " + definition.Unit : "")}";

    private RegisterDefinition ResolveTarget(string target, bool force)
    {
        var definitions = _plan?.Definitions ?? CommonRegisters.Header;

        if (int.TryParse(target, NumberStyles.None, CultureInfo.InvariantCulture, out int address))
        {
            if (definitions.FirstOrDefault(d => d.Address == address) is { } byAddress)
                return byAddress;
            if (definitions.FirstOrDefault(d => d.Contains(address)) is { } inside)
                throw new DiagnosticException(DiagnosticErrorKind.Value,
                    $"Adresse {address} liegt mitten in {inside.Name} ({inside.Address}–{inside.EndAddress - 1}); mehrteilige Werte immer ab ihrer ersten Adresse schreiben.");
            if (force)
                return new RegisterDefinition(address, $"Register {address}", RegisterType.UInt16, RegisterAccess.ReadWrite, "", "ohne Definition");
            throw new DiagnosticException(DiagnosticErrorKind.Value, $"Adresse {address} ist im Registerplan nicht belegt.");
        }

        var matches = definitions.Where(d => d.Name.Contains(target, StringComparison.OrdinalIgnoreCase)).ToList();
        if (matches.FirstOrDefault(d => d.Name.Equals(target, StringComparison.OrdinalIgnoreCase)) is { } exact)
            return exact;
        return matches.Count switch
        {
            1 => matches[0],
            0 => throw new DiagnosticException(DiagnosticErrorKind.Value, $"Kein Register mit dem Namen '{target}'. \"info\" zeigt den Registerplan."),
            _ => throw new DiagnosticException(DiagnosticErrorKind.Value,
                $"'{target}' ist nicht eindeutig: {string.Join(", ", matches.Select(m => $"{m.Name} ({m.Address})"))}.")
        };
    }

    private static double ParseValue(RegisterDefinition definition, string text)
    {
        if (definition.Labels is { } labels)
        {
            int index = labels.ToList().FindIndex(l => l.Equals(text, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
                return index;
        }

        if (!double.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            throw new DiagnosticException(DiagnosticErrorKind.Value, $"'{text}' ist keine Zahl.");
        return value;
    }

    private async Task WatchAsync(string[] parts, CancellationToken cancellationToken)
    {
        int count = int.MaxValue;
        double seconds = 1;
        if (parts.Length > 1 && !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out count))
            throw new DiagnosticException(DiagnosticErrorKind.Value, "Aufruf: watch [anzahl] [sekunden]");
        if (parts.Length > 2 && (!double.TryParse(parts[2].Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out seconds) || seconds < 0.1))
            throw new DiagnosticException(DiagnosticErrorKind.Value, "Das Intervall muss mindestens 0,1 s sein.");

        await _out.WriteLineAsync(count == int.MaxValue ? "Beobachte bis Strg+C …" : $"Beobachte {count}× …");
        for (int i = 0; i < count && !cancellationToken.IsCancellationRequested; i++)
        {
            try
            {
                var r = ReadAll();
                await _out.WriteLineAsync(Summary(r));
            }
            catch (DiagnosticException ex)
            {
                await _out.WriteLineAsync($"{DateTime.Now:HH:mm:ss}  {Prefix(ex.Kind)}{ex.Message} – neuer Versuch …");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    /// <summary>Eine Zeile je Abfrage für <c>watch</c>.</summary>
    private string Summary(IReadOnlyList<ushort> r)
    {
        string Show(RegisterDefinition d) => RegisterFormatter.Short(d, r.Skip(d.Address).Take(d.Count).ToArray());
        string Head(int address) => Show(CommonRegisters.HeaderAt(address));

        string extra = "";
        if (_plan is { } plan && plan.DeviceType == DeviceType.Charger)
        {
            string State(int number) => Show(plan.Get(ChargerRegisters.PointBase + ChargerRegisters.PointStride * (number - 1) + ChargerRegisters.PointState));
            extra = $" | Punkt 1: {State(1)}, Punkt 2: {State(2)} | Grenze {Show(plan.Get(ChargerRegisters.PowerLimit))}";
        }

        return $"{DateTime.Now:HH:mm:ss}  Zeit {Head(CommonRegisters.ModelTime)}  Seq {r[CommonRegisters.Sequence]}  Lauf {r[CommonRegisters.RunId]}  " +
               $"{Head(CommonRegisters.OperatingState)}  {Head(CommonRegisters.ActivePower)}  {Head(CommonRegisters.Energy)}  " +
               $"{Head(CommonRegisters.TimeStatus)}, Simulation {Head(CommonRegisters.SimulationRunning)}, Tempo x{RegisterCodec.Decode(CommonRegisters.HeaderAt(CommonRegisters.Speed), r.Skip(CommonRegisters.Speed).Take(2).ToArray()):0.##}{extra}";
    }

    private async Task PrintRawAsync(string[] parts)
    {
        if (parts.Length != 3
            || !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out int start)
            || !int.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out int count)
            || count is < 1 or > 125)
            throw new DiagnosticException(DiagnosticErrorKind.Value, "Aufruf: raw <adresse> <anzahl 1–125>");

        var registers = Connection.ReadRegisters(start, count);
        for (int i = 0; i < registers.Length; i++)
            await _out.WriteLineAsync($"  {start + i,5}  {registers[i],5}  0x{registers[i]:X4}  {Convert.ToString(registers[i], 2).PadLeft(16, '0')}");
    }

    private async Task PrintInfoAsync()
    {
        await _out.WriteLineAsync($"Gerät: {Connection.Endpoint}, Zeitlimit {Connection.TimeoutMs} ms, {(Connection.IsConnected ? "verbunden" : "nicht verbunden")}");
        var header = ReadHeader();
        await _out.WriteLineAsync($"Typ: {DescribeDevice(header)}");
        await _out.WriteLineAsync(_plan is { } plan
            ? plan.ToMarkdown(Connection.Host, Connection.Port, Connection.UnitId)
            : "Kein gerätespezifischer Registerplan hinterlegt, nur der gemeinsame Kopf:\n" +
              new RegisterPlan(DeviceType.PvPlant, "Kopf", []).ToMarkdown(Connection.Host, Connection.Port, Connection.UnitId));
    }

    private async Task ConnectAsync(string[] parts)
    {
        if (parts.Length is < 2 or > 3)
            throw new DiagnosticException(DiagnosticErrorKind.Value, "Aufruf: connect <host[:port]> [unit]");

        string host = parts[1];
        int port = Connection.Port;
        int colon = host.LastIndexOf(':');
        if (colon >= 0)
        {
            if (!int.TryParse(host[(colon + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out port) || port is < 1 or > 65535)
                throw new DiagnosticException(DiagnosticErrorKind.Value, $"Port in '{parts[1]}' ist ungültig.");
            host = host[..colon];
        }

        int unit = Connection.UnitId;
        if (parts.Length == 3 && (!int.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out unit) || unit is < 1 or > 247))
            throw new DiagnosticException(DiagnosticErrorKind.Value, "Unit-ID muss 1–247 sein.");

        var next = new DeviceConnection(host, port, (byte)unit, Connection.TimeoutMs);
        Connection.Dispose();
        Connection = next;
        _plan = null;
        _deviceType = null;
        await PrintHeaderAsync();
    }
}
