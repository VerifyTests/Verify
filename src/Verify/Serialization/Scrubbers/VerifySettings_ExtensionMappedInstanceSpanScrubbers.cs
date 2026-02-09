namespace VerifyTests;

public partial class VerifySettings
{
    internal Dictionary<string, List<SpanScrubber>>? ExtensionMappedInstanceSpanScrubbers;

    /// <summary>
    /// Modify the resulting test content using custom span-based code.
    /// </summary>
    public void AddSpanScrubber(string extension, int? minLength, int? maxLength, SpanScrubHandler tryConvert, ScrubberLocation location = ScrubberLocation.First)
    {
        var scrubber = new SpanScrubber(minLength, maxLength, tryConvert);

        ExtensionMappedInstanceSpanScrubbers ??= [];

        if (!ExtensionMappedInstanceSpanScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedInstanceSpanScrubbers[extension] = values = [];
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
