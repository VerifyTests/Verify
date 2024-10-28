namespace VerifyTests;

public partial class VerifySettings
{
    internal Dictionary<string, List<Action<StringBuilder, Counter>>> ExtensionMappedInstanceScrubbers = [];

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public VerifySettings AddScrubber(string extension, Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, (builder, _) => scrubber(builder), location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public VerifySettings AddScrubber(string extension, Action<StringBuilder, Counter> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
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

        return this;
    }

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public VerifySettings ScrubMachineName(string extension, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, Scrubbers.ScrubMachineName, location);

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public VerifySettings ScrubUserName(string extension, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, Scrubbers.ScrubUserName, location);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public VerifySettings ScrubLinesContaining(string extension, StringComparison comparison, params string[] stringToMatch) =>
        ScrubLinesContaining(extension, comparison, ScrubberLocation.First, stringToMatch);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public VerifySettings ScrubLinesContaining(string extension, StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        AddScrubber(extension, _ => _.RemoveLinesContaining(comparison, stringToMatch), location);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public VerifySettings ScrubInlineGuids(string extension, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, GuidScrubber.ReplaceGuids, location);

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public VerifySettings ScrubLines(string extension, Func<string, bool> removeLine, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, _ => _.FilterLines(removeLine), location);

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public VerifySettings ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, _ => _.ReplaceLines(replaceLine), location);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public VerifySettings ScrubEmptyLines(string extension, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(extension, _ => _.RemoveEmptyLines(), location);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public VerifySettings ScrubLinesContaining(string extension, ScrubberLocation location = ScrubberLocation.First, params string[] stringToMatch) =>
        ScrubLinesContaining(extension, StringComparison.OrdinalIgnoreCase, location, stringToMatch);
}