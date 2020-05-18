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
        using var reader = new StreamReader(stream, FileHelpers.Utf8NoBOM);
        return new StringBuilder(await reader.ReadToEndAsync());
    }
}