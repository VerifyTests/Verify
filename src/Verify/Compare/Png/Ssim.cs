namespace VerifyTests;

public static class Ssim
{
    const int windowSize = 8;
    const float k1 = 0.01f;
    const float k2 = 0.03f;
    const float l = 255;
    const float c1 = k1 * l * k1 * l;
    const float c2 = k2 * l * k2 * l;

    // Streams both PNGs a band of rows at a time rather than decoding two full images, so peak
    // memory is a handful of rows per image instead of the whole RGBA buffer. Bit-identical to
    // decoding both and calling Compare(PngImage, PngImage).
    public static double Compare(Stream a, Stream b)
    {
        if (TryCompare(a, b, out var score, out var aWidth, out var aHeight, out var bWidth, out var bHeight))
        {
            return score;
        }

        throw new ArgumentException($"Image dimensions must match. a: {aWidth}x{aHeight}, b: {bWidth}x{bHeight}.");
    }

    public static double Compare(byte[] a, byte[] b) =>
        Compare(new MemoryStream(a), new MemoryStream(b));

    // Returns false (without computing a score) when the two images differ in size, so callers that
    // treat a dimension mismatch as a distinct outcome do not pay for a throw. Dimensions come from
    // the PNG headers, so a mismatch is detected before any pixel data is decoded.
    internal static bool TryCompare(Stream a, Stream b, out double score, out int aWidth, out int aHeight, out int bWidth, out int bHeight)
    {
        using var readerA = new PngReader(a);
        using var readerB = new PngReader(b);
        aWidth = readerA.Width;
        aHeight = readerA.Height;
        bWidth = readerB.Width;
        bHeight = readerB.Height;

        if (aWidth != bWidth || aHeight != bHeight)
        {
            score = 0;
            return false;
        }

        score = ComputeStreaming(readerA, readerB, aWidth, aHeight);
        return true;
    }

    static double ComputeStreaming(PngReader a, PngReader b, int width, int height)
    {
        var pool = ArrayPool<byte>.Shared;
        var rowBytes = width * 4;

        if (width < windowSize || height < windowSize)
        {
            var wholeA = pool.Rent(rowBytes * height);
            var wholeB = pool.Rent(rowBytes * height);
            try
            {
                for (var y = 0; y < height; y++)
                {
                    a.ReadRgbaRow(wholeA.AsSpan(y * rowBytes, rowBytes));
                    b.ReadRgbaRow(wholeB.AsSpan(y * rowBytes, rowBytes));
                }

                return WindowSsim(wholeA, wholeB, 0, 0, width, height, width);
            }
            finally
            {
                pool.Return(wholeA);
                pool.Return(wholeB);
            }
        }

        var bandA = pool.Rent(rowBytes * windowSize);
        var bandB = pool.Rent(rowBytes * windowSize);
        try
        {
            double sum = 0;
            var count = 0;
            for (var y = 0; y <= height - windowSize; y += windowSize)
            {
                for (var row = 0; row < windowSize; row++)
                {
                    a.ReadRgbaRow(bandA.AsSpan(row * rowBytes, rowBytes));
                    b.ReadRgbaRow(bandB.AsSpan(row * rowBytes, rowBytes));
                }

                for (var x = 0; x <= width - windowSize; x += windowSize)
                {
                    sum += WindowSsim(bandA, bandB, x, 0, windowSize, windowSize, width);
                    count++;
                }
            }

            return count == 0 ? 1 : sum / count;
        }
        finally
        {
            pool.Return(bandA);
            pool.Return(bandB);
        }
    }

    public static double Compare(PngImage a, PngImage b)
    {
        if (a.Width != b.Width ||
            a.Height != b.Height)
        {
            throw new ArgumentException($"Image dimensions must match. a: {a.Width}x{a.Height}, b: {b.Width}x{b.Height}.");
        }

        var width = a.Width;
        var height = a.Height;
        var rgbaA = a.Rgba;
        var rgbaB = b.Rgba;

        if (width < windowSize || height < windowSize)
        {
            return WindowSsim(rgbaA, rgbaB, 0, 0, width, height, width);
        }

        double sum = 0;
        var count = 0;
        for (var y = 0; y <= height - windowSize; y += windowSize)
        {
            for (var x = 0; x <= width - windowSize; x += windowSize)
            {
                sum += WindowSsim(rgbaA, rgbaB, x, y, windowSize, windowSize, width);
                count++;
            }
        }

        return count == 0 ? 1 : sum / count;
    }

    static float WindowSsim(byte[] rgbaA, byte[] rgbaB, int x0, int y0, int w, int h, int stride)
    {
        float sumA = 0;
        float sumB = 0;
        float sumAA = 0;
        float sumBB = 0;
        float sumAB = 0;
        var n = w * h;
        for (var y = 0; y < h; y++)
        {
            var row = ((y0 + y) * stride + x0) * 4;
            for (var x = 0; x < w; x++)
            {
                var offset = row + x * 4;
                var la = 0.2126f * rgbaA[offset] + 0.7152f * rgbaA[offset + 1] + 0.0722f * rgbaA[offset + 2];
                var lb = 0.2126f * rgbaB[offset] + 0.7152f * rgbaB[offset + 1] + 0.0722f * rgbaB[offset + 2];
                sumA += la;
                sumB += lb;
                sumAA += la * la;
                sumBB += lb * lb;
                sumAB += la * lb;
            }
        }

        var meanA = sumA / n;
        var meanB = sumB / n;
        var varA = sumAA / n - meanA * meanA;
        var varB = sumBB / n - meanB * meanB;
        var cov = sumAB / n - meanA * meanB;

        var numerator = (2 * meanA * meanB + c1) * (2 * cov + c2);
        var denominator = (meanA * meanA + meanB * meanB + c1) * (varA + varB + c2);
        return numerator / denominator;
    }
}
