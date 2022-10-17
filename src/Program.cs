using System.CommandLine.Builder;

using StartupProjects;

var environment = new EnvironmentWrapper();
var consoleFormatProvider = ConsoleHelpers.FormatInfo(environment);

var rootCommand = new RootCommand(
    $"Update a solution's .suo file to use a multiple startup project configuration in Visual Studio using all projects that set <{Constants.ProjectPropertyName}> to true");

// Set the default solution path to CWD
GlobalArguments.Solution.SetDefaultValue(environment.CurrentDirectory);

rootCommand.AddArgument(GlobalArguments.Solution);

rootCommand.AddCommand(new ListCommand(consoleFormatProvider));
rootCommand.AddCommand(new SetCommand(consoleFormatProvider));

var parser = new CommandLineBuilder(rootCommand)
    .UseVersionOption()
    .UseHelp()
    .UseParseDirective()
    .UseSuggestDirective()
    .RegisterWithDotnetSuggest()
    .UseParseErrorReporting()
    .Build();

var parseResult = parser.Parse(args);

return await parseResult.InvokeAsync();
