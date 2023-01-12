using System.ComponentModel;

public class WizardGen
{
    string wizardDir = null!;
    string repoRoot = null!;

    [Fact]
    public async Task Run()
    {
        var solutionDirectory = AttributeReader.GetSolutionDirectory();
        repoRoot = Directory.GetParent(solutionDirectory)!.Parent!.FullName;
        wizardDir = Path.Combine(repoRoot, "docs", "mdsource", "wiz");
        Directory.CreateDirectory(wizardDir);
        PurgeDirectory(wizardDir);
        var wizardRealDir = Path.Combine(repoRoot, "docs", "wiz");
        PurgeDirectory(wizardRealDir);
        var pickOsFile = Path.Combine(wizardDir, "readme.source.md");
        var builder = new StringBuilder("""
            # Getting Started Wizard

            ## Pick OS

            """);
        foreach (var os in Enum.GetValues<Os>())
        {
            await ProcessOs(os, builder);
        }

        await File.WriteAllTextAsync(pickOsFile, builder.ToString());
        try
        {
            Process.Start("mdsnippets", repoRoot);
        }
        catch (Win32Exception)
        {
        }
    }

    async Task ProcessOs(Os os, StringBuilder parentBuilder)
    {
        var fileName = $"{os}";
        var nav = $"[Home](/docs/wiz/readme.md) > [{os}]({fileName}.md)";
        parentBuilder.AppendLine($" * [{os}]({fileName}.md)");
        var pickIdeFile = Path.Combine(wizardDir, $"{fileName}.source.md");
        var builder = new StringBuilder($"""
            # Getting Started Wizard

            {nav}

            ## Pick IDE

            """);
        foreach (var ide in GetIdesForOs(os))
        {
            await ProcessCli(os, ide, builder, fileName, nav);
        }

        await File.WriteAllTextAsync(pickIdeFile, builder.ToString());
    }

    async Task ProcessCli(Os os, Ide ide, StringBuilder parentBuilder, string parentFileName, string nav)
    {
        var fileName = $"{parentFileName}_{ide}";
        nav += $" > [{GetName(ide)}]({fileName}.md)";
        parentBuilder.AppendLine($" * [{GetName(ide)}]({fileName}.md)");
        var pickTestFile = Path.Combine(wizardDir, $"{fileName}.source.md");
        var builder = new StringBuilder($"""
            # Getting Started Wizard

            {nav}

            ## Pick Cli

            Options:

            """);

        foreach (var cli in Enum.GetValues<CliPreference>())
        {
            await ProcessIde(os, ide, cli, builder, fileName, nav);
        }

        await File.WriteAllTextAsync(pickTestFile, builder.ToString());
    }

    async Task ProcessIde(Os os, Ide ide, CliPreference cli, StringBuilder parentBuilder, string parentFileName, string nav)
    {
        var fileName = $"{parentFileName}_{cli}";
        nav += $" > [{GetName(cli)}]({fileName}.md)";
        parentBuilder.AppendLine($" * [{GetName(cli)}]({fileName}.md)");
        var pickTestFile = Path.Combine(wizardDir, $"{fileName}.source.md");
        var builder = new StringBuilder($"""
            # Getting Started Wizard

            {nav}

            ## Pick Test Framework

            Options:

            """);

        foreach (var testFramework in Enum.GetValues<TestFramework>())
        {
            await ProcessTestFramework(os, ide, cli, testFramework, builder, fileName, nav);
        }

        await File.WriteAllTextAsync(pickTestFile, builder.ToString());
    }

