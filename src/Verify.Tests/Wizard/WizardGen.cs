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
        var wizardRealDir = Path.Combine(repoRoot, "docs", "mdsource", "wiz");
        Directory.CreateDirectory(wizardDir);
        PurgeDirectory(wizardDir);
        PurgeDirectory(wizardRealDir);
        var pickOsFile = Path.Combine(wizardDir, "readme.source.md");
        var pickOsBuilder = new StringBuilder("""
            # Getting Started Wizard

            ## Pick OS

            """);
        foreach (var os in Enum.GetValues<Os>())
        {
            await ProcessOs(os, pickOsBuilder);
        }

        await File.WriteAllTextAsync(pickOsFile, pickOsBuilder.ToString());
        try
        {
            Process.Start("mdsnippets", repoRoot);
        }
        catch (Win32Exception)
        {
        }
    }

    async Task ProcessOs(Os os, StringBuilder pickOsBuilder)
    {
        pickOsBuilder.AppendLine($" * [{os}]({os}.md)");
        var pickIdeFile = Path.Combine(wizardDir, $"{os}.source.md");
        var pickIdeBuilder = new StringBuilder($"""
            # Getting Started Wizard

            [Home](/docs/wiz/readme.md) > {os}

            ## Pick IDE

            """);
        foreach (var ide in GetIdesForOs(os))
        {
            await ProcessIde(os, ide, pickIdeBuilder);
        }

        await File.WriteAllTextAsync(pickIdeFile, pickIdeBuilder.ToString());
    }

    async Task ProcessIde(Os os, Ide ide, StringBuilder pickIdeBuilder)
    {
        pickIdeBuilder.AppendLine($" * [{GetName(ide)}]({os}_{ide}.md)");
        var pickTestFile = Path.Combine(wizardDir, $"{os}_{ide}.source.md");
        var pickTestFrameworkBuilder = new StringBuilder($"""
            # Getting Started Wizard

            [Home](/docs/wiz/readme.md) > [{os}]({os}.md) > {GetName(ide)}

            ## Pick Test Framework

            Options:

            """);

        foreach (var testFramework in Enum.GetValues<TestFramework>())
        {
            await ProcessTestFramework(os, ide, testFramework, pickTestFrameworkBuilder);
        }

        await File.WriteAllTextAsync(pickTestFile, pickTestFrameworkBuilder.ToString());
    }

    async Task ProcessTestFramework(Os os, Ide ide, TestFramework testFramework, StringBuilder testFrameworkBuilder)
    {
        testFrameworkBuilder.AppendLine($" * [{testFramework}](result_{os}_{ide}_{testFramework}.md)");
        var file = Path.Combine(wizardDir, $"result_{os}_{ide}_{testFramework}.source.md");
        var builder = new StringBuilder($"""
            # Getting Started Wizard

            [Home](/docs/wiz/readme.md) > [{os}]({os}.md) > [{GetName(ide)}]({os}_{ide}.md) > {testFramework}

            """);

        AppendNugets(builder, testFramework);

        AppendImplicitUsings(builder);

        AppendSourceControlSettings(builder);

        AppendDiffEngineTray(os, builder);

        AppendRider(ide, builder);

        AppendReSharper(ide, builder);

        AppendDiffPlex(builder);

        AppendSample(testFramework, builder);

        AppendDiffTool(os, builder);

        await File.WriteAllTextAsync(file, builder.ToString());
    }

    static void AppendDiffPlex(StringBuilder builder) =>
        builder.AppendLine("""
            ## DiffPlex

            The text comparison behavior of Verify is pluggable. The default behaviour, on failure, is to output both the received
            and the verified contents as part of the exception. This can be noisy when verifying large strings.

            [Verify.DiffPlex](https://github.com/VerifyTests/Verify.DiffPlex) changes the text compare result to highlighting text differences inline.

            This is optional, but recommended.


            ### Add the NuGet

            ```xml
            <PackageReference Include="Verify.DiffPlex" Version="*" />
            ```


            ### Enable

            ```cs
            [ModuleInitializer]
            public static void Initialize() =>
                VerifyDiffPlex.Initialize();
            ```

            """);

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

    static void AppendNugets(StringBuilder builder, TestFramework testFramework) =>
        builder.Append($"""

            ## Add NuGet packages

            Add the following packages to the test project:

            snippet: {testFramework}-nugets

            """);

    static string GetName(Ide ide) =>
        ide switch
        {
            Ide.VisualStudio => "Visual Studio",
            Ide.VisualStudioWithReSharper => "Visual Studio with ReSharper",
            Ide.Rider => "JetBrains Rider",
            Ide.Other => "Other",
            _ => throw new ArgumentOutOfRangeException(nameof(ide), ide, null)
        };

    static void AppendImplicitUsings(StringBuilder builder) =>
        builder.Append("""

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
