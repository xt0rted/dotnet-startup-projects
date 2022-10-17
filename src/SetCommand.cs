namespace StartupProjects;

using SetStartupProjects;

internal class SetCommand : CommandBase
{
    private static readonly Option<bool> YesToAllOption = new(new[] { "-y", "--yes" }, "Automatically answer \"yes\" to any prompts");

    private static readonly Option<string[]> VersionOption = new(new[] { "-v", "--vs" }, "Visual Studio versions to target")
    {
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true,
    };

    public SetCommand(IFormatProvider consoleFormatProvider)
        : base(consoleFormatProvider, "set", "Updates the given solution's .suo file with the configured startup projects")
    {
        VersionOption.AddCompletions(Constants.SupportedVersions);
        VersionOption.SetDefaultValue(Constants.DefaultVersion);

        AddOption(VersionOption);
        AddOption(YesToAllOption);
    }

    protected override int Implementation(
        IConsoleWriter writer,
        InvocationContext context,
        SolutionDetails solution)
    {
        var vsVersions = context.ParseResult.GetValueForOption(VersionOption) ?? Array.Empty<string>();

        foreach (var version in vsVersions)
        {
            if (!Constants.SupportedVersions.Contains(version))
            {
                writer.Error("Unsupported Visual Studio version: {0}", version);

                return 1;
            }
        }

        writer.BlankLine();
        writer.Line("Setting startup projects for:");

        foreach (var version in vsVersions)
        {
            writer.SecondaryLine("  Visual Studio {0}", version);
        }

        writer.BlankLine();

        if (!context.ParseResult.GetValueForOption(YesToAllOption))
        {
            writer.Line("This will overwrite your .suo file, are you sure you want to continue? (Y/N)");

            var response = Console.ReadLine();

            if (!"yes".StartsWith(response ?? "no", StringComparison.OrdinalIgnoreCase))
            {
                writer.Error("Canceling update");

                return 1;
            }

            writer.BlankLine();
        }

        new StartProjectSuoCreator()
            .CreateForSolutionFile(
                solution.File,
                solution.Projects.ConvertAll(p => p.Guid),
                MapVersions(vsVersions));

        writer.Line(writer.ColorText(ConsoleColor.Green, "Finished setting startup projects"));

        return 0;
    }

    private static VisualStudioVersions MapVersions(string[] versions)
    {
        VisualStudioVersions result = 0;

        foreach (var version in versions)
        {
            result |= version switch
            {
                "2019" => VisualStudioVersions.Vs2019,
                "2022" => VisualStudioVersions.Vs2022,
                _ => throw new ArgumentOutOfRangeException(nameof(versions), version, "Unsupported Visual Studio version"),
            };
        }

        return result;
    }
}
