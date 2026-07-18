namespace VerifyTests;

public static class PngDecoder
{
    public static PngImage Decode(Stream stream)
    {
        using var reader = new PngReader(stream);
        var width = reader.Width;
        var height = reader.Height;

        // The output escapes into the returned PngImage, so it is a plain allocation rather than
        // pooled. The reader's transient row scratch is what comes from the pool.
        var rgba = new byte[width * height * 4];
        var rowBytes = width * 4;
        for (var y = 0; y < height; y++)
        {
            reader.ReadRgbaRow(rgba.AsSpan(y * rowBytes, rowBytes));
        }

        return new(width, height, rgba);
    }
}
