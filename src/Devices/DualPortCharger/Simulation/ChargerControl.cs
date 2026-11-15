using Devices.Common.Control;
using DualChargerSim.Models;
using System.Text.Json;

namespace DualChargerSim.Simulation;

public sealed record ChargerControlState(bool IsOn, bool IsAuto, double PowerKw, ChargerPointStatus[] Points);
public sealed record ChargerPointStatus(int Number, ChargingPointState State, string? Vehicle);
public sealed record VehicleSelection(string Model, double StartSoc, double TargetSoc);

public partial class SimulationEngine
{
    public const string ControlKind = "Charger";

    private SimulationControl<EquipmentState<ChargerControlState>> CreateControl()
    {
        var startup = new EquipmentStartup(Config.DeviceId, Config.PowerLimitKW, null, Config.StepMinutes,
            Config.DurationHours, Config.Seed, Path.GetFullPath(Config.OutputDirectory), Config.TimeSync, Config.Modbus, Config.Weather);
        return new(() => new(startup, !Time.IsRunning, Time.Clock.Speed,
            new(Charger.IsOn, IsAutoMode, Charger.PowerLimitKW,
                Charger.Points.Select(point => new ChargerPointStatus(point.Number, point.State, point.Vehicle?.ModelName)).ToArray())),
            ApplyControl);
    }

    private string? ApplyControl(DeviceCommand command)
    {
        switch (command.Action)
        {
            case "stop": Quit(); return null;
            case "pause" or "speed": return SimulationControl.ApplyTimeCommand(Time, command);
            case "mode" when bool.TryParse(command.Value, out var auto): IsAutoMode = auto; return null;
            case "power" when bool.TryParse(command.Value, out var enabled):
                if (enabled) Charger.SwitchOn();
                else Charger.SwitchOff();
                return null;
            case "limit" when SimulationControl.TryNumber(command.Value, out var limit)
                && limit is >= DualCharger.MinPowerLimitKW and <= DualCharger.MaxPowerLimitKW:
                Charger.PowerLimitKW = limit;
                return null;
            case "arrival" when !IsAutoMode && SimulationTime is not null:
                VehicleArrives();
                return null;
        }

        var parts = command.Action.Split(':', 2);
        if (parts.Length != 2 || !int.TryParse(parts[1], out var number) || number < 1 || number > Charger.Points.Count)
            return "Die Aktion ist im aktuellen Betrieb nicht verfügbar.";

        var point = Charger.Points[number - 1];
        return parts[0] switch
        {
            "connect" => Connect(point, command.Value),
            "disconnect" => point.Vehicle is null || point.Disconnect() ? null : "Abstecken ist im aktuellen Zustand nicht möglich.",
            "charge" when bool.TryParse(command.Value, out var enabled) =>
                enabled ? point.State == ChargingPointState.Charging || point.Start() ? null : "Laden kann nicht gestartet werden."
                    : point.State != ChargingPointState.Charging || point.Stop() ? null : "Laden kann nicht angehalten werden.",
            "fault" => TriggerFault(point),
            "reset" => point.State != ChargingPointState.Faulted || point.Reset()
                ? null : $"Reset erst unter {ChargingPoint.ManualResetTemperatureC:0} °C möglich.",
            _ => "Die Aktion ist im aktuellen Betrieb nicht verfügbar."
        };
    }

    private static string? TriggerFault(ChargingPoint point)
    {
        if (point.State != ChargingPointState.Faulted) point.TriggerFault();
        return null;
    }

    private static string? Connect(ChargingPoint point, string? value)
    {
        if (!point.IsFree || value is null) return "Der Ladepunkt ist nicht frei.";
        try
        {
            var selection = JsonSerializer.Deserialize<VehicleSelection>(value);
            var model = VehicleCatalog.Models.FirstOrDefault(model => model.Name == selection?.Model);
            if (selection is null || model is null || !double.IsFinite(selection.StartSoc) || !double.IsFinite(selection.TargetSoc))
                return "Fahrzeug und Ladezustände prüfen.";
            return point.Connect(VehicleCatalog.Create(model, selection.StartSoc, selection.TargetSoc))
                ? null : "Anstecken ist im aktuellen Zustand nicht möglich.";
        }
        catch (Exception exception) when (exception is JsonException or ArgumentOutOfRangeException)
        {
            return "Fahrzeug sowie Start- und Ziel-SOC prüfen.";
        }
    }
}
