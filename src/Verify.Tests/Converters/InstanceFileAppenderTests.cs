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
            .AppendFile("sample.png");

    #endregion

    #region TextFluent

    [Fact]
    public Task TextFluent() =>
        Verify("Foo")
            .AppendFile("extra content");

    #endregion

    [Fact]
    public Task TextBytesFluent() =>
        Verify("Foo")
            .AppendFile(Encoding.UTF8.GetBytes("appendedFile"));

    [Fact]
    public Task TextStreamFluent() =>
        Verify("Foo")
            .AppendFile(new MemoryStream(Encoding.UTF8.GetBytes("appendedFile")));
}