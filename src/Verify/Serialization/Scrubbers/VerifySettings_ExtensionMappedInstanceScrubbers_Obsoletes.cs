namespace VerifyTests;

public partial class VerifySettings
{
    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubMachineName(string extension, ScrubberLocation location) =>
        ScrubMachineName(extension);

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubUserName(string extension, ScrubberLocation location) =>
        ScrubUserName(extension);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubLinesContaining(string extension, StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        ScrubLinesContaining(extension, comparison, stringToMatch);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    [Obsolete(locationObsolete)]
    public void ScrubInlineGuids(string extension, ScrubberLocation location) =>
        ScrubInlineGuids(extension);

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
    [Obsolete(locationObsolete)]
    public void ScrubLinesWithReplace(string extension, Func<string, string?> replaceLine, ScrubberLocation location) =>
        ScrubLinesWithReplace(extension, replaceLine);

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
