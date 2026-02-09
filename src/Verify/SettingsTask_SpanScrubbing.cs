namespace VerifyTests;

public partial class SettingsTask
{
    /// <inheritdoc cref="VerifySettings.AddSpanScrubber(int?,int?,SpanScrubHandler,ScrubberLocation)"/>
    [Pure]
    public SettingsTask AddSpanScrubber(int? minLength, int? maxLength, SpanScrubHandler tryConvert, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.AddSpanScrubber(minLength, maxLength, tryConvert, location);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AddSpanScrubber(string,int?,int?,SpanScrubHandler,ScrubberLocation)"/>
    [Pure]
    public SettingsTask AddSpanScrubber(string extension, int? minLength, int? maxLength, SpanScrubHandler tryConvert, ScrubberLocation location = ScrubberLocation.First)
    {
        CurrentSettings.AddSpanScrubber(extension, minLength, maxLength, tryConvert, location);
        return this;
    }
}
