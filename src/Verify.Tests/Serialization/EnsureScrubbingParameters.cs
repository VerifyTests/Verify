public class EnsureScrubbingParameters
{
    [ModuleInitializer]
    public static void Initialize() =>
        VerifierSettings.AddScrubber((builder, counter, context) =>
        {
            Assert.NotNull(counter);
            Assert.NotNull(settings);
            builder.Replace("EnsureScrubbingParameters", "new value");
        });

    [Fact]
    public Task Test() =>
        Verify("EnsureScrubbingParameters");
}