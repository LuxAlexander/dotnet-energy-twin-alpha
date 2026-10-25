using System.Globalization;
using SimCore.Contracts;
using SimCore.Time;
using SimCore.TimeSync;

namespace Devices.Common;

/// <summary>
/// Zeitquelle eines Geräts: im Normalbetrieb die mit dem Master synchronisierte Zeit
/// (<see cref="SyncedClock"/> + <see cref="TimeSyncClient"/>), standalone eine lokale <see cref="MasterClock"/>.
/// Das Gerät liest die Zeit nur über <see cref="Clock"/> und zählt sie nie selbst hoch.
/// </summary>
public sealed class DeviceTimeSource
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

    private readonly SyncedClock? _synced;
    private readonly TimeSyncClient? _client;
    private readonly MasterClock? _local;

    private DeviceTimeSource(TimeSyncOptions options, ISimClock clock, SyncedClock? synced, TimeSyncClient? client, MasterClock? local)
    {
        Options = options;
        Clock = clock;
        _synced = synced;
        _client = client;
        _local = local;
        if (_client is not null)
            _client.Log += (_, message) => Log?.Invoke(this, message);
    }

    /// <param name="deviceId">Gerätekennung, wird dem Master gemeldet (z. B. <c>DC-01</c>).</param>
    /// <param name="deviceKind">Geräteart für die Anzeige im Master (z. B. <c>Charger</c>).</param>
    public static DeviceTimeSource Create(TimeSyncOptions options, string deviceId, string deviceKind, TimeProvider? timeProvider = null)
    {
        timeProvider ??= TimeProvider.System;
        if (options.Standalone)
        {
            var local = new MasterClock(timeProvider, options.StandaloneStart, options.StandaloneSpeed, TimeSpan.Zero);
            local.Resume();
            return new DeviceTimeSource(options, local, null, null, local);
        }

        var synced = new SyncedClock(timeProvider);
        var client = new TimeSyncClient(synced, options.MasterHost, options.MasterPort, deviceId, deviceKind);
        return new DeviceTimeSource(options, synced, synced, client, null);
    }

    /// <summary>Meldungen der Verbindung zum Master (verbunden, getrennt, Fehler).</summary>
    public event EventHandler<string>? Log;

    public TimeSyncOptions Options { get; }

    public ISimClock Clock { get; }

    public bool IsStandalone => _local is not null;

    public SyncStatus Status => _synced?.Status ?? SyncStatus.Synced;

    /// <summary>Gibt es schon eine virtuelle Zeit? (Mit Master erst nach dem ersten Zeitzustand.)</summary>
    public bool HasTime => Status != SyncStatus.WaitingForMaster;

    /// <summary>Läuft die virtuelle Zeit gerade? Bei verlorenem Master nicht.</summary>
    public bool IsRunning => HasTime && Clock.Mode == SimMode.Running;

    /// <summary>Hält die Verbindung zum Master bis <paramref name="cancellationToken"/>; standalone sofort fertig.</summary>
    public Task RunAsync(CancellationToken cancellationToken) =>
        _client?.RunAsync(cancellationToken) ?? Task.CompletedTask;

    /// <summary>Einzeilige Statusbeschreibung der Zeitquelle für die Geräteanzeige.</summary>
    public string StatusText
    {
        get
        {
            if (_local is not null)
                return $"STANDALONE (lokale Uhr, ohne Master) · {ModeText(_local.Mode)} · x{Speed(_local.Speed)}";

            var synced = _synced!;
            string endpoint = Options.MasterEndpoint;
            return synced.Status switch
            {
                SyncStatus.WaitingForMaster =>
                    $"WARTE AUF MASTER {endpoint}" + (_client!.LastError is { } error ? $" – {error}" : ""),
                SyncStatus.MasterLost =>
                    $"MASTER {endpoint} NICHT ERREICHBAR – Zeit steht, Wiederverbindung läuft",
                _ => $"Master {endpoint} · synchron · {ModeText(synced.Mode)} · x{Speed(synced.Speed)} · Lauf {synced.RunId}" +
                     $" · Abweichung ≤ {FormatSpan(synced.SimUncertainty)} (RTT {synced.RoundTrip.TotalMilliseconds.ToString("0.#", Culture)} ms)"
            };
        }
    }

    /// <summary>Pause/Fortsetzen; nur standalone, mit Master steuert der Master die Zeit.</summary>
    /// <returns>Meldung für die Anzeige.</returns>
    public string TogglePause()
    {
        if (_local is null)
            return "Pause und Tempo steuert der Simulations-Master.";

        if (_local.Latest.Mode == SimMode.Running)
        {
            _local.Pause();
            return "Pausiert (lokale Uhr).";
        }
        _local.Resume();
        return "Fortgesetzt (lokale Uhr).";
    }

    /// <summary>Tempo × <paramref name="factor"/>, begrenzt auf 1–100 000; nur standalone.</summary>
    /// <returns>Meldung für die Anzeige.</returns>
    public string ChangeSpeed(double factor)
    {
        if (_local is null)
            return "Pause und Tempo steuert der Simulations-Master.";

        double before = _local.Latest.Speed;
        double speed = Math.Clamp(before * factor, MasterClock.MinSpeed, MasterClock.MaxSpeed);
        if (speed == before)
            return $"Tempo bleibt bei x{Speed(before)} (erlaubt: {MasterClock.MinSpeed:0}–{MasterClock.MaxSpeed:0}).";
        _local.SetSpeed(speed);
        return $"Tempo auf x{Speed(speed)} gesetzt (lokale Uhr).";
    }

    private static string ModeText(SimMode mode) => mode == SimMode.Running ? "LÄUFT" : "PAUSIERT";

    private static string Speed(double speed) => speed.ToString("0.##", Culture);

    private static string FormatSpan(TimeSpan span) => span.TotalSeconds < 1
        ? $"{span.TotalMilliseconds.ToString("0", Culture)} ms"
        : $"{span.TotalSeconds.ToString("0.#", Culture)} s";
}
