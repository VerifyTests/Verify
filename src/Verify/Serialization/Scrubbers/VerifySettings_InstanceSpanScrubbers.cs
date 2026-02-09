namespace VerifyTests;

public partial class VerifySettings
{
    internal List<SpanScrubber>? InstanceSpanScrubbers;

    /// <summary>
    /// Modify the resulting test content using custom span-based code.
    /// </summary>
    public void AddSpanScrubber(int? minLength, int? maxLength, SpanScrubHandler tryConvert, ScrubberLocation location = ScrubberLocation.First)
    {
        var scrubber = new SpanScrubber(minLength, maxLength, tryConvert);
        AddSpanScrubber(scrubber, location);
    }

    internal void AddSpanScrubber(SpanScrubber scrubber, ScrubberLocation location)
    {
        if (InstanceSpanScrubbers == null)
        {
            InstanceSpanScrubbers = [scrubber];
            return;
        }

        switch (location)
        {
            case ScrubberLocation.First:
                InstanceSpanScrubbers.Insert(0, scrubber);
                break;
            case ScrubberLocation.Last:
                InstanceSpanScrubbers.Add(scrubber);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(location), location, null);
        }
    }

    internal void AddSpanScrubbers(SpanScrubber[] scrubbers, ScrubberLocation location)
    {
        if (InstanceSpanScrubbers == null)
        {
            InstanceSpanScrubbers = [..scrubbers];
            return;
        }

        switch (location)
        {
            case ScrubberLocation.First:
                InstanceSpanScrubbers.InsertRange(0, scrubbers);
                break;
            case ScrubberLocation.Last:
                InstanceSpanScrubbers.AddRange(scrubbers);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(location), location, null);
        }
    }
}
