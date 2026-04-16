public class PngDecoderTests
{
    [Fact]
    public void Empty_1x1_Rgba()
    {
        byte[] pixels = [10, 20, 30, 40];
        var png = PngTestHelper.EncodeRgba(1, 1, pixels);
        var image = PngDecoder.Decode(new MemoryStream(png));
        Assert.Equal(1, image.Width);
        Assert.Equal(1, image.Height);
        Assert.Equal(pixels, image.Rgba);
    }

    [Fact]
    public void Small_8x8_Rgb()
    {
        const int width = 8;
        const int height = 8;
        var rgb = new byte[width * height * 3];
        for (var i = 0; i < rgb.Length; i++)
        {
            rgb[i] = (byte)i;
        }

        var png = PngTestHelper.EncodeRgb(width, height, rgb);
        var image = PngDecoder.Decode(new MemoryStream(png));
        Assert.Equal(width, image.Width);
        Assert.Equal(height, image.Height);
        for (var i = 0; i < width * height; i++)
        {
            Assert.Equal(rgb[i * 3], image.Rgba[i * 4]);
            Assert.Equal(rgb[i * 3 + 1], image.Rgba[i * 4 + 1]);
            Assert.Equal(rgb[i * 3 + 2], image.Rgba[i * 4 + 2]);
            Assert.Equal(255, image.Rgba[i * 4 + 3]);
        }
    }

    [Fact]
    public void Small_Grayscale()
    {
        const int width = 4;
        const int height = 4;
        var gray = new byte[width * height];
        for (var i = 0; i < gray.Length; i++)
        {
            gray[i] = (byte)(i * 16);
        }

        var png = PngTestHelper.EncodeGray(width, height, gray);
        var image = PngDecoder.Decode(new MemoryStream(png));
        for (var i = 0; i < gray.Length; i++)
        {
            Assert.Equal(gray[i], image.Rgba[i * 4]);
            Assert.Equal(gray[i], image.Rgba[i * 4 + 1]);
            Assert.Equal(gray[i], image.Rgba[i * 4 + 2]);
            Assert.Equal(255, image.Rgba[i * 4 + 3]);
        }
    }

    [Fact]
    public void Paletted_With_Transparency()
    {
        const int width = 4;
        const int height = 4;
        byte[] palette =
        [
            255, 0, 0, // red
            0, 255, 0, // green
            0, 0, 255, // blue
            255, 255, 0 // yellow
        ];
        byte[] trns = [0, 128, 255, 255]; // alpha per palette entry

        var indices = new byte[width * height];
        for (var i = 0; i < indices.Length; i++)
        {
            indices[i] = (byte)(i % 4);
        }

        var png = PngTestHelper.EncodePaletted(width, height, indices, palette, trns);
        var image = PngDecoder.Decode(new MemoryStream(png));

        Assert.Equal(width, image.Width);
        Assert.Equal(height, image.Height);

        for (var i = 0; i < indices.Length; i++)
        {
            var idx = indices[i];
            Assert.Equal(palette[idx * 3], image.Rgba[i * 4]);
            Assert.Equal(palette[idx * 3 + 1], image.Rgba[i * 4 + 1]);
            Assert.Equal(palette[idx * 3 + 2], image.Rgba[i * 4 + 2]);
            Assert.Equal(trns[idx], image.Rgba[i * 4 + 3]);
        }
    }

    [Fact]
    public void Paletted_Without_Transparency_Defaults_To_Opaque()
    {
        byte[] palette = [10, 20, 30, 40, 50, 60];
        byte[] indices = [0, 1, 0, 1];
        var png = PngTestHelper.EncodePaletted(2, 2, indices, palette);
        var image = PngDecoder.Decode(new MemoryStream(png));
        for (var i = 0; i < 4; i++)
        {
            Assert.Equal(255, image.Rgba[i * 4 + 3]);
        }
    }

    [Fact]
    public void Medium_64x64()
    {
        const int width = 64;
        const int height = 64;
        var rgba = new byte[width * height * 4];
        var rand = new Random(42);
        rand.NextBytes(rgba);
        var png = PngTestHelper.EncodeRgba(width, height, rgba);
        var image = PngDecoder.Decode(new MemoryStream(png));
        Assert.Equal(rgba, image.Rgba);
    }

    [Fact]
    public void Large_256x256()
    {
        const int width = 256;
        const int height = 256;
        var rgba = new byte[width * height * 4];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var o = (y * width + x) * 4;
                rgba[o] = (byte)x;
                rgba[o + 1] = (byte)y;
                rgba[o + 2] = (byte)(x ^ y);
                rgba[o + 3] = 255;
            }
        }

        var png = PngTestHelper.EncodeRgba(width, height, rgba);
        var image = PngDecoder.Decode(new MemoryStream(png));
        Assert.Equal(width, image.Width);
        Assert.Equal(height, image.Height);
        Assert.Equal(rgba, image.Rgba);
    }

    [Fact]
    public void Rejects_Bad_Signature()
    {
        var bad = new byte[16];
        Assert.Throws<Exception>(() => PngDecoder.Decode(new MemoryStream(bad)));
    }

    [Fact]
    public void Rejects_Truncated_Stream()
    {
        var png = PngTestHelper.EncodeRgba(2, 2, new byte[16]);
        var truncated = png.Take(20).ToArray();
        Assert.ThrowsAny<Exception>(() => PngDecoder.Decode(new MemoryStream(truncated)));
    }
}
