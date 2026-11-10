using EnergyTwin.Web.Presentation;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnergyTwin.Web.Management;

/// <summary>Gespeicherte Instanzkonfiguration. Speichern allein startet keinen Prozess.</summary>
public sealed record InstanceConfig(
    string Id, ModuleKind Kind, string Name, string Group, string Location,
    [property: JsonPropertyName("modbusPort")] int? ModbusPort,
    [property: JsonPropertyName("httpPort")] int? HttpPort,
    [property: JsonPropertyName("speed")] int? Speed);

/// <summary>
/// Lokal persistierte Instanzverwaltung: Anlegen, Entfernen, Konfliktprüfung je Kennung und Port.
/// Ohne Datei erscheint der bekannte Ausgangsstand.
/// </summary>
public sealed class InstanceStore
{
    private static readonly ModuleKind[] AddableKinds = [ModuleKind.Pv, ModuleKind.Battery, ModuleKind.Charger, ModuleKind.ColdStore];
    public static IReadOnlyList<ModuleKind> Addable => AddableKinds;
    public const int MasterPort = 5100;

    private readonly object _gate = new();
    private readonly string _path;
    private readonly int _webPort;
    private List<InstanceConfig> _instances;
    private string? _lastError;

    public InstanceStore(string path, int webPort)
    {
        _path = path;
        _webPort = webPort;
        _instances = LoadOrSeed();
    }

    public string FilePath => _path;

    public IReadOnlyList<InstanceConfig> Instances
    {
        get { lock (_gate) return _instances.ToArray(); }
    }

    public InstanceConfig? Find(string id)
    {
        lock (_gate) return _instances.FirstOrDefault(instance => instance.Id == id);
    }

    public string? LastError
    {
        get { lock (_gate) return _lastError; }
    }

    /// <summary>Fehlerkennung oder null, falls die Instanz anlegbar ist.</summary>
    public InstanceConfig? TryAdd(InstanceConfig candidate)
    {
        lock (_gate)
        {
            var error = Validate(candidate, _instances);
            if (error is not null) { _lastError = error; return null; }
            _instances.Add(candidate);
            _lastError = null;
            Persist();
            return candidate;
        }
    }

    public bool Remove(string id)
    {
        lock (_gate)
        {
            var removed = _instances.RemoveAll(instance => instance.Id == id) > 0;
            if (removed) { _lastError = null; Persist(); }
            return removed;
        }
    }

    /// <summary>Frei verwendbare Kennung nach Typmuster vorschlagen, z. B. PV-02.</summary>
    public string SuggestId(ModuleKind kind)
    {
        lock (_gate)
        {
            var prefix = kind switch
            {
                ModuleKind.Pv => "PV-",
                ModuleKind.Battery => "BAT-",
                ModuleKind.Charger => "DC-",
                ModuleKind.ColdStore => "KH-",
                _ => "X-"
            };
            for (var number = 1; number < 200; number++)
            {
                var candidate = $"{prefix}{number:00}";
                if (_instances.All(instance => instance.Id != candidate)) return candidate;
            }
            return $"{prefix}XX";
        }
    }

    /// <summary>Ersten freien Modbus-Port ab 1502 vorschlagen; vorab freier Port bis zum Start ist nicht garantiert.</summary>
    public int SuggestModbusPort()
    {
        lock (_gate)
        {
            var used = _instances
                .Select(instance => instance.ModbusPort ?? int.MaxValue)
                .Concat([_webPort, MasterPort])
                .ToHashSet();
            for (var port = 1502; port < 65000; port++)
            {
                if (used.Contains(port)) continue;
                if (PortProbe.IsFree(port)) return port;
            }
            return 0;
        }
    }

