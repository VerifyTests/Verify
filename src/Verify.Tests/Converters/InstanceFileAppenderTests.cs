[UsesVerify]
public class InstanceFileAppenderTests
{
    VerifySettings settings;

    public InstanceFileAppenderTests()
    {
        settings = new();
        settings.AppendTextFile("appendedFile");
    }

    [Fact]
    public Task Text() =>
        Verify("Foo", settings);

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
            .AppendTextFile("extra content");

    #endregion

    [Fact]
    public Task TextBytesFluent() =>
        Verify("Foo")
            .AppendTextFile(Encoding.UTF8.GetBytes("appendedFile"));

    [Fact]
    public Task TextStreamFluent() =>
        Verify("Foo")
            .AppendTextFile(new MemoryStream(Encoding.UTF8.GetBytes("appendedFile")));
}