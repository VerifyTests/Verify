public class BypassComparerTests
{
    // Mimics a converter that emits a canonical source target (eg a document) alongside a derived
    // target (eg a rendered image) whose comparer can mask a real difference in the source.
    static Target[] BuildTargets() =>
    [
        new("bypasssource", new MemoryStream("source-content"u8.ToArray()))
        {
            BypassComparersForSubsequentOnDifference = true
        },
        new("bypassderived", new MemoryStream("derived-content"u8.ToArray()))
    ];

    static int derivedCompareCount;

    // A comparer that masks every difference by always reporting equal.
    static Task<CompareResult> MaskingDerivedComparer(Stream received, Stream verified, IReadOnlyDictionary<string, object> context)
    {
        derivedCompareCount++;
        return Task.FromResult(CompareResult.Equal);
    }

    [Fact]
    public async Task SourceEqual_DerivedComparerUsed()
    {
        derivedCompareCount = 0;
        // The source matches its verified file, so no bypass is triggered and the derived comparer
        // runs and masks the difference in the derived target.
        await Verify(null, BuildTargets())
            .UseStreamComparer(MaskingDerivedComparer, "bypassderived")
            .DisableDiff();
        Assert.Equal(1, derivedCompareCount);
    }

    [Fact]
    public async Task SourceDiffers_DerivedComparerBypassed()
    {
        derivedCompareCount = 0;
        // The source differs from its verified file. Because it is flagged, the derived comparer is
        // bypassed (never invoked) and the otherwise-masked derived difference is surfaced.
        var exception = await Assert.ThrowsAsync<VerifyException>(
            () => Verify(null, BuildTargets())
                .UseStreamComparer(MaskingDerivedComparer, "bypassderived")
                .DisableDiff());
        Assert.Equal(0, derivedCompareCount);
        Assert.Contains("bypassderived", exception.Message);
    }
}
