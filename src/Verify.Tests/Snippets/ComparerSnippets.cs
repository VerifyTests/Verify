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
        SharedVerifySettings.RegisterComparer("png", CompareImages);
        await VerifyFile("TheImage.png");
        #endregion
    }

    #region ImageComparer
    static bool CompareImages(Stream stream1, Stream stream2)
    {
        var hash1 = HashImage(stream1);
        var hash2 = HashImage(stream2);
        var score = ImagePhash.GetCrossCorrelation(hash1, hash2);
        return score > .999;
    }

    static Digest HashImage(Stream stream)
    {
        using var bitmap = (Bitmap) Image.FromStream(stream);
        return ImagePhash.ComputeDigest(bitmap.ToLuminanceImage());
    }
    #endregion
}