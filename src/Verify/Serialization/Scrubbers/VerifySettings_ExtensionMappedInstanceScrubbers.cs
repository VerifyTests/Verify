namespace VerifyTests;

public partial class VerifySettings
{
    internal Dictionary<string, List<Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>>>>? ExtensionMappedInstanceScrubbers = [];

    internal Dictionary<string, List<Scrubber>>? ExtensionMappedInstanceSpanScrubbers;

    /// <summary>
    /// Add a <see cref="Scrubber" /> that applies to verified files with a matching extension.
    /// </summary>
    public void AddScrubber(string extension, Scrubber scrubber)
    {
        Ensure.NotNull(scrubber);
        ExtensionMappedInstanceSpanScrubbers ??= [];

        if (!ExtensionMappedInstanceSpanScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedInstanceSpanScrubbers[extension] = values = [];
        }

        values.Add(scrubber);
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(string extension, Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, (builder, _) => scrubber(builder), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(string extension, Action<StringBuilder, Counter> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, (builder, counter, _) =>
            scrubber(builder, counter), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(string extension, Action<StringBuilder, Counter, IReadOnlyDictionary<string, object>> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        ExtensionMappedInstanceScrubbers ??= [];

        if (!ExtensionMappedInstanceScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedInstanceScrubbers[extension] = values = [];
        }

        switch (location)
        {
            case ScrubberLocation.First:
                values.Insert(0, scrubber);
                break;
            case ScrubberLocation.Last:
                values.Add(scrubber);
                break;
        }
    }

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public void ScrubMachineName(string extension) =>
        AddScrubber(extension, UserMachineScrubber.MachineScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubMachineName(string extension, ScrubberLocation location) =>
        ScrubMachineName(extension);

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public void ScrubUserName(string extension) =>
        AddScrubber(extension, UserMachineScrubber.UserScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubUserName(string extension, ScrubberLocation location) =>
        ScrubUserName(extension);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public void ScrubLinesContaining(string extension, StringComparison comparison, params string[] stringToMatch) =>
        AddScrubber(extension, Scrubber.RemoveLinesContaining(comparison, stringToMatch));

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubLinesContaining(string extension, StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(extension, comparison, stringToMatch);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public void ScrubInlineGuids(string extension) =>
        AddScrubber(extension, GuidMatcher.Instance);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubInlineGuids(string extension, ScrubberLocation location) =>
        ScrubInlineGuids(extension);

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public void ScrubLines(string extension, Func<string, bool> removeLine) =>
        AddScrubber(extension, Scrubber.RemoveLines(removeLine));

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubLines(string extension, Func<string, bool> removeLine, ScrubberLocation location) =>
        ScrubLines(extension, removeLine);

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public void ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine) =>
        AddScrubber(extension, Scrubber.ReplaceLines(replaceLine));

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine, ScrubberLocation location) =>
        ScrubLinesWithReplace(extension, replaceLine);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public void ScrubEmptyLines(string extension) =>
        AddScrubber(extension, Scrubber.RemoveEmptyLines());

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubEmptyLines(string extension, ScrubberLocation location) =>
        ScrubEmptyLines(extension);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubLinesContaining(string extension, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(extension, StringComparison.OrdinalIgnoreCase, stringToMatch);
}
