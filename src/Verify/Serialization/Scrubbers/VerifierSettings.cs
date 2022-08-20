﻿namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static List<Action<StringBuilder>> GlobalScrubbers = new();

    static VerifierSettings() =>
        MemberConverter<Exception, string>(_ => _.StackTrace, (_, value) => Scrubbers.ScrubStackTrace(value));

    internal static Dictionary<string, List<Action<StringBuilder>>> ExtensionMappedGlobalScrubbers = new();

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(Action<StringBuilder> scrubber) =>
        AddScrubber(scrubber, ScrubberLocation.First);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        switch (location)
        {
            case ScrubberLocation.First:
                GlobalScrubbers.Insert(0, scrubber);
                break;
            case ScrubberLocation.Last:
                GlobalScrubbers.Add(scrubber);
                break;
        }
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(string extension, Action<StringBuilder> scrubber) =>
        AddScrubber(extension, scrubber, ScrubberLocation.First);

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(string extension, Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        if (!ExtensionMappedGlobalScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedGlobalScrubbers[extension] = values = new();
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
    public static void ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch) =>
        ScrubLinesContaining(comparison, ScrubberLocation.First, stringToMatch);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(StringComparison comparison, ScrubberLocation location, params string[] stringToMatch) =>
        AddScrubber(_ => _.RemoveLinesContaining(comparison, stringToMatch), location);

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public static void ScrubLines(Func<string, bool> removeLine) =>
        ScrubLines(removeLine, ScrubberLocation.First);

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine" /> from the test results.
    /// </summary>
    public static void ScrubLines(Func<string, bool> removeLine, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(_ => _.FilterLines(removeLine), location);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public static void ScrubEmptyLines() =>
        ScrubEmptyLines(ScrubberLocation.First);

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public static void ScrubEmptyLines(ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(_ => _.RemoveEmptyLines(), location);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineGuids() =>
        ScrubInlineGuids(ScrubberLocation.First);

    /// <summary>
    /// Replace inline <see cref="Guid" />s with a placeholder.
    /// </summary>
    public static void ScrubInlineGuids(ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(GuidScrubber.ReplaceGuids, location);


    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public static void ScrubLinesWithReplace(Func<string, string?> replaceLine) =>
        ScrubLinesWithReplace(replaceLine, ScrubberLocation.First);

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine" /> can return the input to ignore the line, or return a different string to replace it.
    /// </summary>
    public static void ScrubLinesWithReplace(Func<string, string?> replaceLine, ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(_ => _.ReplaceLines(replaceLine), location);


    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(params string[] stringToMatch) =>
        ScrubLinesContaining(ScrubberLocation.First, stringToMatch);

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch" /> from the test results.
    /// </summary>
    public static void ScrubLinesContaining(ScrubberLocation location = ScrubberLocation.First, params string[] stringToMatch) =>
        ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, location, stringToMatch);

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public static void ScrubMachineName() =>
        ScrubMachineName(ScrubberLocation.First);

    /// <summary>
    /// Remove the <see cref="Environment.MachineName" /> from the test results.
    /// </summary>
    public static void ScrubMachineName(ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(Scrubbers.ScrubMachineName, location);


    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public static void ScrubUserName() =>
        ScrubUserName(ScrubberLocation.First);

    /// <summary>
    /// Remove the <see cref="Environment.UserName" /> from the test results.
    /// </summary>
    public static void ScrubUserName(ScrubberLocation location = ScrubberLocation.First) =>
        AddScrubber(Scrubbers.ScrubUserName, location);
}