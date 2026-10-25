using System.Globalization;
using System.Text;
using Shared.Config;

namespace Batterie_Sim;

/// <summary>Schreibt pro Simulationsschritt eine Zeile in die CSV-Datei (asynchron, Punkt als Dezimaltrenner).</summary>
public sealed class CsvLogger : IAsyncDisposable
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    private const string Header =
        "SimTime;DeviceId;Mode;Control;SetpointKW;ActualKW;SOC;TempC;Fault;MaxChargeKW;MaxDischargeKW;SocMin;SocMax;Lauf";

    private readonly StreamWriter _writer;
    private readonly BatterieConfig _config;

    public string FilePath { get; }

    private CsvLogger(string filePath, StreamWriter writer, BatterieConfig config)
    {
        FilePath = filePath;
        _writer = writer;
        _config = config;
    }

    /// <summary>Legt Datei in <c>docs/data</c> an (Geräte-ID und Startzeit im Namen) und schreibt die Kopfzeile.</summary>
    public static async Task<CsvLogger> CreateAsync(BatterieConfig config, CancellationToken cancellationToken)
    {
        // Zentral in <Repo>/docs/data, egal aus welchem Ordner gestartet wird
        Directory.CreateDirectory(OutputPaths.CsvDirectory);
        string safeId = string.Concat(config.DeviceId.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        string path = Path.Combine(OutputPaths.CsvDirectory, $"battery_{safeId}_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv");

        var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.Asynchronous);
        var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        await writer.WriteLineAsync(Header.AsMemory(), cancellationToken);
        await writer.FlushAsync(cancellationToken);
        return new CsvLogger(path, writer, config);
    }

    public async Task LogAsync(DateTime simTime, int runId, Batterie battery, ControlMode control, CancellationToken cancellationToken)
    {
        string line = string.Join(';',
            simTime.ToString("yyyy-MM-dd HH:mm:ss", Invariant),
            _config.DeviceId,
            battery.State,
            control,
            battery.SetpointKW.ToString("F2", Invariant),
            battery.ActualKW.ToString("F2", Invariant),
            battery.SocPercent.ToString("F2", Invariant),
            battery.TemperaturC.ToString("F2", Invariant),
            battery.ActiveFault,
            _config.MaxChargeKW.ToString("F2", Invariant),
            _config.MaxDischargeKW.ToString("F2", Invariant),
            _config.SocMinPercent.ToString("F2", Invariant),
            _config.SocMaxPercent.ToString("F2", Invariant),
            runId.ToString(Invariant));

        await _writer.WriteLineAsync(line.AsMemory(), cancellationToken);
    }

    public Task FlushAsync(CancellationToken cancellationToken) => _writer.FlushAsync(cancellationToken);

    public ValueTask DisposeAsync() => _writer.DisposeAsync();
}
