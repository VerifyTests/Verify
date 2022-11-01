// Non-nullable field is uninitialized.

#pragma warning disable CS8618

[UsesVerify]
public class StreamTests
{
    [Fact]
    public Task Stream() =>
        Verify(
            new MemoryStream(new byte[]
            {
                1
            }),
            "bin");

    #region StreamWithExtension

    [Fact]
    public Task StreamWithExtension()
    {
        var stream = new MemoryStream(File.ReadAllBytes("sample.png"));
        return Verify(stream, "png");
    }

    #endregion

    #region FileStream

    [Fact]
    public Task FileStream() =>
        Verify(File.OpenRead("sample.txt"));

    #endregion

    [Fact]
    public Task StreamNotAtStart()
    {
        var stream = new MemoryStream(new byte[]
        {
            1,
            2,
            3,
            4
        });
        stream.Position = 2;
        return Verify(stream);
    }

    [Fact]
    public Task StreamNotAtStartAsText()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("foo"));
        stream.Position = 2;
        return Verify(stream, "txt");
    }

    [Fact]
    public Task Streams() =>
        Verify(
            new List<Stream>
            {
                new MemoryStream(new byte[]
                {
                    1
                }),
                new MemoryStream(new byte[]
                {
                    2
                })
            },
            "bin");


#if !NETFRAMEWORK
    [Fact]
    public Task VerifyBytesAsync() =>
        Verify(File.ReadAllBytesAsync("sample.jpg"), "jpg");
#endif

    #region VerifyFile

    [Fact]
    public Task VerifyFilePath() =>
        VerifyFile("sample.txt");

    #endregion

#if NET6_0

    [Fact]
    public async Task VerifyFileNotLocked()
    {
        await VerifyFile("sampleNotLocked.txt");
        Assert.False(FileEx.IsFileLocked("sampleNotLocked.txt"));
    }

#endif

    #region VerifyFileWithInfo

    [Fact]
    public Task VerifyFileWithInfo() =>
        VerifyFile(
            "sample.txt",
            info: "the info");

    #endregion

    [Fact]
    public Task VerifyFileWithAppend() =>
        VerifyFile("sample.txt")
            .AppendValue("key", "value");

    [Fact]
    public Task OnlyExtension() =>
        VerifyFile(".sample");
}