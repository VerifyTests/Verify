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
    public void Tests(string input, string find, string expected)
    {
        List<KeyValuePair<string, string>> keyValuePairs = [new(find, "{replace}")];
        var builder = new StringBuilder(input);
        builder.ReplaceDirectoryPaths(keyValuePairs);
        Assert.Equal(expected, builder.ToString());
    }
}