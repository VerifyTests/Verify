public class ExcludeTargetsTests :
    BaseTest
{
    [Fact]
    public async Task GlobalExclusionAppliesToAllVerifications()
    {
        VerifierSettings.ExcludeTargets("excludedbin");
        Target[] targets = [new("excludedbin", new MemoryStream("binary-content"u8.ToArray()))];
        var result = await Verify(null, targets)
            .DisableDiff();
        Assert.Single(result.Files);
    }

    // A converter can see a global exclusion through its context, so it can skip the work.
    [Fact]
    public async Task ConverterObservesGlobalExclusion()
    {
        var sawExclusion = false;
        VerifierSettings.RegisterStreamConverter(
            "globalexcludecheck",
            (_, _, context) =>
            {
                sawExclusion = context.IsTargetExcluded("globalexcludecheck");
                return new(null, [new("txt", "pages")]);
            });
        VerifierSettings.ExcludeTargets("globalexcludecheck");

        await Verify(new MemoryStream("input"u8.ToArray()), "globalexcludecheck")
            .DisableDiff();

        Assert.True(sawExclusion);
    }

    [Fact]
    public void AfterVerifyHasBeenRunThrows()
    {
        InnerVerifier.verifyHasBeenRun = true;
        Assert.Throws<Exception>(() => VerifierSettings.ExcludeTargets("excludedbin"));
    }
}
