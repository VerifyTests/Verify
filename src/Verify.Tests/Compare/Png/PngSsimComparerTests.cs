public class PngSsimComparerTests
{
    static readonly Dictionary<string, object> emptyContext = new();

    [Fact]
    public async Task Identical_Byte_For_Byte_Equal()
    {
        var png = PngTestHelper.EncodeRgba(16, 16, RandomRgba(16, 16, seed: 1));
        var result = await PngSsimComparer.Compare(new MemoryStream(png), new MemoryStream(png), emptyContext);
        Assert.True(result.IsEqual);
    }

    [Fact]
    public async Task Dimension_Mismatch_NotEqual_With_Message()
    {
        var a = PngTestHelper.EncodeRgba(10, 10, new byte[10 * 10 * 4]);
        var b = PngTestHelper.EncodeRgba(11, 10, new byte[11 * 10 * 4]);
        var result = await PngSsimComparer.Compare(new MemoryStream(a), new MemoryStream(b), emptyContext);
        Assert.False(result.IsEqual);
        Assert.Contains("dimensions differ", result.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("10x10", result.Message);
        Assert.Contains("11x10", result.Message);
    }

    [Fact]
    public async Task Near_Identical_Passes_Default_Threshold()
    {
        var original = RandomRgba(32, 32, seed: 9);
        var modified = (byte[])original.Clone();
        // Tweak a handful of pixels very slightly.
        for (var i = 0; i < 16; i += 4)
        {
            modified[i] = (byte)(modified[i] ^ 1);
        }

        var a = PngTestHelper.EncodeRgba(32, 32, original);
        var b = PngTestHelper.EncodeRgba(32, 32, modified);
        var result = await PngSsimComparer.Compare(new MemoryStream(a), new MemoryStream(b), emptyContext);
        Assert.True(result.IsEqual, result.Message);
    }

    [Fact]
    public async Task Completely_Different_Fails()
    {
        var black = new byte[32 * 32 * 4];
        var white = new byte[32 * 32 * 4];
        for (var i = 0; i < white.Length; i++)
        {
            white[i] = 255;
        }

        var a = PngTestHelper.EncodeRgba(32, 32, black);
        var b = PngTestHelper.EncodeRgba(32, 32, white);
        var result = await PngSsimComparer.Compare(new MemoryStream(a), new MemoryStream(b), emptyContext);
        Assert.False(result.IsEqual);
        Assert.Contains("SSIM", result.Message);
    }

    [Fact]
    public async Task Threshold_Tuning_Tightens_Comparison()
    {
        var a = PngTestHelper.EncodeRgba(32, 32, RandomRgba(32, 32, seed: 3));
        var modified = RandomRgba(32, 32, seed: 3);
        for (var i = 0; i < modified.Length; i += 4)
        {
            modified[i] = (byte)Math.Clamp(modified[i] + 20, 0, 255);
        }

        var b = PngTestHelper.EncodeRgba(32, 32, modified);

        var previous = PngSsimComparer.Threshold;
        try
        {
            PngSsimComparer.Threshold = 0.5;
            var lenient = await PngSsimComparer.Compare(new MemoryStream(a), new MemoryStream(b), emptyContext);
            Assert.True(lenient.IsEqual);

            PngSsimComparer.Threshold = 0.9999;
            var strict = await PngSsimComparer.Compare(new MemoryStream(a), new MemoryStream(b), emptyContext);
            Assert.False(strict.IsEqual);
        }
        finally
        {
            PngSsimComparer.Threshold = previous;
        }
    }

    [Fact]
    public async Task Corrupt_Png_Throws()
    {
        var valid = PngTestHelper.EncodeRgba(4, 4, new byte[4 * 4 * 4]);
        var corrupt = new byte[8];
        await Assert.ThrowsAnyAsync<Exception>(() =>
            PngSsimComparer.Compare(new MemoryStream(corrupt), new MemoryStream(valid), emptyContext));
    }

    static byte[] RandomRgba(int width, int height, int seed)
    {
        var rgba = new byte[width * height * 4];
        new Random(seed).NextBytes(rgba);
        for (var i = 3; i < rgba.Length; i += 4)
        {
            rgba[i] = 255;
        }

        return rgba;
    }
}
