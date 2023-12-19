[UsesVerify]
public class InstanceFileAppenderTests
{
    VerifySettings settings;

    public InstanceFileAppenderTests()
    {
        settings = new();
        settings.AppendContentAsFile("appendedFile");
    }

    [Fact]
    public Task Text() =>
        Verify("Foo", settings);

    [Fact]
    public Task WithName() =>
        Verify("Foo", settings)
            .AppendContentAsFile("extra content", name: "theName");

    #region AppendFile

    [Fact]
    public Task AppendFile() =>
        Verify("Foo", settings)
            .AppendFile("sample.png");

    #endregion

    #region AppendContentAsFile

    [Fact]
    public Task AppendContentAsFile() =>
        Verify("Foo")
            .AppendContentAsFile("extra content");

    #endregion

    [Fact]
    public Task WithScrubbing() =>
        Verify("Foo")
            .AppendContentAsFile(
                """
                line1
                line2
                line3
                """)
            .ScrubLinesContaining("line2");

    [Fact]
    public Task TextBytesFluent() =>
        Verify("Foo")
            .AppendContentAsFile("appendedFile"u8.ToArray());

    [Fact]
    public Task TextStreamFluent() =>
        Verify("Foo")
            .AppendFile(new MemoryStream("appendedFile"u8.ToArray()));
}