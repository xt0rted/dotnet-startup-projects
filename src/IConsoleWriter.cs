namespace StartupProjects;

internal interface IConsoleWriter
{
    void BlankLine();

    void Line(string? message, params object?[] args);

    void SecondaryLine(string? message, params object?[] args);

    void Error(string? message, params object?[] args);

    string? ColorText(ConsoleColor color, string? value);
}
