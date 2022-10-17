namespace StartupProjects;

internal static class Constants
{
    public static string ProjectPropertyName => "IsDefaultMultiStartupProject";

    public static string DefaultVersion => "2022";

    public static string[] SupportedVersions => new[]
    {
        "2019",
        DefaultVersion,
    };
}
