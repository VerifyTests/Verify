#if DEBUG
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;
// ReSharper disable UnusedParameter.Local

[UsesVerify]
public class ConverterSnippets
{
    [ModuleInitializer]
    public static void RegisterFileConverterType()
    {
        #region RegisterFileConverterType

        VerifierSettings.RegisterFileConverter<Image>(
            #region ConverterCanConvert
            canConvert: (target, extension, context) => Equals(target.RawFormat, ImageFormat.Tiff),
            #endregion
            conversion: (image, settings) =>
            {
                var pages = image.GetFrameCount(FrameDimension.Page);

                List<Target> targets = new();
                for (var index = 0; index < pages; index++)
                {
                    image.SelectActiveFrame(FrameDimension.Page, index);

                    MemoryStream page = new();
                    image.Save(page, ImageFormat.Png);
                    targets.Add(new Target("png", page));
                }

                return new ConversionResult(
                    info: new
                    {
                        image.PixelFormat,
                        image.Size
                    },
                    targets);
            });
        #endregion
    }

    [Fact]
    public async Task Type()
    {
        #region FileConverterTypeVerify

        using var stream = File.OpenRead("sample.tif");
        await Verifier.Verify(Image.FromStream(stream));

        #endregion
    }

    [Fact]
    public async Task Extension()
    {
        #region RegisterFileConverterExtension

        VerifierSettings.RegisterFileConverter(
            fromExtension: "tif",
            conversion: (stream, settings) =>
            {
                using var image = Image.FromStream(stream);
                var pages = image.GetFrameCount(FrameDimension.Page);

                List<Target> targets = new();
                for (var index = 0; index < pages; index++)
                {
                    image.SelectActiveFrame(FrameDimension.Page, index);

                    MemoryStream page = new();
                    image.Save(page, ImageFormat.Png);
                    targets.Add(new Target("png", page));
                }

                return new ConversionResult(
                    info: new
                    {
                        image.PixelFormat,
                        image.Size
                    },
                    targets);
            });

        #endregion

        #region FileConverterExtensionVerify

        await Verifier.VerifyFile("sample.tif");

        #endregion
    }
}
#endif