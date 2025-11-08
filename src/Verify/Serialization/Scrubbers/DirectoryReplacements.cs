// ReSharper disable RedundantSuppressNullableWarningExpression

static class DirectoryReplacements
{
    static List<KeyValuePair<string, string>> replacements = [];

    public static void Replace(StringBuilder builder) =>
        builder.ReplaceDirectoryPaths(replacements);

    public static void UseAssembly(string? solutionDir, string projectDir)
    {
        var values = new Dictionary<string, string>();
        var baseDir = CleanPath(AppDomain.CurrentDomain.BaseDirectory!);
        values[baseDir] = "{CurrentDirectory}";

        var currentDir = CleanPath(Environment.CurrentDirectory);
        values[currentDir] = "{CurrentDirectory}";
#if !NET6_0_OR_GREATER
        if (CodeBaseLocation.CurrentDirectory is not null)
        {
            var codeBaseLocation = CleanPath(CodeBaseLocation.CurrentDirectory);
            values[codeBaseLocation] = "{CurrentDirectory}";
        }
#endif

        var tempPath = CleanPath(Path.GetTempPath());
        values[tempPath] = "{TempPath}";

        if (VerifierSettings.scrubUserProfile)
        {
            var profileDir = CleanPath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            if (!string.IsNullOrWhiteSpace(profileDir))
            {
                values[profileDir] = "{UserProfile}";
            }
        }

        AddProjectAndSolutionReplacements(solutionDir, projectDir, values);
        replacements = values
            .OrderByDescending(_ => _.Key.Length)
            .ToList();
    }

    static void AddProjectAndSolutionReplacements(string? solutionDir, string projectDir, Dictionary<string, string> replacements)
    {
        projectDir = CleanPath(projectDir);
        if (!VerifierSettings.scrubProjectDir &&
            !VerifierSettings.scrubSolutionDir)
        {
            return;
        }

        if (!VerifierSettings.scrubSolutionDir ||
            solutionDir is null)
        {
            replacements[projectDir] = "{ProjectDirectory}";

            return;
        }

        replacements[projectDir] = "{ProjectDirectory}";
        solutionDir = CleanPath(solutionDir);
        if (solutionDir.Length > 1)
        {
            replacements[solutionDir] = "{SolutionDirectory}";
        }
    }

    static string CleanPath(string directory) =>
        directory.TrimEnd('/', '\\');
}