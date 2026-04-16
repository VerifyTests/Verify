public class SsimTests
{
    [Fact]
    public void Identical_Returns_One()
    {
        var image = Random(32, 32, seed: 1);
        var score = Ssim.Compare(image, image);
        Assert.Equal(1.0, score, precision: 6);
    }

    [Fact]
    public void Tiny_Sub_Window()
    {
        // 4x4 — smaller than the 8x8 window, uses single-window path.
        var a = Solid(4, 4, 128);
        var b = Solid(4, 4, 128);
        var score = Ssim.Compare(a, b);
        Assert.Equal(1.0, score, precision: 6);
    }

    [Fact]
    public void Uniform_Gray_Identical()
    {
        var a = Solid(16, 16, 200);
        var b = Solid(16, 16, 200);
        Assert.Equal(1.0, Ssim.Compare(a, b), precision: 6);
    }

    [Fact]
    public void Small_Difference_High_Score()
    {
        var a = Gradient(64, 64);
        var b = Gradient(64, 64);
        // Perturb one pixel.
        b.Rgba[0] ^= 0x10;
        b.Rgba[1] ^= 0x10;
        b.Rgba[2] ^= 0x10;
        var score = Ssim.Compare(a, b);
        Assert.InRange(score, 0.98, 1.0);
    }

    [Fact]
    public void Large_Difference_Low_Score()
    {
        var a = Solid(32, 32, 0);
        var b = Solid(32, 32, 255);
        var score = Ssim.Compare(a, b);
        Assert.True(score < 0.5, $"Expected low score, got {score}");
    }

    [Fact]
    public void Noise_Reduces_Score()
    {
        var a = Gradient(64, 64);
        var b = Gradient(64, 64);
        var rand = new Random(7);
        for (var i = 0; i < b.Rgba.Length; i += 4)
        {
            var noise = rand.Next(-3, 4);
            b.Rgba[i] = (byte)Math.Clamp(b.Rgba[i] + noise, 0, 255);
            b.Rgba[i + 1] = (byte)Math.Clamp(b.Rgba[i + 1] + noise, 0, 255);
            b.Rgba[i + 2] = (byte)Math.Clamp(b.Rgba[i + 2] + noise, 0, 255);
        }

        var score = Ssim.Compare(a, b);
        Assert.True(score < 1.0, $"Expected score < 1.0, got {score}");
        Assert.True(score > 0.9, $"Expected mild noise to retain high score, got {score}");
    }

    static PngImage Random(int w, int h, int seed)
    {
        var rgba = new byte[w * h * 4];
        new Random(seed).NextBytes(rgba);
        return new(w, h, rgba);
    }

    static PngImage Solid(int w, int h, byte value)
    {
        var rgba = new byte[w * h * 4];
        for (var i = 0; i < w * h; i++)
        {
            rgba[i * 4] = value;
            rgba[i * 4 + 1] = value;
            rgba[i * 4 + 2] = value;
            rgba[i * 4 + 3] = 255;
        }

        return new(w, h, rgba);
    }

    static PngImage Gradient(int w, int h)
    {
        var rgba = new byte[w * h * 4];
        for (var y = 0; y < h; y++)
        {
            for (var x = 0; x < w; x++)
            {
                var o = (y * w + x) * 4;
                rgba[o] = (byte)(x * 255 / w);
                rgba[o + 1] = (byte)(y * 255 / h);
                rgba[o + 2] = (byte)((x + y) * 255 / (w + h));
                rgba[o + 3] = 255;
            }
        }

        return new(w, h, rgba);
    }
}
