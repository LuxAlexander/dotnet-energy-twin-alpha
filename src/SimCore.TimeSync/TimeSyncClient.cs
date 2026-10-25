using System.Collections.Concurrent;
using System.Net.Sockets;
using SimCore.Contracts;
using SimCore.Time;

namespace SimCore.TimeSync;

/// <summary>
/// TCP-Client in jedem Simulator. Verbindet sich mit dem Master, übernimmt die Zeitzustände in die
/// <see cref="SyncedClock"/>, misst per Ping/Pong den Uhrenabstand und verbindet sich nach Abbrüchen selbst neu.
/// </summary>
public sealed class TimeSyncClient
{
    public static readonly TimeSpan DefaultPingInterval = TimeSpan.FromSeconds(1);
    public static readonly TimeSpan DefaultReconnectDelay = TimeSpan.FromSeconds(2);

    /// <summary>Anzahl schneller Pings direkt nach dem Verbinden, damit der Uhrenabgleich sofort genau ist.</summary>
    private const int InitialPingCount = 5;
    private static readonly TimeSpan InitialPingInterval = TimeSpan.FromMilliseconds(100);

    private readonly SyncedClock _clock;
    private readonly string _host;
    private readonly int _port;
    private readonly string _clientId;
    private readonly string _clientKind;
    private readonly TimeSpan _pingInterval;
    private readonly TimeSpan _reconnectDelay;
    private readonly ConcurrentDictionary<long, DateTimeOffset> _pendingPings = new();
    private long _nextPingId;
    private volatile bool _isConnected;
    private volatile string? _lastError;

    public TimeSyncClient(SyncedClock clock, string host, int port, string clientId, string clientKind,
        TimeSpan? pingInterval = null, TimeSpan? reconnectDelay = null)
    {
        _clock = clock;
        _host = host;
        _port = port;
        _clientId = clientId;
        _clientKind = clientKind;
        _pingInterval = pingInterval ?? DefaultPingInterval;
        _reconnectDelay = reconnectDelay ?? DefaultReconnectDelay;
    }

    /// <summary>Meldungen für die Anzeige (verbunden, getrennt, Fehler).</summary>
    public event EventHandler<string>? Log;

    public bool IsConnected => _isConnected;

    /// <summary>Letzter Verbindungs- oder Protokollfehler (für die Statusanzeige).</summary>
    public string? LastError => _lastError;

    public string Endpoint => $"{_host}:{_port}";

    /// <summary>Verbindet sich und hält die Verbindung, bis <paramref name="cancellationToken"/> ausgelöst wird.</summary>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await RunConnectionAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex) when (ex is IOException or SocketException or InvalidDataException or ObjectDisposedException)
            {
                _lastError = ex.Message;
                Report($"Master {Endpoint} nicht erreichbar: {ex.Message}");
            }
            finally
            {
                _isConnected = false;
                _pendingPings.Clear();
            }

            try
            {
                await Task.Delay(_reconnectDelay, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task RunConnectionAsync(CancellationToken cancellationToken)
    {
        using var tcp = new TcpClient();
        await tcp.ConnectAsync(_host, _port, cancellationToken);
        await using var channel = new MessageChannel(tcp);
        await channel.SendAsync(new HelloMessage(_clientId, _clientKind, SyncProtocol.Version), cancellationToken);
        _isConnected = true;
        _lastError = null;
        Report($"Mit Master {Endpoint} verbunden");

        using var pingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var pingTask = PingLoopAsync(channel, pingCts.Token);
        try
        {
            await ReceiveLoopAsync(channel, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await SendByeAsync(channel);
            throw;
        }
        finally
        {
            await pingCts.CancelAsync();
            await IgnoreEndAsync(pingTask);
        }
    }

    private async Task ReceiveLoopAsync(MessageChannel channel, CancellationToken cancellationToken)
    {
        while (await channel.ReceiveAsync(cancellationToken) is { } message)
        {
            switch (message)
            {
                case StatesMessage states:
                    ApplyStates(states);
                    break;
                case PongMessage pong when _pendingPings.TryRemove(pong.Id, out var sentLocal):
                    _clock.AddSyncSample(sentLocal, pong.MasterTime, _clock.LocalNow);
                    break;
                case PongMessage:
                    break;                                        // Antwort auf einen alten Ping
                case ByeMessage bye:
                    throw new IOException($"Master hat getrennt: {bye.Reason}");
                default:
                    Report($"Unerwartete Nachricht {message.GetType().Name} ignoriert");
                    break;
            }
        }
        throw new IOException("Verbindung vom Master geschlossen");
    }

    private void ApplyStates(StatesMessage message)
    {
        foreach (var state in message.States)
        {
            try
            {
                if (_clock.ApplyState(state) && _clock.LastLateness > TimeSpan.Zero)
                    Report($"Zeitzustand {state.Sequence} kam {_clock.LastLateness.TotalMilliseconds:0} ms zu spät " +
                           $"(Sprung ca. {(_clock.LastLateness * state.Speed).TotalSeconds:0.#} s Sim-Zeit)");
            }
            catch (ArgumentException ex)
            {
                Report($"Ungültiger Zeitzustand {state.Sequence} verworfen: {ex.Message}");
            }
        }
    }

    private async Task PingLoopAsync(MessageChannel channel, CancellationToken cancellationToken)
    {
        for (int i = 0; i < InitialPingCount; i++)
        {
            await SendPingAsync(channel, cancellationToken);
            await Task.Delay(InitialPingInterval, cancellationToken);
        }

        using var timer = new PeriodicTimer(_pingInterval);
        while (await timer.WaitForNextTickAsync(cancellationToken))
            await SendPingAsync(channel, cancellationToken);
    }

    private async Task SendPingAsync(MessageChannel channel, CancellationToken cancellationToken)
    {
        long id = Interlocked.Increment(ref _nextPingId);
        foreach (long old in _pendingPings.Keys.Where(k => k < id - 20))
            _pendingPings.TryRemove(old, out _);              // nie beantwortete Pings vergessen

        var status = _clock.Status;
        DateTime? simTime = status == SyncStatus.WaitingForMaster ? null : _clock.Now;
        _pendingPings[id] = _clock.LocalNow;
        await channel.SendAsync(
            new PingMessage(id, simTime, _clock.SimUncertainty.TotalSeconds, status.ToString()), cancellationToken);
    }

    private static async Task SendByeAsync(MessageChannel channel)
    {
        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
            await channel.SendAsync(new ByeMessage("Simulator beendet"), timeout.Token);
        }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException or OperationCanceledException)
        {
            // Master schon weg: beim Beenden egal.
        }
    }

    private static async Task IgnoreEndAsync(Task task)
    {
        try
        {
            await task;
        }
        catch (Exception ex) when (ex is OperationCanceledException or IOException or SocketException or ObjectDisposedException)
        {
            // Ping-Schleife endet mit der Verbindung; der Fehler wurde schon von der Empfangsschleife gemeldet.
        }
    }

    private void Report(string message) => Log?.Invoke(this, message);
}
