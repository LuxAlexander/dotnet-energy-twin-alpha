using System.Diagnostics;
using System.Text.Json;
using EnergyTwin.Web.Presentation;

namespace EnergyTwin.Web.Management;

public enum InstanceRunState { NotTracked, ManagedRunning, ExternallyReachable }

public sealed record StartStepResult(string Id, bool Started, string Detail)
{
    public override string ToString() => $"{Id}: {Detail}";
}

/// <summary>
/// Lokale Prozessverwaltung selbst verwalteter Instanzen: Start ohne Konsolenfenster, Beenden des
/// eigenen Prozessbaums, Probe, ob ein Port des Moduls bereits belegt ist. Nach einem Webdienst-Neustart
/// gehen unsere Handles verloren; anhand von Port und Kommandozeile werden unsere Prozesse wieder erkannt
/// und können trotzdem geordnet beendet werden. Programme fremder Anbieter auf belegten Ports bleiben
/// unberührt.
/// </summary>
public sealed class ProcessManager(ILogger<ProcessManager> logger, IConfiguration configuration)
{
    private static string? _repoRootCache;
    private readonly string _repoRoot = FindRoot();
    private readonly object _gate = new();
    private readonly Dictionary<string, Process> _processes = new(StringComparer.Ordinal);
    private readonly SemaphoreSlim _actions = new(1, 1);

