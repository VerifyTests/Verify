public class DirecotryReplacementTests
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
    [InlineData(@"C:\Code\VerifyTests\Verify\src\Verify.Tests\Foo", @"C:\Code\VerifyTests\Verify\src\Verify.Tests", "{replace}Foo")]
    public void Tests(string input, string find, string expected)
    {
        List<KeyValuePair<string, string>> keyValuePairs = [new(find, "{replace}")];
        var builder = new StringBuilder(input);
        builder.ReplaceDirectoryPaths(keyValuePairs);
        Assert.Equal(expected, builder.ToString());
    }

    [Fact]
    public void ProcessLongerDiretoryFirst()
    {
        List<KeyValuePair<string, string>> keyValuePairs = [
            new(@"C:\Parent\Child", "{Child}"),
            new(@"C:\Parent", "{Parent}"),
        ];
        var builder = new StringBuilder(@"C:\Parent\Child\Dir");
        builder.ReplaceDirectoryPaths(keyValuePairs);
        Assert.Equal("{Child}Dir", builder.ToString());
    }
}