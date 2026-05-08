// Legacy entry point retained so existing direct callers keep compiling.
// New code should subclass PatternScrubber and use VerifierSettings.AddScrubber.

static class GuidScrubber
{
    public static void ReplaceGuids(StringBuilder builder, Counter counter) =>
        PatternScrubberRunner.Run(builder, GuidPatternScrubber.Instance, counter);
}
