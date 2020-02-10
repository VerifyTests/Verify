using System.IO;

static class Extensions
{
    public static string ReadString(this Stream stream)
    {
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}