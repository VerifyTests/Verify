// The engine splices out exactly Find.Length chars and skips text shorter than
// Find. Both only hold for ordinal comparisons.
public class ComparisonTests
{
    [Fact]
    public void LinguisticComparisonRejectedForReplace()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Scrubber.Replace("ab", "X", StringComparison.InvariantCulture));
        Assert.Contains("Ordinal", exception.Message);

        Assert.Throws<ArgumentException>(
            () => Scrubber.Replace("ab", "X", StringComparison.CurrentCulture));
        Assert.Throws<ArgumentException>(
            () => Scrubber.Replace("ab", "X", StringComparison.CurrentCultureIgnoreCase));
        Assert.Throws<ArgumentException>(
            () => Scrubber.Replace(StringComparison.InvariantCultureIgnoreCase, false, ("ab", "X")));
    }

    [Fact]
    public void OrdinalComparisonsAccepted()
    {
        Assert.Equal("X!", EngineRunner.Run("ab!", Scrubber.Replace("ab", "X")));
        Assert.Equal("X!", EngineRunner.Run("AB!", Scrubber.Replace("ab", "X", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void LinguisticNeedleDropsShorterLine()
    {
        // Soft hyphen is ignorable, so the 6 char line matches the 7 char needle.
        // The line may not be skipped for being shorter than the needle.
        var scrubber = Scrubber.RemoveLinesContaining(StringComparison.InvariantCultureIgnoreCase, "sec­ret");
        var result = EngineRunner.Run("keep\nsecret\nkeep2", scrubber);
        Assert.Equal("keep\nkeep2", result);
    }

    [Fact]
    public void OrdinalNeedleDropsMatchingLine()
    {
        var scrubber = Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "secret");
        var result = EngineRunner.Run("keep\nsecret\nkeep2", scrubber);
        Assert.Equal("keep\nkeep2", result);
    }
}
