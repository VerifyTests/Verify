// ReSharper disable RedundantSuppressNullableWarningExpression

static partial class DirectoryReplacements
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

    // The anchors and the shortest find are derived from the pairs, so the three
    // travel together rather than as separate statics a scan has to re-read.
    // Assigned once, from AssignTargetAssembly.
    internal sealed class Snapshot(List<Pair> pairs, int shortestFindLength, string anchors)
    {
        public readonly List<Pair> Pairs = pairs;

        public readonly int ShortestFindLength = shortestFindLength;

        // The distinct first chars of the Finds, so scans can skip to candidate
        // positions with a vectorized IndexOfAny instead of probing every position
        public readonly string Anchors = anchors;
    }

    static Snapshot current = new([], int.MaxValue, "");

    internal static Snapshot Current => current;

    // Length of the shortest Find, so callers can cheaply skip values that are
    // too short to contain any replacement. int.MaxValue when there are none.
    public static int ShortestFindLength => current.ShortestFindLength;

    internal static string BuildAnchors(List<Pair> pairs)
    {
        var anchors = new List<char>();
        foreach (var pair in pairs)
        {
            var first = pair.Find[0];
            if (!anchors.Contains(first))
            {
                anchors.Add(first);
            }

            // '/' and '\' are equivalent during matching
            if (first == '/' &&
                !anchors.Contains('\\'))
            {
                anchors.Add('\\');
            }
        }

        return new([.. anchors]);
    }

    public static void Replace(StringBuilder builder)
    {
        var snapshot = current;
        Replace(builder, snapshot.Pairs, snapshot.Anchors);
    }

    public static void Replace(StringBuilder builder, List<Pair> pairs) =>
        Replace(builder, pairs, BuildAnchors(pairs));

    // Legacy pass entry point: materialize once, scan with the span matcher, apply
    // matches position-descending so earlier indexes stay valid.
    static void Replace(StringBuilder builder, List<Pair> pairs, string anchors)
    {
#if DEBUG
        var finds = pairs.Select(_ => _.Find).ToList();
        if (!finds.OrderByDescending(_ => _.Length).SequenceEqual(finds))
        {
            throw new("Pairs should be ordered");
        }

        if (finds.Count != finds.Distinct().Count())
        {
            throw new("Find should be distinct");
        }
#endif
        if (pairs.Count == 0 ||
            builder.Length == 0)
        {
            return;
        }

        // pairs are ordered by length desc, so the last is the shortest. If the
        // builder is shorter than that, no pair can match.
        var shortest = pairs[^1].Find.Length;
        if (builder.Length < shortest)
        {
            return;
        }

        var span = builder.ToString().AsSpan();
        List<(int Index, int Length, string Value)>? matches = null;
        for (var position = 0; position + shortest <= span.Length; position++)
        {
            var skip = span[position..].IndexOfAny(anchors.AsSpan());
            if (skip < 0)
            {
                break;
            }

            position += skip;
            if (position + shortest > span.Length)
            {
                break;
            }

            if (!TryMatchAt(span, position, null, null, pairs, out var matchLength, out var replacement))
            {
                continue;
            }

            matches ??= [];
            matches.Add((position, matchLength, replacement));
            position += matchLength - 1;
        }

        if (matches == null)
        {
            return;
        }

        for (var index = matches.Count - 1; index >= 0; index--)
        {
            var match = matches[index];
            builder.Overwrite(match.Value, match.Index, match.Length);
        }
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
        var ordered = values
            .OrderByDescending(_ => _.Find.Length)
            .ToList();
        current = new(
            ordered,
            ordered.Count == 0 ? int.MaxValue : ordered[^1].Find.Length,
            BuildAnchors(ordered));
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