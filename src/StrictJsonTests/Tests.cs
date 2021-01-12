using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class Tests
{
    static Tests()
    {
        #region UseStrictJson

        VerifierSettings.UseStrictJson();

        #endregion
    }

    [Fact]
    public Task String()
    {
        return Verifier.Verify("Foo");
    }

    [Fact]
    public Task Dynamic()
    {
        return Verifier.Verify(new {value = "Foo"});
    }

    [Fact]
    public async Task Object()
    {
        #region UseStrictJsonVerify

        var target = new Target
        {
            Value = "Foo"
        };
        await Verifier.Verify(target);

        #endregion
    }

    [Fact]
    public Task WithInfo()
    {
        VerifierSettings.RegisterFileConverter(
            "bmp",
            (stream, _) =>
            {
                var info = new
                {
                    Property = "Value"
                };
                var streams = ConvertBmpTpPngStreams(stream);
                return new ConversionResult(info, streams.Select(x => new ConversionStream("png", x)));
            });
        return Verifier.Verify(FileHelpers.OpenRead("sample.bmp"))
            .UseExtension("bmp");
    }

    static IEnumerable<Stream> ConvertBmpTpPngStreams(Stream input)
    {
        Bitmap bitmap = new(input);
        MemoryStream stream = new();
        bitmap.Save(stream, ImageFormat.Png);
        yield return stream;
    }
}

public class Target
{
    public string? Value { get; set; }
}