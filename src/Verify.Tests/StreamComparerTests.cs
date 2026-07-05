public class StreamComparerTests
{
    [Fact]
    public async Task BinaryEquals()
    {
        using var stream1 = File.OpenRead("sample.bmp");
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
    }
}