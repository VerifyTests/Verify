#if DEBUG

[UsesVerify]
public class ComparerSnippets
{
    #region InstanceComparer

    [Fact]
    public Task InstanceComparer()
    {
        var settings = new VerifySettings();
        settings.UseStreamComparer(CompareImages);
        settings.UseExtension("png");
        return VerifyFile("sample.png", settings);
    }

    [Fact]
    public Task InstanceComparerFluent()
    {
        return VerifyFile("sample.png")
            .UseStreamComparer(CompareImages)
            .UseExtension("png");
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