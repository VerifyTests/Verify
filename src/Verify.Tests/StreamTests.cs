[UsesVerify]
public class StreamTests
{
    [Fact]
    public Task Stream() =>
        Verify(
            new MemoryStream(
                new byte[]
                {
                    1
                }));

    [Fact]
    public Task StreamTask() =>
        Verify(
            Task.FromResult(
                new MemoryStream(new byte[]
                {
                    1
                })));

    [Fact]
    public Task ByteArray() =>
        Verify(
            new byte[]
            {
                1
            });

    [Fact]
    public Task ByteArrayTask() =>
        Verify(
            Task.FromResult(
                new byte[]
                {
                    1
                }));

    [Fact]
    public Task ByteArrayWithExtension() =>
        Verify(
            new byte[]
            {
                1
            },
            "bin");

    [Fact]
    public Task StreamMember()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("value"));
        return Verify(
            new
            {
                stream
            });
    }

    [Fact]
    public Task VerifyBytes() =>
        Verify(File.ReadAllBytes("sample.jpg"), "jpg");

    [Fact]
    public async Task EmptyBinary()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => Verify(Array.Empty<byte>(), "bin"));
        Assert.Equal("Empty data is not allowed.", exception.Message);
    }

    [Fact]
    public Task NestedByteArray() =>
        Verify(
            new
            {
                bytes = new byte[]
                {
                    1
                }
            });

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
    public Task FileStreamTask() =>
        Verify(Task.FromResult(File.OpenRead("sample.txt")));

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

    [Fact]
    public Task StreamsWithInfo() =>
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
            "bin",
            info: "info");

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

#if NET6_0_OR_GREATER

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