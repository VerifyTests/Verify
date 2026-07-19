public class OrderingTests
{
    static OrderingTests() =>
        EngineRunner.UseFakeDirectories();

    [Fact]
    public void UnknownMaxRunsFirst()
    {
        // The Match scrubber has no max length, so it runs before the known length
        // Replace even though the Replace was registered first
        var result = EngineRunner.Run(
            "abcdef",
            Scrubber.Replace("abcdef", "R"),
            Scrubber.Match((CharSpan segment, Counter _, IReadOnlyDictionary<string, object> _, out int index, out int length, out string? replacement) =>
            {
                index = segment.IndexOf("abc".AsSpan());
                if (index < 0)
                {
                    length = 0;
                    replacement = null;
                    return false;
                }

                length = 3;
                replacement = "M";
                return true;
            }));
        Assert.Equal("Mdef", result);
    }

    [Fact]
    public void LongestMaxRunsFirst()
    {
        // Registered shortest first; the longer max must still win the overlap
        var result = EngineRunner.Run(
            "abc",
            Scrubber.Replace("bc", "X"),
            Scrubber.Replace("abc", "Y"));
        Assert.Equal("Y", result);
    }

    [Fact]
    public void EqualMax_RegistrationOrderWins()
    {
        var result = EngineRunner.Run(
            "ab",
            Scrubber.Replace("ab", "1"),
            Scrubber.Replace("ab", "2"));
        Assert.Equal("1", result);
    }

    [Fact]
    public void PathReplacementsApply()
    {
        var result = EngineRunner.RunWithDirectoryReplacements("C:/Code/TheSolution/TheProject/file.txt");
        Assert.Equal("{ProjectDirectory}file.txt", result);
    }

    [Fact]
    public void UserScrubberBeatsPathReplacements()
    {
        // Path replacements are pinned last: a user scrubber matching the raw path
        // wins, mirroring MoreSpecificScrubberShouldOverride
        var result = EngineRunner.RunWithDirectoryReplacements(
            "C:/Code/TheSolution/TheProject/file.txt",
            Scrubber.Replace("C:/Code/TheSolution/TheProject/file.txt", "{custom}"));
        Assert.Equal("{custom}", result);
    }

    [Fact]
    public void PathReplacements_RespectQuarantine()
    {
        // The user scrubber consumes the project segment, so the full project path
        // can no longer match; the remaining raw prefix still matches the solution
        // directory (greedily absorbing the trailing separator)
        var result = EngineRunner.RunWithDirectoryReplacements(
            "C:/Code/TheSolution/TheProject",
            Scrubber.Replace("TheProject", "{p}"));
        Assert.Equal("{SolutionDirectory}{p}", result);
    }
}
