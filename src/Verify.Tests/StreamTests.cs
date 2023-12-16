[UsesVerify]
public class StreamTests
{
    [Fact]
    public Task Stream() =>
        Verify(
            new MemoryStream(
            [
                1
            ]));

    class LengthNotSupportedStream(byte[] bytes) : MemoryStream(bytes)
    {
        public override long Length =>
            throw new NotSupportedException();
    }

    [Fact]
    public Task LengthNotSupportedException() =>
        Verify(
            new LengthNotSupportedStream(
            [
                1
            ]));

    [Fact]
    public Task StreamTask() =>
        Verify(
            Task.FromResult(
                new MemoryStream(
                [
                    1
                ])));

    [Fact]
    public Task ByteArray() =>
        Verify(
        [
            1
        ]);

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
            [
                1
            ],
            "bin");

    [Fact]
    public Task StreamMember()
    {
        var stream = new MemoryStream("value"u8.ToArray());
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
        Assert.StartsWith("Empty data is not allowed. Path: ", exception.Message);
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
        var stream = new MemoryStream(
        [
            1,
            2,
            3,
            4
        ]);
        stream.Position = 2;
        return Verify(stream);
    }

    [Fact]
    public Task StreamNotAtStartAsText()
    {
        var stream = new MemoryStream("foo"u8.ToArray());
        stream.Position = 2;
        return Verify(stream, "txt");
    }

    [Fact]
    public Task NoLengthStream()
    {
        var stream = new NoLengthStream(
        [
            1
        ]);

        return Verify(stream);
    }

    [Fact]
    public Task Streams() =>
        Verify(
            new List<Stream>
            {
                new MemoryStream(
                [
                    1
                ]),
                new MemoryStream(
                [
                    2
                ])
            },
            "bin");

    [Fact]
    public Task StreamsWithInfo() =>
        Verify(
            new List<Stream>
            {
                new MemoryStream(
                [
                    1
                ]),
                new MemoryStream(
                [
                    2
                ])
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