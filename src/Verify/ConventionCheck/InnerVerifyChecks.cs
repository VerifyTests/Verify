// Async method lacks 'await' operators and will run synchronously
#pragma warning disable CS1998

namespace VerifyTests;

[Experimental("InnerVerifyChecks")]
public static class InnerVerifyChecks
{
    public static Task Run(Assembly assembly)
    {
        if (!AttributeReader.TryGetSolutionDirectory(assembly, out var directory))
        {
            var projectDirectory = AttributeReader.GetProjectDirectory(assembly);
            directory = Directory.GetParent(projectDirectory)!.FullName;
        }

        return Run(directory);
    }

    internal static async Task Run(string directory)
    {
        var extensions = GetExtensions(directory);
        await CheckGitIgnore(directory);
        await CheckIncorrectlyImportedSnapshots(directory);
        if (extensions.Count == 0)
        {
            return;
        }

        await CheckEditorConfig(directory, extensions);
        await CheckGitAttributes(directory, extensions);
    }

    internal static List<string> GetExtensions(string directory) =>
        // ReSharper disable once RedundantSuppressNullableWarningExpression
        Directory.EnumerateFiles(directory, "*.verified.*", SearchOption.AllDirectories)
            .Select(_ => Path.GetExtension(_)![1..])
            .Distinct()
            .Where(FileExtensions.IsTextExtension)
            .OrderBy(_ => _)
            .ToList();

    internal static async Task CheckIncorrectlyImportedSnapshots(string solutionDirectory)
    {
        var builder = new StringBuilder();
        foreach (var project in Directory.EnumerateFiles(solutionDirectory, "*.csproj", SearchOption.AllDirectories))
        {
            foreach (var line in await File.ReadAllLinesAsync(project))
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

    internal static async Task CheckEditorConfig(string solutionDirectory, List<string> extensions)
    {
        var path = Path.Combine(solutionDirectory, ".editorconfig");
        if (!File.Exists(path))
        {
            return;
        }

        var lines = await File.ReadAllLinesAsync(path);

        if (HasAllExtensions(extensions, lines))
        {
            CheckEditorConfigIndent(path, lines, extensions);
            return;
        }

        throw new VerifyCheckException(
            $"""
             Expected .editorconfig to contain settings for Verify for all text files
             Path: {GetPath(path)}
             Recommended settings:

             # Verify
             {$"[*.{{received,verified}}.{{{string.Join(',', extensions)}}}]"}
             charset = utf-8-bom
             end_of_line = lf
             indent_size = unset
             indent_style = unset
             insert_final_newline = false
             tab_width = unset
             trim_trailing_whitespace = false
             """);
    }

    static string[] indentExtensionsList = ["json", "xml", "html", "htm", "yaml", "cs", "svg"];
    const string indentSectionHeader = "[*.{received,verified}.{json,xml,html,htm,yaml,cs,svg}]";

    static void CheckEditorConfigIndent(string path, string[] lines, List<string> extensions)
    {
        if (!HasIndentExtension(extensions))
        {
            return;
        }

        var sectionIndex = -1;
        var skippedFirst = false;
        for (var i = 0; i < lines.Length; i++)
        {
            var span = lines[i].AsSpan().Trim();
            if (!span.StartsWith("[*.{received,verified}."))
            {
                continue;
            }

            if (!skippedFirst)
            {
                skippedFirst = true;
                continue;
            }

            sectionIndex = i;
            break;
        }

        if (sectionIndex < 0)
        {
            throw new VerifyCheckException(
                $"""
                 Expected .editorconfig to contain indent settings for Verify indented files.
                 Path: {GetPath(path)}
                 Recommended settings:

                 {indentSectionHeader}
                 indent_size = 2
                 indent_style = space
                 """);
        }

        var hasIndentSize = false;
        var hasIndentStyle = false;
        for (var i = sectionIndex + 1; i < lines.Length; i++)
        {
            var span = lines[i].AsSpan().Trim();
            if (span.StartsWith('['))
            {
                break;
            }

            if (span.StartsWith("indent_size"))
            {
                hasIndentSize = true;
                if (span is not "indent_size = 2")
                {
                    throw new VerifyCheckException(
                        $"""
                         .editorconfig has incorrect indent_size for Verify indented section. Expected `indent_size = 2`.
                         Path: {GetPath(path)}
                         """);
                }
            }

            if (span.StartsWith("indent_style"))
            {
                hasIndentStyle = true;
                if (span is not "indent_style = space")
                {
                    throw new VerifyCheckException(
                        $"""
                         .editorconfig has incorrect indent_style for Verify indented section. Expected `indent_style = space`.
                         Path: {GetPath(path)}
                         """);
                }
            }
        }

        if (!hasIndentSize)
        {
            throw new VerifyCheckException(
                $"""
                 .editorconfig is missing indent_size for Verify indented section. Expected `indent_size = 2`.
                 Path: {GetPath(path)}
                 """);
        }

        if (!hasIndentStyle)
        {
            throw new VerifyCheckException(
                $"""
                 .editorconfig is missing indent_style for Verify indented section. Expected `indent_style = space`.
                 Path: {GetPath(path)}
                 """);
        }
    }

    static bool HasIndentExtension(List<string> extensions) =>
        extensions.Any(_ => indentExtensionsList.Contains(_));

    static bool HasAllExtensions(List<string> extensions, string[] lines)
    {
        var line = lines.FirstOrDefault(_ => _.StartsWith("[*.{received,verified}."));
        if (line == null)
        {
            return false;
        }

        var suffix = line[24..^2].Split(',');
        return extensions.All(_ => suffix.Contains(_));
    }

    internal static async Task CheckGitAttributes(string solutionDirectory, List<string> extensions)
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
        var text = await File.ReadAllLinesAsync(path);

        List<string> missing = [];
        List<string> expected = [];
        foreach (var extension in extensions)
        {
            string line;
            if (extension == "bin")
            {
                line = "*.verified.bin binary";
            }
            else
            {
                line = $"*.verified.{extension} text eol=lf working-tree-encoding=UTF-8";
            }

            expected.Add(line);
            if (text.Contains(line))
            {
                continue;
            }

            missing.Add(line);
        }

        if (missing.Count == 0)
        {
            CheckAutoCrlf();
            return;
        }

        var builder = new StringBuilder(
            $"""
             Expected .gitattributes to contain settings for Verify.
             Path: {GetPath(path)}
             Recommended settings:

             # Verify

             """);
        foreach (var line in expected)
        {
            builder.AppendLine(line);
        }

        throw new VerifyCheckException(builder.ToString());
    }

    static void CheckAutoCrlf()
    {
        string? value;
        var startInfo = new ProcessStartInfo("git", "config core.autocrlf")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        try
        {
            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return;
            }

            value = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
        }
        catch
        {
            // git not available
            return;
        }

        if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
        {
            throw new VerifyCheckException(
                """
                git `core.autocrlf` is set to `true`. This causes files to show as modified with no actual content changes when `eol=lf` is configured in .gitattributes.
                To fix, run:

                git config --global core.autocrlf input
                """);
        }
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

        var text = await File.ReadAllTextAsync(path);
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
}
