#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace VerifyTests;

static class InnerVerifyChecks
{
    public static async Task Run(Assembly assembly)
    {
        var directory = AttributeReader.GetSolutionDirectory(assembly);
        await CheckEditorConfig(directory);
        await CheckGitIgnore(directory);
        await CheckIncorrectlyImportedSnapshots(directory);
    }

    static async Task CheckIncorrectlyImportedSnapshots(string solutionDirectory)
    {
        var builder = new StringBuilder();
        foreach (var project in Directory.EnumerateFiles(solutionDirectory, "*.csproj", SearchOption.AllDirectories))
        {
            foreach (var line in await ReadLines(project))
            {
                //<None Update="SampleVbTest.RunResults#helloWorld.verified.cs">
                if (!line.Contains("<None Update=\"") ||
                    !line.Contains(".verified."))
                {
                    continue;
                }

                builder.AppendLine(
                    $"""
                     Project: file:///{project.Replace('\\', '/')}
                     Line: {line.TrimStart()}
                     """);
            }
        }

        if (builder.Length == 0)
        {
            return;
        }

        builder.Insert(
            0,
            """
            Found incorrectly imported verified file.
            This occurs when a test file is copied in the IDE and the IDE incorrectly duplicates the dynamically imported verified file nestings.

            """);
        throw new(builder.ToString());
    }

    static async Task CheckEditorConfig(string solutionDirectory)
    {
        var editorConfigPath = Path.Combine(solutionDirectory, ".editorconfig");
        if (File.Exists(editorConfigPath))
        {
            var text = await ReadText(editorConfigPath);
            if (text.Contains("{received,verified}"))
            {
                return;
            }

            throw new(
                $$"""
                  Expected .editorconfig to contain settings for Verify.
                  Path: {{editorConfigPath}}
                  Recommended settings:

                  # Verify settings
                  # Extensions should contain all the text files used by snapshots
                  [*.{received,verified}.{txt,xml,json}]
                  charset = "utf-8-bom"
                  end_of_line = lf
                  indent_size = unset
                  indent_style = unset
                  insert_final_newline = false
                  tab_width = unset
                  trim_trailing_whitespace = false
                  """);
        }
    }

    static async Task CheckGitIgnore(string solutionDirectory)
    {
        var gitIgnorePath = Path.Combine(solutionDirectory, ".gitIgnore");
        if (!File.Exists(gitIgnorePath))
        {
            gitIgnorePath = Path.Combine(solutionDirectory, "../.gitIgnore");
        }

        if (!File.Exists(gitIgnorePath))
        {
            return;
        }

        var text = await ReadText(gitIgnorePath);
        if (text.Contains("*.received.*"))
        {
            return;
        }

        throw new(
            $"""
              Expected .gitIgnore to contain settings for Verify.
              Path: {gitIgnorePath}
              Recommended settings:

              # VerifyTest
              *.received.*
              """);
    }

    static Task<string> ReadText(string path) =>
#if NET6_0_OR_GREATER
        File.ReadAllTextAsync(path);
#else
        Task.FromResult(File.ReadAllText(path));
#endif

    static Task<string[]> ReadLines(string path) =>
#if NET6_0_OR_GREATER
        File.ReadAllLinesAsync(path);
#else
        Task.FromResult(File.ReadAllLines(path));
#endif
}