using System.IO;
using Bunit;
using Verify;

static class FragmentToStream
{
    public static ConversionResult Convert(IRenderedFragment fragment, VerifySettings settings)
    {
        var stream = new MemoryStream();
        using var writer = stream.BuildLeaveOpenWriter();
        writer.WriteLine(fragment.Markup);

        var instance = ComponentReader.GetInstance(fragment);

        return new ConversionResult(new FragmentInfo(instance), stream);
    }
}