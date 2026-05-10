// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyTests;

public static partial class VerifierSettings
{
    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    [Obsolete("Subclass ContentScrubber and call AddScrubber(string, ContentScrubber). See the scrubber migration guide.")]
    public static void AddScrubber(string extension, Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        AddScrubber(extension, (builder, _, _) => scrubber(builder), location);
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    [Obsolete("Subclass ContentScrubber and call AddScrubber(string, ContentScrubber). See the scrubber migration guide.")]
    public static void AddScrubber(string extension, Action<StringBuilder, Counter> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, (builder, counter, _) => scrubber(builder, counter), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    [Obsolete("Subclass ContentScrubber and call AddScrubber(string, ContentScrubber). See the scrubber migration guide.")]
    public static void AddScrubber(string extension, Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        if (!ExtensionMappedGlobalContentScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedGlobalContentScrubbers[extension] = values = [];
        }

        var adapter = new LambdaContentScrubber(scrubber);
        switch (location)
        {
            case ScrubberLocation.First:
                values.Insert(0, adapter);
                break;
            case ScrubberLocation.Last:
                values.Add(adapter);
                break;
        }
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesContaining(string, StringComparison, params string[]).")]
    public static void ScrubLinesContaining(string extension, StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(extension, comparison, stringToMatch);

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLines(string, Func<string, bool>).")]
    public static void ScrubLines(string extension, Func<string, bool> removeLine, ScrubberLocation location) =>
        ScrubLines(extension, removeLine);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubEmptyLines(string).")]
    public static void ScrubEmptyLines(string extension, ScrubberLocation location) =>
        ScrubEmptyLines(extension);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineGuids(string).")]
    public static void ScrubInlineGuids(string extension, ScrubberLocation location) =>
        ScrubInlineGuids(extension);

    /// <summary>
    /// Scrub lines with an optional replace.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesWithReplace(string, Func<string, string?>).")]
    public static void ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine, ScrubberLocation location) =>
        ScrubLinesWithReplace(extension, replaceLine);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesContaining(string, StringComparison, params string[]).")]
    public static void ScrubLinesContaining(string extension, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(extension, StringComparison.OrdinalIgnoreCase, stringToMatch);

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubMachineName(string).")]
    public static void ScrubMachineName(string extension, ScrubberLocation location) =>
        ScrubMachineName(extension);

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubUserName(string).")]
    public static void ScrubUserName(string extension, ScrubberLocation location) =>
        ScrubUserName(extension);
}
