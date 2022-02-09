namespace VerifyTests;

public partial class VerifySettings
{
    List<Action<StringBuilder>> instanceScrubbers = new();
    internal Dictionary<string, List<Action<StringBuilder>>> extensionMappedInstanceScrubbers = new();

    public IReadOnlyList<Action<StringBuilder>> InstanceScrubbers
    {
        get
        {
            return instanceScrubbers;
        }
    }

    /// <summary>
    /// Remove the <see cref="Environment.MachineName"/> from the test results.
    /// </summary>
    public void ScrubMachineName()
    {
        AddScrubber(Scrubbers.ScrubMachineName);
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(Action<StringBuilder> scrubber)
    {
        instanceScrubbers.Insert(0, scrubber);
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(string extension, Action<StringBuilder> scrubber)
    {
        if (!extensionMappedInstanceScrubbers.TryGetValue(extension, out var values))
        {
            extensionMappedInstanceScrubbers[extension] = values = new();
        }

        values.Add(scrubber);
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch"/> from the test results.
    /// </summary>
    public void ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
    {
        instanceScrubbers.Insert(0, s => s.RemoveLinesContaining(comparison, stringToMatch));
    }

    //TODO: should only do this when it is a string.
    //and instead pass a bool to the json serializer for the object scenario
    //Same for the static
    /// <summary>
    /// Replace inline <see cref="Guid"/>s with a placeholder.
    /// Uses a <see cref="Regex"/> to find <see cref="Guid"/>s inside strings.
    /// </summary>
    public void ScrubInlineGuids()
    {
        instanceScrubbers.Insert(0, GuidScrubber.ReplaceGuids);
    }

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine"/> from the test results.
    /// </summary>
    public void ScrubLines(Func<string, bool> removeLine)
    {
        instanceScrubbers.Insert(0, s => s.FilterLines(removeLine));
    }

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine"/> can return the input to ignore the line, or return a a different string to replace it.
    /// </summary>
    public void ScrubLinesWithReplace(Func<string, string?> replaceLine)
    {
        instanceScrubbers.Insert(0, s => s.ReplaceLines(replaceLine));
    }

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public void ScrubEmptyLines()
    {
        instanceScrubbers.Insert(0, s => s.FilterLines(string.IsNullOrWhiteSpace));
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch"/> from the test results.
    /// </summary>
    public void ScrubLinesContaining(params string[] stringToMatch)
    {
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);
    }
}