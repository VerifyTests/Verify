#if DEBUG

public class ComparerSnippets
{
    #region InstanceComparer

    [Fact]
    public Task InstanceComparer()
    {
        var settings = new VerifySettings();
        settings.UseStreamComparer(CompareImages, "png");
        return VerifyFile("sample.png", settings);
    }

    [Fact]
    public Task InstanceComparerFluent() =>
        VerifyFile("sample.png")
            .UseStreamComparer(CompareImages, "png");

    #endregion

    #region StringComparerInstance

    [Fact]
    public Task StringComparerInstance()
    {
        var settings = new VerifySettings();
        settings.UseStringComparer(CompareIgnoringCase, "txt");
        return Verify("TheText", settings);
    }

    [Fact]
    public Task StringComparerInstanceFluent() =>
        Verify("TheText")
            .UseStringComparer(CompareIgnoringCase, "txt");

    #endregion

    #region InstanceSsimForPng

    [Fact]
    public Task InstanceSsimForPng()
    {
        var settings = new VerifySettings();
        settings.UseSsimForPng();
        return VerifyFile("sample.png", settings);
    }

    [Fact]
    public Task InstanceSsimForPngFluent() =>
        VerifyFile("sample.png")
            .UseSsimForPng(threshold: 0.995);

    #endregion

    #region SsimCompare

    public static double Score(Stream received, Stream verified) =>
        Ssim.Compare(received, verified);

    #endregion

    #region SsimCompareDimensions

    public static double? ScoreIfSameSize(Stream received, Stream verified)
    {
        var receivedImage = PngDecoder.Decode(received);
        var verifiedImage = PngDecoder.Decode(verified);
        if (receivedImage.Width != verifiedImage.Width ||
            receivedImage.Height != verifiedImage.Height)
        {
            return null;
        }

        return Ssim.Compare(receivedImage, verifiedImage);
    }

    #endregion

    public async Task StaticComparer()
    {
        #region StaticComparer

        VerifierSettings.RegisterStreamComparer(
            extension: "png",
            compare: CompareImages);
        await VerifyFile("TheImage.png");

        #endregion
    }

    public async Task StringComparerStatic()
    {
        #region StringComparerStatic

        VerifierSettings.RegisterStringComparer(
            extension: "txt",
            compare: CompareIgnoringCase);
        await Verify("TheText");

        #endregion
    }

    public async Task DefaultStringComparer()
    {
        #region DefaultStringComparer

        VerifierSettings.SetDefaultStringComparer(CompareIgnoringCase);
        await Verify("TheText");

        #endregion
    }

    #region StringComparer

    static Task<CompareResult> CompareIgnoringCase(
        string received,
        string verified,
        IReadOnlyDictionary<string, object> context)
    {
        if (string.Equals(received, verified, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(CompareResult.Equal);
        }

        var result = CompareResult.NotEqual("Differed by more than case");
        return Task.FromResult(result);
    }

    #endregion

    #region ImageComparer

    static Task<CompareResult> CompareImages(
        Stream received,
        Stream verified,
        IReadOnlyDictionary<string, object> context)
    {
        // Fake comparison
        if (received.Length == verified.Length)
        {
            return Task.FromResult(CompareResult.Equal);
        }

        var result = CompareResult.NotEqual();
        return Task.FromResult(result);
    }

    #endregion
}
#endif