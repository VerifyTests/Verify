// Thin facade preserved for the existing call sites; routes to ScrubberPipeline.

static class ApplyScrubbers
{
    public static void ApplyForExtension(string extension, StringBuilder target, VerifySettings settings, Counter counter) =>
        ScrubberPipeline.ApplyForExtension(extension, target, settings, counter);

    public static string ApplyForPropertyValue(CharSpan value, VerifySettings settings, Counter counter) =>
        ScrubberPipeline.ApplyForPropertyValue(value, settings, counter);
}
