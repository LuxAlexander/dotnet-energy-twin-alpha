using EnergyTwin.Web.Management;
using EnergyTwin.Web.Presentation;

namespace EnergyTwin.Web.Live;

public sealed record PlantIssue(string Key, string Title, string Detail, string? Target, string? StartFailureId = null);
public sealed record PlantInstance(InstanceConfig Config, InstanceRunState ProcessState, string Status, bool Running,
    Metric? Metric, PlantIssue[] Issues);
public sealed record PlantSnapshot(PlantInstance[] Instances, PlantIssue[] Issues, LiveSnapshot Mqtt);

/// <summary>Gemeinsame Befunde für Anlagenübersicht und Fehlerdiagnose.</summary>
public sealed class PlantStatus(InstanceStore instances, ProcessManager processes, DeviceSessions sessions,
    LiveStore store, MasterControlClient master, IConfiguration configuration)
{
    public PlantSnapshot Read()
    {
        var mqtt = store.Snapshot();
        var now = DateTimeOffset.UtcNow;
        var masterStatus = master.Snapshot();
        var gatewayId = configuration["Mqtt:GatewayId"] ?? "GW-01";
        var metrics = new OverviewMetrics(mqtt, masterStatus, gatewayId);
        var topics = mqtt.Connected
            ? mqtt.Messages.Where(message => message.Generation == mqtt.Generation).ToDictionary(message => message.Topic)
            : new Dictionary<string, LiveMessage>();

        var gateway = Fresh($"energytwin/gateway/{gatewayId}/status", 45);
        var gatewayOnline = gateway is not null && LiveStore.Text(gateway.Data, "state") == "online";
        var result = new List<PlantInstance>();
        var issues = new List<PlantIssue>();
        foreach (var instance in instances.Instances)
        {
            var state = processes.State(instance);
            var active = DeviceSessions.Supports(instance.Kind) ? sessions.For(instance.Id) : [];
            var running = active.Length > 0 || state == InstanceRunState.ManagedRunning
                || instance.Kind == ModuleKind.Broker && mqtt.Connected
                || instance.Kind == ModuleKind.Gateway && gatewayOnline
                || instance.Kind == ModuleKind.Master && masterStatus.Available;
            var faults = new List<PlantIssue>();
            var target = "/module/" + Uri.EscapeDataString(instance.Id) + "?ansicht=einstellungen";
            void Fault(string key, string title, string detail) =>
                faults.Add(new($"{instance.Id}:{key}", title, detail, target));

            if (processes.StartFailure(instance.Id) is { } failed && !(running && failed.Detail == "Läuft bereits."))
                faults.Add(new($"{instance.Id}:start", "Start oder Neustart fehlgeschlagen", failed.Detail, target, instance.Id));
            if (active.Length > 1 || active.Any(session => session.Startup.Kind != instance.Kind))
                Fault("duplicate", "Geräte-ID mehrfach belegt", "Die gleichzeitigen Prozesse sperren die Bedienung.");
            else if (instance.Kind == ModuleKind.Pv && active.Length == 0 && processes.HasPvContainer(instance.Id))
                Fault("control", "PV-Steuerzugang antwortet nicht", "Der Container läuft. Prüfe seinen Zustand und die Docker-Logs.");
            else if (active.Length == 0 && DeviceSessions.Supports(instance.Kind) && state == InstanceRunState.ManagedRunning)
                Fault("control", "Steuerzugang antwortet nicht", "Der verwaltete Prozess läuft, meldet aber keinen bestätigten Steuerzustand.");

            if (active is [var device] && device.Startup.Kind == instance.Kind)
            {
                if (device.PvState is { } pv)
                {
                    if (pv.InverterFault) Fault("inverter", "Wechselrichterfehler", "Die simulierte Wechselrichterstörung ist aktiv.");
                    if (pv.Overheat) Fault("overheat", "Überhitzung", "Die erzwungene Überhitzung ist aktiv.");
                    if (pv.GridFault) Fault("grid", "Netzstörung", "Die simulierte Netzstörung ist aktiv.");
                }
                if (device.BatteryState is { } battery && battery.Fault != Batterie_Sim.FaultType.None)
                    Fault("battery", battery.Fault == Batterie_Sim.FaultType.EmergencyStop ? "Not-Aus aktiv" : "Batterieüberhitzung",
                        "Details und Rücksetzen stehen in den Geräteeinstellungen.");
                if (device.ColdStoreState is { Device.Fault: not KuehlhausSimulator.FaultType.None } cold)
                    Fault("cold-store", cold.Device.Fault == KuehlhausSimulator.FaultType.CompressorFailure ? "Kompressorausfall" : "Übertemperatur",
                        "Prüfe Solltemperatur, Tür und den Zustand des Kompressors.");
                if (device.ChargerState is { } charger)
                    foreach (var point in charger.Device.Points.Where(point => point.State == DualChargerSim.Models.ChargingPointState.Faulted))
                        Fault($"port-{point.Number}", $"Störung an Ladepunkt {point.Number}", "Der Ladepunkt meldet einen Fehler.");
            }

            if (DeviceSessions.Supports(instance.Kind))
            {
                var topic = $"energytwin/devices/{instance.Id}";
                var status = Fresh($"{topic}/status", 45);
                var message = Fresh($"{topic}/measurements", 15);
                var deviceState = status is null ? "" : LiveStore.Text(status.Data, "state");
                if (deviceState == "decodeError" || deviceState == "offline" && running)
                {
                    var detail = LiveStore.Text(status!.Data, "error");
                    Fault("modbus", deviceState == "offline" ? "Gateway erreicht Modbus nicht" : "Gateway kann Register nicht lesen",
                        detail == "—" ? "Prüfe Port, Unit-ID und Gerätezuordnung." : detail);
                }
                if (active.Length == 0 && message is not null && deviceState is not ("offline" or "decodeError")
                    && LiveStore.Text(message.Data, "quality") == "good"
                    && LiveView.Find(LiveView.Device(message, status), "betriebszustand")?.Value == 3)
                    Fault("reported-fault", "Gerät meldet Störung", "Der aktuelle Modbus-Datensatz meldet den Fehlerzustand.");
                if (running && message is not null && masterStatus.Status?.Current.Mode != SimCore.Contracts.SimMode.Paused
                    && LiveStore.Text(message.Data, "quality") == "stale")
                    Fault("stale", "Modellwerte aktualisieren sich nicht", "Das Gateway meldet veraltete Modellwerte.");
            }
            if (instance.Kind == ModuleKind.Gateway && state == InstanceRunState.ManagedRunning && gateway is not null && !gatewayOnline)
                Fault("gateway", "Gateway meldet sich offline", "Prüfe die Verbindung des Gateways zu RabbitMQ.");
            if (instance.Kind == ModuleKind.Broker && !mqtt.Connected && mqtt.Connection.StartsWith("Verbindung fehlgeschlagen", StringComparison.Ordinal))
                Fault("mqtt", "Web-App erreicht RabbitMQ nicht", mqtt.Connection);
            if (instance.Kind == ModuleKind.Broker && !mqtt.Connected && mqtt.Generation > 0
                && !faults.Any(fault => fault.Key == $"{instance.Id}:mqtt"))
                Fault("mqtt", "MQTT-Verbindung unterbrochen", mqtt.Connection);

            var label = faults.Count > 0 ? "Prüfen"
                : instance.Kind == ModuleKind.Master && masterStatus.Available && masterStatus.Status?.Current.Mode == SimCore.Contracts.SimMode.Paused ? "Pausiert"
                : running ? "Läuft" : state == InstanceRunState.ExternallyReachable ? "Port erreichbar" : "Gestoppt";
            result.Add(new(instance, state, label, running, metrics.For(instance), faults.ToArray()));
            issues.AddRange(faults);
        }
        return new(result.ToArray(), issues.ToArray(), mqtt);

        LiveMessage? Fresh(string topic, int seconds) =>
            topics.TryGetValue(topic, out var message) && now - message.ReceivedAt <= TimeSpan.FromSeconds(seconds) ? message : null;
    }
}
