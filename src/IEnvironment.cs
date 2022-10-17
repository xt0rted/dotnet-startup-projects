namespace StartupProjects;

internal interface IEnvironment
{
    string CurrentDirectory { get; }

    string? GetEnvironmentVariable(string variable);
}
