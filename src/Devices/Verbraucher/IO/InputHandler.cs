using System.Globalization;
using Devices.Common;
using Verbraucher.Simulation;

namespace Verbraucher.IO;

/// <summary>
/// Verarbeitet Tastaturbefehle während der laufenden Simulation. Die Tasten sind bewusst andere als beim
/// Dual-Port-Charger, damit sie nicht verwechselt werden (siehe Tastenhilfe in der Anzeige).
/// </summary>
public class InputHandler
{
    private static readonly TimeSpan QuitPollInterval = TimeSpan.FromMilliseconds(50);
    private const double SpeedFactor = 2;
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-AT");

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
        var room = _engine.Room;

        switch (char.ToUpperInvariant(key.KeyChar))
        {
            case 'T':
                _engine.ToggleDoor();
                break;
            case 'S':
                await AskSetpointAsync(cancellationToken);
                break;
            case 'F':
                room.TriggerFault(FaultType.CompressorFailure);
                _engine.Message = string.Create(Culture,
                    $"Kompressorausfall ausgelöst (Raum {room.RoomTempC:0.0} °C). Die Kühlung steht sofort – R zum Quittieren.");
                break;
            case 'R':
                _engine.Message = room.ResetFault()
                    ? "Störung zurückgesetzt."
                    : room.ActiveFault == FaultType.OverTemperature
                        ? string.Create(Culture,
                            $"Reset erst unter {room.ResetAlarmBelowC:0.0} °C möglich (Raum {room.RoomTempC:0.0} °C).")
                        : "Keine Störung aktiv.";
                break;
            case 'O':
                if (room.IsOn) room.TurnOff(); else room.TurnOn();
                _engine.Message = $"Anlage {(room.IsOn ? "eingeschaltet" : "ausgeschaltet")}.";
                break;
            case 'A':
                _engine.ToggleMode();
                break;
            case 'P':
                _engine.Message = _engine.Time.TogglePause();
                break;
            case '>':
                _engine.Message = _engine.Time.ChangeSpeed(SpeedFactor);
                break;
            case '<':
                _engine.Message = _engine.Time.ChangeSpeed(1 / SpeedFactor);
                break;
            case 'Q':
                _engine.Quit();
                break;
        }
    }

    /// <summary>Solltemperatur eingeben; leere Eingabe behält den Wert, Esc bricht ab.</summary>
    private async Task AskSetpointAsync(CancellationToken cancellationToken)
    {
        var room = _engine.Room;
        string question = string.Create(Culture,
            $"Neue Solltemperatur in °C [{Kuehlhaus.MinSetpointC} … {Kuehlhaus.MaxSetpointC}, aktuell {room.SetpointC:0.0}]:");
        string input = await _renderer.PromptAsync(question, cancellationToken);

        if (input.Length == 0)
        {
            _engine.Message = "Solltemperatur unverändert.";
            return;
        }

        if (!double.TryParse(input.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
            || !double.IsFinite(value))
        {
            _engine.Message = $"'{input}' ist keine Zahl – Solltemperatur unverändert.";
            return;
        }

        _engine.TrySetSetpoint(value, out string message);
        _engine.Message = message;
    }
}
