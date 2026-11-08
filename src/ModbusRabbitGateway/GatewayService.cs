using System.Globalization;

namespace ModbusRabbitGateway;

/// <summary>
/// Hauptschleife: liest im festen Intervall alle Geräte, veröffentlicht Messwerte und Statusänderungen und zeigt
/// je Lesung eine Übersichtszeile. Ein Gerätefehler hält die anderen Geräte nicht auf.
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

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await PublishGatewayStateAsync("online", cancellationToken);
        try
        {
            using var timer = new PeriodicTimer(options.Interval, time);
            do
            {
                await PollAllAsync(cancellationToken);
                if (options.Once)
                    break;
            }
            while (await timer.WaitForNextTickAsync(cancellationToken));
        }
        finally
        {
            // Geordnetes Beenden: „offline“ noch melden, auch wenn das Beenden über das Token kam (kurzes eigenes Zeitlimit).
            using var shutdown = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            try
            {
                await PublishGatewayStateAsync("offline", shutdown.Token);
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
                await publisher.PublishAsync(Contract.DeviceStatusTopic(status.DeviceId), MessageJson.Serialize(status),
                    retain: true, cancellationToken);
                await log.WriteLineAsync(DescribeStatus(status));
            }

            if (outcome.Message is { } message)
            {
                await publisher.PublishAsync(Contract.MeasurementsTopic(message.DeviceId), MessageJson.Serialize(message),
                    retain: false, cancellationToken);
                await log.WriteLineAsync(Summarize(message));
            }
        }
    }

    private Task PublishGatewayStateAsync(string state, CancellationToken cancellationToken)
    {
        var message = new GatewayStatusMessage(Contract.SchemaVersion, options.GatewayId, state, Contract.FormatUtc(time.GetUtcNow()));
        return publisher.PublishAsync(Contract.GatewayStatusTopic(options.GatewayId), MessageJson.Serialize(message),
            retain: true, cancellationToken);
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
