namespace StartupProjects;

public static class GlobalArguments
{
    public static readonly Argument<string> Solution = new("solution", "Optional path of the solution")
    {
        Arity = ArgumentArity.ZeroOrOne,
    };
}
