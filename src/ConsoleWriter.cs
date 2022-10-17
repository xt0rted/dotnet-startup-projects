namespace StartupProjects;

using System.Globalization;

internal class ConsoleWriter : IConsoleWriter
{
    private readonly IConsole _console;
    private readonly IFormatProvider? _consoleFormatProvider;

    public ConsoleWriter(IConsole console, IFormatProvider consoleFormatProvider)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _consoleFormatProvider = consoleFormatProvider ?? throw new ArgumentNullException(nameof(consoleFormatProvider));
    }

    private static AnsiControlCode TextDim { get; } = $"{Ansi.Esc}[2m";

    private void WriteLine(AnsiControlCode modifierOn, AnsiControlCode modifierOff, string? message, params object?[] args)
    {
        if (message is not null)
        {
            _console.Out.Write(modifierOn.ToString(null, _consoleFormatProvider));

            if (args?.Length > 0)
            {
                _console.Out.Write(string.Format(CultureInfo.CurrentCulture, message, args));
            }
            else
            {
                _console.Out.Write(message);
            }

            _console.Out.Write(modifierOff.ToString(null, _consoleFormatProvider));
            _console.Out.Write(Environment.NewLine);
        }
    }

    public void BlankLine()
        => _console.Out.Write(Environment.NewLine);

    public void Line(string? message, params object?[] args)
        => WriteLine(Ansi.Color.Foreground.LightGray, Ansi.Color.Foreground.Default, message, args);

    public void SecondaryLine(string? message, params object?[] args)
        => WriteLine(TextDim, Ansi.Text.BoldOff, message, args);

    public void Error(string? message, params object?[] args)
        => WriteLine(Ansi.Color.Foreground.Red, Ansi.Color.Foreground.Default, message, args);

    public string? ColorText(ConsoleColor color, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        var colorControlCode = color switch
        {
            ConsoleColor.DarkBlue => Ansi.Color.Foreground.Blue,
            ConsoleColor.DarkGreen => Ansi.Color.Foreground.Green,
            ConsoleColor.DarkCyan => Ansi.Color.Foreground.Cyan,
            ConsoleColor.DarkRed => Ansi.Color.Foreground.Red,
            ConsoleColor.DarkMagenta => Ansi.Color.Foreground.Magenta,
            ConsoleColor.DarkYellow => Ansi.Color.Foreground.Yellow,
            ConsoleColor.Gray => Ansi.Color.Foreground.LightGray,
            ConsoleColor.DarkGray => Ansi.Color.Foreground.DarkGray,
            ConsoleColor.Blue => Ansi.Color.Foreground.LightBlue,
            ConsoleColor.Green => Ansi.Color.Foreground.LightGreen,
            ConsoleColor.Cyan => Ansi.Color.Foreground.LightCyan,
            ConsoleColor.Red => Ansi.Color.Foreground.LightRed,
            ConsoleColor.Magenta => Ansi.Color.Foreground.LightMagenta,
            ConsoleColor.Yellow => Ansi.Color.Foreground.LightYellow,
            ConsoleColor.White => Ansi.Color.Foreground.White,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null),
        };

        return string.Concat(
            colorControlCode.ToString(null, _consoleFormatProvider),
            value,
            Ansi.Color.Foreground.Default.ToString(null, _consoleFormatProvider));
    }
}
