using System.Text;
using ModbusDiagnosticClient;

Console.OutputEncoding = Encoding.UTF8;

if (ClientOptions.IsHelpRequested(args))
{
    Console.WriteLine(ClientOptions.Usage);
    return 0;
}

ClientOptions options;
try
{
    options = ClientOptions.Parse(args);
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"Fehler: {ex.Message}\n\n{ClientOptions.Usage}");
    return 64;
}

using var connection = new DeviceConnection(options.Host, options.Port, options.UnitId, options.TimeoutMs);
var session = new DiagnosticSession(connection, Console.Out);

using var app = new CancellationTokenSource();
CancellationTokenSource? command = null;
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;                                              // geordnet beenden statt hart abbrechen
    if (command is { IsCancellationRequested: false } running)
        running.Cancel();                                         // Strg+C beendet zuerst nur den laufenden Befehl (z. B. watch)
    else
        app.Cancel();
};

try
{
    if (options.IsOneShot)
        return await RunOnceAsync(options, session, app.Token);

    return await RunInteractiveAsync(session, app.Token);
}
catch (OperationCanceledException)
{
    return 0;
}

static async Task<int> RunOnceAsync(ClientOptions options, DiagnosticSession session, CancellationToken cancellationToken)
{
    try
    {
        int exitCode = 0;
        foreach (var (target, value) in options.Writes)
        {
            var outcome = await session.WriteAsync(target, value, force: false, cancellationToken);
            Console.WriteLine(outcome.Message);
            if (!outcome.Accepted)
                exitCode = 2;
        }

        if (options.Once)
            await session.PrintAllAsync();
        return exitCode;
    }
    catch (DiagnosticException ex)
    {
        Console.Error.WriteLine(DiagnosticSession.Prefix(ex.Kind) + ex.Message);
        return ex.Kind == DiagnosticErrorKind.Value ? 64 : 1;
    }
}

async Task<int> RunInteractiveAsync(DiagnosticSession session, CancellationToken cancellationToken)
{
    Console.WriteLine("Modbus-Diagnose-Client – \"help\" zeigt die Befehle, Strg+C oder \"quit\" beendet.");
    await session.ExecuteAsync("header", cancellationToken);      // Verbindungsfehler werden angezeigt, die Eingabe bleibt offen

    while (!cancellationToken.IsCancellationRequested)
    {
        await Console.Out.WriteAsync("modbus> ");
        string? line = await Console.In.ReadLineAsync(cancellationToken);
        if (line is null)
            break;                                                // Eingabe zu Ende (z. B. umgeleitet)

        using var current = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        command = current;
        try
        {
            if (!await session.ExecuteAsync(line, current.Token))
                break;
        }
        catch (OperationCanceledException) when (current.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Abgebrochen.");
        }
        finally
        {
            command = null;
        }
    }

    Console.WriteLine("Verbindung getrennt.");
    return 0;
}
