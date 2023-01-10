#if NET7_0

public class WizardGen
{
    string wizardDir;
    public WizardGen()
    {
        var solutionDirectory = AttributeReader.GetSolutionDirectory();
        var repoRoot = Directory.GetParent(solutionDirectory)!.Parent!.FullName;
        wizardDir = Path.Combine(repoRoot, "docs", "mdsource","wiz");
        Directory.CreateDirectory(wizardDir);
        PurgeDirectory(wizardDir);
    }
    [Fact]
    public async Task Run()
    {
        var pickOsFile = Path.Combine(wizardDir, "pickos.source.md");
        var pickOsBuilder = new StringBuilder("# Pick OS");
        foreach (var os in Enum.GetValues<Os>())
        {
            await ProcessOs(os, pickOsBuilder);
        }
        await File.WriteAllTextAsync(pickOsFile, pickOsBuilder.ToString());
    }

    async Task ProcessOs(Os os, StringBuilder pickOsBuilder)
    {
        pickOsBuilder.AppendLine($" * {os}");
        var pickIdeFile = Path.Combine(wizardDir, $"pickide_{os}_.source.md".ToLower());
        var pickIdeBuilder = new StringBuilder("# Pick IDE");
        foreach (var ide in Enum.GetValues<Ide>())
        {
            await ProcessIde(os, ide, pickIdeBuilder);
        }
        await File.WriteAllTextAsync(pickIdeFile, pickIdeBuilder.ToString());
    }

    async Task ProcessIde(Os os, Ide ide, StringBuilder pickIdeBuilder)
    {
        pickIdeBuilder.AppendLine($" * {ide}");
        var pickTestFile = Path.Combine(wizardDir, $"picktest_{os}_{ide}.source.md".ToLower());
        var pickTestFrameworkBuilder = new StringBuilder("# Pick TestFramework");

        foreach (var testFramework in Enum.GetValues<TestFramework>())
        {
            pickTestFrameworkBuilder.AppendLine($" * {ide}");
            await ProcessTestFramework(os, ide, testFramework, pickTestFrameworkBuilder);
        }

        await File.WriteAllTextAsync(pickTestFile, pickTestFrameworkBuilder.ToString());
    }

    async Task ProcessTestFramework(Os os, Ide ide, TestFramework testFramework, StringBuilder testFrameworkBuilder)
    {
        testFrameworkBuilder.AppendLine($" * {testFramework}");
        var file = Path.Combine(wizardDir, $"result_{os}_{ide}_{testFramework}.source.md".ToLower());
        var builder = new StringBuilder("");

        await File.WriteAllTextAsync(file, builder.ToString());
    }

    public static void PurgeDirectory(string directory)
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

}
#endif