static class PngSsimComparer
{
    public static double Threshold { get; set; } = 0.98;

    internal static Task<CompareResult> Compare(Stream received, Stream verified, IReadOnlyDictionary<string, object> context)
    {
        var receivedImage = PngDecoder.Decode(received);
        var verifiedImage = PngDecoder.Decode(verified);

        if (receivedImage.Width != verifiedImage.Width || receivedImage.Height != verifiedImage.Height)
        {
            return Task.FromResult(CompareResult.NotEqual(
                $"PNG dimensions differ. Received: {receivedImage.Width}x{receivedImage.Height}, Verified: {verifiedImage.Width}x{verifiedImage.Height}"));
        }

        var score = Ssim.Compare(receivedImage, verifiedImage);
        if (score >= Threshold)
        {
            return Task.FromResult(CompareResult.Equal);
        }

        return Task.FromResult(CompareResult.NotEqual(
            $"PNG SSIM {score:F4} below threshold {Threshold:F4}."));
    }
}
