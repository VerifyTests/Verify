// ReSharper disable RedundantSuppressNullableWarningExpression

static partial class DirectoryReplacements
{
    public readonly struct KeyValuePair<TKey, TValue>(string key, string value)
    {
        public string Key { get; } = key;
        public string Value { get; } = value;
    }

    static List<KeyValuePair<string, string>> replacements = [];

    public static void Replace(StringBuilder builder) =>
        Replace(builder, replacements);

    public static void UseAssembly(string? solutionDir, string projectDir)
    {
        var values = new List<KeyValuePair<string, string>>();
        var baseDir = CleanPath(AppDomain.CurrentDomain.BaseDirectory!);
        values.Add(new (baseDir, "{CurrentDirectory}"));

        var currentDir = CleanPath(Environment.CurrentDirectory);
        values.Add(new (currentDir, "{CurrentDirectory}"));
#if !NET6_0_OR_GREATER
        if (CodeBaseLocation.CurrentDirectory is not null)
        {
            var codeBaseLocation = CleanPath(CodeBaseLocation.CurrentDirectory);
            values.Add(new (codeBaseLocation, "{CurrentDirectory}"));
        }
#endif

        var tempPath = CleanPath(Path.GetTempPath());
        values.Add(new (tempPath, "{TempPath}"));

        if (VerifierSettings.scrubUserProfile)
        {
            var profileDir = CleanPath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            if (!string.IsNullOrWhiteSpace(profileDir))
            {
                values.Add(new (profileDir, "{UserProfile}"));
            }
        }

        AddProjectAndSolutionReplacements(solutionDir, projectDir, values);
        replacements = values
            .OrderByDescending(_ => _.Key.Length)
            .ToList();
    }

    static void AddProjectAndSolutionReplacements(string? solutionDir, string projectDir, List<KeyValuePair<string, string>> replacements)
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
            replacements.Add(new(projectDir, "{ProjectDirectory}"));

            return;
        }

        replacements.Add(new(projectDir, "{ProjectDirectory}"));
        solutionDir = CleanPath(solutionDir);
        if (solutionDir.Length > 1)
        {
            replacements.Add(new(solutionDir, "{SolutionDirectory}"));
        }
    }

    static string CleanPath(string directory) =>
        directory.TrimEnd('/', '\\');
}