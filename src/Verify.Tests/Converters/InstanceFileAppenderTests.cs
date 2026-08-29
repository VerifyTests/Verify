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

    // The engine disposes the stream of every target it writes, so an appended binary file
    // held as a live stream was dead after the first verification. Both of these are backed
    // by something re-readable, so reusing the settings has to work.
    [Fact]
    public async Task BinaryBytesSettingsReuse()
    {
        var reused = new VerifySettings();
        reused.AppendContentAsFile(new byte[] {1, 2, 3}, "bin", "appendedBytes");

        await Verify("First", reused)
            .UseMethodName("BinaryBytesSettingsReuse_first");
        await Verify("Second", reused)
            .UseMethodName("BinaryBytesSettingsReuse_second");
    }

    [Fact]
    public async Task AppendFileSettingsReuse()
    {
        var reused = new VerifySettings();
        reused.AppendFile("sample.png");

        await Verify("First", reused)
            .UseMethodName("AppendFileSettingsReuse_first");
        await Verify("Second", reused)
            .UseMethodName("AppendFileSettingsReuse_second");
    }
}