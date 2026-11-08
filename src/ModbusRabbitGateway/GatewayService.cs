using System.Globalization;

namespace ModbusRabbitGateway;

/// <summary>
/// Hauptschleife: liest im festen Intervall alle Geräte, veröffentlicht Messwerte und Statusänderungen und zeigt
/// je Lesung eine Übersichtszeile. Ein Gerätefehler hält die anderen Geräte nicht auf; ein Broker-Ausfall hält das Lesen
/// nicht auf (Messwerte werden dann verworfen, Ausfall und Wiederkehr einmal sichtbar gemeldet).
/// Reale Zeit nur über <see cref="TimeProvider"/> (testbar); die virtuelle Zeit kommt ausschließlich aus den Registern.
/// </summary>
public sealed class GatewayService(
    GatewayOptions options,
    IReadOnlyList<DevicePoller> pollers,
    IMessagePublisher publisher,
    TimeProvider time,
    TextWriter log)
{
    private static readonly CultureInfo De = CultureInfo.GetCultureInfo("de-AT");

    private bool _brokerDown;
    private int _dropped;

    /// <summary>Nachrichten, die seit Beginn des aktuellen Broker-Ausfalls nicht übertragen wurden.</summary>
    public int DroppedSinceOutage => _dropped;

    public bool BrokerDown => _brokerDown;

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var onlineSince = time.GetUtcNow();
        var lastOnlineSent = onlineSince;
        await PublishGatewayStateAsync("online", onlineSince, cancellationToken);
        try
        {
            using var timer = new PeriodicTimer(options.Interval, time);
            do
            {
                await PollAllAsync(cancellationToken);
                if (options.Once)
                    break;

                // Wiederholung (RabbitMQ liefert retained nicht an Wildcard-Abos, siehe DevicePoller)
                if (options.StatusRepeat > TimeSpan.Zero && time.GetUtcNow() - lastOnlineSent >= options.StatusRepeat)
                {
                    lastOnlineSent = time.GetUtcNow();
                    await PublishGatewayStateAsync("online", onlineSince, cancellationToken);
                }
            }
            while (await timer.WaitForNextTickAsync(cancellationToken));
        }
        finally
        {
            // Geordnetes Beenden: „offline“ noch melden, auch wenn das Beenden über das Token kam (kurzes eigenes Zeitlimit).
            using var shutdown = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            try
            {
                if (!await PublishGatewayStateAsync("offline", time.GetUtcNow(), shutdown.Token))
                    await log.WriteLineAsync("Gateway-Status „offline“ nicht übertragen (Broker nicht erreichbar).");
            }
            catch (OperationCanceledException)
            {
                await log.WriteLineAsync("Gateway-Status „offline“ konnte nicht mehr gesendet werden.");
            }
        }
    }

    private async Task PollAllAsync(CancellationToken cancellationToken)
    {
        foreach (var poller in pollers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var outcome = poller.Poll(time.GetUtcNow());

            if (outcome.StatusChange is { } status)
            {
                bool sent = await TryPublishAsync(Contract.DeviceStatusTopic(status.DeviceId), MessageJson.Serialize(status),
                    retain: true, cancellationToken);
                if (!outcome.StatusRepeated)                      // Wiederholungen nicht auf der Konsole
                    await log.WriteLineAsync(DescribeStatus(status) + (sent ? "" : NotSent));
            }

            if (outcome.Message is { } message)
            {
                bool sent = await TryPublishAsync(Contract.MeasurementsTopic(message.DeviceId), MessageJson.Serialize(message),
                    retain: false, cancellationToken);
                await log.WriteLineAsync(Summarize(message) + (sent ? "" : NotSent));
            }
        }
    }

    private const string NotSent = "  [nicht übertragen]";

    private async Task<bool> PublishGatewayStateAsync(string state, DateTimeOffset since, CancellationToken cancellationToken)
    {
        var message = new GatewayStatusMessage(Contract.SchemaVersion, options.GatewayId, state, Contract.FormatUtc(since));
        return await TryPublishAsync(Contract.GatewayStatusTopic(options.GatewayId), MessageJson.Serialize(message),
            retain: true, cancellationToken);
    }

    /// <returns><c>false</c>, wenn der Broker die Nachricht nicht angenommen hat; Ausfall und Wiederkehr werden je einmal gemeldet.</returns>
    private async Task<bool> TryPublishAsync(string topic, string payload, bool retain, CancellationToken cancellationToken)
    {
        try
        {
            await publisher.PublishAsync(topic, payload, retain, cancellationToken);
        }
        catch (BrokerUnavailableException ex)
        {
            _dropped++;
            if (!_brokerDown)
            {
                _brokerDown = true;
                await log.WriteLineAsync($"BROKER NICHT ERREICHBAR: {ex.Message} – Geräte werden weiter gelesen, " +
                                         "Messwerte bis zur Wiederverbindung verworfen, Neuversuch automatisch.");
            }
            return false;
        }

        if (_brokerDown)
        {
            await log.WriteLineAsync($"BROKER WIEDER VERBUNDEN – {_dropped} Nachricht(en) in der Ausfallzeit nicht übertragen; " +
                                     "Status-Nachrichten wurden nachgeholt.");
            _brokerDown = false;
            _dropped = 0;
        }
        return true;
    }

    /// <summary>Eine Zeile je Nachricht: Gerät, virtuelle Zeit, Lauf, Sequenzen, Qualität, Wirkleistung.</summary>
    public static string Summarize(MeasurementMessage message)
    {
        var power = message.Measurements.FirstOrDefault(m => m.Key == "wirkleistung");
        string powerText = power?.Value is { } p ? $"{p.ToString("0.###", De)} {power.Unit}" : "–";
        string simTime = message.SimTime.Replace('T', ' ');
        return $"{message.DeviceId,-7} Sim {simTime}  Lauf {message.RunId,-3} Seq {message.Seq,-5} Nr {message.PublishSeq,-6} " +
               $"{message.Quality}/{message.QualityReason,-11} Wirkleistung {powerText}";
    }

    public static string DescribeStatus(DeviceStatusMessage status) => status.State switch
    {
        Contract.DeviceState.Online => $"{status.DeviceId,-7} ONLINE",
        Contract.DeviceState.Offline => $"{status.DeviceId,-7} OFFLINE: {status.Error} – neuer Versuch im nächsten Intervall",
        _ => $"{status.DeviceId,-7} DEKODIERFEHLER: {status.Error} – keine Messwerte veröffentlicht",
    };
}
