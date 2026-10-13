namespace Shared.Config;

/// <summary>
/// Gemeinsame Ablageorte aller Programme. CSV-Ausgaben landen zentral in <c>docs/data</c> im Repository,
/// unabhängig davon, aus welchem Ordner (Repo-Root, Projektordner, bin/Debug/…) das Programm gestartet wird.
/// </summary>
public static class OutputPaths
{
    /// <summary>Datei, an der der Repository-Hauptordner erkannt wird.</summary>
    public const string SolutionFileName = "EnergyTwin.sln";

    /// <summary>
    /// Standardordner für CSV-Ausgaben: <c>&lt;Repo&gt;/docs/data</c>.
    /// Liegt das Programm außerhalb des Repos (z. B. veröffentlicht), wird <c>docs/data</c> im Arbeitsverzeichnis verwendet.
    /// </summary>
    public static string CsvDirectory { get; } =
        Path.Combine(FindRepositoryRoot() ?? Environment.CurrentDirectory, "docs", "data");

    /// <summary>Sucht vom Programmordner aufwärts nach <see cref="SolutionFileName"/>.</summary>
    private static string? FindRepositoryRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, SolutionFileName)))
                return dir.FullName;
        }

        return null;
    }
}