    async Task ProcessTestFramework(Os os, Ide ide, CliPreference cli, TestFramework testFramework, StringBuilder parentBuilder, string parentFileName, string nav)
    {
        var fileName = $"result_{parentFileName}_{testFramework}";
        nav += $" > {testFramework}";
        parentBuilder.AppendLine($" * [{testFramework}]({fileName}.md)");
        var file = Path.Combine(wizardDir, $"{fileName}.source.md");
        var builder = new StringBuilder($"""
            # Getting Started Wizard

            {nav}

            """);

        AppendNugets(builder, testFramework, cli);

        AppendImplicitUsings(builder);

        AppendSourceControlSettings(builder);

        AppendDiffEngineTray(os, builder);

        AppendRider(ide, builder);

        AppendReSharper(ide, builder);

        AppendDiffPlex(builder, cli);

        AppendSample(testFramework, builder);

        AppendDiffTool(os, builder);

        await File.WriteAllTextAsync(file, builder.ToString());
    }

    static void AppendDiffPlex(StringBuilder builder, CliPreference cli)
    {
        string nugetSnippet;
        if (cli == CliPreference.Cli)
        {
            nugetSnippet = """
                ```
                dotnet add package Verify.DiffPlex
                ```
                """;
        }
        else
        {
            nugetSnippet = """
                ```xml
                <PackageReference Include="Verify.DiffPlex" Version="*" />
                ```
                """;
        }

        builder.AppendLine($"""
            ## DiffPlex

            The text comparison behavior of Verify is pluggable. The default behaviour, on failure, is to output both the received
            and the verified contents as part of the exception. This can be noisy when verifying large strings.

            [Verify.DiffPlex](https://github.com/VerifyTests/Verify.DiffPlex) changes the text compare result to highlighting text differences inline.

            This is optional, but recommended.


            ### Add the NuGet

            {nugetSnippet}


            ### Enable

            ```cs
            [ModuleInitializer]
            public static void Initialize() =>
                VerifyDiffPlex.Initialize();
            ```

            """);
    }

    static void AppendSourceControlSettings(StringBuilder builder) =>
        builder.AppendLine("""

            ## Includes/Excludes

            include: include-exclude


            ## Line Endings

            include: line-endings

            """);

    static void AppendSample(TestFramework testFramework, StringBuilder builder) =>
        builder.AppendLine($"""

            ## Sample Test

            snippet: SampleTest{testFramework}

            """);

    static void AppendDiffTool(Os os, StringBuilder builder)
    {
        builder.AppendLine($"""
            ## Diff Tool

            Verify supports many [Diff Tools](https://github.com/VerifyTests/DiffEngine/blob/main/docs/diff-tool.md#supported-tools) for comparing received to verified.
            While IDEs are supported, due to their MDI nature, using a different Diff Tool is recommended.

            Tool supported by {os}:

            """);
        foreach (var tool in ToolsForOs(os))
        {
            if (tool.IsMdi)
            {
                continue;
            }
            builder.AppendLine($" * [{tool.Tool}]({tool.Url})");
        }
    }

    static IEnumerable<Definition> ToolsForOs(Os os) =>
        os switch
        {
            Os.Windows => Definitions.Tools.Where(_ => _.OsSupport.Windows != null),
            Os.MacOS => Definitions.Tools.Where(_ => _.OsSupport.Osx != null),
            Os.Linux => Definitions.Tools.Where(_ => _.OsSupport.Linux != null),
            _ => throw new ArgumentOutOfRangeException(nameof(os), os, null)
        };

