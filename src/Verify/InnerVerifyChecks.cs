#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace VerifyTests;

static class InnerVerifyChecks
{
    public static async Task Run(Assembly assembly)
    {
        var solutionDirectory = AttributeReader.GetSolutionDirectory(assembly);
        await CheckEditorConfig(solutionDirectory);
        foreach (var file in Directory.EnumerateFiles(solutionDirectory, "*.verified.*"))
        {

        }
    }

    static async Task CheckEditorConfig(string solutionDirectory)
    {
        var editorConfigPath = Path.Combine(solutionDirectory, ".editorconfig");
        if (File.Exists(editorConfigPath))
        {
#if NET6_0_OR_GREATER
            var text = await File.ReadAllTextAsync(editorConfigPath);
#else
            var text = File.ReadAllText(editorConfigPath);
#endif
            if (text.Contains("{received,verified}"))
            {
                return;
            }

            throw new VerifyException(
                $$"""
                  Expected .editorconfig to contain settings for Verify.
                  Path: {{editorConfigPath}}
                  Recommended settings:

                  # Verify settings
                  # Extensions should be all the text files used by snapshots
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
}