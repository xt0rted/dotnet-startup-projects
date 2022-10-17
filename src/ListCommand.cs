namespace StartupProjects;

internal class ListCommand : CommandBase
{
    public ListCommand(IFormatProvider consoleFormatProvider)
        : base(consoleFormatProvider, "list", $"List all projects in the given solution with <{Constants.ProjectPropertyName}> set to true")
    {
    }
}
