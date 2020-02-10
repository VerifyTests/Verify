using System.IO;
using System.Threading.Tasks;
using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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
        var percentage = CompareHash.Similarity(hash1, hash2);
        return percentage > 99;
    }

    static ulong HashImage(Stream stream)
    {
        var algorithm = new DifferenceHash();
        using var image = Image.Load<Rgba32>(stream);
        return algorithm.Hash(image);
    }
    #endregion
}