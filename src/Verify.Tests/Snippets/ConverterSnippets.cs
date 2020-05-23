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
    public ConverterSnippets(ITestOutputHelper output) :
        base(output)
    {
    }

    [Fact]
    public async Task Type()
    {
        #region RegisterFileConverterType
        SharedVerifySettings.RegisterFileConverter(
            "png",
            (stream, settings) =>
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
        #region FileConverterVerify
        await VerifyFile("sample.tif");
        #endregion
    }

    [Fact]
    public async Task Extension()
    {
        #region RegisterFileConverterExtension
        SharedVerifySettings.RegisterFileConverter(
            "tif",
            "png",
            (stream, settings) =>
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
        #region FileConverterVerify
        await VerifyFile("sample.tif");
        #endregion
    }
}