#if(NETCOREAPP3_1 && DEBUG)
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class ConverterSnippets
{
    [Fact]
    public async Task Type()
    {
        #region RegisterFileConverterType
        VerifierSettings.RegisterFileConverter<Image>(
            #region ConverterCanConvert
            canConvert: target => Equals(target.RawFormat, ImageFormat.Tiff),
            #endregion
            conversion: (image, settings) =>
            {
                var pages = image.GetFrameCount(FrameDimension.Page);

                var streams = new List<Stream>();
                for (var index = 0; index < pages; index++)
                {
                    image.SelectActiveFrame(FrameDimension.Page, index);

                    var page = new MemoryStream();
                    image.Save(page, ImageFormat.Png);
                    streams.Add(page);
                }

                return new ConversionResult(
                    info: new
                    {
                        image.PixelFormat,
                        image.Size
                    },
                    streamExtension: "png",
                    streams);
            });

        #endregion
        #region FileConverterTypeVerify
        await using var stream = File.OpenRead("sample.tif");
        await Verifier.Verify(Image.FromStream(stream));
        #endregion
    }

    [Fact]
    public async Task Extension()
    {
        #region RegisterFileConverterExtension
        VerifierSettings.RegisterFileConverter(
            fromExtension: "tif",
            toExtension: "png",
            conversion: (stream, settings) =>
            {
                using Image image = Image.FromStream(stream);
                var pages = image.GetFrameCount(FrameDimension.Page);

                var streams = new List<Stream>();
                for (var index = 0; index < pages; index++)
                {
                    image.SelectActiveFrame(FrameDimension.Page, index);

                    var page = new MemoryStream();
                    image.Save(page, ImageFormat.Png);
                    streams.Add(page);
                }

                return new ConversionResult(
                    info: new
                    {
                        image.PixelFormat,
                        image.Size
                    },
                    streamExtension: "png",
                    streams);
            });

        #endregion
        #region FileConverterExtensionVerify
        await Verifier.VerifyFile("sample.tif");
        #endregion
    }
}
#endif
