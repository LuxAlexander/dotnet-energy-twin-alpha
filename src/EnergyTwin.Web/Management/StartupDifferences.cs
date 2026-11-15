using System.Globalization;
using EnergyTwin.Web.Presentation;

namespace EnergyTwin.Web.Management;

public sealed record StartupDifference(string Label, string Active, string Saved);

public static class StartupDifferences
{
    private static readonly Dictionary<string, string> Labels = new()
    {
        ["--peak"] = "Nennleistung (kW)", ["--capacity"] = "Kapazität (kWh)",
        ["--power"] = "Gesamtleistung (kW)", ["--cooling"] = "Kälteleistung (kW)", ["--setpoint"] = "Start-Solltemperatur (°C)",
        ["--soc-start"] = "Start-SOC (%)", ["--soc-min"] = "SOC-Untergrenze (%)", ["--soc-max"] = "SOC-Obergrenze (%)",
        ["--max-charge"] = "Max. Ladeleistung (kW)", ["--max-discharge"] = "Max. Entladeleistung (kW)",
        ["--step"] = "Schrittweite / CSV-Intervall", ["--model-step"] = "Modellschrittweite (min)",
        ["--start"] = "Virtuelle Startzeit", ["--speed"] = "Starttempo", ["--duration"] = "Simulationsdauer (h)",
        ["--master"] = "Master-Adresse", ["--weather"] = "Wetterquelle", ["--location"] = "Standort",
        ["--modbus-port"] = "Modbus-Port", ["--modbus-bind"] = "Modbus-Bind-Adresse", ["--unit-id"] = "Unit-ID",
        ["--output"] = "CSV-Ordner", ["--refresh"] = "Konsolenintervall (ms)", ["--seed"] = "Zufalls-Seed",
        ["--standalone"] = "Betriebsart", ["--no-modbus"] = "Modbus"
    };

    public static StartupDifference[] Compare(InstanceConfig active, InstanceConfig saved)
    {
        if (active.Kind != saved.Kind || saved.Revision < 0) return [];
        var left = Values(active);
        var right = Values(saved);
        return Labels.Keys.Where(key => left.GetValueOrDefault(key) != right.GetValueOrDefault(key))
            .Select(key => new StartupDifference(Labels[key], left.GetValueOrDefault(key, "—"), right.GetValueOrDefault(key, "—")))
            .ToArray();
    }

    private static Dictionary<string, string> Values(InstanceConfig config)
    {
        var arguments = config.Kind switch
        {
            ModuleKind.Pv => PvStartup.Arguments(config.Pv is not null ? config : config with
            {
                Pv = EnergySimulator.Simulation.ArgsParser.Parse(PvStartup.Arguments(config))
            }),
            ModuleKind.Battery => BatteryStartup.Arguments(config.Battery is not null ? config : config with
            {
                Battery = Batterie_Sim.ArgsParser.Parse(BatteryStartup.Arguments(config))
            }),
            ModuleKind.Charger or ModuleKind.ColdStore => EquipmentOptions.Arguments(config.Equipment is not null ? config : config with
            {
                Equipment = EquipmentOptions.Parse(config)
            }),
            _ => []
        };
        var values = new Dictionary<string, string>
        {
            ["--standalone"] = "Master",
            ["--no-modbus"] = config.Pv?.Modbus.Disabled == true || config.Battery?.Modbus.Disabled == true
                || config.Equipment?.Modbus.Disabled == true ? "Aus" : "Ein"
        };
        for (var index = 0; index < arguments.Length; index++)
        {
            var key = arguments[index];
            if (key == "--standalone") { values[key] = "Standalone"; continue; }
            var value = arguments[++index];
            if (key == "--location") value = value.ToLowerInvariant();
            if (key == "--weather" && value == "off") value = "Aus";
            if (key == "--output") value = Path.GetFullPath(value, ProcessManager.FindRoot());
            if (key == "--duration" || key == "--seed" || key is "--peak" or "--capacity" or "--setpoint" or "--power" or "--cooling"
                or "--soc-start" or "--soc-min" or "--soc-max" or "--max-charge" or "--max-discharge" or "--step" or "--model-step" or "--speed")
            {
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                    value = number.ToString(CultureInfo.GetCultureInfo("de-AT"));
            }
            values[key] = value;
        }
        return values;
    }
}
