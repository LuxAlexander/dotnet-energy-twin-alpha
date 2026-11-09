namespace EnergyTwin.Web.Presentation;

public enum ModuleKind { Master, Weather, Pv, Battery, Charger, ColdStore, Broker, Gateway }

public sealed record Metric(string Label, string Value, string Unit, string Tone, string Description);
public sealed record ModulePreview(string Id, string Name, string Group, ModuleKind Kind,
    string Location, IReadOnlyList<Metric> Metrics);

/// <summary>Unveränderlicher Ansichtskatalog; ausdrücklich kein Laufzeit- oder Gerätezustand.</summary>
public sealed class PreviewCatalog
{
    public IReadOnlyList<ModulePreview> Modules { get; } = Array.AsReadOnly(new[]
    {
        Create("master", "Master", "Simulation", ModuleKind.Master, "Gemeinsame Zeitquelle",
            new("Virtuelle Zeit", "12:00:00", "UTC", "purple", "15. Juli 2026 · Beispiel"),
            new("Tempo", "60", "×", "purple", "Virtuelle Sekunden / reale Sekunde"),
            new("Simulationslauf", "7", "", "neutral", "Beispielkennung")),
        Create("weather", "Wetter", "Simulation", ModuleKind.Weather, "Fünf Standorte",
            new("Temperatur", "24,0", "°C", "orange", "Salzburg · Beispiel"),
            new("Einstrahlung", "820", "W/m²", "gold", "Salzburg · Beispiel"),
            new("Wind", "3,2", "m/s", "blue", "Salzburg · Beispiel")),
        Create("PV-01", "PV-01", "PV-Anlagen", ModuleKind.Pv, "Salzburg",
            new("Wirkleistung", "−12,4", "kW", "green", "Einspeisung · Beispiel"),
            new("Einstrahlung", "820", "W/m²", "gold", "Wetter am Standort"),
            new("Modultemperatur", "42,6", "°C", "orange", "Beispielwert")),
        Create("PV-02", "PV-02", "PV-Anlagen", ModuleKind.Pv, "Wien",
            new("Wirkleistung", "−8,1", "kW", "green", "Einspeisung · Beispiel"),
            new("Einstrahlung", "650", "W/m²", "gold", "Wetter am Standort"),
            new("Modultemperatur", "37,2", "°C", "orange", "Beispielwert")),
        Create("BAT-01", "BAT-01", "Batteriespeicher", ModuleKind.Battery, "Salzburg",
            new("Ladezustand", "68", "%", "green", "Beispielwert"),
            new("Wirkleistung", "4,2", "kW", "blue", "Laden · Beispiel"),
            new("Gespeicherte Energie", "13,6", "kWh", "green", "Beispielwert")),
        Create("DC-01", "DC-01", "Ladesäulen", ModuleKind.Charger, "Salzburg",
            new("Gesamtleistung", "62", "kW", "blue", "Beide Anschlüsse · Beispiel"),
            new("Ladepunkte", "2", "", "neutral", "Getrennte Bedienbereiche"),
            new("Leistungsgrenze", "100", "kW", "purple", "Beispiel-Sollwert")),
        Create("KH-01", "KH-01", "Kühlhäuser", ModuleKind.ColdStore, "Salzburg",
            new("Innentemperatur", "−18,2", "°C", "blue", "Beispielwert"),
            new("Solltemperatur", "−18,0", "°C", "orange", "Beispiel-Sollwert"),
            new("Leistungsaufnahme", "8,4", "kW", "purple", "Beispielwert")),
        Create("broker", "RabbitMQ", "Infrastruktur", ModuleKind.Broker, "Lokaler Broker",
            new Metric("Verbindung", "Nicht angebunden", "", "neutral", "Status wird noch nicht abgefragt")),
        Create("gateway", "Gateway", "Infrastruktur", ModuleKind.Gateway, "Modbus ↔ MQTT",
            new Metric("Verbindung", "Nicht angebunden", "", "neutral", "Status wird noch nicht abgefragt"))
    });

    public ModulePreview? Find(string id) => Modules.FirstOrDefault(module => module.Id == id);

    private static ModulePreview Create(string id, string name, string group, ModuleKind kind,
        string location, params Metric[] metrics) =>
        new(id, name, group, kind, location, Array.AsReadOnly(metrics));
}
