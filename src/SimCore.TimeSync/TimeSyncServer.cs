using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using SimCore.Contracts;
using SimCore.Time;

namespace SimCore.TimeSync;

/// <summary>
/// TCP-Server im Simulations-Master. Verteilt die Zeitzustände der <see cref="MasterClock"/> an alle Simulatoren
/// (nach dem Verbinden, bei jeder Änderung und als Heartbeat) und beantwortet Pings für den Uhrenabgleich.
/// </summary>
public sealed class TimeSyncServer
{
    public static readonly TimeSpan DefaultHeartbeatInterval = TimeSpan.FromSeconds(1);
    public static readonly TimeSpan DefaultClientTimeout = TimeSpan.FromSeconds(3);
    public static readonly TimeSpan HelloTimeout = TimeSpan.FromSeconds(5);

    private readonly MasterClock _clock;
    private readonly TcpListener _listener;
    private readonly TimeSpan _heartbeatInterval;
    private readonly TimeSpan _clientTimeout;
    private readonly Lock _lock = new();
    private readonly Dictionary<string, ClientConnection> _clients = [];
    private readonly List<Task> _handlers = [];
    private readonly CancellationTokenSource _connectionsCts = new();   // erst nach dem bye abbrechen
    private readonly Channel<bool> _changes = Channel.CreateUnbounded<bool>(new UnboundedChannelOptions { SingleReader = true });
    private bool _started;

    public TimeSyncServer(MasterClock clock, int port, IPAddress? address = null,
        TimeSpan? heartbeatInterval = null, TimeSpan? clientTimeout = null)
    {
        _clock = clock;
        _listener = new TcpListener(address ?? IPAddress.Loopback, port);
        _heartbeatInterval = heartbeatInterval ?? DefaultHeartbeatInterval;
        _clientTimeout = clientTimeout ?? DefaultClientTimeout;
    }

    /// <summary>Meldungen für die Anzeige (Verbindungen, Fehler).</summary>
    public event EventHandler<string>? Log;

    public TimeSpan HeartbeatInterval => _heartbeatInterval;

    /// <summary>Tatsächliche Adresse nach <see cref="Start"/> (bei Port 0 mit dem zugeteilten Port).</summary>
    public IPEndPoint LocalEndPoint => (IPEndPoint)_listener.LocalEndpoint;

    /// <summary>Alle bekannten Simulatoren, auch getrennte (bis sie sich mit gleicher Kennung neu melden).</summary>
    public IReadOnlyList<TimeSyncClientInfo> Clients
    {
        get
        {
            var now = _clock.MasterNow;
            lock (_lock)
                return [.. _clients.Values.Select(c => c.Snapshot(now, _clientTimeout)).OrderBy(c => c.ClientId)];
        }
    }

    /// <summary>Öffnet den Port. Ein belegter Port fällt hier sofort als <see cref="SocketException"/> auf.</summary>
    public void Start()
    {
        if (_started)
            return;
        _listener.Start();
        _started = true;
    }

