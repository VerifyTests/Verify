// ReSharper disable RedundantSuppressNullableWarningExpression

static class DirectoryReplacements
{
    public readonly struct Pair
    {
        public Pair(string find, string replace)
        {
#if DEBUG
            if (find.Contains('\\'))
            {
                throw new("Slashes should be sanitized");
            }
#endif
            Find = find;
            Replace = replace;
        }

        public string Find { get; }
        public string Replace { get; }
    }

    static List<Pair> items = [];
    static int minLength;
    static int maxLength;

    internal static List<Pair> Items => items;
    internal static int MinLength => minLength;
    internal static int MaxLength => maxLength;

    static void RecalculateLengths()
    {
        if (items.Count == 0)
        {
            minLength = 0;
            maxLength = 0;
            return;
        }

        var min = int.MaxValue;
        var max = 0;
        foreach (var item in items)
        {
            if (item.Find.Length < min)
            {
                min = item.Find.Length;
            }

            if (item.Find.Length > max)
            {
                max = item.Find.Length;
            }
        }

        minLength = min;
        // +1 for the greedy trailing separator
        maxLength = max + 1;
    }

    public static void UseAssembly(string? solutionDir, string projectDir)
    {
        var values = new List<Pair>();
        var baseDir = CleanPath(AppDomain.CurrentDomain.BaseDirectory!);
        values.Add(new(baseDir, "{CurrentDirectory}"));

        var currentDir = CleanPath(Environment.CurrentDirectory);
        if (currentDir != baseDir)
        {
            values.Add(new(currentDir, "{CurrentDirectory}"));
        }
#if !NET6_0_OR_GREATER
        if (CodeBaseLocation.CurrentDirectory is not null)
        {
            var codeBaseLocation = CleanPath(CodeBaseLocation.CurrentDirectory);
            if (baseDir != codeBaseLocation &&
                currentDir != codeBaseLocation)
            {
                values.Add(new(codeBaseLocation, "{CurrentDirectory}"));
            }
        }
#endif

        var tempPath = CleanPath(Path.GetTempPath());
        values.Add(new(tempPath, "{TempPath}"));

        if (VerifierSettings.scrubUserProfile)
        {
            var profileDir = CleanPath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            if (!string.IsNullOrWhiteSpace(profileDir))
            {
                values.Add(new(profileDir, "{UserProfile}"));
            }
        }

        AddProjectAndSolutionReplacements(solutionDir, projectDir, values);
        items = values
            .OrderByDescending(_ => _.Find.Length)
            .ToList();
        RecalculateLengths();
    }

    static void AddProjectAndSolutionReplacements(string? solutionDir, string projectDir, List<Pair> replacements)
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
        directory.Replace('\\', '/').TrimEnd('/');
}