public class UseTextForParametersTests
{
    // The text is appended to the file name verbatim, so it has to be a valid file name.
    // A `:` used to pass validation and then, on Windows, divert the received file into an
    // NTFS alternate data stream, so no received file appeared at all.
    [Theory]
    [InlineData("ratio 16:9")]
    [InlineData("a*b")]
    [InlineData("a?b")]
    [InlineData("a|b")]
    // `#` is reserved for the indexed-target namespace
    [InlineData("a#1")]
    public void InvalidCharactersThrow(string text)
    {
        var settings = new VerifySettings();

        var exception = Assert.Throws<ArgumentException>(() => settings.UseTextForParameters(text));
        Assert.Contains("Invalid character for file name", exception.Message);
    }

    [Fact]
    public void ValidTextIsAccepted()
    {
        var settings = new VerifySettings();
        settings.UseTextForParameters("ratio 16-9");

        Assert.Equal("ratio 16-9", settings.parametersText);
    }
}
