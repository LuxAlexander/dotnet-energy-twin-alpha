using System.Globalization;
using Devices.Common;
using DualChargerSim.Models;
using DualChargerSim.Simulation;

namespace DualChargerSim.IO;

/// <summary>
/// Verarbeitet Tastaturbefehle während der laufenden Simulation.
/// </summary>
public class InputHandler
{
    private static readonly TimeSpan QuitPollInterval = TimeSpan.FromMilliseconds(50);
    private const double PowerStepKW = 10;
    private const double SpeedFactor = 2;

    private readonly SimulationEngine _engine;
    private readonly ConsoleRenderer _renderer;

    public InputHandler(SimulationEngine engine, ConsoleRenderer renderer)
    {
        _engine = engine;
        _renderer = renderer;
    }

    /// <summary>Verarbeitet alle bereits gedrückten Tasten, ohne auf neue zu warten.</summary>
    /// <returns><c>true</c>, wenn mindestens eine Taste verarbeitet wurde.</returns>
    public async Task<bool> HandlePendingKeysAsync(CancellationToken cancellationToken)
    {
        bool handled = false;
        while (_engine.IsRunning && AsyncConsole.TryReadKey() is { } key)
        {
            await HandleAsync(key, cancellationToken);
            handled = true;
        }
        return handled;
    }

    /// <summary>Wartet (ohne zu blockieren) auf Q; bei umgeleiteter Eingabe sofort fertig.</summary>
    public async Task WaitForQuitAsync(CancellationToken cancellationToken)
    {
        while (AsyncConsole.HasKeyboard)
        {
            if (AsyncConsole.TryReadKey() is { Key: ConsoleKey.Q })
                return;
            await Task.Delay(QuitPollInterval, cancellationToken);
        }
    }

    public async Task HandleAsync(ConsoleKeyInfo key, CancellationToken cancellationToken)
    {
        var charger = _engine.Charger;
        var point = _engine.SelectedPoint;
        string name = $"Punkt {point.Number}";

        switch (char.ToUpperInvariant(key.KeyChar))
        {
            case 'P':
                _engine.Message = _engine.Time.TogglePause();
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
                await ConnectVehicleAsync(point, cancellationToken);
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
                _engine.Message = _engine.Time.ChangeSpeed(SpeedFactor);
                break;
            case '<':
                _engine.Message = _engine.Time.ChangeSpeed(1 / SpeedFactor);
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
    private async Task ConnectVehicleAsync(ChargingPoint point, CancellationToken cancellationToken)
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
            .Prepend($"Fahrzeug an {name} anstecken (leere Eingabe oder Esc = Abbruch):")
            .ToList();

        string choice = await _renderer.PromptAsync("Modell Nr.:", cancellationToken, info);
        if (!int.TryParse(choice, out int index) || index < 1 || index > models.Count)
        {
            _engine.Message = "Anstecken abgebrochen.";
            return;
        }

        double? soc = await AskPercentAsync("Start-SOC in % [20]:", 20, cancellationToken);
        double? target = soc is null ? null : await AskPercentAsync("Ziel-SOC in % [100]:", 100, cancellationToken);
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

    private async Task<double?> AskPercentAsync(string question, double defaultValue, CancellationToken cancellationToken)
    {
        string input = await _renderer.PromptAsync(question, cancellationToken);
        if (input.Length == 0)
            return defaultValue;

        return double.TryParse(input.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
               && value is >= 0 and <= 100
            ? value
            : null;
    }
}
