namespace StartupProjects;

internal class EnvironmentWrapper : IEnvironment
{
    public string CurrentDirectory => Environment.CurrentDirectory;

    public string? GetEnvironmentVariable(string variable)
        => Environment.GetEnvironmentVariable(variable);
}
