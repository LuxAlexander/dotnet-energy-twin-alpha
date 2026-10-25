using System.Net.Sockets;
using System.Text;
using SimCore.Contracts;

namespace SimCore.TimeSync;

/// <summary>
/// Eine TCP-Verbindung, über die zeilenweise <see cref="SyncMessage"/>s laufen.
/// Senden ist threadsicher (Heartbeat und Pong können gleichzeitig senden), Empfangen nur aus einer Schleife.
/// </summary>
public sealed class MessageChannel : IAsyncDisposable
{
    /// <summary>Längste erlaubte Zeile; schützt vor Gegenstellen, die nie ein Zeilenende senden.</summary>
    public const int MaxLineLength = 64 * 1024;

    private static readonly UTF8Encoding Utf8 = new(encoderShouldEmitUTF8Identifier: false);

    private readonly TcpClient _client;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly SemaphoreSlim _sendLock = new(1, 1);

    public MessageChannel(TcpClient client)
    {
        _client = client;
        _client.NoDelay = true;                                   // kleine Nachrichten sofort senden (Laufzeit!)
        var stream = client.GetStream();
        _reader = new StreamReader(stream, Utf8, detectEncodingFromByteOrderMarks: false);
        _writer = new StreamWriter(stream, Utf8) { NewLine = "\n" };
        RemoteEndPoint = client.Client.RemoteEndPoint?.ToString() ?? "?";
    }

    public string RemoteEndPoint { get; }

    public async Task SendAsync(SyncMessage message, CancellationToken cancellationToken)
    {
        string line = SyncProtocol.Serialize(message);
        await _sendLock.WaitAsync(cancellationToken);
        try
        {
            await _writer.WriteLineAsync(line.AsMemory(), cancellationToken);
            await _writer.FlushAsync(cancellationToken);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    /// <returns>Die nächste Nachricht oder <c>null</c>, wenn die Gegenstelle die Verbindung geschlossen hat.</returns>
    /// <exception cref="InvalidDataException">Ungültige oder zu lange Nachricht.</exception>
    public async Task<SyncMessage?> ReceiveAsync(CancellationToken cancellationToken)
    {
        string? line = await _reader.ReadLineAsync(cancellationToken);
        if (line is null)
            return null;
        if (line.Length > MaxLineLength)
            throw new InvalidDataException($"Nachricht zu lang ({line.Length} Zeichen).");
        return SyncProtocol.Deserialize(line);
    }

    public ValueTask DisposeAsync()
    {
        // Kein Flush beim Schließen: die Gegenstelle kann schon weg sein.
        _client.Dispose();
        _sendLock.Dispose();
        return ValueTask.CompletedTask;
    }
}
