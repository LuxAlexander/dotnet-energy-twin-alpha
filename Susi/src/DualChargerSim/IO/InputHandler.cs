using System.Globalization;
using DualChargerSim.Models;
using DualChargerSim.Simulation;

namespace DualChargerSim.IO;

/// <summary>
/// Verarbeitet Tastaturbefehle während der laufenden Simulation.
/// </summary>
public class InputHandler
{
    private const double PowerStepKW = 10;
    private const double SpeedFactor = 2;

    private readonly SimulationEngine _engine;
    private readonly ConsoleRenderer _renderer;

    public InputHandler(SimulationEngine engine, ConsoleRenderer renderer)
    {
        _engine = engine;
        _renderer = renderer;
    }

    public void Handle(ConsoleKeyInfo key)
    {
        var charger = _engine.Charger;
        var point = _engine.SelectedPoint;
        string name = $"Punkt {point.Number}";

        switch (char.ToUpperInvariant(key.KeyChar))
        {
            case 'P':
                _engine.IsPaused = !_engine.IsPaused;
                _engine.Message = _engine.IsPaused ? "Pausiert – P zum Fortsetzen." : "Fortgesetzt.";
                break;
            case 'A':
                _engine.IsAutoMode = !_engine.IsAutoMode;
                _engine.Message = _engine.IsAutoMode
                    ? "Auto-Modus an – Fahrzeuge kommen und fahren von selbst."
                    : "Auto-Modus aus – N: Fahrzeug kommt, D: Abstecken.";
                break;
            case 'N':
                if (_engine.IsAutoMode)
                    _engine.Message = "N nur im manuellen Modus – im Auto-Modus kommen Fahrzeuge von selbst.";
                else
                    _engine.VehicleArrives();
                break;
            case '1':
            case '2':
                _engine.SelectedPointIndex = key.KeyChar - '1';
                _engine.Message = $"Punkt {key.KeyChar} gewählt.";
                break;
            case 'C':
                ConnectVehicle(point);
                break;
            case 'D':
                _engine.Message = point.Disconnect()
                    ? $"{name}: Fahrzeug abgesteckt."
                    : $"{name}: Abstecken nicht möglich (Zustand {point.State}).";
                break;
            case 'S':
                ToggleCharging(point, name);
                break;
            case 'F':
                point.TriggerFault();
                _engine.Message = $"{name}: Störung ausgelöst (Kühlungsdefekt, {point.TemperatureC:0} °C).";
                break;
            case 'R':
                _engine.Message = point.Reset()
                    ? $"{name}: Störung zurückgesetzt."
                    : point.State == ChargingPointState.Faulted
                        ? $"{name}: Reset erst unter {ChargingPoint.ManualResetTemperatureC:0} °C möglich (aktuell {point.TemperatureC:0} °C)."
                        : $"{name}: Keine Störung aktiv.";
                break;
            case '+':
                ChangePowerLimit(charger, +PowerStepKW);
                break;
            case '-':
                ChangePowerLimit(charger, -PowerStepKW);
                break;
            case '>':
                ChangeSpeed(SpeedFactor);
                break;
            case '<':
                ChangeSpeed(1 / SpeedFactor);
                break;
            case 'O':
                if (charger.IsOn) charger.SwitchOff(); else charger.SwitchOn();
                _engine.Message = $"Säule {(charger.IsOn ? "eingeschaltet" : "ausgeschaltet")}.";
                break;
            case 'Q':
                _engine.Quit();
                break;
        }
    }

    private void ChangePowerLimit(DualCharger charger, double deltaKW)
    {
        double before = charger.PowerLimitKW;
        charger.PowerLimitKW += deltaKW;

        _engine.Message = charger.PowerLimitKW == before
            ? $"Gesamtleistung bleibt bei {before:0} kW (erlaubt: {DualCharger.MinPowerLimitKW:0}–{DualCharger.MaxPowerLimitKW:0} kW)."
            : $"Gesamtleistung auf {charger.PowerLimitKW:0} kW gesetzt.";
    }

    private void ChangeSpeed(double factor)
    {
        double before = _engine.Speed;
        _engine.Speed *= factor;

        _engine.Message = _engine.Speed == before
            ? $"Tempo bleibt bei {before:0.##}x (erlaubt: {SimulationConfig.MinSpeed:0}–{SimulationConfig.MaxSpeed:0}x)."
            : $"Tempo auf {_engine.Speed:0.##}x gesetzt ({_engine.RealDelay.TotalSeconds:0.###} s pro Schritt).";
    }

    private void ToggleCharging(ChargingPoint point, string name)
    {
        if (point.State == ChargingPointState.Charging)
        {
            point.Stop();
            _engine.Message = $"{name}: Laden gestoppt.";
        }
        else
        {
            _engine.Message = point.Start()
                ? $"{name}: Laden gestartet."
                : $"{name}: Start nicht möglich (Zustand {point.State}).";
        }
    }

    /// <summary>Modell und Start-SOC wählen; die maximale Ladeleistung gehört zum Modell.</summary>
    private void ConnectVehicle(ChargingPoint point)
    {
        string name = $"Punkt {point.Number}";
        if (!point.IsFree)
        {
            _engine.Message = $"{name}: Anstecken nicht möglich (Zustand {point.State}).";
            return;
        }

        var models = VehicleCatalog.Models;
        var info = models
            .Select((m, i) => $"  {i + 1}) {m.Name,-22} {m.BatteryCapacityKWh,4:0} kWh  {m.MaxChargePowerKW,4:0} kW")
            .Prepend($"Fahrzeug an {name} anstecken (leere Eingabe = Abbruch):")
            .ToList();

        string choice = _renderer.Prompt("Modell Nr.:", info);
        if (!int.TryParse(choice, out int index) || index < 1 || index > models.Count)
        {
            _engine.Message = "Anstecken abgebrochen.";
            return;
        }

        double? soc = AskPercent("Start-SOC in % [20]:", 20);
        double? target = soc is null ? null : AskPercent("Ziel-SOC in % [100]:", 100);
        if (soc is null || target is null)
        {
            _engine.Message = "Ungültiger SOC – Anstecken abgebrochen.";
            return;
        }

        var model = models[index - 1];
        try
        {
            point.Connect(VehicleCatalog.Create(model, soc.Value, target.Value));
            _engine.Message = $"{name}: {model.Name} angesteckt – S zum Starten.";
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _engine.Message = $"Anstecken abgebrochen: {ex.Message}";
        }
    }

    private double? AskPercent(string question, double defaultValue)
    {
        string input = _renderer.Prompt(question);
        if (input.Length == 0)
            return defaultValue;

        return double.TryParse(input.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
               && value is >= 0 and <= 100
            ? value
            : null;
    }
}
