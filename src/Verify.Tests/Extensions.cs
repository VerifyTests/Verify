using System.IO;
using System.Threading.Tasks;

static class Extensions
{
    public static async Task<string> ReadString(this Stream stream)
    {
        using StreamReader reader = new(stream, FileHelpers.Utf8NoBOM);
        return await reader.ReadToEndAsync();
    }
}