namespace VerifyTests;

public partial class VerifySettings
{
    List<Action<StringBuilder>> instanceScrubbers = new();
    internal Dictionary<string, List<Action<StringBuilder>>> extensionMappedInstanceScrubbers = new();

    public IReadOnlyList<Action<StringBuilder>> InstanceScrubbers => instanceScrubbers;

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public void ScrubMachineName() =>
        ScrubMachineName(ScrubberLocation.First);

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public void ScrubMachineName(ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(Scrubbers.ScrubMachineName, location);

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public void ScrubUserName() =>
        ScrubUserName(ScrubberLocation.First);

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public void ScrubUserName(ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(Scrubbers.ScrubUserName, location);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(Action<StringBuilder> scrubber) =>
        AddScrubber(scrubber, ScrubberLocation.First);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        switch (location)
        {
            case ScrubberLocation.First:
                instanceScrubbers.Insert(0, scrubber);
                break;
            case ScrubberLocation.Last:
                instanceScrubbers.Add(scrubber);
                break;
        }
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(string extension, Action<StringBuilder> scrubber) =>
        AddScrubber(extension, scrubber, ScrubberLocation.First);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(string extension, Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        if (!extensionMappedInstanceScrubbers.TryGetValue(extension, out var values))
        {
            extensionMappedInstanceScrubbers[extension] = values = new();
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
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public void ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch) =>
        ScrubLinesContaining(comparison, ScrubberLocation.First, stringToMatch);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public void ScrubLinesContaining(StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        AddScrubber(_ => _.RemoveLinesContaining(comparison, stringToMatch), location);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public void ScrubInlineGuids() =>
        ScrubInlineGuids(ScrubberLocation.First);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public void ScrubInlineGuids(ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(GuidScrubber.ReplaceGuids, location);

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public void ScrubLines(Func<string, bool> removeLine) =>
        ScrubLines(removeLine, ScrubberLocation.First);

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public void ScrubLines(Func<string, bool> removeLine, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(_ => _.FilterLines(removeLine), location);

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public void ScrubLinesWithReplace(Func<string, string?> replaceLine) =>
        ScrubLinesWithReplace(replaceLine, ScrubberLocation.First);

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public void ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(_ => _.ReplaceLines(replaceLine), location);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public void ScrubEmptyLines() =>
        ScrubEmptyLines(ScrubberLocation.First);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public void ScrubEmptyLines(ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(builder =>
            {
                builder.FilterLines(string.IsNullOrWhiteSpace);
                if (builder.FirstChar() is '\n')
                {
                    builder.Remove(0, 1);
                }

                if (builder.LastChar() is '\n')
                {
                    builder.Length--;
                }
            },
            location);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public void ScrubLinesContaining(params string[] stringToMatch) =>
        ScrubLinesContaining(ScrubberLocation.First, stringToMatch);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public void ScrubLinesContaining(ScrubberLocation location = ScrubberLocation.First, params string[] stringToMatch) =>
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, location, stringToMatch);
}