namespace StartupProjects;

[UsesVerify]
public sealed class SetCommandTests : TestBase, IDisposable
{
    private readonly string _vsFolderPath;

    public SetCommandTests()
    {
        _vsFolderPath = Path.Join(_integrationPath, ".vs");

        if (Directory.Exists(_vsFolderPath))
        {
            Directory.Delete(_vsFolderPath, recursive: true);
        }
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("foo/bar.sln")]
    [InlineData("src")]
    [InlineData("src/slns")]
    public async Task Should_error_with_invalid_solution_path(string slnPath)
    {
        // Given
        var (console, ctx, command) = SetUpTest(
            (consoleFormatProvider, consoleWriter) => new MockSetCommand(consoleFormatProvider, consoleWriter),
            $"\"{Path.Join(_integrationPath, slnPath)}\" set");

        // When
        var result = await command.InvokeAsync(ctx);

        // Then
        result.ShouldBe(1);

        await Verify(console).UseParameters(slnPath);
    }

    [Fact]
    public async Task Should_error_with_unsupported_vs_Version()
    {
        // Given
        var (console, ctx, command) = SetUpTest(
            (consoleFormatProvider, consoleWriter) => new MockSetCommand(consoleFormatProvider, consoleWriter),
            "set --vs 1984 --yes");

        // When
        var result = await command.InvokeAsync(ctx);

        // Then
        result.ShouldBe(1);

        await Verify(console);
    }

    [Fact]
    public async Task Should_bypass_overwrite_confirmation()
    {
        // Given
        var (console, ctx, command) = SetUpTest(
            (consoleFormatProvider, consoleWriter) => new MockSetCommand(consoleFormatProvider, consoleWriter),
            "set --vs 2022 --yes");

        // When
        var result = await command.InvokeAsync(ctx);

        // Then
        result.ShouldBe(0);

        await Verify(console);
    }

    [Theory]
    [InlineData("2019", "v16")]
    [InlineData("2022", "v17")]
    public async Task Should_update_solution_suo(string version, string folder)
    {
        // Given
        var suoPath = Path.Join(_vsFolderPath, "integration", folder, ".suo");
        var (console, ctx, command) = SetUpTest(
            (consoleFormatProvider, consoleWriter) => new MockSetCommand(consoleFormatProvider, consoleWriter),
            $"set --vs {version} --yes");

        // When
        var result = await command.InvokeAsync(ctx);

        // Then
        result.ShouldBe(0);

        await Verify(File.ReadAllBytesAsync(suoPath)).UseParameters(version, folder);
    }

    public void Dispose()
    {
        if (Directory.Exists(_vsFolderPath))
        {
            Directory.Delete(_vsFolderPath, recursive: true);
        }
    }

    private class MockSetCommand : SetCommand
    {
        private readonly IConsoleWriter _consoleWriter;

        public MockSetCommand(
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
