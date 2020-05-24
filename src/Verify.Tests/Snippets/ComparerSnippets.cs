#if(NETCOREAPP3_1 && DEBUG)
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;
using Verify;
using VerifyXunit;
using Xunit.Abstractions;

public class ComparerSnippets :
    VerifyBase
{
    public ComparerSnippets(ITestOutputHelper output) :
        base(output)
    {
    }

    public async Task InstanceComparer()
    {
        #region InstanceComparer
        var settings = new VerifySettings();
        settings.UseComparer(CompareImages);
        settings.UseExtension("png");
        await Verify("TheImage.png", settings);
        #endregion
    }

    public async Task StaticComparer()
    {
        #region StaticComparer
        SharedVerifySettings.RegisterComparer(
            extension: "png",
            compare: CompareImages);
        await VerifyFile("TheImage.png");
        #endregion
    }

    #region ImageComparer
    static Task<CompareResult> CompareImages(
        VerifySettings settings,
        Stream received,
        Stream verified)
    {
        var hash1 = HashImage(received);
        var hash2 = HashImage(verified);
        var score = ImagePhash.GetCrossCorrelation(hash1, hash2);
        var isEqual = score > .999;
        if (isEqual)
        {
            return Task.FromResult(CompareResult.Equal);
        }

        var message = $"Score greater than .999. Received score: {score}.";
        var result = CompareResult.NotEqual(message);
        return Task.FromResult(result);
    }

    static Digest HashImage(Stream stream)
    {
        using var bitmap = (Bitmap) Image.FromStream(stream);
        return ImagePhash.ComputeDigest(bitmap.ToLuminanceImage());
    }
    #endregion
}
#endif