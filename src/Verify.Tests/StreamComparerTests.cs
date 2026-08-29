public class StreamComparerTests
{
    [Fact]
    public async Task BinaryEquals()
    {
        // ReSharper disable once UseAwaitUsing
        using var stream1 = File.OpenRead("sample.bmp");
        // ReSharper disable once UseAwaitUsing
        using var stream2 = File.OpenRead("sample.bmp");
        var result = await StreamComparer.AreEqual(stream1, stream2);
        Assert.True(result.IsEqual);
    }

    [Fact]
    public async Task EqualWithLengthNotMultipleOfEight()
    {
        // 13 bytes exercises a partial final block; the trailing bytes of a
        // rented buffer must not affect the result.
        var bytes = new byte[13];
        for (var index = 0; index < bytes.Length; index++)
        {
            bytes[index] = (byte) index;
        }

        using var stream1 = new MemoryStream(bytes);
        using var stream2 = new MemoryStream((byte[]) bytes.Clone());
        var result = await StreamComparer.AreEqual(stream1, stream2);
        Assert.True(result.IsEqual);
    }

    [Fact]
    public async Task MixedEqualSpanningMultipleBuffers()
    {
        // The shape FileComparer produces for a non-seekable received stream: it is
        // buffered into a MemoryStream and compared against the verified file, which is
        // always opened for async IO. So one side completes every read inline and the
        // other does not. Large enough to span several buffers, and deliberately not a
        // multiple of the buffer size, so the final block is partial.
        var bytes = new byte[256 * 1024 + 13];
        new Random(1).NextBytes(bytes);

        var path = Path.Combine(Path.GetTempPath(), $"StreamComparerTests_{Guid.NewGuid():N}.bin");
        File.WriteAllBytes(path, bytes);
        try
        {
            using var received = new MemoryStream((byte[]) bytes.Clone());
            // ReSharper disable once UseAwaitUsing
            using var verified = IoHelpers.OpenRead(path);
            var result = await StreamComparer.AreEqual(received, verified);
            Assert.True(result.IsEqual);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task EqualWithShortReads()
    {
        // A stream is free to return fewer bytes than asked for, and ReadBufferAsync
        // accumulates until the buffer is full so both sides stay chunk aligned. A real
        // FileStream rarely short reads, so it takes a stream that always does to cover
        // that loop. Both sides are equal, so a mis-aligned chunk would surface as a
        // spurious NotEqual.
        var bytes = new byte[256 * 1024 + 13];
        new Random(2).NextBytes(bytes);

        using var received = new ShortReadStream(bytes, maxRead: 1023);
        using var verified = new ShortReadStream((byte[]) bytes.Clone(), maxRead: 337);
        var result = await StreamComparer.AreEqual(received, verified);
        Assert.True(result.IsEqual);
    }

    // Returns at most maxRead bytes per call, regardless of how many are asked for. Only
    // the byte[] overload needs overriding: because this is a derived type, MemoryStream
    // routes the span and Memory reads back through it rather than using its fast path.
    class ShortReadStream(byte[] bytes, int maxRead) :
        MemoryStream(bytes)
    {
        public override int Read(byte[] buffer, int offset, int count) =>
            base.Read(buffer, offset, Math.Min(count, maxRead));
    }

    [Fact]
    public async Task NotEqualInPartialFinalBlock()
    {
        var bytes1 = new byte[13];
        var bytes2 = new byte[13];
        bytes2[12] = 1;

        using var stream1 = new MemoryStream(bytes1);
        using var stream2 = new MemoryStream(bytes2);
        var result = await StreamComparer.AreEqual(stream1, stream2);
        Assert.False(result.IsEqual);
    }

    [Fact]
    public async Task BinaryNotEqualsSameLength()
    {
        // ReSharper disable once UseAwaitUsing
        using var stream1 = File.OpenRead("sample.bmp");
        using var stream2 = new MemoryStream();
        await stream1.CopyToAsync(stream2);
        stream2.Position = 100;
        stream2.WriteByte(8);
        stream2.MoveToStart();
        stream1.MoveToStart();

        var result = await StreamComparer.AreEqual(stream1, stream2);
        Assert.False(result.IsEqual);
    }

    [Fact]
    public async Task BinaryNotEquals()
    {
        // ReSharper disable once UseAwaitUsing
        using var stream1 = File.OpenRead("sample.bmp");
        using var stream2 = new MemoryStream();
        stream2.WriteByte(8);
        stream2.MoveToStart();
        var result = await StreamComparer.AreEqual(stream1, stream2);
        Assert.False(result.IsEqual);
    }

    [Fact]
    public async Task PrefixIsNotEqual()
    {
        // received is an 8-byte-aligned prefix of verified; must not be Equal.
        using var received = new MemoryStream(new byte[16]);
        using var verified = new MemoryStream(new byte[32]);
        var result = await StreamComparer.AreEqual(received, verified);
        Assert.False(result.IsEqual);
    }

    [Fact]
    public async Task EmptyReceivedIsNotEqual()
    {
        using var received = new MemoryStream();
        using var verified = new MemoryStream(new byte[16]);
        var result = await StreamComparer.AreEqual(received, verified);
        Assert.False(result.IsEqual);
    }

    [Fact]
    public async Task LongerReceivedIsNotEqual()
    {
        using var received = new MemoryStream(new byte[32]);
        using var verified = new MemoryStream(new byte[16]);
        var result = await StreamComparer.AreEqual(received, verified);
        Assert.False(result.IsEqual);
    }

    [Fact]
    public async Task ShouldNotLock()
    {
        // ReSharper disable UseAwaitUsing
        using var stream1 = File.OpenRead("sample.bmp");
        using var stream2 = File.OpenRead("sample.bmp");
        using (
            new FileStream(
                "sample.bmp",
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
        {
            var result = await StreamComparer.AreEqual(stream1, stream2);
            Assert.True(result.IsEqual);
        }
        // ReSharper restore UseAwaitUsing
    }
}