namespace VerifyTests;

static class Ssim
{
    const int windowSize = 8;
    const float k1 = 0.01f;
    const float k2 = 0.03f;
    const float l = 255;
    const float c1 = k1 * l * k1 * l;
    const float c2 = k2 * l * k2 * l;

    public static double Compare(PngImage a, PngImage b)
    {
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
