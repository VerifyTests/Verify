// An empty replacement quarantines nothing, so the text on either side of it is
// adjacent document text and must stay matchable by later scrubbers.
public class DeletionSeamTests
{
    static DeletionSeamTests() =>
        EngineRunner.UseFakeDirectories();

    [Fact]
    public void LaterScrubberMatchesAcrossDeletion()
    {
        var result = EngineRunner.Run(
            "a--b",
            Scrubber.Replace("--", ""),
            Scrubber.Replace("ab", "X"));
        Assert.Equal("X", result);
    }

    [Fact]
    public void LongerFindMatchesAcrossDeletion()
    {
        var result = EngineRunner.Run(
            "daREDACT-ta",
            Scrubber.Replace("REDACT-", ""),
            Scrubber.Replace("data", "X"));
        Assert.Equal("X", result);
    }

    [Fact]
    public void MultipleDeletionsAllJoined()
    {
        // Equal max lengths, so these run in registration order and the deletion
        // happens first (inline scrubbers otherwise order by descending max length)
        var result = EngineRunner.Run(
            "aXXbXXcXXd",
            Scrubber.Replace("XX", ""),
            Scrubber.Replace("bc", "Y"));
        Assert.Equal("aYd", result);
    }

    [Fact]
    public void DeletionAfterLinePhaseStillJoins()
    {
        var result = EngineRunner.Run(
            "drop\na--b\nkeep",
            Scrubber.RemoveLinesContaining(StringComparison.Ordinal, "drop"),
            Scrubber.Replace("--", ""),
            Scrubber.Replace("ab", "X"));
        Assert.Equal("X\nkeep", result);
    }

    [Fact]
    public void DirectoryReplacementMatchesAcrossDeletion()
    {
        var result = EngineRunner.RunWithDirectoryReplacements(
            "C:/Co[DEL]de/TheSolution/TheProject/file.txt",
            Scrubber.Replace("[DEL]", ""));
        Assert.Equal("{ProjectDirectory}file.txt", result);
    }

    [Fact]
    public void QuarantineSurvivesDeletion()
    {
        var result = EngineRunner.Run(
            "a-b",
            Scrubber.Replace("-", ""),
            Scrubber.Replace("b", "a"),
            Scrubber.Replace("aa", "BOOM"));
        // The 'a' produced by the second scrubber is quarantined, so "aa" must not form
        Assert.Equal("aa", result);
    }
}
