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
    public async Task BinaryNotEqualsSameLength()
    {
        using var stream1 = File.OpenRead("sample.bmp");
        using var stream2 = new MemoryStream();
        await stream1.CopyToAsync(stream2);
        stream2.Position = 100;
        stream2.WriteByte(8);
        stream2.Position = 0;
        stream1.Position = 0;

        var result = await StreamComparer.AreEqual(stream1, stream2);
        Assert.False(result.IsEqual);
    }

    [Fact]
    public async Task BinaryNotEquals()
    {
        using var stream1 = File.OpenRead("sample.bmp");
        using var stream2 = new MemoryStream();
        stream2.WriteByte(8);
        stream2.Position = 0;
        var result = await StreamComparer.AreEqual(stream1, stream2);
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