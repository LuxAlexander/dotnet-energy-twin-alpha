using System.Text;

namespace Shared.Data;

/// <summary>
/// Schreibt tabellarische Daten asynchron als CSV. Die Klasse legt die Datei an, schreibt die Kopfzeile
/// und hängt danach jede Zeile an. Feldinhalte werden bei Bedarf in Anführungszeichen gesetzt.
/// </summary>
public sealed class CsvFileWriter : IAsyncDisposable
{
    private readonly StreamWriter _writer;
    private readonly char _separator;

    public string FilePath { get; }

    private CsvFileWriter(string filePath, StreamWriter writer, char separator)
    {
        FilePath = filePath;
        _writer = writer;
        _separator = separator;
    }

    /// <summary>
    /// Legt den Ordner und eine Datei <c>&lt;filePrefix&gt;_&lt;Datum_Uhrzeit&gt;.csv</c> an und schreibt die Kopfzeile.
    /// </summary>
    public static async Task<CsvFileWriter> CreateAsync(
        string filePrefix,
        string directory,
        string header,
        CancellationToken cancellationToken,
        char separator = ';',
        bool byteOrderMark = true)
    {
        if (string.IsNullOrWhiteSpace(filePrefix))
            throw new ArgumentException("Der Dateiname braucht einen Präfix.", nameof(filePrefix));
        if (string.IsNullOrWhiteSpace(directory))
            throw new ArgumentException("Der Zielordner darf nicht leer sein.", nameof(directory));

        Directory.CreateDirectory(directory);

        string safePrefix = string.Concat(filePrefix.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        string filePath = Path.Combine(directory, $"{safePrefix}_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv");

        var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.Asynchronous);
        var writer = new StreamWriter(stream, new UTF8Encoding(byteOrderMark));
        await writer.WriteLineAsync(header.AsMemory(), cancellationToken);
        await writer.FlushAsync(cancellationToken);
        return new CsvFileWriter(filePath, writer, separator);
    }

    /// <summary>Schreibt eine Datenzeile; Werte mit Trennzeichen, Anführungszeichen oder Zeilenumbruch werden maskiert.</summary>
    public Task WriteRowAsync(IReadOnlyList<string> values, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(values);

        string line = string.Join(_separator, values.Select(Escape));
        return _writer.WriteLineAsync(line.AsMemory(), cancellationToken);
    }

    public Task FlushAsync(CancellationToken cancellationToken) => _writer.FlushAsync(cancellationToken);

    public ValueTask DisposeAsync() => _writer.DisposeAsync();

    private string Escape(string value)
    {
        if (value.IndexOfAny([_separator, '"', '\r', '\n']) < 0)
            return value;

        return '"' + value.Replace("\"", "\"\"") + '"';
    }
}
