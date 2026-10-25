using System.Text;

namespace Devices.Common;

/// <summary>
/// Konsoleneingabe ohne blockierendes Warten: Tasten werden mit <see cref="Task.Delay(TimeSpan, CancellationToken)"/>
/// abgefragt, statt in <see cref="Console.ReadLine"/> oder <see cref="Console.ReadKey()"/> zu hängen.
/// Die Zeitsynchronisation und andere Tasks laufen währenddessen weiter.
/// </summary>
public static class AsyncConsole
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(20);

    public static bool HasKeyboard => !Console.IsInputRedirected;

    /// <summary>Nächste Taste, falls eine gedrückt wurde (wartet nicht).</summary>
    public static ConsoleKeyInfo? TryReadKey() =>
        HasKeyboard && Console.KeyAvailable ? Console.ReadKey(intercept: true) : null;

    /// <summary>
    /// Liest eine Zeile mit Echo, Rücktaste und Esc (= Abbruch, Ergebnis <c>null</c>).
    /// Bei umgeleiteter Eingabe wird die nächste Zeile der Eingabe gelesen.
    /// </summary>
    public static async Task<string?> ReadLineAsync(int maxLength, CancellationToken cancellationToken)
    {
        if (!HasKeyboard)
            return await Console.In.ReadLineAsync(cancellationToken);

        var input = new StringBuilder();
        while (true)
        {
            while (!Console.KeyAvailable)
                await Task.Delay(PollInterval, cancellationToken);

            var key = Console.ReadKey(intercept: true);
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    await Console.Out.WriteLineAsync();
                    return input.ToString();
                case ConsoleKey.Escape:
                    await Console.Out.WriteLineAsync();
                    return null;
                case ConsoleKey.Backspace:
                    if (input.Length > 0)
                    {
                        input.Length--;
                        await Console.Out.WriteAsync("\b \b");
                    }
                    break;
                default:
                    if (!char.IsControl(key.KeyChar) && input.Length < maxLength)
                    {
                        input.Append(key.KeyChar);
                        await Console.Out.WriteAsync(key.KeyChar);
                    }
                    break;
            }
        }
    }
}
