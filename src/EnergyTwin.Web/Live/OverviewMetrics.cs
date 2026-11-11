using EnergyTwin.Web.Management;
using EnergyTwin.Web.Presentation;
using SimCore.Contracts;

namespace EnergyTwin.Web.Live;

/// <summary>Aktuelle Kurzwerte für Startseite und Kartenübersicht.</summary>
public sealed class OverviewMetrics(LiveSnapshot snapshot, (MasterStatus? Status, bool Available) master, string gatewayId)
{
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    public Metric? For(InstanceConfig instance)
    {
        if (instance.ModbusPort is not null)
        {
            var topic = $"energytwin/devices/{instance.Id}";
            var measurements = Fresh($"{topic}/measurements", 15);
            var status = Fresh($"{topic}/status", 45);
            if (measurements is null || LiveStore.Text(measurements.Data, "quality") != "good" ||
                (status is not null && LiveStore.Text(status.Data, "state") is "offline" or "decodeError"))
                return null;
            var metric = LiveView.DeviceMetrics(instance.Kind, LiveView.Device(measurements, status)).FirstOrDefault();
            return metric?.Value == "—" ? null : metric;
        }

        return instance.Kind switch
        {
            ModuleKind.Master when master is { Available: true, Status: { } status } =>
                new("Virtuelle Zeit", $"{status.SimTime:HH:mm:ss}", "", "purple",
                    status.Current.Mode == SimMode.Paused ? "Pausiert" : "Läuft"),
            ModuleKind.Weather => WeatherMetric(),
            ModuleKind.Broker => new("MQTT-Verbindung", snapshot.Connected ? "Verbunden" : "Getrennt", "",
                snapshot.Connected ? "green" : "red", ""),
            ModuleKind.Gateway when Fresh($"energytwin/gateway/{gatewayId}/status", 45) is { } gateway =>
                LiveStore.Text(gateway.Data, "state") switch
                {
                    "online" => new("MQTT-Status", "Online gemeldet", "", "green", ""),
                    "offline" => new("MQTT-Status", "Offline gemeldet", "", "red", ""),
                    _ => null
                },
            _ => null
        };
    }

    private Metric? WeatherMetric()
    {
        var measurements = Fresh("energytwin/weather/salzburg/measurements", 15);
        var status = Fresh("energytwin/weather/status", 45);
        if (measurements is null || status is null ||
            LiveStore.Text(status.Data, "status") is not ("paused" or "continued"))
            return null;

        foreach (var field in new[] { "sourceSession", "masterSession", "runId" })
        {
            var value = LiveStore.Text(measurements.Data, field);
            if (value == "—" || value != LiveStore.Text(status.Data, field)) return null;
        }

        var weather = LiveView.Weather(measurements);
        var temperature = weather is null ? null : LiveView.Measurement(weather.Values, "temperature_c");
        if (temperature is null) return null;
        var description = LiveStore.Text(status.Data, "status") == "paused" ? "Pausierter Stand" : "";
        return new("Temperatur · Salzburg", LiveView.Format(temperature), "°C", "orange", description);
    }

    private LiveMessage? Fresh(string topic, int maxAgeSeconds) => snapshot.Connected
        ? snapshot.Messages.FirstOrDefault(message => message.Topic == topic &&
            message.Generation == snapshot.Generation && _now - message.ReceivedAt <= TimeSpan.FromSeconds(maxAgeSeconds))
        : null;
}
