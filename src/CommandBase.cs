namespace StartupProjects;

using System.Xml;
using System.Xml.Linq;

using SetStartupProjects;

internal abstract class CommandBase : Command, ICommandHandler
{
    protected readonly IFormatProvider _consoleFormatProvider;

    protected CommandBase(
        IFormatProvider consoleFormatProvider,
        string name,
        string description)
        : base(name, description)
    {
        _consoleFormatProvider = consoleFormatProvider ?? throw new ArgumentNullException(nameof(consoleFormatProvider));

        Handler = this;
    }

    public int Invoke(InvocationContext context)
        => throw new NotImplementedException();

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var writer = BuildConsoleWriter(context.Console);

        var solution = await LoadStartupProjectsAsync(writer, context.ParseResult, context.GetCancellationToken());

        if (solution is null)
        {
            return 1;
        }

        return Implementation(writer, context, solution);
    }

    protected virtual IConsoleWriter BuildConsoleWriter(IConsole console)
        => new ConsoleWriter(console, _consoleFormatProvider);

    protected virtual int Implementation(IConsoleWriter writer, InvocationContext context, SolutionDetails solution) => 0;

    protected static async Task<SolutionDetails?> LoadStartupProjectsAsync(
        IConsoleWriter writer,
        ParseResult parseResult,
        CancellationToken cancellationToken)
    {
        var solutionPath = parseResult.GetValueForArgument(GlobalArguments.Solution);
        var solutionFile = FindSolutionFile(writer, solutionPath);

        if (solutionFile is null)
        {
            return null;
        }

        writer.Line("Checking for projects in {0}", solutionFile);

        if (!File.Exists(solutionFile))
        {
            writer.Error("Solution file doesn't exist");

            return null;
        }

        var result = new List<ProjectDetails>();

        foreach (var project in SolutionProjectExtractor.GetAllProjectFiles(solutionFile))
        {
            var isStartupProject = await IsStartupProjectAsync(project.FullPath, cancellationToken);

            if (isStartupProject)
            {
                result.Add(new ProjectDetails(project.Guid, project.RelativePath));
            }
        }

        if (result.Count == 0)
        {
            writer.Error("No projects contain <{0}> or have it set to true", Constants.ProjectPropertyName);

            return null;
        }

        if (result.Count == 1)
        {
            writer.Line("  Found 1 project");
        }
        else
        {
            writer.SecondaryLine("  Found {0} projects", result.Count);
        }

        foreach (var startupProject in result)
        {
            writer.SecondaryLine("  {0}", startupProject.Path);
        }

        return new SolutionDetails(solutionFile, result);
    }

    private static string? FindSolutionFile(IConsoleWriter writer, string path)
    {
        if (string.Equals(Path.GetExtension(path), ".sln", StringComparison.OrdinalIgnoreCase))
        {
            return Path.IsPathFullyQualified(path)
                ? path
                : Path.GetFullPath(path);
        }

        if (!Directory.Exists(path))
        {
            writer.Error("Solution path '{0}' doesn't exist", path);

            return null;
        }

        var files = Directory.GetFiles(path, "*.sln");

        if (files.Length == 0)
        {
            writer.Error("No solution found in '{0}'", path);

            return null;
        }

        if (files.Length > 1)
        {
            writer.Error("More than 1 .sln found in '{0}'", path);

            return null;
        }

        return Path.GetFullPath(files[0]);
    }

    private static async Task<bool> IsStartupProjectAsync(string path, CancellationToken cancellationToken)
    {
        await using (var file = File.OpenRead(path))
        using (var reader = XmlReader.Create(file, new XmlReaderSettings { Async = true }))
        {
            var fileContents = await XElement.LoadAsync(reader, LoadOptions.None, cancellationToken);

            // To support full framework projects we need to ignore namespaces
            fileContents = RemoveNamespaces(fileContents);

            foreach (var group in fileContents.Elements("PropertyGroup"))
            {
                var isStartup = group.Element(Constants.ProjectPropertyName);

                if (string.Equals(isStartup?.Value, "true", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public record SolutionDetails(string File, List<ProjectDetails> Projects);

    public record ProjectDetails(string Guid, string Path);

    // https://stackoverflow.com/a/7238007/39605
    private static XElement RemoveNamespaces(XElement e)
        => new XElement(
            e.Name.LocalName,
            e.Nodes().Select(n => n is XElement xn ? RemoveNamespaces(xn) : n),
            e.HasAttributes
                ? e.Attributes()
                    .Where(a => !a.IsNamespaceDeclaration)
                    .Select(a => new XAttribute(a.Name.LocalName, a.Value))
                : null
        );
}
