using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using SimCore.Contracts;
using SimCore.Time;
using SimCore.TimeSync;

namespace SimulationMaster;

/// <summary>
/// Konsolenbedienung des Masters: flackerfreie Statusanzeige und Tastatur, beides ohne blockierendes Warten.
/// Eingaben (Tempo, Neustart-Zeit) werden zeichenweise gelesen, damit die Anzeige währenddessen weiterläuft.
/// </summary>
public sealed class MasterConsole(MasterClock clock, TimeSyncServer server, MasterOptions options)
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);
    private static readonly TimeSpan RenderInterval = TimeSpan.FromMilliseconds(250);
    private static readonly TimeSpan RedirectedRenderInterval = TimeSpan.FromSeconds(5);
    private const int MaxMessages = 6;
    private const int RedirectedWidth = 120;

    private readonly ConcurrentQueue<string> _messages = new();
    private readonly StringBuilder _input = new();
    private PromptKind _prompt = PromptKind.None;
    private int _lineCount;

    private enum PromptKind { None, Speed, Restart }

    private static bool IsInteractive => !Console.IsOutputRedirected;
    private static bool HasKeyboard => !Console.IsInputRedirected;
    private static int Width => IsInteractive ? Math.Max(Console.WindowWidth - 1, 60) : RedirectedWidth;

    /// <summary>Neue Meldung oben in der Meldungsliste (threadsicher, z. B. aus dem Zeitserver).</summary>
    public void AddMessage(string message)
    {
        string line = $"{DateTime.Now:HH:mm:ss}  {message}";
        _messages.Enqueue(line);
        while (_messages.Count > MaxMessages)
            _messages.TryDequeue(out _);
        if (!IsInteractive)
            Console.WriteLine(line);
    }

    /// <summary>Zeichnet und liest Tasten, bis <paramref name="cancellationToken"/> ausgelöst oder Q gedrückt wird.</summary>
    /// <param name="requestStop">Wird bei Q aufgerufen.</param>
    public async Task RunAsync(Action requestStop, CancellationToken cancellationToken)
    {
        Initialize();
        try
        {
            var interval = IsInteractive ? RenderInterval : RedirectedRenderInterval;
            var nextRender = DateTime.MinValue;
            while (!cancellationToken.IsCancellationRequested)
            {
                bool changed = false;
                while (HasKeyboard && Console.KeyAvailable)
                {
                    if (HandleKey(Console.ReadKey(intercept: true)) == KeyResult.Quit)
                    {
                        requestStop();
                        return;
                    }
                    changed = true;
                }

                if (changed || DateTime.UtcNow >= nextRender)
                {
                    await DrawAsync();
                    nextRender = DateTime.UtcNow + interval;
                }
                await Task.Delay(PollInterval, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    /// <summary>Letzte Anzeige stehen lassen und den Cursor darunter setzen.</summary>
    public async Task ShutdownAsync()
    {
        await DrawAsync();
        if (IsInteractive)
            Console.CursorVisible = true;
    }

    private enum KeyResult { Handled, Quit }

    private KeyResult HandleKey(ConsoleKeyInfo key)
    {
        if (_prompt != PromptKind.None)
        {
            HandlePromptKey(key);
            return KeyResult.Handled;
        }

        switch (key.Key)
        {
            case ConsoleKey.Spacebar:
            case ConsoleKey.P:
                TogglePause();
                break;
            case ConsoleKey.Add:
            case ConsoleKey.OemPlus:
                ChangeSpeed(Math.Min(clock.Latest.Speed * 2, MasterClock.MaxSpeed));
                break;
            case ConsoleKey.Subtract:
            case ConsoleKey.OemMinus:
                ChangeSpeed(Math.Max(clock.Latest.Speed / 2, MasterClock.MinSpeed));
                break;
            case ConsoleKey.T:
                StartPrompt(PromptKind.Speed);
                break;
            case ConsoleKey.N:
                StartPrompt(PromptKind.Restart);
                break;
            case ConsoleKey.D1: case ConsoleKey.NumPad1: RestartSeason("Frühling", 4); break;
            case ConsoleKey.D2: case ConsoleKey.NumPad2: RestartSeason("Sommer", 7); break;
            case ConsoleKey.D3: case ConsoleKey.NumPad3: RestartSeason("Herbst", 10); break;
            case ConsoleKey.D4: case ConsoleKey.NumPad4: RestartSeason("Winter", 1); break;
            case ConsoleKey.Q:
                return KeyResult.Quit;
        }
        return KeyResult.Handled;
    }

    private void TogglePause()
    {
        if (clock.Latest.Mode == SimMode.Running)
        {
            var state = clock.Pause();
            AddMessage($"Pause bei {state.SimAnchor.ToString("dd.MM.yyyy HH:mm:ss", Culture)}");
        }
        else
        {
            var state = clock.Resume();
            AddMessage($"Fortsetzen ab {state.SimAnchor.ToString("dd.MM.yyyy HH:mm:ss", Culture)}");
        }
    }

    private void ChangeSpeed(double speed)
    {
        try
        {
            double old = clock.Latest.Speed;
            clock.SetSpeed(speed);
            if (speed != old)
                AddMessage($"Tempo x{old.ToString("0.##", Culture)} → x{speed.ToString("0.##", Culture)}");
        }
        catch (ArgumentOutOfRangeException)
        {
            AddMessage($"FEHLER: Tempo {speed.ToString(Culture)} abgelehnt (erlaubt {MasterClock.MinSpeed}–{MasterClock.MaxSpeed})");
        }
    }

    private void RestartSeason(string season, int month)
    {
        var target = new DateTime(clock.Now.Year, month, 15, 6, 0, 0);
        Restart(target, season);
    }

    private void Restart(DateTime target, string? label = null)
    {
        var state = clock.Restart(target);
        AddMessage($"Neustart{(label is null ? "" : $" ({label})")}: Lauf {state.RunId} ab " +
                   $"{target.ToString("dd.MM.yyyy HH:mm", Culture)}");
    }

    private void StartPrompt(PromptKind kind)
    {
        _prompt = kind;
        _input.Clear();
    }

    private void HandlePromptKey(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.Escape:
                _prompt = PromptKind.None;
                return;
            case ConsoleKey.Backspace:
                if (_input.Length > 0)
                    _input.Length--;
                return;
            case ConsoleKey.Enter:
                SubmitPrompt(_input.ToString());
                _prompt = PromptKind.None;
                return;
        }
        if (!char.IsControl(key.KeyChar) && _input.Length < 25)
            _input.Append(key.KeyChar);
    }

    private void SubmitPrompt(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        if (_prompt == PromptKind.Speed)
        {
            if (MasterOptions.TryParseNumber(text, out double speed))
                ChangeSpeed(speed);
            else
                AddMessage($"FEHLER: '{text}' ist keine Zahl");
        }
        else if (_prompt == PromptKind.Restart)
        {
            if (MasterOptions.TryParseTime(text, out var target))
                Restart(target);
            else
                AddMessage($"FEHLER: '{text}' hat nicht das Format yyyy-MM-dd HH:mm");
        }
    }

    private static void Initialize()
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (IsInteractive)
        {
            Console.CursorVisible = false;
            Console.Clear();
        }
    }

    private async Task DrawAsync()
    {
        int width = Width;
        var lines = IsInteractive ? BuildLines() : [BuildStatusLine()];

        var sb = new StringBuilder();
        foreach (string line in lines)
            sb.Append(Fit(line, width)).Append('\n');
        for (int i = lines.Count; i < _lineCount; i++)
            sb.Append(new string(' ', width)).Append('\n');

        if (IsInteractive)
        {
            try
            {
                Console.SetCursorPosition(0, 0);
            }
            catch (IOException)
            {
                return;                                           // Fenster wird gerade verändert
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
        }
        await Console.Out.WriteAsync(sb.ToString());
        await Console.Out.FlushAsync();
        _lineCount = lines.Count;
    }

    private string BuildStatusLine()
    {
        var current = clock.Current;
        return $"{current.SimTimeAt(clock.MasterNow).ToString("dd.MM.yyyy HH:mm:ss", Culture)}  " +
               $"{ModeText(current.Mode)}  x{current.Speed.ToString("0.##", Culture)}  Lauf {current.RunId}  " +
               $"Simulatoren {server.Clients.Count(c => c.State == ClientConnectionState.Connected)}";
    }

    private List<string> BuildLines()
    {
        var current = clock.Current;
        var latest = clock.Latest;
        var masterNow = clock.MasterNow;
        var simNow = current.SimTimeAt(masterNow);
        var tolerance = options.Tolerance * current.Speed;

        var lines = new List<string>
        {
            "══ SIMULATIONS-MASTER ══════════════════════════════════════════════════",
            $"  Virtuelle Zeit : {simNow.ToString("ddd dd.MM.yyyy  HH:mm:ss", Culture)}",
            $"  Zustand        : {ModeText(current.Mode)}",
            $"  Tempo          : x{current.Speed.ToString("0.##", Culture)}  (1 s real = {FormatSpan(TimeSpan.FromSeconds(current.Speed))} virtuell)",
            $"  Lauf / Sequenz : {current.RunId} / {current.Sequence}",
        };
        if (latest.Sequence != current.Sequence)
        {
            var inMs = (latest.EffectiveAt - masterNow).TotalMilliseconds;
            lines.Add($"  Angekündigt    : {ModeText(latest.Mode)} x{latest.Speed.ToString("0.##", Culture)} " +
                      $"in {Math.Max(0, inMs).ToString("0", Culture)} ms");
        }
        else
        {
            lines.Add("");
        }

        lines.Add($"  Zeitserver     : {server.LocalEndPoint}  · Heartbeat {options.HeartbeatInterval.TotalMilliseconds:0} ms" +
                  $"  · Vorlauf {clock.LeadTime.TotalMilliseconds:0} ms  · Toleranz {FormatSpan(tolerance)}");
        lines.Add("");

        var clients = server.Clients;
        lines.Add($"── Simulatoren ({clients.Count(c => c.State == ClientConnectionState.Connected)} verbunden) " +
                  "────────────────────────────────────────────");
        lines.Add($"  {"Kennung",-12}{"Art",-12}{"Adresse",-22}{"Virt. Zeit",-11}{"Δ gemessen",-12}{"± max",-10}Status");
        if (clients.Count == 0)
            lines.Add("  (noch keine)");
        foreach (var c in clients)
        {
            string deviation = c.Deviation is { } d ? FormatSigned(d) : "–";
            string uncertainty = c.SimUncertainty is { } u ? FormatSpan(u) : "–";
            string check = c.Deviation is { } dev && c.State == ClientConnectionState.Connected
                ? (dev.Duration() <= tolerance ? " ✓" : " ✗ außerhalb Toleranz")
                : "";
            lines.Add($"  {Fit(c.ClientId, 11),-12}{Fit(c.ClientKind, 11),-12}{Fit(c.RemoteEndPoint, 21),-22}" +
                      $"{c.SimTime?.ToString("HH:mm:ss", Culture) ?? "–",-11}{deviation,-12}{uncertainty,-10}" +
                      $"{StateText(c)}{check}");
        }

        lines.Add("");
        lines.Add("── Meldungen ───────────────────────────────────────────────────────────");
        var messages = _messages.ToArray();
        for (int i = 0; i < MaxMessages; i++)
            lines.Add(i < messages.Length ? "  " + messages[messages.Length - 1 - i] : "");

        lines.Add("");
        lines.Add("  [Leertaste/P] Pause/Fortsetzen   [+/-] Tempo x2/÷2   [T] Tempo eingeben");
        lines.Add("  [N] Neustart mit Zeit   [1-4] Neustart Frühling/Sommer/Herbst/Winter   [Q] Beenden");
        lines.Add(_prompt switch
        {
            PromptKind.Speed => $"  Neues Tempo ({MasterClock.MinSpeed}–{MasterClock.MaxSpeed}), Enter = OK, Esc = Abbruch: {_input}_",
            PromptKind.Restart => $"  Neue Startzeit (yyyy-MM-dd HH:mm), Enter = OK, Esc = Abbruch: {_input}_",
            _ => ""
        });
        return lines;
    }

    private static string ModeText(SimMode mode) => mode == SimMode.Running ? "LÄUFT" : "PAUSIERT";

    private static string StateText(TimeSyncClientInfo client) => client.State switch
    {
        ClientConnectionState.Connected => client.SyncStatus == nameof(SyncStatus.Synced) ? "synchron" : client.SyncStatus ?? "verbunden",
        ClientConnectionState.NotResponding => "KEINE ANTWORT",
        _ => "GETRENNT"
    };

    private static string FormatSpan(TimeSpan span) => span.TotalSeconds switch
    {
        < 1 => $"{span.TotalMilliseconds.ToString("0.#", Culture)} ms",
        < 120 => $"{span.TotalSeconds.ToString("0.##", Culture)} s",
        < 7200 => $"{span.TotalMinutes.ToString("0.#", Culture)} min",
        _ => $"{span.TotalHours.ToString("0.#", Culture)} h"
    };

    private static string FormatSigned(TimeSpan span) => (span < TimeSpan.Zero ? "-" : "+") + FormatSpan(span.Duration());

    private static string Fit(string text, int width) =>
        text.Length > width ? text[..width] : text.PadRight(width);
}
