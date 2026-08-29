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

    // A target name can contain a directory separator, which puts the file below a
    // `{prefix}#` directory where a flat scan cannot see it
    [Fact]
    public void FindVerifiedIncludesNested()
    {
        using var directory = new TempDirectory();
        File.WriteAllText(directory.BuildPath("SomeTest.verified.txt"), "a");
        var nested = WriteNested(directory, "SomeTest#sub", "inner", "file.verified.txt");
        // not a file Verify writes, and the `.*` in the search pattern matches it
        WriteNested(directory, "SomeTest#sub", "extensionless.verified");
        // a different test that happens to start with the same text
        var otherNested = WriteNested(directory, "SomeTestOther#sub", "file.verified.txt");

        var found = MatchingFileFinder.FindVerified("SomeTest", directory)
            .ToList();

        Assert.Equal(2, found.Count);
        Assert.Contains(nested, found);
        Assert.DoesNotContain(otherNested, found);
    }

    [Fact]
    public void DeleteReceivedIncludesNested()
    {
        using var directory = new TempDirectory();
        var nestedReceived = WriteNested(directory, "SomeTest#sub", "inner", "file.received.txt");
        var nestedVerified = WriteNested(directory, "SomeTest#sub", "inner", "file.verified.txt");
        var otherReceived = WriteNested(directory, "SomeTestOther#sub", "file.received.txt");

        MatchingFileFinder.DeleteReceived("SomeTest", directory);

        Assert.False(File.Exists(nestedReceived));
        // only received files are swept, and only for this test
        Assert.True(File.Exists(nestedVerified));
        Assert.True(File.Exists(otherReceived));
    }

    static string WriteNested(TempDirectory directory, params string[] segments)
    {
        var path = Path.Combine([directory.Path, ..segments]);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, "content");
        return path;
    }
}
