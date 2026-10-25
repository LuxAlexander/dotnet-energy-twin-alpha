namespace Devices.Common;

/// <summary>Was seit dem letzten <see cref="StepScheduler.Update"/> passiert ist.</summary>
public enum StepUpdateKind
{
    /// <summary>Noch keine Zeit bekannt.</summary>
    NoTime,

    /// <summary>Normaler Fortschritt (auch 0 Schritte).</summary>
    Advance,

    /// <summary>Erste Zeit erhalten: Modellzeit beginnt bei <see cref="StepUpdate.ModelTime"/>.</summary>
    Started,

    /// <summary>Neuer Lauf (Neustart im Master): Modellzeit springt auf die neue Zeit.</summary>
    Restarted
}

/// <param name="FirstStepAt">Beginn des ersten fälligen Schritts.</param>
/// <param name="StepCount">Anzahl der fälligen Schritte (jeweils <see cref="StepScheduler.Step"/> lang).</param>
/// <param name="SkippedSteps">Übersprungene Schritte, weil mehr als <see cref="StepScheduler.MaxCatchUpSteps"/> fällig waren.</param>
/// <param name="ModelTime">Modellzeit nach diesen Schritten (Zeit des dargestellten Zustands).</param>
public readonly record struct StepUpdate(StepUpdateKind Kind, DateTime FirstStepAt, int StepCount, long SkippedSteps, DateTime ModelTime);

/// <summary>
/// Übersetzt die gemeinsame virtuelle Zeit in feste Modellschritte: Das Modell rechnet immer mit gleicher
/// Schrittweite und hinkt der Uhr höchstens einen Schritt hinterher. Läuft die Uhr schneller als die Schleife,
/// werden mehrere Schritte nachgeholt (gleiches Ergebnis wie ohne Verzögerung).
/// </summary>
public sealed class StepScheduler
{
    public const int DefaultMaxCatchUpSteps = 10_000;

    public StepScheduler(TimeSpan step, int maxCatchUpSteps = DefaultMaxCatchUpSteps)
    {
        if (step <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(step), "Die Schrittweite muss positiv sein.");
        if (maxCatchUpSteps < 1)
            throw new ArgumentOutOfRangeException(nameof(maxCatchUpSteps));
        Step = step;
        MaxCatchUpSteps = maxCatchUpSteps;
    }

    public TimeSpan Step { get; }
    public int MaxCatchUpSteps { get; }

    /// <summary>Zeit des zuletzt berechneten Modellzustands; <c>null</c> vor der ersten Zeit.</summary>
    public DateTime? ModelTime { get; private set; }

    /// <summary>Lauf, zu dem <see cref="ModelTime"/> gehört.</summary>
    public int RunId { get; private set; }

    /// <param name="clockNow">Aktuelle virtuelle Zeit der gemeinsamen Uhr.</param>
    /// <param name="runId">Aktueller Lauf; ein Wechsel bedeutet Neustart mit neuer Zeit.</param>
    public StepUpdate Update(DateTime clockNow, int runId)
    {
        if (ModelTime is not { } modelTime || runId != RunId)
        {
            var kind = ModelTime is null ? StepUpdateKind.Started : StepUpdateKind.Restarted;
            ModelTime = clockNow;
            RunId = runId;
            return new StepUpdate(kind, clockNow, 0, 0, clockNow);
        }

        // Die Uhr kann kurz hinter der Modellzeit liegen (verspätete Pause); dann einfach warten.
        long due = Math.Max(0, (clockNow - modelTime).Ticks / Step.Ticks);
        long skipped = 0;
        if (due > MaxCatchUpSteps)
        {
            skipped = due - MaxCatchUpSteps;
            modelTime += Step * skipped;
            due = MaxCatchUpSteps;
        }

        ModelTime = modelTime + Step * due;
        return new StepUpdate(StepUpdateKind.Advance, modelTime, (int)due, skipped, ModelTime.Value);
    }
}
