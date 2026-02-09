namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static Dictionary<string, List<SpanScrubber>> ExtensionMappedGlobalSpanScrubbers = [];

    /// <summary>
    /// Modify the resulting test content using custom span-based code.
    /// </summary>
    public static void AddSpanScrubber(string extension, int? minLength, int? maxLength, SpanScrubHandler tryConvert, ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        var scrubber = new SpanScrubber(minLength, maxLength, tryConvert);
        if (!ExtensionMappedGlobalSpanScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedGlobalSpanScrubbers[extension] = values = [];
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
}
