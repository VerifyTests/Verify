// ReSharper disable RedundantSuppressNullableWarningExpression

static class DirectoryReplacements
{
    static List<KeyValuePair<string, string>> replacements = [];

    static string ReplaceAltDirChar(this string directory) =>
        directory.Replace(IoHelpers.DirectorySeparator, IoHelpers.AltDirectorySeparator);

    public static void Replace(StringBuilder builder) =>
        builder.ReplaceDirectoryPaths(replacements);

    public static void UseAssembly(string? solutionDir, string projectDir)
    {
        var values = new Dictionary<string, string>();
        var baseDir = CleanPath(AppDomain.CurrentDomain.BaseDirectory!);
        var altBaseDir = baseDir.ReplaceAltDirChar();
        values[baseDir + IoHelpers.DirectorySeparator] = "{CurrentDirectory}";
        values[baseDir] = "{CurrentDirectory}";
        values[altBaseDir + IoHelpers.AltDirectorySeparator] = "{CurrentDirectory}";
        values[altBaseDir] = "{CurrentDirectory}";

        var currentDir = CleanPath(Environment.CurrentDirectory);
        var altCurrentDir = currentDir.ReplaceAltDirChar();
        values[currentDir + IoHelpers.DirectorySeparator] = "{CurrentDirectory}";
        values[currentDir] = "{CurrentDirectory}";
        values[altCurrentDir + IoHelpers.AltDirectorySeparator] = "{CurrentDirectory}";
        values[altCurrentDir] = "{CurrentDirectory}";
#if !NET6_0_OR_GREATER
        if (CodeBaseLocation.CurrentDirectory is not null)
        {
            var codeBaseLocation = CleanPath(CodeBaseLocation.CurrentDirectory);
            var altCodeBaseLocation = codeBaseLocation.ReplaceAltDirChar();
            values[codeBaseLocation + IoHelpers.DirectorySeparator] = "{CurrentDirectory}";
            values[codeBaseLocation] = "{CurrentDirectory}";
            values[altCodeBaseLocation + IoHelpers.AltDirectorySeparator] = "{CurrentDirectory}";
            values[altCodeBaseLocation] = "{CurrentDirectory}";
        }
#endif

        var tempPath = CleanPath(Path.GetTempPath());
        var altTempPath = tempPath.ReplaceAltDirChar();
        values[tempPath + IoHelpers.DirectorySeparator] = "{TempPath}";
        values[tempPath] = "{TempPath}";
        values[altTempPath + IoHelpers.AltDirectorySeparator] = "{TempPath}";
        values[altTempPath] = "{TempPath}";

        if (VerifierSettings.scrubUserProfile)
        {
            var profileDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!string.IsNullOrWhiteSpace(profileDir))
            {
                var altProfileDir = profileDir.ReplaceAltDirChar();
                values[profileDir] = "{UserProfile}";
                values[altProfileDir] = "{UserProfile}";
            }
        }

        AddProjectAndSolutionReplacements(solutionDir, projectDir, values);
        replacements = values
            .OrderByDescending(_ => _.Key.Length)
            .ToList();
    }

    static void AddProjectAndSolutionReplacements(string? solutionDir, string projectDir, Dictionary<string, string> replacements)
    {
        if (!VerifierSettings.scrubProjectDir &&
            !VerifierSettings.scrubSolutionDir)
        {
            return;
        }

        var altProjectDir = projectDir.ReplaceAltDirChar();
        var altProjectDirTrimmed = altProjectDir.TrimEnd('/', '\\');
        var projectDirTrimmed = projectDir.TrimEnd('/', '\\');

        if (!VerifierSettings.scrubSolutionDir ||
            solutionDir is null)
        {
            replacements[projectDir] = "{ProjectDirectory}";
            replacements[projectDirTrimmed] = "{ProjectDirectory}";
            replacements[altProjectDir] = "{ProjectDirectory}";
            replacements[altProjectDirTrimmed] = "{ProjectDirectory}";

            return;
        }

        var altSolutionDir = solutionDir.ReplaceAltDirChar();

        replacements[projectDir] = "{ProjectDirectory}";
        replacements[projectDirTrimmed] = "{ProjectDirectory}";
        replacements[altProjectDir] = "{ProjectDirectory}";
        replacements[altProjectDirTrimmed] = "{ProjectDirectory}";

        replacements[solutionDir] = "{SolutionDirectory}";
        if (solutionDir.Length > 1)
        {
            var solutionDirectoryTrimmed = solutionDir.TrimEnd('/', '\\');
            replacements[solutionDirectoryTrimmed] = "{SolutionDirectory}";
        }

        replacements[altSolutionDir] = "{SolutionDirectory}";
        if (solutionDir.Length > 1)
        {
            var altSolutionDirTrimmed = altSolutionDir.TrimEnd('/', '\\');
            replacements[altSolutionDirTrimmed] = "{SolutionDirectory}";
        }
    }

    static string CleanPath(string directory) =>
        directory.TrimEnd('/', '\\');
}