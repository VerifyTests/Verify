namespace VerifyTests;

public partial class VerifySettings
{
    internal Dictionary<string, List<PatternScrubber>>? ExtensionMappedInstancePatternScrubbers;
    internal Dictionary<string, List<LineScrubber>>? ExtensionMappedInstanceLineScrubbers;
    internal Dictionary<string, List<ContentScrubber>>? ExtensionMappedInstanceContentScrubbers;

    /// <summary>
    /// Register a <see cref="PatternScrubber" /> for files with the given extension.
    /// </summary>
    public void AddScrubber(string extension, PatternScrubber scrubber)
    {
        ExtensionMappedInstancePatternScrubbers ??= [];
        if (!ExtensionMappedInstancePatternScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedInstancePatternScrubbers[extension] = values = [];
        }

        values.Add(scrubber);
    }

    /// <summary>
    /// Register a <see cref="LineScrubber" /> for files with the given extension.
    /// </summary>
    public void AddScrubber(string extension, LineScrubber scrubber)
    {
        ExtensionMappedInstanceLineScrubbers ??= [];
        if (!ExtensionMappedInstanceLineScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedInstanceLineScrubbers[extension] = values = [];
        }

        values.Add(scrubber);
    }

    /// <summary>
    /// Register a <see cref="ContentScrubber" /> for files with the given extension.
    /// </summary>
    public void AddScrubber(string extension, ContentScrubber scrubber)
    {
        ExtensionMappedInstanceContentScrubbers ??= [];
        if (!ExtensionMappedInstanceContentScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedInstanceContentScrubbers[extension] = values = [];
        }

        values.Add(scrubber);
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    [Obsolete("Subclass ContentScrubber and call AddScrubber(string, ContentScrubber). See the scrubber migration guide.")]
    public void AddScrubber(string extension, Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, (builder, _, _) => scrubber(builder), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    [Obsolete("Subclass ContentScrubber and call AddScrubber(string, ContentScrubber). See the scrubber migration guide.")]
    public void AddScrubber(string extension, Action<StringBuilder, Counter> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, (builder, counter, _) => scrubber(builder, counter), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    [Obsolete("Subclass ContentScrubber and call AddScrubber(string, ContentScrubber). See the scrubber migration guide.")]
    public void AddScrubber(string extension, Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        ExtensionMappedInstanceContentScrubbers ??= [];
        if (!ExtensionMappedInstanceContentScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedInstanceContentScrubbers[extension] = values = [];
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
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public void ScrubMachineName(string extension) =>
        AddScrubber(extension, UserMachineScrubber.MachinePatternScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubMachineName(string).")]
    public void ScrubMachineName(string extension, ScrubberLocation location) =>
        ScrubMachineName(extension);

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public void ScrubUserName(string extension) =>
        AddScrubber(extension, UserMachineScrubber.UserPatternScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubUserName(string).")]
    public void ScrubUserName(string extension, ScrubberLocation location) =>
        ScrubUserName(extension);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public void ScrubLinesContaining(string extension, StringComparison comparison, params string[] stringToMatch)
    {
        Ensure.NotNullOrEmpty(stringToMatch);
        AddScrubber(extension, new RemoveLinesContainingScrubber(comparison, stringToMatch));
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesContaining(string, StringComparison, params string[]).")]
    public void ScrubLinesContaining(string extension, StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(extension, comparison, stringToMatch);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public void ScrubInlineGuids(string extension) =>
        AddScrubber(extension, GuidPatternScrubber.Instance);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubInlineGuids(string).")]
    public void ScrubInlineGuids(string extension, ScrubberLocation location) =>
        ScrubInlineGuids(extension);

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public void ScrubLines(string extension, Func<string, bool> removeLine) =>
        AddScrubber(extension, new FilterLinesScrubber(removeLine));

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLines(string, Func<string, bool>).")]
    public void ScrubLines(string extension, Func<string, bool> removeLine, ScrubberLocation location) =>
        ScrubLines(extension, removeLine);

    /// <summary>
    /// Scrub lines with an optional replace.
    /// </summary>
    public void ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine) =>
        AddScrubber(extension, new ReplaceLinesScrubber(replaceLine));

    /// <summary>
    /// Scrub lines with an optional replace.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesWithReplace(string, Func<string, string?>).")]
    public void ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine, ScrubberLocation location) =>
        ScrubLinesWithReplace(extension, replaceLine);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public void ScrubEmptyLines(string extension) =>
        AddScrubber(extension, RemoveEmptyLinesScrubber.Instance);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubEmptyLines(string).")]
    public void ScrubEmptyLines(string extension, ScrubberLocation location) =>
        ScrubEmptyLines(extension);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete("ScrubberLocation is obsolete. Use ScrubLinesContaining(string, StringComparison, params string[]).")]
    public void ScrubLinesContaining(string extension, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(extension, StringComparison.OrdinalIgnoreCase, stringToMatch);
}
