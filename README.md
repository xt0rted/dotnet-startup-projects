# <img src="assets/icon.svg" align="left" height="45"> dotnet-startup-projects

[![CI build status](https://github.com/xt0rted/dotnet-startup-projects/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/xt0rted/dotnet-startup-projects/actions/workflows/ci.yml)
[![NuGet Package](https://img.shields.io/nuget/v/startup-projects?logo=nuget)](https://www.nuget.org/packages/startup-projects)
[![GitHub Package Registry](https://img.shields.io/badge/github-package_registry-yellow?logo=nuget)](https://nuget.pkg.github.com/xt0rted/index.json)
[![Project license](https://img.shields.io/github/license/xt0rted/dotnet-startup-projects)](LICENSE)

A `dotnet` tool to manage multiple Visual Studio startup projects for a solution.

## Installation

This tool can be installed globally:

```console
dotnet tool install startup-projects --global
```

Or locally:

```console
dotnet new tool-manifest
dotnet tool install startup-projects
```

Only projects with a `<IsDefaultMultiStartupProject>` property set to `true` will be included in the multiple startup configuration.
Since this tool doesn't use MSBuild to load the project files the property can not be set in `Directory.Build.props`, it must be in the project file itself.

Running this will either create a new `.suo` file for the solution, or overwrite the existing one.
This means any settings saved in your existing `.suo` file will be lost.
You will be prompted to confirm this before the file is overwritten.

## Keeping current

Tools like [Dependabot](https://github.com/github/feedback/discussions/13825) and [Renovate](https://github.com/marketplace/renovate) don't currently support updating dotnet local tools.
One way to automate this is to use a [GitHub Actions workflow](https://github.com/xt0rted/dotnet-tool-update-test) to check for updates and create PRs when new versions are available, which is what this repo does.

## Options

Name | Description
-- | --
`--version` | Show version information
`--help` | Show help and usage information

## Arguments

Name | Description
-- | --
`<solution>` | The path to the solution file to use (defaults to the current working directory)

## Commands

### `list`

List the startup projects for the current solution.

#### Options

This command has no options.

### `set`

Set the startup projects for the current solution.

#### Options

Name | Description
-- | --
`-y`, `--yes` | Automatically answer `yes` to any prompts
`-v`, `--vs` | Visual Studio versions to target (defaults to `2022`)

> **Note**: The supported Visual Studio versions are `2019` and `2022`

## Usage

List all projects configured to be startup projects:

```console
dotnet startup-projects list
```

Set the startup projects for the current solution:

```console
dotnet startup-projects set -v 2019 -y
```

It may be helpful to add a script for this to your `global.json` to make it easier to run:

```json
{
  "scripts": {
    "startup": "dotnet startup-projects --vs 2022 --yes"
  }
}
```

> **Note**: This requires [`run-script`](https://github.com/xt0rted/dotnet-run-script) to be used in the project.

### Color output

This tool supports the `DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION` environment variable.
Setting this to `1` or `true` will force color output on all platforms.
Due to a limitation of the `Console` apis this will not work on Windows when output is redirected.

There is also support for the `NO_COLOR` environment variable.
Setting this to any value will disable color output.
