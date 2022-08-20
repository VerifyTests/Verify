namespace VerifyTests;

public partial class SettingsTask
{
    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public SettingsTask AddScrubber(Action<StringBuilder> scrubber)
    {
        CurrentSettings.AddScrubber(scrubber);
        return this;
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public SettingsTask AddScrubber(Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.AddScrubber(scrubber, location);
        return this;
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public SettingsTask AddScrubber(string extension, Action<StringBuilder> scrubber)
    {
        CurrentSettings.AddScrubber(extension, scrubber);
        return this;
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public SettingsTask AddScrubber(string extension, Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.AddScrubber(extension, scrubber, location);
        return this;
    }

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public SettingsTask ScrubInlineGuids()
    {
        CurrentSettings.ScrubInlineGuids();
        return this;
    }

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public SettingsTask ScrubInlineGuids(ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubInlineGuids(location);
        return this;
    }

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public SettingsTask ScrubMachineName()
    {
        CurrentSettings.ScrubMachineName();
        return this;
    }

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public SettingsTask ScrubMachineName(ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubMachineName(location);
        return this;
    }

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public SettingsTask ScrubUserName()
    {
        CurrentSettings.ScrubUserName();
        return this;
    }

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public SettingsTask ScrubUserName(ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubUserName(location);
        return this;
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public SettingsTask ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(comparison, stringToMatch);
        return this;
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public SettingsTask ScrubLinesContaining(StringComparison comparison, ScrubberLocation location = ScrubberLocation.First, params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(comparison, location, stringToMatch);
        return this;
    }

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public SettingsTask ScrubLines(Func<string, bool> removeLine)
    {
        CurrentSettings.ScrubLines(removeLine);
        return this;
    }

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public SettingsTask ScrubLines(Func<string, bool> removeLine, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubLines(removeLine, location);
        return this;
    }

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public SettingsTask ScrubLinesWithReplace(Func<string, string?> replaceLine)
    {
        CurrentSettings.ScrubLinesWithReplace(replaceLine);
        return this;
    }

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public SettingsTask ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubLinesWithReplace(replaceLine, location);
        return this;
    }

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public SettingsTask ScrubEmptyLines()
    {
        CurrentSettings.ScrubEmptyLines();
        return this;
    }

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public SettingsTask ScrubEmptyLines(ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.ScrubEmptyLines(location);
        return this;
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public SettingsTask ScrubLinesContaining(params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(stringToMatch);
        return this;
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public SettingsTask ScrubLinesContaining(ScrubberLocation location = ScrubberLocation.First, params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(location, stringToMatch);
        return this;
    }
}