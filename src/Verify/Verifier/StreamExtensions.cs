using System.IO;
using System.Text;
using System.Threading.Tasks;

static class StreamExtensions
{
    public static void MoveToStart(this Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }
    }

    public static async Task<StringBuilder> ReadAsString(this Stream stream)
    {
        using StreamReader reader = new(stream, FileHelpers.Utf8NoBOM);
        StringBuilder builder = new(await reader.ReadToEndAsync());
        builder.FixNewlines();
        return builder;
    }
}