using System.Globalization;
using Batterie_Sim;
using EnergyTwin.Web.Presentation;

namespace EnergyTwin.Web.Management;

public static class BatteryStartup
{
    public static InstanceConfig FromStartup(SimulatorOptions startup) =>
        new(startup.Config.DeviceId, ModuleKind.Battery, startup.Config.DeviceId, "Batteriespeicher",
            startup.Weather.Location, startup.Modbus.Port, null, null) { Battery = startup, Revision = -1 };

    public static string[] Arguments(InstanceConfig instance)
    {
        var startup = instance.Battery;
        string[] common = ["--id", instance.Id, "--modbus-port", $"{instance.ModbusPort}", "--location", instance.Location];
        if (startup is null) return common;

        var model = startup.Config;
        var time = startup.TimeSync;
        return
        [
            .. common,
            "--capacity", Number(model.CapacityKWh),
            "--max-charge", Number(model.MaxChargeKW), "--max-discharge", Number(model.MaxDischargeKW),
            "--soc-min", Number(model.SocMinPercent), "--soc-max", Number(model.SocMaxPercent),
            "--soc-start", Number(model.StartSocPercent),
            "--step", Number(startup.StepSeconds),
            "--weather", startup.Weather.Url ?? "off",
            "--modbus-bind", startup.Modbus.Bind, "--unit-id", $"{startup.Modbus.UnitId}",
            .. startup.Seed is int seed ? new[] { "--seed", $"{seed}" } : [],
            .. time.Standalone
                ? new[] { "--standalone", "--start", time.StandaloneStart.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    "--speed", Number(time.StandaloneSpeed) }
                : new[] { "--master", time.MasterEndpoint },
            .. time.Standalone && startup.DurationHours is double duration ? new[] { "--duration", Number(duration) } : []
        ];
    }

    public static string Number(double value) => value.ToString(CultureInfo.InvariantCulture);
}
