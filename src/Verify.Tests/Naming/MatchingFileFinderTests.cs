public class MatchingFileFinderTests
{
    [Fact]
    public void FindVerifiedWithTrailingSeparator()
    {
        using var directory = new TempDirectory();
        File.WriteAllText(directory.BuildPath("SomeTest.verified.txt"), "a");
        File.WriteAllText(directory.BuildPath("Other.verified.txt"), "b");

        var withSeparator = directory + Path.DirectorySeparatorChar.ToString();
        var found = MatchingFileFinder.FindVerified("SomeTest", withSeparator)
            .ToList();

        Assert.Single(found);
        Assert.EndsWith("SomeTest.verified.txt", found[0]);
    }

    [Fact]
    public void FindVerifiedWithoutTrailingSeparator()
    {
        using var directory = new TempDirectory();
        File.WriteAllText(directory.BuildPath("SomeTest.verified.txt"), "a");
        File.WriteAllText(directory.BuildPath("Other.verified.txt"), "b");

        var found = MatchingFileFinder.FindVerified("SomeTest", directory)
            .ToList();

        Assert.Single(found);
    }

    // The Win32 pattern `{prefix}*.verified.*` also matches a stray file named exactly
    // `{prefix}.verified` (a trailing `.*` matches "no extension"), which is shorter than
    // the non-indexed pattern being compared against
    [Fact]
    public void StrayFileWithNoExtensionIsIgnored()
    {
        using var directory = new TempDirectory();
        File.WriteAllText(directory.BuildPath("SomeTest.verified"), "a");
        File.WriteAllText(directory.BuildPath("SomeTest.verified.txt"), "b");

        var found = MatchingFileFinder.FindVerified("SomeTest", directory)
            .ToList();

        Assert.Single(found);
        Assert.EndsWith("SomeTest.verified.txt", found[0]);
    }
}
