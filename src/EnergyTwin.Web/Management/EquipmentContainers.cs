using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Devices.Common.Control;
using EnergyTwin.Web.Presentation;

namespace EnergyTwin.Web.Management;

/// <summary>
/// Kenndaten eines Geräte-Containers: Modultyp, Anzeigename (z. B. „Kühlhaus“ für „Kühlhaus-Container“), Kürzel für
/// Image, Labels und Containernamen, Projektordner unter <c>src/Devices</c> mit gleichnamigem Dockerfile,
/// Steuerkennung des Geräts sowie fester Steuerport und Umgebungsvariable im Container.
/// </summary>
public sealed record EquipmentContainerProfile(ModuleKind Kind, string Name, string Short, string Project,
    string ControlKind, int ControlPort, string ControlEnvironment)
{
    public string Image => $"energytwin/{Short}:lb04";
}

/// <summary>
/// Verwaltet ausschließlich Container der Ladesäule bzw. des Kühlhauses, die diese Web-App auf dem lokalen
/// Docker Desktop angelegt hat. Aufbau und Sicherheitsgrenzen entsprechen <see cref="PvContainers"/>.
/// </summary>
public abstract class EquipmentContainers(ILogger logger, EquipmentContainerProfile profile)
{
    private const string DataTarget = "/data";
    private const string ControlTarget = "/run/energytwin/control";
    private readonly string _runLabel = $"energytwin.web.{profile.Short}-run";
    private readonly string _deviceLabel = $"energytwin.web.{profile.Short}-id";
    private readonly string _containerControlPort = $"{profile.ControlPort}/tcp";
    private string RunsDirectory => Path.Combine(DeviceControl.DirectoryPath, $"{profile.Short}-containers");
    private string Name => profile.Name;
    private volatile DeviceContainerRun[] _running = [];

    public EquipmentContainerProfile Profile => profile;

    private sealed record DockerResult(int ExitCode, string Output, string Error)
    {
        public bool Success => ExitCode == 0;
        public string Detail => Error.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault() ?? $"Docker-Exitcode {ExitCode}.";
    }

    public DeviceContainerRun[] For(string id) => _running.Where(run => run.Startup.Id == id).ToArray();

