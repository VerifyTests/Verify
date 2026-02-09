namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static List<SpanScrubber> GlobalSpanScrubbers = [];

    /// <summary>
    /// Modify the resulting test content using custom span-based code.
    /// </summary>
    public static void AddSpanScrubber(int? minLength, int? maxLength, SpanScrubHandler tryConvert, ScrubberLocation location = ScrubberLocation.First)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        var scrubber = new SpanScrubber(minLength, maxLength, tryConvert);
        switch (location)
        {
            case ScrubberLocation.First:
                GlobalSpanScrubbers.Insert(0, scrubber);
                break;
            case ScrubberLocation.Last:
                GlobalSpanScrubbers.Add(scrubber);
                break;
        }
    }

    internal static void AddSpanScrubber(SpanScrubber scrubber, ScrubberLocation location)
    {
        switch (location)
        {
            case ScrubberLocation.First:
                GlobalSpanScrubbers.Insert(0, scrubber);
                break;
            case ScrubberLocation.Last:
                GlobalSpanScrubbers.Add(scrubber);
                break;
        }
    }

    internal static void AddSpanScrubbers(SpanScrubber[] scrubbers, ScrubberLocation location)
    {
        switch (location)
        {
            case ScrubberLocation.First:
                GlobalSpanScrubbers.InsertRange(0, scrubbers);
                break;
            case ScrubberLocation.Last:
                GlobalSpanScrubbers.AddRange(scrubbers);
                break;
        }
    }
}
