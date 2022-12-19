[UsesVerify]
public class InstanceFileAppenderTests
{
    VerifySettings settings;

    public InstanceFileAppenderTests()
    {
        settings = new();
        settings.AppendFile("appendedFile");
    }

    [Fact]
    public Task Text() =>
        Verify("Foo", settings);

    [Fact]
    public Task WithName() =>
        Verify("Foo", settings)
            .AppendFile("extra content", name: "theName");

    #region BinaryFluent

    [Fact]
    public Task BinaryFluent() =>
        Verify("Foo", settings)
            .AppendFile(File.OpenRead("sample.png"));

    #endregion

    #region TextFluent

    [Fact]
    public Task TextFluent() =>
        Verify("Foo")
            .AppendFile("extra content");

    #endregion

    [Fact]
    public Task WithScrubbing() =>
        Verify("Foo")
            .AppendFile("""
                line1
                line2
                line3
                """)
            .ScrubLinesContaining("line2");

    [Fact]
    public Task TextBytesFluent() =>
        Verify("Foo")
            .AppendFile(Encoding.UTF8.GetBytes("appendedFile"));

    [Fact]
    public Task TextStreamFluent() =>
        Verify("Foo")
            .AppendFile(new MemoryStream(Encoding.UTF8.GetBytes("appendedFile")));
}