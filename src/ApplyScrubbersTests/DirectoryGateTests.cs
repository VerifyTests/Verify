// Path replacement is pinned last, so it must run against the scrubbed document
// rather than being gated on the length of the pre-scrub source.
public class DirectoryGateTests
{
    static DirectoryGateTests() =>
        EngineRunner.UseFakeDirectories();

    [Fact]
    public void TransformExpandedTextIsPathScrubbed()
    {
        var result = EngineRunner.RunWithDirectoryReplacements(
            "here",
            Scrubber.ReplaceLines((string line) => line == "here" ? "C:/Code/TheSolution/TheProject/file.txt" : line));
        Assert.Equal("{ProjectDirectory}file.txt", result);
    }

    [Fact]
    public void ReplacementTextStaysQuarantined()
    {
        var result = EngineRunner.RunWithDirectoryReplacements(
            "x",
            Scrubber.Replace("x", "C:/Code/TheSolution/TheProject/f.txt"));
        // Replacement text is quarantined, so it is deliberately not path scrubbed
        Assert.Equal("C:/Code/TheSolution/TheProject/f.txt", result);
    }

    [Fact]
    public void ShortSourceWithNoScrubbersIsUnchanged() =>
        Assert.Equal("here", EngineRunner.RunWithDirectoryReplacements("here"));
}
