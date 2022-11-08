#if DEBUG && NET6_0_OR_GREATER

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

            canConvert: (target, context) => Equals(target.RawFormat, ImageFormat.Tiff),

            #endregion

            conversion: (image, settings) =>
            {
                var pages = image.GetFrameCount(FrameDimension.Page);

                var targets = new List<Target>();
                for (var index = 0; index < pages; index++)
                {
                    image.SelectActiveFrame(FrameDimension.Page, index);

                    var page = new MemoryStream();
                    image.Save(page, ImageFormat.Png);
                    targets.Add(new("png", page));
                }

                return new(
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
        await Verify(Image.FromStream(stream));

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

                var targets = new List<Target>();
                for (var index = 0; index < pages; index++)
                {
                    image.SelectActiveFrame(FrameDimension.Page, index);

                    var page = new MemoryStream();
                    image.Save(page, ImageFormat.Png);
                    targets.Add(new("png", page));
                }

                return new(
                    info: new
                    {
                        image.PixelFormat,
                        image.Size
                    },
                    targets);
            });

        #endregion

        #region FileConverterExtensionVerify

        await VerifyFile("sample.tif");

        #endregion
    }
}
#endif