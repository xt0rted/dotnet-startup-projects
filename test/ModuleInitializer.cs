﻿namespace StartupProjects;

using System.Runtime.CompilerServices;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.AddExtraSettings(settings => settings.Converters.Add(new ConsoleConverter()));
    }
}
