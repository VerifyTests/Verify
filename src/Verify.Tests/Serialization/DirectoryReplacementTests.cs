public class DirectoryReplacementTests
{
    [Theory]
    [InlineData("C:/path", "C:/path", "{replace}")]
    [InlineData("/C:/path", "C:/path", "/{replace}")]
    [InlineData("C:/path/", "C:/path", "{replace}")]
    [InlineData(@"C:\path", "C:/path", "{replace}")]
    [InlineData(@"\C:\path", "C:/path", @"\{replace}")]
    [InlineData(@"C:\path\", "C:/path", "{replace}")]
    [InlineData("C:/path", @"C:\path", "{replace}")]
    [InlineData("C:/path/", @"C:\path", "{replace}")]
    [InlineData(@"C:\path", @"C:\path", "{replace}")]
    [InlineData(@"C:\path\", @"C:\path", "{replace}")]
    [InlineData("C:/path/a", "C:/path", "{replace}a")]
    [InlineData(@"C:\path\a", "C:/path", "{replace}a")]
    [InlineData("C:/path/a", @"C:\path", "{replace}a")]
    [InlineData(@"C:\path\a", @"C:\path", "{replace}a")]
    [InlineData("C:/path/a/", "C:/path", "{replace}a/")]
    [InlineData(@"C:\path\a/", "C:/path", "{replace}a/")]
    [InlineData("C:/path/a/", @"C:\path", "{replace}a/")]
    [InlineData(@"C:\path\a/", @"C:\path", "{replace}a/")]
    [InlineData("C:/path/ ", "C:/path", "{replace} ")]
    [InlineData(@"C:\path ", "C:/path", "{replace} ")]
    [InlineData(@"C:\path\ ", "C:/path", "{replace} ")]
    [InlineData("C:/path ", @"C:\path", "{replace} ")]
    [InlineData("C:/path/ ", @"C:\path", "{replace} ")]
    [InlineData(@"C:\path ", @"C:\path", "{replace} ")]
    [InlineData(@"C:\path\ ", @"C:\path", "{replace} ")]
    [InlineData(@"\C:\path\ ", @"C:\path", @"\{replace} ")]
    [InlineData(@"/C:\path\ ", @"C:\path", "/{replace} ")]
    [InlineData(@":C:\path\ ", @"C:\path", ":{replace} ")]
    [InlineData(@" C:\path\ ", @"C:\path", " {replace} ")]
    [InlineData(@" C:\path\", @"C:\path", " {replace}")]
    [InlineData("overlyeager/tmp/replacement", "/tmp", "overlyeager/tmp/replacement")]
    public void BasicReplacements(string input, string find, string expected)
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs = [new(find, "{replace}")];
        var builder = new StringBuilder(input);
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal(expected, builder.ToString());
    }

    [Fact]
    public void ProcessLongerDirectoryFirst()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\Parent\Child", "{Child}"),
            new(@"C:\Parent", "{Parent}"),
        ];
        var builder = new StringBuilder(@"C:\Parent\Child\Dir");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("{Child}Dir", builder.ToString());
    }

    [Fact]
    public void ProcessLongerDirectoryFirst_Reversed()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\Parent", "{Parent}"),
            new(@"C:\Parent\Child", "{Child}"),
        ];
        var builder = new StringBuilder(@"C:\Parent\Child\Dir");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("{Child}Dir", builder.ToString());
    }

    [Fact]
    public void MultipleReplacements()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\Path1", "{Path1}"),
            new(@"D:\Path2", "{Path2}"),
        ];
        var builder = new StringBuilder(@"C:\Path1\file.txt and D:\Path2\other.txt");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("{Path1}file.txt and {Path2}other.txt", builder.ToString());
    }

    [Fact]
    public void NoMatches()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\NotFound", "{replace}"),
        ];
        var builder = new StringBuilder(@"C:\Different\Path");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal(@"C:\Different\Path", builder.ToString());
    }

    [Fact]
    public void EmptyStringBuilder()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\Path", "{replace}"),
        ];
        var builder = new StringBuilder();
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("", builder.ToString());
    }

    [Fact]
    public void EmptyPathsList()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs = [];
        var builder = new StringBuilder(@"C:\Path\file.txt");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal(@"C:\Path\file.txt", builder.ToString());
    }

    [Theory]
    [InlineData("aC:/path", "C:/path", "aC:/path")] // Letter before
    [InlineData("9C:/path", "C:/path", "9C:/path")] // Digit before
    [InlineData("C:/patha", "C:/path", "C:/patha")] // Letter after
    [InlineData("C:/path9", "C:/path", "C:/path9")] // Digit after
    public void InvalidPrecedingOrTrailingCharacters(string input, string find, string expected)
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs = [new(find, "{replace}")];
        var builder = new StringBuilder(input);
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal(expected, builder.ToString());
    }

    [Fact]
    public void MixedSeparators()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new("C:/path", "{replace}"),
        ];
        var builder = new StringBuilder(@"C:\path\file.txt");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("{replace}file.txt", builder.ToString());
    }

    [Fact]
    public void LargeString_CrossChunkMatching()
    {
        // Create a string that will definitely span multiple chunks
        var largePath = @"C:\VeryLongPath";
        // Force multiple chunks
        var padding = new string(' ', 8192);

        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(largePath, "{replaced}"),
        ];

        var builder = new StringBuilder(padding + largePath + @"\file.txt");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal(padding + "{replaced}file.txt", builder.ToString());
    }

    [Fact]
    public void MultipleOccurrencesOfSamePath()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\Path", "{Path}"),
        ];
        var builder = new StringBuilder(@"C:\Path\a.txt and C:\Path\b.txt");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("{Path}a.txt and {Path}b.txt", builder.ToString());
    }

    [Fact]
    public void OverlappingPaths_ThreeLevels()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\A", "{A}"),
            new(@"C:\A\B", "{B}"),
            new(@"C:\A\B\C", "{C}"),
        ];
        var builder = new StringBuilder(@"C:\A\B\C\file.txt");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("{C}file.txt", builder.ToString());
    }

    [Fact]
    public void NonOverlappingMultiplePaths()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\A", "{A}"),
            new(@"D:\B", "{B}"),
        ];
        var builder = new StringBuilder(@"C:\A\file.txt in D:\B\other.txt");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("{A}file.txt in {B}other.txt", builder.ToString());
    }

    [Fact]
    public void PathAtStart()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\Path", "{replace}"),
        ];
        var builder = new StringBuilder(@"C:\Path\file.txt");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("{replace}file.txt", builder.ToString());
    }

    [Fact]
    public void PathAtEnd()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\Path", "{replace}"),
        ];
        var builder = new StringBuilder(@"Prefix C:\Path");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("Prefix {replace}", builder.ToString());
    }

    [Fact]
    public void PathAtEndWithSeparator()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\Path", "{replace}"),
        ];
        var builder = new StringBuilder(@"Prefix C:\Path\");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("Prefix {replace}", builder.ToString());
    }

    [Fact]
    public void DoubleSlashes()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\Path", "{replace}"),
        ];
        var builder = new StringBuilder(@"C:\Path\\file.txt");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal(@"{replace}\file.txt", builder.ToString());
    }

    [Fact]
    public void UnixStylePaths()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new("/usr/local/bin", "{bin}"),
        ];
        var builder = new StringBuilder("/usr/local/bin/app");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("{bin}app", builder.ToString());
    }

    [Fact]
    public void UnixStylePathsWithBackslashInContent()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new("/usr/local/bin", "{bin}"),
        ];
        var builder = new StringBuilder(@"\usr\local\bin\app");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("{bin}app", builder.ToString());
    }

    [Fact]
    public void PathsInQuotes()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\Path", "{replace}"),
        ];
        var builder = new StringBuilder(
            """
            "C:\Path\file.txt"
            """);
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal(
            """
            "{replace}file.txt"
            """,
            builder.ToString());
    }

    [Fact]
    public void VeryLongPath()
    {
        var longPath = $@"C:\{string.Join(@"\", Enumerable.Range(1, 50).Select(i => $"Dir{i}"))}";
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(longPath, "{long}"),
        ];
        var builder = new StringBuilder(longPath + @"\file.txt");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal("{long}file.txt", builder.ToString());
    }

    [Fact]
    public void SpaceInPath()
    {
        List<DirectoryReplacements.KeyValuePair<string, string>> keyValuePairs =
        [
            new(@"C:\Program Files", "{pf}"),
        ];
        var builder = new StringBuilder(@"C:\Program Files\App\file.txt");
        DirectoryReplacements.Replace(builder, keyValuePairs);
        Assert.Equal(@"{pf}App\file.txt", builder.ToString());
    }
}