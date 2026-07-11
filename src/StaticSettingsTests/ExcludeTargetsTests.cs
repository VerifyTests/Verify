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

    [Fact]
    public void AfterVerifyHasBeenRunThrows()
    {
        InnerVerifier.verifyHasBeenRun = true;
        Assert.Throws<Exception>(() => VerifierSettings.ExcludeTargets("excludedbin"));
    }
}
