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
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public void ScrubMachineName(string extension) =>
        AddScrubber(extension, UserMachineScrubber.MachinePatternScrubber());

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public void ScrubUserName(string extension) =>
        AddScrubber(extension, UserMachineScrubber.UserPatternScrubber());

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public void ScrubLinesContaining(string extension, StringComparison comparison, params string[] stringToMatch)
    {
        Ensure.NotNullOrEmpty(stringToMatch);
        AddScrubber(extension, new RemoveLinesContainingScrubber(comparison, stringToMatch));
    }

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public void ScrubInlineGuids(string extension) =>
        AddScrubber(extension, GuidPatternScrubber.Instance);

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public void ScrubLines(string extension, LineFilter removeLine) =>
        AddScrubber(extension, new FilterLinesScrubber(removeLine));

    /// <summary>
    /// Scrub lines with an optional replace.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public void ScrubLinesWithReplace(string extension, LineReplace replaceLine) =>
        AddScrubber(extension, new ReplaceLinesScrubber(replaceLine));

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public void ScrubEmptyLines(string extension) =>
        AddScrubber(extension, RemoveEmptyLinesScrubber.Instance);
}
