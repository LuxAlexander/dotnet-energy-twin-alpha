using System.Globalization;
using Devices.Common.Control;
using EnergyTwin.Web.Presentation;

namespace EnergyTwin.Web.Management;

public static class EquipmentOptions
{
    public static InstanceConfig FromStartup(ModuleKind kind, EquipmentStartup startup) =>
        new(startup.DeviceId, kind, startup.DeviceId, kind == ModuleKind.Charger ? "Ladesäulen" : "Kühlhäuser",
            startup.Weather.Location, startup.Modbus.Port, null, null) { Equipment = startup, Revision = -1 };

    public static EquipmentStartup Parse(InstanceConfig instance)
    {
        var arguments = Arguments(instance);
        if (instance.Kind == ModuleKind.Charger)
        {
            var config = DualChargerSim.IO.ArgsParser.Parse(arguments);
            ValidateTiming(config.StepMinutes, config.DurationHours, config.TimeSync.StandaloneStart);
            return new(config.DeviceId, config.PowerLimitKW, null, config.StepMinutes, config.DurationHours,
                config.Seed, config.OutputDirectory, config.TimeSync, config.Modbus, config.Weather);
        }

        var cold = KuehlhausSimulator.IO.ArgsParser.Parse(arguments);
        ValidateTiming(cold.StepMinutes, cold.DurationHours, cold.TimeSync.StandaloneStart);
        return new(cold.DeviceId, cold.CoolingPowerKW, cold.SetpointC, cold.StepMinutes, cold.DurationHours,
            cold.Seed, cold.OutputDirectory, cold.TimeSync, cold.Modbus, cold.Weather);
    }

    private static void ValidateTiming(double stepMinutes, double? durationHours, DateTime start)
    {
        if (!double.IsFinite(stepMinutes) || stepMinutes < TimeSpan.FromTicks(1).TotalMinutes
            || stepMinutes >= TimeSpan.MaxValue.TotalMinutes)
            throw new ArgumentException("Die Modellschrittweite liegt außerhalb des darstellbaren Zeitbereichs.");
        if (durationHours is double duration && (!double.IsFinite(duration) || duration >= (DateTime.MaxValue - start).TotalHours))
            throw new ArgumentException("Die Simulationsdauer liegt außerhalb des darstellbaren Zeitbereichs.");
    }

    public static string[] Arguments(InstanceConfig instance)
    {
        var setup = instance.Equipment;
        string[] common = ["--id", instance.Id, "--modbus-port", $"{instance.ModbusPort}", "--location", instance.Location];
        if (setup is null) return common;

        var time = setup.TimeSync;
        return
        [
            .. common,
            instance.Kind == ModuleKind.Charger ? "--power" : "--cooling", Number(setup.PowerKw),
            .. setup.SetpointC is double setpoint ? new[] { "--setpoint", Number(setpoint) } : [],
            "--step", Number(setup.StepMinutes), "--output", setup.OutputDirectory,
            "--modbus-bind", setup.Modbus.Bind, "--unit-id", $"{setup.Modbus.UnitId}",
            "--weather", setup.Weather.Url ?? "off",
            .. setup.Seed is int seed ? new[] { "--seed", $"{seed}" } : [],
            .. time.Standalone
                ? new[] { "--standalone", "--start", time.StandaloneStart.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    "--speed", Number(time.StandaloneSpeed) }
                : new[] { "--master", time.MasterEndpoint },
            .. time.Standalone && setup.DurationHours is double duration ? new[] { "--duration", Number(duration) } : []
        ];
    }

    public static string Number(double value) => value.ToString(CultureInfo.InvariantCulture);
}
