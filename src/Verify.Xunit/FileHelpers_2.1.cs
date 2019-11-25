#if(NETSTANDARD2_1)
using System.IO;
using System.Threading.Tasks;

static partial class FileHelpers
{
    public static Task WriteText(string filePath, string text)
    {
        return File.WriteAllTextAsync(filePath, text);
    }

    public static Task<string> ReadText(string filePath)
    {
        return File.ReadAllTextAsync(filePath);
    }
}
#endif