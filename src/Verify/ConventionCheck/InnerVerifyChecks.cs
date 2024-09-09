#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace VerifyTests;

[Experimental("InnerVerifyChecks")]
public static class InnerVerifyChecks
{
    public static Task Run(Assembly assembly)
    {
        var directory = AttributeReader.GetSolutionDirectory(assembly);
        return Run(directory);
    }

    internal static async Task Run(string directory)
    {
        await CheckEditorConfig(directory);
        await CheckGitIgnore(directory);
        await CheckIncorrectlyImportedSnapshots(directory);
        await CheckGitAttributes(directory);
    }

    internal static async Task CheckIncorrectlyImportedSnapshots(string solutionDirectory)
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
                     Project: {GetPath(project)}
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
        throw new VerifyCheckException(builder.ToString());
    }

    internal static async Task CheckEditorConfig(string solutionDirectory)
    {
        var path = Path.Combine(solutionDirectory, ".editorconfig");
        if (!File.Exists(path))
        {
            return;
        }

        path = Path.GetFullPath(path);
        var text = await ReadText(path);
        if (text.Contains("{received,verified}") ||
            text.Contains("# Verify"))
        {
            return;
        }

        throw new VerifyCheckException(
            $$"""
              Expected .editorconfig to contain settings for Verify.
              Path: {{GetPath(path)}}
              Recommended settings:

              # Verify
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

    internal static async Task CheckGitAttributes(string solutionDirectory)
    {
        var path = Path.Combine(solutionDirectory, ".gitattributes");
        if (!File.Exists(path))
        {
            path = Path.Combine(solutionDirectory, "../.gitattributes");
        }

        if (!File.Exists(path))
        {
            return;
        }

        path = Path.GetFullPath(path);
        var text = await ReadText(path);
        if (text.Contains("*.verified.") ||
            text.Contains("# Verify"))
        {
            return;
        }

        throw new VerifyCheckException(
            $"""
             Expected .gitattributes to contain settings for Verify.
             Path: {GetPath(path)}
             Recommended settings:

             # Verify
             # Extensions should contain all the text files used by snapshots
             *.verified.txt text eol=lf working-tree-encoding=UTF-8
             *.verified.xml text eol=lf working-tree-encoding=UTF-8
             *.verified.json text eol=lf working-tree-encoding=UTF-8
             """);
    }

    static string GetPath(string path) =>
        $"file:///{path.Replace('\\', '/')}";

    internal static async Task CheckGitIgnore(string solutionDirectory)
    {
        var path = Path.Combine(solutionDirectory, ".gitIgnore");
        if (!File.Exists(path))
        {
            path = Path.Combine(solutionDirectory, "../.gitIgnore");
        }

        if (!File.Exists(path))
        {
            return;
        }

        path = Path.GetFullPath(path);
        var text = await ReadText(path);
        if (text.Contains("*.received.*") ||
            text.Contains("*.received/") ||
            text.Contains("# Verify"))
        {
            return;
        }

        throw new VerifyCheckException(
            $"""
             Expected .gitIgnore to contain settings for Verify.
             Path: {GetPath(path)}
             Recommended settings:

             # Verify
             *.received.*
             *.received/
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