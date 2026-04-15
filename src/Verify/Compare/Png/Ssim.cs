namespace VerifyTests;

static class Ssim
{
    const int windowSize = 8;
    const double k1 = 0.01;
    const double k2 = 0.03;
    const double l = 255;
    const double c1 = k1 * l * k1 * l;
    const double c2 = k2 * l * k2 * l;

    public static double Compare(PngImage a, PngImage b)
    {
        var width = a.Width;
        var height = a.Height;
        var lumA = ToLuminance(a);
        var lumB = ToLuminance(b);

        if (width < windowSize || height < windowSize)
        {
            // Single window over whole image.
            return WindowSsim(lumA, lumB, 0, 0, width, height, width);
        }

        double sum = 0;
        var count = 0;
        for (var y = 0; y <= height - windowSize; y += windowSize)
        {
            for (var x = 0; x <= width - windowSize; x += windowSize)
            {
                sum += WindowSsim(lumA, lumB, x, y, windowSize, windowSize, width);
                count++;
            }
        }

        return count == 0 ? 1 : sum / count;
    }

    static double WindowSsim(double[] a, double[] b, int x0, int y0, int w, int h, int stride)
    {
        double sumA = 0;
        double sumB = 0;
        var n = w * h;
        for (var y = 0; y < h; y++)
        {
            var row = (y0 + y) * stride + x0;
            for (var x = 0; x < w; x++)
            {
                sumA += a[row + x];
                sumB += b[row + x];
            }
        }

        var meanA = sumA / n;
        var meanB = sumB / n;

        double varA = 0;
        double varB = 0;
        double cov = 0;
        for (var y = 0; y < h; y++)
        {
            var row = (y0 + y) * stride + x0;
            for (var x = 0; x < w; x++)
            {
                var da = a[row + x] - meanA;
                var db = b[row + x] - meanB;
                varA += da * da;
                varB += db * db;
                cov += da * db;
            }
        }

        varA /= n;
        varB /= n;
        cov /= n;

        var numerator = (2 * meanA * meanB + c1) * (2 * cov + c2);
        var denominator = (meanA * meanA + meanB * meanB + c1) * (varA + varB + c2);
        return numerator / denominator;
    }

    static double[] ToLuminance(PngImage image)
    {
        var pixels = image.Width * image.Height;
        var result = new double[pixels];
        var rgba = image.Rgba;
        for (var i = 0; i < pixels; i++)
        {
            var r = rgba[i * 4];
            var g = rgba[i * 4 + 1];
            var b = rgba[i * 4 + 2];
            result[i] = 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        return result;
    }
}
