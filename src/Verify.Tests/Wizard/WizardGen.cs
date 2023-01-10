public class WizardGen
{
    string wizardDir;

    public WizardGen()
    {
        var solutionDirectory = AttributeReader.GetSolutionDirectory();
        var repoRoot = Directory.GetParent(solutionDirectory)!.Parent!.FullName;
        wizardDir = Path.Combine(repoRoot, "docs", "mdsource", "wiz");
        var wizardRealDir = Path.Combine(repoRoot, "docs", "mdsource", "wiz");
        Directory.CreateDirectory(wizardDir);
        PurgeDirectory(wizardDir);
        PurgeDirectory(wizardRealDir);
    }

    [Fact]
    public async Task Run()
    {
        var pickOsFile = Path.Combine(wizardDir, "pickos.source.md");
        var pickOsBuilder = new StringBuilder("# Pick OS\n\n");
        foreach (var os in Enum.GetValues<Os>())
        {
            await ProcessOs(os, pickOsBuilder);
        }

        await File.WriteAllTextAsync(pickOsFile, pickOsBuilder.ToString());
    }

    async Task ProcessOs(Os os, StringBuilder pickOsBuilder)
    {
        pickOsBuilder.AppendLine($" * [{os}](pickide_{os}.md)");
        var pickIdeFile = Path.Combine(wizardDir, $"pickide_{os}.source.md");
        var pickIdeBuilder = new StringBuilder($"""
            # Pick IDE
            
            Selected OS: {os}

            Options:

            """);
        foreach (var ide in GetIdesForOs(os))
        {
            await ProcessIde(os, ide, pickIdeBuilder);
        }

        await File.WriteAllTextAsync(pickIdeFile, pickIdeBuilder.ToString());
    }

    async Task ProcessIde(Os os, Ide ide, StringBuilder pickIdeBuilder)
    {
        pickIdeBuilder.AppendLine($" * [{ide}](picktest_{os}_{ide}.md)");
        var pickTestFile = Path.Combine(wizardDir, $"picktest_{os}_{ide}.source.md");
        var pickTestFrameworkBuilder = new StringBuilder($"""
            # Pick Test Framework

            Selected OS: {os}

            Selected IDE: {ide}

            Options:

            """);

        foreach (var testFramework in Enum.GetValues<TestFramework>())
        {
            pickTestFrameworkBuilder.AppendLine($" * {ide}");
            await ProcessTestFramework(os, ide, testFramework, pickTestFrameworkBuilder);
        }

        await File.WriteAllTextAsync(pickTestFile, pickTestFrameworkBuilder.ToString());
    }

    async Task ProcessTestFramework(Os os, Ide ide, TestFramework testFramework, StringBuilder testFrameworkBuilder)
    {
        testFrameworkBuilder.AppendLine($" * [{testFramework}](result_{os}_{ide}_{testFramework}.md)");
        var file = Path.Combine(wizardDir, $"result_{os}_{ide}_{testFramework}.source.md");
        var builder = new StringBuilder("");

        await File.WriteAllTextAsync(file, builder.ToString());
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
            Os.Win => new[]
            {
                Ide.VisualStudio,
                Ide.VisualStudioWithResharper,
                Ide.Rider,
                Ide.Other,
            },
            Os.Mac => new[]
            {
                Ide.Rider,
                Ide.VisualStudioMac,
                Ide.Other,
            },
            Os.Linux => new[]
            {
                Ide.Other,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(os), os, null)
        };
}
