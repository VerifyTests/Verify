using System.IO;

static partial class FileHelpers
{
    public static void DeleteIfEmpty(string path)
    {
        var fileInfo = new FileInfo(path);
        if (fileInfo.Exists && fileInfo.Length == 0)
        {
            fileInfo.Delete();
        }
    }

    public static string Extension(string path)
    {
        return Path.GetExtension(path).Substring(1);
    }

    public static void WriteEmptyText(string path)
    {
        File.CreateText(path).Dispose();
    }
}