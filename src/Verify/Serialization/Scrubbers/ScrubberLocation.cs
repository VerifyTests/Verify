namespace VerifyTests;

[Obsolete("ScrubberLocation is no longer used. Pattern scrubbers are ordered by MaxLength descending; line and content scrubbers run in registration order. See the scrubber migration guide.")]
public enum ScrubberLocation
{
    First,
    Last
}
