#if(NETCOREAPP3_1 && DEBUG)
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ConverterSnippets :
    VerifyBase
{
    [Fact]
    public async Task Type()
    {
        #region RegisterFileConverterType
        SharedVerifySettings.RegisterFileConverter<Image>(
            toExtension: "png",
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
                    streams);
            });

        #endregion
        #region FileConverterTypeVerify
        await using var stream = File.OpenRead("sample.tif");
        await Verify(Image.FromStream(stream));
        #endregion
    }

    [Fact]
    public async Task Extension()
    {
        #region RegisterFileConverterExtension
        SharedVerifySettings.RegisterFileConverter(
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
                    streams);
            });

        #endregion
        #region FileConverterExtensionVerify
        await VerifyFile("sample.tif");
        #endregion
    }

    public ConverterSnippets(ITestOutputHelper output) :
        base(output)
    {
    }
}
#endif
