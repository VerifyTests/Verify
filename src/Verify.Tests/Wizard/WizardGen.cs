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
        Process.Start("mdsnippets", repoRoot);
    }

    async Task ProcessOs(Os os, StringBuilder pickOsBuilder)
    {
        pickOsBuilder.AppendLine($" * [{os}](pickide_{os}.md)");
        var pickIdeFile = Path.Combine(wizardDir, $"pickide_{os}.source.md");
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
        pickIdeBuilder.AppendLine($" * [{GetName(ide)}](picktest_{os}_{ide}.md)");
        var pickTestFile = Path.Combine(wizardDir, $"picktest_{os}_{ide}.source.md");
        var pickTestFrameworkBuilder = new StringBuilder($"""
            # Getting Started Wizard

            [Home](/docs/wiz/readme.md) > [{os}](pickide_{os}.md) > {GetName(ide)}

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

            [Home](/docs/wiz/readme.md) > [{os}](pickide_{os}.md) > [{GetName(ide)}](picktest_{os}_{ide}.md) > {testFramework}

            """);

        AppendNugets(builder, testFramework);

        AppendImplicitUsings(builder);

        AppendDiffEngineTray(os, builder);

        AppendRider(ide, builder);

        AppendReSharper(ide, builder);

        await File.WriteAllTextAsync(file, builder.ToString());
    }

    static void AppendNugets(StringBuilder builder, TestFramework testFramework) =>
        builder.Append($"""

            ### Add NuGet packages

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
            
            ### Implicit Usings
            
            include: implicit-usings

            """);

    static void AppendDiffEngineTray(Os os, StringBuilder builder)
    {
        if (os != Os.Windows)
        {
            return;
        }

        builder.Append("""
                
                ## DiffEngineTray

                Install [DiffEngineTray](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md)

                DiffEngineTray sits in the Windows tray. It monitors pending changes in snapshots, and provides a mechanism for accepting or rejecting those changes.

                ```
                dotnet tool install -g DiffEngineTray
                ```

                This is optional, but recommended.
                """);
    }

    static void AppendReSharper(Ide ide, StringBuilder builder)
    {
        if (ide != Ide.VisualStudioWithReSharper)
        {
            return;
        }

        builder.Append("""
                
                ## ReSharper Plugin

                Install [ReSharper Plugin](https://plugins.jetbrains.com/plugin/17241-verify-support)

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

        builder.Append("""
                
                ## Rider Plugin

                Install [Rider Plugin](https://plugins.jetbrains.com/plugin/17240-verify-support)

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
                Ide.Other,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(os), os, null)
        };
}
