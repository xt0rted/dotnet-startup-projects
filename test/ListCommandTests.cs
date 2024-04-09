namespace StartupProjects;

public class ListCommandTests : TestBase
{
    [Fact]
    public async Task Should_list_all_configured_projects()
    {
        // Given
        var (console, ctx, command) = SetUpTest(
            (consoleFormatProvider, consoleWriter) => new MockListCommand(consoleFormatProvider, consoleWriter),
            "list");

        // When
        var result = await command.InvokeAsync(ctx);

        // Then
        result.ShouldBe(0);

        await Verify(console);
    }

    internal class MockListCommand : ListCommand
    {
        private readonly IConsoleWriter _consoleWriter;

        public MockListCommand(
            IFormatProvider consoleFormatProvider,
            IConsoleWriter consoleWriter)
            : base(consoleFormatProvider)
        {
            _consoleWriter = consoleWriter;
        }

        protected override IConsoleWriter BuildConsoleWriter(IConsole console)
            => _consoleWriter;
    }
}
