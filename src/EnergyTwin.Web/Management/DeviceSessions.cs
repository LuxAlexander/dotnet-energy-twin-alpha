using DualChargerSim.Simulation;
using KuehlhausSimulator.Simulation;
using SimulationConfig = EnergySimulator.Simulation.SimulationConfig;
using Batterie_Sim;
using System.Globalization;
using Devices.Common.Control;
using EnergySimulator.Simulation;
using EnergyTwin.Web.Presentation;

namespace EnergyTwin.Web.Management;

public sealed class DeviceSessions(InstanceStore store, ILogger<DeviceSessions> logger, PvContainers pvContainers,
    ColdStoreContainers coldStoreContainers, ChargerContainers chargerContainers) : BackgroundService
{
    private readonly HttpClient _client = new(new SocketsHttpHandler { UseProxy = false, AllowAutoRedirect = false })
    {
        Timeout = TimeSpan.FromSeconds(4)
    };
    private volatile Session[] _sessions = [];

    public sealed record Session(DeviceRegistration Registration, InstanceConfig Startup,
        PvControlState? PvState = null, BatteryControlState? BatteryState = null,
        EquipmentState<ChargerControlState>? ChargerState = null, EquipmentState<ColdStoreControlState>? ColdStoreState = null,
        string? ContainerId = null);
    public Session[] For(string id) => _sessions.Where(session => session.Registration.Id == id
        && DeviceControl.IsRegistered(session.Registration)).ToArray();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        do
        {
            try
            {
                var pvDocker = await pvContainers.DiscoverAsync(stoppingToken);
                var equipmentDocker = (await coldStoreContainers.DiscoverAsync(stoppingToken))
                    .Select(run => (Run: run, coldStoreContainers.Profile))
                    .Concat((await chargerContainers.DiscoverAsync(stoppingToken))
                        .Select(run => (Run: run, chargerContainers.Profile)));
                var sessions = await Task.WhenAll(DeviceControl.Discover()
                    .Select(device => ProbeAsync(device, stoppingToken))
                    .Concat(pvDocker.SelectMany(run => DeviceControl.Discover(run.ControlDirectory)
                        .Where(device => device.Kind == "PV" && device.Id == run.Startup.Id)
                        .Select(device => ProbeAsync(device with { Port = run.ControlPort }, stoppingToken, run))))
                    .Concat(equipmentDocker.SelectMany(entry => DeviceControl.Discover(entry.Run.ControlDirectory)
                        .Where(device => device.Kind == entry.Profile.ControlKind
                            && device.Id == entry.Run.Startup.Id && entry.Run.Startup.Kind == entry.Profile.Kind)
                        .Select(device => ProbeAsync(device with { Port = entry.Run.ControlPort }, stoppingToken, entry.Run)))));
                _sessions = sessions.OfType<Session>().ToArray();
                store.SetDiscovered(_sessions.Select(session => session.Startup));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
            {
                logger.LogWarning(ex, "Geräteerkennung konnte nicht gelesen werden.");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task<Session?> ProbeAsync(DeviceRegistration device, CancellationToken cancellationToken,
        DeviceContainerRun? container = null)
    {
        switch (device.Kind)
        {
            case "PV":
                var pv = (await SendAsync<PvControlState>(device, null, cancellationToken))?.State;
                if (pv?.Startup.DeviceId != device.Id) return null;
                if (container is null) return new(device, FromStartup(pv.Startup), PvState: pv);
                return new(device, container.Startup with { Revision = -1 },
                    PvState: pv with { Startup = container.Startup.Pv! }, ContainerId: container.ContainerId);
            case BatteryControlState.Kind:
                var battery = (await SendAsync<BatteryControlState>(device, null, cancellationToken))?.State;
                return battery?.Startup.Config.DeviceId == device.Id
                    ? new(device, BatteryStartup.FromStartup(battery.Startup), BatteryState: battery) : null;
            case DualChargerSim.Simulation.SimulationEngine.ControlKind:
                var charger = (await SendAsync<EquipmentState<ChargerControlState>>(device, null, cancellationToken))?.State;
                if (charger?.Startup.DeviceId != device.Id) return null;
                if (container is null) return new(device, EquipmentOptions.FromStartup(ModuleKind.Charger, charger.Startup),
                    ChargerState: charger);
                return new(device, container.Startup with { Revision = -1 },
                    ChargerState: charger with { Startup = container.Startup.Equipment! }, ContainerId: container.ContainerId);
            case KuehlhausSimulator.Simulation.SimulationEngine.ControlKind:
                var coldStore = (await SendAsync<EquipmentState<ColdStoreControlState>>(device, null, cancellationToken))?.State;
                if (coldStore?.Startup.DeviceId != device.Id) return null;
                if (container is null) return new(device, EquipmentOptions.FromStartup(ModuleKind.ColdStore, coldStore.Startup),
                    ColdStoreState: coldStore);
                return new(device, container.Startup with { Revision = -1 },
                    ColdStoreState: coldStore with { Startup = container.Startup.Equipment! }, ContainerId: container.ContainerId);
            default:
                return null;
        }
    }

    public async Task<DeviceReply<T>?> SendAsync<T>(DeviceRegistration device, DeviceCommand? command,
        CancellationToken cancellationToken)
    {
        try { return await DeviceControl.RequestAsync<T>(_client, device, command, cancellationToken); }
        catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException or System.Text.Json.JsonException)
        { return null; }
    }

    public static bool Supports(ModuleKind kind) => kind is ModuleKind.Pv or ModuleKind.Battery or ModuleKind.Charger or ModuleKind.ColdStore;

    public static InstanceConfig FromStartup(SimulationConfig startup) =>
        new(startup.DeviceId, ModuleKind.Pv, startup.DeviceId, "PV-Anlagen", startup.Location,
            startup.Modbus.Port, null, null) { Pv = startup, Revision = -1 };

    public override void Dispose()
    {
        base.Dispose();
        _client.Dispose();
    }
}

public static class PvStartup
{
    public static string[] Arguments(InstanceConfig instance)
    {
        var config = instance.Pv;
        string[] common = ["--id", instance.Id, "--modbus-port", $"{instance.ModbusPort}", "--location", instance.Location];
        if (config is null) return common;
        var time = config.TimeSync;
        return
        [
            .. common,
            "--peak", config.PeakPowerKw.ToString(CultureInfo.InvariantCulture),
            "--step", $"{config.StepMinutes}", "--model-step", config.ModelStepMinutes.ToString(CultureInfo.InvariantCulture),
            "--refresh", $"{config.RefreshMs}", "--output", config.OutputDirectory,
            "--weather", config.WeatherUrl ?? "off",
            "--modbus-bind", config.Modbus.Bind, "--unit-id", $"{config.Modbus.UnitId}",
            .. config.Seed is int seed ? new[] { "--seed", $"{seed}" } : [],
            .. time.Standalone
                ? new[] { "--standalone", "--start", time.StandaloneStart.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    "--speed", time.StandaloneSpeed.ToString(CultureInfo.InvariantCulture) }
                : new[] { "--master", time.MasterEndpoint }
        ];
    }
}
