namespace VerifyTests;

/// <summary>
/// Decides whether what follows a verified prefix names a run other than this one.
///
/// A verified name is
/// <c>{Type}.{Method}{_parameters}{.uniqueness}{#target}{#index}.verified.{extension}</c>, so the
/// uniqueness segments sit between the parameters and the target name, the index, or the verified
/// marker. Under the unique directory conventions the same segments are in a directory name, with
/// the snapshot files below it, so the segments also end at a directory separator.
///
/// Segments are matched whole, against the values <see cref="Namer" /> can actually produce. A
/// substring search cannot tell <c>Tests.NetworkClient</c> from a <c>Net</c> uniqueness segment, and
/// cannot tell a <c>DotNet8_0</c> written by another target framework from the <c>DotNet9_0</c> this
/// run writes itself.
/// </summary>
static class UniquenessSegments
{
    static HashSet<string> platforms =
    [
        with(StringComparer.Ordinal),
        "Windows",
        "Linux",
        "OSX",
        "Android",
        "IOS"
    ];

    // Namer.Architecture is RuntimeInformation.ProcessArchitecture lowercased. Listed rather than
    // reflected off the Architecture enum, which grew over time: reflecting it would make a name
    // recognised or not depending on the runtime reading it, so a snapshot an s390x agent produced
    // would go unrecognised by a .NET Framework run, which only knows the first four.
    static HashSet<string> architectures =
    [
        with(StringComparer.Ordinal),
        "x86",
        "x64",
        "arm",
        "arm64",
        "wasm",
        "s390x",
        "loongarch64",
        "armv6",
        "ppc64le",
        "riscv64"
    ];

    // Every value Namer.GetRuntimeAndVersion and Namer.GetSimpleFrameworkName can return, before
    // the major and minor version is appended.
    static HashSet<string> frameworks =
    [
        with(StringComparer.Ordinal),
        "Net",
        "DotNet",
        "Mono"
    ];

    /// <param name="tail">The verified path from the end of the matched prefix.</param>
    /// <param name="frameworksCovered">
    /// Whether a manifest is in hand for every target framework. When one is, the union of them has
    /// already settled the runtime and framework axis and the segments naming it are not consulted.
    /// </param>
    public static bool BelongsToAnotherRun(string tail, bool frameworksCovered)
    {
        var end = EndOfUniqueness(tail);
        var start = 0;
        // The first segment is the parameter text, which is not uniqueness. Under the file
        // convention the tail opens with the separator, making that segment empty.
        var isParameters = true;
        while (start < end)
        {
            var next = tail.IndexOf('.', start, end - start);
            if (next == -1)
            {
                next = end;
            }

            if (!isParameters &&
                IsFromAnotherRun(tail[start..next], frameworksCovered))
            {
                return true;
            }

            isParameters = false;
            start = next + 1;
        }

        return false;
    }

    /// <summary>
    /// Uniqueness runs out at the target name or index, at the verified marker, or, for the unique
    /// directory conventions, where the directory holding the snapshots ends.
    /// </summary>
    static int EndOfUniqueness(string tail)
    {
        var end = tail.Length;
        for (var index = 0; index < tail.Length; index++)
        {
            var character = tail[index];
            if (character == '#' ||
                character == Path.DirectorySeparatorChar ||
                character == Path.AltDirectorySeparatorChar)
            {
                return index;
            }
        }

        var marker = tail.IndexOf(".verified", StringComparison.Ordinal);
        if (marker != -1 &&
            marker < end)
        {
            return marker;
        }

        return end;
    }

    static bool IsFromAnotherRun(string segment, bool frameworksCovered)
    {
        if (segment.Length == 0)
        {
            return false;
        }

        if (platforms.Contains(segment))
        {
            return !MatchesCurrentPlatform(segment);
        }

        if (architectures.Contains(segment))
        {
            return !string.Equals(segment, Namer.Architecture, StringComparison.Ordinal);
        }

        if (frameworksCovered)
        {
            return false;
        }

        return IsFramework(segment) &&
               !MatchesCurrentFramework(segment);
    }

    /// <summary>
    /// Namer.OperatingSystemPlatform throws rather than name a platform it does not know. Nothing
    /// this run produces can be said to match in that case, so the file is left alone.
    /// </summary>
    static bool MatchesCurrentPlatform(string segment)
    {
        try
        {
            return string.Equals(segment, Namer.OperatingSystemPlatform, StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
    }

    static bool MatchesCurrentFramework(string segment)
    {
        if (string.Equals(segment, Namer.Runtime, StringComparison.Ordinal) ||
            string.Equals(segment, Namer.RuntimeAndVersion, StringComparison.Ordinal))
        {
            return true;
        }

        // The target framework names need a TargetFrameworkAttribute, and throw without one.
        try
        {
            return string.Equals(segment, Namer.TargetFrameworkName, StringComparison.Ordinal) ||
                   string.Equals(segment, Namer.TargetFrameworkNameAndVersion, StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
    }

    static bool IsFramework(string segment) =>
        frameworks.Contains(WithoutVersion(segment));

    /// <summary>
    /// Strips the <c>{major}_{minor}</c> a versioned uniqueness segment ends with, so that
    /// <c>DotNet9_0</c> is recognised as the same axis as <c>DotNet</c>. Anything that is not
    /// exactly that shape is returned whole, and fails the name lookup on its own.
    /// </summary>
    static string WithoutVersion(string segment)
    {
        var underscore = segment.LastIndexOf('_');
        if (underscore < 1 ||
            underscore == segment.Length - 1)
        {
            return segment;
        }

        for (var index = underscore + 1; index < segment.Length; index++)
        {
            if (!char.IsDigit(segment[index]))
            {
                return segment;
            }
        }

        var nameEnd = underscore;
        while (nameEnd > 0 &&
               char.IsDigit(segment[nameEnd - 1]))
        {
            nameEnd--;
        }

        if (nameEnd == underscore)
        {
            return segment;
        }

        return segment[..nameEnd];
    }
}
