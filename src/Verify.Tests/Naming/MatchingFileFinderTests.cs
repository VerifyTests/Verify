public class MatchingFileFinderTests
{
    [Fact]
    public void FindVerifiedWithTrailingSeparator()
    {
        var directory = CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(directory, "SomeTest.verified.txt"), "a");
            File.WriteAllText(Path.Combine(directory, "Other.verified.txt"), "b");

            var withSeparator = directory + Path.DirectorySeparatorChar;
            var found = MatchingFileFinder.FindVerified("SomeTest", withSeparator)
                .ToList();

            Assert.Single(found);
            Assert.EndsWith("SomeTest.verified.txt", found[0]);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public void FindVerifiedWithoutTrailingSeparator()
    {
        var directory = CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(directory, "SomeTest.verified.txt"), "a");
            File.WriteAllText(Path.Combine(directory, "Other.verified.txt"), "b");

            var found = MatchingFileFinder.FindVerified("SomeTest", directory)
                .ToList();

            Assert.Single(found);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    // The Win32 pattern `{prefix}*.verified.*` also matches a stray file named exactly
    // `{prefix}.verified` (a trailing `.*` matches "no extension"), which is shorter than
    // the non-indexed pattern being compared against
    [Fact]
    public void StrayFileWithNoExtensionIsIgnored()
    {
        var directory = CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(directory, "SomeTest.verified"), "a");
            File.WriteAllText(Path.Combine(directory, "SomeTest.verified.txt"), "b");

            var found = MatchingFileFinder.FindVerified("SomeTest", directory)
                .ToList();

            Assert.Single(found);
            Assert.EndsWith("SomeTest.verified.txt", found[0]);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    static string CreateTempDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"MatchingFileFinder{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        return directory;
    }
}