    /// <summary>Läuft bis <paramref name="cancellationToken"/> ausgelöst wird, verabschiedet dann alle Simulatoren.</summary>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Start();
        _clock.StateChanged += OnStateChanged;
        try
        {
            await Task.WhenAll(
                AcceptLoopAsync(cancellationToken),
                BroadcastLoopAsync(cancellationToken),
                HeartbeatLoopAsync(cancellationToken));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            _clock.StateChanged -= OnStateChanged;
            _listener.Stop();
            await SendToAllAsync(new ByeMessage("Simulations-Master beendet"), CancellationToken.None);
            await _connectionsCts.CancelAsync();
            Task[] handlers;
            lock (_lock)
                handlers = [.. _handlers];
            await Task.WhenAll(handlers);
            _connectionsCts.Dispose();
        }
    }

    private void OnStateChanged(object? sender, TimeState state) => _changes.Writer.TryWrite(true);

    private async Task AcceptLoopAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var tcp = await _listener.AcceptTcpClientAsync(cancellationToken);
            var handler = HandleClientAsync(tcp, _connectionsCts.Token);
            lock (_lock)
            {
                _handlers.RemoveAll(t => t.IsCompleted);
                _handlers.Add(handler);
            }
        }
    }

    private async Task BroadcastLoopAsync(CancellationToken cancellationToken)
    {
        await foreach (var _ in _changes.Reader.ReadAllAsync(cancellationToken))
            await SendToAllAsync(new StatesMessage(_clock.CurrentStates), cancellationToken);
    }

    private async Task HeartbeatLoopAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(_heartbeatInterval);
        while (await timer.WaitForNextTickAsync(cancellationToken))
            await SendToAllAsync(new StatesMessage(_clock.CurrentStates), cancellationToken);
    }

    /// <summary>Sendet parallel an alle offenen Verbindungen; ein langsamer Simulator hält die anderen nicht auf.</summary>
    private async Task SendToAllAsync(SyncMessage message, CancellationToken cancellationToken)
    {
        ClientConnection[] open;
        lock (_lock)
            open = [.. _clients.Values.Where(c => c.IsOpen)];

        await Task.WhenAll(open.Select(async client =>
        {
            try
            {
                await client.Channel.SendAsync(message, cancellationToken);
            }
            catch (Exception ex) when (IsConnectionError(ex))
            {
                Report($"{client.ClientId}: Senden fehlgeschlagen ({ex.Message})");
                await client.Channel.DisposeAsync();                // Empfangsschleife endet und meldet „getrennt“
            }
        }));
    }

    private async Task HandleClientAsync(TcpClient tcp, CancellationToken cancellationToken)
    {
        var channel = new MessageChannel(tcp);
        ClientConnection? client = null;
        try
        {
            var hello = await ReceiveHelloAsync(channel, cancellationToken);
            if (hello is null)
                return;

            client = Register(hello, channel);
            await channel.SendAsync(new StatesMessage(_clock.CurrentStates), cancellationToken);

            while (await channel.ReceiveAsync(cancellationToken) is { } message)
            {
                client.Touch(_clock.MasterNow);
                switch (message)
                {
                    case PingMessage ping:
                        client.Report(ping, _clock.Now);
                        await channel.SendAsync(new PongMessage(ping.Id, _clock.MasterNow), cancellationToken);
                        break;
                    case ByeMessage bye:
                        Report($"{client.ClientId} meldet sich ab: {bye.Reason}");
                        return;
                    default:
                        Report($"{client.ClientId}: unerwartete Nachricht {message.GetType().Name} ignoriert");
                        break;
                }
            }
            Report($"{client.ClientId}: Verbindung von der Gegenstelle geschlossen");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (InvalidDataException ex)
        {
            Report($"{client?.ClientId ?? channel.RemoteEndPoint}: Protokollfehler, Verbindung getrennt ({ex.Message})");
        }
        catch (Exception ex) when (IsConnectionError(ex))
        {
            Report($"{client?.ClientId ?? channel.RemoteEndPoint}: Verbindung abgebrochen ({ex.Message})");
        }
        finally
        {
            client?.MarkClosed(_clock.MasterNow);
            await channel.DisposeAsync();
        }
    }

    private async Task<HelloMessage?> ReceiveHelloAsync(MessageChannel channel, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(HelloTimeout);

        SyncMessage? first;
        try
        {
            first = await channel.ReceiveAsync(timeout.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            Report($"{channel.RemoteEndPoint}: kein hello innerhalb {HelloTimeout.TotalSeconds:0} s, getrennt");
            return null;
        }

        string? rejection = first switch
        {
            null => null,
            HelloMessage { ProtocolVersion: SyncProtocol.Version } => null,
            HelloMessage h => $"Protokollversion {h.ProtocolVersion} nicht unterstützt (erwartet {SyncProtocol.Version})",
            _ => $"erste Nachricht muss hello sein, war {first.GetType().Name}"
        };
        if (rejection is not null)
        {
            Report($"{channel.RemoteEndPoint}: abgelehnt, {rejection}");
            await channel.SendAsync(new ByeMessage(rejection), cancellationToken);
            return null;
        }
        return first as HelloMessage;
    }

    private ClientConnection Register(HelloMessage hello, MessageChannel channel)
    {
        var client = new ClientConnection(hello, channel, _clock.MasterNow);
        ClientConnection? previous;
        lock (_lock)
        {
            _clients.TryGetValue(hello.ClientId, out previous);
            _clients[hello.ClientId] = client;
        }

        if (previous is { IsOpen: true })
            Report($"{hello.ClientId}: neue Verbindung ersetzt die alte (doppelte Kennung?)");
        Report($"{hello.ClientId} ({hello.ClientKind}) verbunden von {channel.RemoteEndPoint}");
        return client;
    }

    private void Report(string message) => Log?.Invoke(this, message);

    private static bool IsConnectionError(Exception ex) =>
        ex is IOException or SocketException or ObjectDisposedException;

    /// <summary>Veränderlicher Zustand einer Verbindung, nur unter eigener Sperre geändert.</summary>
    private sealed class ClientConnection(HelloMessage hello, MessageChannel channel, DateTimeOffset connectedAt)
    {
        private readonly Lock _lock = new();
        private DateTimeOffset _lastSeen = connectedAt;
        private bool _closed;
        private PingMessage? _lastPing;
        private TimeSpan? _deviation;

        public string ClientId => hello.ClientId;
        public MessageChannel Channel => channel;

        public bool IsOpen
        {
            get { lock (_lock) return !_closed; }
        }

        public void Touch(DateTimeOffset now)
        {
            lock (_lock) _lastSeen = now;
        }

        public void Report(PingMessage ping, DateTime masterSimNow)
        {
            lock (_lock)
            {
                _lastPing = ping;
                _deviation = ping.SimTime is { } simTime && ping.SyncStatus == nameof(SyncStatus.Synced)
                    ? simTime - masterSimNow
                    : null;
            }
        }

        public void MarkClosed(DateTimeOffset now)
        {
            lock (_lock)
            {
                _closed = true;
                _lastSeen = now;
            }
        }

        public TimeSyncClientInfo Snapshot(DateTimeOffset now, TimeSpan timeout)
        {
            lock (_lock)
            {
                var state = _closed ? ClientConnectionState.Disconnected
                    : now - _lastSeen > timeout ? ClientConnectionState.NotResponding
                    : ClientConnectionState.Connected;
                TimeSpan? uncertainty = _lastPing?.SimUncertaintySeconds is { } s ? TimeSpan.FromSeconds(s) : null;
                return new TimeSyncClientInfo(hello.ClientId, hello.ClientKind, channel.RemoteEndPoint, state,
                    _lastSeen, _lastPing?.SimTime, _deviation, uncertainty, _lastPing?.SyncStatus);
            }
        }
    }
}