    public async Task<DeviceContainerRun[]> DiscoverAsync(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(RunsDirectory) || !Directory.EnumerateDirectories(RunsDirectory).Any())
            return _running = [];

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(3));
        cancellationToken = timeout.Token;
        var list = await DockerAsync(["ps", "--no-trunc", "--filter", $"label={_runLabel}", "--format", "{{.ID}}"], cancellationToken);
        if (!list.Success) return _running = [];
        var ids = list.Output.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Where(IsContainerId).ToArray();
        if (ids.Length == 0) return _running = [];
        var inspect = await DockerAsync(["container", "inspect", .. ids], cancellationToken);
        if (!inspect.Success) return _running;
        using var json = JsonDocument.Parse(inspect.Output);
        return _running = json.RootElement.EnumerateArray().Select(ReadRun).OfType<DeviceContainerRun>().ToArray();
    }

    private DeviceContainerRun? ReadRun(JsonElement container)
    {
        if (ReadOwner(container) is not { } owned || !container.GetProperty("State").GetProperty("Running").GetBoolean())
            return null;
        var ports = container.GetProperty("NetworkSettings").GetProperty("Ports");
        if (ports.ValueKind != JsonValueKind.Object || !ports.TryGetProperty(_containerControlPort, out var published)
            || published.ValueKind != JsonValueKind.Array)
            return null;
        foreach (var endpoint in published.EnumerateArray())
        {
            if (endpoint.GetProperty("HostIp").GetString() == "127.0.0.1"
                && int.TryParse(endpoint.GetProperty("HostPort").GetString(), out var port))
                return new(container.GetProperty("Id").GetString()!, port, owned.ControlDirectory, owned.Startup);
        }
        return null;
    }

    private (string ControlDirectory, InstanceConfig Startup)? ReadOwner(JsonElement container)
    {
        var labels = container.GetProperty("Config").GetProperty("Labels");
        if (labels.ValueKind != JsonValueKind.Object || !labels.TryGetProperty(_runLabel, out var key)
            || !Guid.TryParseExact(key.GetString(), "N", out var run))
            return null;
        var directory = Path.Combine(RunsDirectory, run.ToString("N"));
        try
        {
            if (!File.Exists(Path.Combine(directory, "startup.json"))) return null;
            var startup = JsonSerializer.Deserialize<InstanceConfig>(File.ReadAllText(Path.Combine(directory, "startup.json")));
            if (startup is not { Equipment: not null } || startup.Kind != profile.Kind
                || container.GetProperty("Name").GetString() != "/" + ContainerName(startup.Id))
                return null;
            return (Path.Combine(directory, "control"), startup);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            logger.LogWarning(ex, "Startwerte eines {Device}-Containers konnten nicht gelesen werden.", Name);
        }
        return null;
    }

    private async Task<string?> RemoveStoppedContainerAsync(string id, CancellationToken cancellationToken)
    {
        var inspected = await DockerAsync(["container", "inspect", ContainerName(id)], cancellationToken);
        if (!inspected.Success) return null;
        using var json = JsonDocument.Parse(inspected.Output);
        var container = json.RootElement[0];
        if (ReadOwner(container) is not { } owner || owner.Startup.Id != id)
            return "Der Docker-Containername ist bereits anderweitig belegt.";
        if (container.GetProperty("State").GetProperty("Running").GetBoolean())
            return $"Der {Name}-Container läuft bereits.";
        var result = await DockerAsync(["rm", container.GetProperty("Id").GetString()!], cancellationToken);
        if (!result.Success) return $"Der gestoppte {Name}-Container konnte nicht entfernt werden. {result.Detail}";
        DeleteRunFiles(owner.ControlDirectory);
        return null;
    }

    public async Task<string?> EnsureImageAsync(CancellationToken cancellationToken = default)
    {
        var result = await DockerAsync(["image", "inspect", "--format", "{{.Id}}", profile.Image], cancellationToken);
        if (result.Success) return null;
        var engine = await DockerAsync(["info", "--format", "{{.OSType}}"], cancellationToken);
        if (!engine.Success)
            return $"Docker Desktop ist nicht erreichbar. Docker Desktop mit Linux-Containern starten. {engine.Detail}";
        if (engine.Output.Trim() != "linux")
            return $"Für {Name}-Container muss Docker Desktop Linux-Container verwenden.";

        var root = ProcessManager.FindRoot();
        var dockerfile = Path.Combine(root, "docker", $"{profile.Project}.Dockerfile");
        if (!File.Exists(dockerfile) || !Directory.Exists(Path.Combine(root, "src", "Devices", profile.Project)))
            return $"Das {Name}-Image fehlt und kann ohne Dockerfile und {Name}-Quellcode nicht automatisch erstellt werden. Bitte das vollständige Repository verwenden.";

        logger.LogInformation("{Device}-Image {Image} fehlt und wird automatisch erstellt. Der erste Build kann mehrere Minuten dauern.", Name, profile.Image);
        var build = await DockerAsync(["build", "--file", dockerfile, "--tag", profile.Image, root], cancellationToken,
            TimeSpan.FromMinutes(10));
        if (!build.Success)
        {
            logger.LogWarning("Automatischer {Device}-Image-Build fehlgeschlagen: {Output}\n{Error}", Name, build.Output, build.Error);
            var detail = build.Error.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .LastOrDefault() ?? build.Detail;
            return $"Das {Name}-Image konnte nicht automatisch erstellt werden. Beim ersten Build ist Internet erforderlich. Details stehen im Webdienst-Protokoll. {detail}";
        }
        var verified = await DockerAsync(["image", "inspect", "--format", "{{.Id}}", profile.Image], cancellationToken);
        return verified.Success ? null : $"Das erstellte {Name}-Image ist nicht verfügbar. {verified.Detail}";
    }

    public async Task<StartStepResult> StartAsync(InstanceConfig config, DeviceSessions sessions)
    {
        using var timeout = new CancellationTokenSource();
        var token = timeout.Token;
        var existing = await DiscoverAsync(token);
        if (existing.Any(run => run.Startup.Id == config.Id) || sessions.For(config.Id).Length > 0)
            return new(config.Id, false, $"Die {Name}-Instanz läuft bereits oder ihre Geräte-ID ist belegt.");
        if (config.ModbusPort is not int modbusPort || !PortProbe.IsFree(modbusPort))
            return new(config.Id, false, $"Modbus-Port {config.ModbusPort} fehlt oder ist belegt.");
        if (await EnsureImageAsync() is { } unavailable) return new(config.Id, false, unavailable);
        // Die Startfrist beginnt erst nach dem möglicherweise mehrminütigen Image-Build.
        timeout.CancelAfter(TimeSpan.FromSeconds(20));

        var directory = Path.Combine(RunsDirectory, Guid.NewGuid().ToString("N"));
        var controlDirectory = Path.Combine(directory, "control");
        string? containerId = null;
        var started = false;
        try
        {
            var logical = EquipmentOptions.Parse(config);
            var originalBind = logical.Modbus.Bind;
            logical = logical with { Modbus = logical.Modbus with { Bind = "127.0.0.1" } };
            var hostOutput = Path.GetFullPath(logical.OutputDirectory, ProcessManager.FindRoot());
            Directory.CreateDirectory(hostOutput);
            if (await RemoveStoppedContainerAsync(config.Id, token) is { } conflict)
                return new(config.Id, false, conflict);
            Directory.CreateDirectory(controlDirectory);
            await File.WriteAllTextAsync(Path.Combine(directory, "startup.json"),
                JsonSerializer.Serialize(config with { Equipment = logical }), token);

            var runtime = logical with
            {
                OutputDirectory = DataTarget,
                Modbus = logical.Modbus with { Bind = "0.0.0.0" },
                TimeSync = IsLoopback(logical.TimeSync.MasterHost)
                    ? logical.TimeSync with { MasterHost = "host.docker.internal" }
                    : logical.TimeSync,
                Weather = logical.Weather.Url is { } weather
                    && Uri.TryCreate(weather, UriKind.Absolute, out var url) && url.IsLoopback
                        ? logical.Weather with { Url = new UriBuilder(url) { Host = "host.docker.internal" }.Uri.AbsoluteUri }
                        : logical.Weather,
            };

            var result = await DockerAsync([
                "create", "--pull", "never", "--name", ContainerName(config.Id),
                "--label", $"{_runLabel}={Path.GetFileName(directory)}",
                "--label", $"{_deviceLabel}={config.Id}", "--restart", "no",
                "--env", $"{profile.ControlEnvironment}=1",
                "--publish", $"127.0.0.1:{modbusPort}:{modbusPort}", "--publish", $"127.0.0.1::{_containerControlPort}",
                "--mount", BindMount(hostOutput, DataTarget),
                "--mount", BindMount(controlDirectory, ControlTarget),
                profile.Image, .. EquipmentOptions.Arguments(config with { Equipment = runtime })
            ], token);
            if (IsContainerId(result.Output.Trim())) containerId = result.Output.Trim();
            if (!result.Success || containerId is null)
                return new(config.Id, false, $"{Name}-Container konnte nicht angelegt werden. {result.Detail}");
            var launch = await DockerAsync(["start", containerId], token);
            if (!launch.Success)
                return new(config.Id, false, $"{Name}-Container konnte nicht gestartet werden. {launch.Detail}");

            while (!token.IsCancellationRequested)
            {
                if (sessions.For(config.Id) is [var session] && session.ContainerId == containerId
                    && session.Startup.Kind == profile.Kind)
                {
                    started = true;
                    var note = originalBind == "0.0.0.0" ? " Modbus ist am Windows-Host nur lokal erreichbar." : "";
                    return new(config.Id, true, $"{Name}-Container gestartet.{note}");
                }
                var state = await DockerAsync(["container", "inspect", "--format", "{{.State.Running}}", containerId], token);
                if (!state.Success || state.Output.Trim() != "true") break;
                await Task.Delay(300, token);
            }
            var logs = await DockerAsync(["logs", "--tail", "6", containerId], CancellationToken.None);
            return new(config.Id, false, $"{Name}-Steuerzugang nicht bestätigt. {(string.IsNullOrWhiteSpace(logs.Error) ? logs.Output.Trim() : logs.Detail)}");
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            return new(config.Id, false, $"{Name}-Steuerzugang nach dem Start nicht rechtzeitig bestätigt.");
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            return new(config.Id, false, $"{Name}-Start fehlgeschlagen. {exception.Message}");
        }
        finally
        {
            if (!started && containerId is not null)
            {
                await DockerAsync(["stop", "--timeout", "5", containerId], CancellationToken.None);
                var removed = await DockerAsync(["rm", containerId], CancellationToken.None);
                if (removed.Success) DeleteRunFiles(controlDirectory);
            }
            else if (!started && containerId is null) DeleteRunFiles(controlDirectory);
        }
    }

    public async Task<StartStepResult> FinishStopAsync(string id, string containerId, bool useDockerStop,
        string? controlDirectory = null)
    {
        controlDirectory ??= _running.FirstOrDefault(run => run.ContainerId == containerId)?.ControlDirectory;
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(18));
        var result = await DockerAsync(useDockerStop ? ["stop", "--timeout", "10", containerId] : ["wait", containerId], timeout.Token);
        if (!result.Success) return new(id, false, $"{Name}-Container konnte nicht beendet werden. {result.Detail}");
        var remove = await DockerAsync(["rm", containerId], timeout.Token);
        if (remove.Success)
        {
            _running = _running.Where(run => run.ContainerId != containerId).ToArray();
            if (controlDirectory is not null) DeleteRunFiles(controlDirectory);
        }
        var detail = !useDockerStop && result.Output.Trim() != "0"
            ? $"{Name}-Container mit Exitcode {result.Output.Trim()} beendet." : $"{Name}-Container beendet.";
        return remove.Success ? new(id, true, detail)
            : new(id, false, $"{Name}-Container gestoppt, konnte aber nicht entfernt werden. {remove.Detail}");
    }

    private void DeleteRunFiles(string controlDirectory)
    {
        try
        {
            if (Directory.Exists(controlDirectory))
            {
                foreach (var file in Directory.EnumerateFiles(controlDirectory))
                {
                    var name = Path.GetFileName(file);
                    if (name.EndsWith(".json.tmp", StringComparison.Ordinal)) name = name[..^4];
                    if (Guid.TryParseExact(Path.GetFileNameWithoutExtension(name), "D", out _)
                        && name.EndsWith(".json", StringComparison.Ordinal))
                        File.Delete(file);
                }
                Directory.Delete(controlDirectory);
            }
            var directory = Path.GetDirectoryName(controlDirectory)!;
            if (Directory.Exists(directory))
            {
                File.Delete(Path.Combine(directory, "startup.json"));
                Directory.Delete(directory);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(exception, "Temporäre Registrierungsdateien eines {Device}-Containers konnten nicht entfernt werden.", Name);
        }
    }

    private static string BindMount(string path, string target) =>
        $"\"type=bind\",\"source={path.Replace("\"", "\"\"")}\",\"target={target}\"";

    private static bool IsLoopback(string host) => host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
        || (IPAddress.TryParse(host.Trim('[', ']'), out var address) && IPAddress.IsLoopback(address));

    private static bool IsContainerId(string value) => value.Length == 64 && value.All(Uri.IsHexDigit);

    private string ContainerName(string id) =>
        $"energytwin-{profile.Short}-" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(id))).ToLowerInvariant()[..24];

    private static async Task<DockerResult> DockerAsync(string[] arguments, CancellationToken cancellationToken,
        TimeSpan? commandTimeout = null)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(commandTimeout ?? TimeSpan.FromSeconds(15));
        var executable = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "Docker", "Docker", "resources", "bin", "docker.exe");
        var info = new ProcessStartInfo(File.Exists(executable) ? executable : "docker")
        {
            UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true
        };
        if (OperatingSystem.IsWindows())
        {
            info.ArgumentList.Add("--context");
            info.ArgumentList.Add("desktop-linux");
        }
        foreach (var argument in arguments) info.ArgumentList.Add(argument);
        try
        {
            using var process = Process.Start(info)!;
            var output = process.StandardOutput.ReadToEndAsync(timeout.Token);
            var error = process.StandardError.ReadToEndAsync(timeout.Token);
            try
            {
                await process.WaitForExitAsync(timeout.Token);
                return new(process.ExitCode, await output, await error);
            }
            catch (OperationCanceledException)
            {
                if (!process.HasExited)
                {
                    try { process.Kill(); }
                    catch (InvalidOperationException) { }
                }
                return new(-1, "", "Docker hat nicht rechtzeitig geantwortet.");
            }
        }
        catch (Win32Exception exception) { return new(-1, "", exception.Message); }
    }
}
