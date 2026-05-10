// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static Dictionary<string, List<PatternScrubber>> ExtensionMappedGlobalPatternScrubbers = [];
    internal static Dictionary<string, List<LineScrubber>> ExtensionMappedGlobalLineScrubbers = [];
    internal static Dictionary<string, List<ContentScrubber>> ExtensionMappedGlobalContentScrubbers = [];

    /// <summary>
    /// Register a <see cref="PatternScrubber" /> for files with the given extension.
    /// </summary>
    public static void AddScrubber(string extension, PatternScrubber scrubber)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        if (!ExtensionMappedGlobalPatternScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedGlobalPatternScrubbers[extension] = values = [];
        }

        values.Add(scrubber);
    }

    /// <summary>
    /// Register a <see cref="LineScrubber" /> for files with the given extension.
    /// </summary>
    public static void AddScrubber(string extension, LineScrubber scrubber)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        if (!ExtensionMappedGlobalLineScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedGlobalLineScrubbers[extension] = values = [];
        }

        values.Add(scrubber);
    }

    /// <summary>
    /// Register a <see cref="ContentScrubber" /> for files with the given extension.
    /// </summary>
    public static void AddScrubber(string extension, ContentScrubber scrubber)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        if (!ExtensionMappedGlobalContentScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedGlobalContentScrubbers[extension] = values = [];
        }

        values.Add(scrubber);
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(string extension, StringComparison comparison, params string[] stringToMatch)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Ensure.NotNullOrEmpty(stringToMatch);
        AddScrubber(extension, new RemoveLinesContainingScrubber(comparison, stringToMatch));
    }

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public static void ScrubLines(string extension, LineFilter removeLine)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(extension, new FilterLinesScrubber(removeLine));
    }

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public static void ScrubEmptyLines(string extension)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(extension, RemoveEmptyLinesScrubber.Instance);
    }

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineGuids(string extension)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(extension, GuidPatternScrubber.Instance);
    }

    /// <summary>
    /// Scrub lines with an optional replace.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public static void ScrubLinesWithReplace(string extension, LineReplace replaceLine)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(extension, new ReplaceLinesScrubber(replaceLine));
    }

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public static void ScrubMachineName(string extension)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(extension, UserMachineScrubber.MachinePatternScrubber());
    }

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public static void ScrubUserName(string extension)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(extension, UserMachineScrubber.UserPatternScrubber());
    }
}
