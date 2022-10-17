namespace StartupProjects;

[UsesVerify]
public abstract class TestBase
{
    protected readonly string _integrationPath;

    protected TestBase()
    {
        // Update the default solution location to the integration folder, which mimics running inside of it from the cli
        var cwd = AttributeReader.GetProjectDirectory(GetType().Assembly);

        _integrationPath = Path.GetFullPath(Path.Join(cwd, "../integration"));

        GlobalArguments.Solution.SetDefaultValue(_integrationPath);
    }

    internal static (IConsole, InvocationContext, TCommand) SetUpTest<TCommand>(
        Func<IFormatProvider, IConsoleWriter, TCommand> commandBuilder, string commandLine) where TCommand : Command, ICommandHandler
    {
        var console = new TestConsole();
        var consoleFormatProvider = new ConsoleFormatInfo
        {
            SupportsAnsiCodes = false,
        };
        var consoleWriter = new ConsoleWriter(console, consoleFormatProvider);

        var command = commandBuilder(consoleFormatProvider, consoleWriter);
        var rootCommand = new MockRootCommand(command);
        var parseResult = rootCommand.Parse(commandLine);
        var ctx = new InvocationContext(parseResult);

        return (console, ctx, command);
    }

    internal class MockRootCommand : RootCommand
    {
        public MockRootCommand(Command command)
        {
            AddCommand(command);

            AddArgument(GlobalArguments.Solution);
        }
    }
}