    static void AppendNugets(StringBuilder builder, TestFramework testFramework, CliPreference cli)
    {
        builder.AppendLine("""
            
            ## Add NuGet packages
            
            Add the following packages to the test project:

            """);
        switch (cli)
        {
            case CliPreference.Cli:
                switch (testFramework)
                {
                    case TestFramework.xUnit:
                        builder.AppendLine("""
                            ```
                            dotnet add package Microsoft.NET.Test.Sdk
                            dotnet add package Verify.Xunit
                            dotnet add package Xunit
                            dotnet add package xunit.runner.visualstudio
                            ```
                            """);
                        break;
                    case TestFramework.NUnit:
                        builder.AppendLine("""
                            ```
                            dotnet add package Microsoft.NET.Test.Sdk
                            dotnet add package NUnit
                            dotnet add package NUnit3TestAdapter
                            dotnet add package Verify.NUnit
                            ```
                            """);
                        break;
                    case TestFramework.MSTest:
                        builder.AppendLine("""
                            ```
                            dotnet add package Microsoft.NET.Test.Sdk
                            dotnet add package MSTest.TestAdapter
                            dotnet add package MSTest.TestFramework
                            dotnet add package Verify.MSTest
                            ```
                            """);
                        break;
                    case TestFramework.Expecto:
                        builder.AppendLine("""
                            ```
                            dotnet add package Microsoft.NET.Test.Sdk
                            dotnet add package YoloDev.Expecto.TestSdk
                            dotnet add package Expecto
                            dotnet add package Verify.Expecto
                            ```
                            """);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(testFramework), testFramework, null);
                }
                break;
            case CliPreference.Gui:
                builder.AppendLine($"""
                    
                    snippet: {testFramework}-nugets

                    """);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cli), cli, null);
        }
    }

    static string GetName(Ide ide) =>
        ide switch
        {
            Ide.VisualStudio => "Visual Studio",
            Ide.VisualStudioWithReSharper => "Visual Studio with ReSharper",
            Ide.Rider => "JetBrains Rider",
            Ide.Other => "Other",
            _ => throw new ArgumentOutOfRangeException(nameof(ide), ide, null)
        };

    static string GetName(CliPreference preference) =>
        preference switch
        {
            CliPreference.Cli => "Prefer CLI",
            CliPreference.Gui => "Prefer GUI",
            _ => throw new ArgumentOutOfRangeException(nameof(preference), preference, null)
        };

    static void AppendImplicitUsings(StringBuilder builder) =>
        builder.AppendLine("""

            ## Implicit Usings

            include: implicit-usings

            """);

    static void AppendDiffEngineTray(Os os, StringBuilder builder)
    {
        if (os != Os.Windows)
        {
            return;
        }

        builder.AppendLine("""

                ## DiffEngineTray

                Install [DiffEngineTray](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md)

                DiffEngineTray sits in the Windows tray. It monitors pending changes in snapshots, and provides a mechanism for accepting or rejecting those changes.

                ```
                dotnet tool install -g DiffEngineTray
                ```

                This is optional, but recommended. Also consider enabling [Run at startup](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md#run-at-startup).

                """);
    }

    static void AppendReSharper(Ide ide, StringBuilder builder)
    {
        if (ide != Ide.VisualStudioWithReSharper)
        {
            return;
        }

        builder.AppendLine("""

                ## ReSharper Plugin

                Install the [ReSharper Plugin](https://plugins.jetbrains.com/plugin/17241-verify-support)

                Provides a mechanism for contextually accepting or rejecting snapshot changes inside the ReSharper test runner.

                This is optional, but recommended.

                """);
    }

    static void AppendRider(Ide ide, StringBuilder builder)
    {
        if (ide != Ide.Rider)
        {
            return;
        }

        builder.AppendLine("""

                ## Rider Plugin

                Install the [Rider Plugin](https://plugins.jetbrains.com/plugin/17240-verify-support)

                Provides a mechanism for contextually accepting or rejecting snapshot changes inside the Rider test runner.

                This is optional, but recommended.

                """);
    }

    static void PurgeDirectory(string directory)
    {
        foreach (var subDirectory in Directory.EnumerateDirectories(directory))
        {
            Directory.Delete(subDirectory, true);
        }

        foreach (var file in Directory.EnumerateFiles(directory))
        {
            File.Delete(file);
        }
    }

    static Ide[] GetIdesForOs(Os os) =>
        os switch
        {
            Os.Windows => new[]
            {
                Ide.VisualStudio,
                Ide.VisualStudioWithReSharper,
                Ide.Rider,
                Ide.Other,
            },
            Os.MacOS => new[]
            {
                Ide.Rider,
                Ide.Other,
            },
            Os.Linux => new[]
            {
                Ide.Rider,
                Ide.Other,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(os), os, null)
        };
}
