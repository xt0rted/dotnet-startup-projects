namespace StartupProjects;

internal class TestEnvironment : IEnvironment
{
    private readonly Dictionary<string, string?> _variables = new(StringComparer.OrdinalIgnoreCase);

    public TestEnvironment(string? currentDirectory = null)
    {
        CurrentDirectory = currentDirectory ?? AttributeReader.GetProjectDirectory(GetType().Assembly);
    }

    public string CurrentDirectory { get; }

    public string? GetEnvironmentVariable(string variable)
        => _variables.TryGetValue(variable, out var value) ? value : null;

    public void SetEnvironmentVariable(string variable, string? value)
        => _variables[variable] = value;
}
