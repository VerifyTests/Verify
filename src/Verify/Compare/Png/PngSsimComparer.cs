static class PngSsimComparer
{
    public static double Threshold { get; set; } = 0.98;

    internal static Task<CompareResult> Compare(Stream received, Stream verified, IReadOnlyDictionary<string, object> context) =>
        Compare(received, verified, Threshold);

    internal static StreamCompare BuildCompare(double threshold) =>
        (received, verified, _) => Compare(received, verified, threshold);

    internal static Task<CompareResult> Compare(Stream received, Stream verified, double threshold)
    {
        // Streams both images a band at a time; dimensions come from the headers, so a size
        // mismatch is reported without decoding pixels and without materializing full images.
        if (!Ssim.TryCompare(received, verified, out var score, out var receivedWidth, out var receivedHeight, out var verifiedWidth, out var verifiedHeight))
        {
            return Task.FromResult(CompareResult.NotEqual(
                $"PNG dimensions differ. Received: {receivedWidth}x{receivedHeight}, Verified: {verifiedWidth}x{verifiedHeight}"));
        }

        if (score >= threshold)
        {
            return Task.FromResult(CompareResult.Equal);
        }

        return Task.FromResult(CompareResult.NotEqual(
            $"PNG SSIM {score:F4} below threshold {threshold:F4}."));
    }
}
