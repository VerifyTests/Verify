#if DEBUG
using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class ComparerSnippets
{
    #region InstanceComparer

    [Fact]
    public Task InstanceComparer()
    {
        var settings = new VerifySettings();
        settings.UseComparer(CompareImages);
        settings.UseExtension("png");
        return Verifier.VerifyFile("sample.png", settings);
    }

    [Fact]
    public Task InstanceComparerFluent()
    {
        return Verifier.VerifyFile("sample.png")
            .UseComparer(CompareImages)
            .UseExtension("png");
    }

    #endregion

    public async Task StaticComparer()
    {
        #region StaticComparer

        VerifierSettings.RegisterComparer(
            extension: "png",
            compare: CompareImages);
        await Verifier.VerifyFile("TheImage.png");

        #endregion
    }

    #region ImageComparer

    static Task<CompareResult> CompareImages(
        VerifySettings settings,
        Stream received,
        Stream verified)
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