    /// <summary>Startreihenfolge der Anlage: Time Master zuerst (Zeitquelle für alle Geräte), dann Gateway → Wetter → Geräte. Master startet pausiert.</summary>
    public IReadOnlyList<InstanceConfig> StartupOrder()
    {
        lock (_gate) return _instances
            .OrderBy(instance => instance.Kind switch
            {
                ModuleKind.Master => 0,
                ModuleKind.Gateway => 1,
                ModuleKind.Weather => 2,
                ModuleKind.Pv or ModuleKind.Battery or ModuleKind.Charger or ModuleKind.ColdStore => 3,
                _ => 4
            })
            .ToArray();
    }

    private string? Validate(InstanceConfig candidate, List<InstanceConfig> existing)
    {
        if (string.IsNullOrWhiteSpace(candidate.Id) || candidate.Id.Length > 40) return "Kennung fehlt oder ist zu lang.";
        if (!AddableKinds.Contains(candidate.Kind)) return "Nur Geräteinstanzen (PV, Batterie, Ladesäule, Kühlhaus) sind anlegbar.";
        if (existing.Any(instance => instance.Id == candidate.Id)) return $"Kennung {candidate.Id} ist bereits belegt.";
        var ports = existing
            .Select(instance => instance.ModbusPort ?? instance.HttpPort ?? int.MaxValue)
            .Concat([_webPort, MasterPort])
            .ToHashSet();
        if (candidate.ModbusPort is not { } modbusPort) return "Modbus-Port fehlt.";
        if (modbusPort is < 1024 or > 65535) return "Modbus-Port muss zwischen 1024 und 65535 liegen.";
        if (ports.Contains(modbusPort)) return $"Port {modbusPort} ist in den gespeicherten Instanzen bereits belegt.";
        return null;
    }

    private void Persist()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_path, JsonSerializer.Serialize(_instances, options));
        }
        catch (IOException)
        {
            // Übernahme bleibt wirksam, das Speichern kann an Rechte oder Sperre scheitern.
            _lastError = "Änderung übernommen, aber lokales Speichern fehlgeschlagen.";
        }
    }

    private List<InstanceConfig> LoadOrSeed()
    {
        try
        {
            if (File.Exists(_path))
                return JsonSerializer.Deserialize<List<InstanceConfig>>(File.ReadAllText(_path)) ?? Seed();
        }
        catch (Exception ex) when (ex is IOException or JsonException) { }
        var seeded = Seed();
        PersistFirst(seeded);
        return seeded;
    }

    private void PersistFirst(List<InstanceConfig> instances)
    {
        try { File.WriteAllText(_path, JsonSerializer.Serialize(instances, new JsonSerializerOptions { WriteIndented = true })); }
        catch (IOException) { }
    }

    private List<InstanceConfig> Seed() => new()
    {
        new("master", ModuleKind.Master, "Master", "Simulation", "Gemeinsame Zeitquelle", null, null, 600),
        new("weather", ModuleKind.Weather, "Wetter", "Simulation", "Fünf Standorte", null, 8080, null),
        new("gateway", ModuleKind.Gateway, "Gateway", "Infrastruktur", "Modbus ↔ MQTT", null, null, null),
        new("broker", ModuleKind.Broker, "RabbitMQ", "Infrastruktur", "Lokaler Broker", null, null, null),
        new("PV-01", ModuleKind.Pv, "PV-01", "PV-Anlagen", "Salzburg", 1502, null, null),
        new("BAT-01", ModuleKind.Battery, "BAT-01", "Batteriespeicher", "Salzburg", 1503, null, null),
        new("DC-01", ModuleKind.Charger, "DC-01", "Ladesäulen", "Salzburg", 1504, null, null),
        new("KH-01", ModuleKind.ColdStore, "KH-01", "Kühlhäuser", "Salzburg", 1505, null, null),
    };
}

/// <summary>Kurze TCP-Bind-Prüfung; keine Reservierung über das eigentliche Starten hinaus.</summary>
public static class PortProbe
{
    public static bool IsFree(int port)
    {
        try
        {
            using var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, port);
            listener.Start();
            listener.Stop();
            return true;
        }
        catch (System.Net.Sockets.SocketException) { return false; }
    }
}
