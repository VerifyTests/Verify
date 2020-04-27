using System.IO;
using System.Threading.Tasks;

static class Extensions
{
    public static async Task<string> ReadString(this Stream stream)
    {
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}