    public static string FindRoot()
    {
        if (_repoRootCache is not null) return _repoRootCache;
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "EnergyTwin.sln")))
            directory = directory.Parent;
        _repoRootCache = directory?.FullName ?? throw new InvalidOperationException("Repository-Ordner nicht gefunden.");
        return _repoRootCache;
    }

    public InstanceRunState State(InstanceConfig config)
    {
        lock (_gate)
            if (_processes.TryGetValue(config.Id, out var process) && !process.HasExited)
                return InstanceRunState.ManagedRunning;
        var port = ListenPort(config);
        if (port is null) return InstanceRunState.NotTracked;
        return PortProbe.IsFree(port.Value) ? InstanceRunState.NotTracked : InstanceRunState.ExternallyReachable;
    }

    public bool IsRunning(InstanceConfig config) =>
        State(config) == InstanceRunState.ManagedRunning || FindOurselves(config, ListenPort(config)) is not null;

    public async Task<string> StartInstanceAsync(InstanceConfig config, IReadOnlyList<InstanceConfig> configs)
    {
        if (config.Kind == ModuleKind.Broker) return (await StartBrokerAsync()).Detail;

        var result = await StartAsync(config, configs);
        if (result.Started && config.ModbusPort is not null)
        {
            var gateway = await SyncGatewayAsync(configs, startIfStopped: false);
            if (gateway is { Started: false }) return $"{result.Detail} Gateway: {gateway.Detail}";
        }
        return result.Detail;
    }

    /// <summary>
    /// Startet das Gateway mit der aktuellen Geräteliste neu, damit neue oder entfernte Anlagen ohne
    /// manuellen Neustart in den Live-Daten erscheinen. Ist das Gateway gestoppt, wird es nur auf
    /// ausdrücklichen Wunsch gestartet; fremde Prozesse bleiben geschützt.
    /// </summary>
    public async Task<StartStepResult?> SyncGatewayAsync(IReadOnlyList<InstanceConfig> configs, bool startIfStopped)
    {
        var gateway = configs.FirstOrDefault(instance => instance.Kind == ModuleKind.Gateway);
        if (gateway is null) return null;
        var running = IsRunning(gateway);
        if (!running && !startIfStopped) return null;
        if (running)
        {
            var stop = await StopAsync(gateway);
            if (stop is not null && !stop.Started) return stop;
        }
        return await StartAsync(gateway, configs);
    }

    /// <summary>Startet dotnet ohne Fenster; schlägt fehl, wenn bereits verwaltet gelaufen oder der Port belegt ist.</summary>
    public async Task<StartStepResult> StartAsync(InstanceConfig config, IReadOnlyList<InstanceConfig> configs)
    {
        await _actions.WaitAsync();
        try
        {
            var state = State(config);
            if (state == InstanceRunState.ManagedRunning)
                return new StartStepResult(config.Id, false, "Läuft bereits.");
            var port = ListenPort(config);
            if (state == InstanceRunState.ExternallyReachable)
                return new StartStepResult(config.Id, false, $"Port {port} ist bereits belegt.");
            if (port is null && FindOurselves(config, null) is not null)
                return new StartStepResult(config.Id, false, "Läuft bereits extern.");

            var logPath = Path.Combine(Path.GetTempPath(), "energytwin-web", $"{config.Id}.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            var info = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = _repoRoot,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = config.Kind == ModuleKind.Master,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            foreach (var argument in CommandFor(config, configs))
                info.ArgumentList.Add(argument);
            if (config.Kind == ModuleKind.Master)
            {
                info.ArgumentList.Add("--mqtt-host");
                info.ArgumentList.Add(configuration["Mqtt:Host"] ?? "127.0.0.1");
                info.ArgumentList.Add("--mqtt-port");
                info.ArgumentList.Add(configuration.GetValue("Mqtt:Port", 1883).ToString(System.Globalization.CultureInfo.InvariantCulture));
                if (!string.IsNullOrWhiteSpace(configuration["Mqtt:EnvFile"]))
                {
                    info.ArgumentList.Add("--mqtt-env");
                    info.ArgumentList.Add(Path.GetFullPath(configuration["Mqtt:EnvFile"]!));
                }
            }
            var process = Process.Start(info);
            if (process is null) return new StartStepResult(config.Id, false, "Start fehlgeschlagen.");
            if (info.RedirectStandardInput) process.StandardInput.Close();

            // Kurz warten und prüfen, ob der Prozess sofort wieder abstürzt (z. B. wegen
            // Port-Konflikt, fehlender Build-Ausgabe oder Initialisierungsfehler).
            await Task.Delay(400);
            if (process.HasExited)
            {
                lock (_gate) _processes.Remove(config.Id);
                var stderr = ReadStderr(process);
                return new StartStepResult(config.Id, false,
                    string.IsNullOrWhiteSpace(stderr)
                        ? "Prozess ist unmittelbar nach dem Start beendet – Protokoll unter %TEMP%\\energytwin-web prüfen."
                        : $"Prozess beendet direkt nach dem Start: {stderr.Trim()}");
            }

            lock (_gate) _processes[config.Id] = process;
            _ = Redirect(process.StandardOutput, logPath, append: false);
            _ = Redirect(process.StandardError, logPath, append: true);
            logger.LogInformation("Instanz {Id} gestartet (PID {Pid}), Protokoll {Log}", config.Id, process.Id, logPath);
            return new StartStepResult(config.Id, true, "Gestartet.");
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or System.ComponentModel.Win32Exception)
        {
            logger.LogWarning(exception, "Instanz {Id} konnte nicht gestartet werden", config.Id);
            return new StartStepResult(config.Id, false, "Start fehlgeschlagen. Details stehen im Webdienst-Protokoll.");
        }
        finally { _actions.Release(); }
    }

    public async Task<StartStepResult?> StopAsync(InstanceConfig config)
    {
        Process? process;
        lock (_gate) _processes.TryGetValue(config.Id, out process);
        if (process is not null && !process.HasExited)
            return await StopProcessAsync(config, process, "Prozess geordnet beendet.");

        var port = ListenPort(config);
        if (port is not null && PortProbe.IsFree(port.Value))
            return new StartStepResult(config.Id, false, "Läuft nicht — nichts beendet.");

        var external = FindOurselves(config, port);
        if (external is not { } pid)
            return new StartStepResult(config.Id, false,
                port is null
                    ? "Nicht als verwalteter Prozess laufend — nichts beendet."
                    : $"Port {port} von einem fremden Programm belegt — zum Schutz nicht beendet.");

        try
        {
            var target = Process.GetProcessById(pid);
            return await StopProcessAsync(config, target, $"Extern gestartet, an Port {port} und Kommandozeile erkannt — geordnet beendet.");
        }
        catch (Exception)
        {
            return new StartStepResult(config.Id, false, "Beenden fehlgeschlagen — Zugriff auf den Prozess fehlte.");
        }
    }

    private async Task<StartStepResult> StopProcessAsync(InstanceConfig config, Process process, string okText)
    {
        try
        {
            process.Kill(entireProcessTree: true);
            await process.WaitForExitAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
        }
        catch (OperationCanceledException)
        {
            return new StartStepResult(config.Id, false, "Beenden nicht bestätigt, Prozess evtl. hängt.");
        }
        catch (Exception)
        {
            return new StartStepResult(config.Id, false, "Beenden fehlgeschlagen.");
        }
        lock (_gate) _processes.Remove(config.Id);
        return new StartStepResult(config.Id, true, okText);
    }

    /// <summary>Geordnete Reihenfolge: Master zuerst, dann Geräte, Wetter und Gateway; Broker nicht berührt.</summary>
    public async Task<IReadOnlyList<StartStepResult>> StopAllAsync(IReadOnlyList<InstanceConfig> configs)
    {
        var order = new[] { ModuleKind.Master, ModuleKind.Charger, ModuleKind.ColdStore, ModuleKind.Battery, ModuleKind.Pv, ModuleKind.Weather, ModuleKind.Gateway };
        var results = new List<StartStepResult>();
        foreach (var kind in order)
        {
            foreach (var config in configs.Where(instance => instance.Kind == kind))
            {
                var result = await StopAsync(config);
                if (result is not null) results.Add(result);
            }
        }
        return results;
    }

    /// <summary>Sucht unter dotnet-Prozessen unseren Simulator anhand Markern in der Kommandozeile; gibt die PID zurück.</summary>
    private static int? FindOurselves(InstanceConfig config, int? port)
    {
        var marker = config.Kind switch
        {
            ModuleKind.Pv => DeviceMarker("PvSimulator", config),
            ModuleKind.Battery => DeviceMarker("BatterySimulator", config),
            ModuleKind.Charger => DeviceMarker("DualPortCharger", config),
            ModuleKind.ColdStore => DeviceMarker("KuehlhausSimulator", config),
            ModuleKind.Weather when port is { } weatherPort =>
                $"--project src/WeatherSimulator -- --port {weatherPort}",
            ModuleKind.Master => "--project src/SimulationMaster --",
            ModuleKind.Gateway => "--project src/ModbusRabbitGateway --",
            _ => null
        };
        if (marker is null) return null;
        foreach (var (pid, commandLine) in DotnetCommandLines())
            if (commandLine.Contains(marker, StringComparison.Ordinal))
                return pid;
        return null;
    }

    private static string DeviceMarker(string project, InstanceConfig config) =>
        $"--project src/Devices/{project} -- --id {config.Id} --modbus-port {config.ModbusPort}";

    private static IReadOnlyList<(int Pid, string CommandLine)> DotnetCommandLines()
    {
        if (OperatingSystem.IsWindows())
        {
            var output = RunViaStdin("powershell",
                "-NoProfile -Command -",
                "Get-CimInstance Win32_Process -Filter \"Name='dotnet.exe'\" | Select-Object ProcessId,CommandLine | ConvertTo-Json -Compress",
                15);
            if (output is null) return [];
            try
            {
                using var document = JsonDocument.Parse(output);
                if (document.RootElement.ValueKind == JsonValueKind.Array)
                    return document.RootElement.EnumerateArray()
                        .Select(entry => (entry.GetProperty("ProcessId").GetInt32(),
                            entry.TryGetProperty("CommandLine", out var line) ? line.GetString() ?? string.Empty : string.Empty))
                        .ToList();
                return [(document.RootElement.GetProperty("ProcessId").GetInt32(),
                    document.RootElement.GetProperty("CommandLine").GetString() ?? string.Empty)];
            }
            catch (JsonException) { return []; }
        }
        if (OperatingSystem.IsLinux())
        {
            var lines = new List<(int, string)>();
            foreach (var directory in Directory.EnumerateDirectories("/proc"))
            {
                if (!int.TryParse(Path.GetFileName(directory), out var pid)) continue;
                try
                {
                    var bytes = File.ReadAllBytes(Path.Combine(directory, "cmdline"));
                    var commandLine = System.Text.Encoding.UTF8.GetString(bytes).Replace('\0', ' ');
                    if (commandLine.Contains("dotnet", StringComparison.OrdinalIgnoreCase)) lines.Add((pid, commandLine));
                }
                catch (IOException) { }
            }
            return lines;
        }
        return [];
    }

    private static string? RunViaStdin(string fileName, string arguments, string stdinText, int timeoutSeconds)
    {
        try
        {
            var psi = new ProcessStartInfo(fileName, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            using var process = Process.Start(psi);
            if (process is null) return null;
            process.StandardInput.WriteLine(stdinText);
            process.StandardInput.Close();
            var text = process.StandardOutput.ReadToEnd();
            process.WaitForExit(timeoutSeconds * 1000);
            return text;
        }
        catch (Exception) { return null; }
    }

    private static string? RunCapture(string fileName, string arguments, int timeoutSeconds)
    {
        try
        {
            var psi = new ProcessStartInfo(fileName, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            using var process = Process.Start(psi);
            if (process is null) return null;
            var text = process.StandardOutput.ReadToEnd();
            process.WaitForExit(timeoutSeconds * 1000);
            return text;
        }
        catch (Exception) { return null; }
    }

    /// <summary>Startet den vorhandenen RabbitMQ-Container über docker compose; Fehler enthalten die docker-Ausgabe.</summary>
    public async Task<StartStepResult> StartBrokerAsync()
    {
        var composeDirectory = Path.Combine(_repoRoot, "docker");
        var (fileName, arguments) = ComposeCommand(composeDirectory);
        if (fileName is null)
            return new StartStepResult("broker", false, "docker wurde nicht gefunden — Docker Desktop installiert und läuft?");

        string output;
        int exitCode;
        try
        {
            var psi = new ProcessStartInfo(fileName, arguments)
            {
                WorkingDirectory = composeDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            using var process = Process.Start(psi);
            if (process is null) return new StartStepResult("broker", false, "docker compose konnte nicht gestartet werden.");
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            var stdout = process.StandardOutput.ReadToEndAsync(timeout.Token);
            var stderr = process.StandardError.ReadToEndAsync(timeout.Token);
            var streams = await Task.WhenAll(stdout, stderr);
            output = string.Join("\n", streams.Select(stream => stream.Trim()));
            await process.WaitForExitAsync(timeout.Token);
            exitCode = process.ExitCode;
        }
        catch (System.ComponentModel.Win32Exception exception)
        {
            return new StartStepResult("broker", false, $"docker nicht startbar: {exception.Message}");
        }
        catch (Exception exception)
        {
            return new StartStepResult("broker", false, $"docker compose: {exception.Message}");
        }
        if (exitCode != 0)
        {
            var detail = FirstLine(output) is { } line ? $" docker: {line}" : string.Empty;
            return new StartStepResult("broker", false, $"docker compose beendet mit Code {exitCode} — Docker läuft? {detail}".Trim());
        }
        var note = FirstLine(output) is { } first ? $" ({first})" : string.Empty;
        return new StartStepResult("broker", true, $"docker compose: Broker gestartet oder bereits laufend.{note}");
    }

    private static (string? FileName, string Arguments) ComposeCommand(string composeDirectory)
    {
        if (RunCapture("docker", "version --format \"{{.Client.Version}}\"", 15) is not null)
            return ("docker", "compose up -d");
        var candidates = new[]
        {
            Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\Docker\Docker\resources\bin\docker.exe"),
            Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Docker\resources\bin\docker.exe"),
            "/usr/local/bin/docker",
            "/usr/bin/docker",
        };
        foreach (var candidate in candidates)
            if (File.Exists(candidate)) return (candidate, "compose up -d");
        return (null, "compose up -d");
    }

    private static string? FirstLine(string text)
    {
        foreach (var part in text.Split('\n'))
            if (part.Trim().Length > 0)
                return part.Trim();
        return null;
    }

    private static int? ListenPort(InstanceConfig config) => config.Kind switch
    {
        ModuleKind.Pv or ModuleKind.Battery or ModuleKind.Charger or ModuleKind.ColdStore => config.ModbusPort,
        ModuleKind.Weather => config.HttpPort,
        ModuleKind.Master => InstanceStore.MasterPort,
        _ => null
    };

    /// <summary>dotnet-Kommandozeile je Instanz; Master startet standardmäßig pausiert.</summary>
    private static string[] CommandFor(InstanceConfig config, IReadOnlyList<InstanceConfig> configs)
    {
        var (project, options) = config.Kind switch
        {
            ModuleKind.Pv => ("Devices/PvSimulator", DeviceOptions(config)),
            ModuleKind.Battery => ("Devices/BatterySimulator", DeviceOptions(config)),
            ModuleKind.Charger => ("Devices/DualPortCharger", DeviceOptions(config)),
            ModuleKind.ColdStore => ("Devices/KuehlhausSimulator", DeviceOptions(config)),
            ModuleKind.Weather => ("WeatherSimulator", new[] { "--port", $"{config.HttpPort}", "--mqtt" }),
            ModuleKind.Master => ("SimulationMaster", new[] { "--speed", $"{config.Speed}", "--mqtt" }),
            ModuleKind.Gateway => ("ModbusRabbitGateway", new[] { "--interval", "1" }.Concat(configs
                .Where(device => device.ModbusPort is not null)
                .SelectMany(device => new[] { "--device", $"{device.Id}=127.0.0.1:{device.ModbusPort}" }))),
            _ => throw new InvalidOperationException($"Kein Startkommando für {config.Kind}.")
        };
        return ["run", "--no-build", "--project", $"src/{project}", "--", .. options];
    }

    private static string[] DeviceOptions(InstanceConfig config) =>
        ["--id", config.Id, "--modbus-port", $"{config.ModbusPort}", "--location", config.Location ?? "salzburg"];

    private static async Task Redirect(StreamReader reader, string path, bool append)
    {
        try
        {
            using var writer = new StreamWriter(path, append: append);
            string? line;
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                await writer.WriteLineAsync(line);
                await writer.FlushAsync();
            }
        }
        catch (IOException) { }
    }

    /// <summary>Liest die stderr-Ausgabe eines bereits beendeten Prozesses, um den Absturzgrund zu ermitteln.</summary>
    private static string ReadStderr(Process process)
    {
        try
        {
            return process.StandardError.ReadToEnd();
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
