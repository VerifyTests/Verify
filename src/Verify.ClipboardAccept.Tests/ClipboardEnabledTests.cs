public class ClipboardEnabledTests
{
    [Fact]
    public void ParseEnvironmentVariable()
    {
        Assert.False(ClipboardEnabled.ParseEnvironmentVariable(null));
        Assert.False(ClipboardEnabled.ParseEnvironmentVariable(""));
        Assert.False(ClipboardEnabled.ParseEnvironmentVariable("false"));
        Assert.False(ClipboardEnabled.ParseEnvironmentVariable("0"));
        Assert.False(ClipboardEnabled.ParseEnvironmentVariable("no"));
        Assert.True(ClipboardEnabled.ParseEnvironmentVariable("true"));
        Assert.True(ClipboardEnabled.ParseEnvironmentVariable("1"));
        Assert.True(ClipboardEnabled.ParseEnvironmentVariable("yes"));
        Assert.True(ClipboardEnabled.ParseEnvironmentVariable("ON"));
    }

    [Fact]
    public void ParseEnvironmentVariable_unknownDoesNotThrow() =>
        // Runs from a static constructor, so an unrecognized value must not throw
        // (which would poison the type with a TypeInitializationException).
        Assert.False(ClipboardEnabled.ParseEnvironmentVariable("foo"));

    void EnableClipboard() =>
    #region EnableClipboard
        ClipboardAccept.Enable();
    #endregion

    [Fact]
    public void EscapePath_Posix()
    {
        // Inside double quotes, shell metacharacters must be escaped so paths
        // containing them are not expanded.
        Assert.Equal(@"\$HOME/x", ClipboardAccept.EscapePath("$HOME/x", windows: false));
        Assert.Equal(@"a\`b", ClipboardAccept.EscapePath("a`b", windows: false));
        Assert.Equal("plain/path", ClipboardAccept.EscapePath("plain/path", windows: false));
    }

    [Fact]
    public void EscapePath_Windows() =>
        Assert.Equal(@"C:\dir\file", ClipboardAccept.EscapePath(@"C:\dir\file", windows: true));
}