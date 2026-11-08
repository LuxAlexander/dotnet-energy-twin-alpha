using Modbus.Contracts;

namespace ModbusRabbitGateway;

/// <summary>Schwellen für den Qualitätsstatus <c>stale</c> (Nachrichtenvertrag §3.1).</summary>
/// <param name="StaleModelTime">
/// Modellzeit, die bei laufender Simulation ohne neue Gerätesequenz vergehen darf. Gemessen als reale Zeit × Tempo,
/// damit die Grenze bei jedem Tempo zur Schrittweite der Geräte (1 s bis einige Minuten Modellzeit) passt.
/// </param>
/// <param name="StaleMinimumReal">Reale Mindestwartezeit, damit kurze Aussetzer bei hohem Tempo nicht sofort zählen.</param>
public sealed record QualitySettings(TimeSpan StaleModelTime, TimeSpan StaleMinimumReal)
{
    public static QualitySettings Default { get; } = new(TimeSpan.FromHours(1), TimeSpan.FromSeconds(3));
}

/// <summary>Zustand je Gerät zwischen zwei Lesungen: Nachrichtenzähler, letzte Sequenz, letzter gültiger Stand.</summary>
public sealed class DeviceTracker
{
    public long PublishSeq { get; internal set; }
    public int? LastRunId { get; internal set; }
    public int? LastSeq { get; internal set; }
    public string? LastSimTime { get; internal set; }

    /// <summary>Reale Zeit der letzten Lesung, bei der sich Lauf oder Sequenz geändert hatten.</summary>
    public DateTimeOffset LastChangeAt { get; internal set; }
}

/// <summary>Ergebnis einer Dekodierung: entweder eine Nachricht oder ein Dekodierfehler.</summary>
public sealed record BuildResult(MeasurementMessage? Message, string? Error)
{
    public static BuildResult Failed(string error) => new(null, error);
}

/// <summary>
/// Dekodiert die Register 0–49 eines Geräts nach seinem Registerplan zu einer <see cref="MeasurementMessage"/>
/// (Nachrichtenvertrag §3). Keine Ein-/Ausgabe, keine Uhr: die Abfragezeit kommt als Parameter.
/// </summary>
public static class MessageBuilder
{
    /// <summary>Register, die im Umschlag der Nachricht stehen oder nur Befehle sind (§3.2); nicht als Messwert.</summary>
    private static readonly HashSet<int> NotMeasurements =
    [
        CommonRegisters.DeviceType, CommonRegisters.DeviceInstance, CommonRegisters.ModelTime, CommonRegisters.RunId,
        CommonRegisters.Sequence, CommonRegisters.TimeStatus, CommonRegisters.SimulationRunning, CommonRegisters.ResetCommand,
    ];

    /// <summary>Register des Plans, die als Messwert in die Nachricht kommen, nach Adresse.</summary>
    public static IEnumerable<RegisterDefinition> MeasuredRegisters(RegisterPlan plan) =>
        plan.Definitions.Where(d => !NotMeasurements.Contains(d.Address));

    public static BuildResult Build(DeviceEndpoint device, ReadOnlySpan<ushort> registers, DateTimeOffset polledAt,
        DeviceTracker tracker, QualitySettings settings)
    {
        if (registers.Length < CommonRegisters.TotalRegisters)
            return BuildResult.Failed($"Antwort zu kurz: {registers.Length} statt {CommonRegisters.TotalRegisters} Register.");

        ushort rawType = registers[CommonRegisters.DeviceType];
        var type = (DeviceType)rawType;
        if (!Enum.IsDefined(type) || RegisterPlans.For(type) is not { } plan)
            return BuildResult.Failed($"Unbekannter Gerätetyp {rawType} in Register 0 (kein Registerplan).");
        if (device.ExpectedType is { } expectedType && expectedType != type)
            return BuildResult.Failed($"Falsches Gerät: {device.DeviceId} erwartet {expectedType}, an {device.Endpoint} meldet sich {type}.");

        int instance = registers[CommonRegisters.DeviceInstance];
        if (instance != device.ExpectedInstance)
            return BuildResult.Failed(
                $"Falsches Gerät: {device.DeviceId} erwartet Geräte-Nummer {device.ExpectedInstance}, gelesen {instance}.");

        uint epoch = RegisterCodec.DecodeUInt32(registers[CommonRegisters.ModelTime], registers[CommonRegisters.ModelTime + 1]);
        int runId = registers[CommonRegisters.RunId];
        int seq = registers[CommonRegisters.Sequence];
        var timeStatus = (TimeStatus)registers[CommonRegisters.TimeStatus];
        bool running = registers[CommonRegisters.SimulationRunning] == 1;
        double speed = RegisterCodec.DecodeFloat32(registers[CommonRegisters.Speed], registers[CommonRegisters.Speed + 1]);

        bool changed = tracker.LastSeq != seq || tracker.LastRunId != runId;
        if (changed)
            tracker.LastChangeAt = polledAt;

        var measurements = new List<Measurement>();
        bool allFinite = true;
        foreach (var definition in MeasuredRegisters(plan))
        {
            double value = RegisterCodec.Decode(definition, registers.Slice(definition.Address, definition.Count));
            if (!double.IsFinite(value))
            {
                allFinite = false;
                measurements.Add(new Measurement(MeasurementKeys.From(definition.Name), definition.Name, definition.Address, null, definition.Unit));
                continue;
            }

            if (definition.Type == RegisterType.Float32)
                value = Math.Round(value, 3);
            measurements.Add(new Measurement(MeasurementKeys.From(definition.Name), definition.Name, definition.Address, value,
                definition.Unit, definition.LabelFor(value)));
        }

        var (quality, reason) = Assess(epoch, timeStatus, running, speed, changed, polledAt - tracker.LastChangeAt, settings);
        if (!allFinite && quality == Contract.Quality.Good)
            (quality, reason) = (Contract.Quality.Uncertain, Contract.Reason.InvalidValue);

        string simTime = Contract.FormatSimTime(RegisterCodec.FromSeconds(epoch));
        tracker.PublishSeq++;
        tracker.LastRunId = runId;
        tracker.LastSeq = seq;
        tracker.LastSimTime = simTime;

        return new BuildResult(new MeasurementMessage(
            Contract.SchemaVersion, device.DeviceId, type.ToString(), simTime, epoch, runId, seq, tracker.PublishSeq,
            Contract.FormatUtc(polledAt), quality, reason, measurements), null);
    }

    /// <summary>Qualitätsstatus nach Vertrag §3.1, schwerster Grund zuerst.</summary>
    public static (string Quality, string Reason) Assess(uint epoch, TimeStatus timeStatus, bool running, double speed,
        bool changed, TimeSpan sinceChange, QualitySettings settings)
    {
        if (timeStatus == TimeStatus.NoTime || epoch == 0)
            return (Contract.Quality.Bad, Contract.Reason.NoTime);
        if (timeStatus == TimeStatus.MasterLost)
            return (Contract.Quality.Bad, Contract.Reason.MasterLost);

        if (running && !changed && sinceChange >= settings.StaleMinimumReal
            && sinceChange.TotalSeconds * Math.Max(speed, 0) >= settings.StaleModelTime.TotalSeconds)
            return (Contract.Quality.Uncertain, Contract.Reason.Stale);

        if (timeStatus == TimeStatus.Standalone)
            return (Contract.Quality.Uncertain, Contract.Reason.Standalone);

        return running ? (Contract.Quality.Good, Contract.Reason.Running) : (Contract.Quality.Good, Contract.Reason.Paused);
    }
}
